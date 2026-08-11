using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
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

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.LogoutPath = "/Account/Logout";
    o.AccessDeniedPath = "/Account/Denied";
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
    o.Cookie.Name = "svei_auth";
});

// ── App services ──────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILang, LangService>();
builder.Services.AddScoped<ISettings, SettingsService>();
builder.Services.AddScoped<IMediaService, MediaService>();

builder.Services.AddControllersWithViews();

// Large uploads (factory photo galleries, CVs, datasheets)
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 100L * 1024 * 1024;   // 100 MB
    o.ValueLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 100L * 1024 * 1024);

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

app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");
app.UseStaticFiles();
app.UseRouting();

// Legacy redirects + culture prefix ("/" → "/ar", "/index.html" → "/ar")
app.UseSveiCulture();

app.UseAuthentication();
app.UseAuthorization();

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
