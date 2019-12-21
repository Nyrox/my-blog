module Html

let element 
    (name: string) 
    (props: (string * string) list) 
    (children: string list): string = 

    let propsString = 
        props 
            |> Seq.map (fun (n, v) -> sprintf " %s=\"%s\"" n v)
            |> String.concat ""
  
    sprintf "<%s%s>%s</%s>" name (propsString) (children |> String.concat "") name


module Attributes =
    let attribute (name: string) (value: string) = name, value
    
    let className   = attribute "class"
    let href        = attribute "href"
    let rel         = attribute "rel"
    let src         = attribute "src"
    let target      = attribute "target"

module Primitives =
    let html  = element "html"
    let head  = element "head"
    let style = element "style"
    let script = element "script"
    let link  = element "link"
    let body  = element "body"
    let div   = element "div" 
    let h1    = element "h1"
    let h2    = element "h2"
    let h3    = element "h3"
    let h4    = element "h4"
    let h5    = element "h5"
    let h6    = element "h6"
    let p     = element "p"
    let a     = element "a"
    let main  = element "main"
    let span  = element "span"
    let img   = element "img"
    let ul    = element "ul"
    let ol    = element "ol"

module Helpers =
    open Primitives
    open Attributes

    let hyperlink href' children = a [href href'] children
    let stylesheet href' = link [href href'; rel "stylesheet"] []
    let javascript_file src' = script [src src'] []