using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;

namespace SVEI.Web.Controllers.Admin
{
    /// <summary>
    /// Shared plumbing for every admin controller: auth, audit trail,
    /// sidebar badge counts and toast messages.
    /// </summary>
    [Authorize(Roles = "Admin,Editor")]
    [Route("Admin/[controller]")]
    public abstract class AdminBaseController : Controller
    {
        protected readonly AppDbContext Db;
        protected readonly ILang Lang;

        protected AdminBaseController(AppDbContext db, ILang lang)
        { Db = db; Lang = lang; }

        protected bool IsAr => Lang.IsAr;
        protected string Pick(string ar, string en) => IsAr ? ar : en;

        public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            await LoadBadgesAsync();
            await next();
        }

        /// <summary>Unread counters shown next to the inbox links in the sidebar.</summary>
        protected async Task LoadBadgesAsync()
        {
            var badges = new Dictionary<string, int>
            {
                ["messages"]            = await Db.ContactMessages.CountAsync(x => !x.IsRead && !x.IsArchived),
                ["quotes"]              = await Db.QuoteRequests.CountAsync(x => !x.IsRead),
                ["applications"]        = await Db.JobApplications.CountAsync(x => !x.IsRead),
                ["event-registrations"] = await Db.EventRegistrations.CountAsync(x => !x.IsRead),
                ["partners"]            = await Db.PartnerRequests.CountAsync(x => !x.IsRead),
            };
            ViewData["Badges"] = badges;
            ViewData["InboxTotal"] = badges.Values.Sum();
        }

        // ── toast helpers ─────────────────────────────────────────────────────
        protected void Ok(string ar, string en) => TempData["ok"] = Pick(ar, en);
        protected void Warn(string ar, string en) => TempData["warn"] = Pick(ar, en);
        protected void Err(string ar, string en) => TempData["err"] = Pick(ar, en);

        // ── audit trail ───────────────────────────────────────────────────────
        protected async Task AuditAsync(string action, string entity, object? id, string? summary = null)
        {
            Db.AuditLogs.Add(new AuditLog
            {
                UserName   = User.Identity?.Name,
                Action     = action,
                EntityName = entity,
                EntityId   = id?.ToString(),
                Summary    = summary,
                IpAddress  = HttpContext.Connection.RemoteIpAddress?.ToString(),
                CreatedAt  = DateTime.UtcNow
            });
            await Db.SaveChangesAsync();
        }
    }
}
