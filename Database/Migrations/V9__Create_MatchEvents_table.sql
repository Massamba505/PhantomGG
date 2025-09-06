CREATE TABLE MatchEvents (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWID()),
    MatchId UNIQUEIDENTIFIER NOT NULL,
    EventType VARCHAR(20) NOT NULL,
    Minute INT NOT NULL,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    PlayerName VARCHAR(100) NULL,
    Description VARCHAR(500) NULL,
    
    CONSTRAINT PK_MatchEvents PRIMARY KEY (Id),
    CONSTRAINT FK_MatchEvents_Match FOREIGN KEY (MatchId)
        REFERENCES Matches (Id) ON DELETE CASCADE,
    CONSTRAINT FK_MatchEvents_Team FOREIGN KEY (TeamId)
        REFERENCES Teams (Id),
);

CREATE INDEX IX_MatchEvents_MatchId ON MatchEvents (MatchId);