module fQuery


open Browser.Types
open Fable.Core
open Browser.Dom
open Browser.CssExtensions

let inline (~%) (x: ^A) : ^B = (^B : (static member From: ^A -> ^B) x)

let array list = JS.Constructors.Array.from list

type fQuery =
    | Elements of HTMLElement[]

type selector =
    | String of string
    | Element of Element
    | D of Document
    static member inline From(e: Element) = Element e
    static member inline From(s: string) = String s
    static member inline From(d: Document) = D d

let rec f (selector: selector) : fQuery =
    match selector with
    | String s -> document.querySelectorAll s
                    |> JS.Constructors.Array.from
                    |> Elements
    | Element element ->
        match element with
            | null -> array [] |> Elements
            | _ ->
                element.setAttribute ("fquery", "include")
                let elementList = document.querySelectorAll "[fquery=include]"
                element.removeAttribute "fquery"
                elementList
                    |> JS.Constructors.Array.from
                    |> Elements
    | D _ ->
            f(%"html")

let private applyUnitFunction (elementFunc: HTMLElement -> unit) fquery =
    match fquery with
        | Elements elements ->
            elements
                |> Array.iter elementFunc
    fquery

let get fquery=
    match fquery with
        | Elements e -> e

let css (property: string) (value: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.style.setProperty(property, value)
    applyUnitFunction elementFunc fquery

let attr (attribute: string) (value: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.setAttribute(attribute, value)
    applyUnitFunction elementFunc fquery

let text (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerText <- value
    applyUnitFunction elementFunc fquery

let html (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerHTML <- value
    applyUnitFunction elementFunc fquery

let private getEventStringAlias event =
    match event with
    | "ready" -> "DOMContentLoaded"
    | _ -> event


let private eventOnTarget (e: Event) (selector: string) (callback: Event->unit) =
    let target: HTMLElement = e.target :?> HTMLElement
    if(target.matches(selector)) then
        callback e

(* Event Handling Function *)
let on (event: string) (selector: string) (callback: Event -> unit) fquery =
    let eventString = getEventStringAlias event
    let elements = get fquery

    elements |> Array.iter (
        fun elem ->
            if selector = "" then
                if event = "ready" then
                    document.addEventListener(eventString, callback)
                elem.addEventListener(eventString, callback)
            else
                elem.addEventListener(eventString,  fun e -> eventOnTarget e selector callback )
       )
    fquery

(* Class Functions *)

let addClass (className: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.add className
    applyUnitFunction elementFunc fquery

let removeClass (className: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.remove className
    applyUnitFunction elementFunc fquery

let toggleClass (className: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.toggle className |> ignore
    applyUnitFunction elementFunc fquery

let first fquery =
    match fquery with
        | Elements elements ->
            [elements.[0]]
                |> array
                |> Elements

let last fquery =
    match fquery with
        | Elements elements ->
            [elements.[elements.Length - 1]]
                |> array
                |> Elements