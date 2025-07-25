CREATE TABLE Matches (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TournamentId UNIQUEIDENTIFIER NOT NULL,
    HomeTeamId UNIQUEIDENTIFIER NOT NULL,
    AwayTeamId UNIQUEIDENTIFIER NOT NULL,
    ScheduledTime DATETIME2 NOT NULL,
    Venue NVARCHAR(100) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'scheduled',
    HomeTeamScore SMALLINT DEFAULT 0,
    AwayTeamScore SMALLINT DEFAULT 0,

    CONSTRAINT FK_Match_Tournament FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Match_HomeTeam FOREIGN KEY (HomeTeamId) REFERENCES Teams(Id),
    CONSTRAINT FK_Match_AwayTeam FOREIGN KEY (AwayTeamId) REFERENCES Teams(Id),
    CONSTRAINT CK_Match_Status CHECK (Status IN ('scheduled', 'in_progress', 'completed', 'postponed', 'canceled')),
    CONSTRAINT CK_Match_Teams_Different CHECK (HomeTeamId <> AwayTeamId)
);
