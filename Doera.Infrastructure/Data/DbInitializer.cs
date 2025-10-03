using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Doera.Infrastructure.Data {
    internal class DbInitializer(
            ApplicationDbContext _context,
            ILogger<DbInitializer> _logger
        ) : IDbInitializer {

        public async Task Initialize() {
            await ApplyMigrationsAsync();
            // await SeedAsync();
        }

        private async Task ApplyMigrationsAsync() {
            try {
                var pending = await _context.Database.GetPendingMigrationsAsync();
                if (pending.Any()) {
                    _logger.LogInformation("Applying {Count} pending migration(s): {Migrations}",
                        pending.Count(), string.Join(", ", pending));
                    await _context.Database.MigrateAsync();
                    _logger.LogInformation("Database migrations applied successfully.");
                } else {
                    _logger.LogInformation("No pending migrations. Database is up to date.");
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while applying database migrations.");
                throw;
            }
        }

        // private async Task SeedAsync() { }
    }
}
