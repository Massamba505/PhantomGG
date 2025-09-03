CREATE TABLE Tournaments (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    Name VARCHAR(200) NOT NULL,
    Description TEXT NOT NULL,
    Location VARCHAR(200) NULL,
    RegistrationDeadline DATETIME2 NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    MaxTeams INT NOT NULL DEFAULT 9,
    EntryFee DECIMAL(10,2) NULL DEFAULT 0.00,
    Prize DECIMAL(10,2) NULL DEFAULT 0.00,
    ContactEmail VARCHAR(100) NULL,
    BannerUrl VARCHAR(MAX) NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Active',
    Organizer UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT PK_Tournaments PRIMARY KEY (Id),
    CONSTRAINT FK_Tournaments_Organizer FOREIGN KEY (Organizer) 
        REFERENCES Users (Id)
);

CREATE INDEX IX_Tournaments_Status ON Tournaments (Status);
CREATE INDEX IX_Tournaments_Organizer ON Tournaments (Organizer);
CREATE INDEX IX_Tournaments_StartDate ON Tournaments (StartDate);
CREATE INDEX IX_Tournaments_IsActive ON Tournaments (IsActive);
