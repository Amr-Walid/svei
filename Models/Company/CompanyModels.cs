using System.ComponentModel.DataAnnotations;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  TIMELINE / MILESTONE — company history
    // ═══════════════════════════════════════════════════════════════════════════
    public class Milestone
    {
        public int Id { get; set; }

        [MaxLength(20)] public string? Year { get; set; }
        [MaxLength(60)] public string? MonthAr { get; set; }
        [MaxLength(60)] public string? MonthEn { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [MaxLength(900)] public string? TextAr { get; set; }
        [MaxLength(900)] public string? TextEn { get; set; }

        public string? ImagePath { get; set; }
        [MaxLength(60)] public string? Icon { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  TEAM MEMBER / LEADERSHIP
    // ═══════════════════════════════════════════════════════════════════════════
    public class TeamMember
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [MaxLength(200)] public string? RoleAr { get; set; }
        [MaxLength(200)] public string? RoleEn { get; set; }
        [MaxLength(1500)] public string? BioAr { get; set; }
        [MaxLength(1500)] public string? BioEn { get; set; }

        public string? PhotoPath { get; set; }
        [MaxLength(200)] public string? Email { get; set; }
        [MaxLength(300)] public string? LinkedInUrl { get; set; }
        [MaxLength(300)] public string? TwitterUrl { get; set; }

        /// <summary>leadership | management | engineering | other</summary>
        [MaxLength(60)] public string GroupKey { get; set; } = "leadership";

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  CERTIFICATION / QUALITY BADGE — ISO etc.
    // ═══════════════════════════════════════════════════════════════════════════
    public class Certification
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [MaxLength(900)] public string? DescAr { get; set; }
        [MaxLength(900)] public string? DescEn { get; set; }

        [MaxLength(120)] public string? IssuedBy { get; set; }
        [MaxLength(120)] public string? CertNumber { get; set; }
        public DateTime? IssuedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public string? LogoPath { get; set; }
        public string? DocumentPath { get; set; }

        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  SITE LOCATION — factory / office / branch pins on the map
    // ═══════════════════════════════════════════════════════════════════════════
    public class SiteLocation
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [MaxLength(500)] public string? AddressAr { get; set; }
        [MaxLength(500)] public string? AddressEn { get; set; }

        [MaxLength(50)] public string? Phone { get; set; }
        [MaxLength(50)] public string? Phone2 { get; set; }
        [MaxLength(200)] public string? Email { get; set; }
        [MaxLength(200)] public string? WorkingHoursAr { get; set; }
        [MaxLength(200)] public string? WorkingHoursEn { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int MapZoom { get; set; } = 14;

        /// <summary>factory | office | branch | warehouse</summary>
        [MaxLength(40)] public string LocationType { get; set; } = "factory";

        public string? ImagePath { get; set; }
        public bool IsPrimary { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  SOCIAL LINK
    // ═══════════════════════════════════════════════════════════════════════════
    public class SocialLink
    {
        public int Id { get; set; }

        [Required, MaxLength(80)] public string Platform { get; set; } = "";     // facebook, linkedin, ...
        [Required, MaxLength(400)] public string Url { get; set; } = "";
        [MaxLength(60)] public string? Icon { get; set; }
        [MaxLength(120)] public string? LabelAr { get; set; }
        [MaxLength(120)] public string? LabelEn { get; set; }
        [MaxLength(20)] public string? Color { get; set; }

        public bool IsActive { get; set; } = true;
        public bool ShowInHeader { get; set; } = false;
        public bool ShowInFooter { get; set; } = true;
        public int SortOrder { get; set; } = 0;
    }
}
