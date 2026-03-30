-- 1. Create database if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'Elastic')
BEGIN
    CREATE DATABASE Elastic;
END;
GO

-- 2. Switch to the Elastic database
USE Elastic;
GO

-- 3. Create Users table if it does not exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.tables 
    WHERE name = 'Users'
)
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER NOT NULL 
            CONSTRAINT PK_Users PRIMARY KEY 
            DEFAULT NEWID(),

        Firstname NVARCHAR(50) NOT NULL,
        Lastname NVARCHAR(50) NOT NULL,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END;
GO

-- 4. Create Outbox table if it does not exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.tables 
    WHERE name = 'OutboxEvents'
)
BEGIN
    CREATE TABLE OutboxEvents (
        Id UNIQUEIDENTIFIER NOT NULL 
            CONSTRAINT PK_OutboxEvents PRIMARY KEY 
            DEFAULT NEWID(),

        [Type] NVARCHAR(200) NOT NULL,

        Payload NVARCHAR(MAX) NOT NULL,

        OccurredOnUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

        ProcessedOnUtc DATETIME2 NULL,

        Error NVARCHAR(MAX) NULL
    );
END;
GO

-- 5. Create Payments table if it does not exist
IF NOT EXISTS (
    SELECT 1 
    FROM sys.tables 
    WHERE name = 'Payments'
)
BEGIN
    CREATE TABLE Payments (
        Id UNIQUEIDENTIFIER NOT NULL 
            CONSTRAINT PK_Payments PRIMARY KEY 
            DEFAULT NEWID(),

        Amount INT NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        Currency NVARCHAR(50) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END;
GO

-- 6. Create IdempotencyKeys table if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'IdempotencyKeys')
BEGIN
    CREATE TABLE IdempotencyKeys (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_IdempotencyKeys PRIMARY KEY DEFAULT NEWID(),
        IdempotencyKey NVARCHAR(450) NOT NULL,
        PaymentId UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_IdempotencyKey UNIQUE (IdempotencyKey),
        CONSTRAINT UQ_PaymentId UNIQUE (PaymentId)
    );
END;
GO

-- 7. Create Profiles table if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Profiles')
BEGIN
    CREATE TABLE Profiles (
        UserId UNIQUEIDENTIFIER NOT NULL,

        Bio NVARCHAR(250) NULL,

        CONSTRAINT PK_Profiles PRIMARY KEY (UserId),

        CONSTRAINT FK_Profiles_Users 
            FOREIGN KEY (UserId) 
            REFERENCES Users(Id)
            ON DELETE CASCADE
    );
END;
GO

-- 8. Create Languages table if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Languages')
BEGIN
    CREATE TABLE Languages (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        Proficiency NVARCHAR(50) NOT NULL,
        CONSTRAINT FK_Languages_User FOREIGN KEY (UserId) REFERENCES Users(Id)
);
END;
GO