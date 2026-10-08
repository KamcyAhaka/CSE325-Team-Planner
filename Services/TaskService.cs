using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>CRUD operations for project tasks.</summary>
public class TaskService
{
    private readonly IMongoCollection<ProjectTask> _tasks;

    public TaskService(IMongoDatabase database)
    {
        _tasks = database.GetCollection<ProjectTask>("Tasks");
    }

    public async Task CreateTaskAsync(ProjectTask task)
    {
        EntityValidator.EnsureValid(task);

        task.CreatedAt = DateTime.UtcNow;

        await _tasks.InsertOneAsync(task);
    }

    public async Task<List<ProjectTask>> GetTasksByProjectAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException(
                "Project id is required.",
                nameof(projectId));
        }

        var filter = Builders<ProjectTask>.Filter.Eq(task => task.ProjectId, projectId);

        return await _tasks.Find(filter).ToListAsync();
    }

    public async Task<List<ProjectTask>> GetTasksByBoardAsync(string boardId)
    {
        if (string.IsNullOrWhiteSpace(boardId))
        {
            throw new ArgumentException(
                "Board id is required.",
                nameof(boardId));
        }

        var filter = Builders<ProjectTask>.Filter.Eq(task => task.BoardId, boardId);

        return await _tasks.Find(filter).ToListAsync();
    }

    public async Task<ProjectTask?> GetTaskByIdAsync(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new ArgumentException(
                "Task id is required.",
                nameof(taskId));
        }

        return await _tasks
            .Find(t => t.Id == taskId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateTaskAsync(ProjectTask task)
    {
        EntityValidator.EnsureValid(task);

        var filter = Builders<ProjectTask>.Filter.Eq(t => t.Id, task.Id);
        var update = Builders<ProjectTask>.Update
            .Set(t => t.Title, task.Title)
            .Set(t => t.Description, task.Description)
            .Set(t => t.OwnerId, task.OwnerId)
            .Set(t => t.BoardId, task.BoardId)
            .Set(t => t.Status, task.Status)
            .Set(t => t.DueDate, task.DueDate);

        var result = await _tasks.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new AppValidationException("Task not found.");
        }
    }

    public async Task UpdateTaskStatusAsync(string taskId, ProjectTaskStatus status)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new ArgumentException(
                "Task id is required.",
                nameof(taskId));
        }

        var filter = Builders<ProjectTask>.Filter.Eq(task => task.Id, taskId);
        var update = Builders<ProjectTask>.Update.Set(task => task.Status, status);

        var result = await _tasks.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new AppValidationException("Task not found.");
        }
    }

    public async Task DeleteTaskAsync(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new ArgumentException(
                "Task id is required.",
                nameof(taskId));
        }

        var result = await _tasks.DeleteOneAsync(t => t.Id == taskId);

        if (result.DeletedCount == 0)
        {
            throw new AppValidationException("Task not found.");
        }
    }
}
