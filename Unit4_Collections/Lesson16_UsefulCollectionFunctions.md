# Lesson 16 Useful Collection Functions
## 16.1 Mapping Functions
__Mapping functions__ take a collection of items and return another collection of items.

### 16.1.1 map
`map` converts all the items in a collection from one shape to another shape, and always returns the same number of items in the output collection as were passed in.

```fsharp
let numbers = [ 1 .. 10 ]
let timesTwo n = n * 2
let output = numbers |> List.map timesTwo
```

### 16.1.2 iter
`iter` is like map, except the function you pass in must return `unit`.
Useful as an end function of a pipeline, such as saving records to a database - in effect, any function having side effects.

```fsharp
[ "Michael", 23; "Jordan", 32 ]
|> List.iter (fun(name, age) -> printfn "%O is %d" name age)
```

### 16.1.3 collect
`collect` has many other names, including `SelectMany`, `FlatMap`, `Flatten`, and even `Bind`.
It takes in a list of items, and a function returning a `new collection from each item in that collection_ - and then merges them all back into a _single_ list.

```fsharp
type Order = { OrderId : int }
type Customer = { Orders : Order list }
let customers : Customer list = []
let orders = customers |> List.collect(fun c -> c.Orders)
```

Use `collect` to resolve many-to-many relationships so you can treat all sibling children as a single concatenated list.

### 16.1.4 pairwise
Takes a list and returns a new list of _tuple pairs_ of the original adjacent items.
One situation `pairwise` is useful is for calculating the "distance" between a list of ordered items.

```fsharp
open System
[ DateTime(2010,5,1)
  DateTime(2010,6,1)
  DateTime(2010,6,12)
  DateTime(2010,7,3) ]
|> List.pairwise
|> List.map(fun (a,b) -> b - a)
|> List.map(fun time -> time.TotalDays)
```

A common variation of `pairwise` is _windowed_.
This function allows you to control how many elements exist in each window (rather than fixed at two elements).

## 16.2 Grouping functions
### 16.2.1 groupBy
The output is a collection of simple tuples.
The first element of a tuple is the key, and the second element is the collection of items in that group.
The `projection` function returns a key on which all the items in the list will be grouped.

Each version of `groupBy` ensure each grouping will be returned in the same type of collection; so groups in `Seq.groupBy` will be lazily evaluated, but `Array` and `List` will not be.

### 16.2.2 countBy
Similar to `groupBy` but instead of returning the items in the group, it returns the number of items in each group.

### 16.2.3 partition
`partition` always splits a collection into two collections.
You supply a predicate (a function returning true or false) and a collection; it returns two collections based on the predicate.
Since `partition` always returns two collections, you can safely deconstruct the output.

```fsharp
let londonCustomers, otherCustomers =
    customers |> List.partition (fun c -> c.Town = "London")
```

## 16.3 More on collections
### 16.3.1 Aggregates
All aggregate functions operate on a similar principle: take a collection of items and merge them into a smaller collection of items (often just one).
Generally, aggregate functions are the last collection function in a pipeline.

```fsharp
let numbers = [ 1.0 .. 10.0 ]
let total = numbers |> List.sum
let average = numbers |> List.average
let max = numbers |> List.max
let min = numbers |> List.min
```

All of these functions are specialized versions of a more generalized function called `fold` (in LINQ it's called `Aggregate()`).
More details in the next lesson.

### 16.3.2 Miscellaneous functions
| F# | LINQ | Comments |
| --- | --- | --- |
| find | Single() | see also findIndex, findBack, and findIndexBack |
| head | First() | |
| item | ElementAt() | get element at given index |
| take | Take() | F# take throws an exception if insufficient elements in the collection; use truncate for equivalent behavior to LINQ's Take() |
| exists | Any() | |
| forall | All() | |
| contains | contains() | |
| filter | Where() | |
| length | Count() | |
| distinct | Distinct() | |
| sortBy | OrderBy() | |

### 16.3.3 Converting between collections
Each collection module has functions to convert to and from each collection type.
These begin with `of` or `to`.

```fsharp
let numberOne =
    [ 1 .. 5 ]
    |> List.toArray
    |> Seq.ofArray
    |> Seq.head
```