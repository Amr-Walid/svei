using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class NewsController : PublicControllerBase
    {
        private const int PageSize = 9;

        public NewsController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index(string? category, string? q, int page = 1)
        {
            if (!Cfg.GetBool("feature.news", true)) return NotFound();

            var sections = await LoadSectionsAsync("news");
            SeoFromSection(sections, "hero", Cfg.T("nav.news"));

            var now = DateTime.UtcNow;
            var query = Db.NewsPosts.AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.IsPublished && x.PublishedAt <= now);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category != null && x.Category.Slug == category);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x =>
                    x.TitleAr.Contains(term) || x.TitleEn.Contains(term) ||
                    (x.ExcerptAr != null && x.ExcerptAr.Contains(term)) ||
                    (x.ExcerptEn != null && x.ExcerptEn.Contains(term)) ||
                    (x.Tags != null && x.Tags.Contains(term)));
            }

            var total = await query.CountAsync();
            page = Math.Max(1, page);

            var items = await query
                .OrderByDescending(x => x.PublishedAt)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .ToListAsync();

            var vm = new NewsVm
            {
                Paged = new PagedVm<NewsPost>
                {
                    Items = items, Page = page, PageSize = PageSize, TotalItems = total,
                    Query = q, Filter = category
                },
                Categories = await Db.NewsCategories.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                CategorySlug = category
            };

            // Featured strip only on the unfiltered first page
            if (page == 1 && string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(q))
            {
                vm.Featured = await Db.NewsPosts.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsPublished && x.IsFeatured && x.PublishedAt <= now)
                    .OrderByDescending(x => x.PublishedAt).Take(3).ToListAsync();
            }

            return View(vm);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (!Cfg.GetBool("feature.news", true)) return NotFound();
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/news");

            var post = await Db.NewsPosts.AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsPublished);

            if (post is null) return NotFound();

            await Db.NewsPosts.Where(x => x.Id == post.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

            await LoadSectionsAsync("news");
            Seo(post.MetaTitle ?? Lang.Pick(post.TitleAr, post.TitleEn),
                post.MetaDesc ?? Lang.Pick(post.ExcerptAr, post.ExcerptEn),
                post.CoverImagePath);

            var vm = new NewsDetailsVm
            {
                Post = post,
                Related = await Db.NewsPosts.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsPublished && x.Id != post.Id && x.CategoryId == post.CategoryId)
                    .OrderByDescending(x => x.PublishedAt).Take(3).ToListAsync(),
                Prev = await Db.NewsPosts.AsNoTracking()
                    .Where(x => x.IsPublished && x.PublishedAt < post.PublishedAt)
                    .OrderByDescending(x => x.PublishedAt).FirstOrDefaultAsync(),
                Next = await Db.NewsPosts.AsNoTracking()
                    .Where(x => x.IsPublished && x.PublishedAt > post.PublishedAt)
                    .OrderBy(x => x.PublishedAt).FirstOrDefaultAsync()
            };

            // Not enough same-category posts? Backfill with the newest ones.
            if (vm.Related.Count < 3)
            {
                var ids = vm.Related.Select(r => r.Id).Append(post.Id).ToList();
                var fill = await Db.NewsPosts.AsNoTracking()
                    .Include(x => x.Category)
                    .Where(x => x.IsPublished && !ids.Contains(x.Id))
                    .OrderByDescending(x => x.PublishedAt).Take(3 - vm.Related.Count).ToListAsync();
                vm.Related.AddRange(fill);
            }

            return View(vm);
        }
    }
}
