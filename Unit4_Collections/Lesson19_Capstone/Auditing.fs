module Capstone3.Auditing

open Capstone3.Operations
open Capstone3.Domain

/// Logs to the console
// let printTransaction _ accountId message = printfn "Account %O: %s" accountId message
let printTransaction _ accountId transaction =
    printfn "Account %O: %s of %M (approved: %b)" accountId transaction.Operation transaction.Amount transaction.Accepted

// Logs to both console and file system
let composedLogger = 
    let loggers =
        [ FileRepository.writeTransaction
          printTransaction ]
    fun accountId owner message ->
        loggers
        |> List.iter(fun logger -> logger accountId owner message)