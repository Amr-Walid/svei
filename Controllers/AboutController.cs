using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class AboutController : PublicControllerBase
    {
        public AboutController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            var sections = await LoadSectionsAsync("about");
            SeoFromSection(sections, "hero", Cfg.T("nav.about"));

            var vm = new AboutVm
            {
                Values = await Db.InfoCards.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "about.values")
                    .OrderBy(x => x.SortOrder).ToListAsync(),

                Milestones = await Db.Milestones.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Team = await Db.TeamMembers.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Certifications = await Db.Certifications.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Stats = await Db.StatCounters.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Lines = await Db.ProductionLines.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }
    }
}
