-- =============================================
-- Create RefreshTokens Table
-- Description: Store refresh tokens for JWT authentication
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE RefreshTokens (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Token NVARCHAR(500) NOT NULL,
        ExpiresAt DATETIME2 NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        RevokedAt DATETIME2 NULL,
        
        -- Foreign key constraints
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) 
            REFERENCES Users(Id) ON DELETE CASCADE
    );

    -- Create indexes for performance
    CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens (Token);
    CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens (UserId);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens (ExpiresAt);
    CREATE INDEX IX_RefreshTokens_RevokedAt ON RefreshTokens (RevokedAt);
END
GO
