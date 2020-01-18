# Lesson 31 Building Schemas from Live Data
A current restriction on type providers is they cannot generate discriminated unions.
For example, if the JSON type provider sees different types of data across rows for the same field, it generates a type such as `StringOrDateTime`, which will have both optional `string` and `DateTime` properties, one of which will contain `Some` value.

## 31.1 Working with JSON
### 31.1.1 Live
```fsharp
open FSharp.Data
type TvListing = JsonProvider<"http://www.bbc.co.uk/programmes/genres/comedy/schedules/upcoming.json">
let tvListing = TvListing.GetSample()
let title = tvListing.Broadcasts.[0].Programme.DisplayTitles.Title
```

### 31.1.2 Examples of live schema type providers
Here are examples of type providers that can work off public data sources:
- JSON type provider - provides a typed schema
- HTML type provider - provides a typed schema
- Swagger type provider - provides a generated client for a Swagger-enabled HTTP endpoint, using swagger metadata to create a strongly typed model
- Azure Storage type provider - provides a client for blob/queue/table storage assets
- WSDL type provider - provides a client for SOAP-based web services

Even if the source data changed schema, we know it instantly.
When you open the solution in Visual Studio, the F# compiler is used to validate the code (and provide IntelliSense), which in turn will connect to the data source.

## 31.2 Avoiding problems with live schemas
Working directly with remote data to generate schemas is awesome since you do not have to download anything locally, or simulate an external service.
But this raises some issues because compilation of your code is linked to a remote data source you might not control.

### 31.2.1 Large data sources
Does the entire dataset have to be loaded for the type provider to work?

### 32.2.2 Inferred schemas
Imagine you are using a CSV file and a field is missing values for all lines except the last.
Does the type provider need to read the entire dataset to infer the type?
What if there are many resources, all following the same schema, which should be used?

### 31.2.3 Priced schemas
Some data sources charge you for every request you make.

### 31.2.4 Connectivity

### Notes
Some type providers can help with these problems.
For example, some type providers allow you to limit the number of lines the data source runs against for schema inference.

## 31.3 Mixing local and remote datasets
Type providers have two phases of operation: compilation and runtime.

The compile phase generates types based off a point-in-time schema (whether its local or remote is irrelevant).
The phase kicks in both at edit time and compile time.

The runtime phase uses the previously compiled types to work with data matching that schema.
This might be the same data used to generate the schema, but it can also be another (possibly remote) data source.

When either coding or building, the type provider will use the static data source to generate a schema and push that into the type system.
Later at runtime, you might use another data source with the same schema against the type provider.
The main point is you can use a local sample dataset for development and compilation, while redirecting to a potentially larger, real-world dataset at runtime, which solves the problems listed in section 31.2

### 31.3.1 Redirecting type providers to new data
So far we have generated a type provider instance by using the `GetSample()` function on the type.
This loads in all data from the source that you used to generate the schema.
But you can repoint a type provider at a secondary data source with the same schema by using the `Load` function