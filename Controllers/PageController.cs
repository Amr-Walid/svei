using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers
{
    /// <summary>Renders admin-authored static pages (privacy, terms, custom …).</summary>
    public class PageController : PublicControllerBase
    {
        public PageController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        [Route("/{culture:regex(^ar|en$)}/page/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return NotFound();

            var page = await Db.StaticPages.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsPublished);

            if (page is null) return NotFound();

            Seo(page.MetaTitle ?? Lang.Pick(page.TitleAr, page.TitleEn),
                page.MetaDesc ?? Trim(Lang.Pick(page.BodyAr, page.BodyEn), 200),
                page.HeroImagePath);

            return View(page);
        }
    }
}
