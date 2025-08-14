-- =============================================
-- Create Users Table
-- Description: Core user entity for authentication
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Email NVARCHAR(256) NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        ProfilePictureUrl NVARCHAR(255) NULL,
        EmailConfirmed BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LockoutEnd DATETIME2 NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        
        -- Foreign key constraint
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) 
            REFERENCES Roles(Id) ON DELETE NO ACTION
    );

    -- Create indexes for performance
    CREATE UNIQUE INDEX IX_Users_Email ON Users (Email);
    CREATE INDEX IX_Users_IsActive ON Users (IsActive);
    CREATE INDEX IX_Users_EmailConfirmed ON Users (EmailConfirmed);
    CREATE INDEX IX_Users_LockoutEnd ON Users (LockoutEnd);
    CREATE INDEX IX_Users_RoleId ON Users (RoleId);
END
GO
