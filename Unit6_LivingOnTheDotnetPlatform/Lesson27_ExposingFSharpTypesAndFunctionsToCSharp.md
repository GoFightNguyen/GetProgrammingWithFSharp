# Lesson 27 Exposing F# types and functions to C#
## 27.1 Using F# types in C#
### 27.1.1 Records
Records appear as classes in C#
- a constructor requiring all fields
  - a default ctor _is not_ normally generated, so it is impossible to create the record in an un/partially initailized state
- a public readonly property for each field
- implements various interfaces in order to allow structural equality checking
- a method for each member function

### 27.1.2 Tuples
Tuples are instances of the `System.Tuple` type in C#.
As of the writing of this book, it was hypothesized using the `struct` keyword in F# would map to the `System.ValueTuple` introduced in C# 7.

### 27.1.3 Discriminated Unions
DUs are roughly equivalent to a class hierarchy in C#, except they're a closed set of classes
- one clase per case
- static helper methods to check which case the value is
- static helper methods to create instances of a case yourself
- an instance will be of the high-level DU type, you have to explicitly cast it to the case type

## 27.2 More on F# interoperability
### 27.2.1 Using namespaces and modules
Namespaces in F# are the same thing in C#.

A module is a static class in C#
- a public property for each simple value or record value
- a method for each function
- a nested class for each type

### 27.2.2 Using F# functions in C#
Although functions can be tupled or curried in F#, both are represented as tupled in C#, meaning all args are required at once.

There is an exception.
An already partially curried function will be represented in C#, but it is nonidiomatic C#.
So try to avoid providing partially applied functions to C#.
If you must, wrap them in a "normal" F# function explicitly taking in all args required by the partially applied version, and supplies those args manually.

## 27.3 Summarizing F# to C# Interoperability
Summary table
| Element | Renders as | C# compatibility |
| --- | --- | --- |
| Records | immutable class | high |
| Tuples | System.Tuple (look at 27.1.2 for possible change) | med/high |
| Discriminated Unions | classes with builder methods | med/low |
| Namespaces | Namespaces | high |
| Modules | static classes | high |
| Functions | static methods | high/med |

### 27.3.1 Gotchas
#### Incompatible Types
Some types, such as unit of measure and type providers, do not exist in C#.
Generally, this is because there's no CLR support for them, and they're erased at compile time.

#### CLI Mutable
On rare occassions, you'll need to create an F# record from C# in an unitialized state (or partially initialized).
This primarily happens when reflection is used to create objects.
In F#, you can use the `[<CLIMutable>]` attribute on a record.
This changes nothing in F#.
But affects the underlying IL emitted so C# code can access a default ctor and properties have setters

#### Options
You can consume F# option types in C# if you add a reference to `FSharp.Core`.
But they're not idiomatic to work with in C#.

Instead, add extension methods removing the need for supplying type args.

#### Collections
F# arrays are standard .NET arrays, so no problem.
Sequences appear as `IEnumerable<T>`, so no problem.

The F# list is not the standard .NET generic list (which is called `ResizeArray` in F#).
Although you can use LINQ methods on it, avoid exposing it to C# clients.
It is of limited use and you will need to add a reference to `FSharp.Core`.