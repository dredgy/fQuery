module jQuery

open System
open Browser
open Browser.Types
open Fable.Core

type jQuery =
      abstract first : unit -> jQuery
      abstract last : unit -> jQuery
      abstract css : string * string -> jQuery
      abstract attr : string -> string
      abstract attr : string * string -> jQuery
      abstract value : string -> string
      abstract value : string * string -> jQuery
      abstract text : string -> jQuery
      abstract text : unit -> string
      abstract html : string -> jQuery
      abstract html : unit -> String
      abstract is : string -> bool
      abstract find: string -> jQuery
      abstract closest : string -> jQuery
      abstract parent : unit -> jQuery
      abstract parent: string -> jQuery
      abstract children: string -> jQuery
      abstract children: unit -> jQuery
      abstract next: unit -> jQuery
      abstract next: string -> jQuery
      abstract prev: unit -> jQuery
      abstract prev: string -> jQuery
      abstract on : string * string * (Event -> unit) -> jQuery
      abstract on : string * (Event -> unit) -> jQuery
      abstract addClass : string -> jQuery
      abstract removeClass : string -> jQuery
      abstract toggleClass : string -> jQuery
      abstract data : string * string -> jQuery
      abstract data : string -> obj
      abstract hasClass : string -> bool


[<Emit("window['$']($0)")>]
let j (selector: string) : jQuery = jsNative

