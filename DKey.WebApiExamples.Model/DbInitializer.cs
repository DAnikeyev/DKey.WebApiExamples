using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DKey.WebApiExamples.Model;

public static class DbInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

        context.Database.EnsureCreated();

        EnsureIndexes(context);
    }

    private static void EnsureIndexes(GameDbContext context)
    {
        var providerName = context.Database.ProviderName;

        if (providerName?.Contains("Npgsql") == true)
        {
            context.Database.OpenConnection();

            try
            {
                var checkIndexSql = "SELECT COUNT(1) FROM pg_indexes WHERE tablename = 'Games' AND indexname = 'IX_Games_Name'";
                var indexExists = context.Database.ExecuteSqlRaw(checkIndexSql) > 0;

                if (!indexExists)
                {
                    var createIndexSql = "CREATE INDEX IF NOT EXISTS \"IX_Games_Name\" ON \"Games\" (\"Name\")";
                    context.Database.ExecuteSqlRaw(createIndexSql);
                }
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
    }
}