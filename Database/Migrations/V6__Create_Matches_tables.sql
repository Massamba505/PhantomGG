CREATE TABLE Matches (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    HomeTeamId UNIQUEIDENTIFIER NOT NULL,
    AwayTeamId UNIQUEIDENTIFIER NOT NULL,
    MatchDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 1,
    HomeScore INT NULL DEFAULT 0,
    AwayScore INT NULL DEFAULT 0,
    
    CONSTRAINT PK_Matches PRIMARY KEY (Id),
    CONSTRAINT FK_Matches_Tournament FOREIGN KEY (TournamentId)
        REFERENCES Tournaments (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Matches_HomeTeam FOREIGN KEY (HomeTeamId)
        REFERENCES Teams (Id),
    CONSTRAINT FK_Matches_AwayTeam FOREIGN KEY (AwayTeamId)
        REFERENCES Teams (Id)
);

CREATE INDEX IX_Matches_TournamentId ON Matches (TournamentId);
CREATE INDEX IX_Matches_MatchDate ON Matches (MatchDate);
CREATE INDEX IX_Matches_Status ON Matches (Status);