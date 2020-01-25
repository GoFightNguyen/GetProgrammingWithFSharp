# Lesson 40 Unit Testing in F#
## 40.1 Knowing when to unit test in F#
### 40.1.1 Unit-testing complexity
In many cases, the author suggests unit testing does not make sense in F# since the type system is so strong.
But you may want unit tests for complex rules or situations where the type system does not protect you.
Some examples:
- complex business rules, particularly with conditionals
- complex parsing
- a list must have a certain number of elements in it

Conversely, here are some cases in which you can often avoid unit testing because the compiler provides a greater degree of confidence that the code is behaving correctly:
- expressions.
F# encourages writing expressions using immutable values.
Functions taking in a value and returning another one are simple to reason about
- exhaustive pattern matching.
The F# compiler indicates if you missed cases for conditional logic.
- single-case discriminated unions.
These provide confidence you have not accidentally mixed up fields of the same type.
- option types.
No nulls, instead you deal with the notion of absence-of-value only when it's a real possibility.

### 40.1.2 Type-Driven Development
Coined by Mark Seeman, it is a kind of F# version of test-driven development.
Type-Driven Development refers to the idea that you use your types to encode business rules and make illegal states unrepresentable, thus driving development and rules through the type system, rather than through unit tests.

## 40.2 Performing basic unit testing in F#
Look at the example code using XUnit.

## 40.3 Testing DSLs in F#
You can create your own domain-specific language (DSL) on top of test libraries so your tests take advantage of F#'s language features to make tests quicker and easier to read/write.
An example:
```fsharp
// a simple wrapper around Assert.True
let isTrue (b:bool) = Assert.True b

[<Fact>]
let ``Large, young teams are correctly identified``() =
    //existing code elided...

    // use a pipeline with the native XUnit assertion library
    department |> isLargeAndYoungTeam |> Assert.True

    // use the wrapper function instead
    department |> isLargeAndYoungTeam |> isTrue
```

F# already has some ready-made DSL wrapper libraries around the various test frameworks.
Let's look at some.

### 40.3.1 FsUnit
All you to make fluent pipelines of conditions as tests.
Wrappers exist for both NUnit and xUnit.
```fsharp
open FsUnit.Xunit
[<Fact>]
let ``ex`` () =
    something |> should equal true

    something |> should be (greater than 10)

    "issac" |> should startWith "isa"
    [ 1 .. 5 ] |> should contain 3
```

### 40.3.2 Unquote
Works with both xUnit and NUnit.
Provides a way to easily assert whether the result of a comparison is true or false (to check whether two values are equal to each other).
Recall, F# types implement equality already.

This also takes advantage of F#'s quotations language feature, which is not covered in this book.
For now, just understand that F#'s quotation language feature is a way to treat a block of code as data that can be programmed against (think C# expression tree).