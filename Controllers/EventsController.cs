using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class EventsController : PublicControllerBase
    {
        private const int PageSize = 9;

        public EventsController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        // ══════════════════════════════════════════════════════════════════════
        //  LIST
        // ══════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Index(string status = "all", string? type = null, string? q = null, int page = 1)
        {
            if (!Cfg.GetBool("feature.events", true)) return NotFound();

            var sections = await LoadSectionsAsync("events");
            SeoFromSection(sections, "hero", Cfg.T("nav.events"));

            var today = DateTime.UtcNow.Date;
            var query = Db.Events.AsNoTracking().Where(x => x.IsPublished);

            query = status switch
            {
                "upcoming" => query.Where(x => x.StartDate != null && x.StartDate > today),
                "ongoing"  => query.Where(x => x.StartDate != null && x.StartDate <= today &&
                                               (x.EndDate ?? x.StartDate) >= today),
                "past"     => query.Where(x => (x.EndDate ?? x.StartDate) < today),
                _          => query
            };

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.EventType == type);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x =>
                    x.TitleAr.Contains(term) || x.TitleEn.Contains(term) ||
                    (x.SummaryAr != null && x.SummaryAr.Contains(term)) ||
                    (x.SummaryEn != null && x.SummaryEn.Contains(term)));
            }

            var total = await query.CountAsync();
            page = Math.Max(1, page);

            // Upcoming first (soonest first), then past (most recent first)
            var items = await query
                .OrderByDescending(x => x.StartDate >= today)
                .ThenBy(x => x.StartDate >= today ? x.StartDate : null)
                .ThenByDescending(x => x.StartDate)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .ToListAsync();

            var vm = new EventsVm
            {
                Paged = new PagedVm<Event>
                {
                    Items = items, Page = page, PageSize = PageSize, TotalItems = total,
                    Query = q, Filter = status
                },
                Status = status,
                EventType = type,
                Types = await Db.Events.AsNoTracking()
                    .Where(x => x.IsPublished)
                    .Select(x => x.EventType).Distinct().ToListAsync()
            };

            // Highlighted upcoming strip on the unfiltered first page
            if (page == 1 && status == "all" && string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(q))
            {
                vm.Upcoming = await Db.Events.AsNoTracking()
                    .Where(x => x.IsPublished && (x.EndDate ?? x.StartDate) >= today)
                    .OrderBy(x => x.StartDate).Take(3).ToListAsync();
            }

            return View(vm);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DETAILS
        // ══════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Details(string slug)
        {
            if (!Cfg.GetBool("feature.events", true)) return NotFound();
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/events");

            var ev = await Db.Events.AsNoTracking()
                .Include(x => x.Images.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsPublished);

            if (ev is null) return NotFound();

            await Db.Events.Where(x => x.Id == ev.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

            await LoadSectionsAsync("events");
            Seo(ev.MetaTitle ?? Lang.Pick(ev.TitleAr, ev.TitleEn),
                ev.MetaDesc ?? Lang.Pick(ev.SummaryAr, ev.SummaryEn),
                ev.CoverImagePath);

            var registered = await Db.EventRegistrations.CountAsync(r => r.EventId == ev.Id);

            var vm = new EventDetailsVm
            {
                Event = ev,
                RegisteredCount = registered,
                IsFull = ev.Capacity.HasValue && registered >= ev.Capacity.Value,
                Form = new EventRegistrationForm { EventId = ev.Id },
                Related = await Db.Events.AsNoTracking()
                    .Where(x => x.IsPublished && x.Id != ev.Id)
                    .OrderByDescending(x => x.StartDate).Take(3).ToListAsync()
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  REGISTER
        // ══════════════════════════════════════════════════════════════════════
        [HttpPost("/{culture:regex(^ar|en$)}/events/register"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(EventRegistrationForm form, string? website)
        {
            if (!Cfg.GetBool("feature.events", true)) return NotFound();

            var ev = await Db.Events.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == form.EventId && x.IsPublished);
            if (ev is null) return NotFound();

            var backUrl = $"/events/{ev.Slug}";

            if (IsSpam(website)) return RedirectLocalized(backUrl);

            if (!ev.AllowRegistration)
            {
                TempData["Error"] = Cfg.T("msg.form_error");
                return RedirectLocalized(backUrl);
            }

            if (string.IsNullOrWhiteSpace(form.FullName) || string.IsNullOrWhiteSpace(form.Email))
            {
                TempData["Error"] = Cfg.T("msg.form_error");
                return RedirectLocalized(backUrl);
            }

            // Capacity guard
            if (ev.Capacity.HasValue)
            {
                var count = await Db.EventRegistrations.CountAsync(r => r.EventId == ev.Id);
                if (count >= ev.Capacity.Value)
                {
                    TempData["Error"] = Lang.IsAr ? "اكتمل العدد لهذه الفعالية." : "This event is fully booked.";
                    return RedirectLocalized(backUrl);
                }
            }

            // Duplicate guard
            var email = form.Email.Trim().ToLowerInvariant();
            var exists = await Db.EventRegistrations
                .AnyAsync(r => r.EventId == ev.Id && r.Email.ToLower() == email);

            if (!exists)
            {
                Db.EventRegistrations.Add(new EventRegistration
                {
                    EventId  = ev.Id,
                    FullName = form.FullName.Trim(),
                    Email    = email,
                    Phone    = form.Phone?.Trim(),
                    Company  = form.Company?.Trim(),
                    Notes    = form.Notes?.Trim()
                });
                await Db.SaveChangesAsync();
            }

            return Success(backUrl, "msg.registered");
        }
    }
}
