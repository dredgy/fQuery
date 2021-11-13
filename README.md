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
f('div.className#id')
```
