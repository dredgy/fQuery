module sample
open System
open fQuery
open jQuery
open Browser
open Fable.Core.JsInterop

let bodyClicked _ = console.log "Body Clicked"

type person = {
    name: String
}

let docReady _ =

    let personData =
       f(%"div")
           |> next ""
           |> prev ""
           |> children ""
           |> children ""
           |> first
           |> css "display" "block"
           |> css "height" "200px"
           |> css "width" "200px"
           |> css "display" "flex"
           |> css "background-color" "black"
           |> css "color" "white"
           |> hide
           |> toggle

    console.log personData

f(%document)
    |> on "ready" "" docReady
    |> ignore