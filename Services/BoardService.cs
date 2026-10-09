using MongoDB.Driver;
using TeamProjectPlanner.Models;

namespace TeamProjectPlanner.Services;

/// <summary>CRUD operations for project boards.</summary>
public class BoardService
{
    private readonly IMongoCollection<Board> _boards;
    private readonly IMongoCollection<ProjectTask> _tasks;

    /// <summary>Initializes the service with the MongoDB boards and tasks collections.</summary>
    public BoardService(IMongoDatabase database)
    {
        _boards = database.GetCollection<Board>("Boards");
        _tasks = database.GetCollection<ProjectTask>("Tasks");
    }

    /// <summary>Creates a new board within an existing project.</summary>
    public async Task CreateBoardAsync(Board board)
    {
        EntityValidator.EnsureValid(board);

        await _boards.InsertOneAsync(board);
    }

    /// <summary>Returns all boards belonging to the given project.</summary>
    public async Task<List<Board>> GetBoardsByProjectAsync(string projectId)
    {
        return await _boards
            .Find(board => board.ProjectId == projectId)
            .ToListAsync();
    }

    /// <summary>Returns the board with the given id, or null if it does not exist.</summary>
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

    /// <summary>Updates the name and description of an existing board.</summary>
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

    /// <summary>Deletes a board and all the tasks it contains.</summary>
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

        // Cascade delete: remove the board's tasks so no orphaned documents remain.
        await _tasks.DeleteManyAsync(t => t.BoardId == boardId);
    }
}
