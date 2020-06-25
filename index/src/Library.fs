namespace NyroxTech.Index

[<AutoOpen>]
module Export =
    open System
    open System.IO
    open System.Text.RegularExpressions
    
    open FSharp.Markdown
    open FSharp.Json
    
    type PostMeta = {
        slug: string;
        title: string;
        description: string;
        date: DateTime;
        series: Option<string>;
    }
    
    type Index = {
        sortedPosts: PostMeta list;
        series: string list;
    }
    
    let rec parseHeader (reader: StreamReader) =
        let line = reader.ReadLine ()
    
        match Regex.IsMatch (line, "---") with
        | true -> Map.empty
        | false -> 
            let groups = (Regex.Match (line, "(\w+?):(.*)")).Groups
            let key = groups.[1].Value
            let value = groups.[2].Value.TrimStart()
            let value =
                match key with
                | "description" -> Markdown.WriteHtml <| Markdown.Parse value
                | _ -> value

            Map.add key value (parseHeader reader)
    
    
    let parseFile (file: string) =
        use reader = new StreamReader(file)
    
        if not (Regex.IsMatch (reader.ReadLine (), "---")) then
            failwith (sprintf "Error reading file: %s \nExpected first line of post to be start of header block '---'" file)
    
        let metadata = parseHeader reader
        let body = reader.ReadToEnd ()
    
        (metadata, body)
    
    
    let validateHeader (header: Map<string, string>): PostMeta =
        {
            slug= header.["slug"];
            title= header.["title"];
            description=header.["description"];
            date=DateTime.Parse(header.["date"]);
            series=header.TryFind "series"
        }
    
    
    let buildIndex root = 
        let indexDir = Path.Combine [|root; ".index/"|]
        let postsDir = Path.Combine [|root; "posts/"|]
    
        printfn "Starting indexing...\n"
        printfn "Root Directory: %s" root
        printfn "Sourcing posts from: %s" postsDir
        printfn "Writing index to: %s" indexDir
    
        Directory.CreateDirectory (Path.Combine [| indexDir; "posts/" |]) |> ignore
        Directory.EnumerateFiles postsDir
            |> Seq.map (parseFile >> (Pair.mapFirst validateHeader))
            |> Seq.map (fun (meta, body) -> 
                let html = Markdown.WriteHtml (Markdown.Parse body)
                File.WriteAllText (Path.Combine [| indexDir; "posts/"; meta.slug |], html)
                meta
            )
            |> Seq.sortByDescending (fun p -> p.date)
            |> fun posts -> 
                let series = 
                    posts 
                        |> Seq.map (fun p -> p.series)
                        |> Seq.choose id
                        |> Seq.distinct

                {
                    sortedPosts=Seq.toList posts
                    series=Seq.toList series
                }
            |> Json.serialize
            |> (fun json -> 
                File.WriteAllText (Path.Combine [| indexDir; "index.json" |], json)
            )
    