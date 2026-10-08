using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>Operations for managing project membership.</summary>
public class ProjectMemberService
{
    private readonly IMongoCollection<ProjectMember> _members;

    public ProjectMemberService(IMongoDatabase database)
    {
        _members = database.GetCollection<ProjectMember>("ProjectMembers");
    }

    public async Task AddMemberAsync(ProjectMember member)
    {
        EntityValidator.EnsureValid(member);

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

    public async Task RemoveMemberAsync(string memberId)
    {
        if (string.IsNullOrWhiteSpace(memberId))
        {
            throw new ArgumentException(
                "Member id is required.",
                nameof(memberId));
        }

        await _members.DeleteOneAsync(m => m.Id == memberId);
    }
}