namespace Phantoms.Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested entity cannot be found. 
/// The GlobalExceptionHandler maps this to HTTP 404.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException()
        : base("The requested resource was not found.") { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}
