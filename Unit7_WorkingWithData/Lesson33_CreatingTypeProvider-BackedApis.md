# Lesson 33 Creating type provider-backed APIs
This lesson shows how to create APIs driven by type providers that other components can consume.

## 33.1 Creating a tightly coupled type provider API
```fsharp
type Package = HtmlProvider<"sample-package.html">

// Function signature is string -> decimal.
// Callers have no idea a type provider is being used
let getDownloadsForPackage packageName =
    let package = Package.Load(sprintf "https://www.nuget.org/packages/%s" packageName)
    package.Tables.``Version History``.Rows
    |> Seq.sumBy(fun p -> p.Downloads)

// Function returns HtmlProvider<...>.VersionHistory.Row option
// this is not great because it lets the type provider leak out
let getDetailsForVersion versionText packageName =
    let package = Package.Load(sprintf "https://www.nuget.org/packages/%s" packageName)
    package.Tables.``Version History``.Rows |> Seq.tryFind(fun p -> p.Version.Contains versionText)
```

## 33.2 Creating a decoupled API
To create a truly decoupled API from the type provider, you must construct types (such as F# records and discriminated unions) and then map from the provided types to these.

### 33.2.1 Reasons for not exposing provided types over an API
- the business domain may not fit exactly with the data supplied by a type provider
- at the time of writing the book, provided types could not create records or discriminated unions; thus, decreasing the richness of your domain
- provided types generally cannot be consumed outside F#
- most type providers create types that are erased at runtime
  - thus, you cannot reflect over them (wouldn't be able to use something like Newtonsoft.Json)

### 33.2.2 Enriching a domain by using F# types
```fsharp
// Creating a custom domain for NuGet package statistics
open System
type PackageVersion =
    | CurrentVersion
    | Prerelease
    | Old
type VersionDetails =
    { Version : Version
      Downloads : decimal
      PackageVersion : PackageVersion
      LastUpdated : DateTime }
type NuGetPackage =
    { PackageName : string
      Versions : VersionDetails list }
```

Of course, the API would need to be modified so it returned these objects.