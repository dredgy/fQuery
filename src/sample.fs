module sample
open fQuery
open Browser

let docReady e =
    let button = f(%"button")
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

    console.log button

let bodyClicked _ = console.log "Body Clicked"

f(%document)
    |> on "ready" "" docReady
    |> on "click" "" bodyClicked
    |> ignore