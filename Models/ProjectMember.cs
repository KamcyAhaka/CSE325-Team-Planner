using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Role of a member within a project.</summary>
public enum ProjectRole
{
    Owner,
    Member
}

/// <summary>Membership link between a user and a project.</summary>
public class ProjectMember
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [Required(ErrorMessage = "Project is required.")]
    public string ProjectId { get; set; } = string.Empty;

    [Required(ErrorMessage = "User is required.")]
    public string UserId { get; set; } = string.Empty;

    public ProjectRole Role { get; set; }

    public DateTime JoinedAt { get; set; }
}