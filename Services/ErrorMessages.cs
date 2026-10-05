using MongoDB.Driver;

namespace TeamProjectPlanner.Services;

/// <summary>
/// Turns exceptions into messages that are safe and helpful to show users.
/// Technical details belong in the logs, never in the UI.
/// </summary>
public static class ErrorMessages
{
    public const string Generic = "Something went wrong. Please try again.";
    public const string DatabaseUnavailable = "We couldn't reach the database. Please check your connection and try again.";
    public const string Duplicate = "That item already exists.";
    public const string Forbidden = "You don't have permission to do that.";

    public static string ForUser(Exception exception) => exception switch
    {
        AppValidationException validation => validation.Message,
        MongoWriteException write when write.WriteError?.Category == ServerErrorCategory.DuplicateKey => Duplicate,
        UnauthorizedAccessException => Forbidden,
        TimeoutException or MongoConnectionException => DatabaseUnavailable,
        _ => Generic
    };
}
