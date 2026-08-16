using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace SVEI.Web.Services
{
    /// <summary>
    /// Request throttling for the public site.
    ///
    /// Two distinct problems are being solved here and they need different
    /// budgets:
    ///
    ///  • Abuse — the contact / quote / apply / subscribe forms are anonymous
    ///    writes straight into the database. Before this, a script could insert
    ///    unlimited rows (and unlimited CV files on disk) at line speed.
    ///  • Load — a single client should not be able to monopolise the worker
    ///    threads and starve real visitors.
    ///
    /// Everything is partitioned per client IP so one abuser cannot degrade the
    /// service for everyone, which is what a single global counter would do.
    /// </summary>
    public static class RateLimiting
    {
        public const string Forms = "forms";
        public const string Login = "login";

        /// <summary>
        /// Header that exempts a caller from the global read budget, used by the
        /// load-test harness to measure real server capacity rather than measure
        /// the limiter. Only honoured when a secret is configured, and the value
        /// is compared in fixed time. Left unset in production, this is inert.
        /// </summary>
        public const string BypassHeader = "X-SVEI-LoadTest";

        private static string? _bypassSecret;

        public static IServiceCollection AddSveiRateLimiter(
            this IServiceCollection services, IConfiguration? config = null)
        {
            _bypassSecret = config?["Security:LoadTestToken"];

            services.AddRateLimiter(o =>
            {
                o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // ── Global read budget ────────────────────────────────────────
                // Generous on purpose: a real visitor loading a page pulls CSS,
                // JS, fonts and a dozen images, and offices behind one NAT share
                // an IP. This only trips on automated hammering. Static files are
                // excluded so assets never consume a visitor's page budget.
                o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                {
                    if (IsLoadTest(ctx)) return RateLimitPartition.GetNoLimiter("loadtest");

                    var path = ctx.Request.Path.Value ?? "";
                    if (path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/img/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/fonts/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
                        return RateLimitPartition.GetNoLimiter("static");

                    return RateLimitPartition.GetTokenBucketLimiter(ClientKey(ctx), _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            // Burst of 240 absorbs a normal page + its XHRs, then
                            // refills at 4 req/s sustained.
                            TokenLimit = 240,
                            TokensPerPeriod = 40,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });

                // ── Public form writes ────────────────────────────────────────
                // A human sends one message; five in ten minutes is already
                // generous and makes bulk spam pointless.
                o.AddPolicy(Forms, ctx =>
                    RateLimitPartition.GetFixedWindowLimiter(ClientKey(ctx), _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        }));

                // ── Login ─────────────────────────────────────────────────────
                // Identity lockout already protects a *known* account; this also
                // covers username enumeration and spraying across many accounts,
                // which per-account lockout cannot see.
                o.AddPolicy(Login, ctx =>
                    RateLimitPartition.GetFixedWindowLimiter(ClientKey(ctx), _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(15),
                            QueueLimit = 0
                        }));

                // Tell well-behaved clients when to come back instead of leaving
                // them to retry blindly.
                o.OnRejected = async (ctx, token) =>
                {
                    if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry))
                        ctx.HttpContext.Response.Headers.RetryAfter =
                            ((int)retry.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);

                    ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    var ar = (ctx.HttpContext.Request.Path.Value ?? "").StartsWith("/ar",
                        StringComparison.OrdinalIgnoreCase);

                    await ctx.HttpContext.Response.WriteAsync(ar
                        ? "عدد كبير من الطلبات. من فضلك حاول مرة أخرى بعد قليل."
                        : "Too many requests. Please try again shortly.", token);
                };
            });

            return services;
        }

        /// <summary>
        /// Partition key for a caller. ForwardedHeaders has already replaced
        /// RemoteIpAddress with the real client address when the request came
        /// through a proxy we trust, so reading the connection is correct here
        /// and cannot be spoofed by an untrusted client.
        /// </summary>
        private static string ClientKey(HttpContext ctx)
            => ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        /// <summary>Constant-time check of the load-test bypass header.</summary>
        private static bool IsLoadTest(HttpContext ctx)
        {
            if (string.IsNullOrEmpty(_bypassSecret)) return false;
            if (!ctx.Request.Headers.TryGetValue(BypassHeader, out var v)) return false;

            return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(v.ToString()),
                System.Text.Encoding.UTF8.GetBytes(_bypassSecret));
        }
    }
}
