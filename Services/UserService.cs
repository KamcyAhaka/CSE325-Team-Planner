using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

public class UserService
{
    private readonly IMongoCollection<AppUser> _users;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public UserService(IMongoDatabase database)
    {
        _users = database.GetCollection<AppUser>("Users");

        var indexModel = new CreateIndexModel<AppUser>(
            Builders<AppUser>.IndexKeys.Ascending(user => user.Email),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "IX_Users_Email"
            });

        _users.Indexes.CreateOne(indexModel);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _users
            .Find(user => user.Email == normalizedEmail)
            .FirstOrDefaultAsync();
    }

    public async Task<AppUser> CreateUserAsync(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Password is required.",
                nameof(password));
        }

        var user = new AppUser
        {
            Email = email.Trim().ToLowerInvariant(),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? email : displayName,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        await _users.InsertOneAsync(user);

        return user;
    }

    public async Task<bool> VerifyPasswordAsync(AppUser user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
