module BusinessLogicTests

open Xunit
open BusinessLogic

[<Fact>]
let isLargeAndYoungTeam_TeamIsLargeAndYoung_ReturnsTrue() =
    let department =
        {
            Name = "Super Team"
            Team = [ for i in 1 .. 15 -> { Name = sprintf "Person %d" i; Age = 19}]
        }
    Assert.True(department |> isLargeAndYoungTeam)

[<Fact>]
let ``Large, young teams are correctly identified``() =
    let department =
        {
            Name = "Super Team"
            Team = [ for i in 1 .. 15 -> { Name = sprintf "Person %d" i; Age = 19}]
        }
    Assert.True(department |> isLargeAndYoungTeam)