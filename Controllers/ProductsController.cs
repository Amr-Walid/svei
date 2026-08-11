using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class ProductsController : PublicControllerBase
    {
        private const int PageSize = 12;

        public ProductsController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index(string? category, string? brand, string? line, string? q, int page = 1)
        {
            if (!Cfg.GetBool("feature.products", true)) return NotFound();

            var sections = await LoadSectionsAsync("products");
            SeoFromSection(sections, "hero", Cfg.T("nav.products"));

            var query = Db.Products.AsNoTracking()
                .Include(x => x.Category).Include(x => x.Brand)
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category != null && x.Category.Slug == category);
            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(x => x.Brand != null && x.Brand.Slug == brand);
            if (!string.IsNullOrWhiteSpace(line))
                query = query.Where(x => x.ProductionLine != null && x.ProductionLine.Slug == line);
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(x =>
                    x.NameAr.Contains(term) || x.NameEn.Contains(term) ||
                    (x.ShortDescAr != null && x.ShortDescAr.Contains(term)) ||
                    (x.ShortDescEn != null && x.ShortDescEn.Contains(term)));
            }

            var total = await query.CountAsync();
            page = Math.Max(1, page);

            var items = await query
                .OrderByDescending(x => x.IsFeatured).ThenBy(x => x.SortOrder).ThenByDescending(x => x.CreatedAt)
                .Skip((page - 1) * PageSize).Take(PageSize)
                .ToListAsync();

            var vm = new ProductsVm
            {
                Paged = new PagedVm<Product>
                {
                    Items = items, Page = page, PageSize = PageSize, TotalItems = total, Query = q
                },
                Categories = await Db.ProductCategories.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                Brands = await Db.Brands.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                Lines = await Db.ProductionLines.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                CategorySlug = category, BrandSlug = brand, LineSlug = line
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(string slug)
        {
            if (!Cfg.GetBool("feature.products", true)) return NotFound();
            if (string.IsNullOrWhiteSpace(slug)) return RedirectLocalized("/products");

            var product = await Db.Products.AsNoTracking()
                .Include(x => x.Category).Include(x => x.Brand).Include(x => x.ProductionLine)
                .Include(x => x.Images.OrderBy(i => i.SortOrder))
                .Include(x => x.Specs.OrderBy(s => s.SortOrder))
                .FirstOrDefaultAsync(x => x.Slug == slug && x.IsActive);

            if (product is null) return NotFound();

            await Db.Products.Where(x => x.Id == product.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));

            await LoadSectionsAsync("products");
            Seo(product.MetaTitle ?? Lang.Pick(product.NameAr, product.NameEn),
                product.MetaDesc ?? Lang.Pick(product.ShortDescAr, product.ShortDescEn),
                product.MainImagePath);

            var vm = new ProductDetailsVm
            {
                Product = product,
                Related = await Db.Products.AsNoTracking()
                    .Where(x => x.IsActive && x.Id != product.Id &&
                                (x.CategoryId == product.CategoryId || x.ProductionLineId == product.ProductionLineId))
                    .OrderBy(x => x.SortOrder).Take(4).ToListAsync()
            };

            return View(vm);
        }
    }
}
