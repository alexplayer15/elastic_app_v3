-- 1. Create database if it does not exist
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'Users')
BEGIN
    CREATE DATABASE Users;
END;
GO

-- 2. Switch to the Users database
USE Users;
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