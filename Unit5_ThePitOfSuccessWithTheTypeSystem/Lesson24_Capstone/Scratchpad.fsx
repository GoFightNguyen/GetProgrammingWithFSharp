#load "Domain.fs"
#load "Operations.fs"

open Capstone.Operations
open Capstone.Domain
open System

type Command =
    | Withdraw
    | Deposit
    | Exit

let tryParseCommand commandLetter =
    match commandLetter with
    | 'w' -> Some Withdraw
    | 'd' -> Some Deposit
    | 'x' -> Some Exit
    | _ -> None

tryParseCommand 'x'
