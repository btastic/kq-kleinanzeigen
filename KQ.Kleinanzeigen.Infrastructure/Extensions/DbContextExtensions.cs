using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KQ.Kleinanzeigen.Infrastructure.Extensions;

public static class DbContextExtensions
{
    public static void EnsureMigrationsApplied(this DbContext context)
    {
        IEnumerable<string> second = from m in context.GetService<IHistoryRepository>().GetAppliedMigrations()
                                     select m.MigrationId;
        if (context.GetService<IMigrationsAssembly>().Migrations.Select<KeyValuePair<string, TypeInfo>, string>((KeyValuePair<string, TypeInfo> m) => m.Key).Except(second).Any())
        {
            context.Database.Migrate();
        }
    }
}
