# Lesson 38 Consuming HTTP data
F# provides libraries for working with HTTP data quickly and easily.
We will explore three options.

## 38.1 Using FSharp.Data to work with HTTP endpoints
FSharp.Data's JSON type provider works well at consuming HTTP endpoints exposing JSON data.

```fsharp
open FSharp.Data

type AllAnimalsResponse = JsonProvider<"https://localhost:5001/animal">
let names = AllAnimalsResponse.GetSamples()
|> Seq.map(fun a -> a.Name)
|> Seq.toArray

type GetAnimalResponse = JsonProvider<"https://localhost:5001/animal/Felix">
let getAnimal =
    sprintf "https://localhost:5001/animal/%s"
    >> GetAnimalResponse.Load
getAnimal "Fido"
```

Using FSharp.Data does have a few restrictions.
It does not provide total control over things such as HTTP headers.
It works only with JSON data.
Contains no mechanisms for discovering routes; you must know them already and you must create a separate type for each type of data exposed.
There is no built-in way of handling various response codes.
You need to write code to wrap a call within a try/with block and convert it into a discriminated union (for example, Result<'T>).

```fsharp
type Result<'TSuccess> = Success of 'TSuccess | Failure of exn

// combinator function to wrap any code in a try/with block and convert to Result
let ofFunc code =
    try code() |> Success
    with | ex -> Failure ex
let getAnimalSafe animal =
    (fun () ->
        sprintf "https://localhost:5001/animal/%s" animal
        |> GetAnimalResponse.Load
    )
    |> ofFunc
let frodo = getAnimalSafe "NoSuchAnimal"
```

## 38.2 Working with HTTP.fs
HTTP.fs is a package enabling close control over both creating requests and processing responses.
You use a pipeline to create a request.
The response is returned as part of an object also containing any headers, cookies, the content length, and the response code.

```fsharp
open HttpClient

createRequest Get "https://localhost:5001/animal"
|> withCookie { name = "Foo"; value = "Bar" }
|> withHeader (ContentType "test/json")
|> withKeepAlive true
|> getResponse
```

Example output:
```fsharp
{
    StatusCode = 200;
    EntityBody = Some "[{"Name":"Fido","Species":"Dog"},{"Name":"Felix","Species":"Cat"}]";
    ContentLength = 66L;
    Cookies = map [];
    Headers =
        Map
        [(ContentTypeResponse, "application/json; charset=utf-8");
        (DateResponse, "Mon, 16 Jan 2017 15:34:02 GMT");
        (Server, "Microsoft-IIS/10.0"); (NonStandard "X-Powered-By", "ASP.NET");
        (NonStandard "X-SourceFiles", <elided>)];
}
```

Since HTTP.fs is a library, and not a framework, you can use it however you want.

Downsides.
You usually create a wrapper facade around HTTP.fs so you do not expose its low-level outputs.
No provided types; you must create your own types, share types across both server and client, or use something like the JSON type provider to give you types based on sample JSON.

## 38.3 Using the Swagger type provider
Swagger/Swashbuckle.

The Swagger type provider can generate a full API from the Seagger endpoint.
```fsharp
open SwaggerProvider
type SwaggerAnimal = SwaggerProvider<"https://localhost:5001/animal">
let api = SwaggerAnimal()
```

The automatic API and type generation from this provider are great, you get automatic route discovery and type generation.
The only requirement is the source must expose an accurate schema via Swagger.