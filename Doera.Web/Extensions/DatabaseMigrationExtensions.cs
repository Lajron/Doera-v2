using System.Threading.Tasks;
using Doera.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Web.Extensions;

public static class DatabaseMigrationExtensions {
    public static async Task InitializeDatabaseAsync(this WebApplication app) {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.Initialize();
    }

    public static void InitializeDatabase(this WebApplication app) =>
        app.InitializeDatabaseAsync().GetAwaiter().GetResult();
}