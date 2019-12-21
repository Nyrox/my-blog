module Pages

open System
open System.IO

open Html.Primitives
open Html.Attributes
open Html.Helpers

open Templates.Post
open Templates.Sidebar

open FSharp.Json
open NyroxTech.Index


let blogIndex = 
    buildIndex "./"
    File.ReadAllText ".index/index.json"
        |> Json.deserialize<Index>


let baseHref () = 
    match Environment.GetEnvironmentVariable "PROD" with
    | null -> "http://localhost:8080/"
    | _ -> "https://blog.nyrox.dev/"

let layout content =
    html [] [
        head [] [
            Html.element "base" [href <| baseHref ()] []
            // Open Sans
            stylesheet "https://fonts.googleapis.com/css?family=Open+Sans:400,400i,600,600i,700,700i&display=swap"
            // Fira Code
            stylesheet "https://fonts.googleapis.com/css?family=Fira+Code&display=swap"

            stylesheet "/css/main.css"
            stylesheet "/css/syntax-highlighting/hybrid.css"
        ]
        body [] [
            main [] [
                div [className "main-content"] [
                    content
                ]
                sidebar
            ]

            javascript_file "/highlight.pack.js"
            script [] [
                "hljs.initHighlightingOnLoad();"
            ]
        ]
    ]


let index () =
    postList blogIndex.sortedPosts
        |> layout

let post postSlug =
    let post = 
        blogIndex.sortedPosts
            |> Seq.tryFind (fun b -> b.slug = postSlug)
        
    match post with
    | Some post -> Suave.Successful.OK <| layout (postDetail post)
    | None -> Suave.RequestErrors.NOT_FOUND ""

let series seriesSlug =
    let series =
        blogIndex.series
            |> Seq.tryFind (fun s -> s = seriesSlug)
    
    match series with
    | Some series -> 
        let posts =
            blogIndex.sortedPosts
                |> Seq.filter (fun p -> p.series = Some series)

        Suave.Successful.OK <| layout (postList posts)
    | None -> Suave.RequestErrors.NOT_FOUND ""
