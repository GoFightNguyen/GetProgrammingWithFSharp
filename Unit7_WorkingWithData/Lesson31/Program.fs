// Learn more about F# at http://fsharp.org

open System
open FSharp.Data
open XPlot.GoogleCharts

type StarWarsPerson = JsonProvider<"https://swapi.co/api/people/1/?format=json">
type Films = HtmlProvider<"https://en.wikipedia.org/wiki/Robert_De_Niro_filmography">
type Package = HtmlProvider<"sample-package.html">

[<EntryPoint>]
let main argv =
    printfn "JSON Type Provider"
    let starWarsPerson = StarWarsPerson.GetSample()
    printfn "The name is %s" starWarsPerson.Name
    
    printfn ""

    printfn "HTML Type Provider"
    let deNiro = Films.GetSample()
    deNiro.Tables.Film.Rows
    |> Array.countBy(fun r -> string r.Year)
    |> Chart.SteppedArea
    |> Chart.Show

    printfn ""

    printfn "Redirecting type provider"
    let nunit = Package.Load "https://www.nuget.org/packages/nunit"
    let ef = Package.Load "https://www.nuget.org/packages/entityframework"
    let newtonsoftJson = Package.Load "https://www.nuget.org/packages/newtonsoft.json"
    [ nunit; ef; newtonsoftJson]
    |> Seq.collect(fun p -> p.Tables.``Version History``.Rows)
    |> Seq.sortByDescending(fun r -> r.Downloads)
    |> Seq.take 10
    |> Seq.map(fun r -> r.Version, r.Downloads)
    |> Chart.Column
    |> Chart.Show

    0 // return an integer exit code
