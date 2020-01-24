namespace Lesson37.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging

type Animal = { Name : string; Species : string }

[<ApiController>]
[<Route("[controller]")>]
type AnimalController (logger : ILogger<AnimalController>) =
    inherit ControllerBase()

    [<HttpGet>]
    member __.Get() =
        logger.LogInformation "getting animals"
        [
            { Name = "Fido"; Species = "Dog" }
            { Name = "Felix"; Species = "Cat"}
        ]

    [<HttpGet>]
    [<Route("one")>]
    member __.GetAnimal() =
         logger.LogInformation "getting animal"
         async { 
                return { Name = "Fido"; Species = "Dog" } 
             } |> Async.StartAsTask