using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;

namespace PhantomGG.API.Services.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<User> LoginAsync(LoginRequest request);
}