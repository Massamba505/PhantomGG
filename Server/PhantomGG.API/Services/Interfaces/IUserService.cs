using PhantomGG.API.DTOs.User;

namespace PhantomGG.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
}
