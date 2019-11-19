# Lesson 17 Maps, Dictionaries, and Sets
## 17.1 Dictionaries
### 17.1.1 Mutable dictionaries
This is the `System.Collections.Generic.Dictionary` we are used to from C#.
You can use F# syntax and get generic type inference by doing `let inventory = Dictionary<_,_>()`.

### 17.1.2 Immutable dictionaries
`dict` is a helper function to create an immutable `IDictionary`.
Since the return object is immutable, you cannot add/remove items from it.
Instead, you supply it up front with a sequence of tuples representing the key/value pairs, which then become the fixed contents of the dictionary.

```fsharp
open System.Collections.Generic
let inventory : IDictionary<string, float> =
    [ "Apples", 0.33; "Oranges", 0.23; "Bananas", 0.45 ]
    |> dict

inventory.Add("Pineapples", 0.85)   // throws exception
```

This is useful for creating a lookup and never modifying it again.
But it sucks because `IDictionary` still includes methods like Add despite `dict` making the type immutable.
The solution is to use `Map` type.

## 17.2 The F# Map
`Map` is an immutable key/value lookup.
Like `dict`, it allows quick creation of a lookup based on a sequence of tuples, but unlike `dict`, it allows adding or removing items only by using a similar mechanism to modifying records or lists.
Entries from the existing `Map` are copied to a new `Map`, and then the item is added/removed.

```fsharp
let inventory =
    [ "Apples", 0.33; "Oranges", 0.23; "Bananas", 0.45 ]
    |> Map.ofList

let newInventory =
    inventory
    |> Map.add "Pineapples" 0.87
    |> Map.remove "Apples"
```

Useful because you get immutability _and_ the ability to add/remove items.

Calling `add` on a `Map` that already contains the key replaces the old value with the new one as it creates the new `Map`.
The original `Map` still retains the original value.

### 17.2.1 Useful Map functions
The `Map` module includes functions allowing you to treat maps as though they were enumerable collections, using the same chained pipelines such as:
- `map`
- `filter`
- `iter`
- `partition`
An important note is that these `Map` higher-order functions take in both the key and the value for each element in the map, whereas `List`, for example, takes in only the value.

```fsharp
let cheapFruit, expensiveFruit =
    inventory
    |> Map.partition (fun fruit cost -> cost < 0.3)
```
Note that key and value are passed as curried functions.

When to use Dictionary, dict, or Map?
- use `Map` as the default
- use the `dict` function to generate an `IDictionary` needed for interoperability with other BCL code
- use the BCL `Dictionary` if you need mutability, or have specific performance requirements

## 17.3 Sets
`Set` implements a standard mathematical set of data.
In math, a set is a collection of distinct objects.
Thus, `Set` cannot contain duplicates and automatically removes repeated items.

```fsharp
let myBasket = [ "Apples"; "Apples"; "Bananas"; "Pineapples" ]
let fruitsILike = myBasket |> Set.ofList
```

Sets in F# are especially useful for performing set-based operations on two sets.
The `Set` module includes operator overloads for addition/subtraction, which internally redirects to `Set.union` and `Set.difference`.
Sets work with any type supporting comparison, which F# records and tuples do by default!

```fsharp
// comparing List and Set based operation
let yourBasket = [ "Kiwi"; "Bananas"; "Grapes" ]
let allFruitsList = (fruits @ otherFruits) |> List.distinct
let fruitsYouLike = yourBasket |> Set.ofList
let allFruits = fruitsILike + fruitsYouLike
```

```fsharp
// some Set-based operations
let fruitsJustForMe = allFruits - fruitsYouLike
let fruitsWeCanShare = fruitsILike |> Set.intersect fruitsYouLike
let doILikeAllYourFruits = fruitsILike |> Set.isSubset fruitsYouLike
```