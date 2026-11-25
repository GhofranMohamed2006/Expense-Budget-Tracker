namespace Pennywise.Common.Exceptions;

/// <summary>Base for expected business errors. Middleware maps them to ProblemDetails.</summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    protected AppException(string message, int statusCode) : base(message) => StatusCode = statusCode;
}

public class ValidationFailedException : AppException
{
    public IDictionary<string, string[]> Errors { get; }
    public ValidationFailedException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", 400) => Errors = errors;
    public ValidationFailedException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = new[] { message } }) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized.") : base(message, 401) { }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Forbidden.") : base(message, 403) { }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found.") : base(message, 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}
