# Lesson 41 Property-based testing in F#
Property-based testing can help when:
- there are "unbounded" inputs
- tests use arbitrary cases
- something is too complex to cover, or even identify, all the possible permutations.
This is common for integration tests.

## 41.1. Understanding property-based testing
The idea behind property-based testing is you allow the machine to create test data for you, based on guidelines that you provide.
Then you test behaviors, or properties of the system that should hold true for any input values.
Write enough properties, and you prove the functionality of the function as a whole.

Property-based testing requires a different approach to thinking about tests.
Since the inputs are generated for you, you cannot hardcode the expected result of a test.
This is where properties enter.
A property is a kind of relationship you can test on the output of your production code, without knowing the value of it.

For example.
Consider a method that takes in a string and flips the case of all the characters.
Your properties might be: same number of letters, no letter is the same case, and every letter is the same.

### 41.1.1 How to identify properties
Here are _some examples_ of how you might identify properties:
- identify specific properties about the behavior of the inputs and outputs
- identify a relationship between two functions.
For example, a relationship between the two functions `a + b = c` and `a - b = d` is `c - d = b x 2`
- compare another implementation of the same function

## 41.2 Introducing FsCheck
The author uses FsCheck.Xunit to demonstrate property-based testing.
Please refer to the book for more info.

## 41.3 Controlling data generation
Please refer to the book for more info.

You use _guard clauses_ in the test function to control data generation.
You use the `lazy` keyword (alias for System.Lazy) to ensure FsCheck only runs the test code when the guard check has passed.
`==>` is a custom operator in FsCheck.

```fsharp
[<Property>]
let ``Always has same number of letters`` (input:string) =
    input <> null ==> lazy
        let output = input |> flipCase
        input.Length = output.Length
```

### 41.3.2 Generators and arbitrary values
If a guard clause is not sufficient, you create a custom generator.