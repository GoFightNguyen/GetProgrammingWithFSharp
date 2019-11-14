module Capstone2.Operations

open Capstone2.Domain

let deposit amount account =
    let newBalance = account.Balance + amount
    { account with Balance = newBalance }

let withdrawal amount account =
    if amount > account.Balance then account
    else { account with Balance = account.Balance - amount }

let auditAs operationName audit operation amount account = 
    audit account (sprintf "Performaing a %s operation for $%M..." operationName amount)
    
    let updatedAccount = operation amount account
    let accountIsUnchanged = (updatedAccount = account)
    
    if accountIsUnchanged then audit account (sprintf "Transaction rejected")
    else  audit updatedAccount (sprintf "Transaction accepted! Balance is now $%M" updatedAccount.Balance)
    
    updatedAccount