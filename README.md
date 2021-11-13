# fQuery - jQuery for Fable

<p>
	I love F#, and I use it in all my personal apps, at least on the server side. I've never used Fable for the client though,
	because of external Javascript dependencies that would be too painful to write bindings for. The common denominator, both in my personal life
	and in the enterprise, is jQuery.
</p>

<p>
	fQuery is a clone of jQuery, written in F#, and built for F# client-side apps using Fable. 
</p>

## Goals
<p>
	fQuery takes a functional first approach to jQuery, with the eventual aim of implementing all functions that are in jQuery. 
	It is aiming to provide the syntactical sugar that jQuery offers (combined with the syntactical sugar of F# pipes).
</p>

<p>
	It is <b>NOT</b> intended to provide browser compatiblity in the way that jQuery originally was. 
	I might also update some jQuery functionality to make it more consistent with modern Javascript standards.
	There will be no implementation of deprecrated jQuery functions like <b>.click()</b>.
</p>


## Current Implementation
<p>
	This project is in its infancy, but it's probably more time consuming than particularly difficult.
<p>

### Selecting Elements.

<p>In jQuery, you use the <b>$</b> function to select elements. E.g <b>$('div.className#id')</b>.</p>
<p>In fQuery, it's essentially identical, but you use the <b>f</b> function.

```f#
(* Returns all divs with the class "className" and the id "id". *)
f(%"div.className#id")
```

<p>`f()` can take either a String, an Element or the document itself.</p>

```f#
let fQueryDocument = f(%document)

let button = document.querySelector "button"
let fqueryButton = f(%buttons)

let buttons = f(%"button")
```

<p>
At this stage, f() returns a collection of HTMLElements. Originally it returned a special union type of "fQuery". Currently the fquery type is just defined as HTMLElement[]
since the item could be either a collection of elements or a Document. For now, I have a workaround but not sure if it will hold when things get more complex.
</p>

<p>Are all perfectly valid and will return an fQuery type which can be passed to other fQuery functions.</p>


### Chaining with Pipes

<p>Pipes are the whole point of F# right?</p>
<p>All fQuery functions currently implemented both take and return a value of type fQuery (an alias of HTMLElement[]). So you can chain them infinitely</p>

```f#
    let paragraphs = f(%"p")
            |> css "color" "blue"
            |> css "width" "200px"
            |> css "height" "200px"
            |> last //Changes from here will only apply to the last paragraph
            |> css "color" "red"
            |> attr "id" "woot"
            |> on "click" "" (fun e -> console.log "Hi" )
            |> addClass "testClass"
            |> removeClass "my-button"
```				

### Adding and Removing Classes
<p>Just like you would in jQuery (that will be a recurring theme), except functions are called with pipe operators rather than on an object.</p>
<p>There are 3 class functions implemented at this stage.</p>

> #### addClass (className: string)
<small>Adds a class to the selected elements</small>
```f#
let links = f(%"a[href]") |> addClass "active"
```

> #### removeClass (className: string)
<small>Removes a class from the selected elements</small>
```f#
let links = f(%"a[href]") |> removeClass "active"
```
	
> #### toggleClass (className: string)
<small>If an element already has a class, remove it. Else add it.</small>
```f#
let links = f(%"a[href]") |> toggleClass "active"
```

> #### Not yet implemented
<p>jQuery allows you to pass an array of class names to the class functions. I do not intend to overload the addClass or removeClass functions,
but will likely implement separate <b>addClasses</b> and <b>removeClasses</b> functions for handling more than one at once.
</p>


### Attributes and Style functions
<p>The functions here have the same name as the original jQuery functions.</p>

> #### attr (attribute: string) (value: string)
<small>Sets an attribute on selected elements.</small>

```f#
let links = f(%"a[href]") 
		|> attr "href" "https://github.com"
```


> #### css (property: string) (value: string)
<small>Sets a CSS property of the selected elements</small>

```f#
let links = f(%"a[href]") 
		|> css "color" "red"
		|> css "background-color" "blue"
```

> #### Not yet implemented
<p>I'm probably not aware of all of jQuery's functions, <b>prop()</b> is probably the largest and most important to implement.</p>

### Plucking from the fQuery collection

> #### first
<small>Returns the first item in an fQuery collection</small>

```f#
let firstParagraph = f(%"p") |> first
```	

> #### last
<small>Returns the last item in an fQuery collection</small>
```f#
let lastParagraph = f(%"p") |> last
```	

### Event Handlers ###
<p>
	I love the way jQuery handles event handlers, and fQuery replicates that to the best of it's ability, providing 
	near-identical syntax.
</p>
<p>
	jQuery has an overloaded definition for it's event functions, fQuery does not. It uses a single function with 3 parameters. 
	You can leave the 2nd parameter as an empty string if you don't need it, the I would recommend attaching all events to the body/document
	and using the second parameter.
</p>	

> #### on (eventName: string) (selector: string or "") (callback: function Event -> unit)
<small>
Attaches an event to the selected elements.
The first argument is the name of the event. It can also be a comma separated list of events. 
The second argument is a query selector.  If it is left as an empty string, the event will be attached directly to selected element. 
If a selector is specified, the event will only be fired if an element matching the selector is the event target. 
</small>

```f#
let docReady e = console.log e
f (%document) |> on "ready" "" docReady //fires when the document is ready

f (String "body")
    |> on "click" "" (fun _ -> console.log("Body clicked")) //Fires whenever the body is clicked
    |> on "click, mouseover" "button" (fun _ -> console.log("Just a button")) //Only fires if a button is clicked (or hovered over)
```

> #### Not Implemented yet
<b>off()</b> is a little harder to implement than I thought. I will have to see how jQuery handles it because the implementation I have is quite buggy.


### Dom Traversal Functions
<p>Lots to implement here, jQuery can do alot with the Dom!</p>

>#### find (selector: string)
<small>
Starting from the selected elements, finds all descendants that match the given selector.
</small>

```f#
f(%"div")
	|> find "span.selected"
```

>#### closest (selector: string)
<small>
Starting from the selected elements, traverses upwards to find the first ancestor that matches the selector.
</small>

```f#
f(%"span.selected")
	|> closest "div"
```	

>#### parent (selector: string)
<small>
Selects the direct parent of the selected element
</small>

```f#
f(%"span.selected")|> parent
```	
