CREATE TABLE Tournaments (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    Name VARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    Location VARCHAR(200) NULL,
    FormatId UNIQUEIDENTIFIER NOT NULL,
    RegistrationStartDate DATETIME2 NULL,
    RegistrationDeadline DATETIME2 NULL,
    StartDate DATETIME2 NOT NULL,
    MinTeams INT NOT NULL DEFAULT 2,
    MaxTeams INT NOT NULL DEFAULT 16,
    MaxPlayersPerTeam INT NOT NULL DEFAULT 11,
    MinPlayersPerTeam INT NOT NULL DEFAULT 7,
    EntryFee DECIMAL(10,2) NULL DEFAULT 0.00,
    PrizePool DECIMAL(10,2) NULL DEFAULT 0.00,    
    ContactEmail VARCHAR(100) NULL,
    BannerUrl VARCHAR(MAX) NULL,
    LogoUrl VARCHAR(MAX) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Draft',
    MatchDuration INT NULL DEFAULT 90,    
    OrganizerId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsPublic BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Tournaments PRIMARY KEY (Id),
    CONSTRAINT FK_Tournaments_Organizer FOREIGN KEY (OrganizerId) 
        REFERENCES Users (Id),
    CONSTRAINT FK_Tournaments_Format FOREIGN KEY (FormatId) 
        REFERENCES TournamentFormats (Id),
);

CREATE INDEX IX_Tournaments_Status ON Tournaments (Status);
CREATE INDEX IX_Tournaments_Organizer ON Tournaments (OrganizerId);
CREATE INDEX IX_Tournaments_StartDate ON Tournaments (StartDate);
CREATE INDEX IX_Tournaments_IsActive ON Tournaments (IsActive);
CREATE INDEX IX_Tournaments_IsPublic ON Tournaments (IsPublic);
CREATE INDEX IX_Tournaments_FormatId ON Tournaments (FormatId);