# Lesson 9 Shaping Data with Tuples
Tuples are a great way to quickly pass small bits of data around when classes or similar elements feel like overkill.

## 9.2 Tuple basics
F# has native language support for tuples.
```fsharp
let parseName(name:string) =
    let parts = name.Split(' ')
    let forename = parts.[0]
    let surname = parts.[1]
    forename, surname
let name = parseName("Isaac Abraham")
let forename, surname = name    //deconstruct a tuple into meaningful values
let fname, sname = parseName("Isaac Abraham")   //deconstruct a tuple directly from a function call
```

### 9.2.1 When should I use tuples?
They're great for internal helper functions and for storing intermediary state

## 9.3 More-complex tuples
### 9.3.1 Tuple type signatures
Type notation for a three-part tuple is `type * type * type`.
Example: `string * string * int`

### 9.3.2 Nested tuples
```fsharp
let nameAndAge = ("Joe", "Bloggs"), 28
let name, age = nameAndAge
let (forename, surname), theAge = nameAndAge
```
The type signature of `nameAndAge` is (string * string) * int

### 9.3.3 Wildcards
If there are elements of a tuple you are not interested in, you can discard them during deconstruction by assigning those parts to `_`.
The underscore symbol tells the type system (and developer) that you explicitly do not want this value.
```fsharp
let nameAndAge = "Jason", 25
let name, _ = nameAndAge
```

### 9.3.4 Type inference with tuples
F# can infer tuples, even for function arguments.
F# will automatically genericize tuples within functions if a tuple element is unused within a function.
```fsharp
let addNumbers arguments =
    let a, b, c, _ = arguments
    a + b
```
The compiler can infer the types of `a` and `b` as integers.
It infers `c` and the wildcard as generics.
Therefore, the type of arguments is `int * int * 'a * 'b`.

## 9.4 Tuple best practices
### 9.4.1 Tuples and the BCL
This example shows how F# uses tuple language support to elegantly remove the need for `out` parameters.
In other words, we can replace an `out` parameter with a tuple in a single call.
`result` is the `out` parameter.
```fsharp
open System
let number = "123";
let result, parsed = Int32.TryParse(number)
```
That example also demonstrates a nicety of F# handling interoperability with C#.

### 9.4.2 When not to use tuples
If the tuple has more than 3 elements, be cautious.

The most obvious is in public contracts, particularly when different elements of the tuple are of the same type and open to misinterpretation.