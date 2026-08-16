using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Data.Seed;
using SVEI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────────────────
var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    else
        opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=svei.db");
});

// ── Identity ──────────────────────────────────────────────────────────────────
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(o =>
    {
        o.Password.RequiredLength = 8;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
        o.User.RequireUniqueEmail = true;
        o.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(o =>
{
    // Five wrong passwords locks the account for 15 minutes. Combined with the
    // per-IP login limiter this makes both single-account brute force and
    // cross-account password spraying impractical.
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.AllowedForNewUsers = true;
});

// Secure-only cookies are the correct production posture: svei.tech is served
// over HTTPS, so the auth and antiforgery cookies must never be allowed onto a
// plain-HTTP connection where they could be read in transit.
//
// It has to stay overridable, though, because a plain-HTTP listener (the local
// preview) cannot issue a Secure cookie at all — the antiforgery system throws
// outright rather than silently degrading. Secure by default, opt out only for
// an HTTP preview via Security:RequireSecureCookies=false.
var requireSecureCookies =
    builder.Configuration.GetValue("Security:RequireSecureCookies", true);

var cookieSecurity = requireSecureCookies
    ? CookieSecurePolicy.Always
    : CookieSecurePolicy.SameAsRequest;

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.LogoutPath = "/Account/Logout";
    o.AccessDeniedPath = "/Account/Denied";
    // Shorter than the previous 7 days: this cookie is a standing key to the
    // whole CMS, and sliding expiration keeps an active admin signed in anyway.
    o.ExpireTimeSpan = TimeSpan.FromHours(12);
    o.SlidingExpiration = true;
    o.Cookie.Name = "svei_auth";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;          // Lax, not Strict, so the post-login redirect works
    o.Cookie.SecurePolicy = cookieSecurity;
    o.Cookie.IsEssential = true;
});

// The antiforgery cookie deserves the same treatment as the auth cookie.
builder.Services.AddAntiforgery(o =>
{
    o.Cookie.Name = "svei_csrf";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.Cookie.SecurePolicy = cookieSecurity;
});

// ── App services ──────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILang, LangService>();
builder.Services.AddScoped<ISettings, SettingsService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddSingleton<IHtmlSanitizer, HtmlSanitizerService>();

builder.Services.AddSveiRateLimiter(builder.Configuration);

// Compression makes the HTML/CSS/JS payload a fraction of the size, which is
// the single biggest win for both perceived speed and how many concurrent
// visitors a given amount of bandwidth can serve.
builder.Services.AddResponseCompression(o =>
{
    o.EnableForHttps = true;
    o.Providers.Add<BrotliCompressionProvider>();
    o.Providers.Add<GzipCompressionProvider>();
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/json", "image/svg+xml", "application/xml", "text/xml"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

builder.Services.AddControllersWithViews();
builder.Services.AddResponseCaching();

// Behind the hosting reverse proxy, honour the real client scheme and IP so
// redirects use https and the rate limiter partitions per visitor rather than
// lumping every visitor under the proxy's address.
//
// X-Forwarded-For is only trusted from proxies we name. Accepting it from any
// source would let an attacker put a random address in the header on every
// request and, because the rate limiter partitions on client IP, get an
// unlimited budget — it would also poison the IP recorded in the audit log and
// on every contact message. Configure Security:KnownProxies with the real proxy
// address(es) when deploying behind one; with none configured the socket
// address is used, which is correct for direct exposure.
var knownProxies = builder.Configuration
    .GetSection("Security:KnownProxies").Get<string[]>() ?? Array.Empty<string>();

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.ForwardLimit = 1;
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();

    foreach (var p in knownProxies)
        if (System.Net.IPAddress.TryParse(p, out var ip))
            o.KnownProxies.Add(ip);
});

// Upload ceilings. The old 100 MB limit applied to *every* endpoint including
// anonymous ones, so a handful of concurrent requests could exhaust memory —
// a trivial denial of service. Individual actions raise this where needed via
// [RequestSizeLimit].
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 30L * 1024 * 1024;    // 30 MB
    o.ValueLengthLimit = 8 * 1024 * 1024;              // 8 MB per field
    o.MemoryBufferThreshold = 128 * 1024;              // spill to disk past 128 KB
    o.MultipartHeadersLengthLimit = 32 * 1024;
    // Cap how many fields a single form post may carry, so a crafted body
    // cannot force thousands of model-binding allocations.
    o.ValueCountLimit = 2048;
    o.KeyLengthLimit = 2048;
});
builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 30L * 1024 * 1024;
    o.Limits.MaxRequestHeadersTotalSize = 32 * 1024;
    o.Limits.MaxConcurrentConnections = 2000;
    o.Limits.MaxConcurrentUpgradedConnections = 200;
    // Drop slowloris-style connections that trickle bytes to hold a thread.
    o.Limits.MinRequestBodyDataRate =
        new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    o.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(20);
    o.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
    o.AddServerHeader = false;
});

var app = builder.Build();

// ── Pipeline ──────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();

// First in the pipeline so the headers are attached to every response, including
// static files, error pages and rate-limiter rejections.
app.UseSveiSecurityHeaders();

app.UseResponseCompression();
app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");

// Long, immutable caching for fingerprinted assets. asp-append-version stamps a
// ?v= hash onto the URL, so the content can never go stale behind the cache and
// repeat visitors stop re-downloading the whole front end.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.Context.Request.Path.Value ?? "";
        var versioned = ctx.Context.Request.Query.ContainsKey("v");
        var cacheable = path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/img/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/fonts/", StringComparison.OrdinalIgnoreCase) ||
                        path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase);

        if (cacheable)
            ctx.Context.Response.Headers.CacheControl = versioned
                ? "public,max-age=31536000,immutable"
                : "public,max-age=604800";
    }
});

app.UseRouting();

// Rate limiting sits after routing (so per-endpoint policies resolve) but before
// the culture middleware, which does a database read on every request and is
// exactly what we do not want an attacker to be able to spam.
app.UseRateLimiter();

// Legacy redirects + culture prefix ("/" → "/ar", "/index.html" → "/ar")
app.UseSveiCulture();

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCaching();

// ── Routes ────────────────────────────────────────────────────────────────────
// Admin (no culture prefix — admin UI is Arabic RTL)
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "" });

app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Login}/{id?}",
    defaults: new { controller = "Account" });

// Culture-prefixed public routes: /ar/... and /en/...
app.MapControllerRoute(
    name: "news-detail",
    pattern: "{culture:regex(^ar|en$)}/news/{slug}",
    defaults: new { controller = "News", action = "Details" });

app.MapControllerRoute(
    name: "events-detail",
    pattern: "{culture:regex(^ar|en$)}/events/{slug}",
    defaults: new { controller = "Events", action = "Details" });

app.MapControllerRoute(
    name: "careers-detail",
    pattern: "{culture:regex(^ar|en$)}/careers/{slug}",
    defaults: new { controller = "Careers", action = "Details" });

app.MapControllerRoute(
    name: "products-detail",
    pattern: "{culture:regex(^ar|en$)}/products/{slug}",
    defaults: new { controller = "Products", action = "Details" });

app.MapControllerRoute(
    name: "services-detail",
    pattern: "{culture:regex(^ar|en$)}/services/{slug}",
    defaults: new { controller = "Services", action = "Details" });

app.MapControllerRoute(
    name: "lines-detail",
    pattern: "{culture:regex(^ar|en$)}/production-lines/{slug}",
    defaults: new { controller = "ProductionLines", action = "Details" });

// Friendly aliases so URLs read naturally in both languages
app.MapControllerRoute(
    name: "production-lines",
    pattern: "{culture:regex(^ar|en$)}/production-lines",
    defaults: new { controller = "ProductionLines", action = "Index" });

app.MapControllerRoute(
    name: "localized-default",
    pattern: "{culture:regex(^ar|en$)}/{controller=Home}/{action=Index}/{id?}");

// Bare "/" and any unprefixed path → redirect to the default culture
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Migrate + seed ────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedRunner.RunAsync(sp);
}

app.Run();
