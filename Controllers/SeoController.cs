using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers
{
    /// <summary>robots.txt and a bilingual sitemap.xml generated from the database.</summary>
    public class SeoController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ISettings _cfg;

        public SeoController(AppDbContext db, ISettings cfg) { _db = db; _cfg = cfg; }

        [Route("/robots.txt")]
        public IActionResult Robots()
        {
            var baseUrl = BaseUrl();
            var sb = new StringBuilder()
                .AppendLine("User-agent: *")
                .AppendLine("Disallow: /Admin/")
                .AppendLine("Disallow: /Account/")
                .AppendLine("Allow: /")
                .AppendLine()
                .AppendLine($"Sitemap: {baseUrl}/sitemap.xml");

            return Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }

        [Route("/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var baseUrl = BaseUrl();
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(ns + "urlset");

            void Add(string path, DateTime? lastMod = null, string freq = "weekly", string priority = "0.7")
            {
                foreach (var culture in LangService.Supported)
                {
                    var loc = $"{baseUrl}/{culture}{(path == "/" ? "" : path)}";
                    var el = new XElement(ns + "url",
                        new XElement(ns + "loc", loc),
                        new XElement(ns + "changefreq", freq),
                        new XElement(ns + "priority", priority));
                    if (lastMod.HasValue)
                        el.Add(new XElement(ns + "lastmod", lastMod.Value.ToString("yyyy-MM-dd")));
                    root.Add(el);
                }
            }

            // Static pages
            Add("/", null, "daily", "1.0");
            Add("/about", null, "monthly", "0.8");
            Add("/services", null, "monthly", "0.8");
            Add("/production-lines", null, "monthly", "0.8");
            Add("/brands", null, "monthly", "0.6");
            Add("/contact", null, "yearly", "0.7");
            if (_cfg.GetBool("feature.quote", true)) Add("/quote", null, "yearly", "0.8");
            if (_cfg.GetBool("feature.products", true)) Add("/products", null, "weekly", "0.8");
            if (_cfg.GetBool("feature.news", true)) Add("/news", null, "daily", "0.8");
            if (_cfg.GetBool("feature.events", true)) Add("/events", null, "daily", "0.8");
            if (_cfg.GetBool("feature.careers", true)) Add("/careers", null, "daily", "0.8");
            if (_cfg.GetBool("feature.gallery", true)) Add("/gallery", null, "monthly", "0.6");

            // Dynamic content
            foreach (var s in await _db.ServiceItems.AsNoTracking().Where(x => x.IsActive)
                         .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                Add($"/services/{s.Slug}", s.UpdatedAt, "monthly", "0.7");

            foreach (var l in await _db.ProductionLines.AsNoTracking().Where(x => x.IsActive)
                         .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                Add($"/production-lines/{l.Slug}", l.UpdatedAt, "monthly", "0.7");

            if (_cfg.GetBool("feature.products", true))
                foreach (var p in await _db.Products.AsNoTracking().Where(x => x.IsActive)
                             .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                    Add($"/products/{p.Slug}", p.UpdatedAt, "weekly", "0.6");

            if (_cfg.GetBool("feature.news", true))
                foreach (var n in await _db.NewsPosts.AsNoTracking().Where(x => x.IsPublished)
                             .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                    Add($"/news/{n.Slug}", n.UpdatedAt, "monthly", "0.6");

            if (_cfg.GetBool("feature.events", true))
                foreach (var e in await _db.Events.AsNoTracking().Where(x => x.IsPublished)
                             .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                    Add($"/events/{e.Slug}", e.UpdatedAt, "weekly", "0.6");

            if (_cfg.GetBool("feature.careers", true))
                foreach (var j in await _db.JobPostings.AsNoTracking().Where(x => x.IsActive)
                             .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                    Add($"/careers/{j.Slug}", j.UpdatedAt, "weekly", "0.6");

            if (_cfg.GetBool("feature.gallery", true))
                foreach (var g in await _db.GalleryAlbums.AsNoTracking().Where(x => x.IsPublished)
                             .Select(x => new { x.Slug, x.CreatedAt }).ToListAsync())
                    Add($"/gallery/{g.Slug}", g.CreatedAt, "monthly", "0.5");

            foreach (var p in await _db.StaticPages.AsNoTracking().Where(x => x.IsPublished)
                         .Select(x => new { x.Slug, x.UpdatedAt }).ToListAsync())
                Add($"/page/{p.Slug}", p.UpdatedAt, "yearly", "0.3");

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), root);
            return Content(doc.ToString(), "application/xml", Encoding.UTF8);
        }

        private string BaseUrl()
        {
            var configured = _cfg.GetRaw("site.base_url");
            return string.IsNullOrWhiteSpace(configured)
                ? $"{Request.Scheme}://{Request.Host}"
                : configured.TrimEnd('/');
        }
    }
}
