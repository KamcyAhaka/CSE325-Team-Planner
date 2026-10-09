namespace TeamProjectPlanner.Services;

/// <summary>Severity level of a toast notification.</summary>
public enum ToastLevel
{
    Success,
    Info,
    Warning,
    Error
}

/// <summary>A single toast notification shown to the user.</summary>
public sealed record ToastMessage(Guid Id, ToastLevel Level, string Message, TimeSpan Duration);

/// <summary>Holds the toast notifications for the current user's circuit. Rendered by the ToastHost component.</summary>
public class ToastService
{
    private const int MaxVisible = 5;

    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan ErrorDuration = TimeSpan.FromSeconds(8);

    private readonly List<ToastMessage> _toasts = new();
    private readonly object _lock = new();

    /// <summary>Raised whenever the toast list changes so subscribed components can re-render.</summary>
    public event Action? Changed;

    /// <summary>Returns a snapshot of the currently visible toasts, oldest first.</summary>
    public IReadOnlyList<ToastMessage> Toasts
    {
        get
        {
            lock (_lock)
            {
                return _toasts.ToList();
            }
        }
    }

    /// <summary>Shows a success toast.</summary>
    public void Success(string message) => Show(ToastLevel.Success, message);

    /// <summary>Shows an informational toast.</summary>
    public void Info(string message) => Show(ToastLevel.Info, message);

    /// <summary>Shows a warning toast.</summary>
    public void Warning(string message) => Show(ToastLevel.Warning, message);

    /// <summary>Shows an error toast, which stays visible longer than the other levels.</summary>
    public void Error(string message) => Show(ToastLevel.Error, message);

    /// <summary>Adds a toast at the given level and raises <see cref="Changed"/>.</summary>
    public void Show(ToastLevel level, string message, TimeSpan? duration = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var toast = new ToastMessage(
            Guid.NewGuid(),
            level,
            message,
            duration ?? (level == ToastLevel.Error ? ErrorDuration : DefaultDuration));

        lock (_lock)
        {
            _toasts.Add(toast);

            // Keep at most MaxVisible toasts so a burst of actions cannot flood the screen;
            // the oldest one is dropped first.
            if (_toasts.Count > MaxVisible)
            {
                _toasts.RemoveAt(0);
            }
        }

        Changed?.Invoke();
    }

    /// <summary>Removes the toast with the given id and raises <see cref="Changed"/> if it existed.</summary>
    public void Dismiss(Guid id)
    {
        bool removed;

        lock (_lock)
        {
            removed = _toasts.RemoveAll(toast => toast.Id == id) > 0;
        }

        if (removed)
        {
            Changed?.Invoke();
        }
    }
}
