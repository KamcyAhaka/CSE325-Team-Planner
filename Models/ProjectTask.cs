using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

public enum ProjectTaskStatus
{
    ToDo,
    InProgress,
    Done
}

public class ProjectTask
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ProjectId { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;

    public ProjectTaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }
}
