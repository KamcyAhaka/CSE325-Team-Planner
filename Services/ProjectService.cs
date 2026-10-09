using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>CRUD operations for projects.</summary>
public class ProjectService
{
    private readonly IMongoCollection<Project> _projects;
    private readonly IMongoCollection<Board> _boards;
    private readonly IMongoCollection<ProjectTask> _tasks;
    private readonly IMongoCollection<ProjectMember> _members;

    /// <summary>Initializes the service with the MongoDB collections it operates on.</summary>
    public ProjectService(IMongoDatabase database)
    {
        _projects = database.GetCollection<Project>("Projects");
        _boards = database.GetCollection<Board>("Boards");
        _tasks = database.GetCollection<ProjectTask>("Tasks");
        _members = database.GetCollection<ProjectMember>("ProjectMembers");
    }

    /// <summary>Creates a new project and registers its owner as an Owner-role member.</summary>
    public async Task CreateProjectAsync(Project project)
    {
        EntityValidator.EnsureValid(project);

        await _projects.InsertOneAsync(project);

        // The creator automatically becomes a member with the Owner role,
        // so they can manage boards, tasks and other members.
        var ownerMember = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = project.OwnerId,
            Role = ProjectRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        await _members.InsertOneAsync(ownerMember);
    }

    /// <summary>Returns all projects the user owns or has been invited to as a member.</summary>
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

        // A project is visible if the user owns it or appears in their memberships.
        return await _projects
            .Find(p => p.OwnerId == userId || memberProjectIds.Contains(p.Id))
            .ToListAsync();
    }

    /// <summary>Returns the project with the given id, or null if it does not exist.</summary>
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

    /// <summary>Updates the editable fields of an existing project.</summary>
    public async Task UpdateProjectAsync(Project project)
    {
        EntityValidator.EnsureValid(project);

        var filter = Builders<Project>.Filter.Eq(p => p.Id, project.Id);
        var update = Builders<Project>.Update
            .Set(p => p.Name, project.Name)
            .Set(p => p.Description, project.Description)
            .Set(p => p.StartDate, project.StartDate)
            .Set(p => p.EndDate, project.EndDate);

        var result = await _projects.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new AppValidationException("Project not found.");
        }
    }

    /// <summary>Deletes a project and all of its dependent boards, tasks and memberships.</summary>
    public async Task DeleteProjectAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException(
                "Project id is required.",
                nameof(projectId));
        }

        var result = await _projects.DeleteOneAsync(p => p.Id == projectId);

        if (result.DeletedCount == 0)
        {
            throw new AppValidationException("Project not found.");
        }

        // Cascade delete: remove all dependent documents so no orphaned data remains.
        await _boards.DeleteManyAsync(b => b.ProjectId == projectId);
        await _tasks.DeleteManyAsync(t => t.ProjectId == projectId);
        await _members.DeleteManyAsync(m => m.ProjectId == projectId);
    }
}