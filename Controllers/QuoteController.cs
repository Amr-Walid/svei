using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels;

namespace SVEI.Web.Controllers
{
    public class QuoteController : PublicControllerBase
    {
        private readonly IMediaService _media;

        public QuoteController(AppDbContext db, ILang lang, ISettings cfg, IMediaService media)
            : base(db, lang, cfg) { _media = media; }

        public async Task<IActionResult> Index(int? line)
        {
            if (!Cfg.GetBool("feature.quote", true)) return NotFound();

            var sections = await LoadSectionsAsync("quote");
            SeoFromSection(sections, "hero", Cfg.T("nav.quote"));

            var vm = new QuoteVm
            {
                Lines = await Db.ProductionLines.AsNoTracking()
                    .Where(x => x.IsActive).OrderBy(x => x.SortOrder).ToListAsync(),
                Steps = await Db.InfoCards.AsNoTracking()
                    .Where(x => x.IsActive && x.GroupKey == "quote.steps")
                    .OrderBy(x => x.SortOrder).ToListAsync(),
                Form = new QuoteForm { ProductionLineId = line }
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [EnableRateLimiting(RateLimiting.Forms)]
        [RequestSizeLimit(30 * 1024 * 1024)]
        public async Task<IActionResult> Send(QuoteForm form)
        {
            if (!Cfg.GetBool("feature.quote", true)) return NotFound();
            if (IsSpam(form.Website)) return RedirectLocalized("/quote");

            if (string.IsNullOrWhiteSpace(form.FullName) ||
                string.IsNullOrWhiteSpace(form.Email) ||
                string.IsNullOrWhiteSpace(form.Phone))
            {
                TempData["Error"] = Cfg.T("msg.form_error");
                return RedirectLocalized("/quote");
            }

            var attachment = await _media.SaveFileAsync(form.Attachment, "rfq");

            Db.QuoteRequests.Add(new QuoteRequest
            {
                FullName         = form.FullName.Trim(),
                Email            = form.Email.Trim().ToLowerInvariant(),
                Phone            = form.Phone.Trim(),
                Company          = form.Company?.Trim(),
                Country          = form.Country?.Trim(),
                ProductionLineId = form.ProductionLineId,
                ProductType      = form.ProductType?.Trim(),
                Quantity         = form.Quantity,
                TargetBudget     = form.TargetBudget?.Trim(),
                Timeline         = form.Timeline?.Trim(),
                Details          = form.Details?.Trim(),
                AttachmentPath   = attachment,
                IpAddress        = ClientIp(),
                Language         = Lang.Code
            });
            await Db.SaveChangesAsync();

            return Success("/quote");
        }
    }
}
