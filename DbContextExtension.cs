using Microsoft.EntityFrameworkCore;

namespace Todos;

public static class DbContextExtension
{
    public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sqlite")
            ?? throw new InvalidOperationException("Missing configure Sqlite connection string.");
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
    }
}