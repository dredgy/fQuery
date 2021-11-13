module sample
open fQuery
open Browser

let docReady e =
    let p = f(%"button")
                |> css "color" "blue"
                |> css "width" "200px"
                |> css "height" "200px"
                |> last
                |> text "Wooble"
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