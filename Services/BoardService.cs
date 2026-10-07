using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>Operations for creating and listing boards.</summary>
public class BoardService
{
    private readonly IMongoCollection<Board> _boards;

    public BoardService(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
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
}
