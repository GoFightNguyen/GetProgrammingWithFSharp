#load "Domain.fs"
#load "Operations.fs"

open Capstone3.Operations
open Capstone3.Domain
open System


let isValidCommand command =
    command = 'd' || command = 'w' || command = 'x'

let isStopCommand command =
    command = 'x'

let getAmount command =
    match command with
    | 'w' -> command, 25M
    | 'd' -> command, 50M
    | 'x' -> command, 0M

let processCommand account (command:char, amount:decimal) =
    if command = 'w' then
        withdraw amount account
    else if command = 'd' then
        deposit amount account
    else
        account        


let openingAccount =
    { Owner = { Name = "Isaac" }; Balance = 0M; AccountId = Guid.Empty }
let account =
    let commands = [ 'd'; 'w'; 'z'; 'f'; 'd'; 'x'; 'w' ]

    commands
    |> Seq.filter isValidCommand
    |> Seq.takeWhile (not << isStopCommand)
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount