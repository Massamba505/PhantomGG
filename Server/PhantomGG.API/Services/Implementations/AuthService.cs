using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        bool isEmailDuplicate = await _userRepository.EmailExistsAsync(request.Email);
        if (isEmailDuplicate)
        {
            throw new InvalidOperationException("Email already exists");
        }
            
        PasswordHashResult passwordHash = _passwordHasher.CreatePasswordHash(request.Password);
        
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash.Hash,
            PasswordSalt = passwordHash.Salt,
            ProfilePictureUrl = $"https://eu.ui-avatars.com/api/?name={request.FirstName}+{request.LastName}&size=250",
            Role = UserRoles.Organizer.ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        return user;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        bool isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        return user;
    }
}