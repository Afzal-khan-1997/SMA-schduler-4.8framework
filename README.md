# SMA Scheduler - .NET Framework 4.8

SMA Scheduler is a VB.NET Windows Forms project planner inspired by Microsoft Project. This edition targets .NET Framework 4.8 and opens in Visual Studio 2022.

## Included

- SMA Planner start form with live-project search, four schedule templates, recent schedules, and 20th-to-20th project counters.
- SMA Scheduler form with an editable task grid and custom Gantt timeline.
- New Project, BRE/ROL Update, BRE Within Update, and Feedback Update templates.
- FS, SS, FF, and SF task dependencies.
- Editable task dates, decimal durations, percent complete, multiple resources, assignment dates, and decimal resource hours.
- Weekend Plan option for scheduling Saturdays and Sundays.
- Capacity Planning workbook storage with an eight-hour daily limit.
- Project and monthly capacity Excel exports.
- Embedded animated planner pie chart and Gantt rendering.
- Persistent color themes shared by SMA Planner and SMA Scheduler.
- Local project library using `.smaschedule` files.
- SQL-ready repository and schema for a later database connection.

## Requirements

- Windows 10 or Windows 11.
- Visual Studio 2022 with the **.NET desktop development** workload.
- .NET Framework 4.8 Developer Pack.

## Open And Build

Open `SMAScheduler48.sln` in Visual Studio 2022 and select **Build > Build Solution**.

Command-line builds should use Visual Studio's full-framework MSBuild:

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" .\SMAScheduler48.sln /t:Build /p:Configuration=Debug
```

The Debug executable is written to:

```text
bin\Debug\SMAScheduler.exe
```

## Local Storage

Saved schedules are stored in:

```text
Documents\SMA Scheduler\Projects
```

Capacity workbooks are stored in:

```text
Documents\SMA Scheduler\Capacity Planning
```

The Save command also lets the user export the project plan and capacity workbook to a selected folder.

## SQL Server

SQL integration remains disabled until connection details and production table mappings are supplied. The integration boundary is in `SqlProjectRepository.vb`, and the planned schema is available in `schema.sql`.
