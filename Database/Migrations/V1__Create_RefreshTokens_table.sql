IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Token NVARCHAR(100) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        RevokedAt DATETIME2 NULL,
        CreatedByIp NVARCHAR(50) NULL,
        RevokedByIp NVARCHAR(50) NULL,
        ReplacedByToken NVARCHAR(255) NULL,
        ReasonRevoked NVARCHAR(255) NULL,
        
        CONSTRAINT FK_RefreshTokens_AspNetUsers FOREIGN KEY (UserId) 
            REFERENCES AspNetUsers(Id) ON DELETE CASCADE
    );
    
    PRINT 'RefreshTokens table created successfully';
    
    -- Create index for faster token lookups
    CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
    PRINT 'Index on RefreshTokens.Token created';
    
    -- Create index for user tokens
    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
    PRINT 'Index on RefreshTokens.UserId created';
END
