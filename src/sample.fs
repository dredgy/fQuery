module sample
open fQuery
open Browser
open Browser.Types

let docReady (_) =

    let p = f(String "p")
                |> last
                |> css "color" "blue"
                |> css "width" "200px"
                |> css "height" "200px"
                |> attr "id" "boob"
                |> on "click" "" (fun e -> console.log "Hi" )
                |> addClass "booby"
                |> removeClass "my-button"

    console.log p


f (D document)
    |> on "ready" "" docReady
    |> on "click" "button" (fun _ -> console.log("Lol"))
    |> ignore