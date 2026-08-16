using SVEI.Web.Models;

namespace SVEI.Web.ViewModels
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  HOME
    // ═══════════════════════════════════════════════════════════════════════════
    public class HomeVm
    {
        public List<HeroSlide> Slides { get; set; } = new();
        public List<StatCounter> Stats { get; set; } = new();
        public List<InfoCard> Features { get; set; } = new();
        public List<ProductionLine> Lines { get; set; } = new();
        public List<ServiceItem> Services { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        public List<NewsPost> LatestNews { get; set; } = new();
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<JobPosting> OpenJobs { get; set; } = new();
        public List<FaqItem> Faqs { get; set; } = new();
        public List<Testimonial> Testimonials { get; set; } = new();
        public List<Certification> Certifications { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  ABOUT
    // ═══════════════════════════════════════════════════════════════════════════
    public class AboutVm
    {
        public List<InfoCard> Values { get; set; } = new();
        public List<Milestone> Milestones { get; set; } = new();
        public List<TeamMember> Team { get; set; } = new();
        public List<Certification> Certifications { get; set; } = new();
        public List<StatCounter> Stats { get; set; } = new();
        public List<ProductionLine> Lines { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  SERVICES / PRODUCTION LINES / BRANDS
    // ═══════════════════════════════════════════════════════════════════════════
    public class ServicesVm
    {
        public List<ServiceItem> Services { get; set; } = new();
        public List<InfoCard> Process { get; set; } = new();
        public List<FaqItem> Faqs { get; set; } = new();
    }

    public class ServiceDetailsVm
    {
        public ServiceItem Service { get; set; } = null!;
        public List<ServiceItem> Others { get; set; } = new();
        public List<FaqItem> Faqs { get; set; } = new();
    }

    public class ProductionLinesVm
    {
        public List<ProductionLine> Lines { get; set; } = new();
        public List<StatCounter> Stats { get; set; } = new();
    }

    public class ProductionLineDetailsVm
    {
        public ProductionLine Line { get; set; } = null!;
        public List<Product> Products { get; set; } = new();
        public List<ProductionLine> Others { get; set; } = new();
    }

    public class BrandsVm
    {
        public List<Brand> Clients { get; set; } = new();
        public List<Brand> Partners { get; set; } = new();
        public List<Brand> Suppliers { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PRODUCTS
    // ═══════════════════════════════════════════════════════════════════════════
    public class ProductsVm
    {
        public PagedVm<Product> Paged { get; set; } = new();
        public List<ProductCategory> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<ProductionLine> Lines { get; set; } = new();
        public string? CategorySlug { get; set; }
        public string? BrandSlug { get; set; }
        public string? LineSlug { get; set; }
    }

    public class ProductDetailsVm
    {
        public Product Product { get; set; } = null!;
        public List<Product> Related { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  NEWS
    // ═══════════════════════════════════════════════════════════════════════════
    public class NewsVm
    {
        public PagedVm<NewsPost> Paged { get; set; } = new();
        public List<NewsCategory> Categories { get; set; } = new();
        public List<NewsPost> Featured { get; set; } = new();
        public string? CategorySlug { get; set; }
    }

    public class NewsDetailsVm
    {
        public NewsPost Post { get; set; } = null!;
        public List<NewsPost> Related { get; set; } = new();
        public NewsPost? Prev { get; set; }
        public NewsPost? Next { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  EVENTS
    // ═══════════════════════════════════════════════════════════════════════════
    public class EventsVm
    {
        public List<Event> Upcoming { get; set; } = new();
        public PagedVm<Event> Paged { get; set; } = new();
        /// <summary>all | upcoming | ongoing | past</summary>
        public string Status { get; set; } = "all";
        public string? EventType { get; set; }
        public List<string> Types { get; set; } = new();
    }

    public class EventDetailsVm
    {
        public Event Event { get; set; } = null!;
        public List<Event> Related { get; set; } = new();
        public EventRegistrationForm Form { get; set; } = new();
        public int RegisteredCount { get; set; }
        public bool IsFull { get; set; }
    }

    public class EventRegistrationForm
    {
        public int EventId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Notes { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  CAREERS
    // ═══════════════════════════════════════════════════════════════════════════
    public class CareersVm
    {
        public PagedVm<JobPosting> Paged { get; set; } = new();
        public List<JobCategory> Categories { get; set; } = new();
        /// <summary>Contract types that at least one open posting actually uses.</summary>
        public List<string> Types { get; set; } = new();
        public List<InfoCard> WhyUs { get; set; } = new();
        public string? CategorySlug { get; set; }
        public string? JobType { get; set; }
    }

    public class JobDetailsVm
    {
        public JobPosting Job { get; set; } = null!;
        public List<JobPosting> Similar { get; set; } = new();
        public JobApplicationForm Form { get; set; } = new();
    }

    public class JobApplicationForm
    {
        public int JobId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? City { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? CoverLetter { get; set; }
        public IFormFile? Cv { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  GALLERY
    // ═══════════════════════════════════════════════════════════════════════════
    public class GalleryVm
    {
        public List<GalleryAlbum> Albums { get; set; } = new();
    }

    public class GalleryDetailsVm
    {
        public GalleryAlbum Album { get; set; } = null!;
        public List<GalleryAlbum> Others { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  CONTACT / QUOTE
    // ═══════════════════════════════════════════════════════════════════════════
    public class ContactVm
    {
        public List<SiteLocation> Locations { get; set; } = new();
        public List<SocialLink> Social { get; set; } = new();
        public List<FaqItem> Faqs { get; set; } = new();
        public ContactForm Form { get; set; } = new();
    }

    public class ContactForm
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; } = "";
        /// <summary>Honeypot — must stay empty.</summary>
        public string? Website { get; set; }
    }

    public class QuoteVm
    {
        public List<ProductionLine> Lines { get; set; } = new();
        public List<InfoCard> Steps { get; set; } = new();
        public QuoteForm Form { get; set; } = new();
    }

    public class QuoteForm
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Company { get; set; }
        public string? Country { get; set; }
        public int? ProductionLineId { get; set; }
        public string? ProductType { get; set; }
        public int? Quantity { get; set; }
        public string? TargetBudget { get; set; }
        public string? Timeline { get; set; }
        public string? Details { get; set; }
        public IFormFile? Attachment { get; set; }
        /// <summary>Honeypot — must stay empty.</summary>
        public string? Website { get; set; }
    }
}
