using DKey.WebApiExamples.ControllerService.Attributes;
using DKey.WebApiExamples.Model;
using Microsoft.AspNetCore.Mvc;

namespace DKey.WebApiExamples.ControllerService.Controllers;

[ApiController]
[Route("games")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _repository;

    public GamesController(IGameRepository repository)
    {
        _repository = repository;
    }

    // GET: /games
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    // GET: /games/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _repository.GetByIdAsync(id);
        return game is null ? NotFound() : Ok(game);
    }

    // POST: /games
    [HttpPost]
    public async Task<IActionResult> Create(GameDto gameDto)
    {
        var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment);
        await _repository.AddAsync(game);
        return Created($"/games/{game.Id}", game);
    }

    // PUT: /games/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, GameDto gameDto)
    {
        var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment)
        {
            Id = id
        };

        var updatedGame = await _repository.UpdateAsync(game);
        return updatedGame is null ? NotFound() : Ok(updatedGame);
    }

    // DELETE: /games/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}