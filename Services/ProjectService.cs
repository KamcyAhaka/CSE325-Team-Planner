using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>Operations for creating and listing projects.</summary>
public class ProjectService
{
    private readonly IMongoCollection<Project> _projects;
    private readonly IMongoCollection<ProjectMember> _members;

    public ProjectService(IMongoDatabase database)
    {
        _projects = database.GetCollection<Project>("Projects");
        _members = database.GetCollection<ProjectMember>("ProjectMembers");
    }

    public async Task CreateProjectAsync(Project project)
    {
        EntityValidator.EnsureValid(project);

        await _projects.InsertOneAsync(project);

        var ownerMember = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = project.OwnerId,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        await _members.InsertOneAsync(ownerMember);
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        return await _projects.Find(_ => true).ToListAsync();
    }
}