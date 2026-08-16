using Microsoft.AspNetCore.Http;

namespace SVEI.Web.Services
{
    /// <summary>
    /// Central place that answers "which language is this request?" and picks the
    /// right value out of every (…Ar / …En) pair in the data model.
    /// </summary>
    public interface ILang
    {
        /// <summary>"ar" or "en"</summary>
        string Code { get; }
        bool IsAr { get; }
        bool IsRtl { get; }
        string Dir { get; }
        string HtmlLang { get; }
        /// <summary>Pick the localized value with a graceful fallback to the other language.</summary>
        string? Pick(string? ar, string? en);
        /// <summary>Prefix a site-relative path with the current culture segment.</summary>
        string Url(string path);
        /// <summary>
        /// Culture-prefix a link that came from the database (button and card URLs the
        /// admin types in). Unlike <see cref="Url"/> this is defensive: it leaves
        /// external, mail/tel, fragment and already-prefixed links untouched, so an
        /// admin can paste anything into the field and get a sane result.
        /// </summary>
        string? LinkUrl(string? url);
        /// <summary>Same path in the other language (for the language switcher).</summary>
        string SwitchUrl(string currentPathAndQuery);
    }

    public class LangService : ILang
    {
        public const string DefaultCulture = "ar";
        public static readonly string[] Supported = { "ar", "en" };

        public LangService(IHttpContextAccessor accessor)
        {
            Code = Resolve(accessor.HttpContext);
        }

        public string Code { get; }
        public bool IsAr => Code == "ar";
        public bool IsRtl => IsAr;
        public string Dir => IsRtl ? "rtl" : "ltr";
        public string HtmlLang => IsAr ? "ar" : "en";

        public string? Pick(string? ar, string? en)
        {
            if (IsAr) return string.IsNullOrWhiteSpace(ar) ? en : ar;
            return string.IsNullOrWhiteSpace(en) ? ar : en;
        }

        public string Url(string path)
        {
            path = "/" + (path ?? "").TrimStart('/');
            if (path == "/") return $"/{Code}";
            return $"/{Code}{path}";
        }

        public string? LinkUrl(string? url)
        {
            var u = (url ?? "").Trim();
            if (u.Length == 0) return null;

            // Anything that is not a plain site-relative path is passed through as
            // typed: absolute URLs (the online shop), protocol-relative links,
            // mailto:/tel:, in-page anchors and query-only links.
            if (u.StartsWith("#") || u.StartsWith("?") || u.StartsWith("//")) return u;
            if (u.Contains("://")) return u;
            if (u.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) return u;
            if (u.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)) return u;
            if (u.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)) return u;

            // Tolerate an admin typing "contact" instead of "/contact".
            if (!u.StartsWith("/")) u = "/" + u;

            // Already carries a culture segment ("/en/contact" or just "/en")?
            // Respect it — an admin may be linking deliberately across languages.
            foreach (var c in Supported)
            {
                if (u.Equals($"/{c}", StringComparison.OrdinalIgnoreCase)) return u;
                if (u.StartsWith($"/{c}/", StringComparison.OrdinalIgnoreCase)) return u;
            }

            // Static assets and framework endpoints are not localized.
            if (u.StartsWith("/img/", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase) ||
                u.StartsWith("/Account", StringComparison.OrdinalIgnoreCase))
                return u;

            return Url(u);
        }

        public string SwitchUrl(string currentPathAndQuery)
        {
            var other = IsAr ? "en" : "ar";
            var p = currentPathAndQuery ?? "/";
            foreach (var c in Supported)
            {
                if (p.Equals($"/{c}", StringComparison.OrdinalIgnoreCase)) return $"/{other}";
                if (p.StartsWith($"/{c}/", StringComparison.OrdinalIgnoreCase))
                    return $"/{other}" + p.Substring(c.Length + 1);
            }
            return $"/{other}" + (p == "/" ? "" : p);
        }

        /// <summary>Reads the culture from the route ({culture}) then the cookie, else default.</summary>
        public static string Resolve(HttpContext? ctx)
        {
            if (ctx is null) return DefaultCulture;

            var routeVal = ctx.GetRouteValue("culture")?.ToString();
            if (IsSupported(routeVal)) return routeVal!.ToLowerInvariant();

            var seg = ctx.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (IsSupported(seg)) return seg!.ToLowerInvariant();

            if (ctx.Request.Cookies.TryGetValue(CookieName, out var cookie) && IsSupported(cookie))
                return cookie!.ToLowerInvariant();

            return DefaultCulture;
        }

        public const string CookieName = "svei_lang";

        public static bool IsSupported(string? c) =>
            !string.IsNullOrWhiteSpace(c) &&
            Supported.Contains(c.ToLowerInvariant());
    }

    internal static class RouteValueExtensions
    {
        public static object? GetRouteValue(this HttpContext ctx, string key)
            => ctx.Request.RouteValues.TryGetValue(key, out var v) ? v : null;
    }
}
