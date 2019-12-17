type Disk =
| HardDisk of RPM:int * Platters:int
| SolidState
| MMC of NumberOfPins:int

let describe disk =
    match disk with
    | SolidState -> "I'm a newfangled SSD"
    | MMC 1 -> "I have only 1 pin"
    | MMC pins when pins < 5 -> "I'm an MMC with a few pins"
    | MMC _ -> "I'm an MMC with pins"
    | HardDisk(5400, _) -> "I'm a slow hard disk"
    | HardDisk(_, 7) -> "I have 7 spindles"
    | HardDisk _ -> "I'm a hard disk"
    

// let disk = SolidState
// let disk = MMC 5
let disk = HardDisk(3200, 3)
describe disk