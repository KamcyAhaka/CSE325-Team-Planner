using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Kanban board belonging to a project.</summary>
public class Board
{
    /// <summary>MongoDB ObjectId, generated when the entity is created.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Display name of the board shown in the project workspace.</summary>
    [Required(ErrorMessage = "Board name is required.")]
    [StringLength(100, ErrorMessage = "Board name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional longer text describing the board's purpose.</summary>
    [StringLength(500, ErrorMessage = "Board description cannot exceed 500 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Id of the project this board belongs to.</summary>
    [Required(ErrorMessage = "Project is required.")]
    public string ProjectId { get; set; } = string.Empty;
}