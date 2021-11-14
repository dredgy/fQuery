module fQuery


open System
open Browser.Types
open Fable.Core
open Browser.Dom
open Browser.CssExtensions
open FSharp.Core

let inline (~%) (x: ^A) : ^B = (^B : (static member From: ^A -> ^B) x)

let array (list: NodeList) : HTMLElement[] = JS.Constructors.Array.from list

type fQuery = HTMLElement[]


type selector =
    | String of string
    | Element of HTMLElement
    | Document of Document
    static member inline From(e: HTMLElement) = Element e
    static member inline From(s: string) = String s
    static member inline From(d: Document) = Document d

let rec f (selector: selector) : fQuery =
    match selector with
    | String s -> document.querySelectorAll s |> array
    | Element element ->
        match element with
            | null -> [] |> JS.Constructors.Array.from
            | _ ->
                element.setAttribute ("fquery", "include")
                let elementList = document.querySelectorAll "[fquery=include]"
                element.removeAttribute "fquery"
                array elementList
    | Document _ ->
            f(%"html")


let private applyFunctionToAllElements (elementFunc: HTMLElement -> unit) (fquery: fQuery) =
    fquery
        |> FSharp.Collections.Array.iter elementFunc
    fquery

let get (fquery: fQuery) = fquery

let first (fquery: fQuery) : fQuery =
    if fquery.Length > 0 then
        [fquery.[0]] |> JS.Constructors.Array.from
    else [] |> JS.Constructors.Array.from

let last (fquery: fQuery) : fQuery =
    if fquery.Length > 0 then
        [fquery.[fquery.Length - 1]] |> JS.Constructors.Array.from
    else [] |> JS.Constructors.Array.from

let css (property: string) (value: string) fquery =
    let elementFunc = fun (elem: HTMLElement) -> elem.style.setProperty(property, value)
    applyFunctionToAllElements elementFunc fquery

let attr (attribute: string) (value: string) fquery =
     let elementFunc = fun (elem: HTMLElement) -> elem.setAttribute(attribute, value)
     applyFunctionToAllElements elementFunc fquery

let getAttr (attribute: string) fquery =
     let result =
        fquery
        |> first
        |> Array.map (fun (elem: HTMLElement) ->
            if elem.hasAttribute(attribute) then
                elem.getAttribute(attribute)
            else "")
     if result.Length > 0 then result.[0]
     else ""


let text (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerText <- value
    applyFunctionToAllElements elementFunc fquery

let getText (fquery: fQuery) =
    fquery
        |> FSharp.Collections.Array.map(fun elem -> elem.innerText)
        |> String.concat ""

let html (value: string) fquery =
    let elementFunc = fun (element: HTMLElement) -> element.innerHTML <- value
    applyFunctionToAllElements elementFunc fquery
let HTML = html

let getHtml (fquery: fQuery) =
    fquery
        |> FSharp.Collections.Array.map(fun elem -> elem.innerHTML)
        |> String.concat ""
let getHTML = getHtml

let isFilter (selector: string) (fquery: fQuery) : fQuery =
    fquery
        |> Array.filter (fun elem -> elem.matches selector)

let is (selector: string) (fquery: fQuery) =
    let matches = fquery |> isFilter selector
    matches.Length > 0

(* Dom Traversal *)
let find (selector: string) (fquery: fQuery) : fQuery =
    fquery
        |> Array.map (fun elem -> elem.querySelectorAll selector |> array)
        |> Array.concat
        |> Array.distinct

let closest (selector: string) (fquery: fQuery) : fQuery =
    fquery
        |> Array.map (fun elem -> elem.closest selector)
        |> Array.filter (fun elem -> elem.IsSome)
        |> Array.map (fun elem -> (Option.get elem) :?> HTMLElement)
        |> Array.distinct

let parent (selector: string) (fquery: fQuery) : fQuery =
    fquery
        |> Array.filter(
            fun elem ->
                if selector <> "" then
                    elem.parentElement <> null && elem.parentElement.matches selector
                else elem.parentElement <> null
            )
        |> Array.map (fun elem -> elem.parentElement)


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
    let events = event.Split ','

    events |> Array.iter (fun event ->
    let eventString = getEventStringAlias (event.Trim())
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



(* Data *)
type dataStorage() =
    let _storage = JS.Constructors.WeakMap.Create()

    member this.saveData (element: HTMLElement) (key: string) (value: 'a) =
        let boxedValue = box value
        if _storage.has(element) then
          _storage.delete(element) |> ignore
        _storage.set(element, Map.empty.Add(key, boxedValue)) |> ignore

    member this.getData (element: HTMLElement) (key: String) = _storage.get(element)

let storedData = dataStorage()

let data (key: string) (value: 'a) (fquery: fQuery) =
    fquery |> Array.iter(
        fun elem ->
            storedData.saveData elem key value
    )
    fquery

let getData<'a> key (fquery: fQuery): 'a =
    let data = fquery |> Array.map(fun elem -> storedData.getData elem key)
    unbox data.[0].[key]
