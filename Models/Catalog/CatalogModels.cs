using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SVEI.Web.Models
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  PRODUCTION LINE — Cell Phones / Power Storage / Wearables / Audio
    // ═══════════════════════════════════════════════════════════════════════════
    public class ProductionLine
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        [MaxLength(600)] public string? ShortDescAr { get; set; }
        [MaxLength(600)] public string? ShortDescEn { get; set; }
        public string? DescAr { get; set; }
        public string? DescEn { get; set; }

        /// <summary>e.g. 2000 — units per day</summary>
        public int? CapacityPerDay { get; set; }
        [MaxLength(80)] public string? CapacityUnitAr { get; set; }
        [MaxLength(80)] public string? CapacityUnitEn { get; set; }

        public string? ImagePath { get; set; }
        public string? IconPath { get; set; }
        [MaxLength(60)] public string? Icon { get; set; }
        [MaxLength(20)] public string? AccentColor { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductionLineSpec> Specs { get; set; } = new List<ProductionLineSpec>();
    }

    public class ProductionLineSpec
    {
        public int Id { get; set; }
        public int ProductionLineId { get; set; }

        [Required, MaxLength(200)] public string LabelAr { get; set; } = "";
        [Required, MaxLength(200)] public string LabelEn { get; set; } = "";
        [MaxLength(300)] public string? ValueAr { get; set; }
        [MaxLength(300)] public string? ValueEn { get; set; }
        [MaxLength(60)] public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;

        public ProductionLine ProductionLine { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PRODUCT CATEGORY
    // ═══════════════════════════════════════════════════════════════════════════
    public class ProductCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";
        [MaxLength(600)] public string? DescAr { get; set; }
        [MaxLength(600)] public string? DescEn { get; set; }

        [MaxLength(60)] public string? Icon { get; set; }
        public string? ImagePath { get; set; }

        public int? ParentId { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ProductCategory? Parent { get; set; }
        public ICollection<ProductCategory> Children { get; set; } = new List<ProductCategory>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  PRODUCT
    // ═══════════════════════════════════════════════════════════════════════════
    public class Product
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductionLineId { get; set; }
        public int? BrandId { get; set; }

        [Required, MaxLength(250)] public string NameAr { get; set; } = "";
        [Required, MaxLength(250)] public string NameEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";
        [MaxLength(80)] public string? Sku { get; set; }
        [MaxLength(80)] public string? ModelNumber { get; set; }

        [MaxLength(600)] public string? ShortDescAr { get; set; }
        [MaxLength(600)] public string? ShortDescEn { get; set; }
        public string? DescAr { get; set; }
        public string? DescEn { get; set; }

        public string? MainImagePath { get; set; }
        [MaxLength(300)] public string? ExternalShopUrl { get; set; }
        [MaxLength(300)] public string? DatasheetPath { get; set; }

        public decimal? Price { get; set; }
        [MaxLength(10)] public string? Currency { get; set; } = "EGP";
        public bool ShowPrice { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public bool InStock { get; set; } = true;
        public int ViewCount { get; set; } = 0;
        public int SortOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public ProductCategory? Category { get; set; }
        public ProductionLine? ProductionLine { get; set; }
        public Brand? Brand { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductSpec> Specs { get; set; } = new List<ProductSpec>();
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required] public string ImagePath { get; set; } = "";
        [MaxLength(250)] public string? AltAr { get; set; }
        [MaxLength(250)] public string? AltEn { get; set; }
        public int SortOrder { get; set; } = 0;

        public Product Product { get; set; } = null!;
    }

    public class ProductSpec
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required, MaxLength(200)] public string LabelAr { get; set; } = "";
        [Required, MaxLength(200)] public string LabelEn { get; set; } = "";
        [MaxLength(400)] public string? ValueAr { get; set; }
        [MaxLength(400)] public string? ValueEn { get; set; }
        public int SortOrder { get; set; } = 0;

        public Product Product { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  BRAND / PARTNER — Oraimo, Infinix, Itel, Gtide, UniTronics
    // ═══════════════════════════════════════════════════════════════════════════
    public class Brand
    {
        public int Id { get; set; }

        [Required, MaxLength(200)] public string NameAr { get; set; } = "";
        [Required, MaxLength(200)] public string NameEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        [MaxLength(900)] public string? DescAr { get; set; }
        [MaxLength(900)] public string? DescEn { get; set; }

        public string? LogoPath { get; set; }
        public string? LogoLightPath { get; set; }
        [MaxLength(300)] public string? WebsiteUrl { get; set; }

        /// <summary>client | partner | supplier | certification</summary>
        [MaxLength(40)] public string BrandType { get; set; } = "client";

        public bool IsActive { get; set; } = true;
        public bool ShowInStrip { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  SERVICE — OEM/ODM, SMT, Assembly, QC, Packaging ...
    // ═══════════════════════════════════════════════════════════════════════════
    public class ServiceItem
    {
        public int Id { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";

        [MaxLength(600)] public string? ShortDescAr { get; set; }
        [MaxLength(600)] public string? ShortDescEn { get; set; }
        public string? DescAr { get; set; }
        public string? DescEn { get; set; }

        [MaxLength(60)] public string? Icon { get; set; }
        public string? ImagePath { get; set; }
        public string? IconImagePath { get; set; }
        [MaxLength(20)] public string? AccentColor { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)] public string? MetaTitle { get; set; }
        [MaxLength(300)] public string? MetaDesc { get; set; }

        public ICollection<ServiceFeature> Features { get; set; } = new List<ServiceFeature>();
    }

    public class ServiceFeature
    {
        public int Id { get; set; }
        public int ServiceItemId { get; set; }

        [Required, MaxLength(250)] public string TextAr { get; set; } = "";
        [Required, MaxLength(250)] public string TextEn { get; set; } = "";
        [MaxLength(60)] public string? Icon { get; set; }
        public int SortOrder { get; set; } = 0;

        public ServiceItem ServiceItem { get; set; } = null!;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  GALLERY
    // ═══════════════════════════════════════════════════════════════════════════
    public class GalleryAlbum
    {
        public int Id { get; set; }

        [Required, MaxLength(250)] public string TitleAr { get; set; } = "";
        [Required, MaxLength(250)] public string TitleEn { get; set; } = "";
        [Required, MaxLength(180)] public string Slug { get; set; } = "";
        [MaxLength(600)] public string? DescAr { get; set; }
        [MaxLength(600)] public string? DescEn { get; set; }

        public string? CoverImagePath { get; set; }

        public bool IsPublished { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
    }

    public class GalleryImage
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }

        [Required] public string ImagePath { get; set; } = "";
        [MaxLength(250)] public string? CaptionAr { get; set; }
        [MaxLength(250)] public string? CaptionEn { get; set; }
        public int SortOrder { get; set; } = 0;

        public GalleryAlbum Album { get; set; } = null!;
    }
}
