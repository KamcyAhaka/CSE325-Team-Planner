namespace TeamProjectPlanner.Services;

/// <summary>Thrown when user input breaks a business rule. The message is safe to show to users.</summary>
public class AppValidationException : Exception
{
    public AppValidationException(string message)
        : this(new[] { message })
    {
    }

    public AppValidationException(IReadOnlyList<string> errors)
        : base(string.Join(" ", errors))
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }
}
