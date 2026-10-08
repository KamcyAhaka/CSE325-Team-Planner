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
    }

    public async Task<List<Project>> GetProjectsForUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));
        }

        var memberProjectIds = await _members
            .Find(m => m.UserId == userId)
            .Project(m => m.ProjectId)
            .ToListAsync();

        return await _projects
            .Find(p => p.OwnerId == userId || memberProjectIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException(
                "Project id is required.",
                nameof(projectId));
        }

        return await _projects
            .Find(p => p.Id == projectId)
            .FirstOrDefaultAsync();
    }
}