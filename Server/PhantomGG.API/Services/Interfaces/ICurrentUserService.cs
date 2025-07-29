namespace PhantomGG.API.Services.Interfaces;

public interface ICurrentUserService
{
    public Guid? UserId { get; }
    public string? Email { get; }
    public string? Role {  get; }
}
