using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TeamProjectPlanner.Services;

public class AuthService
{
    private readonly UserService _userService;
    private readonly HttpContext _httpContext;

    public AuthService(UserService userService, HttpContext httpContext)
    {
        _userService = userService;
        _httpContext = httpContext;
    }

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

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await _httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
