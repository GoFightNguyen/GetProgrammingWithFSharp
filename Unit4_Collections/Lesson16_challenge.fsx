open System.IO

let folderPath = @"C:\src\GitHub-Public\GetProgrammingWithFSharp"

type Info =
    { DirName : string
      Size : int64 
      NumberOfFiles : int
      AvgFileSize : double }

let getStats (dir, files: array<FileInfo>) =
    let dirSize = files |> Array.sumBy (fun f -> f.Length)
    let numberOfFiles = files |> Array.length
    let avgFileSize = double dirSize / double numberOfFiles

    // another way
    // let fileSizes = files |> Array.map (fun f -> f.Length)
    // let dirSize = fileSizes |> Array.sum
    // let numberOfFiles = files |> Array.length
    // let avgFileSize = fileSizes |> Array.map double |> Array.average

    { DirName = dir
      Size = dirSize
      NumberOfFiles = numberOfFiles
      AvgFileSize = avgFileSize }

Directory.GetDirectories folderPath
    |> Array.map (fun d -> d, Directory.GetFiles d)
    |> Array.map (fun (d,f) -> d, f |> Array.map FileInfo)
    |> Array.map getStats
    |> Array.sortByDescending (fun a -> a.Size)