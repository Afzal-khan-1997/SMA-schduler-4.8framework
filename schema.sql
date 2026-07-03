/*
Core SQL objects for SMA Scheduler 4.8.

Expected existing master employee table:
    dbo.Employees(EmployeeId, EmployeeName) or dbo.Employees(EmployeeId, firstname)

The app now reads from SQL first in this order:
1. dbo.TaskTemplate
2. dbo.EmployeeDailyAvailability
3. dbo.TaskAssignment / dbo.SmaScheduleProjects / dbo.SmaScheduleTasks
*/

IF OBJECT_ID('dbo.TaskAssignment', 'U') IS NOT NULL
    DROP TABLE dbo.TaskAssignment;

IF OBJECT_ID('dbo.EmployeeDailyAvailability', 'U') IS NOT NULL
    DROP TABLE dbo.EmployeeDailyAvailability;

IF OBJECT_ID('dbo.TaskTemplate', 'U') IS NOT NULL
    DROP TABLE dbo.TaskTemplate;

IF OBJECT_ID('dbo.SmaScheduleTasks', 'U') IS NOT NULL
    DROP TABLE dbo.SmaScheduleTasks;

IF OBJECT_ID('dbo.SmaScheduleProjects', 'U') IS NOT NULL
    DROP TABLE dbo.SmaScheduleProjects;

IF OBJECT_ID('dbo.SmaResourceAvailability', 'U') IS NOT NULL
    DROP TABLE dbo.SmaResourceAvailability;

CREATE TABLE dbo.TaskTemplate
(
    TaskTemplateId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TemplateName NVARCHAR(200) NOT NULL,
    TaskOrder INT NOT NULL,
    TaskName NVARCHAR(300) NOT NULL,
    ProjectSize NVARCHAR(50) NULL,
    ProjectType NVARCHAR(50) NULL,
    DefaultHours DECIMAL(12,2) NOT NULL DEFAULT 0,
    DependencyTaskOrder INT NULL,
    DependencyType NVARCHAR(2) NULL,
    ModuleId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.EmployeeDailyAvailability
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    EmployeeId INT NOT NULL,
    WorkDate DATE NOT NULL,
    DefaultHours DECIMAL(5,2) NOT NULL DEFAULT 8,
    LeaveHours DECIMAL(5,2) NOT NULL DEFAULT 0,
    AvailableHours DECIMAL(5,2) NOT NULL DEFAULT 8,
    EntryType NVARCHAR(50) NULL,
    Remarks NVARCHAR(300) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy NVARCHAR(100) NULL,
    CONSTRAINT UQ_EmployeeDailyAvailability UNIQUE(EmployeeId, WorkDate)
);

CREATE TABLE dbo.TaskAssignment
(
    AssignmentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProjectId INT NOT NULL,
    ProjectName NVARCHAR(200) NOT NULL,
    VersionNumber NVARCHAR(50) NULL,
    ProjectSize NVARCHAR(50) NULL,
    ProjectType NVARCHAR(50) NULL,
    TaskId INT NOT NULL,
    TaskName NVARCHAR(300) NOT NULL,
    TaskOrder INT NOT NULL,
    EmployeeId INT NOT NULL,
    WorkDate DATE NOT NULL,
    AssignedHours DECIMAL(12,2) NOT NULL DEFAULT 0,
    StartDate DATE NULL,
    FinishDate DATE NULL,
    DependencyTaskOrder INT NULL,
    DependencyType NVARCHAR(2) NULL,
    Remarks NVARCHAR(300) NULL,
    UpdatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy NVARCHAR(100) NULL
);

CREATE TABLE dbo.SmaScheduleProjects
(
    ProjectId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProjectName NVARCHAR(200) NOT NULL UNIQUE,
    ProjectVersion NVARCHAR(50) NOT NULL DEFAULT '1.0',
    ProjectSize NVARCHAR(50) NULL,
    ProjectType NVARCHAR(50) NULL,
    TotalProjectHours DECIMAL(12,2) NOT NULL DEFAULT 0,
    ResourcesNeeded INT NOT NULL DEFAULT 0,
    ResourceHours DECIMAL(12,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.SmaScheduleTasks
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProjectId INT NOT NULL,
    TaskId INT NOT NULL,
    DatabaseTaskId INT NULL,
    TaskName NVARCHAR(300) NOT NULL,
    StartDate DATE NOT NULL,
    DurationDays DECIMAL(12,3) NOT NULL,
    FinishDate DATE NOT NULL,
    PercentComplete INT NOT NULL,
    Predecessors NVARCHAR(200) NULL,
    DependencyType NVARCHAR(2) NOT NULL DEFAULT 'FS',
    AssignedTo NVARCHAR(200) NULL,
    AssignmentDate DATE NULL,
    ResourceNames NVARCHAR(500) NULL,
    ResourceAllocations NVARCHAR(1000) NULL,
    DailyResourceAllocations NVARCHAR(MAX) NULL,
    ResourceHours DECIMAL(12,2) NOT NULL DEFAULT 0,
    ModuleId INT NULL,
    PlannerTaskId NVARCHAR(200) NULL,
    CONSTRAINT FK_SmaScheduleTasks_Project FOREIGN KEY(ProjectId) REFERENCES dbo.SmaScheduleProjects(ProjectId),
    CONSTRAINT UQ_SmaScheduleTasks_Project_Task UNIQUE(ProjectId, TaskId)
);

CREATE TABLE dbo.SmaResourceAvailability
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ResourceName NVARCHAR(200) NOT NULL,
    WorkDate DATE NOT NULL,
    AvailableHours DECIMAL(12,2) NOT NULL DEFAULT 8,
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_SmaResourceAvailability UNIQUE(ResourceName, WorkDate)
);
