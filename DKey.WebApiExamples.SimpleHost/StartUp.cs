using DKey.WebApiExamples.Model;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DKey.WebApiExamples.SimpleHost;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configure database
        services.AddDbContext<GameDbContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        });

        // Register repositories
        services.AddScoped<IGameRepository, GameRepository>();

        // Add routing capabilities
        services.AddRouting();

        // Configure HTTPS
        services.AddHttpsRedirection(options =>
        {
            options.HttpsPort = 5001; // Set your HTTPS port here
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseHttpsRedirection();

        // Initialize the database
        DbInitializer.Initialize(app.ApplicationServices);

        // Configure routing
        app.UseRouter(routes =>
        {
            // GET all games
            routes.MapGet("games", async (request, response, routeData) =>
            {
                var repository = request.HttpContext.RequestServices.GetRequiredService<IGameRepository>();
                var games = await repository.GetAllAsync();
                await response.WriteAsJsonAsync(games);
            });

            // GET game by ID
            routes.MapGet("games/{id}", async (request, response, routeData) =>
            {
                var idValue = routeData.Values["id"]?.ToString();
                if (Guid.TryParse(idValue, out var id))
                {
                    var repository = request.HttpContext.RequestServices.GetRequiredService<IGameRepository>();
                    var game = await repository.GetByIdAsync(id);
                    if (game == null)
                    {
                        response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }
                    await response.WriteAsJsonAsync(game);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });

            // POST create game
            routes.MapPost("games", async (request, response, routeData) =>
            {
                var gameDto = await request.ReadFromJsonAsync<GameDto>();
                if (gameDto == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var repository = request.HttpContext.RequestServices.GetRequiredService<IGameRepository>();
                var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment);
                await repository.AddAsync(game);

                response.StatusCode = StatusCodes.Status201Created;
                response.Headers.Add("Location", $"/games/{game.Id}");
                await response.WriteAsJsonAsync(game);
            });

            // PUT update game
            routes.MapPut("games/{id}", async (request, response, routeData) =>
            {
                var idValue = routeData.Values["id"]?.ToString();
                if (!Guid.TryParse(idValue, out var id))
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var gameDto = await request.ReadFromJsonAsync<GameDto>();
                if (gameDto == null)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var repository = request.HttpContext.RequestServices.GetRequiredService<IGameRepository>();
                var game = new Game(gameDto.Name, gameDto.Score, gameDto.Comment)
                {
                    Id = id
                };

                var updatedGame = await repository.UpdateAsync(game);
                if (updatedGame == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }

                await response.WriteAsJsonAsync(updatedGame);
            });

            // DELETE game
            routes.MapDelete("games/{id}", async (request, response, routeData) =>
            {
                var idValue = routeData.Values["id"]?.ToString();
                if (!Guid.TryParse(idValue, out var id))
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var repository = request.HttpContext.RequestServices.GetRequiredService<IGameRepository>();
                var result = await repository.DeleteAsync(id);

                response.StatusCode = result ? StatusCodes.Status204NoContent : StatusCodes.Status404NotFound;
            });
        });
    }
}