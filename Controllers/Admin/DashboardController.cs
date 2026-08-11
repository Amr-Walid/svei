using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels.Admin;

namespace SVEI.Web.Controllers.Admin
{
    [Route("Admin")]
    public class DashboardController : AdminBaseController
    {
        public DashboardController(AppDbContext db, ILang lang) : base(db, lang) { }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var vm = new DashboardVm
            {
                Products        = await Db.Products.CountAsync(),
                ProductionLines = await Db.ProductionLines.CountAsync(),
                News            = await Db.NewsPosts.CountAsync(),
                Events          = await Db.Events.CountAsync(),
                Jobs            = await Db.JobPostings.CountAsync(),
                Brands          = await Db.Brands.CountAsync(),
                Services        = await Db.ServiceItems.CountAsync(),
                GalleryImages   = await Db.GalleryImages.CountAsync(),
                Pages           = await Db.StaticPages.CountAsync(),
                Settings        = await Db.SiteSettings.CountAsync(),
                Translations    = await Db.Translations.CountAsync(),
                Sections        = await Db.PageSections.CountAsync(),

                UnreadMessages      = await Db.ContactMessages.CountAsync(x => !x.IsRead && !x.IsArchived),
                UnreadQuotes        = await Db.QuoteRequests.CountAsync(x => !x.IsRead),
                UnreadApplications  = await Db.JobApplications.CountAsync(x => !x.IsRead),
                UnreadRegistrations = await Db.EventRegistrations.CountAsync(x => !x.IsRead),
                UnreadPartners      = await Db.PartnerRequests.CountAsync(x => !x.IsRead),
                Subscribers         = await Db.NewsletterSubscribers.CountAsync(x => x.IsActive),
            };

            // ── recent inbox activity (merged feed) ──────────────────────────
            var recent = new List<RecentItem>();

            recent.AddRange(await Db.ContactMessages.OrderByDescending(x => x.CreatedAt).Take(6)
                .Select(x => new RecentItem(
                    IsAr ? "رسالة" : "Message", "fa-solid fa-envelope",
                    x.FullName + (x.Subject != null ? " — " + x.Subject : ""),
                    "/Admin/e/messages/edit/" + x.Id, x.CreatedAt, !x.IsRead)).ToListAsync());

            recent.AddRange(await Db.QuoteRequests.OrderByDescending(x => x.CreatedAt).Take(6)
                .Select(x => new RecentItem(
                    IsAr ? "طلب سعر" : "Quote", "fa-solid fa-file-invoice-dollar",
                    x.FullName + (x.Company != null ? " — " + x.Company : ""),
                    "/Admin/e/quotes/edit/" + x.Id, x.CreatedAt, !x.IsRead)).ToListAsync());

            recent.AddRange(await Db.JobApplications.Include(x => x.Job)
                .OrderByDescending(x => x.CreatedAt).Take(6)
                .Select(x => new RecentItem(
                    IsAr ? "طلب توظيف" : "Application", "fa-solid fa-file-signature",
                    x.FullName + " — " + (IsAr ? x.Job.TitleAr : x.Job.TitleEn),
                    "/Admin/e/applications/edit/" + x.Id, x.CreatedAt, !x.IsRead)).ToListAsync());

            recent.AddRange(await Db.EventRegistrations.Include(x => x.Event)
                .OrderByDescending(x => x.CreatedAt).Take(6)
                .Select(x => new RecentItem(
                    IsAr ? "تسجيل فعالية" : "Registration", "fa-solid fa-user-check",
                    x.FullName + " — " + (IsAr ? x.Event.TitleAr : x.Event.TitleEn),
                    "/Admin/e/event-registrations/edit/" + x.Id, x.CreatedAt, !x.IsRead)).ToListAsync());

            vm.Recent = recent.OrderByDescending(r => r.When).Take(12).ToList();

            // ── upcoming events ──────────────────────────────────────────────
            var now = DateTime.UtcNow.Date;
            vm.Upcoming = await Db.Events
                .Where(e => e.StartDate != null && e.StartDate >= now)
                .OrderBy(e => e.StartDate).Take(5)
                .Select(e => new UpcomingItem(
                    IsAr ? e.TitleAr : e.TitleEn, e.StartDate,
                    "/Admin/e/events/edit/" + e.Id, e.EventType))
                .ToListAsync();

            // ── audit trail ──────────────────────────────────────────────────
            vm.Activity = await Db.AuditLogs.OrderByDescending(a => a.CreatedAt).Take(10)
                .Select(a => new AuditItem(a.UserName, a.Action, a.EntityName, a.Summary, a.CreatedAt))
                .ToListAsync();

            return View("~/Views/Admin/Dashboard/Index.cshtml", vm);
        }
    }
}
