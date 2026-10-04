using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

public class ProjectService
{
    private readonly IMongoCollection<Project> _projects;

    public ProjectService(IMongoDatabase database)
    {
        _projects = database.GetCollection<Project>("Projects");
    }

    public async Task CreateProjectAsync(Project project)
    {
        EntityValidator.EnsureValid(project);

        await _projects.InsertOneAsync(project);
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        return await _projects.Find(_ => true).ToListAsync();
    }
}