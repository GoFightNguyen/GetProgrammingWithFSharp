# Lesson 5 Trusting the compiler

## 5.2 F# type-inference basics
Type-inference in F# is pervasive.

In F#, the compiler can infer the following:
- local bindings (like var in C#)
- input arguments for both built-in and custom F# types
- return types

F# uses the Hindley-Milner (HM) type system for inference.
The HM type systems _do_ impose some restrictions in order to operate that are surprising, as we will see shortly.

ex: Explicit type annotations
```fsharp
let add (a:int, b:int) : int =
    let answer:int = a + b
    answer
```
A type signature in F# has 3 main parts:
- the function name
- all input argument type(s)
- the return type

Using the example above
- we could omit the return type. F# still infers the return type to be int based on the result of the final expression
- we could then remove the type annotation from b. Again, F# infers it is an int because __implicit conversions aren't allowed in F#__. Thus, int is inferred because b is being added to an int
- we could then remove the annotations from everything. The compiler still says all the types are integers. In this case, it's because the `+` operator binds by default to integers => all values are ints

### 5.2.1 Limitations of type inference
#### Working with the BCL
Type inference works best with types __native__ to F#, meaning primitive types or types you define in F#.
It does not work so well with a type from the .NET BCL (including a C# type) - although often a single annotation will suffice.

In this example, the first `getLength` will not compile because F# does not know `name` is a string.
This can be corrected by annotating the `name` argument.
```fsharp
let getLength name = sprintf "Name is %d letters." name.Length
let getLength (name:string) = sprintf "Name is %d letters." name.Length
```
#### Classes and overloaded methods
In F#, functions declared using the `let` syntax cannot be overloaded.

### 5.2.2 Type-infrerred generics
In addition to applying type inference to values, F# also applies it for _type arguments_.
You can use either an underscore to specify a placeholder for the generic type argument, or omit the argument completely.
```fsharp
open System.Collections.Generic
let numbers = List<_>()
numbers.Add(10)
numbers.Add(20)

let otherNumbers = List()
otherNumbers.Add(10)
otherNumbers.Add(20)
```
F# infers the type based on the first available usage of the type argument.
So `numbers.Add(10)` tells the compiler that `List` is of type `int`.

F# also automatically makes functions generic when needed.
If the compiler cannot infer the type of the final expression in a function, then it will make the function generic.
```fsharp
let createList(first, second) =
    let output = List()
    output.Add(first)
    output.Add(second)
    output
// val createList : first:'a * second:'a -> List<'a>
```
Note: `'a` is the same as `T` in C#.

## 5.3 Follow the breadcrumbs
Because type inference escapes function scope in F#, unlike in C#, the compiler will go through your entire code base and notify you where the types _eventually_ clash.