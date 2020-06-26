[https://blog.nyrox.dev](https://blog.nyrox.dev)

# Source code for my blog

Depends only on a web server, rest is hand-written, including HTML generation!  
It uses function composition as a DSL for generating HTML, similar to Elm, at serve-time.  
  
You can read about it [here](https://blog.nyrox.dev/post/functions-are-better-templates).

### Indexer

Contained in /index. A fairly simple FSharp library that takes in a markdown file with some custom syntax and compiles it to HTML.  
The main custom syntax added is meta-data that is parsed into a key-value list.  
  
Syntax:
```md
---
slug: blog-slug
title: Blog Title
description: Long blog description...lalalala lorem ipsum blabla
date: 2020-03-20
---

...blog content
```
