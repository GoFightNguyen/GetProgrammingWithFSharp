# Lesson 10 Shaping Data with Records
The record is a more fully featured data structure (as compared to tuple) more akin to a C# class.

## 10.1 POCOs done right: records in F#
F# records are best described as simple-to-use objects designed to store, transfer, and access immutable data that have named fields.

### 10.1.1 Record basics
The following example:
- includes a constuctor requiring all fields to be provided
- Supports immutability - provides public access for all fields (which are themselves read-only)
- full structural equality (as opposed to C# reference equality) throughout the _entire object graph_
```fsharp
type Address =
    { Street : string
      Town : string
      City : string }

// Could also do like this
type Address = { Street : string, Town : string, City : string }
```

### 10.1.2 Creating records
```fsharp
// This relies on the Address type in the above example
let address =
    { Street = "the street"
      Town = "the town"
      City = "the city" }
```
When creating a record you must set all fields.
If you do not the compiler warns you.

## 10.2 Doing more with records
### 10.2.1 Type inference with records
Although creating an instance of a record looks like a dynamic object, the compiler knows it is a static type.
The compiler infers the type based on the _properties assigned to the object_.

You can explicitly tell the compiler by specifying the type of the left-side binding or by prefixing fields with the type name.
It is encouraged to avoid using explicit types unless necessary.
One benefit of prefixing a field with the type is the compiler provides immediate IntelliSense.
```fsharp
let address : Address =
    { Street = "the street"
      Town = "town"
      City = "city" }

let address2 =
    { Address.Street = "street"
      Town = "town"
      City = "city" }
```

F# will also infer a record type based on _usage_ of an instance.

Although records compile down to classes, we want records to act as DTOs.
You normally should not add member methods to a record.

### 10.2.2 Working with immutable records
Fields on F# records are immutable by default.
How do we model change without mutating a field on a record?
F# provides a __copy-and-update__ syntax.
You provide a record _with_ the modifications you want and F# creates a copy of the record with those changes applied.
```fsharp
let updatedCustomer =
    { customer with
        Age = 31
        EmailAddress = "j@bus.com" }
```

At the time of this book, records are reference types.
Thus, in rare occassions, it might be necessary to have mutable fields on a record instead of always allocating a new record on the heap.
_If you absolutely must, you can mark a field as mutable on the record._

## 10.3 Tips and tricks with records
### 10.3.2 Shadowing
Shadowing allows you to reuse existing named bindings.
You reuse the symbol but with a new value, this is not mutation.
```fsharp
let myHome = { Street = "The Street"; Town = "The Town"; City = "The City" }
let myHome = { myHome with City = "The Other City" }
let myHome = { myHome with City = "The Third City" }
```
Shadowing is an alternative to using _copy-and-update_ into a new symbol.

### 10.3.3 When to use records
Benefits of records:
- explicitly name fields
- _copy-and-update_ syntax
- can be consumed easily in other .NET languages and systems expecting classes (records compile to classes)