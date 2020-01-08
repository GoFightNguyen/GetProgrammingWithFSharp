# Lesson 26 Working with NuGet Packages
## 26.1 Using NuGet with F#
NuGet packages work out of the box with F# projects.

### 26.1.2 Experimenting with scripts
The F# REPL can be used with NuGet packages to explore and test a NuGet package.
Use a script to test how a package works - in isolation - so that you can learn how to use it properly.
In the script, use the `#r` directive to reference the assembly of a NuGet package you downloaded.

### 26.1.3 Loading source files in scripts
Use the `#load` directive.
When loading in an .fs file, you must also load in
- assemblies it depends on
- other .fs files it depends on

### 26.1.4 Improving the referencing experience
When referencing multiple assemblies in your script, recall from Lesson 25 you can use the `#I` directive to add a directory as part of the search listing used by `#r`.
Therefore, if multiple assemblies are in the same location, you do not have to repeat the shared part of the path in each `#r`.

At the time of this book, there was consideration for creating a `#nuget` directive.

## 26.2 Working with Paket
Paket is an open source, flexible, and powerful dependency management client for .NET aiming to simplify dependency management.
It is backward-compatible with the NuGet service.

Many open-source F# projects use Paket.

Some benefits of Paket:
- keeps dependencies stable across all projects in your solution or repo; no mixed versions
- allows you to focus on the top-level dependencies, while it internally manages the children for you
- scripts work more easily with Paket-sourced dependencies
- doesn't couple you to Visual Studio