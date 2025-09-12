namespace PhantomGG.Service.Exceptions;

public class UnauthorizedException(string message) : DomainException(message) { }