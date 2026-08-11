using System.ComponentModel.DataAnnotations;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  CONTACT MESSAGE
    // ═══════════════════════════════════════════════════════════════════════════
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string FullName { get; set; } = "";
        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(200)] public string? Company { get; set; }
        [MaxLength(250)] public string? Subject { get; set; }
        [Required] public string Message { get; set; } = "";

        public bool IsRead { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public bool IsStarred { get; set; } = false;
        [MaxLength(1500)] public string? AdminReply { get; set; }
        public DateTime? RepliedAt { get; set; }

        [MaxLength(60)] public string? IpAddress { get; set; }
        [MaxLength(10)] public string? Language { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  QUOTE / RFQ REQUEST — manufacturing enquiry
    // ═══════════════════════════════════════════════════════════════════════════
    public class QuoteRequest
    {
        public int Id { get; set; }

        [Required, MaxLength(150)] public string FullName { get; set; } = "";
        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [Required, MaxLength(30)] public string Phone { get; set; } = "";
        [MaxLength(200)] public string? Company { get; set; }
        [MaxLength(120)] public string? Country { get; set; }

        public int? ProductionLineId { get; set; }
        [MaxLength(250)] public string? ProductType { get; set; }
        public int? Quantity { get; set; }
        [MaxLength(120)] public string? TargetBudget { get; set; }
        [MaxLength(120)] public string? Timeline { get; set; }
        public string? Details { get; set; }
        public string? AttachmentPath { get; set; }

        /// <summary>new | contacted | quoted | won | lost</summary>
        [MaxLength(30)] public string Status { get; set; } = "new";
        public bool IsRead { get; set; } = false;
        [MaxLength(1500)] public string? AdminNotes { get; set; }

        [MaxLength(60)] public string? IpAddress { get; set; }
        [MaxLength(10)] public string? Language { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ProductionLine? ProductionLine { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PARTNERSHIP REQUEST
    // ═══════════════════════════════════════════════════════════════════════════
    public class PartnerRequest
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string CompanyName { get; set; } = "";
        [Required, MaxLength(150)] public string ContactName { get; set; } = "";
        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [MaxLength(30)] public string? Phone { get; set; }
        [MaxLength(300)] public string? Website { get; set; }
        [MaxLength(120)] public string? Country { get; set; }
        [MaxLength(150)] public string? PartnershipType { get; set; }
        public string? Message { get; set; }
        public string? AttachmentPath { get; set; }

        [MaxLength(30)] public string Status { get; set; } = "new";
        public bool IsRead { get; set; } = false;
        [MaxLength(1500)] public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  NEWSLETTER SUBSCRIBER
    // ═══════════════════════════════════════════════════════════════════════════
    public class NewsletterSubscriber
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string Email { get; set; } = "";
        [MaxLength(150)] public string? Name { get; set; }
        [MaxLength(10)] public string? Language { get; set; }

        public bool IsActive { get; set; } = true;
        [MaxLength(80)] public string? UnsubscribeToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
