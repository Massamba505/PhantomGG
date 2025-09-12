namespace PhantomGG.Service.Exceptions;

public class NotFoundException(string message) : DomainException(message) { }
