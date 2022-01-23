module fQuery


open System
open Browser.Types
open Fable.Core
open Browser.Dom
open Browser.DomExtensions
open Browser.CssExtensions
open FSharp.Core
open Thoth.Json

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

let getCss (property: string) (fquery: fQuery) =
    if fquery.Length > 0 then
        let styles = window.getComputedStyle(fquery.[0])
        Some (styles.getPropertyValue(property))
    else None


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

let value (value: string) fquery = fquery |> attr "value" value
let getValue fquery = getAttr "value" fquery

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
    if selector <> "" then
        fquery
        |> Array.filter (fun elem -> elem.matches selector)
    else fquery

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
        |> Array.map (fun elem -> elem.parentElement)
        |> isFilter selector

let children (selector: string) (fquery: fQuery) : fQuery =
    fquery
        |> Array.map(
            fun parent ->
                let childArray: HTMLElement[] = parent.children |> JS.Constructors.Array.from
                childArray
        )
        |> Array.concat
        |> Array.distinct
        |> isFilter selector


let rec getNextElement (element: HTMLElement) : HTMLElement =
    let nextSibling = element.nextSibling :?> HTMLElement
    if nextSibling = null || nextSibling.nodeType = Node.ELEMENT_NODE then
        nextSibling
    else getNextElement (nextSibling)

let rec getPrevElement (element: HTMLElement) : HTMLElement =
    let prevSibling = element.previousSibling :?> HTMLElement
    if prevSibling = null || prevSibling.nodeType = Node.ELEMENT_NODE then
        prevSibling
    else getPrevElement (prevSibling)

let private prevOrNext (direction: string) (selector: string) (fquery: fQuery) =
    let funcToUse = if direction = "next" then getNextElement else getPrevElement
    fquery
        |> Array.map funcToUse
        |> Array.filter (fun elem -> elem <> null)
        |> isFilter selector

let next (selector: string) (fquery: fQuery) = prevOrNext "next" selector fquery
let prev (selector: string) (fquery: fQuery) = prevOrNext "prev" selector fquery



let private getEventStringAlias event =
    match event with
    | "ready" -> "DOMContentLoaded"
    | _ -> event


let private eventOnTarget (e: Event) (selector: string) (callback: Event->unit) =
    let target: HTMLElement = e.target :?> HTMLElement
    if(target.matches(selector)) then
        callback e

(* Event Handling Function *)
let on (event: string) (selector: string) (callback: Event -> unit) (fquery: fQuery) =
    let events = event.Split ','

    events |> Array.iter (fun event ->
    let eventString = getEventStringAlias (event.Trim())
    fquery |> Array.iter (
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

let hasClassFilter (className: string) fquery = fquery |> isFilter("."+className)

let hasClass (className: string) fquery = (fquery |> hasClassFilter className).Length > 0


(* Data *)
type dataStorage() =
    let _storage = JS.Constructors.WeakMap.Create()

    member this.saveData (element: HTMLElement) (key: string) (value: 'a) =
        let boxedValue = box value
        if _storage.has(element) then
          let existingData : Map<string, obj> = _storage.get(element)
          if existingData.ContainsKey key then
              let oldData = existingData.Remove key
              let newData = oldData.Add(key, value)
              _storage.set(element, newData) |> ignore
          else
              let newData = existingData.Add(key, value)
              _storage.set(element, newData) |> ignore
        else _storage.set(element, Map.empty.Add(key, boxedValue)) |> ignore

    member inline this.getData<'a> (element: HTMLElement) (key: String) =
        let data =
            if _storage.has(element) then _storage.get(element)
            else Map.empty

        let value = data.TryFind key
        match value with
            | Some _ -> value |> Option.bind Option.ofObj
            | None ->
                let dataAttribute = "data-"+key

                match element.hasAttribute(dataAttribute) with
                | false -> None
                | true ->
                    let attributeValue = element.getAttribute(dataAttribute)
                    let decodedAttribute = Decode.Auto.fromString<'a>(attributeValue)
                    match decodedAttribute with
                        | Error _ -> Some (box attributeValue)
                        | Ok decodedValue -> Some (box decodedValue)


let storedData = dataStorage()

let data (key: string) (value: 'a) (fquery: fQuery) =
    fquery |> Array.iter(fun element -> storedData.saveData element key value)
    fquery

let inline getData<'a> key (fquery: fQuery): 'a Option =
    let data =
        fquery
            |> Array.map(fun element -> storedData.getData<'a> element key)
            |> Array.tryHead

    match data with
        | Some result -> unbox result
        | None -> None


let private getDisplay fquery =
    match (fquery |> (getCss "display")) with
        | Some display -> display
        | None -> "initial"

(* Hide and Show *)
let hide (fquery: fQuery) =
    let displayValue = getDisplay fquery
    fquery
    |> css "display" "none"
    |> data "previous-display" displayValue

let show (fquery: fQuery) =
    let displayValue : string =
        match (fquery |> getData "previous-display") with
         | Some display -> display
         | None -> "initial"

    fquery |> css "display" displayValue

let toggle fquery =
    let displayValue = getDisplay fquery
    match displayValue with
        | "none" -> show fquery
        | _ -> hide fquery

let toggleBool (hideOrShow: bool) fquery =
    match hideOrShow with
        | true -> show fquery
        | false -> hide fquery

