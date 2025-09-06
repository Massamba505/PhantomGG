CREATE TABLE Teams (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    Name VARCHAR(200) NOT NULL,
    ShortName VARCHAR(10) NULL,
    ManagerName VARCHAR(100) NOT NULL,
    ManagerEmail VARCHAR(100) NOT NULL,
    ManagerPhone VARCHAR(10) NULL,
    LogoUrl VARCHAR(MAX) NULL,
    TeamPhotoUrl VARCHAR(MAX) NULL,
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    RegistrationStatus VARCHAR(20) NOT NULL DEFAULT 'Pending',
    RegistrationDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ApprovedDate DATETIME2 NULL,
    NumberOfPlayers INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Teams PRIMARY KEY (Id),
    CONSTRAINT FK_Teams_Tournament FOREIGN KEY (TournamentId) 
        REFERENCES Tournaments (Id) ON DELETE CASCADE,
);
CREATE INDEX IX_Teams_Name ON Teams (Name);
CREATE INDEX IX_Teams_ShortName ON Teams (ShortName);
CREATE INDEX IX_Teams_ManagerName ON Teams (ManagerName);
CREATE INDEX IX_Teams_TournamentId ON Teams (TournamentId);
CREATE INDEX IX_Teams_RegistrationStatus ON Teams (RegistrationStatus);
CREATE INDEX IX_Teams_IsActive ON Teams (IsActive);