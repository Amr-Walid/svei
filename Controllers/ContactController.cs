using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class ContactController : PublicControllerBase
    {
        public ContactController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        public async Task<IActionResult> Index()
        {
            var sections = await LoadSectionsAsync("contact");
            SeoFromSection(sections, "hero", Cfg.T("nav.contact"));

            var vm = new ContactVm
            {
                Locations = await Db.SiteLocations.AsNoTracking()
                    .Where(x => x.IsActive)
                    .OrderByDescending(x => x.IsPrimary).ThenBy(x => x.SortOrder).ToListAsync(),
                Social = await Db.SocialLinks.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                Faqs = await Db.FaqItems.AsNoTracking()
                    .Where(x => x.IsActive && (x.GroupKey == "contact" || x.GroupKey == "general"))
                    .OrderBy(x => x.SortOrder).ToListAsync()
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimiting.Forms)]
        public async Task<IActionResult> Send(ContactForm form)
        {
            if (IsSpam(form.Website)) return RedirectLocalized("/contact");

            if (string.IsNullOrWhiteSpace(form.FullName) ||
                string.IsNullOrWhiteSpace(form.Email) ||
                string.IsNullOrWhiteSpace(form.Message))
            {
                TempData["Error"] = Cfg.T("msg.form_error");
                return RedirectLocalized("/contact");
            }

            Db.ContactMessages.Add(new ContactMessage
            {
                FullName  = form.FullName.Trim(),
                Email     = form.Email.Trim().ToLowerInvariant(),
                Phone     = form.Phone?.Trim(),
                Company   = form.Company?.Trim(),
                Subject   = form.Subject?.Trim(),
                Message   = form.Message.Trim(),
                IpAddress = ClientIp(),
                Language  = Lang.Code
            });
            await Db.SaveChangesAsync();

            return Success("/contact");
        }
    }
}
