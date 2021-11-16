module sample
open System
open fQuery
open Browser
open Fable.Core.JsInterop

let bodyClicked _ = console.log "Body Clicked"

type person = {
    name: String
}

let docReady e =
    let personData =
       f(%"body")
           |> data "person" {|name="Josh"|}
           |> getData<{|name:string|}> "person"

    console.log personData

f(%document)
    |> on "ready" "" docReady
    |> ignore