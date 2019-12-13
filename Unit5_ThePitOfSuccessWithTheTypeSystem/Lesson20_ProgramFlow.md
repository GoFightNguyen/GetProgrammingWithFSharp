# Lesson 20 Program flow in F#
There is no concept of the `break` command, you cannot exit out of a loop prematurely.
To simulate a premature exit of a loop, consider replacing the loop with a sequence of values that you `filter` on (or `takeWhile`), and loop over that sequence instead.

## 20.1 loops
F# looping constructs, although officially expressions, are inherently imperative, designed to work with side effects.

### 20.1.1 `for` loops
```fsharp
// upward-counting
for number in 1 .. 10 do
    printfn "%d Hello!" number

// downward-counting
for number in 10 .. -1 .. 1 do
    printfn "%d Hello!" number

// typical for-each-style
let customerIds = [ 45 .. 99 ]
for customerId in customerIds do
    printfn "%d bought something!" customerId

// range with custom stepping; every 2nd number
for even in 2 .. 2 .. 10 do
    printfn "%d is an even number!" even
```

### 20.1.2 `while` loops
```fsharp
open System.IO
let reader = new StreamReader(File.OpenRead @"File.txt")    1
while (not reader.EndOfStream) do                           2
    printfn "%s" (reader.ReadLine())
```

### 20.1.3 comprehensions
Comprehensions generate collections of data based on `for` loop-style syntax.
```fsharp
open System

let arrayOfChars = [| for c in 'a' .. 'z' -> Char.ToUpper c |]
let listOfSquares = [ for i in 1 .. 10 -> i * i ]
let seqOfStrings = seq { for i in 2 .. 4 .. 20 -> sprintf "Number %d" i }
```

## 20.2 Branching logic in F#
Pattern matching is a construct for handling branching logic.
It is an expression-based branching mechanism allowing _inline binding_ for a wide variety of F# constructs - in other words, the ability to deconstruct a tuple or record while pattern matching.

Let's work through an example to illustrate how to improve upon if/else expressions.

### 20.2.1 Priming exercise - customer credit limits
```fsharp
// if/then expressions for complex logic
let limit =
    if score = "medium" && years = 1 then 500
    elif score = "good" && (years = 0 || years = 1) then 750
    elif score = "good" && years = 2 then 1000
    elif score = "good" then 2000
    else 250
```

### 20.2.2 say hello to pattern matching
```fsharp
let limit =
    match customer with
    | "medium", 1 -> 500
    | "good", 0 | "good", 1 -> 750
    | "good", 2 -> 1000
    | "good", _ -> 2000
    | _ -> 250
```

In pattern matching:
- you always test against a single source object
- you match against patterns representing specific cases
- you can model multiple patterns to a single, shared output
- you can model catchall-style scenarios with the wildcard pattern, even on just a subset of the overall pattern
- pattern matching is, of course, an expression, so the match returns a value; all the different branches must return the same type

### 20.2.3 Exhaustive checking
The compiler provides exhaustive checking for pattern matching.
If an input cannot be matched at runtime, F# will throw an exception.
The compiler will warn you if you do not cater to all possibilities, and tell you about rules that can never be reached.

You should always put the most specific patterns first and the most general ones last.

### 20.2.4 Guards
A `when` guard clause allows you to check within a pattern rather than just matching against values.
```fsharp
let getCreditLimit customer =
    match customer with
    | "medium", 1 -> 500
    | "good", years when years < 2 -> 750
    | ... // etc.
```

The compiler will not try to figure out anything that happens inside the guard.
In other words, it will not be able to perform exhaustive pattern matching, although it will still exhaust all possibilities that it _can_ prove.

## 20.3 Flexible pattern matching
So far, we have looked at two types of pattern matching
- constant matching
- tuple pattern matching (the ability to deconstruct and match against a tuple of values)
Now we look at a few more common types of matches

### 20.3.1 Collections
The compiler can safely extract values out of the list in one operation.
```fsharp
type Customer { Balance : int; Name : string}
let handleCustomers customers =
    match customers with
    | [] -> failwith "No customers"
    | [ customer ] -> printfn "Single customer, name is %s" customer.Name
    | [ first; second ] -> print fn "Two customers, balance = %d" (first.Balance + second.Balance)
    | customers -> printfn "Customers supplied: %d" customers.Length
```

### 20.3.2 Records
```fsharp
type Customer { Balance : int; Name : string}
let getStatus customer =
    match customer with
    | { Balance = 0 } -> "Customer has empty balance!"
    | { Name = "Isaac" } -> "This is a great customer!"
    | { Name = name; Balance = 50 } -> sprintf "%s has a large balance!" name
    | { Name = name } -> sprintf "%s is a normal customer" name
{ Balance = 50; Name = "Joe" } |> getStatus
```

Here is an example pattern matching against a list of 3 items with specific fields
```fsharp
match customers with
| [ { Name = "Tanya" }; { Balance = 25 }; _ ] -> "It's a match!"    // customers has 3 items, first is named Tanya, second has balance of 25
| _ -> "No match!"
```

## 20.4 To match or not to match
The only time it's simpler to use if/then, instaed of pattern matching, is when you're working with code that return `unit` and you're implicilty missing the default branch (else).
In this case, the compiler will create the default handler for the else branch, but the match construct always expects an explicit default handler.