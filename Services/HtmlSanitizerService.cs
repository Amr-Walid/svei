using Ganss.Xss;

namespace SVEI.Web.Services
{
    /// <summary>
    /// Cleans admin-entered rich text before it is stored.
    ///
    /// Every FieldKind.Html value ends up in a view as @Html.Raw(...), which by
    /// definition bypasses Razor's encoder — that is the whole point of a WYSIWYG
    /// field. So the trust boundary has to be the *write* path instead: whatever
    /// reaches the database must already be safe to emit verbatim.
    ///
    /// Without this, an Editor (the lower-privileged of the two roles) could store
    /// &lt;script&gt; in a product description and run script in an Admin's session
    /// on the public site — privilege escalation from Editor to Admin.
    /// </summary>
    public interface IHtmlSanitizer
    {
        /// <summary>Returns the value with scripts, event handlers and hostile URLs removed.</summary>
        string? Clean(string? html);
    }

    public class HtmlSanitizerService : IHtmlSanitizer
    {
        // HtmlSanitizer is thread-safe once configured and is relatively expensive
        // to construct, so it is built once and shared.
        private static readonly HtmlSanitizer Sanitizer = Build();

        private static HtmlSanitizer Build()
        {
            var s = new HtmlSanitizer();

            // Start from a closed set. The Quill toolbar in the admin can only
            // produce these, so anything else is either hand-crafted or hostile.
            s.AllowedTags.Clear();
            foreach (var t in new[]
            {
                "p", "br", "hr", "span", "div",
                "h1", "h2", "h3", "h4", "h5", "h6",
                "strong", "b", "em", "i", "u", "s", "sub", "sup",
                "ul", "ol", "li", "blockquote", "pre", "code",
                "a", "img", "figure", "figcaption",
                "table", "thead", "tbody", "tfoot", "tr", "th", "td"
            }) s.AllowedTags.Add(t);

            s.AllowedAttributes.Clear();
            foreach (var a in new[]
            {
                "href", "title", "target", "rel",
                "src", "alt", "width", "height", "loading",
                "class", "dir", "colspan", "rowspan"
            }) s.AllowedAttributes.Add(a);

            // "style" is deliberately NOT allowed: it enables clickjacking-style
            // overlays and position:fixed cover-ups even with no script at all.
            s.AllowedCssProperties.Clear();

            // Only real navigable schemes. This is what stops javascript: and
            // data:text/html hrefs, the classic way to get script through a
            // sanitizer that only filters tags.
            s.AllowedSchemes.Clear();
            s.AllowedSchemes.Add("http");
            s.AllowedSchemes.Add("https");
            s.AllowedSchemes.Add("mailto");
            s.AllowedSchemes.Add("tel");

            s.KeepChildNodes = true;

            // Any link that leaves the site opens in a new tab, so force the
            // rel that prevents the opened page from touching window.opener.
            s.PostProcessNode += (_, e) =>
            {
                if (e.Node is AngleSharp.Html.Dom.IHtmlAnchorElement a &&
                    a.GetAttribute("target") == "_blank")
                    a.SetAttribute("rel", "noopener noreferrer");
            };

            return s;
        }

        public string? Clean(string? html)
            => string.IsNullOrWhiteSpace(html) ? html : Sanitizer.Sanitize(html);
    }
}
