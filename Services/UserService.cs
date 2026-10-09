using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>CRUD operations for users with password hashing and email indexing.</summary>
public class UserService
{
    private readonly IMongoCollection<AppUser> _users;
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    // Created on first use so a database outage doesn't crash dependency injection
    private static readonly SemaphoreSlim IndexLock = new(1, 1);
    private static bool _indexCreated;

    /// <summary>Creates the service bound to the Users collection of the given database.</summary>
    public UserService(IMongoDatabase database)
    {
        _users = database.GetCollection<AppUser>("Users");
    }

    /// <summary>Creates the unique email index once per process using double-checked locking.</summary>
    private async Task EnsureEmailIndexAsync()
    {
        if (_indexCreated)
        {
            return;
        }

        await IndexLock.WaitAsync();

        try
        {
            if (_indexCreated)
            {
                return;
            }

            var indexModel = new CreateIndexModel<AppUser>(
                Builders<AppUser>.IndexKeys.Ascending(user => user.Email),
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "IX_Users_Email"
                });

            await _users.Indexes.CreateOneAsync(indexModel);
            _indexCreated = true;
        }
        finally
        {
            IndexLock.Release();
        }
    }

    /// <summary>Finds a user by email, ignoring surrounding whitespace and letter case.</summary>
    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        await EnsureEmailIndexAsync();

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _users
            .Find(user => user.Email == normalizedEmail)
            .FirstOrDefaultAsync();
    }

    /// <summary>Searches users by email or display name, returning at most 10 matches.</summary>
    public async Task<List<AppUser>> SearchUsersAsync(string query)
    {
        var normalized = query.Trim().ToLowerInvariant();

        if (normalized.Length == 0)
        {
            return new List<AppUser>();
        }

        // Case-insensitive regex so the query matches both the lowercase email and the display name.
        var pattern = new BsonRegularExpression(Regex.Escape(normalized), "i");
        var filter = Builders<AppUser>.Filter.Or(
            Builders<AppUser>.Filter.Regex(u => u.Email, pattern),
            Builders<AppUser>.Filter.Regex(u => u.DisplayName, pattern));

        return await _users.Find(filter).Limit(10).ToListAsync();
    }

    /// <summary>Loads several users in one query and returns them keyed by user id.</summary>
    public async Task<Dictionary<string, AppUser>> GetUsersByIdsAsync(
        IEnumerable<string> userIds)
    {
        var ids = userIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return new Dictionary<string, AppUser>();
        }

        var users = await _users
            .Find(user => ids.Contains(user.Id))
            .ToListAsync();

        return users.ToDictionary(user => user.Id);
    }

    /// <summary>Registers a new user with a hashed password; the display name falls back to the email.</summary>
    public async Task<AppUser> CreateUserAsync(string email, string password, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new AppValidationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new AppValidationException("Password is required.");
        }

        await EnsureEmailIndexAsync();

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

    /// <summary>Checks a password against the stored hash and transparently upgrades outdated hashes.</summary>
    public async Task<bool> VerifyPasswordAsync(AppUser user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        // If the hash was produced by an older algorithm, store a fresh one so
        // future logins are verified with the current algorithm.
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
