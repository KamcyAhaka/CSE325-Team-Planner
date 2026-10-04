namespace TeamProjectPlanner.Services;

/// <summary>
/// Runs a UI action with consistent error handling: logs failures, shows a friendly
/// error toast, and optionally a success toast. Use it from interactive pages instead
/// of writing try/catch in every handler.
/// </summary>
public class UiActionRunner
{
    private readonly ToastService _toasts;
    private readonly ILogger<UiActionRunner> _logger;

    public UiActionRunner(ToastService toasts, ILogger<UiActionRunner> logger)
    {
        _toasts = toasts;
        _logger = logger;
    }

    /// <summary>Runs an action. Returns true when it succeeded.</summary>
    public async Task<bool> RunAsync(Func<Task> action, string? successMessage = null)
    {
        try
        {
            await action();

            if (successMessage is not null)
            {
                _toasts.Success(successMessage);
            }

            return true;
        }
        catch (Exception exception)
        {
            Report(exception);
            return false;
        }
    }

    /// <summary>Runs an action that returns data. Returns default when it failed.</summary>
    public async Task<T?> RunAsync<T>(Func<Task<T>> action, string? successMessage = null)
    {
        try
        {
            var result = await action();

            if (successMessage is not null)
            {
                _toasts.Success(successMessage);
            }

            return result;
        }
        catch (Exception exception)
        {
            Report(exception);
            return default;
        }
    }

    private void Report(Exception exception)
    {
        if (exception is AppValidationException)
        {
            _logger.LogInformation("Validation failed: {Message}", exception.Message);
        }
        else
        {
            _logger.LogError(exception, "UI action failed");
        }

        _toasts.Error(ErrorMessages.ForUser(exception));
    }
}
