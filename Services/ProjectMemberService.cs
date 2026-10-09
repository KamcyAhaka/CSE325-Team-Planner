using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>Operations for managing project membership.</summary>
public class ProjectMemberService
{
    private readonly IMongoCollection<ProjectMember> _members;

    /// <summary>Initializes the service with the MongoDB project members collection.</summary>
    public ProjectMemberService(IMongoDatabase database)
    {
        _members = database.GetCollection<ProjectMember>("ProjectMembers");
    }

    /// <summary>Adds a user to a project, rejecting duplicate memberships.</summary>
    public async Task AddMemberAsync(ProjectMember member)
    {
        EntityValidator.EnsureValid(member);

        // A user can only belong to a project once, so duplicates are rejected.
        var alreadyMember = await _members
            .Find(m => m.ProjectId == member.ProjectId && m.UserId == member.UserId)
            .FirstOrDefaultAsync();

        if (alreadyMember != null)
        {
            throw new AppValidationException(
                "User is already a member of this project.");
        }

        await _members.InsertOneAsync(member);
    }

    /// <summary>Returns all members of the given project.</summary>
    public async Task<List<ProjectMember>> GetMembersByProjectAsync(string projectId)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException(
                "Project id is required.",
                nameof(projectId));
        }

        return await _members
            .Find(m => m.ProjectId == projectId)
            .ToListAsync();
    }

    /// <summary>Removes a member from their project by membership id.</summary>
    public async Task RemoveMemberAsync(string memberId)
    {
        if (string.IsNullOrWhiteSpace(memberId))
        {
            throw new ArgumentException(
                "Member id is required.",
                nameof(memberId));
        }

        // The UI only offers removal to the project owner, so the owner's own
        // membership can never be deleted through this path.
        var result = await _members.DeleteOneAsync(m => m.Id == memberId);

        if (result.DeletedCount == 0)
        {
            throw new AppValidationException("Member not found.");
        }
    }
}