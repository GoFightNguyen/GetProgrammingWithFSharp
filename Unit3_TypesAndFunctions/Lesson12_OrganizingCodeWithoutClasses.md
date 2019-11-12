# Lesson 12 Organizing Code without Classes
In F#, since we are typically using stateless functions operating over immutable data, follow these default rules for organizing code:
- place related types together in namespaces
- place related stateless functions together in modules

## 12.1 Using namespaces and modules
### 12.1.1 Namespaces in F#
Namespaces are essentially identical to those in C# in terms of functionality.
Namespaces _logically_ organize data types, such as records, as well as modules.

### 12.1.2 Modules in F#
Modules hold `let`-bound functions.
In F#, modules can also be used like namespaces in that they can store types as well.
You can think of F# modules:
- like static classes in C#, or
- like namespaces but can also store functions

Create a module for a file by using the `module <myModule>` declaration at the top of the file.

You can declare both the namespace and module simultaneously.
`module MyApp.BusLogic.DataAccess` means module DataAccess resides in the MyApp.BusLogic namespace.

### 12.1.4 Opening modules
It can be more natural to think of modules as namespaces that happen to also store functions because modules can be _opened_, like namespaces.
This is useful when you want to stop referring to the module name in order to access types or functions.
Instead, you can call the functions directly.

### 12.1.5 namespaces vs modules
Modules are not a complete replacement for namespaces.
Namespaces can span multiple files, modules cannot.
Use namespaces to logically group types and modules.
Use modules primarily to store functions, and secondly to store types that are tightly related to those functions.

## 12.3 Tips for working with modules and namespaces
### 12.3.1 Access modifiers
By default, types and functions are public.
You can mark them private.

### 12.3.2 The global namespace
If you do not supply a parent namespace when declaring namespaces or modules, it appears in the `global` namespace, which is always open.

### 12.3.3 Automatic opening of modules
You can also have a module automatically open, without the caller explicitly having to use an open declaration, by adding the `[<AutoOpen>]` attribute on the module.
With this attribute applied, opening the parent namespace in the module will automatically open access to the module as well.

### 12.3.4 Scripts
Some of the preceding rules work differently with scripts.
You can create `let`-bound functions directly in a script.
An _implicit_ module is created for you based on the name of the script.