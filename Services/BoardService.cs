using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>CRUD operations for project boards.</summary>
public class BoardService
{
    private readonly IMongoCollection<Board> _boards;
    private readonly IMongoCollection<ProjectTask> _tasks;

    public BoardService(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
        _tasks = database.GetCollection<ProjectTask>("Tasks");
    }

    public async Task CreateBoardAsync(Board board)
    {
        EntityValidator.EnsureValid(board);

        await _boards.InsertOneAsync(board);
    }

    public async Task<List<Board>> GetBoardsByProjectAsync(string projectId)
    {
        return await _boards
            .Find(board => board.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByIdAsync(string boardId)
    {
        if (string.IsNullOrWhiteSpace(boardId))
        {
            throw new ArgumentException(
                "Board id is required.",
                nameof(boardId));
        }

        return await _boards
            .Find(board => board.Id == boardId)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateBoardAsync(Board board)
    {
        EntityValidator.EnsureValid(board);

        var filter = Builders<Board>.Filter.Eq(b => b.Id, board.Id);
        var update = Builders<Board>.Update
            .Set(b => b.Name, board.Name)
            .Set(b => b.Description, board.Description);

        var result = await _boards.UpdateOneAsync(filter, update);

        if (result.MatchedCount == 0)
        {
            throw new AppValidationException("Board not found.");
        }
    }

    public async Task DeleteBoardAsync(string boardId)
    {
        if (string.IsNullOrWhiteSpace(boardId))
        {
            throw new ArgumentException(
                "Board id is required.",
                nameof(boardId));
        }

        var result = await _boards.DeleteOneAsync(b => b.Id == boardId);

        if (result.DeletedCount == 0)
        {
            throw new AppValidationException("Board not found.");
        }

        await _tasks.DeleteManyAsync(t => t.BoardId == boardId);
    }
}
