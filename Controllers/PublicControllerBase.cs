using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers
{
    /// <summary>
    /// Shared plumbing for every public-facing controller:
    /// loads the admin-managed PageSections for the page and exposes them to the view,
    /// plus small helpers for SEO metadata and localized redirects.
    /// </summary>
    public abstract class PublicControllerBase : Controller
    {
        protected readonly AppDbContext Db;
        protected readonly ILang Lang;
        protected readonly ISettings Cfg;

        protected PublicControllerBase(AppDbContext db, ILang lang, ISettings cfg)
        {
            Db = db; Lang = lang; Cfg = cfg;
        }

        /// <summary>Loads every visible section of a page into ViewData["Sections"].</summary>
        protected async Task<Dictionary<string, PageSection>> LoadSectionsAsync(string pageKey)
        {
            var sections = await Db.PageSections.AsNoTracking()
                .Where(s => s.PageKey == pageKey && s.IsVisible)
                .OrderBy(s => s.SortOrder)
                .ToDictionaryAsync(s => s.SectionKey, StringComparer.OrdinalIgnoreCase);

            ViewData["Sections"] = sections;
            ViewData["PageKey"] = pageKey;
            return sections;
        }

        /// <summary>Sets the &lt;title&gt; / meta description / OG image for the current view.</summary>
        protected void Seo(string? title, string? desc = null, string? image = null)
        {
            if (!string.IsNullOrWhiteSpace(title)) ViewData["Title"] = title;
            if (!string.IsNullOrWhiteSpace(desc)) ViewData["MetaDescription"] = Trim(desc, 300);
            if (!string.IsNullOrWhiteSpace(image)) ViewData["OgImage"] = image;
        }

        /// <summary>Uses the page's "hero" section (if any) to fill the SEO tags.</summary>
        protected void SeoFromSection(IReadOnlyDictionary<string, PageSection> sections,
                                      string sectionKey = "hero",
                                      string? fallbackTitle = null)
        {
            sections.TryGetValue(sectionKey, out var s);
            Seo(Lang.Pick(s?.TitleAr, s?.TitleEn) ?? fallbackTitle,
                Lang.Pick(s?.SubtitleAr, s?.SubtitleEn),
                s?.ImagePath);
        }

        protected static string? Trim(string? text, int max)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var plain = System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", " ");
            plain = System.Text.RegularExpressions.Regex.Replace(plain, @"\s+", " ").Trim();
            return plain.Length <= max ? plain : plain[..max].TrimEnd() + "…";
        }

        /// <summary>Redirect to a site path keeping the current culture prefix.</summary>
        protected IActionResult RedirectLocalized(string path) => Redirect(Lang.Url(path));

        /// <summary>Simple, cheap spam guard for public forms (honeypot field).</summary>
        protected static bool IsSpam(string? honeypot) => !string.IsNullOrWhiteSpace(honeypot);

        protected string? ClientIp() => HttpContext.Connection.RemoteIpAddress?.ToString();

        /// <summary>Adds the success flash message and redirects back to the page.</summary>
        protected IActionResult Success(string path, string? messageKey = "msg.form_success")
        {
            TempData["Success"] = Cfg.T(messageKey ?? "msg.form_success");
            return RedirectLocalized(path);
        }
    }
}
