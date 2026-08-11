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
