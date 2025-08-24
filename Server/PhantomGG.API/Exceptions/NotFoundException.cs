namespace PhantomGG.API.Exceptions;

public class NotFoundException(string message) : DomainException(message) { }
