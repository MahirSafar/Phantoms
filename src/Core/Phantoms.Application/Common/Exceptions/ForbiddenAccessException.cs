namespace Phantoms.Application.Common.Exceptions;

/// <summary>
/// Thrown when a user attempts an action they don't have permission for.
/// The GlobalExceptionHandler maps this to HTTP 403.
/// </summary>
public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("You do not have permission to perform this action.") { }

    public ForbiddenAccessException(string message)
        : base(message) { }
}
