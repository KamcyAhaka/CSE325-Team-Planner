using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamProjectPlanner.Models;

/// <summary>Workflow status of a project task.</summary>
public enum ProjectTaskStatus
{
    /// <summary>The task is waiting to be started.</summary>
    ToDo,

    /// <summary>The task is currently being worked on.</summary>
    InProgress,

    /// <summary>The task has been completed.</summary>
    Done
}

/// <summary>Task entity belonging to a project.</summary>
public class ProjectTask
{
    /// <summary>MongoDB ObjectId, generated when the entity is created.</summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>Short name of the task shown on the board.</summary>
    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(200, ErrorMessage = "Task title cannot exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional longer text describing the task.</summary>
    [StringLength(2000, ErrorMessage = "Task description cannot exceed 2000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Id of the project this task belongs to; used for ownership checks.</summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Id of the board column that currently holds the task.</summary>
    [Required(ErrorMessage = "Board is required.")]
    public string BoardId { get; set; } = string.Empty;

    /// <summary>Id of the user who created the task.</summary>
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>Current workflow position of the task on the board.</summary>
    public ProjectTaskStatus Status { get; set; }

    /// <summary>Server-assigned creation timestamp; never taken from client input.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Optional date by which the task should be finished.</summary>
    public DateTime? DueDate { get; set; }
}
