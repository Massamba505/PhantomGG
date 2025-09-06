CREATE TABLE TournamentStandings (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    MatchesPlayed INT NOT NULL DEFAULT 0,
    Wins INT NOT NULL DEFAULT 0,
    Draws INT NOT NULL DEFAULT 0,
    Losses INT NOT NULL DEFAULT 0,
    GoalsFor INT NOT NULL DEFAULT 0,
    GoalsAgainst INT NOT NULL DEFAULT 0,
    Points INT NOT NULL DEFAULT 0,
    Position INT NULL,
    
    CONSTRAINT PK_TournamentStandings PRIMARY KEY (Id),
    CONSTRAINT FK_TournamentStandings_Tournament FOREIGN KEY (TournamentId)
        REFERENCES Tournaments (Id) ON DELETE CASCADE,
    CONSTRAINT FK_TournamentStandings_Team FOREIGN KEY (TeamId)
        REFERENCES Teams (Id) ON DELETE CASCADE,
);

CREATE INDEX IX_TournamentStandings_TournamentId ON TournamentStandings (TournamentId);
CREATE INDEX IX_TournamentStandings_Points ON TournamentStandings (Points DESC);