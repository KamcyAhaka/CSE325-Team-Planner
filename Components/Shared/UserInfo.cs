using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TeamProjectPlanner.Components.Shared;

/// <summary>Reads display info from the authenticated user's claims.</summary>
public static class UserInfo
{
    public static string GetDisplayName(AuthenticationState authState) =>
        authState.User.FindFirstValue(ClaimTypes.Name) ?? "User";

    public static string GetEmail(AuthenticationState authState) =>
        authState.User.FindFirstValue(ClaimTypes.Email) ?? "";

    public static string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0) return "U";

        var first = parts[0][0].ToString();

        if (parts.Length < 2) return first.ToUpperInvariant();

        var last = parts[^1][0].ToString();
        return (first + last).ToUpperInvariant();
    }
}
