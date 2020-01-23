# Lesson 34 Using Type Providers in the real world
## 34.1 Securely accessing connection strings with type providers
In previous lessons we learned how to point to live data sources in a type provider, by redirecting from a static data source to a remote one at runtime.
But sometimes we need a secret key at runtime - connection string, username and password, etc.

### 34.1.1 Working with configuration files
Many type providers have the ability to use _application configuration files_.
Immediate benefits are:
- config files are a normal part of .NET
- config file values can be replaced at deployment or runtime without any changes to the application code or binaries
- no secure strings reside in the code base

### 34.1.2 Problems with configuration files
Although config files are well supported in .NET, a couple of issues may arise with config files that make it difficult to work with them and type providers.
This is particularly true when using config files as the source for both compile-time and runtime data.

If you are working in a script and try to bind to a type provider driven from a connection string (or `#load` a file containing code that does this), it is likely that the type provider will try to search within `FSI.exe.config` rather than your application config file.
This is because F# Interactive has its own app.config file.

To aid in this, separate your code that __operates__ on data from code responsible for directly retrieving data.
In some ways this is a good practice because it reduces coupling between your business application and the source of data for them, which in turn makes testability easier (and using them from scipts easier).

## 34.2 Manually passing connection strings
The second options is more like what was discussed in previous lessons: using a static, hardcoded connection for compile-time code, and redirecting to a secure connection at runtime.

In this way, you control your application at compile time through a well-known sample dataset.
Then at runtime, you (optionally) redirect to another data source such as a test database, integration server, etc.

This approach decouples your data access code from the retrieval of the connection string to the data source.

## 34.3 Continuous integration with type providers
### 34.3.1 Data as part of the CI process
You know that type providers generate types at compile time not through a custom tool, but through the F# compiler.
When you build, the type provider kicks in, accessing the data source from which it generates types that are used later in the compilation process.

Since CI servers build your code too, the build server also needs access to a valid data source in order to compile your code.

## Best Practices
Always remember that type providers nearly always have two modes of operation - compile time and runtime - and that you can usually redirect type providers to a different data source at runtime.

Author's advice:
| Compile time | Runtime | effort | best for |
| --- | --- | --- | --- |
| Literal Values | Literal Values | very easy | simple systems, scripts, fixed data sources |
| config file | config file | easy | simple redirection, improved security |
| literal values | function argument | medium | script drivers,  large teams, full control |