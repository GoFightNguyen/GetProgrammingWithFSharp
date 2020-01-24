# Lesson 36 Asynchronous Workflows
Asynchronous workflows allow you to orchestrate asynchronous and multithreaded code in a manageable way.
These provide ways to parallelize code more easily, allowing you to create high-performing and scalable applications.

## 36.1 Comparing synchronous and asynchronous models
In simple terms
- synchronous work happens in a single, sequential flow of execution
- asynchronous work represents the notion of doing work in the background, and when it completes, receiving notification that the background work has finished, before consuming the output and continuing

### 36.1.1 Threads, Tasks, and I/O-bound workloads
Brief overview of some ways to perform background work in .NET

##### Threads
The lowest primitive for allocating background work is the thread.
A .NET app has a thread pool with a finite number of threads; when you execute work on a background thread, the thread pool assigns a thread to carry out the work.
When the work is finished, the thread pool reclaims the thread, ready for the next piece of background workd.

##### Tasks
Tasks are a higher-level abstraction over threads.
They are easier to work with and reason about, with good support for cancellation and parallelism.

##### I/O-bound workloads
Both threads and tasks can be thought of as supporting _CPU-bound workloads_ - work that is carried out in the current process.
I/O-bound workloads are background tasks you want to execute that __do not__ need a thread to run on.
Instead of using a thread from the pool (which both the thread and task models do), the OS provides a low-level callback that .NET monitors; when the external system returns with data, .NET picks up the response and resumes work.
These sorts of methods do not block threads while running.

## 36.2 Introducing asynchronous workflows
The async/await pattern you are used to from C# is based on F#'s asynchronous workflows - although async/await is not quite as flexible.

Threads and tasks are library features to schedule background work, but async workflow is a language-level feature to achieve the same thing.

### 36.2.1 Async workflow basics
The gist of async workflows: wrap any code block you want to execute in the background in an `async { }` block.

```fsharp
printfn "Loading data!"
System.Threading.Thread.Sleep(5000)
printfn "Loaded data!"
printfn "My name is Simon"

async {
    printfn "Loading data!"
    System.Threading.Thread.Sleep(5000)
    printfn "Loaded data!" }
|> Async.Start
printfn "My name is Simon"
```

The previous example was a fire-and-forget one.
Now asynchronously execute code that returns a value.

```fsharp
let asyncHello : Async<string> = async { return "Hello" }

// Compiler error
let length = asyncHello.Length

let text = asyncHello |> Async.RunSynchronously
let lengthTwo = text.Length
```

Some notes:
- unlike normal expressions, the result of an async expression must be prefixed with the `return` keyword
- `Async.RunSynchronously` is the equivalent of `Task.Result`; it blocks the current thread until the workflow is completed
- Unlike `Task.Result`, repeatedly calling `RunSynchronously` on an async block will re-execute the code every time

## 36.3 Composing asynchronous values
One way to unwrap an asynchronous value is to call `Async.RunSynchronously`.
But this blocks the current thread until the async workflow has executed.

F# has a built-in way to continue when a background workflow completes, use `let!` and `Async.Start`
```fsharp
let getTextAsync = async { return "Hello" }
let printHelloWorld =
    async {
        let! text : string = getTextAsync
        return printf "%s World" text
    }
printHelloWorld |> Async.Start
```

## 36.4 Using fork/join
fork/join: launch several async workflows in the background, wait until _all_ of them complete, and then continue with all the results combined.

`Async.Parallel` combines multiple workflows of the same type into a single workflow.
It collates a collection of async workflows into a single, combined workflow.
It does not start the new workflow.
Similar to `Task.WhenAll`.

```fsharp
let random = System.Random()
let pickANumberAsync = async { return random.Next(10) }
let createFiftyNumbers =
    let workflows = [ for i in 1 .. 50 -> pickANumberAsync ]

    async {
        let! numbers = workflows |> Async.Parallel
        printfn "Total is %d" (numbers |> Array.sum)
    }
createFiftyNumbers |> Async.Start
```

## 36.5 Using tasks and async workflows
Let's compare the `Async` type in F# with the .NET `Task` type.

### 36.5.1 Interoperating with tasks
F# has combinators (or transformation functions) allowing you to go between `Task` and `Async`.
- `Async.AwaitTask` converts a task into an async workflow
- `Async.StartAsTask` converts an async workflow into a task

```fsharp
let downloadData url = async {
    let! data =
        wc.DownloadDataTaskAsync(System.Uri url) |> Async.AwaitTask
    return data.Length }

let downloadedBytes =
    urls
    |> Array.map downloadData
    |> Async.Parallel
    |> Async.StartAsTask
printfn "You downloaded %d characters" (Array.sum downloadedBytes.Result)
```

### 36.5.2 Comparing tasks and async
| | Task and async/await | F# async workflows |
| --- | --- | --- |
| Native support in F# | yes | yes |
| Allows status reporting | yes | no |
| clarity | hard to know where async starts and stops | very clear |
| unification | Task and Task types | Unified Async<T> |
| statefulness | Task result evaluated only once | infinite |

General guidance: internally in your F# code use async workflows wherever possible.
You might want to use Tasks instead if:
- interop-ing
- extremely large numbers of CPU-bound items, where Task is more efficient

If using a library exposing tasks, you generally want to immediately convert them to asyncs so you can use `let!` on them within an async block.

### 36.5.3 useful async keywords
| command | usage |
| --- | --- |
| let! | used within an async block to unwrap an Async value to T |
| do! | used within an async block to wait for an Async<unit> to complete |
| return! | used within an async block as a shorthand for let! and return |
| Async.AwaitTask | converts Task<T> to Async<T>, or Task to Async<unit> |
| Async.StartAsTask | converts Async<T> to Task<T> |
| Async.RunSynchronously | synchronously unwraps Async<T> to <T> | 
| Async.Start | starts an Async<unit> computation in the background (fire-and-forget) |
| Async.Ignore | converts Async<T> to Async<unit> |
| Async.Parallel | converts Async<T> array to Async<T array> |
| Async.Catch | converts Async into a two-case DU of T or Exception |