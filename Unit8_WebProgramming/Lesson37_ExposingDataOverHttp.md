# Lesson 37 Exposing data over HTTP
## 37.2 Abstracting HTTP codes from F#
You might create a layer of abstraction from your application code and HTTP request.
Look at this section in the book for an idea of how.

## 37.3 Working with Async
The Web API has native support for working with tasks (async/await).
Let's see how to work with asynchronous data in F# by using F#'s `Async` type, and yet still interoperate with the Web API's support for tasks.
Look at `__.GetAnimal()`

### 37.4.1 Modeling web requests as functions
All though this section is about `Suave`, it had some good general info.

Web applications are a great fit for functional programming and F#
- web apps are by nature nearly always stateless; you take in a request and give back a response
- web apps often need to use asynchronous programming, which F# has excellent support for
- web apps - particularly the back end - are often data-centric