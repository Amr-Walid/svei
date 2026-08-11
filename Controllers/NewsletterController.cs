using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers
{
    /// <summary>AJAX newsletter subscribe (posted by site.js) + unsubscribe link.</summary>
    public class NewsletterController : PublicControllerBase
    {
        public NewsletterController(AppDbContext db, ILang lang, ISettings cfg) : base(db, lang, cfg) { }

        [HttpPost]
        [Route("/{culture:regex(^ar|en$)}/newsletter/subscribe")]
        [Route("/newsletter/subscribe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string? email, string? name)
        {
            if (!Cfg.GetBool("feature.newsletter", true))
                return Json(new { ok = false, message = Cfg.T("msg.form_error") });

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return Json(new { ok = false, message = Cfg.T("msg.form_error") });

            var normalized = email.Trim().ToLowerInvariant();

            var existing = await Db.NewsletterSubscribers
                .FirstOrDefaultAsync(x => x.Email == normalized);

            if (existing is not null)
            {
                if (existing.IsActive)
                    return Json(new { ok = false, message = Cfg.T("msg.already_subscribed") });

                // Re-activate a previously unsubscribed address
                existing.IsActive = true;
                existing.Language = Lang.Code;
                await Db.SaveChangesAsync();
                return Json(new { ok = true, message = Cfg.T("msg.subscribed") });
            }

            Db.NewsletterSubscribers.Add(new NewsletterSubscriber
            {
                Email            = normalized,
                Name             = name?.Trim(),
                Language         = Lang.Code,
                UnsubscribeToken = Guid.NewGuid().ToString("N")
            });
            await Db.SaveChangesAsync();

            return Json(new { ok = true, message = Cfg.T("msg.subscribed") });
        }

        [Route("/{culture:regex(^ar|en$)}/newsletter/unsubscribe/{token}")]
        public async Task<IActionResult> Unsubscribe(string token)
        {
            var sub = await Db.NewsletterSubscribers
                .FirstOrDefaultAsync(x => x.UnsubscribeToken == token);

            if (sub is not null)
            {
                sub.IsActive = false;
                await Db.SaveChangesAsync();
            }

            TempData["Success"] = Lang.IsAr
                ? "تم إلغاء اشتراكك في النشرة البريدية."
                : "You have been unsubscribed from our newsletter.";

            return RedirectLocalized("/");
        }
    }
}
