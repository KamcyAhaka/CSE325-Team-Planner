using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Application user entity stored in the Users collection.</summary>
public class AppUser : IValidatableObject
{
    /// <summary>MongoDB ObjectId, generated when the entity is created.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Login email, stored in lowercase and unique across users.</summary>
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [StringLength(255, ErrorMessage = "Email is too long.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Form-only field; never persisted.</summary>
    [BsonIgnore]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Hashed password produced by ASP.NET Core Identity; the plain password is never stored.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Name shown to other users; falls back to the email at registration.</summary>
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Server-assigned timestamp of when the account was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Password policy: at least 8 characters with an uppercase letter, a lowercase letter and a digit.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Password))
        {
            yield break;
        }

        if (Password.Length < 8)
        {
            yield return new ValidationResult("Password must be at least 8 characters long.", new[] { nameof(Password) });
            yield break;
        }

        if (!Password.Any(char.IsUpper))
        {
            yield return new ValidationResult("Password must contain at least one uppercase letter.", new[] { nameof(Password) });
        }
        else if (!Password.Any(char.IsLower))
        {
            yield return new ValidationResult("Password must contain at least one lowercase letter.", new[] { nameof(Password) });
        }
        else if (!Password.Any(char.IsDigit))
        {
            yield return new ValidationResult("Password must contain at least one number.", new[] { nameof(Password) });
        }
    }
}
