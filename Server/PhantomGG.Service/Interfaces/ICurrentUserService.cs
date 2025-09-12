using PhantomGG.Models.DTOs.User;

namespace PhantomGG.Service.Interfaces;

public interface ICurrentUserService
{
    CurrentUserDto GetCurrentUser();
    bool IsAuthenticated();
    bool IsInRole(string role);
}
