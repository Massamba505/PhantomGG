CREATE TABLE TournamentTeams (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    Status INT NOT NULL DEFAULT 1,
    RequestedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    AcceptedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_TournamentTeams PRIMARY KEY (Id),
    CONSTRAINT FK_TournamentTeams_Tournament FOREIGN KEY (TournamentId) 
        REFERENCES Tournaments(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TournamentTeams_Team FOREIGN KEY (TeamId) 
        REFERENCES Teams(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_TournamentTeams_Unique UNIQUE (TournamentId, TeamId)
);

CREATE INDEX IX_TournamentTeams_Tournament ON TournamentTeams (TournamentId);
CREATE INDEX IX_TournamentTeams_Team ON TournamentTeams (TeamId);
CREATE INDEX IX_TournamentTeams_Status ON TournamentTeams (Status);