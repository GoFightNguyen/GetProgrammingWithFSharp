module Capstone2.Auditing

open Capstone2.Domain

let consoleAudit account message =
    printfn "Account %d: %s" account.ID message