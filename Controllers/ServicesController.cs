using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class ServicesController : PublicControllerBase
    {
        public ServicesController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            var sections = await LoadSectionsAsync("services");
            SeoFromSection(sections, "hero", Cfg.T("nav.services"));

            var vm = new ServicesVm
            {
                Services = await Db.ServiceItems.AsNoTracking()
                    .Include(x => x.Features)
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Process = await Db.InfoCards.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "services.process")
                    .OrderBy(x => x.SortOrder).ToListAsync(),

                Faqs = await Db.FaqItems.AsNoTracking()
                    .Where(x => x.IsActive && (x.GroupKey == "services" || x.GroupKey == "general"))
                    .OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/services");

            var service = await Db.ServiceItems.AsNoTracking()
                .Include(x => x.Features.OrderBy(f => f.SortOrder))
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            if (service is null) return NotFound();

            await LoadSectionsAsync("services");
            Seo(service.MetaTitle ?? Lang.Pick(service.TitleAr, service.TitleEn),
                service.MetaDesc ?? Lang.Pick(service.ShortDescAr, service.ShortDescEn),
                service.ImagePath);

            var vm = new ServiceDetailsVm
            {
                Service = service,
                Others = await Db.ServiceItems.AsNoTracking()
                    .Where(x => x.IsActive && x.Id != service.Id)
                    .OrderBy(x => x.SortOrder).Take(4).ToListAsync(),
                Faqs = await Db.FaqItems.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "services")
                    .OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }
    }
}
