using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class GalleryController : PublicControllerBase
    {
        public GalleryController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            if (!Cfg.GetBool("feature.gallery", true)) return NotFound();

            var sections = await LoadSectionsAsync("gallery");
            SeoFromSection(sections, "hero", Cfg.T("nav.gallery"));

            var vm = new GalleryVm
            {
                Albums = await Db.GalleryAlbums.AsNoTracking()
                    .Include(x => x.Images.OrderBy(i => i.SortOrder))
                    .Where(x => x.IsPublished).OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }

        [Route("/{culture:regex(^ar|en$)}/gallery/{slug}")]
        public async Task<IActionResult> Details(string slug)
        {
            if (!Cfg.GetBool("feature.gallery", true)) return NotFound();
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/gallery");

            var album = await Db.GalleryAlbums.AsNoTracking()
                .Include(x => x.Images.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsPublished);

            if (album is null) return NotFound();

            await LoadSectionsAsync("gallery");
            Seo(Lang.Pick(album.TitleAr, album.TitleEn),
                Lang.Pick(album.DescAr, album.DescEn),
                album.CoverImagePath);

            var vm = new GalleryDetailsVm
            {
                Album = album,
                Others = await Db.GalleryAlbums.AsNoTracking()
                    .Where(x => x.IsPublished && x.Id != album.Id)
                    .OrderBy(x => x.SortOrder).Take(4).ToListAsync()
            };

            return View(vm);
        }
    }
}
