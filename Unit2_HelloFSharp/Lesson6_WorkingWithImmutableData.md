# Lesson 6 Working with Immutable Data
## 6.2 Being Explicit about Mutation
By default, values are immutable.

### 6.2.1 Mutability basics in F#
F# allows you to use mutation when needed, but it forces you to be explicit about it.

To make a value mutable, you must use the `mutable` keyword.
To change its value you must use the assignment operator `<-`.
```fsharp
let mutable name = "isaac"
name <- "kate"
```

### 6.2.2 Working with mutable objects
Do not create your own mutable types, but working with the BCL is a fact of life since F# is part of .NET.
The BCL is inherently OO-based and filled with mutable structures, so let us learn how to interact with them.
```fsharp
open System.Windows.Forms
let form = new Form()
form.Show()
form.Width <- 400
```
In this example, note that the `form` symbol is immutable, but the object it refers to is mutable.
That is why we can change the width.

F# includes a shortcut for creating mutable data structures that assigns all properties in a single action.
This shortcut is similar to object initializers in C#, expect in F# it works by making properties appear as optional constructor args.
```fsharp
open Sytem.Windows.Forms
let form = new Form(Text = "Hello", Width = 400)
form.Show()
```

## 6.3 Modeling State
Let's learn how to model data with state without resorting to mutation.

### 6.3.1 Working with mutable data
```fsharp
let mutable petrol = 100.0

let drive(distance) =
    if distance = "far" then petrol <- petrol / 2.0
    elif distance = "medium" then petrol <- petrol - 10.0
    else petrol <- petrol - 1.0

drive("far")
drive("medium")
drive("short")

petrol
```
Note these issues:
- `drive()` has no outputs.
It silently modifies the mutable `petrol` variable; you cannot know this from the type system.
- `drive()` is not deterministic.
You cannot know the behavior of this method without knowing the (hidden) state.
Calling `drive("far")` three times changes the value of `petrol` every time, depending on the previous calls
- The ordering of method calls matters. Switch the order of calls to `drive()` and you'll get a different answer

### 6.3.2 Working with immutable data
In this mode of operation, you do not mutate data.
Instead, you create _copies_ of the state with updates applied, and return that for the caller to work with; that state may be passed in to other calls that themselves generate new state.
```fsharp
let drive(petrol, distance) =
    if distance = "far" then petrol / 2.0
    elif distance = "medium" then petrol - 10.0
    else petrol - 1.0

let petrol = 100.0
let firstState = drive(petrol, "far")
let secondState = drive(firstState, "medium")
let finalState = drive(secondState, "short")

petrol
firstState
secondState
finalState
```
By "threading" the state through each function call, storing the intermediate states in values that are then manually passed to the next function call, we gain a few benefits:
- easier to reason about behavior since there are no hidden side effects and a new version of the state is returned
- `drive` is a __pure function__.
The only values that can affect the result are supplied as input arguments.
A pure function is one that varies based only on the args explicitly passed to it.
Thus, you can repeatably call it with the same args and get the same result (deterministic).
- you can see the value of each intermediate step as you work towards the final state.

### 6.3.3 Other benefits of immutable data
- Thread safety.
Because there's no shared mutable state, there is no concern for race conditions
- encapsulation isn't as important since there is no need to "hide" mutable data