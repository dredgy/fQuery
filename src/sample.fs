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
    let x =
        f %"p.select"
            |> getData<person> "test-data"

    console.log x.Value

f(%document)
    |> on "ready" "" docReady
    |> ignore