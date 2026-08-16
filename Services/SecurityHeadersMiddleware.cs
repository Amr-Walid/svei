namespace SVEI.Web.Services
{
    /// <summary>
    /// Adds the response headers that turn a browser into an ally instead of a
    /// delivery mechanism. Applied to every response, including static files and
    /// the error pages, so there is no path that escapes the policy.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

        /// <summary>
        /// The site loads every script and stylesheet from its own origin (Leaflet,
        /// Swiper, Quill, GLightbox and Sortable are all vendored under wwwroot/lib),
        /// so 'self' is enough for script-src and no CDN needs allowing.
        ///
        /// Notes on the two relaxations:
        ///  • style-src allows 'unsafe-inline' because the views carry ~81 style=""
        ///    attributes for admin-driven colours and widths, which cannot be
        ///    nonced. Inline *style* cannot execute script, so this does not
        ///    reopen XSS — and the sanitizer strips style from stored content.
        ///  • img-src allows https: and data: so the admin can point an image at an
        ///    external URL and Quill can paste base64 previews.
        ///
        /// object-src 'none' + base-uri 'self' + frame-ancestors 'none' are the
        /// three directives that actually block the residual injection tricks
        /// (plugin payloads, &lt;base&gt; hijacking and clickjacking).
        /// </summary>
        private const string Csp =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'; " +
            "frame-src 'self' https://www.google.com https://maps.google.com; " +
            "media-src 'self'; " +
            "object-src 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'; " +
            "frame-ancestors 'none'; " +
            "upgrade-insecure-requests";

        public async Task InvokeAsync(HttpContext ctx)
        {
            var h = ctx.Response.Headers;

            // OnStarting so the headers survive the pipeline re-execution that
            // UseStatusCodePagesWithReExecute performs for 404s.
            ctx.Response.OnStarting(() =>
            {
                h["X-Content-Type-Options"] = "nosniff";
                h["X-Frame-Options"] = "DENY";
                h["Referrer-Policy"] = "strict-origin-when-cross-origin";
                h["Content-Security-Policy"] = Csp;
                h["Permissions-Policy"] =
                    "geolocation=(), microphone=(), camera=(), payment=(), usb=(), interest-cohort=()";
                h["Cross-Origin-Opener-Policy"] = "same-origin";
                h["X-Permitted-Cross-Domain-Policies"] = "none";

                // Kestrel's own fingerprint — no reason to advertise the stack.
                h.Remove("Server");
                h.Remove("X-Powered-By");
                h.Remove("X-AspNet-Version");

                // Anything a visitor uploaded is served inert. Images still need
                // to render in <img>, so they keep inline disposition but get the
                // sandbox CSP; documents (CVs, RFQ attachments) are forced to
                // download so nothing can ever be presented as a page on our
                // origin. Browsers ignore Content-Disposition for subresource
                // loads anyway, but keeping images inline avoids any surprise.
                var path = ctx.Request.Path.Value ?? "";
                if (path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                {
                    h["Content-Security-Policy"] = "default-src 'none'; sandbox";
                    h["X-Content-Type-Options"] = "nosniff";

                    var ext = Path.GetExtension(path).ToLowerInvariant();
                    var isImage = ext is ".webp" or ".jpg" or ".jpeg" or ".png"
                                      or ".gif" or ".bmp" or ".ico";
                    if (!isImage) h["Content-Disposition"] = "attachment";
                }

                return Task.CompletedTask;
            });

            await _next(ctx);
        }
    }

    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSveiSecurityHeaders(this IApplicationBuilder app)
            => app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
