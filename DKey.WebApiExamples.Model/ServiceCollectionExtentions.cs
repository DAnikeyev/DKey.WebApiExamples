using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DKey.WebApiExamples.Model;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameDbContext(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<GameDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
}