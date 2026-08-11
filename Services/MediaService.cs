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
        private static readonly string[] BlockedExts = { ".exe", ".dll", ".bat", ".cmd", ".sh", ".ps1", ".js", ".php", ".aspx", ".cshtml" };

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
            if (BlockedExts.Contains(ext)) return null;

            // SVG passes through untouched (ImageSharp can't rasterize it).
            if (ext == ".svg") return await SaveFileAsync(file, folder);
            if (!ImageExts.Contains(ext)) return await SaveFileAsync(file, folder);

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
            if (BlockedExts.Contains(ext)) return null;

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
        private string EnsureDir(string folder)
        {
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(root, "uploads", folder);
            Directory.CreateDirectory(dir);
            return dir;
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
