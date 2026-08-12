using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.ViewComponents
{
    /// <summary>Renders a DB-driven menu (header or any footer column).</summary>
    public class SiteMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        private readonly ILang _lang;

        public SiteMenuViewComponent(AppDbContext db, ILang lang) { _db = db; _lang = lang; }

        public async Task<IViewComponentResult> InvokeAsync(string menuKey, string view = "Default")
        {
            var menu = await _db.Menus
                .AsNoTracking()
                .Include(m => m.Items.Where(i => i.IsActive))
                .FirstOrDefaultAsync(m => m.MenuKey == menuKey && m.IsActive);

            var vm = new MenuVm { MenuKey = menuKey, Title = _lang.Pick(menu?.TitleAr, menu?.TitleEn) };
            if (menu is null) return View(view, vm);

            var currentPath = HttpContext.Request.Path.Value ?? "/";

            var roots = menu.Items.Where(i => i.ParentId is null).OrderBy(i => i.SortOrder);
            foreach (var item in roots)
            {
                var node = Map(item, currentPath);
                node.Children = menu.Items
                    .Where(c => c.ParentId == item.Id)
                    .OrderBy(c => c.SortOrder)
                    .Select(c => Map(c, currentPath))
                    .ToList();
                vm.Items.Add(node);
            }

            return View(view, vm);
        }

        private MenuItemVm Map(Models.MenuItem i, string currentPath)
        {
            var url = Localize(i.Url);
            return new MenuItemVm
            {
                Label = _lang.Pick(i.LabelAr, i.LabelEn) ?? "",
                Url = url,
                Icon = i.Icon,
                Badge = _lang.Pick(i.BadgeTextAr, i.BadgeTextEn),
                OpenInNewTab = i.OpenInNewTab,
                IsHighlighted = i.IsHighlighted,
                IsActive = IsActive(url, currentPath)
            };
        }

        /// <summary>Internal links get the /{culture} prefix; external links pass through.</summary>
        private string Localize(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return "#";
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith('#'))
                return url;

            return _lang.Url(url);
        }

        private static bool IsActive(string url, string currentPath)
        {
            if (url.StartsWith("http") || url == "#") return false;
            if (url.Equals(currentPath, StringComparison.OrdinalIgnoreCase)) return true;

            // "/ar" home should not light up on every page
            var segments = url.Trim('/').Split('/');
            if (segments.Length <= 1) return false;

            return currentPath.StartsWith(url + "/", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>Footer: about text, contact block, social links and link columns.</summary>
    public class SiteFooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        private readonly ILang _lang;
        private readonly ISettings _cfg;

        public SiteFooterViewComponent(AppDbContext db, ILang lang, ISettings cfg)
        {
            _db = db; _lang = lang; _cfg = cfg;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new FooterVm
            {
                AboutText = _cfg.Get("footer.about"),
                Copyright = _cfg.Get("footer.copyright").Replace("{year}", DateTime.UtcNow.Year.ToString()),
                // The footer sits on a dark surface, so use the light logo — and its
                // Arabic wordmark variant on /ar, since the Latin artwork has the
                // English company name baked in.
                LogoPath = _lang.IsAr
                    ? _cfg.GetRaw("site.logo_light_ar", _cfg.GetRaw("site.logo_light", "/img/brand/logo-light.svg"))
                    : _cfg.GetRaw("site.logo_light", "/img/brand/logo-light.svg"),
                NewsletterTitle = _cfg.Get("footer.newsletter_title"),
                NewsletterText = _cfg.Get("footer.newsletter_text"),
                ShowNewsletter = _cfg.GetBool("feature.newsletter", true),
                PrimaryLocation = await _db.SiteLocations.AsNoTracking()
                    .Where(l => l.IsActive)
                    .OrderByDescending(l => l.IsPrimary).ThenBy(l => l.SortOrder)
                    .FirstOrDefaultAsync(),
                Social = await _db.SocialLinks.AsNoTracking()
                    .Where(s => s.IsActive && s.ShowInFooter)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync()
            };

            return View(vm);
        }
    }
}
