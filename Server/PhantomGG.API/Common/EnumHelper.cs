namespace PhantomGG.API.Common;

public static class EnumHelper
{
    public static T? ToEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        if (Enum.TryParse<T>(value, ignoreCase, out var result) && Enum.IsDefined(typeof(T), result))
        {
            return result;
        }

        return null;
    }
}