module Templates.Header

open NyroxTech.Index

open Html.Primitives
open Html.Attributes
open Html.Helpers


let headerEntry ref name label =
    a [href ref] [
        button [("id", name)] [label]
    ]

let header =
    div [className "header"] [
        headerEntry "/" "home-btn" "Home"
    ]
