module sample
open fQuery
open Browser
open Browser.Types


let docReady e =
    let p = f(%"p")
                |> last
                |> css "color" "blue"
                |> css "width" "200px"
                |> css "height" "200px"
                |> attr "id" "boob"
                |> on "click" "" (fun e -> console.log "Hi" )
                |> addClass "booby"
                |> removeClass "my-button"

    console.log p

let buttonClicked _ = console.log "Hi"

f(%document)
    |> on "ready" "" docReady
    |> on "click" "button" buttonClicked
    |> ignore