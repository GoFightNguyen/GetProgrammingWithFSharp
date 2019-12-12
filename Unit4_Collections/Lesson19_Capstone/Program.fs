module Capstone3.Program

open System
open Capstone3.Domain
open Capstone3.Operations

let withdrawWithAudit = auditAs "withdraw" Auditing.composedLogger withdraw
let depositWithAudit = auditAs "deposit" Auditing.composedLogger deposit
let loadAccountFromDisk = FileRepository.findTransactionsOnDisk >> Operations.loadAccount

let isValidCommand command =
    command = 'd' || command = 'w' || command = 'x'

let isStopCommand command =
    command = 'x'

let getAmount command =
    match command with
    | 'w' -> command, 25M
    | 'd' -> command, 50M
    | 'x' -> command, 0M

let getAmountConsole command =
    Console.WriteLine()
    Console.Write "Enter Amount: "
    let amount = Console.ReadLine() |> Decimal.Parse
    command, amount

[<EntryPoint>]
let main _ =
    let openingAccount =
        Console.Write "Please enter your name: "
        Console.ReadLine() |> loadAccountFromDisk
    
    printfn "Current balance is £%M" openingAccount.Balance

    let consoleCommands = 
        seq {
            while true do
                Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
                yield Console.ReadKey().KeyChar
        }

    let processCommand account (command, amount) =
        printfn ""
        let account =
            if command = 'd' then account |> depositWithAudit amount
            else account |> withdrawWithAudit amount
        printfn "Current balance is £%M" account.Balance
        account

    let closingAccount =
        consoleCommands
        |> Seq.filter isValidCommand
        |> Seq.takeWhile (not << isStopCommand)
        // |> Seq.map getAmount
        |> Seq.map getAmountConsole
        |> Seq.fold processCommand openingAccount

    Console.Clear()
    printfn "Closing Balance:\r\n %A" closingAccount
    Console.ReadKey() |> ignore

    0