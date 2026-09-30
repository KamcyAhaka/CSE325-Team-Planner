using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

public enum ProjectRole
{
    Owner,
    Member
}

public class ProjectMember
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string ProjectId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public ProjectRole Role { get; set; }

    public DateTime JoinedAt { get; set; }
}