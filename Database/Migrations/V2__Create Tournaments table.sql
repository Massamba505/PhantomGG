    CREATE TABLE Tournaments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NOT NULL,
    Format NVARCHAR(20) NOT NULL,
    OrganizerId UNIQUEIDENTIFIER NOT NULL,
    IsPublic BIT NOT NULL DEFAULT 1,
    StartDate DATE NOT NULL,
    RegistrationDeadline DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'upcoming',
    BannerImageUrl NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT CK_Tournament_Format CHECK (Format IN ('knockout', 'league')),
    CONSTRAINT CK_Tournament_Status CHECK (Status IN ('upcoming', 'ongoing', 'completed', 'canceled')),
    CONSTRAINT FK_Tournament_Organizer FOREIGN KEY (OrganizerId) REFERENCES Users(Id) ON DELETE CASCADE
);
