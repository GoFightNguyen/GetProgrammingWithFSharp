namespace Capstone3.Domain

open System

type Customer = { Name : string }
type Account = { AccountId : Guid; Owner : Customer; Balance : decimal }
type Transaction = { Amount : decimal; Operation : string; TimeStamp : DateTime; Accepted : bool}

module Transactions =
    let serialize transaction =
        sprintf "%O***%s***%M***%b"
            transaction.TimeStamp
            transaction.Operation
            transaction.Amount
            transaction.Accepted

    /// Deserializes a transaction
    let deserialize (fileContents:string) =
        let parts = fileContents.Split([|"***"|], StringSplitOptions.None)
        { TimeStamp = DateTime.Parse parts.[0]
          Operation = parts.[1]
          Amount = Decimal.Parse parts.[2]
          Accepted = Boolean.Parse parts.[3] }        