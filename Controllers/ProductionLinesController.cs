using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class ProductionLinesController : PublicControllerBase
    {
        public ProductionLinesController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            var sections = await LoadSectionsAsync("production-lines");
            SeoFromSection(sections, "hero", Cfg.T("nav.production_lines"));

            var vm = new ProductionLinesVm
            {
                Lines = await Db.ProductionLines.AsNoTracking()
                    .Include(x => x.Specs.OrderBy(s => s.SortOrder))
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Stats = await Db.StatCounters.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/production-lines");

            var line = await Db.ProductionLines.AsNoTracking()
                .Include(x => x.Specs.OrderBy(s => s.SortOrder))
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            if (line is null) return NotFound();

            await LoadSectionsAsync("production-lines");
            Seo(line.MetaTitle ?? Lang.Pick(line.NameAr, line.NameEn),
                line.MetaDesc ?? Lang.Pick(line.ShortDescAr, line.ShortDescEn),
                line.ImagePath);

            var vm = new ProductionLineDetailsVm
            {
                Line = line,
                Products = await Db.Products.AsNoTracking()
                    .Where(x => x.IsActive && x.ProductionLineId == line.Id)
                    .OrderBy(x => x.SortOrder).Take(8).ToListAsync(),
                Others = await Db.ProductionLines.AsNoTracking()
                    .Where(x => x.IsActive && x.Id != line.Id)
                    .OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }
    }
}
