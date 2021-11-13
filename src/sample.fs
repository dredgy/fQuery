module sample
open fQuery
open Browser


let bodyClicked _ = console.log "Body Clicked"

let docReady e =
    let x =
        f(%"p.select")
            |> parent "#test"
            |> css "background-color" "red"
            |> on "mouseover,click" "" bodyClicked

    console.log x
    ()


f(%document)
    |> on "ready" "" docReady
    |> ignore