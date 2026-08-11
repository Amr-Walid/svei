using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  MENU + MENU ITEM — header / footer navigation, fully admin-managed
    // ═══════════════════════════════════════════════════════════════════════════
    public class Menu
    {
        public int Id { get; set; }

        /// <summary>header | footer-1 | footer-2 | footer-3 | topbar | mobile</summary>
        [Required, MaxLength(60)] public string MenuKey { get; set; } = "";
        [MaxLength(150)] public string? TitleAr { get; set; }
        [MaxLength(150)] public string? TitleEn { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int? ParentId { get; set; }

        [Required, MaxLength(150)] public string LabelAr { get; set; } = "";
        [Required, MaxLength(150)] public string LabelEn { get; set; } = "";
        [MaxLength(400)] public string? Url { get; set; }
        [MaxLength(60)] public string? Icon { get; set; }
        [MaxLength(80)] public string? BadgeTextAr { get; set; }
        [MaxLength(80)] public string? BadgeTextEn { get; set; }

        public bool OpenInNewTab { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsHighlighted { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        public Menu Menu { get; set; } = null!;
        public MenuItem? Parent { get; set; }
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  TRANSLATION — UI strings (buttons, labels) editable from admin
    // ═══════════════════════════════════════════════════════════════════════════
    public class Translation
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string Key { get; set; } = "";
        [MaxLength(60)] public string Group { get; set; } = "common";
        [MaxLength(1000)] public string? ValueAr { get; set; }
        [MaxLength(1000)] public string? ValueEn { get; set; }
        [MaxLength(300)] public string? Note { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  MEDIA ASSET — central media library
    // ═══════════════════════════════════════════════════════════════════════════
    public class MediaAsset
    {
        public int Id { get; set; }

        [Required, MaxLength(400)] public string Path { get; set; } = "";
        [MaxLength(400)] public string? ThumbPath { get; set; }
        [MaxLength(250)] public string? FileName { get; set; }
        [MaxLength(120)] public string? ContentType { get; set; }
        public long SizeBytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        [MaxLength(250)] public string? AltAr { get; set; }
        [MaxLength(250)] public string? AltEn { get; set; }
        [MaxLength(80)] public string? Folder { get; set; }
        [MaxLength(300)] public string? Tags { get; set; }

        [MaxLength(200)] public string? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string SizeLabel => SizeBytes switch
        {
            < 1024 => $"{SizeBytes} B",
            < 1024 * 1024 => $"{SizeBytes / 1024.0:0.#} KB",
            _ => $"{SizeBytes / 1048576.0:0.##} MB"
        };
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  AUDIT LOG
    // ═══════════════════════════════════════════════════════════════════════════
    public class AuditLog
    {
        public int Id { get; set; }

        [MaxLength(200)] public string? UserName { get; set; }
        [MaxLength(60)] public string? Action { get; set; }      // create | update | delete | login
        [MaxLength(120)] public string? EntityName { get; set; }
        [MaxLength(60)] public string? EntityId { get; set; }
        [MaxLength(600)] public string? Summary { get; set; }
        [MaxLength(60)] public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  REDIRECT — old .html URLs → new routes (preserve SEO from svei.tech)
    // ═══════════════════════════════════════════════════════════════════════════
    public class UrlRedirect
    {
        public int Id { get; set; }

        [Required, MaxLength(400)] public string FromPath { get; set; } = "";
        [Required, MaxLength(400)] public string ToPath { get; set; } = "";
        public bool IsPermanent { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public int HitCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
