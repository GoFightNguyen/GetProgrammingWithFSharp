# Lesson 30 Introducing Type Providers
## 30.1 Understanding type providers
An F# type provider is a component providing types, properties, and methods for use in your program.
Type providers are a significant part of F# support for information-rich programming.

At their most basic, type providers are just F# assemblies that can be plugged into the F# compiler, and then can be used at edit time to generate entire type systems for you to work with _as you type_.
They can be used with _live_ data sources, and even other programming languages.

Type providers can affect type systems without rebuilding the project, because they run in the background as you write code.
The term information-rich programming refers to the concept of bringing disparate data sources into the F# programming language in an extensible way.

## 30.2 Working with your first type provider
Using soccer results from a CSV file, we will answer: which three teams won at home the most over the whole season.

### 30.2.1 Working with CSV files today
We're used to a process such as this:

Understand source data (Excel) -> Create C# POCO -> Skip header row -> convert rows into POCOs -> Perform business logic

This is not great for exploring quickly and easily.

### 30.2.2 Introducing FSharp.Data
`FSharp.Data` provides generated types when working with data in CSV, JSON, or XML formats.
Don't have to manually parse the dataset.
Don't have to figure out the types; this type provider scans through the first few rows and infers the types based on the contents of the file.

```fsharp
// XPlot provides access to charts available in Google charts as well as Plotly.

#r @"..\..\packages\FSharp.Data\lib\net40\FSharp.Data.dll"
open FSharp.Data
type Football = CsvProvider< @"..\..\data\FootballResults.csv">
let data = Football.GetSample().Rows |> Seq.toArray

data
|> Seq.filter(fun row ->
    row.``Full Time Home Goals`` > row.``Full Time Away Goals``)
|> Seq.countBy(fun row -> row.``Home Team``)
|> Seq.sortByDescending snd
|> Seq.take 10
|> Chart.Column
|> Chart.Show
```

#### Backtick Members
Note the fields listed have spaces in them.
F# has a feature called backtick members; just place a double backtick at the beginning and end of the member definition, and you can put spaces, numbers, or other characters in the member definition.
Intellisense is not always correct for these.

#### Type Erasure
Most type providers are _erasing_.
The types generated exist only at compile time.
At runtime, the types are erased and usually compile down to plain objects; you won't see the fields if you use reflection.

Type erasing providers are efficient because they can create type systems with thousands of types without any runtime overhead, because at runtime they're of type object.

A downside of erasing is it makes them difficult to work with in C#.

_Generative_ type providers allow for runtime reflection, but are not common.

### 30.2.3 Inferring types and schemas
When working with type providers, you must realize the type system is driven by an external data source.

When it comes to schema inferernce, some type providers work differently from others.
Examples:
- `FSharp.Data` allows you to manually override the schema by supplying a custom argument to the type provider
- Others, like for SQL, can use schema guidance from the source system