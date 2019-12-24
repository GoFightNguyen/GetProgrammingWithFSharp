# Lesson 23 Business Rules as Code
In this lesson, we will model a customer's contact details in code that adheres to a number of simple rules.

## 23.1 Specific types in F#
Rule 1: a customer can be contacted by email, phone, or post
```fsharp
type Customer =
    { CustomerId : string
      Email : string
      Telephone: string
      Address: string }
```

### 23.1.2 Single-case discriminated unions
Notice the Customer type has multiple string fields, making it easy to mix values of the same type; it is easy to accidentally provide an email value to the address for example.

To solve this, F# has single-case DUs.
We can use a single-case DU as a wrapper class to prevent accidentally mixing up values.
Here's the syntax:
```fsharp
// single-case DU to store a string Address
type Address = Address of string
// creating an instance of a wrapped Address
let myAddress = Address "1 the street"
// comparing a wrapped Address and a raw string will not compile
let isTheSameAddress = (myAddress = "1 the street")
// unwrap an Address into its raw string as addressData
let (Address addressData) = myAddress
```

When defining a single-case DU
- you can omit the pipe to separate cases, and even put it all on a single line.
- Since the contents of the discriminated union are obvious, you can omit the name of the value argument (`string` instead of `address:string`)

Note that after you wrap up a single value in a single-case DU, you cannot compare a "raw" value with it.
You must either wrap a raw value or unwrap the DU to compare the two.

Creating wrapper types for contact details:
```fsharp
type CustomerId = CustomerId of string
type Email = Email of string
type Telephone = Telephone of string
type Address = Address of string
type Customer =
    { CustomerId : CustomerId
      Email : Email
      Telephone : Telephone
      Address : Address }
```

### 23.1.3 Combining dicriminated unions
Rule 2: model that only one of the contact details should be allowed at any point in time
```fsharp
type CustomerId = CustomerId of string

type ContactDetails =
    | Address of string
    | Telephone of string
    | Email of string

type Customer =
    { CustomerId : CustomerId
      ContactDetails : ContactDetails }
```

### 23.1.4 Using optional values within a domain
Rule 3: Customers should have a mandatory primary contact detail and an optional secondary contact detail
```fsharp
type CustomerId = CustomerId of string

type ContactDetails =
    | Address of string
    | Telephone of string
    | Email of string

type Customer =
    { CustomerId : CustomerId
      PrimaryContactDetails : ContactDetails
      SeconaryContactDetails : ContactDetails option }
```

Benefits:
- never have to null check the Customer's primary contact details
- modeled the data in such a way there can only be one of three types at once
- modeled optional secondary contact details, and would use pattern matching to safely handle both value and absence-of-value cases

## 23.2 Encoding business rules with marker types
Rule 4: Customers should be validated as genuine customers, based on whether their primary contact detail is an email address from a specific domain.
Only when customers have gone through this validation process should they receive a welcome email.
Note that you'll also need to perform further functionality in the future, depending on whether a customer is genuine.

```fsharp
// single-case DU to wrap around Customer
type GenuineCustomer = GenuineCustomer of Customer

let validateCustomer customer =
    match customer.PrimaryContactDetails with
    | Email e when e.EndsWith "SuperCorp.com" -> Some(GenuineCustomer customer)
    | Address _ | Telephone _ -> Some(GenuineCustomer customer)
    | Email _ -> None

let sendWelcomeEmail (GenuineCustomer customer) =
    printfn "Hello, %A, and welcome to our site!" customer.CustomerId
```

GenuineCustomer is a single-case DU acting as a __marker type__: it wraps around a standard `Customer` and allows you to treat it differently.
`sendWelcomeEmail` allows only a `GenuineCustomer` as input.
This is the key point; it is now impossible to call this function with an unvalidated customer.

### 23.2.1 When and when not to use marker types
Benefits of __marker types__:
- distinguishing between states
- functions that act only on a specific marker type
- use them at the boundary of your application to perform validation on unchecked data and converting it into checked versions of data, which provides security that you can never run certain code on invalid data

Author's advice is to start simple:
- use single-case DUs as wrapper cases to prevent simple errors such as mixing up values of the same type
- take it further with markert types to represent states
- be careful, taking the use of marker types too far can make it difficult to wade through code

## 23.3 Results vs exceptions
In F#, you can use exceptions by using `try .. with`.
But exceptions aren't encoded within the type system.

An alternative to using exceptions is to use a result, a two-case discriminated union holding either a Success or Failure.
```fsharp
// simple result discriminated union
type Result<'a> =
    | Success of 'a
    | Failure of string

// handling both success and failure cases up front
match methodThatReturnsAResult with
    | Success obj -> printfn "operated with %A" obj
    | Failure error -> printfn "unable to operate: %s" error
```

F# 4.1 includes a `Result` type built into the standard library.

When to use which?
- if an error occurs and is something you do not want to reason about, stick to exceptions
- if you do want to reason about it, use result