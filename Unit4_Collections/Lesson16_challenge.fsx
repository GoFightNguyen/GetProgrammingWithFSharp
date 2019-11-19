open System.IO

let folderPath = @"C:\src\GitHub-Public\GetProgrammingWithFSharp"

type Info =
    { DirName : string
      Size : int64 
      NumberOfFiles : int
      AvgFileSize : double }

let getStats (dir, files: array<FileInfo>) =
    let size = files |> Array.sumBy (fun f -> f.Length)
    let numberOfFiles = files |> Array.length
    let avgFileSize = double size / double numberOfFiles
    { DirName = dir
      Size = size
      NumberOfFiles = numberOfFiles
      AvgFileSize = avgFileSize }

Directory.GetDirectories folderPath
    |> Array.map (fun d -> d, Directory.GetFiles d)
    |> Array.map (fun (d,f) -> d, f |> Array.map FileInfo)
    |> Array.map getStats
    |> Array.sortByDescending (fun a -> a.Size)