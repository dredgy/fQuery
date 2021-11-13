﻿# fQuery - jQuery for Fable

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
	<b>It is <b>NOT</b> intended to provide browser compatible in the way that jQuery originally was. 
	I might also update some jQuery functionality to make it more consistent with modern Javascript standards.
</p>


## Current Implementation
<p>
	This project is in its infancy, but it's probably more time consuming than particularly difficult.
<p>

### Selecting Elements.

<p>In jQuery, you use the `$` function to select elements. E.g `$('div.className#id')`.</p>
<p>In fQuery, it's essentially identical, but you use the `f` function.

```
(* Returns all dives with the class "className" and the id "id". *)
f(String 'div.className#id')
```

<p>`f()` can take either a String, an Element or the document itself.</p>

```
let fQueryDocument = f(D document)

let buttons = document.querySelector "button"
let fqueryButtons = f(Element buttons)

let buttons = f(String "button")
```

<p>Are all perfectly valid and will return an fQuery type which can be passed to other fQuery functions.</p>

### Adding and Removing Classes
<p>Just like you would in jQuery (that will be a recurring theme), except functions are called with pipe operators rather than on an object.</p>
<p>There are 3 class functions implemented at this stage.</p>

#### addClass
<small>Adds a class to the selected elements</small>
```
	let links = f("a[href]") |> addClass "active"
```

#### removeClass
<small>Removes a class to the selected elements</small>
```
	let links = f("a[href]") |> removeClass "active"
```
	
#### toggleClass
<small>If an element already has a class, remove it. Else add it.</small>
```
	let links = f("a[href]") |> toggleClass "active"
```

#### Not yet implemented
<p>jQuery allows you to pass an array of class names to the class functions. I do not intend to overload the addClass or removeClass functions,
but will likely implement separate `addClasses` and `removeClasses` functions for handling more than one at once.
</p>


### Attributes and Style functions
<p>The functions here have the same name as the original jQuery functions</p>
