using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.Security.Interfaces;

public interface ICurrentUserService
{
    CurrentUserDto GetCurrentUser();
    bool IsAuthenticated();
    bool IsInRole(string role);
}
