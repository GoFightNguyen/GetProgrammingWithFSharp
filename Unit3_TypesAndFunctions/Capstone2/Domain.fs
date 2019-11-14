namespace Capstone2.Domain

type Customer =
    { Name: string }

type Account =
    { Balance: decimal
      ID: int
      Customer: Customer }