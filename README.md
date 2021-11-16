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

## Differences from jQuery
<p>
There are a few differences to jQuery - some are necessitated by the change of language, but
others are just to make the most of modern Javascript. jQuery was made when Javascript was
a very basic language - it didn't even have an array foreach method. So a lot of things that
jQuery were made to solve, were solved in later versions of Javascript.
</p>
<p>
fQuery acts a relatively thin wrapper between F# and modern Javascript, while providing access
to jQuery's helper functions.
</p>

### Functions vs Objects

<p>
In Javascript, jQuery return it's own type of object, that you can perform manipulations on.
</p>
<p>
In F#, <b>fQuery</b> provides a set of functions that return a collection of HTML elements.
Each of these functions also take a collection of HTML elements as an argument, so you can
almost always pass the return of one function into another, and chain functions together infinitely.
</p>
<p>
Since <b>fQuery</b> works on a collection of native HTML elements, you can also manipulate
the elements directly with modern Javascript features, which gives it even more flexibility.
</p>


### Set and Get
<p>
The other big difference between jQuery and <b>fQuery</b> is how values are setted and getted.
In jQuery, you use the same function to set and get. E.g, in jQuery:
</p>

```js
//Sets the id attribute of the first input on the page.
$('input').first().attr('id', 'myId')

//Returns the id attribute of the first input on the page.
$('input').first().attr('id')
```
<p>
In <b>fQuery</b>, set and get functions are different. This allows the
set functions to be chainable.</p>

<p>
The setter functions have the same function names as in jQuery, whereas the getter functions
are prefaced with "get". For example <b>data</b> sets data on selected elements, whereas
<b>getData</b> retrieves it. The same code as above, but in F# with <b>fQuery</b>:
</p>

```f#
//Sets the id attribute of the first input on the page.
f(%"input") |> first |> attr "id" "myId"

//Returns the id attribute of the first input on the page.
f(%"input") |> first |> getAttr "id"
```

## Examples

### Selecting Elements.

<p>In jQuery, you use the <b>$</b> function to select elements. E.g <b>$('div.className#id')</b>.</p>
<p>In fQuery, it's essentially identical, but you use the <b>f</b> function.

```f#
(* Returns all divs with the class "className" and the id "id". *)
f(%"div.className#id")
```

<p>f() can take either a String, an Element or the document itself.</p>

```f#
let fQueryDocument = f(%document)

let button = document.querySelector "button"
let fqueryButton = f(%button)

let buttons = f(%"button")
```
<p>Are all perfectly valid and will return an fQuery type which can be passed to other fQuery functions.</p>
<p>
At this stage, f() returns a collection of HTMLElements. Originally it returned a special union type of "fQuery". Since the item could be either a collection of elements or a Document. For now, I have a workaround that means I don't have to handle cases where document is the selected node. I'm not sure if it will hold when things get more complex but in the meantime the fQuery type is simply defined as an alias of HTMLElement[]
</p>


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
### Element Manipulation
<p>Just like you would in jQuery (that will be a recurring theme), except functions are called with pipe operators rather than on an object.</p>

>> #### addClass (className: string) : fQuery
><small>Adds a class to the selected elements</small>
```f#
let links = f(%"a[href]") |> addClass "active"
```

>> #### removeClass (className: string) : fQuery
><small>Removes a class from the selected elements</small>
```f#
let links = f(%"a[href]") |> removeClass "active"
```

>> #### toggleClass (className: string) : fQuery
><small>If an element already has a class, remove it. Else add it.</small>
```f#
let links = f(%"a[href]") |> toggleClass "active"
```

>> #### attr (attribute: string) (value: string) : fQuery
><small>Sets an attribute on selected elements.</small>

```f#
let links = f(%"a[href]")
		|> attr "href" "https://github.com"
```


>> #### css (property: string) (value: string) : fQuery
><small>Sets a CSS property of the selected elements</small>

```f#
let links = f(%"a[href]")
		|> css "color" "red"
		|> css "background-color" "blue"
```

>> #### text (value: string) : fQuery
><small>
>Sets the text value of selected elements.
>Unlike the jQuery function which is used to both get
>and set text, this function can only set text so that it can
>be used in pipes. To get the text from selected elements use
>getText instead.
></small>

```f#
let headings = f(%"h1")
                |> css "font-size" "3em"
                |> text "This is a heading"
                |> addClass "heading"
```

>> #### getText : string
><small>
>    Retrieves the text value of the selected elements.
>    Returns a string.
></small>

```f#
let heading = f(%"h1:first-of-type") |> getText
```

>> #### html (value: string) : fQuery
><small>
>Sets the inner HTML value of selected elements.
>Unlike the jQuery function which is used to both get
>and set html, this function can only set, so that it can
>be used in pipes. To get the inner html from selected elements use
>getHTML instead.
></small>

```f#
let headings = f(%"h1")
                |> css "font-size" "3em"
                |> html "<small>This is a heading</small>"
```

>> #### getHtml : string
><small>
>    Retrieves the inner html value of the selected elements.
>Returns a string.
></small>
```f#
let heading = f(%"h1:fi**rst-of-type") |> getHtml
```

>>#### is (selector: string) : boolean
> Checks to see if at least one of the selected elements
> matches the provided selector. <b>is</b> breaks the pipeline
> as it returns a boolean.
```f#
let paragraphs = $("%p") |> is "div"
```

>>#### isFilter (selector: string) : fQuery
> Filters the selected elements to those that match the selector
> Unlike <b>is</b>, isFilter does not break the pipeline, so you
> can continue passing the result to other fQuery functions.
```f#
    let paragraphs = $("%p") |> is ".className"
```

>> #### Not yet implemented
><p>I'm probably not aware of all of jQuery's functions, <b>prop()</b> is probably the largest and most important to implement.</p>

### Plucking from the fQuery collection

>> #### first : fQuery
><small>Gives the first item in an fQuery collection</small>

```f#
let firstParagraph = f(%"p") |> first
```

>> #### last : fQuery
><small>Gives the last item in an fQuery collection</small>
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
	You can leave the 2nd parameter as an empty string if you don't need it, though I would recommend attaching all events to the body/document
	and using the second parameter.
</p>

>> #### on (eventName: string) (selector: string or "") (callback: function Event -> unit) : fQuery
><small>
>Attaches an event to the selected elements.
>The first argument is the name of the event. It can also be a comma separated list of events.
>The second argument is a query selector.  If it is left as an empty string, the event will be attached directly to selected element.
>If a selector is specified, the event will only be fired if an element matching the selector is the event target.
></small>

```f#
let docReady e = console.log e
f (%document) |> on "ready" "" docReady //fires when the document is ready

f (%"body")
    |> on "click" "" (fun _ -> console.log("Body clicked")) //Fires whenever the body is clicked
    |> on "click, mouseover" "button" (fun _ -> console.log("Just a button")) //Only fires if a button is clicked (or hovered over)
```

> #### Not Implemented yet
<b>off()</b> is a little harder to implement than I thought. I will have to see how jQuery handles it because the implementation I have is quite buggy.

### Data
<p>Setting data is done just like in Javascript.</p>

>> #### data (key: string) (value: any) : fQuery
> Attaches data to all of the selected elements. This data can be of any type.

```f#
f(%"body") |> data "person" {|name="Josh"|}
```
>> ### getData&lt;type&gt; (key: string) : Option&lt;type&gt;
> Returns an F# option type of the specified data. If the data has not been set previously, then
> <b>fQuery</b> will check the element to see if has a matching data attribute. The type must be explicitly stated.
>
 ```f#
 let personData =
    f(%"body")
        |> data "person" {|name="Josh"|}
        |> getData<{name:string}> "person"

  console.log personData.Value
```

### Dom Traversal Functions
<p>Lots to implement here, jQuery can do alot with the Dom!</p>

>>#### find (selector: string) : fQuery
><small>
>Starting from the selected elements, finds all descendants that match the given selector.
></small>

```f#
f(%"div")
    |> find "span.selected"
```

>>#### closest (selector: string) : fQuery
><small>
>Starting from the selected elements, traverses upwards to find the first ancestor that matches the selector.
></small>

```f#
f(%"span.selected")
	|> closest "div"
```

>>#### parent (selector: string) : fQuery
><small>
>Selects the direct parent of the selected element.
>Selector can be either an empty string "" or a query selector.
>If a selector is passed in, then if the parent does not match that selector it will be filtered out.
></small>

```f#
f(%"span.selected")|> parent ""
```
