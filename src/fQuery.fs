module fQuery

open System
open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable
open Browser.Dom
open Browser.Css
open Browser.CssExtensions
open Browser.Blob
open Browser.Event
open Browser.DomExtensions

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

let css (property: string) (value: string) fquery =
    match fquery with
    | Elements elements ->
        elements |> Array.iter (fun elem -> elem.style.setProperty(property, value))
        fquery
    | Doc _ -> fquery


let attr (attribute: string) (value: string) fquery =
    match fquery with
    | Elements elements ->
        elements |> Array.iter (fun elem -> elem.setAttribute(attribute, value))
        fquery
    | Doc _ -> fquery

let getEventStringAlias event =
    match event with
    | "ready" -> "DOMContentLoaded"
    | _ -> event


let eventOnTarget (e: Event) (selector: string) (callback: Event->unit) =
    let target: HTMLElement = e.target :?> HTMLElement
    if(target.matches(selector)) then
        callback e

(* Event Handling Function *)
let on (event: string) (selector: string) (callback: Event -> unit) fquery =
    let eventString = getEventStringAlias event
    match fquery with
    | Elements elements ->
        elements |> Array.iter (fun elem ->
            if selector = "" then
                elem.addEventListener(eventString, callback)
            else
                elem.addEventListener(eventString,  fun e -> eventOnTarget e selector callback )
        )
    | Doc doc ->
        if selector = "" then
            doc.addEventListener(eventString, callback)
        else
            doc.addEventListener(eventString,  fun e -> eventOnTarget e selector callback )

    fquery

(* Class Functions *)

let addClass (className: string) fquery =
    match fquery with
        | Elements elements ->
            elements
                |> Array.iter (fun elem -> elem.classList.add className)
            fquery
        | _ -> fquery

let removeClass (className: string) fquery =
    match fquery with
        | Elements elements ->
            elements
                |> Array.iter (fun elem -> elem.classList.remove className)
            fquery
        | _ -> fquery

let toggleClass (className: string) fquery =
    match fquery with
        | Elements elements ->
            elements
                |> Array.iter (fun elem -> elem.classList.toggle className |> ignore)
            fquery
        | _ -> fquery

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

