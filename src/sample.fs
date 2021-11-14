module sample
open System
open fQuery
open Browser


let bodyClicked _ = console.log "Body Clicked"

type person = {
    name: String
}

let docReady e =
    let x =
        f(%"div")
            |> css "background-color" "red"
            |> addClass "divClass"
            |> data "testData" {name="Josh"}
            |> find "p.select"
            |> first
            |> parent ""
            |> closest ".test"
            |> css "color" "blue"
            |> attr "id" "wooble"
            |> on "click" "" (fun e -> console.log "Hi" )
            |> removeClass "testClass"
            |> text "This is the selected div"
            |> getData<person> "testData"

    console.log(x)

    let y = f(%".subTest")
                |> attr "test" "test"

    console.log y
    ()


f(%document)
    |> on "ready" "" docReady
    |> ignore