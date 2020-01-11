# Lesson 28 Architecting Hybrid Language Applications
## 28.1 Crossing Language Boundaries
### 28.1.1 Accepting data from external systems
Using F# to model your domain is effective and useful.
But if you expose data structures that are foreign to C#, you make your API tricky to consume (Lesson 27).
On the other hand, using a simple data model means giving up features in F#.

To mitigate this, define an internal F# domain while using a public API designed to be easy for non-F# consumers to work with.
You then marshal data between the two formats as you move in and out of the F# world.
In essence, you create a validation/transformation layer.

```fsharp
// simple model at the boundary, which can be consumed by C# easily.
// Using only records since they are lightweight and easy to work with in C#
type OrderItemRequest = { ItemId : int; Count : int }
type OrderRequest = 
    {
        OrderId : int
        CustomerName : string
        Comment : string
        EmailUpdates : string
        TelephoneUpdates : string
        Items : IEnumerable<OrderItemRequest>
    }

// the stronger, internal model meant
type OrderId = OrderId of int
type ItemId = ItemId of int
type OrderItem = { ItemId : ItemId; Count : int }
type UpdatePreference =
    | EmailUpdates of string
    | TelephoneUpdates of string
type Order =
    {
        OrderId : OrderId
        CustomerName : string
        ContactPreference : UpdatePreference option
        Comment : string option
        Items : OrderItem list
    }

// It's simple to go from a weaker model to a stronger model.
// At the entrance to the F# module, accept the weak model, but immediately validate and transform it over to the stronger model.
// Here is the validation/transformation
{
    CustomerName =
        match orderRequest.CustomerName with
        | null -> failwith "Customer name required"
        | name -> name
    Comment = orderRequest.Comment |> Option.ofObj
    ContactPreference =
        match Option.ofObj orderRequest.EmailUpdates, Option.ofObj orderRequest.TelephoneUpdates with
        | None, None -> None
        | Some email, None -> Some(Emailupdates email)
        | None, Some phone -> Some(TelephoneUpdates phone)
        | Some _, Some _ -> failWith "only one of telephone and email should be supplied"
}
```

## 28.2 Case study - WPF monopoly
The source code is at https://github.com/isaacabraham/monopoly.
The specific commit is 092c53d.