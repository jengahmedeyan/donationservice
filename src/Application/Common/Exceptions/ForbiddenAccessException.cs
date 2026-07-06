namespace Application.Common.Exceptions;

/// <summary>Thrown when a caller tries to access a resource they don't own. Mapped to HTTP 403.</summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("You are not allowed to access this resource.") { }
}
