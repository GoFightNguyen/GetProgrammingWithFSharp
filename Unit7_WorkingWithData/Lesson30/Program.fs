// Learn more about F# at http://fsharp.org

open FSharp.Data

type Football = CsvProvider<"FootballResults.csv">

[<EntryPoint>]
let main argv =
    let data = Football.GetSample().Rows |> Seq.toArray

    data
    |> Seq.filter(fun row ->
        row.``Full Time Home Goals`` > row.``Full Time Away Goals``)
    |> Seq.countBy(fun row -> row.``Home Team``)
    |> Seq.sortByDescending snd
    |> Seq.take 10
    |> Seq.iter(fun row -> printfn "%s" (fst row))

    0 // return an integer exit code
