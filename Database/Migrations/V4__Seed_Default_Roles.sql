-- =============================================
-- Seed Default Roles
-- Description: Insert default system roles
-- =============================================

-- Insert default roles if they don't exist
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @ModeratorRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();

-- Admin Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES (@AdminRoleId, 'Admin');
END

-- Moderator Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Organizer')
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES (@ModeratorRoleId, 'Organizer');
END

-- User Role
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'User')
BEGIN
    INSERT INTO Roles (Id, Name)
    VALUES (@UserRoleId, 'User');
END
GO
