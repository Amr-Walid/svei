using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SVEI.Web.Data;
using SVEI.Web.Models;

namespace SVEI.Web.Services
{
    public interface IMediaService
    {
        Task<string?> SaveImageAsync(IFormFile? file, string folder, int maxWidth = 1920, bool makeThumb = true);
        Task<string?> SaveFileAsync(IFormFile? file, string folder);
        void Delete(string? webPath);
    }

    public class MediaService : IMediaService
    {
        private static readonly string[] ImageExts = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp" };

        /// <summary>
        /// Extensions a non-image upload (CV, datasheet, certificate) may use.
        ///
        /// This is deliberately an ALLOWLIST. The previous blocklist could only
        /// reject the extensions someone thought to enumerate, so an anonymous
        /// visitor could attach "cv.html" or "cv.svg" to a job application and
        /// have it stored under wwwroot and served from the site's own origin as
        /// text/html — a stored XSS that runs with the admin's session when the
        /// admin opens the CV from the inbox. SVG is excluded for the same
        /// reason: it is an active document that can carry &lt;script&gt;.
        /// </summary>
        private static readonly string[] AllowedFileExts =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".txt", ".csv", ".rtf", ".odt", ".zip"
        };

        /// <summary>Content types that must never be echoed back by the static file handler.</summary>
        private static readonly string[] AllowedFileContentTypes =
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.oasis.opendocument.text",
            "application/rtf", "application/zip", "application/x-zip-compressed",
            "application/octet-stream",
            "text/plain", "text/csv", "text/rtf"
        };

        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public MediaService(IWebHostEnvironment env, AppDbContext db, IHttpContextAccessor http)
        {
            _env = env; _db = db; _http = http;
        }

        /// <summary>Saves an upload, converting raster images to WebP and generating a thumbnail.</summary>
        public async Task<string?> SaveImageAsync(IFormFile? file, string folder, int maxWidth = 1920, bool makeThumb = true)
        {
            if (file is null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Anything that is not a raster image we recognise is handed to the
            // document path, which applies its own allowlist. SVG deliberately
            // goes there too (and is rejected): it is a scriptable document, not
            // a safe image, and it used to be passed through untouched.
            if (!ImageExts.Contains(ext)) return await SaveFileAsync(file, folder);

            folder = SafeFolder(folder);
            var dir = EnsureDir(folder);
            var name = $"{Guid.NewGuid():N}.webp";
            var full = Path.Combine(dir, name);

            await using (var stream = file.OpenReadStream())
            using (var img = await Image.LoadAsync(stream))
            {
                var width = img.Width; var height = img.Height;

                if (img.Width > maxWidth)
                    img.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth, 0)
                    }));

                await img.SaveAsync(full, new WebpEncoder { Quality = 82 });

                string? thumbPath = null;
                if (makeThumb)
                {
                    var thumbName = $"thumb_{name}";
                    using var thumb = img.Clone(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(480, 0)
                    }));
                    await thumb.SaveAsync(Path.Combine(dir, thumbName), new WebpEncoder { Quality = 78 });
                    thumbPath = $"/uploads/{folder}/{thumbName}";
                }

                var webPath = $"/uploads/{folder}/{name}";
                await RecordAsync(webPath, thumbPath, file, folder, width, height);
                return webPath;
            }
        }

        /// <summary>Saves a non-image file (CV, datasheet, certificate) verbatim.</summary>
        public async Task<string?> SaveFileAsync(IFormFile? file, string folder)
        {
            if (file is null || file.Length == 0) return null;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedFileExts.Contains(ext)) return null;

            // Cross-check the declared content type. The extension is what the
            // static file handler uses to pick a Content-Type, so the two must
            // agree before we persist anything under wwwroot.
            var ctype = (file.ContentType ?? "").Split(';')[0].Trim().ToLowerInvariant();
            if (ctype.Length > 0 && !AllowedFileContentTypes.Contains(ctype)) return null;

            folder = SafeFolder(folder);
            var dir = EnsureDir(folder);
            var name = $"{Guid.NewGuid():N}{ext}";
            var webPath = $"/uploads/{folder}/{name}";

            await using (var fs = new FileStream(Path.Combine(dir, name), FileMode.Create))
                await file.CopyToAsync(fs);

            await RecordAsync(webPath, null, file, folder, null, null);
            return webPath;
        }

        public void Delete(string? webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath)) return;
            if (!webPath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return;

            TryDelete(webPath);

            // Companion thumbnail, if any.
            var dir = Path.GetDirectoryName(webPath)!.Replace('\\', '/');
            var file = Path.GetFileName(webPath);
            TryDelete($"{dir}/thumb_{file}");

            var row = _db.MediaAssets.FirstOrDefault(m => m.Path == webPath);
            if (row is not null) { _db.MediaAssets.Remove(row); _db.SaveChanges(); }
        }

        // ── helpers ───────────────────────────────────────────────────────────
        /// <summary>
        /// The folder comes from a form field on the admin media screen, so it is
        /// caller-controlled. Reduce it to a single safe segment and then verify
        /// the resolved path really is inside wwwroot/uploads before creating it,
        /// so no combination of "..", absolute paths or separators can escape.
        /// </summary>
        private string EnsureDir(string folder)
        {
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploads = Path.GetFullPath(Path.Combine(root, "uploads"));

            var safe = new string((folder ?? "").Trim()
                .Where(c => char.IsLetterOrDigit(c) || c is '-' or '_')
                .ToArray());
            if (safe.Length == 0) safe = "library";
            if (safe.Length > 40) safe = safe[..40];

            var dir = Path.GetFullPath(Path.Combine(uploads, safe));
            if (!dir.StartsWith(uploads + Path.DirectorySeparatorChar, StringComparison.Ordinal) &&
                !string.Equals(dir, uploads, StringComparison.Ordinal))
                dir = uploads;

            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>Folder name as it will appear in the public URL (matches EnsureDir).</summary>
        private static string SafeFolder(string folder)
        {
            var safe = new string((folder ?? "").Trim()
                .Where(c => char.IsLetterOrDigit(c) || c is '-' or '_')
                .ToArray());
            if (safe.Length == 0) safe = "library";
            return safe.Length > 40 ? safe[..40] : safe;
        }

        private void TryDelete(string webPath)
        {
            try
            {
                var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var abs = Path.Combine(root, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(abs)) File.Delete(abs);
            }
            catch { /* best effort */ }
        }

        private async Task RecordAsync(string path, string? thumb, IFormFile file, string folder, int? w, int? h)
        {
            _db.MediaAssets.Add(new MediaAsset
            {
                Path = path,
                ThumbPath = thumb,
                FileName = Path.GetFileName(file.FileName),
                ContentType = file.ContentType,
                SizeBytes = file.Length,
                Width = w,
                Height = h,
                Folder = folder,
                UploadedBy = _http.HttpContext?.User?.Identity?.Name
            });
            await _db.SaveChangesAsync();
        }
    }
}
