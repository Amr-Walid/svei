using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;

namespace SVEI.Web.Services
{
    /// <summary>
    /// 1. Honours admin-managed UrlRedirects (old .html links from the previous site).
    /// 2. Sends any public path that has no /ar or /en prefix to the visitor's language.
    /// 3. Persists the chosen language in a cookie.
    /// </summary>
    public class CultureMiddleware
    {
        private static readonly string[] Bypass =
        {
            "/admin", "/account", "/css", "/js", "/lib", "/img", "/fonts", "/uploads",
            "/favicon.ico", "/robots.txt", "/sitemap.xml", "/.well-known"
        };

        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext ctx, AppDbContext db)
        {
            var path = ctx.Request.Path.Value ?? "/";

            if (Bypass.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(ctx);
                return;
            }

            // ── 1. Legacy redirects (e.g. /index.html → /) ────────────────────
            var normalized = path.TrimEnd('/');
            if (normalized.Length == 0) normalized = "/";

            var redirect = await db.UrlRedirects.AsNoTracking()
                .FirstOrDefaultAsync(r => r.IsActive && r.FromPath == normalized);

            if (redirect is not null)
            {
                var target = string.IsNullOrWhiteSpace(redirect.ToPath) ? "/" : redirect.ToPath;
                if (!target.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                    !LangService.IsSupported(FirstSegment(target)))
                {
                    target = "/" + CurrentCulture(ctx) + "/" + target.TrimStart('/');
                    target = target.TrimEnd('/');
                    if (target.Length == 0) target = "/" + CurrentCulture(ctx);
                }

                ctx.Response.Redirect(target, redirect.IsPermanent);
                return;
            }

            // ── 2. Missing culture prefix → redirect ──────────────────────────
            var first = FirstSegment(path);
            if (!LangService.IsSupported(first))
            {
                var culture = CurrentCulture(ctx);
                var suffix = path == "/" ? "" : path;
                ctx.Response.Redirect($"/{culture}{suffix}{ctx.Request.QueryString}", false);
                return;
            }

            // ── 3. Remember the language ──────────────────────────────────────
            var chosen = first!.ToLowerInvariant();
            if (!ctx.Request.Cookies.TryGetValue(LangService.CookieName, out var cookie) || cookie != chosen)
            {
                ctx.Response.Cookies.Append(LangService.CookieName, chosen, new CookieOptions
                {
                    Expires  = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = false,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Path     = "/"
                });
            }

            await _next(ctx);
        }

        private static string? FirstSegment(string path) =>
            path.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        private static string CurrentCulture(HttpContext ctx)
        {
            if (ctx.Request.Cookies.TryGetValue(LangService.CookieName, out var c) && LangService.IsSupported(c))
                return c!.ToLowerInvariant();

            // Fall back to the browser's Accept-Language
            var header = ctx.Request.Headers.AcceptLanguage.ToString();
            if (!string.IsNullOrWhiteSpace(header) &&
                header.Contains("en", StringComparison.OrdinalIgnoreCase) &&
                !header.Contains("ar", StringComparison.OrdinalIgnoreCase))
                return "en";

            return LangService.DefaultCulture;
        }
    }

    public static class CultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseSveiCulture(this IApplicationBuilder app)
            => app.UseMiddleware<CultureMiddleware>();
    }
}
