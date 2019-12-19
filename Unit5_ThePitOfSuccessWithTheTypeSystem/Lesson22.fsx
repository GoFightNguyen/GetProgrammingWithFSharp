let tryLoadCustomer id =
    match id with
    | id when id >= 2 && id <= 7 -> Some (sprintf "Customer %d" id)
    | _ -> None

[0..10]
|> List.choose tryLoadCustomer