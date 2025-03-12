using Microsoft.EntityFrameworkCore;

namespace DKey.WebApiExamples.Model;

public class GameRepository : IGameRepository
{
    private readonly GameDbContext _context;

    public GameRepository(GameDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _context.Games.FindAsync(id);
    }

    public async Task<Game> AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
        return game;
    }

    public async Task<Game?> UpdateAsync(Game game)
    {
        var existingGame = await _context.Games.FindAsync(game.Id);

        if (existingGame == null)
            return null;

        existingGame.Name = game.Name;
        existingGame.Score = game.Score;
        existingGame.Comment = game.Comment;

        await _context.SaveChangesAsync();
        return existingGame;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
            return false;

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }
}