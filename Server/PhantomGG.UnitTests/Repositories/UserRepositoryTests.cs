using NUnit.Framework;
using FluentAssertions;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Implementations;
using PhantomGG.UnitTests.Helpers;
using PhantomGG.Common.Enums;

namespace PhantomGG.UnitTests.Repositories;

[TestFixture]
public class UserRepositoryTests
{
    private PhantomContext _context;
    private UserRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new UserRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_ValidUser_ReturnsCreatedUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = (int)UserRoles.User,
            EmailVerified = false,
            ProfilePictureUrl = "",
            IsActive = true
        };

        await _repository.CreateAsync(user);
        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result.Email.Should().Be("john@example.com");
    }

    [Test]
    public async Task GetByIdAsync_ExistingUser_ReturnsUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = (int)UserRoles.User,
            EmailVerified = false,
            ProfilePictureUrl = "",
            IsActive = true
        };
        await _repository.CreateAsync(user);

        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result.Email.Should().Be("john@example.com");
    }

    [Test]
    public async Task GetByEmailAsync_ExistingUser_ReturnsUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = (int)UserRoles.User,
            EmailVerified = false,
            ProfilePictureUrl = "",
            IsActive = true
        };
        await _repository.CreateAsync(user);

        var result = await _repository.GetByEmailAsync("john@example.com");

        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
    }

    [Test]
    public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = (int)UserRoles.User,
            EmailVerified = false,
            ProfilePictureUrl = "",
            IsActive = true
        };
        await _repository.CreateAsync(user);

        var result = await _repository.EmailExistsAsync("john@example.com");

        result.Should().BeTrue();
    }

    [Test]
    public async Task EmailExistsAsync_NonExistingEmail_ReturnsFalse()
    {
        var result = await _repository.EmailExistsAsync("nonexistent@example.com");

        result.Should().BeFalse();
    }

    [Test]
    public async Task UpdateAsync_ExistingUser_ReturnsUpdatedUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash123",
            Role = (int)UserRoles.User,
            EmailVerified = false,
            ProfilePictureUrl = "",
            IsActive = true
        };
        await _repository.CreateAsync(user);

        user.FirstName = "Jane";
        await _repository.UpdateAsync(user);
        var result = await _repository.GetByIdAsync(user.Id);

        result.FirstName.Should().Be("Jane");
    }
}
