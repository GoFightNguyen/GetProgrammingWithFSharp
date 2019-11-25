# Lesson 13 Achieving Code Reuse in F#
Let's look at using functions and type inference to create lightweight code reuse, and to pass functionality (rather than data) through a system.

A __higher-order function__ is a function taking in another function as one of its arguments.

| | OO | Functional |
| --- | --- | --- |
| Contract Specification | Interface (nominal) | Function (structural) |
| Common patterns | Strategy/command | Higher-order function |
| Verbosity | med/heavy | lightweight |
| Composability and reuse | medium | high |
| dimensionality | multiple methods per interface | single functions |

## 13.2 Implementing higher-order functions in F#
### 13.2.1 Basics of higher-order functions
```fsharp
type Customer = { Age : int}
let where filter customers =
    seq {
        for customer in customers do
            if filter customer then
                yield customer
    }

// An F# list, expressed using [ ; ; ; ] syntax
let customers = [ { Age = 21}; { Age = 35 }; { Age = 36 } ]
let isOver35 customer = customer.Age > 35

// supplying the isOver35 function into the where function
customers |> where isOver35
// passing a function inline using lambda syntax
customers |> where (fun customer -> customer.Age > 35)
```
`seq { }` is a type of _computation expression_, this is discussed more for asynchronous programming.
Here it's used to express that you're generating a sequence of customers by using the `yield` keyword.

In the `where` function
- `filter` is identified as a function taking in a generic
- `customers` is identified as a `seq` (F# shorthand for IEnumerable<T>) because it is used within a `for` loop

### 13.2.2 When to pass functions as arguments
Passing functions is the primary way of achieving reuse.
When coupled with F#'s ability to infer, compose, and pipeline functions, passing functions as args is easy.

## 13.3 Dependencies as functions
It is often preferable to explicitly pass in dependencies as functions rather than a larger interface containing some unnecessary methods.
In F#, we can just as easily pass in dependencies, but instead of passing them into constructors of classes, we pass them into functions directly.

You should supply dependencies as the __first__ arg(s) in a function.
This allows partial application of the function.
This partially applied function might itself be passed into other functions, which will have no coupling.