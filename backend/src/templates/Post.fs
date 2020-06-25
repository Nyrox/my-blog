module Templates.Post

open NyroxTech.Index

open Html.Primitives
open Html.Attributes
open Html.Helpers

open System
open System.IO

let scream (s: string) = 
    s 
        |> Seq.map System.Char.ToUpper
        |> System.String.Concat

let badge text =
    span [className "badge"] [text]

let month i = 
    [|
        "JAN"
        "FEB"
        "MAR"
        "APR"
        "MAY"
        "JUN"
        "JUL"
        "AUG"
        "SEP"
        "OKT"
        "NOV"
        "DEZ"
    |].[i - 1]


let formatDate (date: DateTime) =
    sprintf "%s %02i, %d" (month date.Month) date.Day date.Year

let postPreview (post: PostMeta) =
    div [className "post-preview"] [
        div [className "post-header"] [
            hyperlink ("/post/" + post.slug) [
                h1 [className "title"] [post.title]
            ]
            ul [className "meta"] [
                badge (scream (formatDate post.date))
                match post.series with
                | Some s -> span [] [
                   "Series: " + (hyperlink ("/series/" + s) <| [badge s])
                    ]
                | None -> ""
            ]
        ]
        p [] [post.description]
    ]


let postList posts =
    div [className "post-list"] 
        (posts
            |> Seq.map postPreview
            |> Seq.toList
        )



let postDetail (post: PostMeta) =
    let content = File.ReadAllText (Path.Combine [| "./.index/posts/"; post.slug |])
    
    div [className "post-header"] [
        h1 [className "title"] [post.title]
        ul [className "meta"] [
            badge (scream (formatDate post.date))
            match post.series with
            | Some s -> span [] ["Series: " + badge s]
            | None -> ""
        ]
    ] + p [] [content]
