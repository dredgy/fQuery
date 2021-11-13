﻿module sample
open fQuery
open Browser
open Browser.Types


let docReady e =
    let p = f(%"p")
                |> css "color" "blue"
                |> css "width" "200px"
                |> css "height" "200px"
                |> last
                |> css "color" "red"
                |> attr "id" "woot"
                |> on "click" "" (fun e -> console.log "Hi" )
                |> addClass "testClass"
                |> removeClass "my-button"

    console.log p

let buttonClicked _ = console.log "Hi"

f(%document)
    |> on "ready" "" docReady
    |> on "click" "button" buttonClicked
    |> ignore