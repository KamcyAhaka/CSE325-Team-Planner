using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TeamProjectPlanner.Components.Shared;

/// <summary>Reads display info from the authenticated user's claims.</summary>
public static class UserInfo
{
    /// <summary>Returns the user's display name from claims, or "User" when missing.</summary>
    public static string GetDisplayName(AuthenticationState authState) =>
        authState.User.FindFirstValue(ClaimTypes.Name) ?? "User";

    /// <summary>Returns the user's email from claims, or an empty string when missing.</summary>
    public static string GetEmail(AuthenticationState authState) =>
        authState.User.FindFirstValue(ClaimTypes.Email) ?? "";

    /// <summary>Builds avatar initials (first + last name letter) from a display name.</summary>
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
