
open System
open System.Threading
open System.IO

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful


let app =
    choose 
        [ GET >=> choose
            [ path "/" >=> request (fun _ -> OK (Pages.index ()))
              pathScan "/post/%s" (Pages.post)
              pathScan "/series/%s" (Pages.series)
              Suave.Files.browseHome
              Suave.RequestErrors.NOT_FOUND "Not found" ]
          POST >=> choose
            [ path "/hello" >=> OK "Hello Post" ]
        ]

[<EntryPoint>]
let main argv = 
    let conf = { defaultConfig with 
                    homeFolder = Some (Path.GetFullPath "./public") }

    startWebServer conf app

    0 // return an integer exit code