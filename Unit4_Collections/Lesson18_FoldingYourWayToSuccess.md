# Lesson 18 Folding your way to success

## 18.1 Understanding aggregations and accumulators
Aggregation functions, such as `sum` and `min`, take a collection of things and return a single other thing.

### 18.1.1 Creating an aggregation function
Performing any aggregation, or __fold__, generally requires three things:
- the input collection
- an accumulator to hold the state of the output result as it's built up
- an initial (empty) value for the accumulator to start with

Here's an example of an imperative implementation of sum
```fsharp
let sum inputs =
    let mutable accumulator = 0
    for input in inputs do
        accumulator <- accumulator + input
    accumulator
```

This could be rewritten using recursion, which maintains its own state.
But we will look at doing this using a collection-based way without any mutation and accumulation.

## 18.2 Saying hello to fold
`fold` is a generalized way to achieve an aggregation.
It's a higher-order function that allows you to supply an input collection, a start state for the accumulator, and a function saying how to accumulate data.

The previous example implemented through `fold`.
This has the same three key elements, but you do not have to explicitly store an accumulator value or iterate over the collection.
```fsharp
let sum inputs = 
    Seq.fold
        (fun state input -> state + input)
        0
        inputs
```

### 18.2.1 Making fold more readable
The double pipeline operator acts like the normal pipeline, but takes in the last two args and moves them to the front as a tuple.

Here are some ways to make `fold` read in a more logical way
```fsharp
inputs |> Seq.fold (fun state input -> state + input) 0
// double pipeline
(0, inputs) ||> Seq.fold (fun state input -> state + input)
```

### 18.2.2 Using related fold functions
- `foldBack` - like `fold`, but goes backward
- `mapFold` - emits a sequence of mapped results and a final state
- `reduce` - simplified version of `fold`, uses the first element in the collection as the initial state so you don't have to supply one. Throws an exception on an empty input
- `scan` - similar to `fold`, but generates the intermediate results as well as the final state
- `unfold` - generates a sequence from a single starting state. Similar to the `yield` keyword

### 18.2.3 Folding instead of while loops
What if you don't have an up-front collection of data?
```fsharp
// Accumulating through a while loop
open System.IO
let mutable totalChars = 0
let sr = new StreamReader(File.OpenRead "book.txt")

while (not sr.EndOfStream) do
    let line = sr.ReadLine()
    totalChars <- totalChars + line.ToCharArray().Length

// Simulate the collection by using the yield keyword (sequence expressions)
open System.IO
let lines : string seq =
    seq {
        use sr = new StreamReader(File.OpenRead @"book.txt")
        while (not sr.EndOfStream) do
            yield sr.ReadLine() }
(0, lines) ||> Seq.fold(fun total line -> total + line.Length)
```

The `seq {}` block is a form of _computation expression_, which is a special block in which certain keywords, such as `yield`, can be used. Look in unit 8.
Here, we yield items to lazily generate a sequence.

## 18.3 Composing functions with fold
Composing functions with `fold` means given a list of functions having the same signature, produce a single function that runs all of them together.

```fsharp
open System
type Rule = string -> bool * string
let rules : Rule list =
    [
        fun text -> (text.Split ' ').Length = 3, "must be 3 words"
        fun text -> text.Length <= 30, "max length is 30 chars"
        fun text -> text
                    |> Seq.filter Char.IsLetter
                    |> Seq.filter Char.IsUpper, "All letters must be caps"
    ]
```

Rule is a _type alias_.
Type aliases let you define a signature that you can use in place of another one.
It is not a type.
The definition it aliases is interchangeable with it, and the alias will be erased at runtime.
Type aliases are a way to improve documentation and readability.

### 18.3.1 Composing the rules manually
Using the preceding rules, here's how to manually compose them into a single "super" rule
```fsharp
let validateManual (rules : Rule list) word =
    let passed,error = rules.[0] word
    if not passed then false, error
    else
        let passed,error = rules.[1] word
        if not passed then false, error
        else
            let passed,error = rules.[2] word
            if not passed then false, error
            else true, ""
```

### 18.3.2 Folding functions together
The approach in 18.3.1 does not scale well.
Let's do this instead.
```fsharp
let buildValidator (rules : Rule list) =
    rules
    |> List.reduce (fun firstRule secondRule ->
        fun word ->
            let passed,error = firstRule word
            if passed then
                let passed,error = secondRule word
                if passed then true, ""
                else false, error
            else false, error
    )

let validate = buildValidator rules
let word = "HELLO FrOM F#"
validate word
```

You can think of folding functions together as the composite pattern in OO.