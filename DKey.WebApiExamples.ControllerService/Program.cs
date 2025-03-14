using DKey.WebApiExamples.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers - just the minimal required setup
builder.Services.AddControllers();

// Configure database
builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register repositories
builder.Services.AddScoped<IGameRepository, GameRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

// Map controller routes
app.MapControllers();

// Initialize the database
DbInitializer.Initialize(app.Services);

app.Run();