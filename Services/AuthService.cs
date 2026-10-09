using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TeamProjectPlanner.Services;

/// <summary>Cookie-based authentication with login and logout.</summary>
public class AuthService
{
    private readonly UserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Creates the service using the given user store and HTTP context accessor.</summary>
    public AuthService(
        UserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>Validates the credentials and signs the user in with a persistent cookie.</summary>
    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return (false, "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Password is required.");
        }

        var user = await _userService
            .GetByEmailAsync(email);

        if (user is null || !await _userService.VerifyPasswordAsync(user, password))
        {
            return (false, "Invalid email or password.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            // The cookie is persistent and lives for 7 days before the user must log in again.
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return (true, null);
    }

    /// <summary>Signs the current user out by clearing the authentication cookie.</summary>
    public async Task LogoutAsync()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}