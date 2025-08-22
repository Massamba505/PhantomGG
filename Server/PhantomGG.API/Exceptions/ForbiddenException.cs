namespace PhantomGG.API.Exceptions;

public class ForbiddenException(string message) : DomainException(message) { }
