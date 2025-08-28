namespace PhantomGG.API.DTOs.User;

public class UserActivityDto
{
    public List<ActivityItemDto> Activities { get; set; } = new();
    public int TotalCount { get; set; }
}

public class ActivityItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? EntityId { get; set; }
}
