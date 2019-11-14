// Learn more about F# at http://fsharp.org

open System
open Capstone2.Domain
open Capstone2.Operations
open Capstone2.Auditing

[<EntryPoint>]
let main argv =
    let name = 
        Console.Write("Enter your name: ")
        Console.ReadLine()
    let balance = 
        Console.Write("What is your balance: ")
        Console.ReadLine() |> Decimal.Parse

    let mutable account =
        { Balance = balance
          ID = 3
          Customer = { Name = name }}

    let consoleAuditDeposit = deposit |> auditAs "deposit" consoleAudit
    let consoleAuditWithdraw = withdrawal |> auditAs "withdraw" consoleAudit

    while true do
        let operation =
            Console.Write("(d)eposit or (w)ithdrawal: ")
            Console.ReadLine()

        let amount =
            Console.Write("Amount: ")
            Console.ReadLine() |> Decimal.Parse

        account <-
            if operation = "d" then account |> consoleAuditDeposit amount
            else account |> consoleAuditWithdraw amount

    0 // return an integer exit code
