module Capstone3.Operations

open System
open Capstone3.Domain

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount account =
    if amount > account.Balance then account
    else { account with Balance = account.Balance - amount }

/// Deposits an amount into an account
let deposit amount account =
    { account with Balance = account.Balance + amount }

/// Runs some account operation such as withdraw or deposit with auditing.
// let auditAs operationName audit operation amount account =
//     let audit = audit account.AccountId account.Owner.Name
//     audit (sprintf "%O: Performing a %s operation for £%M..." DateTime.UtcNow operationName amount)
//     let updatedAccount = operation amount account
    
//     let accountIsUnchanged = (updatedAccount = account)

//     if accountIsUnchanged then audit (sprintf "%O: Transaction rejected!" DateTime.UtcNow) 
//     else audit (sprintf "%O: Transaction accepted! Balance is now £%M." DateTime.UtcNow updatedAccount.Balance)

//     updatedAccount
let auditAs operationName audit operation amount account =
    let updatedAccount = operation amount account
    let accountIsUnchanged = (updatedAccount = account)

    let transaction = 
        let transaction = { Operation = operationName; Amount = amount; TimeStamp = DateTime.UtcNow; Accepted = true }
        if accountIsUnchanged then { transaction with Accepted = false}
        else transaction

    audit account.AccountId account.Owner.Name transaction
    updatedAccount

/// Creates an account from a historical set of transactions
let loadAccount (owner, accountId, transactions) =
    let openingAccount = { AccountId = accountId; Balance = 0M; Owner = { Name = owner } }

    transactions
    |> Seq.sortBy(fun txn -> txn.TimeStamp)
    |> Seq.fold(fun account txn ->
            if txn.Operation = "withdraw" then account |> withdraw txn.Amount
            else account |> deposit txn.Amount
        ) 
        openingAccount