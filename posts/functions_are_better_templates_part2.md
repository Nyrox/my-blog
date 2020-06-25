---
slug: functions-are-better-templates-part2
title: Using plain functions for HTML Templating - Part 2
description: In the [last article](https://blog.nyrox.dev/post/functions-are-better-templates) we went through how to construct a DSL from simple functions to generate HTML. In this post I want to show you how to use this DSL in practice, to replicate the features a templating language should have, we laid out in the previous article. To illustrate these techniques I am going to show some code from this very website you are reading right now.
date: 2019-12-30
---

In the [last article](https://blog.nyrox.dev/post/functions-are-better-templates) we went through how to construct a DSL from simple functions to generate HTML. In this post I want to show you how to use this DSL in practice, to replicate the features a templating language should have, we laid out in the previous article. To illustrate these techniques I am going to show some code from this very website you are reading right now.  
  
If you want to browse the code, it's public at: [https://github.com/nyrox/my-blog](https://github.com/nyrox/my-blog)

## A quick recap

If you remember, in the last article we left off at this little magic function:  
  
```fs
let element 
    (tag: string) 
    (attrs: (string * string) list) 
    (children: string list): string = 

    let attrsString = 
        attrs
            |> Seq.map (fun (n, v) -> sprintf " %s=\"%s\"" n v)
            |> String.concat ""
  
    sprintf "<%s%s>%s</%s>" tag (attrsString) (children |> String.concat "") tag
```  
  
Now that we are actually going to use this function, if you wanna follow along create a new file (in my project it's just Html.fs) and put in there all the helpers we wrote and it should look something like this:  
  
```fs
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
    let src         = attribute "src"
    let target      = attribute "target"
    ... // whatever you need

module Primitives =
    let html  = element "html"
    let head  = element "head"
    let body  = element "body"
    let div   = element "div" 
    let p     = element "p"
    let a     = element "a"
    let h1    = element "h1"
    let h2    = element "h2"
    ... // whatever you need

module Helpers =
    open Primitives
    open Attributes

    // to be filled in
```  
  
And this is all there is to it! We will not have to touch this file ever again, apart from adding new tags, attributes or helper functions. If you want the full file with all the helpers I use, grab it: [https://github.com/Nyrox/my-blog/blob/master/backend/src/Html.fs](https://github.com/Nyrox/my-blog/blob/master/backend/src/Html.fs).  
  
## Hello components

The first thing anyone wants when they look at a templating language in 2019 is obviously components. Everyone is using React or Vue nowadays and even on the server side we wanna be able to reuse code and split our templates up into nice digestable pieces. For this little example we are going to reimplement some of the components from this website.  
  
Which brings us to one of the major advantages of using F# are our templating language, which is that we can work with our domain model directly. So we are going to have a struct to contain all the data about a post, which we can use to generate components that represent that data.  
  
```fs 
type PostMeta = {
    slug: string;
    title: string;
    description: string;
    date: DateTime;
}
```  
  
Going off of this little struct, our goal is going to be to recreate the front page of [https://blog.nyrox.dev](blog.nyrox.dev). So we are going to have a list of posts representing all of the posts on the blog and we want to *transform* it into a long string of html. And if you are a seasoned functional programmer, the word *transform* should ring some bells, because we are going to use functions and pipelining to archieve this.  
  
Thus let's begin by taking one PostMeta instance and generating one entry in our list of posts, with the title, the date and a short description.  
  
### What is a component

A component at it's core is a transformation from a piece of data into the visual representation of that data. In fact the simplest way to construct a reusable component in our little world is a function with the signature `'a -> string`  where `'a` is some data and the returned string is the html for that component. That is easy enough to turn into code!  
  
```fs
// don't forget these
open Html.Primitives
open Html.Attributes

let postPreview (post: PostMeta): string =
    div [className "post-preview"] [
        ...
    ]
```  

And since our component takes the PostMeta struct as a parameter, we can just fill in the blanks with that data.
  
```fs
let postPreview (post: PostMeta): string =
    div [className "post-preview"] [
        div [className "post-header"] [
            h1 [className "title"] [post.title]
            ul [className "meta"] [
                post.date.ToString()
            ]
        ]
        p [] [post.description]
    ]
```  
  
To users of frameworks like React this should immediately look familiar, as they have recently been pushing hard towards using functional components, rather than stateful class-components.


### Composition and dealing with lists

Now comes the really fun part where we get to use our functions to do actual fun things.  
Displaying a list of things is pretty straightforward. To display a list of PostMeta entries, we just map them over our component function:
```fs
let postList posts =
    div [className "post-list"] 
        (posts
            |> Seq.map postPreview
            |> Seq.toList
        )
```  
  
Since our element function takes a list of children, this works beautifully. If you want to embed the list of components next to other elements, you can just reduce them first:
```fs
let postList posts =
    div [className "post-list"] [
        someOtherThing ()
        (posts
            |> Seq.map postPreview
            |> Seq.reduce (+)
        )
    ]
```  
  
This way, the list of postPreviews will be combined into one long string before being passed off. 

### Template Inheritance through Composition

When writing a website, it becomes obvious pretty quickly that you don't want to rewrite parts of it which are the same for every page, over and over again. Imagine this basic layout:
```fs
let home =
    html [] [
        head [] [
            stylesheet "/css/main.css"
            ... // others
        ]
    ]
    body [] [
        main [] [
            div [className "main-content"] [
                p [] ["Hello world!"]
            ]
            sidebar
        ]

        javascript_file "/some_filthy_analytics.js"
    ]
```  
  
When you now go to make another page in your application, you will quickly notice that the only part that needs to change in this layout is the main-content section. You might see where I am going with this: We just pull out the things that do change, and pass them as parameters!
```fs
let layout content =
    html [] [
        head [] [
            stylesheet "/css/main.css"
            ... // others
        ]
    ]
    body [] [
        main [] [
            div [className "main-content"] [
                content
            ]
            sidebar
        ]

        javascript_file "/some_filthy_analytics.js"
    ]

let home =
    layout (p [] "Hello world!")

let postDetail post =
    layout (renderPost post)

...
```  
  
This is how I build pages in my codebase and it is very close to how traditional templating engines model inheritance. In some sense ours is slightly more powerful though, as nothing stops us from parameterizing layouts further! For example we might want a parameter dictating whether or not to show the sidebar at all! Many frameworks can do this, but a lot of older ones would have required you to write 2 completely seperate layouts for that, which is pretty prone to errors (and also just a hassle).


### Conclusion

Should you actually do this? Maybe, maybe not. But it does come with some pretty neat benefits, primarily simplicity. I wrote the initial draft of this blog a year ago, at the time of writing this and coming back to it, I had 0 problems understanding the code and modifying it.  
  
Performance seems alright, but I haven't tested it so who knows.  Use at your own discretion, but most importantly: Have fun!