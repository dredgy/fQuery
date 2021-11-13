module fQuery


open Browser.Types
open Fable.Core
open Browser.Dom
open Browser.CssExtensions


let inline (~%) (x: ^A) : ^B = (^B : (static member From: ^A -> ^B) x)
let console = console
let array list = JS.Constructors.Array.from list

type fQuery =
    | Elements of HTMLElement[]
    | Doc of Document

type selector =
    | String of string
    | Element of Element
    | D of Document
    static member inline From(e: Element) = Element e
    static member inline From(s: string) = String s
    static member inline From(d: Document) = D d

let f (selector: selector) : fQuery =
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
    | D document -> Doc document

let private applyUnitFunction (elementFunc: HTMLElement -> unit) (docFunc: Document -> unit) fquery =
    match fquery with
        | Elements elements ->
            elements
                |> Array.iter elementFunc
        | Doc doc -> docFunc doc
    fquery

let get fquery=
    match fquery with
        | Elements e -> e
        | Doc doc -> [doc :?> HTMLElement] |> array

let css (property: string) (value: string) fquery =
    let documentFunc = fun _ -> ()
    let elementFunc = fun (elem: HTMLElement) -> elem.style.setProperty(property, value)
    applyUnitFunction elementFunc documentFunc fquery

let attr (attribute: string) (value: string) fquery =
    let documentFunc = fun _ -> ()
    let elementFunc = fun (elem: HTMLElement) -> elem.setAttribute(attribute, value)
    applyUnitFunction elementFunc documentFunc fquery

let text (value: string) fquery =
    let documentFunc = fun _ -> ()
    let elementFunc = fun (element: HTMLElement) -> element.innerText <- value
    applyUnitFunction elementFunc documentFunc fquery

let html (value: string) fquery =
    let documentFunc = fun _ -> ()
    let elementFunc = fun (element: HTMLElement) -> element.innerHTML <- value
    applyUnitFunction elementFunc documentFunc fquery

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
                elem.addEventListener(eventString, callback)
            else
                elem.addEventListener(eventString,  fun e -> eventOnTarget e selector callback )
       )
    fquery

(* Class Functions *)

let addClass (className: string) fquery =
    let docFunc = fun _ -> ()
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.add className
    applyUnitFunction elementFunc docFunc fquery

let removeClass (className: string) fquery =
    let docFunc = fun _ -> ()
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.remove className
    applyUnitFunction elementFunc docFunc fquery

let toggleClass (className: string) fquery =
    let docFunc = fun _ -> ()
    let elementFunc = fun (elem: HTMLElement) -> elem.classList.toggle className |> ignore
    applyUnitFunction elementFunc docFunc fquery

let first fquery =
    match fquery with
        | Elements elements ->
            [elements.[0]]
                |> array
                |> Elements
        | _ -> fquery

let last fquery =
    match fquery with
        | Elements elements ->
            [elements.[elements.Length - 1]]
                |> array
                |> Elements
        | _ -> fquery