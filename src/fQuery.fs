module fQuery


open Browser.Types
open Fable.Core
open Browser.Dom
open Browser.CssExtensions

let inline (~%) (x: ^A) : ^B = (^B : (static member From: ^A -> ^B) x)

let array list = JS.Constructors.Array.from list

type fQuery = HTMLElement[]

type selector =
    | String of string
    | Element of Element
    | Document of Document
    static member inline From(e: Element) = Element e
    static member inline From(s: string) = String s
    static member inline From(d: Document) = Document d

let rec f (selector: selector) : fQuery =
    match selector with
    | String s -> document.querySelectorAll s
                    |> JS.Constructors.Array.from
    | Element element ->
        match element with
            | null -> array []
            | _ ->
                element.setAttribute ("fquery", "include")
                let elementList = document.querySelectorAll "[fquery=include]"
                element.removeAttribute "fquery"
                JS.Constructors.Array.from elementList
    | Document _ ->
            f(%"html")

let private applyFunctionToAllElements (elementFunc: HTMLElement -> unit) (fquery: fQuery) =
    fquery
        |> Array.iter elementFunc
    fquery

let get (fquery: fQuery) = fquery

let css (property: string) (value: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.style.setProperty(property, value)
    applyFunctionToAllElements elementFunc fquery

let attr (attribute: string) (value: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.setAttribute(attribute, value)
    applyFunctionToAllElements elementFunc fquery

let text (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerText <- value
    applyFunctionToAllElements elementFunc fquery

let html (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerHTML <- value
    applyFunctionToAllElements elementFunc fquery

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
    applyFunctionToAllElements elementFunc fquery

let removeClass (className: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.remove className
    applyFunctionToAllElements elementFunc fquery

let toggleClass (className: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.toggle className |> ignore
    applyFunctionToAllElements elementFunc fquery

let first (fquery: fQuery) =
    [fquery.[0]] |> array

let last (fquery: fQuery) =
    [fquery.[fquery.Length - 1]]
        |> array