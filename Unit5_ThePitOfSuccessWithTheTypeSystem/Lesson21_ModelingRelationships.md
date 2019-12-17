# Lesson 21 Modeling relationships

## 21.1 Composition
```fsharp
// composition using records
type Disk = { SizeGb : int }
type Computer =
    { Manufacturer : string
      Disks: Disk list }

let myPc =
    { Manufacturer = "Computers Inc."
      Disks =
        [ { SizeGb = 100 }
          { SizeGb = 250 }
          { SizeGb = 500 } ] }
```

## 21.2 Discriminated Unions
The standard functional programming answer to modeling is-a relationships is by using __discriminated unions__.
aka sum types, case classes, or algebraic data types.
Think of discriminated unions in one of two ways:
- a normal type hierarchy, but _closed_.
  - All the different subtypes are defined up front; you cannot declare subtypes later
- a form of C#-style enums, but with the ability to add metadata to each enum case

```fsharp
type Disk =
| HardDisk of RPM:int * Platters:int
| SolidState
| MMC of NumberOfPins:int
```

### 21.2.1 Creating instaces of DUs
```fsharp
let myHardDisk = HardDisk(RPM = 250, Platters = 7)  //explicitly named args
let myHardDiskShort = HardDisk(250,7)   //lightweight syntax
let args = 250,7
let myHardDiskTupled = HardDisk args    // pass all values as a single arg
let myMMC = MMC 5
let mySsd = SolidState  // creating a DU case without metadata

// all values on the left side are typed as the base type (Disk), rather
// than the specific subtypes
```

### 21.2.2 Accessing an instance of a DU
You cannot dot into an instance of a DU (such as myHardDisk from previous example) to acces all the fields.
This is because the type of a DU instance is always the base type.
We must _unwrap_ the instance into a subtype.
To do this, we use pattern matching.

```fsharp
let seek disk =
    match disk with
    | HardDisk(5400, 5) -> "seeking very slowly"
    | HardDisk(rpm, _) -> sprintf "seeking at RPM %d" rpm
    | MMC _ -> "seeking quietly but slowly" // match on any type of MMC
    | SolidState -> "already found it"
```

In F#, you put all implementations in a single place.
Every time you add a new type, you must amend this function.
Seems bizarre, but has benefits:
- F# safely checks the type of the subclass for you
  - you cannot accidentally access a field not on the subtype
- you can use pattern matching to unbind specific values to variables
- exhaustive matching because of using pattern matching

Discriminated Unions represent a fixed type hierarchy; you can only create subtypes where the DU is defined.

Best Practice: Be as explicit as possible with match cases over discriminated unions.
Do not let your last case be simply `_`, otherwise if you add a new type to the DU, you will not be reminded to handle the new case.

## 21.3 Tips for working with discriminated unions
### 21.3.1 Nested DUs
A nested DU is a type of a type.
```fsharp
// the DU to be nested
type MMCDisk =
| RsMmc
| MmcPlus
| SecureMMC

// add the nested DU to the parent case
type Disk =
| MMC of MMCDisk * NumberOfPins:int

// match on top-level AND nested DU simultaneously 
match disk with
| MMC(MmcPlus, 3) -> "Seeking quietly but slowly"
| MMC(SecureMMC, 6) -> "Seeking quietly with 6 pins."
```

### 21.3.2 Shared fields
Sharing common fields is not supported with DUs; you canont put common fields on the base of the DU.
To "share" fields, use a combination of a record and a discriminated union.
Create a wrapper record to hold any common fields, plus one more field containing the discriminated union.

```fsharp
type Disk =
| HardDisk of RPM:int * Platters:int
| SolidState
| MMC of NumberOfPins:int

type DiskInfo =
    { Manufacturer : string
      SizeGb : int
      DiskData : Disk }

type Computer = { Manufacturer : string; Disks : DiskInfo list}
let myPc =
    { Manufacturer = "Computers Inc"
      Disks =
        [
            { 
                Manufacturer = "HardDisks Inc"
                SizeGb = 100
                DiskData = HardDisk(5400,7)
            }
            {
                Manufacturer = "SuperDisks Corp."
                SizeGb = 250
                DiskData = SolidState
            }
        ]}
```

### 21.3.3 Printing out DUs
To print out the contents of a DU in a human-readable form, call `sprintf "%A"` on a DU.
The compiler will pretty-print the entire case for you.
No need to manually match all cases.

## 21.4 More about discriminated unions
### 21.4.1 Comparing OO hierarchies and discriminated unions
If you need to have an extensible set of open, pluggable subtypes that can be dynamically added, discriminated unions are __not__ a great fit.

For DUs with a large number of cases (hundreds) that change quickly, a record or raw function might be a better fit, or falling back to a class-based inheritance model.

If you have a fixed (or slowly changing) set of cases, then a DU is the better fit.
- lightweight, easy to work with, and flexible since you add new behaviors without affecting the rest of your code base
- pattern matching
- easier to reason about since all implementations are in a single place

### 21.4.2 Creating enums
Example of creating a standard .NET enum.
```fsharp
type Printer = 
| Inkjet = 0
| Laserjet = 1
| DotMatrix = 2
```

You must give each case an explicit ordinal.
You cannot assoicate metadata with any case.

Although you can pattern match over an enum, enums cannot be exhaustively matched over since any int can be cast to an enum.
Therefore, you will always need a catchall wildcard handler to an enum pattern match.