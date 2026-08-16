using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace SVEI.Web.TagHelpers
{
    /// <summary>
    /// Turns a plain <c>&lt;img src="/img/..."&gt;</c> into a responsive image.
    ///
    /// The site's art is already WebP at sensible dimensions, but a card that is
    /// painted 270 CSS px wide was still downloading the full 1200 px master —
    /// about five times the pixels the screen can resolve. tools/optimize-images.py
    /// pre-renders a width ladder at the *same* encoder quality and records it in
    /// wwwroot/img/variants.json; this helper reads that manifest and emits the
    /// matching <c>srcset</c>, so the browser fetches the smallest file that still
    /// covers its device-pixel ratio. Nothing is re-encoded per request.
    ///
    /// It also fills in intrinsic <c>width</c>/<c>height</c> when the view did not
    /// supply them. That is what stops the layout jumping while images stream in
    /// (Cumulative Layout Shift), which is a large part of why the page *felt*
    /// slow even once bytes were reasonable.
    ///
    /// Opt out per image with <c>sv-responsive="false"</c> (used for logos and
    /// anything that must stay pixel-exact).
    /// </summary>
    [HtmlTargetElement("img", Attributes = "src")]
    public class ResponsiveImageTagHelper : TagHelper
    {
        private const string CacheKey = "svei:img:variants";
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        public ResponsiveImageTagHelper(IWebHostEnvironment env, IMemoryCache cache)
        {
            _env = env;
            _cache = cache;
        }

        /// <summary>Set false to leave the tag completely untouched.</summary>
        [HtmlAttributeName("sv-responsive")]
        public bool Responsive { get; set; } = true;

        /// <summary>
        /// The CSS width this image occupies, used for the <c>sizes</c> attribute.
        /// Without it the browser assumes 100vw and over-fetches on desktop.
        /// </summary>
        [HtmlAttributeName("sv-sizes")]
        public string? Sizes { get; set; }

        /// <summary>Marks the LCP image: eager + high priority, never lazy.</summary>
        [HtmlAttributeName("sv-priority")]
        public bool Priority { get; set; }

        /// <summary>
        /// Emit the real URL into <c>data-src</c>/<c>data-srcset</c> instead of
        /// <c>src</c>/<c>srcset</c>, leaving a transparent placeholder behind.
        ///
        /// Needed for carousel slides: they are all stacked inside the viewport, so
        /// the browser treats them as visible and <c>loading="lazy"</c> is ignored —
        /// every slide downloads at once and competes with the LCP image. Script
        /// swaps the attributes in after load (see heroSlider in site.js).
        /// </summary>
        [HtmlAttributeName("sv-defer")]
        public bool Defer { get; set; }

        private const string Placeholder =
            "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg'%3E%3C/svg%3E";

        // Ordered widest-last so the manifest ladder can be emitted directly.
        private sealed record Variant(int W, int H, string Url);
        private sealed record Entry(int W, int H, List<Variant> Variants);

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("sv-responsive");
            output.Attributes.RemoveAll("sv-sizes");
            output.Attributes.RemoveAll("sv-priority");
            output.Attributes.RemoveAll("sv-defer");

            var src = output.Attributes["src"]?.Value?.ToString();
            if (string.IsNullOrWhiteSpace(src)) return;

            // Decoding async costs nothing and keeps image decode off the main
            // thread, so a heavy hero cannot block first paint.
            if (output.Attributes["decoding"] == null)
                output.Attributes.SetAttribute("decoding", "async");

            if (Priority)
            {
                // The hero is the Largest Contentful Paint element. Lazy-loading
                // it (or leaving it at default priority) delays the one image the
                // score is actually measured against.
                output.Attributes.RemoveAll("loading");
                output.Attributes.SetAttribute("fetchpriority", "high");
            }
            else if (output.Attributes["loading"] == null)
            {
                output.Attributes.SetAttribute("loading", "lazy");
            }

            // A deferred image must never keep its real src, even when it has no
            // manifest entry (uploaded art, non-WebP) — otherwise the whole point
            // of deferring is lost.
            void ParkSrcOnly()
            {
                output.Attributes.SetAttribute("data-src", src);
                output.Attributes.SetAttribute("src", Placeholder);
            }

            if (!Responsive)
            {
                if (Defer) ParkSrcOnly();
                return;
            }

            // Only local, non-query, non-data URLs can map to the manifest.
            if (src.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
                src.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                src.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                src.StartsWith("//", StringComparison.Ordinal))
            {
                if (Defer && !src.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) ParkSrcOnly();
                return;
            }

            var clean = src.Split('?')[0].Split('#')[0];
            if (!clean.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                if (Defer) ParkSrcOnly();
                return;
            }

            var map = LoadManifest();
            if (!map.TryGetValue(clean, out var entry))
            {
                if (Defer) ParkSrcOnly();
                return;
            }

            // Intrinsic dimensions prevent layout shift. Only set when the view
            // has not already been explicit.
            if (output.Attributes["width"] == null && output.Attributes["height"] == null &&
                entry.W > 0 && entry.H > 0)
            {
                output.Attributes.SetAttribute("width", entry.W);
                output.Attributes.SetAttribute("height", entry.H);
            }

            if (entry.Variants.Count < 2 || output.Attributes["srcset"] != null)
            {
                // Nothing to choose between (or the view was explicit), but a
                // deferred image still must not fetch during first paint.
                if (Defer) ParkSrcOnly();
                return;
            }

            var srcset = string.Join(", ", entry.Variants.Select(v => $"{v.Url} {v.W}w"));

            if (Defer)
            {
                // Park both the URL and the ladder; the placeholder keeps the box
                // laid out (width/height are already set) with no network cost.
                output.Attributes.SetAttribute("data-src", src);
                output.Attributes.SetAttribute("data-srcset", srcset);
                output.Attributes.SetAttribute("data-sizes", Sizes ?? "100vw");
                output.Attributes.SetAttribute("src", Placeholder);
                return;
            }

            output.Attributes.SetAttribute("srcset", srcset);
            output.Attributes.SetAttribute("sizes", Sizes ?? "100vw");
        }

        private Dictionary<string, Entry> LoadManifest()
        {
            return _cache.GetOrCreate(CacheKey, e =>
            {
                // Long slide: the manifest only changes when the build regenerates
                // variants, and a stale entry would just mean a missing srcset.
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                var path = Path.Combine(_env.WebRootPath ?? "", "img", "variants.json");
                var result = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
                if (!File.Exists(path)) return result;

                try
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(path));
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        var w = prop.Value.TryGetProperty("w", out var wv) ? wv.GetInt32() : 0;
                        var h = prop.Value.TryGetProperty("h", out var hv) ? hv.GetInt32() : 0;
                        var list = new List<Variant>();
                        if (prop.Value.TryGetProperty("variants", out var arr))
                        {
                            foreach (var v in arr.EnumerateArray())
                            {
                                var vu = v.GetProperty("url").GetString();
                                if (string.IsNullOrEmpty(vu)) continue;
                                list.Add(new Variant(v.GetProperty("w").GetInt32(),
                                                     v.GetProperty("h").GetInt32(), vu));
                            }
                        }
                        result[prop.Name] = new Entry(w, h, list.OrderBy(v => v.W).ToList());
                    }
                }
                catch
                {
                    // A malformed manifest must never take the site down; the worst
                    // case is that images fall back to the single full-size file.
                    return new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
                }
                return result;
            }) ?? new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
