DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @OrganizerRoleId UNIQUEIDENTIFIER = NEWID();
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID();

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetRoles
    (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES
    (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ORGANIZER')
BEGIN
    INSERT INTO AspNetRoles
    (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@OrganizerRoleId, 'Organizer', 'ORGANIZER', NEWID());
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'USER')
BEGIN
    INSERT INTO AspNetRoles
    (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES
    (@UserRoleId, 'User', 'USER', NEWID());
END
Go