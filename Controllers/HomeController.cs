using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class HomeController : PublicControllerBase
    {
        public HomeController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            // Bare "/" (no culture segment) → send to the default culture
            if (!Request.RouteValues.ContainsKey("culture"))
                return RedirectLocalized("/");

            await LoadSectionsAsync("home");
            Seo(null, Cfg.Get("seo.description"));

            var vm = new HomeVm
            {
                Slides = await Db.HeroSlides.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Stats = await Db.StatCounters.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "home")
                    .OrderBy(x => x.SortOrder).ToListAsync(),

                Features = await Db.InfoCards.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "home.features")
                    .OrderBy(x => x.SortOrder).ToListAsync(),

                Lines = await Db.ProductionLines.AsNoTracking()
                    .Include(x => x.Specs)
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Services = await Db.ServiceItems.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).Take(6).ToListAsync(),

                Brands = await Db.Brands.AsNoTracking()
                    .Where(x => x.IsActive && x.ShowInStrip)
                    .OrderBy(x => x.SortOrder).ToListAsync(),

                Certifications = await Db.Certifications.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),

                Testimonials = await Db.Testimonials.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync()
            };

            if (Cfg.GetBool("feature.products", true))
            {
                vm.FeaturedProducts = await Db.Products.AsNoTracking()
                    .Where(x => x.IsActive && x.IsFeatured)
                    .OrderBy(x => x.SortOrder).Take(8).ToListAsync();
            }

            // The `feature.*` flags gate a whole module (its pages and sitemap entries),
            // whereas the `home.show_*` flags below only control whether that module
            // also gets a teaser block on the home page. They default to false so the
            // home page stays focused on the factory itself; /news, /events and
            // /careers keep working either way and remain in the nav and sitemap.
            if (Cfg.GetBool("feature.news", true) && Cfg.GetBool("home.show_news", false))
            {
                vm.LatestNews = await Db.NewsPosts.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsPublished && x.PublishedAt <= DateTime.UtcNow)
                    .OrderByDescending(x => x.PublishedAt).Take(3).ToListAsync();
            }

            if (Cfg.GetBool("feature.events", true) && Cfg.GetBool("home.show_events", false))
            {
                var today = DateTime.UtcNow.Date;
                vm.UpcomingEvents = await Db.Events.AsNoTracking()
                    .Where(x => x.IsPublished && (x.EndDate ?? x.StartDate) >= today)
                    .OrderBy(x => x.StartDate).Take(3).ToListAsync();

                // Nothing upcoming? Show the most recent past events instead of an empty block.
                if (vm.UpcomingEvents.Count == 0)
                {
                    vm.UpcomingEvents = await Db.Events.AsNoTracking()
                        .Where(x => x.IsPublished)
                        .OrderByDescending(x => x.StartDate).Take(3).ToListAsync();
                }
            }

            if (Cfg.GetBool("feature.careers", true) && Cfg.GetBool("home.show_careers", false))
            {
                vm.OpenJobs = await Db.JobPostings.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsActive && (x.ClosingDate == null || x.ClosingDate >= DateTime.UtcNow.Date))
                    .OrderByDescending(x => x.IsUrgent).ThenByDescending(x => x.PostedAt)
                    .Take(4).ToListAsync();
            }

            if (Cfg.GetBool("home.show_faq", false))
            {
                vm.Faqs = await Db.FaqItems.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "general")
                    .OrderBy(x => x.SortOrder).Take(6).ToListAsync();
            }

            return View(vm);
        }

        [Route("/Home/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["Title"] = Lang.IsAr ? "حدث خطأ" : "Something went wrong";
            Response.StatusCode = 500;
            return View("Error");
        }

        [Route("/Home/NotFoundPage")]
        public IActionResult NotFoundPage()
        {
            ViewData["Title"] = Lang.IsAr ? "الصفحة غير موجودة" : "Page not found";
            return View("NotFound");
        }
    }
}
