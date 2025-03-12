namespace DKey.WebApiExamples.Model;

public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task<Game> AddAsync(Game game);
    Task<Game?> UpdateAsync(Game game);
    Task<bool> DeleteAsync(Guid id);
}