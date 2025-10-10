CREATE TABLE Tournaments (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(200) NOT NULL,
    Location VARCHAR(200) NOT NULL,
    BannerUrl VARCHAR(MAX) NULL,
    LogoUrl VARCHAR(MAX) NULL,
    RegistrationStartDate DATETIME2 NOT NULL,
    RegistrationDeadline DATETIME2 NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    MinTeams INT NOT NULL,
    MaxTeams INT NOT NULL,
    Status INT NOT NULL DEFAULT 1,
    OrganizerId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL,
    IsPublic BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Tournaments PRIMARY KEY (Id),
    CONSTRAINT FK_Tournaments_Organizer FOREIGN KEY (OrganizerId) 
        REFERENCES Users (Id)
);

CREATE INDEX IX_Tournaments_Status ON Tournaments (Status);
CREATE INDEX IX_Tournaments_Name ON Tournaments (Name);
CREATE INDEX IX_Tournaments_Organizer ON Tournaments (OrganizerId);
CREATE INDEX IX_Tournaments_StartDate ON Tournaments (StartDate);
CREATE INDEX IX_Tournaments_IsPublic ON Tournaments (IsPublic);