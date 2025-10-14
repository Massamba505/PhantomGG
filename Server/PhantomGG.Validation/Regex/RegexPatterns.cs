namespace PhantomGG.Validation.Regex;

public static class RegexPatterns
{
    public const string FullNamePattern = @"^[A-Za-zÀ-ÿ'-]{2,50}$";
    public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#+\-_])[A-Za-z\d@$!%*?&#+\-_]{8,}$";
    public const string OnlyLettersPattern = @"^[A-Za-z\s'-]+$";
    public const string AlphanumericPattern = @"^[A-Za-z0-9]+$";
}
