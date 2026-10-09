using MongoDB.Driver;

namespace TeamProjectPlanner.Services;

/// <summary>Turns exceptions into messages that are safe and helpful to show users; technical details belong in the logs.</summary>
public static class ErrorMessages
{
    /// <summary>Fallback message for failures with no more specific explanation.</summary>
    public const string Generic = "Something went wrong. Please try again.";

    /// <summary>Shown when the database cannot be reached or times out.</summary>
    public const string DatabaseUnavailable = "We couldn't reach the database. Please check your connection and try again.";

    /// <summary>Shown when a unique index rejects an insert (for example a duplicate email).</summary>
    public const string Duplicate = "That item already exists.";

    /// <summary>Shown when the current user is not allowed to perform the action.</summary>
    public const string Forbidden = "You don't have permission to do that.";

    /// <summary>Picks the user-facing message for an exception, keeping technical details out of the UI.</summary>
    public static string ForUser(Exception exception) => exception switch
    {
        AppValidationException validation => validation.Message,
        MongoWriteException write when write.WriteError?.Category == ServerErrorCategory.DuplicateKey => Duplicate,
        UnauthorizedAccessException => Forbidden,
        TimeoutException or MongoConnectionException => DatabaseUnavailable,
        _ => Generic
    };
}
