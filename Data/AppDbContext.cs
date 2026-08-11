using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── CONTENT ───────────────────────────────────────────────────────────
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
        public DbSet<PageSection> PageSections => Set<PageSection>();
        public DbSet<InfoCard> InfoCards => Set<InfoCard>();
        public DbSet<StatCounter> StatCounters => Set<StatCounter>();
        public DbSet<HeroSlide> HeroSlides => Set<HeroSlide>();
        public DbSet<FaqItem> FaqItems => Set<FaqItem>();
        public DbSet<Testimonial> Testimonials => Set<Testimonial>();
        public DbSet<StaticPage> StaticPages => Set<StaticPage>();

        // ── CATALOG ───────────────────────────────────────────────────────────
        public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
        public DbSet<ProductionLineSpec> ProductionLineSpecs => Set<ProductionLineSpec>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<ProductSpec> ProductSpecs => Set<ProductSpec>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
        public DbSet<ServiceFeature> ServiceFeatures => Set<ServiceFeature>();
        public DbSet<GalleryAlbum> GalleryAlbums => Set<GalleryAlbum>();
        public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();

        // ── COMMUNITY ─────────────────────────────────────────────────────────
        public DbSet<NewsCategory> NewsCategories => Set<NewsCategory>();
        public DbSet<NewsPost> NewsPosts => Set<NewsPost>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventImage> EventImages => Set<EventImage>();
        public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
        public DbSet<JobCategory> JobCategories => Set<JobCategory>();
        public DbSet<JobPosting> JobPostings => Set<JobPosting>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();

        // ── COMPANY ───────────────────────────────────────────────────────────
        public DbSet<Milestone> Milestones => Set<Milestone>();
        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
        public DbSet<Certification> Certifications => Set<Certification>();
        public DbSet<SiteLocation> SiteLocations => Set<SiteLocation>();
        public DbSet<SocialLink> SocialLinks => Set<SocialLink>();

        // ── INBOX ─────────────────────────────────────────────────────────────
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<QuoteRequest> QuoteRequests => Set<QuoteRequest>();
        public DbSet<PartnerRequest> PartnerRequests => Set<PartnerRequest>();
        public DbSet<NewsletterSubscriber> NewsletterSubscribers => Set<NewsletterSubscriber>();

        // ── SETTINGS ──────────────────────────────────────────────────────────
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Translation> Translations => Set<Translation>();
        public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<UrlRedirect> UrlRedirects => Set<UrlRedirect>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ── CONTENT ───────────────────────────────────────────────────────
            b.Entity<SiteSetting>().HasIndex(x => x.Key).IsUnique();
            b.Entity<PageSection>().HasIndex(x => new { x.PageKey, x.SectionKey }).IsUnique();
            b.Entity<InfoCard>().HasIndex(x => x.GroupKey);
            b.Entity<StatCounter>().HasIndex(x => x.GroupKey);
            b.Entity<FaqItem>().HasIndex(x => x.GroupKey);
            b.Entity<StaticPage>().HasIndex(x => x.Slug).IsUnique();

            // ── CATALOG ───────────────────────────────────────────────────────
            b.Entity<ProductionLine>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<ProductionLineSpec>()
                .HasOne(x => x.ProductionLine).WithMany(x => x.Specs)
                .HasForeignKey(x => x.ProductionLineId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<ProductCategory>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<ProductCategory>()
                .HasOne(x => x.Parent).WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<Product>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<Product>().Property(x => x.Price).HasPrecision(18, 2);
            b.Entity<Product>()
                .HasOne(x => x.Category).WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
            b.Entity<Product>()
                .HasOne(x => x.ProductionLine).WithMany(x => x.Products)
                .HasForeignKey(x => x.ProductionLineId).OnDelete(DeleteBehavior.SetNull);
            b.Entity<Product>()
                .HasOne(x => x.Brand).WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.SetNull);
            b.Entity<ProductImage>()
                .HasOne(x => x.Product).WithMany(x => x.Images)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<ProductSpec>()
                .HasOne(x => x.Product).WithMany(x => x.Specs)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<Brand>().HasIndex(x => x.Slug).IsUnique();

            b.Entity<ServiceItem>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<ServiceFeature>()
                .HasOne(x => x.ServiceItem).WithMany(x => x.Features)
                .HasForeignKey(x => x.ServiceItemId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<GalleryAlbum>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<GalleryImage>()
                .HasOne(x => x.Album).WithMany(x => x.Images)
                .HasForeignKey(x => x.AlbumId).OnDelete(DeleteBehavior.Cascade);

            // ── COMMUNITY ─────────────────────────────────────────────────────
            b.Entity<NewsCategory>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<NewsPost>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<NewsPost>()
                .HasOne(x => x.Category).WithMany(x => x.Posts)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);

            b.Entity<Event>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<EventImage>()
                .HasOne(x => x.Event).WithMany(x => x.Images)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<EventRegistration>()
                .HasOne(x => x.Event).WithMany(x => x.Registrations)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Cascade);

            b.Entity<JobCategory>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<JobCategory>()
                .HasOne(x => x.Parent).WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
            b.Entity<JobPosting>().HasIndex(x => x.Slug).IsUnique();
            b.Entity<JobPosting>()
                .HasOne(x => x.Category).WithMany(x => x.Jobs)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.Entity<JobApplication>()
                .HasOne(x => x.Job).WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobId).OnDelete(DeleteBehavior.Cascade);

            // ── INBOX ─────────────────────────────────────────────────────────
            b.Entity<QuoteRequest>()
                .HasOne(x => x.ProductionLine).WithMany()
                .HasForeignKey(x => x.ProductionLineId).OnDelete(DeleteBehavior.SetNull);
            b.Entity<NewsletterSubscriber>().HasIndex(x => x.Email).IsUnique();

            // ── SETTINGS ──────────────────────────────────────────────────────
            b.Entity<Menu>().HasIndex(x => x.MenuKey).IsUnique();
            b.Entity<MenuItem>()
                .HasOne(x => x.Menu).WithMany(x => x.Items)
                .HasForeignKey(x => x.MenuId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<MenuItem>()
                .HasOne(x => x.Parent).WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

            b.Entity<Translation>().HasIndex(x => x.Key).IsUnique();
            b.Entity<UrlRedirect>().HasIndex(x => x.FromPath).IsUnique();
            b.Entity<MediaAsset>().HasIndex(x => x.Folder);
            b.Entity<AuditLog>().HasIndex(x => x.CreatedAt);
        }
    }
}
