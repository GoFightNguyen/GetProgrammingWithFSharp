# Lesson 4. Saying a little, doing a lot
F# aims to give the developer a powerful, static type system with an extremely lightweight syntax.
In F#, the overall emphasis is to enable you to solve _complex_ problems with _simple_ code.

## 4.1 Binding values in F#
The `let` keyword binds values to symbols.
The value could be a primitive type to an object to a function, basically anything.
```fsharp
let age = 35
let website = System.Uri "http://fsharp.org"
let add (first, second) = first + second
```
Takeaways:
* no types - __most__ of the time you do not need to specify types.
The compiler determines type using type inference, see Lesson 5.
* no `new` keyword - F# views a constructor as a function, like any other "normal" function you might define.
The `new` keyword is optional, and generally only used when constructing objects implementing `IDisposable`
* no semicolons - optional because the compiler uses the newline to determine you've finished an expression
* no brackets for function args - F# defines function args in two ways, __tupled form__ and __curried form__.
For now, just know that when calling or defining
  * functions that take a single arg do not need round brackets
  * functions that take zero or multiple args need them, as well as commas to separate the args

### 4.1.1 `let` is not `var`
In C#, `var` declares a variable that can be modified later.
In F#, `let` binds an __immutable value__ to a symbol.

## 4.2 Scoping Values
Scoping allows us not only to show intent by explaining where and when a value is of use within a program, but also to protect us from bugs by reducing the possibilities for a value to be used within an application.
```fsharp
open System
let doStuffWithTwoNumbers(first, second) =
    let added = first + second
    Console.WriteLine("{0} + {1} = {2}", first, second, added)
    let doubled = added * 2
    doubled
```
Not sure where to put this, but here are some takeaways from this code:
* no `return` keyword - F# assumes the _final expression_ of a scope is the result of that scope.
* no accessibility modifier - `public` is the default for top-level values.
With nested scopes(described in 4.2.1), you hide values effectively without resorting to accessibility
* no static modifier - static is the default way of working in F#

### 4.2.1 Nested Scopes
Consider this code sample.
year and age are public.
But if we only care about estimatedAge, then year and age do not need to be public.
```fsharp
let year = DateTime.Now.Year
let age = year - 1979
let estimatedAge = sprintf "You are about %d years old!" age
```
We can make eliminate those symbols from being public by nesting them inside the scope of estimatedAge
```fsharp
let estimatedAge =
    let age =
        let year = DateTime.Now.Year
        year - 1979
    sprintf "You are about %d years old!" age
```

### 4.2.2 Nested functions
Recall, F# treats functions as values.
Thus, we can create functions within other functions.

In this example, calculateAge is the nested function.
```fsharp
let estimateAges(familyName, year1, year2, year3) =
    let calculateAge yearOfBirth =
        let year = System.DateTime.Now.Year
        year - yearOfBirth

    let estimatedAge1 = calculateAge year1
    let estimatedAge2 = calculateAge year2
    let estimatedAge3 = calculateAge year3

    let averageAge = (estimatedAge1 + estimatedAge2 + estimatedAge3)
    sprintf "Average age for family %s is %d" familyName averageAge
```

A nested function can access any values defined in its containing (parent) scope without having to supply them as args to the nested function.
When you return such a code block, this is known as a __closure__.