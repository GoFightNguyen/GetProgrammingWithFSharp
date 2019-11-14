open System
type Customer =
    { Name: string }

type Account =
    { Balance: decimal
      ID: int
      Customer: Customer }

let account =
    { Balance = 100m
      ID = 1
      Customer = { Name = "Jason Nguyen" } }

let deposit amount account =
    let newBalance = account.Balance + amount
    { account with Balance = newBalance }

let withdrawal amount account =
    if amount > account.Balance then account
    else { account with Balance = account.Balance - amount }

let consoleAudit account message =
    printfn "Account %d: %s" account.ID message


let auditAs operationName audit operation amount account = 
    audit account (sprintf "Performaing a %s operation for $%M..." operationName amount)
    
    let updatedAccount = operation amount account
    let accountIsUnchanged = (updatedAccount = account)
    
    if accountIsUnchanged then audit account (sprintf "Transaction rejected")
    else  audit updatedAccount (sprintf "Transaction accepted! Balance is now $%M" updatedAccount.Balance)
    
    updatedAccount

let withdrawWithConsoleAudit = auditAs "withdraw" consoleAudit withdrawal
let depositWithConsoleAudit = auditAs "deposit" consoleAudit deposit

// account
// |> deposit 100m
// |> withdrawal 50m
// |> withdrawal 150m

account
|> depositWithConsoleAudit 100m
|> withdrawWithConsoleAudit 50m