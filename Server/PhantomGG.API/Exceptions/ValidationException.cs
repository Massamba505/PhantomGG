namespace PhantomGG.API.Exceptions;

public class ValidationException(string message) : DomainException(message) { }