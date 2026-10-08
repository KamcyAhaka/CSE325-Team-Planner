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

        await _tasks.UpdateOneAsync(filter, update);
    }

    public async Task DeleteTaskAsync(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            throw new ArgumentException(
                "Task id is required.",
                nameof(taskId));
        }

        var filter = Builders<ProjectTask>.Filter.Eq(task => task.Id, taskId);

        await _tasks.DeleteOneAsync(filter);
    }
}
