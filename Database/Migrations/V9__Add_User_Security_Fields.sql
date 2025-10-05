ALTER TABLE Users ADD 
    EmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationToken VARCHAR(255) NULL,
    EmailVerificationTokenExpiry DATETIME2 NULL,
    PasswordResetToken VARCHAR(255) NULL,
    PasswordResetTokenExpiry DATETIME2 NULL,
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    AccountLockedUntil DATETIME2 NULL,
    LastLoginAt DATETIME2 NULL;

CREATE INDEX IX_Users_EmailVerificationToken ON Users (EmailVerificationToken);
CREATE INDEX IX_Users_PasswordResetToken ON Users (PasswordResetToken);
CREATE INDEX IX_Users_AccountLockedUntil ON Users (AccountLockedUntil);
