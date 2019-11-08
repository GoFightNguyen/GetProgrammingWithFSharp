# Lesson 11 Building Composable Functions

| | C# methods | F# let-bound functions |
| --- | --- | --- |
| Behavior | statements or expressions | expressions by default |
| Scope | Instance (object) or static (type) | static (module level or nested function) |
| Overloading | allowed | not supported |
| Currying | not supported | native support |

## 11.1 Partial function application
The following two functions appear to do the same thing, except one uses brackets and commas for input arguments, and one does not.
The former is referred to as _tupled_ form and the latter as _curried_ form.
```fsharp
// Tupled function. int * int -> int
let tupledAdd(a,b) = a + b
let answer = tupledAdd (5,10)

// Curried function int -> int -> int
let curriedAdd a b = a + b
let answer = curriedAdd 5 10
```
_Tupled_ functions require all arguments at once, and have a signature of (type1 * type2 ... * typeN) -> result.
F# considers all the arguments as a _single object_.

A _Curried_ function is a function that itself returns a function.
Curried functions allow you to supply only _some_ of the arguments to a function, and get back a _new function_ expecting the remaining arguments.
Curried functions have a signature of arg1 -> arg2 ... -> argN -> result.
```fsharp
let add f s = f + s // creating a function in curried form
let addFive = add 5 // partially applying "add" to get back a new function
let fifteen = addFive 10
```

Partial application is the _act_ of calling a curried function to get back a new function.

## 11.2 Constraining functions
A use for curried functions is creating a more constrained version of a function (sometimes called a wrapper function).
In this example, `buildDtThisYear` constrains one to create a date for only this year.
```fsharp
let buildDt year month day = DateTime(year, month, day)
let buildDtThisYear = buildDt 2019
```

### 11.2.1 Pipelines
Pipelines - calling methods in an ordered fashion, with the output of one method acting as the input to the next.

F# has a special operator called the _forward pipe_, it looks like `arg |> function`.
Take the value on the __left-hand side__ of the pipe, and flip it over to the __right-hand side__ as the __last__ argument to the function.
```fsharp
// using temp variables to pass data to the next method
let time =
    let directory = Directory.GetCurrentDirectory()
    Directory.GetCreationTime directory
checkCreation time

// simplistic chaining of functions. Problem is we read the code in the opposite order of operation
checkCreation(
    Directory.GetCreationTime(
        Directory.GetCurrentDirectory()))

// forward piping
Directory.GetCurrentDirectory()
|> Directory.GetCreationTime
|> checkCreation
```
The pipeline is extremely useful for composing code together into a human-readable domain-specific language (DSL).

|| C# Extension Methods | F# pipelines |
| --- | --- | --- |
| Scope | Methods must be explicitly designed to be extension methods in a static class with the extension point decorated with the this keyword | Any single-arg .NET method (including the BCL) and all curried functions can be chained together |
| Extension Point | First arg in method signature | Last arg in function |
| Currying Support | none | first class |
| Paradigm | Not always a natural fit for OO paradigm with private state | natural fit for stateless functions |

## 11.3 Composing functions together
The compose operator `>>` lets you build a _new_ function by plugging a _set_ of composable functions together.
This is useful for naming an entire pipeline, in other words turning that pipeline into a named function.
```fsharp
// Composing the pipeline we created from the previous example
let checkCurrentDirectoryAge =
    Directory.GetCurrentDirectory()
    >> Directory.GetCreationTime
    >> checkCreation
let description = checkCurrentDirectoryAge
```