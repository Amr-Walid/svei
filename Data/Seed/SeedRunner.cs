using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>
    /// Idempotent seeding — every block checks for existing rows first, so the app
    /// can be restarted safely and admin edits are never overwritten.
    /// </summary>
    public static class SeedRunner
    {
        public static async Task RunAsync(IServiceProvider sp)
        {
            var db = sp.GetRequiredService<AppDbContext>();
            var cfg = sp.GetRequiredService<IConfiguration>();

            await SeedIdentityAsync(sp, cfg);
            await SeedSettings.RunAsync(db);
            await SeedContent.RunAsync(db);
            await SeedCatalog.RunAsync(db);
            await SeedCommunity.RunAsync(db);
            await SeedNavigation.RunAsync(db);
        }

        private static async Task SeedIdentityAsync(IServiceProvider sp, IConfiguration cfg)
        {
            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var role in new[] { "Admin", "Editor" })
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole(role));

            var email = cfg["AdminSeed:Email"] ?? "admin@svei.tech";

            // No hardcoded password fallback. A literal in the source is a
            // credential that ships in the build and survives every config
            // change, so if the deployment has not supplied one we create no
            // account at all rather than a predictable one. Set it via
            // AdminSeed__Password in the environment (or user-secrets locally).
            var password = cfg["AdminSeed:Password"];

            var user = await userMgr.FindByEmailAsync(email);
            if (user is null)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    sp.GetRequiredService<ILogger<AppDbContext>>().LogWarning(
                        "No AdminSeed:Password configured — the initial admin account was not created. " +
                        "Set AdminSeed__Password and restart.");
                    return;
                }

                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userMgr.CreateAsync(user, password);
                if (!result.Succeeded) return;
            }

            if (!await userMgr.IsInRoleAsync(user, "Admin"))
                await userMgr.AddToRoleAsync(user, "Admin");
        }

        // ── shared helpers used by the seed partials ──────────────────────────
        internal static async Task AddIfEmptyAsync<T>(AppDbContext db, DbSet<T> set, IEnumerable<T> rows) where T : class
        {
            if (await set.AnyAsync()) return;
            set.AddRange(rows);
            await db.SaveChangesAsync();
        }
    }
}
