module sample
open fQuery
open Browser


let bodyClicked _ = console.log "Body Clicked"

let docReady e =
    let x =
        f(%"div")
            |> getHtml

    console.log x
    ()


f(%document)
    |> on "ready" "" docReady
    |> ignore