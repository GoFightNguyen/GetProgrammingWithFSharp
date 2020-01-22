// Learn more about F# at http://fsharp.org

open System
open FSharp.Data

// create a static instance of the type provider
type Package = HtmlProvider<"sample-package.html">

let getPackage packageName =
    packageName |> sprintf "https://www.nuget.org/packages/%s" |>
    Package.Load                   

let getDownloadsForPackage packageName =
    let package = Package.Load(sprintf "https://www.nuget.org/packages/%s" packageName)
    package.Tables.``Version History``.Rows
    |> Seq.sumBy(fun p -> p.Downloads)

let getDetailsForVersion versionText packageName =
    let package = getPackage packageName
    package.Tables.``Version History``.Rows |> Seq.tryFind(fun p -> p.Version.Contains versionText)

////// If the above is used as is, it is tightly coupled.
////// The following is a decoupled approach

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

let parse (versionText:string) =
    let getVersionPart (version:string) isCurrent =
        match version.Split '-', isCurrent with
        | [| version; _ |], true
        | [| version |], true -> Version.Parse version, CurrentVersion
        | [| version; _ |], false -> Version.Parse version, Prerelease
        | [| version |], false -> Version.Parse version, Old
        | _ -> failwith "unknown version format"

    let parts = versionText.Split ' ' |> Seq.toList |> List.rev
    match parts with
    | [] -> failwith "Must be at least two elements to a version"
    | "version)" :: "(this" :: version :: _ -> getVersionPart version true
    | version :: _ -> getVersionPart version false

let enrich (versionHistory:Package.VersionHistory.Row seq) = 
    { PackageName =
        match versionHistory |> Seq.map(fun row -> row.Version.Split ' ' |> Array.toList |> List.rev) |> Seq.head with
        | "version)" :: "(this" :: _ :: name | _ :: name -> List.rev name |> String.concat " "
        | _ -> failwith "Unable to parse version name"
      Versions =
        versionHistory 
        |> Seq.map(fun versionHistory ->
            let version, packageVersion = parse versionHistory.Version
            { Version = version
              Downloads = versionHistory.Downloads
              LastUpdated = versionHistory.``Last updated``
              PackageVersion = packageVersion })
        |> Seq.toList }

// let loadPackageVersions = getPackage >> getVersionsForPackage >> enrich >> (fun p -> p.Versions)
// let getDetailsForVersion version = loadPackageVersions >> Seq.find(fun p -> p.Version = version)
// let getDetailsForCurrentVersion = loadPackageVersions >> Seq.find(fun p -> p.PackageVersion = CurrentVersion)

// "Newtonsoft.Json" |> getDetailsForVersion (Version.Parse "9.0.1")

// let getDownloadsForPackage = loadPackageVersions >> Seq.sumBy(fun p -> p.Downloads)

[<EntryPoint>]
let main argv =
    printfn "%O" (getDownloadsForPackage "EntityFramework")
    0 // return an integer exit code
