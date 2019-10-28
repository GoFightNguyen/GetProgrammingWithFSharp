# Lesson 7 Expressions and Statements
## 7.1 Comparing statements and expressions
According to C# documentation:
> Statements
>
> The actions that a program takes are expressed in statements.
> Common actions include declaring variables, assigning values, calling methods, looping through collections, and branching to one or another block of code, depending on a given condition.

> Expressions
>
> An expression is a sequence of one or more operands and zero or more operators that can be evaluated to a single value, object, method, or namespace.

The author explains it like this:
||Returns Something?|Has side-effects?|
|---|---|---|
|statements|Never|Always|
|expressions|Always|Rarely|

In other words, in C#, statements are generally for program flow and expressions are generally for returning a value. A method is generally the smallest way to create an expression in C#.

Example of statements in C#
```csharp
public void DescribeAge(int age)
{
    string ageDescription = null;
    var greeting = "Hello";
    if (age < 18)
        ageDescription = "Child!";
    else if (age < 65)
        greeting = "Adult!";
    Console.WriteLine($"{greeting}! You are a '{ageDescription}'.");
}
```

Same example but with expressions. GetText is the expression.
```csharp
private static string GetText(int age) {
    if (age < 18) return "Child!";
    else if (age < 65) return "Adult!";
    else return "OAP!";
}

public void DescribeAge(int age) {
    var ageDescription = GetText(age);
    var greeting = "Hello";
    Console.WriteLine($"{greeting}! You are a '{ageDescription}'.");
}
```

## 7.2 Using expressions in F#
F# treats expressions as a first-class element.

### 7.2.1 Working with expressions
F# encourages expressions as the default way of working.
Virtually everything in F# is an expression.
Some examples:
- There is no notion of a void function.
Every function __must__ return something.
- All program-flow branching mechanisms are expressions.
- All values are expressions.

Here is the F# version of the C# examples above:
```fsharp
open System
let describeAge age =
    let ageDescription =
        if age < 18 then "Child!"
        elif age < 65 then "Adult!"
        else "OAP!"

    let greeting = "Hello"
    Console.WriteLine("{0}! You are a '{1}'.", greeting, ageDescription)
```

Notice the if/then block is an expression, and even acts more like a function considering it has an (implicit) input and an explicit result.

### 7.2.2 Encouraging composability
A further benefit of expressions is that they encourage __composability__.

### 7.2.3 Introducing `unit`
`unit` is a type representing the absence of a specific value.
Every function in F# must return a value, even if that value is `unit`.
Every function in F# must receive at least one input, event if that input is `unit`.

Recall, there is no `void` in F#.

`unit` appears in F# to be a regular object that can be returned from any piece of code and even bound to a symbol.
But `unit` does not behave like a proper .NET object at runtime.
For example, `unit.GetHashCode()` currently throws a null reference exception.

### 7.2.4 Discarding results
F# indicates you might be doing something wrong if you call a function and do not use the return value.
This happens since F# is not a pure functional language; in the impure .NET world there are functions performing side effects such as file I/O.

`ignore` allows you to explicitly discard the result of a function call.
`ignore` takes in a value and discards it, before returning `unit`, and because F# allow syou to silently ignore expressions returning `unit`, the warning goes away.
```fsharp
let writeTextToDisk text =
    let path = System.IO.Path.GetTempFileName()
    System.IO.File.WriteAllText(path, text)
    path

let createManyFiles() =
    ignore(writeTextToDisk "The quick brown fox jumped over the lazy dog")
    ignore(writeTextToDisk "The quick brown fox jumped over the lazy dog")
    writeTextToDisk "The quick brown fox jumped over the lazy dog"

createManyFiles()
```

## 7.3 Forcing statement-based evaluation
Moving to expressions means you can no longer have unfinished if/else branches, or even early return statements from functions.
However, if you _really really_ need to work with statement-like evaluation (this should be an exception to the rule), then you can do so by ensuring the code in a given branch return `unit`.

In this example, the if/else expression has been turned into a statement.
There is no result of the conditional, just a set of side effects returning `unit`.
```fsharp
let now = System.DateTime.UtcNow.TimeOfDay.TotalHours

if now < 12.0 then Console.WriteLine "It's morning"
elif now < 18.0 then Console.WriteLine "It's afternoon"
elif now < 20.0 then ignore(5 + 5)
else ()
```