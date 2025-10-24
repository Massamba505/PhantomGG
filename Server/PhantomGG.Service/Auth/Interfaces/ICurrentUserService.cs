using PhantomGG.Models.DTOs.User;

namespace PhantomGG.Service.Auth.Interfaces;

public interface ICurrentUserService
{
    CurrentUserDto? GetCurrentUser();
    bool IsAuthenticated();
    bool IsInRole(string role);
}
