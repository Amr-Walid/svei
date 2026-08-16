using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class CareersController : PublicControllerBase
    {
        private const int PageSize = 10;
        private readonly IMediaService _media;

        public CareersController(AppDbContext db, ILang lang, ISettings cfg, IMediaService media)
            : base(db, lang, cfg) { _media = media; }

        // ══════════════════════════════════════════════════════════════════════
        //  LIST
        // ══════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Index(string? category, string? type, string? q, int page = 1)
        {
            if (!Cfg.GetBool("feature.careers", true)) return NotFound();

            var sections = await LoadSectionsAsync("careers");
            SeoFromSection(sections, "hero", Cfg.T("nav.careers"));

            var today = DateTime.UtcNow.Date;

            // Every posting that is currently open. Used both as the base for the
            // result list and as the source of the filter facets below.
            var live = Db.JobPostings.AsNoTracking()
                .Where(x => x.IsActive && (x.ClosingDate == null || x.ClosingDate >= today));

            // Only offer a chip when at least one open posting would match it —
            // otherwise a visitor can pick a department or a contract type and land
            // on an empty result set. Facets ignore the active category/type filter
            // so the visitor can always switch between them.
            var liveCategoryIds = await live.Select(x => x.CategoryId).Distinct().ToListAsync();
            var liveTypes = await live
                .Where(x => x.JobType != "")
                .Select(x => x.JobType).Distinct().ToListAsync();

            // Typed as IQueryable so the conditional .Where() calls below can
            // reassign it (Include() returns the narrower IIncludableQueryable).
            IQueryable<JobPosting> query = live.Include(x => x.Category);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category.Slug == category);
            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.JobType == type);

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

            var items = await query
                .OrderByDescending(x => x.IsUrgent).ThenBy(x => x.SortOrder).ThenByDescending(x => x.PostedAt)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .ToListAsync();

            var vm = new CareersVm
            {
                Paged = new PagedVm<JobPosting>
                {
                    Items = items, Page = page, PageSize = PageSize, TotalItems = total,
                    Query = q, Filter = category
                },
                Categories = await Db.JobCategories.AsNoTracking()
                    .Where(x => x.IsActive && liveCategoryIds.Contains(x.Id))
                    .OrderBy(x => x.SortOrder).ToListAsync(),
                // Preserve the canonical display order from JobLabels.Types.
                Types = JobLabels.Types.Where(t => liveTypes.Contains(t)).ToList(),
                WhyUs = await Db.InfoCards.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "careers.why_us")
                    .OrderBy(x => x.SortOrder).ToListAsync(),
                CategorySlug = category,
                JobType = type
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DETAILS
        // ══════════════════════════════════════════════════════════════════════
        public async Task<IActionResult> Details(string slug)
        {
            if (!Cfg.GetBool("feature.careers", true)) return NotFound();
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/careers");

            var job = await Db.JobPostings.AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            if (job is null) return NotFound();

            await Db.JobPostings.Where(x => x.Id == job.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

            await LoadSectionsAsync("careers");
            Seo(job.MetaTitle ?? Lang.Pick(job.TitleAr, job.TitleEn),
                job.MetaDesc ?? Lang.Pick(job.SummaryAr, job.SummaryEn));

            var vm = new JobDetailsVm
            {
                Job = job,
                Form = new JobApplicationForm { JobId = job.Id },
                Similar = await Db.JobPostings.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsActive && x.Id != job.Id && x.CategoryId == job.CategoryId)
                    .OrderByDescending(x => x.PostedAt).Take(3).ToListAsync()
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  APPLY
        // ══════════════════════════════════════════════════════════════════════
        [HttpPost("/{culture:regex(^ar|en$)}/careers/apply"), ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimiting.Forms)]
        [RequestSizeLimit(30 * 1024 * 1024)]
        public async Task<IActionResult> Apply(JobApplicationForm form, string? website)
        {
            if (!Cfg.GetBool("feature.careers", true)) return NotFound();

            var job = await Db.JobPostings.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == form.JobId && x.IsActive);
            if (job is null) return NotFound();

            var backUrl = $"/careers/{job.Slug}";

            if (IsSpam(website)) return RedirectLocalized(backUrl);

            if (job.IsClosed)
            {
                TempData["Error"] = Lang.IsAr ? "انتهى موعد التقديم لهذه الوظيفة." : "Applications for this job are closed.";
                return RedirectLocalized(backUrl);
            }

            if (string.IsNullOrWhiteSpace(form.FullName) ||
                string.IsNullOrWhiteSpace(form.Email) ||
                string.IsNullOrWhiteSpace(form.Phone))
            {
                TempData["Error"] = Cfg.T("msg.form_error");
                return RedirectLocalized(backUrl);
            }

            var email = form.Email.Trim().ToLowerInvariant();

            // Prevent duplicate applications to the same job
            var already = await Db.JobApplications
                .AnyAsync(a => a.JobId == job.Id && a.Email.ToLower() == email);
            if (already)
            {
                TempData["Error"] = Lang.IsAr
                    ? "لقد قدّمت على هذه الوظيفة من قبل."
                    : "You have already applied for this position.";
                return RedirectLocalized(backUrl);
            }

            var cvPath = await _media.SaveFileAsync(form.Cv, "cv");

            Db.JobApplications.Add(new JobApplication
            {
                JobId             = job.Id,
                FullName          = form.FullName.Trim(),
                Email             = email,
                Phone             = form.Phone.Trim(),
                City              = form.City?.Trim(),
                YearsOfExperience = form.YearsOfExperience,
                LinkedInUrl       = form.LinkedInUrl?.Trim(),
                CoverLetter       = form.CoverLetter?.Trim(),
                CvPath            = cvPath
            });
            await Db.SaveChangesAsync();

            return Success(backUrl, "msg.applied");
        }
    }
}
