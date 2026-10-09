using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Project entity owned by a user.</summary>
public class Project : IValidatableObject
{
    /// <summary>MongoDB ObjectId, generated when the entity is created.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Display name of the project.</summary>
    [Required(ErrorMessage = "Project name is required.")]
    [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Short description of what the project is about.</summary>
    [Required(ErrorMessage = "Project description is required.")]
    [StringLength(1000, ErrorMessage = "Project description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Planned start of the project timeline.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Planned end of the project timeline; must not be before <see cref="StartDate"/>.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>Id of the user who owns the project and can manage its members.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Cross-field validation: the end date must not be before the start date.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult(
                "Project end date cannot be before the start date.",
                new[] { nameof(EndDate) });
        }
    }
}