-- =============================================
-- Create Roles Table
-- Description: User roles for authorization
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(50) NOT NULL
    );

    -- Create indexes
    CREATE UNIQUE INDEX IX_Roles_Name ON Roles (Name);
END
GO
