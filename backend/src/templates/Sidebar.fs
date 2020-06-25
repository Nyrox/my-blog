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
        p [] ["Hey what is up I am Mark , a complete nerd and pizza enthusiast. I mostly write about technology and I am not afraid to use the M word: Monad. There I said it."]
        p [] ["If you are interested in what I do, check me out boi:"]

        div [className "links"] [
            p [] [a [href "https://github.com/nyrox"; target "_blank"] [badge "Github"]]
            p [] [a [href "https://twitter.com/smhmynyrox"; target "_blank"] [badge "Twitter"]]
        ]

        div [className "sidebar-end-floater"] [
            a [("id", "top-btn")] ["Back up"]
        ]

        script [] [
            "document.querySelector('#top-btn').onclick = (() => { document.body.scrollTop = 0; document.documentElement.scrollTop = 0; })"
        ]
    ]