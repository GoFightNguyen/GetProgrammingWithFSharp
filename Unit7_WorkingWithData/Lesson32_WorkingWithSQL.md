# Lesson 32 Working with SQL
This lesson teaches you how to use type providers with MSSQL.

## 32.2 Introducing the SqlClient project
The SqlClient package (open source) is a data access layer designed specifically for MS SQL containing several type providers.
I believe the namespace is FSharp.Data.SqlClient.

This project provides a stateless model for working with databases.

### 32.2.1 Querying data with the SqlCommandProvider
```fsharp
open FSharp.Data

let [<Literal>] Conn =
    "Server=(localdb)\MSSQLLocalDb;Database=AdventureWorksLT;Integrated Security=SSPI"
// create a strongly typed SQL query
type GetCustomers =
    SqlCommandProvider<"Select * From SalesLT.Customer", conn>
// execute the query
let customers = GetCustomers.Create(conn).Execute() |> Seq.toArray
let customer = customers.[0]
```

Regarding the connection string `Conn`.
It is marked with the `[<Literal>]` attribute so it is a compile-time constant, which is needed when passing values as args to type providers.
`Conn` is Pascal-cased, which is best practice for literals.

When creating the query, the connection string is used as a compile-time souce for schema generation.
When executing the query, the connection string is used as the runtime data source. 

Because SQL has a type system, there's no need to infer the type based on a sample of data - the schema from SQL is used directly by F#.
This type provider validates the SQL you write against the SQL server itself while generating types (at compile-time only, of course).

### 32.2.2 Inserting data
Using the SqlClient package, there are two ways to insert data:
- create insert or update commands and execute them
- use a wrapper around .NET data tables

To use the .NET data tables approach, I believe you use the `SqlProgrammabilityProvider`.

### 32.2.3 Working with reference data
The SqlClient package also addresses reference data (think lookup tables).
The type provider `SqlEnumProvider` helps by generating a class with values for all reference data values based on an arbitrary query.

```fsharp
type Categories = SqlEnumProvider<"Select Name, ProductCategoryId From SalesLT.ProductCateegory", Conn>
let woolyHats = Categories.``Wooly Hats``
printfn "Wooly Hats has ID %d" woolyHats
```

## 32.3 Using the SQLProvider
The type provider `SQLProvider` works with Oracle, SQLite, Postgres, and other ODBC data sources.
It takes an ORM approach.

### 32.3.1 Querying data
As an ORM, SQLProvider supports the IQueryable pattern.

```fsharp
open FSharp.Data.Sql
type AdventureWorks = SqlDataProvider<ConnectionString="<connection string goes here>", UseOptionTypes = true>
// get a handle to a sessionized data context
let context = AdventureWorks.GetDataContext()
let customers =
    // query expression...an equivalent to a LINQ query in C#
    query {
        for customer in context.SalesLt.Customer do
        take 10
    } |> Seq.toArray
let customer = customers.[0]
```

You can project results to custom records or use tuples (remember F# does not have anonymous types).

```fsharp
query {
    for customer in context.SalesLt.Customer do
    where (customer.CompanyName = Some "Sharp Bikes")
    select (customer.FirstName, customer.LastName)
    distinct
}
```