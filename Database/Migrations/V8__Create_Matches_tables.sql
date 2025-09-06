CREATE TABLE Matches (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    Venue VARCHAR(200) NULL,
    HomeTeamId UNIQUEIDENTIFIER NOT NULL,
    AwayTeamId UNIQUEIDENTIFIER NOT NULL,
    MatchDate DATETIME2 NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Scheduled',
    HomeScore INT NULL,
    AwayScore INT NULL,
    
    CONSTRAINT PK_Matches PRIMARY KEY (Id),
    CONSTRAINT FK_Matches_Tournament FOREIGN KEY (TournamentId)
        REFERENCES Tournaments (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Matches_HomeTeam FOREIGN KEY (HomeTeamId)
        REFERENCES Teams (Id),
    CONSTRAINT FK_Matches_AwayTeam FOREIGN KEY (AwayTeamId)
        REFERENCES Teams (Id),
);

CREATE INDEX IX_Matches_TournamentId ON Matches (TournamentId);
CREATE INDEX IX_Matches_MatchDate ON Matches (MatchDate);