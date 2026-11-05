# 🅿️ Parking Permits

A Windows Forms app (C# · .NET 10 · SQL Server LocalDB) for managing a university's parking permits.
You can **list, search, filter, sort, add, edit and delete** permits, see an **overview dashboard**,
and look at a **drawn parking map** that shows which spots in each zone are taken.

![Main window](screenshots/01-main-window.png)

---

## Contents
1. [How to run it](#how-to-run-it)
2. [Features](#features)
3. [UML class diagram](#uml-class-diagram)
4. [Rules the app enforces](#rules-the-app-enforces)

---

## How to run it

1. In Visual Studio 2026: **View → SQL Server Object Explorer**
2. Right-click **(localdb)\MSSQLLocalDB → New Query**
3. Paste the contents of `ParkingPermits/database.sql` and press **Execute**
4. Open `ParkingPermits.sln`
5. **Build → Rebuild Solution**, then press **F5**

> The script can be run more than once: it only creates what is missing and inserts the 18 test permits only into an empty table.

---

## Features

### Loading screen

When the app starts, a short loading screen (about two seconds) shows the logo with a turning ring and a loading bar, then the main window opens.

![Loading screen](screenshots/00-splash.png)

### Permits page: find a permit and act on it

| Area | What it does |
|---|---|
| **Search box** | Finds permits by plate number *or* owner name while you type |
| **Type** | Shows only Student, Staff or Guest permits (works together with search) |
| **Sort by** | Plate A–Z · Owner A–Z · Expiring first · Expiring last |
| **Status tabs** | All · Active · Ending soon · Awaiting payment · Expired, each with a live count |
| **Coloured rows** | 🟩 valid · 🟨 ends within 30 days · 🟧 awaiting payment · 🟥 expired |
| **Details panel** | The selected permit: number plate, owner, status, a "days left" ring, Edit and Delete |

A search for **beridze** finds exactly three permits:

![Search result](screenshots/02-search-result.png)

### Adding and editing a permit

**+ New permit** opens an empty dialog. **Edit** (or a double-click on a row) opens the same dialog
already filled in. A parking-pass preview on the left updates as you type.
**Enter** saves and **Esc** cancels.

| Add | Edit |
|---|---|
| ![Add dialog](screenshots/03-add-dialog.png) | ![Edit dialog](screenshots/04-edit-dialog.png) |

After every add, edit or delete, a green notice confirms it in the top bar, and the grid, tabs,
overview and map update straight away. **Delete** always asks *Yes / No* first.

### Overview page: the big picture

Summary cards, a status chart, permits by type, spots taken in each zone, and a **Needs attention**
list of permits ending soon or waiting for payment. Click one to open it.

![Overview](screenshots/05-overview.png)

### Parking map page: who is parked where

Each zone is drawn as a car park with **8 spots**. A paid permit is a car (yellow when it ends within
30 days), an unpaid one is a dashed **reserved** spot, and the rest are **free**.
Hover over a spot to see who holds it; click it to open the permit.

![Parking map](screenshots/06-parking-map.png)

### Export to CSV

**Export CSV** saves the rows currently shown in the grid to a `.csv` file (UTF-8, opens correctly in Excel).
It only **reads** the list: nothing in the database is changed, added or deleted.

A sample export of all 18 permits (as of 5 Nov 2026) is in [`exports/permits-export.csv`](exports/permits-export.csv).

---

## UML class diagram

The database table `Permits` (in `ParkingPermitsDb`) and the classes of the project.
Only the main members of the forms are shown.

```mermaid
classDiagram
    direction TB

    class Permits {
        <<database table>>
        Id : INT IDENTITY PK
        PlateNumber : NVARCHAR[20]
        OwnerName : NVARCHAR[100]
        PermitType : NVARCHAR[20]
        Zone : NVARCHAR[10]
        ValidUntil : DATE
        IsPaid : BIT
        trg_Permits_PaidCannotBeUndone() trigger
    }

    class Permit {
        +Id : int
        +PlateNumber : string
        +OwnerName : string
        +PermitType : string
        +Zone : string
        +ValidUntil : DateTime
        +IsPaid : bool
        +/IsExpired : bool
        +/IsActive : bool
        +/TakesSpot : bool
        +/DaysLeft : int
        +/ExpiresSoon : bool
        +/Validity : string
        +/Payment : string
    }

    class PermitRepository {
        -connectionString : string
        -sortOrders : string[]
        +Search(text, permitType, sortIndex) List~Permit~
        +PlateExists(plateNumber, excludeId) bool
        +CountTakenSpots(zone, excludeId) int
        +Add(permit) void
        +Update(permit) void
        +Delete(id) void
        -EscapeLikeText(text)$ string
        -AddPermitParameters(command, permit) void
    }

    class ParkingLot {
        <<static>>
        +SpotsPerZone : int = 8$
        +Zones : string[] = A, B, C, D$
    }

    class MainForm {
        -repository : PermitRepository
        -allPermits : List~Permit~
        -searchResults : List~Permit~
        -shownPermits : List~Permit~
        -LoadPermits() void
        -ShowSelectedStatusTab() void
        -btnAdd_Click() void
        -EditSelectedPermit() void
        -btnDelete_Click() void
        -btnExport_Click() void
        -dgvPermits_CellFormatting() void
        -ShowPage(page) void
        ~GetStatusColor(permit)$ Color
    }

    class PermitForm {
        -repository : PermitRepository
        -permitId : int
        +CurrentPermit : Permit
        +PermitForm()
        +PermitForm(permit)
        -btnSave_Click() void
        -ShowError(message, control) void
        -pnlPreview_Paint() void
    }

    class SplashForm {
        -tmrAnimation : Timer
        +SplashForm()
        -tmrAnimation_Tick() void
        -SplashForm_Paint() void
    }

    class Program {
        <<static>>
        -Main()$ void
    }

    class GraphicsHelper {
        <<static>>
        +DrawLicensePlate()$ void
        +DrawBadge()$ void
        +DrawProgressRing()$ void
        +DrawCar()$ void
        +DrawParkingSign()$ void
    }

    class Palette {
        <<static>>
        colours of the whole app
    }

    class DetailsPalette {
        <<static>>
        colours of the details panel
    }

    class BufferedPanel
    class CardPanel {
        +SetCardColors(fill, border) void
    }
    class RoundedButton {
        -fadeTimer : Timer
        -hoverAmount : float
    }

    Form <|-- MainForm
    Form <|-- PermitForm
    Form <|-- SplashForm
    Panel <|-- BufferedPanel
    BufferedPanel <|-- CardPanel
    Button <|-- RoundedButton

    Program ..> SplashForm : shows first
    Program ..> MainForm : runs
    MainForm ..> PermitForm : opens for Add / Edit
    MainForm --> "1" PermitRepository
    PermitForm --> "1" PermitRepository
    MainForm o-- "0..*" Permit : shows
    PermitForm ..> Permit : creates
    PermitRepository ..> Permit : reads and writes
    PermitRepository ..> Permits : SQL with parameters
    MainForm ..> ParkingLot
    PermitForm ..> ParkingLot
    MainForm *-- CardPanel
    MainForm *-- RoundedButton
    PermitForm *-- RoundedButton
    MainForm ..> GraphicsHelper
    PermitForm ..> GraphicsHelper
    GraphicsHelper ..> Palette
    MainForm ..> DetailsPalette
```

The calculated properties of `Permit`: **IsExpired** (end date has passed), **IsActive** (paid and not expired),
**TakesSpot** (not expired, so it holds a spot), **DaysLeft** (0 if expired or unpaid), **ExpiresSoon**
(active and ends within 30 days), **Validity** (Valid · Expires soon · Awaiting payment · Expired) and
**Payment** (Paid · Unpaid).

---

## Rules the app enforces

**In the Add / Edit dialog** (a message explains the problem and the dialog stays open):
- Plate number is required and must look like `AB-123-CD`, and no two permits may share a plate.
- Owner is required and must be a first and last name in English letters, e.g. *Nino Beridze*.
- A new permit cannot end in the past, and no permit can run more than 2 years ahead.
- A zone with no free spot is refused: *"Zone C is full (8 of 8 spots taken)"*.
- Once a permit is paid, the *paid* box is locked.

**In the database:**
- A paid permit can never be changed back to unpaid; the trigger `trg_Permits_PaidCannotBeUndone` refuses it.

**Everywhere:**
- Everything the user types reaches SQL Server as a **parameter**, never as part of the SQL text.
  Searching for `' OR 1=1 --`, `%` or `_` finds nothing and causes no error.
- Every zone has the same number of spots, set in one place: `ParkingLot.SpotsPerZone` (8).
  A permit holds its spot until it expires.

> **Expiring first / last:** permits that can be used now come first, sorted by end date,
> then permits awaiting payment, then expired ones.
