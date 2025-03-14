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


// Example method using HttpRequest directly
app.MapPost("/process-request", async (HttpRequest request, IGameRepository repository) =>
{
    try
    {
        // Access request headers
        var userAgent = request.Headers["User-Agent"].ToString();

        // Read the request body manually if needed
        GameDto? gameDto = await request.ReadFromJsonAsync<GameDto>();

        if (gameDto == null)
        {
            return Results.BadRequest("Invalid game data");
        }

        // Process the data
        var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment);
        await repository.AddAsync(game);

        // Return response with custom headers
        var response = Results.Created($"/games/{game.Id}", game);

        // You can also log request information
        Console.WriteLine($"Request processed from: {userAgent}");

        return response;
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error processing request: {ex.Message}");
    }
});

app.Run();