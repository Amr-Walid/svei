using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SVEI.Web.Services
{
    public static class SlugHelper
    {
        /// <summary>
        /// URL-safe slug. Keeps Arabic letters (Egypt-facing site, Arabic URLs are fine
        /// and better for SEO), strips punctuation, collapses whitespace to dashes.
        /// </summary>
        public static string Generate(string? text, string? fallback = null)
        {
            if (string.IsNullOrWhiteSpace(text)) text = fallback;
            if (string.IsNullOrWhiteSpace(text)) return Guid.NewGuid().ToString("N")[..10];

            var s = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                // Drop Latin combining accents but keep Arabic letters intact.
                if (cat == UnicodeCategory.NonSpacingMark && ch < 0x0600) continue;
                sb.Append(ch);
            }

            s = sb.ToString().Normalize(NormalizationForm.FormC);
            s = Regex.Replace(s, @"[^\p{L}\p{Nd}\s-]", "");   // letters, digits, space, dash
            s = Regex.Replace(s, @"[\s_]+", "-");
            s = Regex.Replace(s, @"-{2,}", "-").Trim('-');

            if (s.Length > 150) s = s[..150].Trim('-');
            return string.IsNullOrWhiteSpace(s) ? Guid.NewGuid().ToString("N")[..10] : s;
        }

        /// <summary>Appends -2, -3 … until the slug is unique inside the given table.</summary>
        public static async Task<string> UniqueAsync<T>(
            DbSet<T> set,
            Func<T, string> slugSelector,
            string baseSlug,
            int? ignoreId = null,
            Func<T, int>? idSelector = null) where T : class
        {
            var slug = string.IsNullOrWhiteSpace(baseSlug) ? Generate(null) : baseSlug;
            var existing = await set.AsNoTracking().ToListAsync();

            bool Taken(string candidate) => existing.Any(e =>
                string.Equals(slugSelector(e), candidate, StringComparison.OrdinalIgnoreCase) &&
                (ignoreId is null || idSelector is null || idSelector(e) != ignoreId));

            if (!Taken(slug)) return slug;

            for (var i = 2; i < 1000; i++)
            {
                var candidate = $"{slug}-{i}";
                if (!Taken(candidate)) return candidate;
            }
            return $"{slug}-{Guid.NewGuid():N}"[..150];
        }
    }
}
