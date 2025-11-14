using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Users.Implementations;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Security.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;
    private Mock<IImageService> _mockImageService = null!;
    private Mock<IUserValidationService> _mockUserValidationService = null!;
    private Mock<HybridCache> _mockCache = null!;
    private UserService _userService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockImageService = new Mock<IImageService>();
        _mockUserValidationService = new Mock<IUserValidationService>();
        _mockCache = new Mock<HybridCache>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockImageService.Object,
            _mockUserValidationService.Object,
            _mockCache.Object
        );
    }

    [Test]
    public async Task GIVEN_ValidUserAndEmailNotChanged_WHEN_UpdateProfileAsync_THEN_UpdatesUserSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "massamba@phantomgg.com",
            Role = (int)UserRoles.User,
            ProfilePictureUrl = "",
            IsActive = true
        };

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "massamba@phantomgg.com"
        };

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockCache
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        var result = await _userService.UpdateProfileAsync(userId, updateRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("Jane"));
        Assert.That(result.LastName, Is.EqualTo("Smith"));
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync($"user_{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GIVEN_NewEmailAlreadyExists_WHEN_UpdateProfileAsync_THEN_ThrowsConflictException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "massamba@phantomgg.com",
            Role = (int)UserRoles.User,
            ProfilePictureUrl = "",
            IsActive = true
        };

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "updatemassamba@phantomgg.com"
        };

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(x => x.EmailExistsAsync("updatemassamba@phantomgg.com"))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ConflictException>(
            async () => await _userService.UpdateProfileAsync(userId, updateRequest));

        Assert.That(ex!.Message, Is.EqualTo("Email address is already in use"));
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task GIVEN_ValidCurrentPassword_WHEN_ChangePasswordAsync_THEN_UpdatesPasswordSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "massamba@phantomgg.com",
            PasswordHash = "old-hashed-password",
            Role = (int)UserRoles.User,
            ProfilePictureUrl = "",
            IsActive = true
        };

        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!"
        };

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword("OldPassword123!", "old-hashed-password"))
            .Returns(true);

        _mockPasswordHasher
            .Setup(x => x.HashPassword("NewPassword123!"))
            .Returns("new-hashed-password");

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockCache
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        await _userService.ChangePasswordAsync(userId, changePasswordRequest);

        // Assert
        Assert.That(user.PasswordHash, Is.EqualTo("new-hashed-password"));
        _mockUserRepository.Verify(x => x.UpdateAsync(user), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync($"user_{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void GIVEN_InvalidCurrentPassword_WHEN_ChangePasswordAsync_THEN_ThrowsValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "massamba@phantomgg.com",
            PasswordHash = "old-hashed-password",
            Role = (int)UserRoles.User,
            ProfilePictureUrl = "",
            IsActive = true
        };

        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!"
        };

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.VerifyPassword("WrongPassword123!", "old-hashed-password"))
            .Returns(false);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            async () => await _userService.ChangePasswordAsync(userId, changePasswordRequest));

        Assert.That(ex!.Message, Is.EqualTo("Current password is incorrect"));
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task GIVEN_ValidFile_WHEN_UploadProfilePictureAsync_THEN_UploadsImageSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "massamba@phantomgg.com",
            ProfilePictureUrl = string.Empty,
            Role = (int)UserRoles.User,
            IsActive = true
        };

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockImageService
            .Setup(x => x.SaveImageAsync(mockFile.Object, ImageType.ProfilePicture, userId))
            .ReturnsAsync("https://phantomgg.com/profile.jpg");

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockCache
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        var result = await _userService.UploadProfilePictureAsync(userId, mockFile.Object);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProfilePictureUrl, Is.EqualTo("https://phantomgg.com/profile.jpg"));
        Assert.That(user.ProfilePictureUrl, Is.EqualTo("https://phantomgg.com/profile.jpg"));
        _mockImageService.Verify(x => x.SaveImageAsync(mockFile.Object, ImageType.ProfilePicture, userId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(user), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync($"user_{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GIVEN_ExistingProfilePicture_WHEN_UploadProfilePictureAsync_THEN_DeletesOldImageAndUploadsNew()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "massamba@phantomgg.com",
            ProfilePictureUrl = "https://phantomgg.com/old-profile.jpg",
            Role = (int)UserRoles.User,
            IsActive = true
        };

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ReturnsAsync(user);

        _mockImageService
            .Setup(x => x.DeleteImageAsync("https://phantomgg.com/old-profile.jpg"))
            .ReturnsAsync(true);

        _mockImageService
            .Setup(x => x.SaveImageAsync(mockFile.Object, ImageType.ProfilePicture, userId))
            .ReturnsAsync("https://phantomgg.com/new-profile.jpg");

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockCache
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        // Act
        var result = await _userService.UploadProfilePictureAsync(userId, mockFile.Object);

        // Assert
        Assert.That(result.ProfilePictureUrl, Is.EqualTo("https://phantomgg.com/new-profile.jpg"));
        _mockImageService.Verify(x => x.DeleteImageAsync("https://phantomgg.com/old-profile.jpg"), Times.Once);
        _mockImageService.Verify(x => x.SaveImageAsync(mockFile.Object, ImageType.ProfilePicture, userId), Times.Once);
    }

    [Test]
    public void GIVEN_InactiveUser_WHEN_UpdateProfileAsync_THEN_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "massamba@phantomgg.com"
        };

        _mockUserValidationService
            .Setup(x => x.ValidateUserIsActiveAsync(userId))
            .ThrowsAsync(new ForbiddenException("User account is not active. Please contact support"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _userService.UpdateProfileAsync(userId, updateRequest));

        Assert.That(ex!.Message, Does.Contain("User account is not active"));
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
