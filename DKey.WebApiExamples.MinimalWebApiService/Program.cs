using System;
using DKey.WebApiExamples.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure database
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

// Initialize the database
DbInitializer.Initialize(app.Services);

// GET all games
app.MapGet("/games", async (IGameRepository repository) =>
    await repository.GetAllAsync());

// GET game by ID
app.MapGet("/games/{id}", async (Guid id, IGameRepository repository) =>
{
    var game = await repository.GetByIdAsync(id);
    return game is null ? Results.NotFound() : Results.Ok(game);
});

// POST create game
app.MapPost("/games", async (GameDto gameDto, IGameRepository repository) =>
{
    var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment);
    await repository.AddAsync(game);
    return Results.Created($"/games/{game.Id}", game);
});

// PUT update game
app.MapPut("/games/{id}", async (Guid id, GameDto gameDto, IGameRepository repository) =>
{
    var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment)
    {
        Id = id
    };

    var updatedGame = await repository.UpdateAsync(game);
    return updatedGame is null ? Results.NotFound() : Results.Ok(updatedGame);
});

// DELETE game
app.MapDelete("/games/{id}", async (Guid id, IGameRepository repository) =>
{
    var result = await repository.DeleteAsync(id);
    return result ? Results.NoContent() : Results.NotFound();
});

app.Run();