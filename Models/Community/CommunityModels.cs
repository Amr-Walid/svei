using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  NEWS
    // ═══════════════════════════════════════════════════════════════════════════
    public class NewsCategory
    {
        public int Id { get; set; }
        [Required, MaxLength(150)] public string NameAr { get; set; } = "";
        [Required, MaxLength(150)] public string NameEn { get; set; } = "";
        [Required, MaxLength(150)] public string Slug { get; set; } = "";
        [MaxLength(20)] public string? Color { get; set; }
        [MaxLength(60)] public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<NewsPost> Posts { get; set; } = new List<NewsPost>();
    }

    public class NewsPost
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        [MaxLength(600)] public string? ExcerptAr { get; set; }
        [MaxLength(600)] public string? ExcerptEn { get; set; }
        public string? BodyAr { get; set; }
        public string? BodyEn { get; set; }

        public string? CoverImagePath { get; set; }
        [MaxLength(250)] public string? CoverAltAr { get; set; }
        [MaxLength(250)] public string? CoverAltEn { get; set; }

        [MaxLength(120)] public string? AuthorName { get; set; }
        [MaxLength(400)] public string? Tags { get; set; }   // comma separated

        public bool IsPublished { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public int ViewCount { get; set; } = 0;

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public NewsCategory? Category { get; set; }

        [NotMapped]
        public string[] TagList => string.IsNullOrWhiteSpace(Tags)
            ? Array.Empty<string>()
            : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  EVENTS — exhibitions, factory openings, training days, ceremonies
    // ═══════════════════════════════════════════════════════════════════════════
    public class Event
    {
        public int Id { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        [MaxLength(600)] public string? SummaryAr { get; set; }
        [MaxLength(600)] public string? SummaryEn { get; set; }
        public string? DescAr { get; set; }
        public string? DescEn { get; set; }

        /// <summary>exhibition | conference | training | ceremony | visit | launch | other</summary>
        [MaxLength(40)] public string EventType { get; set; } = "other";

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(250)] public string? LocationAr { get; set; }
        [MaxLength(250)] public string? LocationEn { get; set; }
        [MaxLength(300)] public string? MapUrl { get; set; }

        [MaxLength(300)] public string? RegistrationUrl { get; set; }
        public bool AllowRegistration { get; set; } = false;
        public int? Capacity { get; set; }

        public string? CoverImagePath { get; set; }
        [MaxLength(300)] public string? VideoUrl { get; set; }

        public bool IsPublished { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public int SortOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public ICollection<EventImage> Images { get; set; } = new List<EventImage>();
        public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();

        /// <summary>upcoming | ongoing | past</summary>
        [NotMapped]
        public string Status
        {
            get
            {
                var today = DateTime.UtcNow.Date;
                var s = StartDate?.Date;
                var e = (EndDate ?? StartDate)?.Date;
                if (s is null) return "past";
                if (s > today) return "upcoming";
                if (e >= today) return "ongoing";
                return "past";
            }
        }
    }

    public class EventImage
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        [Required] public string ImagePath { get; set; } = "";
        [MaxLength(250)] public string? CaptionAr { get; set; }
        [MaxLength(250)] public string? CaptionEn { get; set; }
        public int SortOrder { get; set; } = 0;

        public Event Event { get; set; } = null!;
    }

    public class EventRegistration
    {
        public int Id { get; set; }
        public int EventId { get; set; }

        [Required, MaxLength(150)] public string FullName { get; set; } = "";
        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(200)] public string? Company { get; set; }
        [MaxLength(1000)] public string? Notes { get; set; }

        public bool IsConfirmed { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Event Event { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  CAREERS
    // ═══════════════════════════════════════════════════════════════════════════
    public class JobCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string NameAr { get; set; } = "";
        [Required, MaxLength(150)] public string NameEn { get; set; } = "";
        [Required, MaxLength(150)] public string Slug { get; set; } = "";
        [MaxLength(60)] public string? Icon { get; set; }

        public int? ParentId { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public JobCategory? Parent { get; set; }
        public ICollection<JobCategory> Children { get; set; } = new List<JobCategory>();
        public ICollection<JobPosting> Jobs { get; set; } = new List<JobPosting>();

        [NotMapped]
        public string IconClass => string.IsNullOrWhiteSpace(Icon) ? "fa-solid fa-briefcase" : Icon;
    }

    public class JobPosting
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        /// <summary>Full-time | Part-time | Shift | Contract | Internship | Remote</summary>
        [MaxLength(50)] public string JobType { get; set; } = "Full-time";
        /// <summary>Entry | Junior | Mid | Senior | Manager</summary>
        [MaxLength(50)] public string? ExperienceLevel { get; set; }

        [MaxLength(200)] public string? LocationAr { get; set; }
        [MaxLength(200)] public string? LocationEn { get; set; }
        [MaxLength(150)] public string? SalaryRange { get; set; }
        public int Vacancies { get; set; } = 1;

        [MaxLength(600)] public string? SummaryAr { get; set; }
        [MaxLength(600)] public string? SummaryEn { get; set; }
        public string? DescAr { get; set; }
        public string? DescEn { get; set; }
        public string? RequirementsAr { get; set; }
        public string? RequirementsEn { get; set; }
        public string? BenefitsAr { get; set; }
        public string? BenefitsEn { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsUrgent { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public int SortOrder { get; set; } = 0;

        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosingDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public JobCategory Category { get; set; } = null!;
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();

        [NotMapped]
        public bool IsClosed => ClosingDate.HasValue && ClosingDate.Value.Date < DateTime.UtcNow.Date;
    }

    public class JobApplication
    {
        public int Id { get; set; }
        public int JobId { get; set; }

        [Required, MaxLength(150)] public string FullName { get; set; } = "";
        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [Required, MaxLength(30)] public string Phone { get; set; } = "";
        [MaxLength(200)] public string? City { get; set; }
        public int? YearsOfExperience { get; set; }
        [MaxLength(300)] public string? LinkedInUrl { get; set; }
        public string? CoverLetter { get; set; }
        public string? CvPath { get; set; }

        /// <summary>new | reviewing | shortlisted | interviewed | rejected | hired</summary>
        [MaxLength(30)] public string Status { get; set; } = "new";
        public bool IsRead { get; set; } = false;
        [MaxLength(1500)] public string? AdminNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public JobPosting Job { get; set; } = null!;
    }
}
