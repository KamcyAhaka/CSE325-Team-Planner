using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Role of a member within a project.</summary>
public enum ProjectRole
{
    /// <summary>The user who created the project and can manage its members.</summary>
    Owner,

    /// <summary>A regular collaborator invited to the project.</summary>
    Member
}

/// <summary>Membership link between a user and a project.</summary>
public class ProjectMember
{
    /// <summary>MongoDB ObjectId, generated when the entity is created.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Id of the project this membership refers to.</summary>
    [Required(ErrorMessage = "Project is required.")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Id of the user who holds this membership.</summary>
    [Required(ErrorMessage = "User is required.")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>Whether the user owns the project or is a regular member.</summary>
    public ProjectRole Role { get; set; }

    /// <summary>Server-assigned timestamp of when the membership was created.</summary>
    public DateTime JoinedAt { get; set; }
}