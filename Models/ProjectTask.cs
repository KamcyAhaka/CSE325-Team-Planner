using System.ComponentModel.DataAnnotations;
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

    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(200, ErrorMessage = "Task title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Task description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Task project is required.")]
    public string ProjectId { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;

    public ProjectTaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }
}
