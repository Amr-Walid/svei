using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels.Admin;

namespace SVEI.Web.Controllers.Admin
{
    [Route("Admin/Media")]
    public class MediaController : AdminBaseController
    {
        private readonly IMediaService _media;

        public MediaController(AppDbContext db, ILang lang, IMediaService media) : base(db, lang)
            => _media = media;

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] string? q, [FromQuery] string? folder, [FromQuery] int page = 1)
        {
            const int size = 48;
            var query = Db.MediaAssets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(m =>
                    (m.FileName != null && m.FileName.Contains(term)) ||
                    (m.AltAr != null && m.AltAr.Contains(term)) ||
                    (m.AltEn != null && m.AltEn.Contains(term)) ||
                    (m.Tags != null && m.Tags.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(folder))
                query = query.Where(m => m.Folder == folder);

            var total = await query.CountAsync();
            page = Math.Max(1, page);

            var vm = new MediaVm
            {
                Query = q,
                Folder = folder,
                Page = page,
                PageSize = size,
                Total = total,
                Items = await query.OrderByDescending(m => m.CreatedAt)
                                   .Skip((page - 1) * size).Take(size).ToListAsync(),
                Folders = await Db.MediaAssets.Where(m => m.Folder != null)
                                  .Select(m => m.Folder!).Distinct().OrderBy(f => f).ToListAsync(),
                TotalBytes = await Db.MediaAssets.SumAsync(m => m.SizeBytes)
            };
            return View("~/Views/Admin/Media/Index.cshtml", vm);
        }

        [HttpPost("upload")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(80 * 1024 * 1024)]
        public async Task<IActionResult> Upload(List<IFormFile> files, string? folder)
        {
            var target = string.IsNullOrWhiteSpace(folder) ? "library" : folder.Trim();
            var n = 0;

            foreach (var f in files.Where(f => f.Length > 0))
            {
                var saved = f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                    ? await _media.SaveImageAsync(f, target)
                    : await _media.SaveFileAsync(f, target);
                if (saved is not null) n++;
            }

            if (n > 0)
            {
                await AuditAsync("upload", "MediaAsset", null, $"{n} file(s) -> {target}");
                Ok($"تم رفع {n} ملف ✓", $"Uploaded {n} file(s) ✓");
            }
            else Warn("لم يتم رفع أي ملف.", "No files were uploaded.");

            return RedirectToAction(nameof(Index), new { folder });
        }

        [HttpPost("delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromForm] string? folder)
        {
            var asset = await Db.MediaAssets.FindAsync(id);
            if (asset is null) return NotFound();

            _media.Delete(asset.Path);
            Db.MediaAssets.Remove(asset);
            await Db.SaveChangesAsync();

            await AuditAsync("delete", "MediaAsset", id, asset.FileName);
            Ok("تم الحذف ✓", "Deleted ✓");
            return RedirectToAction(nameof(Index), new { folder });
        }

        /// <summary>Saves the Alt text edits for a media asset.</summary>
        [HttpPost("alt/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Alt([FromRoute] int id, [FromForm] string? altAr, [FromForm] string? altEn, [FromForm] string? tags)
        {
            var asset = await Db.MediaAssets.FindAsync(id);
            if (asset is null) return NotFound();

            asset.AltAr = altAr;
            asset.AltEn = altEn;
            asset.Tags = tags;
            await Db.SaveChangesAsync();

            Ok("تم الحفظ ✓", "Saved ✓");
            return RedirectToAction(nameof(Index), new { folder = asset.Folder });
        }
    }
}
