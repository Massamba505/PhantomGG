namespace PhantomGG.API.Exceptions;

public class UnauthorizedException(string message) : DomainException(message) { }