module Templates.Sidebar

open NyroxTech.Index

open Html.Primitives
open Html.Attributes
open Html.Helpers

let badge = Templates.Post.badge

let sidebar =
    div [className "sidebar"] [
        div [className "author-side"] [
            img [src "images/your_humble_author.jpg"] []
            img [className "stoerer"; src "svg/stoerer.svg"] []
        ]
        p [] ["A blog about the struggles of a youngish developer with many a short attention span and an interest in too many things at once."]
        p [] ["If you are interested in what I do, here are some more things you might find interesting:"]
    
        a [href "https://github.com/nyrox"; target "_blank"] [badge "Github"]
    ]