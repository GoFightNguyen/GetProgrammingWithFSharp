# Lesson 22 Fixing the billion-dollar mistake
## 22.2 Improving matters with the F# type system
The .NET type system isn't geared toward allowing us to easily and consistently reason about mandatory and optional data.
So let's see how F# addresses these types of data.

### 22.2.1 Mandatory data in F#
All F# types (tuples, records, and discriminated unions) behave in the same way, they are all mandatory by default.
It is illegal to assign null to any symbol that's of an F#-declared type; you cannot even assign null to a field.

Recall you cannot create a record with only some of the fields assigned, and you cannot omit fields when creating discriminated unions.
Therefore any code using only F# types cannot get a null reference exception; there is no notion of null in the type system.

Side note.
At the time of authoring the book, all three F# types were classes at runtime, and therefore reference types.
In a version after that, F# will/did introduce support to compile them to value types instead (this is really for performance and interoperability scenarios, not because of nullability issues).

### 22.2.2 The option type
F# has an __Option type__, aka Maybe in some languages.

```fsharp
let aNumber : int = 10
let maybeANumber : int option = Some 10 // creating an optional number

let calculateAnnualPremium score =
    match score with
    | Some 0 -> 250
    | Some score when score < 0 -> 400
    | Some score when score > 0 -> 150
    | None -> printfn "No score supplied, using temp premium" 300

calculateAnnualPremium (Some 250)
calculateAnnualPremium None
```

`Option` is a 2-case discriminated union: `Some (value)` or `None`.
When pattern matching, you must explicitly handle both cases.

Notice in the example, we cannot simply pass in 250.
We must wrap it as `Some 250`.

When dotting into an optional discriminated union, you see three properties: `IsSome`, `IsNone`, and `Value`.
The first two are sometimes useful, but generally you should favor pattern matching or helper functions.
`Value` will go straight to the value or throw a null reference exception if it doesn't exist.
__Don't ever use `Value`.__
Use pattern matching instead so you are forced to deal with `Some` and `None`.

## 22.3 Using the Option module
Many of the helper functions in the `Option` module take in an optional value, perform an operation, and then return another optional value.

### 22.3.1 Mapping
A higher-order function taking in an optional value and a mapping function to act on it
- does the mapping only if the value is `Some`
- if `None`, it does nothing
- the mapping function does not have to know about options; the act of checking is taken care of for you
The mapping function itself does not return a type of option.

```fsharp
// let describe be a function not designed to work with optional scores (does not specify option parameters and does not have an option return type)

// a standard match over an option
let description =
    match customer.SafetyScore with
    | Some score -> Some(describe score)
    | None -> None

// using Option.map to act on the Some case
let descriptionTwo =
    customer.SafeScore
    |> Option.map(fun score -> describe score)

// shorthand of descriptionTwo
let shorthand = customer.SafetyScore |> Option.map describe

// a new function that safely executes describe over optional values
let optionalDescribe = Option.map describe
```

All 3 expressions do the same thing: they run describe only if SafetyScore is `Some` value.

You should use `Option.map` rather than an explicit pattern match when the `None` case returns `None`.

### 22.3.2 Binding
`Option.bind` is the same as `Option.map`, except it works with mapping functions that _themselves_ return options.
This flattens/unwraps the return value from, let's say, `Option<Option<int>>` to `Option<int>`.
This could be helpful since the mapping function itself returns an option, thus you would be getting an option of an option.  

### 22.3.3 Filtering
Filter an option by using `Option.filter`.
In other words, run a predicate over an optional value.
If the value is `Some`, run the predicate.
If it passes, keep the optional value; otherwise, return `None`.

## 22.4 Collections and options
### 22.4.2 List.choose
Like a specialized combination of `map` and `filter` in one.
It allows you to apply a function that might return a value, and then automatically strip out any of the items that returned `None`.

### 22.4.3 "Try" functions
Throughout the collection modules, there are "try" functions.
These return an `Option` value: `Some` value if something was found, and `None` otherwise.