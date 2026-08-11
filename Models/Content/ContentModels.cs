using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  SITE SETTING — key/value store. Every global string lives here.
    //  Group examples: general | contact | social | seo | home | footer | theme
    // ═══════════════════════════════════════════════════════════════════════════
    public class SiteSetting
    {
        public int Id { get; set; }

        [Required, MaxLength(120)] public string Key { get; set; } = "";
        [MaxLength(60)] public string Group { get; set; } = "general";

        /// <summary>Arabic value (or the single value for non-localized keys).</summary>
        public string? ValueAr { get; set; }
        /// <summary>English value. Falls back to ValueAr when empty.</summary>
        public string? ValueEn { get; set; }

        /// <summary>text | textarea | html | image | number | bool | color | url | email</summary>
        [MaxLength(20)] public string InputType { get; set; } = "text";

        [MaxLength(200)] public string? LabelAr { get; set; }
        [MaxLength(200)] public string? LabelEn { get; set; }
        [MaxLength(400)] public string? HintAr { get; set; }

        /// <summary>Localized keys show two inputs in admin; non-localized show one.</summary>
        public bool IsLocalized { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PAGE SECTION — every heading / sub-heading / paragraph / CTA on any page.
    //  Identified by (PageKey, SectionKey). Admin edits these directly.
    // ═══════════════════════════════════════════════════════════════════════════
    public class PageSection
    {
        public int Id { get; set; }

        /// <summary>home | about | services | products | contact | careers | news | events | gallery | quote</summary>
        [Required, MaxLength(60)] public string PageKey { get; set; } = "";
        [Required, MaxLength(80)] public string SectionKey { get; set; } = "";

        [MaxLength(150)] public string? EyebrowAr { get; set; }
        [MaxLength(150)] public string? EyebrowEn { get; set; }

        [MaxLength(300)] public string? TitleAr { get; set; }
        [MaxLength(300)] public string? TitleEn { get; set; }

        [MaxLength(400)] public string? SubtitleAr { get; set; }
        [MaxLength(400)] public string? SubtitleEn { get; set; }

        public string? BodyAr { get; set; }
        public string? BodyEn { get; set; }

        [MaxLength(120)] public string? ButtonTextAr { get; set; }
        [MaxLength(120)] public string? ButtonTextEn { get; set; }
        [MaxLength(300)] public string? ButtonUrl { get; set; }

        [MaxLength(120)] public string? Button2TextAr { get; set; }
        [MaxLength(120)] public string? Button2TextEn { get; set; }
        [MaxLength(300)] public string? Button2Url { get; set; }

        public string? ImagePath { get; set; }
        public string? Image2Path { get; set; }
        [MaxLength(300)] public string? VideoUrl { get; set; }

        [MaxLength(60)] public string? Icon { get; set; }
        /// <summary>Free-form extra data (JSON) for one-off section needs.</summary>
        public string? ExtraJson { get; set; }

        public bool IsVisible { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped] public string Ref => $"{PageKey}.{SectionKey}";
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  INFO CARD — reusable card list bound to a section (features, values, why-us)
    // ═══════════════════════════════════════════════════════════════════════════
    public class InfoCard
    {
        public int Id { get; set; }

        /// <summary>Groups cards, e.g. "home.features", "about.values", "services.list"</summary>
        [Required, MaxLength(80)] public string GroupKey { get; set; } = "";

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";

        [MaxLength(900)] public string? TextAr { get; set; }
        [MaxLength(900)] public string? TextEn { get; set; }

        [MaxLength(60)] public string? Icon { get; set; }
        public string? ImagePath { get; set; }
        [MaxLength(300)] public string? LinkUrl { get; set; }
        [MaxLength(120)] public string? LinkTextAr { get; set; }
        [MaxLength(120)] public string? LinkTextEn { get; set; }

        [MaxLength(20)] public string? AccentColor { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  STAT COUNTER — the animated numbers strip
    // ═══════════════════════════════════════════════════════════════════════════
    public class StatCounter
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string LabelAr { get; set; } = "";
        [Required, MaxLength(150)] public string LabelEn { get; set; } = "";

        public double Value { get; set; }
        [MaxLength(20)] public string? Prefix { get; set; }
        [MaxLength(20)] public string? Suffix { get; set; }
        public int Decimals { get; set; } = 0;

        [MaxLength(60)] public string? Icon { get; set; }
        [MaxLength(80)] public string GroupKey { get; set; } = "home";

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  HERO SLIDE
    // ═══════════════════════════════════════════════════════════════════════════
    public class HeroSlide
    {
        public int Id { get; set; }

        [MaxLength(150)] public string? EyebrowAr { get; set; }
        [MaxLength(150)] public string? EyebrowEn { get; set; }
        [Required, MaxLength(300)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(300)] public string TitleEn { get; set; } = "";
        [MaxLength(600)] public string? SubtitleAr { get; set; }
        [MaxLength(600)] public string? SubtitleEn { get; set; }

        public string? ImagePath { get; set; }
        [MaxLength(300)] public string? VideoUrl { get; set; }

        [MaxLength(120)] public string? ButtonTextAr { get; set; }
        [MaxLength(120)] public string? ButtonTextEn { get; set; }
        [MaxLength(300)] public string? ButtonUrl { get; set; }
        [MaxLength(120)] public string? Button2TextAr { get; set; }
        [MaxLength(120)] public string? Button2TextEn { get; set; }
        [MaxLength(300)] public string? Button2Url { get; set; }

        /// <summary>left | center | right</summary>
        [MaxLength(20)] public string Align { get; set; } = "center";
        public int OverlayOpacity { get; set; } = 55;

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  FAQ
    // ═══════════════════════════════════════════════════════════════════════════
    public class FaqItem
    {
        public int Id { get; set; }

        [Required, MaxLength(400)] public string QuestionAr { get; set; } = "";
        [Required, MaxLength(400)] public string QuestionEn { get; set; } = "";
        public string? AnswerAr { get; set; }
        public string? AnswerEn { get; set; }

        [MaxLength(80)] public string GroupKey { get; set; } = "general";
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  TESTIMONIAL
    // ═══════════════════════════════════════════════════════════════════════════
    public class Testimonial
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string NameAr { get; set; } = "";
        [Required, MaxLength(150)] public string NameEn { get; set; } = "";
        [MaxLength(200)] public string? RoleAr { get; set; }
        [MaxLength(200)] public string? RoleEn { get; set; }
        [MaxLength(1200)] public string? QuoteAr { get; set; }
        [MaxLength(1200)] public string? QuoteEn { get; set; }

        public string? AvatarPath { get; set; }
        public int Rating { get; set; } = 5;

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  STATIC / LEGAL PAGE — Privacy, Terms, custom pages
    // ═══════════════════════════════════════════════════════════════════════════
    public class StaticPage
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string Slug { get; set; } = "";
        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        public string? BodyAr { get; set; }
        public string? BodyEn { get; set; }

        public string? HeroImagePath { get; set; }
        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public bool IsPublished { get; set; } = true;
        public bool ShowInFooter { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
