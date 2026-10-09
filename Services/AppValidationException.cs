namespace TeamProjectPlanner.Services;

/// <summary>Thrown when user input breaks a business rule. The message is safe to show to users.</summary>
public class AppValidationException : Exception
{
    /// <summary>Creates the exception with a single validation message.</summary>
    public AppValidationException(string message)
        : this(new[] { message })
    {
    }

    /// <summary>Creates the exception from several messages, joined into one user-facing string.</summary>
    public AppValidationException(IReadOnlyList<string> errors)
        : base(string.Join(" ", errors))
    {
        Errors = errors;
    }

    /// <summary>The individual validation messages that failed.</summary>
    public IReadOnlyList<string> Errors { get; }
}
