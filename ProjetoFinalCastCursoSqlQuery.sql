IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CourseCategories] (
    [Id] int NOT NULL IDENTITY,
    [CategoryName] nvarchar(max) NULL,
    CONSTRAINT [PK_CourseCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [RoleName] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Courses] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [StudentsQuantity] int NOT NULL,
    [CourseCategoryId] int NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Courses_CourseCategories_CourseCategoryId] FOREIGN KEY ([CourseCategoryId]) REFERENCES [CourseCategories] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeName] nvarchar(max) NULL,
    [UserName] nvarchar(max) NULL,
    [PasswordHash] varbinary(max) NULL,
    [PasswordSalt] varbinary(max) NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CourseLogs] (
    [Id] int NOT NULL IDENTITY,
    [InclusionDate] datetime2 NOT NULL,
    [LastUpdateDate] datetime2 NOT NULL,
    [CourseId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_CourseLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CourseLogs_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseLogs_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CourseLogs_CourseId] ON [CourseLogs] ([CourseId]);
GO

CREATE INDEX [IX_CourseLogs_EmployeeId] ON [CourseLogs] ([EmployeeId]);
GO

CREATE INDEX [IX_Courses_CourseCategoryId] ON [Courses] ([CourseCategoryId]);
GO

CREATE INDEX [IX_Employees_RoleId] ON [Employees] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211114210935_InitialCreate', N'5.0.12');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Courses] DROP CONSTRAINT [FK_Courses_CourseCategories_CourseCategoryId];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'StudentsQuantity');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Courses] ALTER COLUMN [StudentsQuantity] int NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'CourseCategoryId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Courses] ALTER COLUMN [CourseCategoryId] int NULL;
GO

ALTER TABLE [Courses] ADD [CourseName] nvarchar(max) NULL;
GO

ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_CourseCategories_CourseCategoryId] FOREIGN KEY ([CourseCategoryId]) REFERENCES [CourseCategories] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211117133045_incluindoCampoNomeCurso', N'5.0.12');
GO

COMMIT;
GO