using Microsoft.EntityFrameworkCore;

namespace Alfa.ChatMS.Helper
{
    public static class EnsureMigration
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T : DbContext
        {
            using var services = app.ApplicationServices.CreateScope();

            var dbContext = services.ServiceProvider.GetService<T>();

            dbContext.Database.Migrate();
        }
    }
}
