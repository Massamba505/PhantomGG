using NUnit.Framework;
using FluentAssertions;
using PhantomGG.Service.Infrastructure.Security.Implementations;

namespace PhantomGG.UnitTests.Infrastructure;

[TestFixture]
public class PasswordHasherTests
{
    private BcryptPasswordHasher _passwordHasher;

    [SetUp]
    public void Setup()
    {
        _passwordHasher = new BcryptPasswordHasher();
    }

    [Test]
    public void HashPassword_ValidPassword_ReturnsHash()
    {
        var password = "TestPassword123!";
        var hash = _passwordHasher.HashPassword(password);
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Test]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "TestPassword123!";
        var hash = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(password, hash);
        result.Should().BeTrue();
    }

    [Test]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = _passwordHasher.HashPassword(password);
        var result = _passwordHasher.VerifyPassword(wrongPassword, hash);
        result.Should().BeFalse();
    }
}
