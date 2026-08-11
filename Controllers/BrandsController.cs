using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class BrandsController : PublicControllerBase
    {
        public BrandsController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            var sections = await LoadSectionsAsync("brands");
            SeoFromSection(sections, "hero", Cfg.T("nav.brands"));

            var all = await Db.Brands.AsNoTracking()
                .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync();

            var vm = new BrandsVm
            {
                Clients   = all.Where(x => x.BrandType == "client").ToList(),
                Partners  = all.Where(x => x.BrandType == "partner").ToList(),
                Suppliers = all.Where(x => x.BrandType is "supplier" or "certification").ToList()
            };

            // If nothing was categorised, treat everything as a client logo.
            if (vm.Clients.Count == 0 && vm.Partners.Count == 0 && vm.Suppliers.Count == 0)
                vm.Clients = all;

            return View(vm);
        }
    }
}
