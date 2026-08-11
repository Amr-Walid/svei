using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SVEI.Web.Data;
using SVEI.Web.Models;

namespace SVEI.Web.Services
{
    /// <summary>
    /// Cached access to SiteSettings + Translations so views can read any string
    /// without hitting the DB on every render.
    /// </summary>
    public interface ISettings
    {
        string Get(string key, string? fallback = null);
        string GetRaw(string key, string? fallback = null);
        bool GetBool(string key, bool fallback = false);
        int GetInt(string key, int fallback = 0);
        string T(string key, string? fallback = null);
        void Invalidate();
        Task<Dictionary<string, SiteSetting>> AllAsync();
    }

    public class SettingsService : ISettings
    {
        private const string SettingsCacheKey = "svei:settings";
        private const string TransCacheKey = "svei:translations";

        private readonly AppDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILang _lang;

        public SettingsService(AppDbContext db, IMemoryCache cache, ILang lang)
        {
            _db = db; _cache = cache; _lang = lang;
        }

        private Dictionary<string, SiteSetting> Settings =>
            _cache.GetOrCreate(SettingsCacheKey, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _db.SiteSettings.AsNoTracking()
                    .ToDictionary(s => s.Key, StringComparer.OrdinalIgnoreCase);
            })!;

        private Dictionary<string, Translation> Translations =>
            _cache.GetOrCreate(TransCacheKey, e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return _db.Translations.AsNoTracking()
                    .ToDictionary(t => t.Key, StringComparer.OrdinalIgnoreCase);
            })!;

        /// <summary>Localized setting value.</summary>
        public string Get(string key, string? fallback = null)
        {
            if (Settings.TryGetValue(key, out var s))
            {
                var v = s.IsLocalized ? _lang.Pick(s.ValueAr, s.ValueEn) : (s.ValueAr ?? s.ValueEn);
                if (!string.IsNullOrWhiteSpace(v)) return v!;
            }
            return fallback ?? "";
        }

        /// <summary>Non-localized value (always ValueAr slot) — urls, colors, numbers.</summary>
        public string GetRaw(string key, string? fallback = null)
        {
            if (Settings.TryGetValue(key, out var s))
            {
                var v = s.ValueAr ?? s.ValueEn;
                if (!string.IsNullOrWhiteSpace(v)) return v!;
            }
            return fallback ?? "";
        }

        public bool GetBool(string key, bool fallback = false)
        {
            var v = GetRaw(key);
            if (string.IsNullOrWhiteSpace(v)) return fallback;
            return v is "1" or "true" or "True" or "yes" or "on";
        }

        public int GetInt(string key, int fallback = 0)
            => int.TryParse(GetRaw(key), out var n) ? n : fallback;

        /// <summary>UI string from the Translations table.</summary>
        public string T(string key, string? fallback = null)
        {
            if (Translations.TryGetValue(key, out var t))
            {
                var v = _lang.Pick(t.ValueAr, t.ValueEn);
                if (!string.IsNullOrWhiteSpace(v)) return v!;
            }
            return fallback ?? key;
        }

        public void Invalidate()
        {
            _cache.Remove(SettingsCacheKey);
            _cache.Remove(TransCacheKey);
        }

        public Task<Dictionary<string, SiteSetting>> AllAsync()
            => _db.SiteSettings.AsNoTracking()
                 .OrderBy(s => s.Group).ThenBy(s => s.SortOrder)
                 .ToDictionaryAsync(s => s.Key, StringComparer.OrdinalIgnoreCase);
    }
}
