# Lesson 15 Working with Collections
## 15.1 F# Collection basics
### 15.1.2 The collection modules
There are three modules, each tied to an associated F# collection datatype - `List`, `Array`, and `Seq` - containing functions designed for querying (and generating) collections.
Although each module is optimized for the datatype in question, they contain virtually idential surface areas; thus, you can reuse the same skills across each.

The following demonstrates the standard pattern for F# collection module functions
```fsharp
let usaCustomers = Seq.filter areFromUSA sequenceOfCustomers
let numbersDoubled = Array.map (fun number -> number * 2) arrayOfNumbers
let customersByCity = List.groupBy (fun c -> c.City) customerList
let ukCustomers = sequenceOfCustomers |> Seq.filter areFromUK
let tripledNumbers = arrayOfNumbers |> Array.map (fun number -> number * 3)
let customersByCountry = customerList |> List.groupBy (fun c -> c.Country)
```

Favor F#'s collection libraries over the standard LINQ functions.
They're designed specifically with F#'s type system in mind and usually lead to more succint and idiomatic solutions.
F# also has a `query { }` construct allowing use of `IQueryable` queries.

### 15.1.3 Transformation pipelines
The key is to think about _what_ you want to do rather than _how_.
Think of a transformation pipeline as a set of dumb workers that take in a set of data and give back a new one.

```fsharp
type FootballResult =
    { HomeTeam : string
      AwayTeam : string
      HomeGoals : int
      AwayGoals : int }
let create (ht, hg) (at, ag) =
    { HomeTeam = ht; AwayTeam = at; HomeGoals = hg; AwayGoals = ag }
let results =
    [ create ("Messiville", 1) ("Ronaldo City", 2)
      create ("Messiville", 1) ("Bale Town", 3)
      create ("Bale Town", 3) ("Ronaldo City", 1)
      create ("Bale Town", 2) ("Messiville", 1)
      create ("Ronaldo City", 4) ("Messiville", 2)
      create ("Ronaldo City", 1) ("Bale Town", 2) ]

// Which teams won the most away games?
let isAwayWin result = result.AwayGoals > result.HomeGoals

results
|> List.Filter isAwayWin
|> List.countBy(fun result -> result.AwayTeam)
|> List.sortByDescending(fun (_, awayWins) -> awayWins)
```

Interesting properties of this transformation pipeline:
- all stages are composed together with simple functions and pipelines. It is easy to add a new stage in the middle
- each operation is a pure function that is completely decoupled from the overall pipeline
- No stage affects the input collection

In F#, when inside the collections world, you can perform one or many transformations on them.
You do not have to check for null or empty because the collection functions do it for you!

### 15.1.5 Compose, compose, compose
If you find yourself writing a function that takes in a collection and _manually_ iterates over it, there's probably an existing collection function in F# to use instead.

## 15.2 Collection types in F#
Since F# uses commas to create tuples, use the semicolon to separate items in an array, list, or sequence.
Instead of using a semicolon, you can place each element on a new line.
If you use commas, you will end up with a collection containing a single tuple.

### 15.2.1 Working with sequences `seq`
Sequences are effectively an alias for the BCL `IEnumerable<T>` type.
For this lesson, consider them interchangeable with LINQ-generated sequences in that they're lazily evaluated and (by default) don't cache evaluations.
Because arrays and F# lists implement `IEnumerable<T>`, you can use functions in the `Seq` module over both of them.

Create sequences using `seq { }`

### 15.2.2 Using .NET arrays
```fsharp
let numbers = [| 1; 2; 3; 4; 6 ]  // create an array
let first = numbers.[0]
let firstThree = numbers.[0...2]  // array-slicing
numbers.[0] <- 99 // mutate the value of an item in the array
```

As with sequences, you can iterate over arrays using `for ... do`.
Since arrays are just standard BCL arrays, they're high-performance but mutable.
But you can safely rely on the `Array` module functions to create new arrays on each operation.

### 15.2.3 Immutable lists
F# lists are native to F#, these are different than the BCL `List<T>` aka `ResizeArray` in F#.
F# lists are immutable.
After you create a list, you cannot add or remove items (and if the data inside the list is immutable, it's entirely fixed).
```fsharp
let numbers = [ 1; 2; 3; 4; 5; 6]
let numbersQuick = [ 1 .. 6 ]  // shorthand form of list creation (valid on arrays and seqs too)
let head :: tail = numbers  // deconstruct a list into head(1) and a tail (2..6)
let moreNumbers = 0 :: numbers  // create a new list by pre-pending 0 to numbers
let evenMoreNumbers = moreNumbers @ [ 7 .. 9 ]  // merge moreNumbers and [7..9] to create a new list
```
Most of the functionality in the example is also achievable using the `List` module.

### Comparing and contrasting collections
| | Seq | List | Array |
| --- | --- | --- | --- |
| Eager-Lazy | lazy | eager | eager |
| forward-only | sometimes | never | never |
| immutable | yes | yes | no |
| performance | medium | med/high | high |
| pattern matching support | none | good | medium |
| interop with C# | good | medium | good |