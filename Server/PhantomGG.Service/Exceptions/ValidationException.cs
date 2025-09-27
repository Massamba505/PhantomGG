namespace PhantomGG.Service.Exceptions;

public class ValidationException(string message) : DomainException(message) { }