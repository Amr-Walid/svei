using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SVEI.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    EntityName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    EntityId = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    DescAr = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: true),
                    LogoLightPath = table.Column<string>(type: "TEXT", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    BrandType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInStrip = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DescAr = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    IssuedBy = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CertNumber = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: true),
                    DocumentPath = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsStarred = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdminReply = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    RepliedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    SummaryAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    SummaryEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescAr = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LocationAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    LocationEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    MapUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    RegistrationUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    AllowRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: true),
                    CoverImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaqItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionAr = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    QuestionEn = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    AnswerAr = table.Column<string>(type: "TEXT", nullable: true),
                    AnswerEn = table.Column<string>(type: "TEXT", nullable: true),
                    GroupKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GalleryAlbums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    DescAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    CoverImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryAlbums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroSlides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EyebrowAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    EyebrowEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    SubtitleEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ButtonTextAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ButtonTextEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ButtonUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Button2TextAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Button2TextEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Button2Url = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Align = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    OverlayOpacity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSlides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InfoCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TextAr = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    TextEn = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    LinkUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    LinkTextAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    LinkTextEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    AccentColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobCategories_JobCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ThumbPath = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    AltAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    AltEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Folder = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    UploadedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuKey = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    MonthAr = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    MonthEn = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TextAr = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    TextEn = table.Column<string>(type: "TEXT", maxLength: 900, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    UnsubscribeToken = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PageKey = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    SectionKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EyebrowAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    EyebrowEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    SubtitleEn = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    BodyAr = table.Column<string>(type: "TEXT", nullable: true),
                    BodyEn = table.Column<string>(type: "TEXT", nullable: true),
                    ButtonTextAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ButtonTextEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ButtonUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Button2TextAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Button2TextEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Button2Url = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    Image2Path = table.Column<string>(type: "TEXT", nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    ExtraJson = table.Column<string>(type: "TEXT", nullable: true),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    PartnershipType = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdminNotes = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    DescAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_ProductCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    ShortDescAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ShortDescEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescAr = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true),
                    CapacityPerDay = table.Column<int>(type: "INTEGER", nullable: true),
                    CapacityUnitAr = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    CapacityUnitEn = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    IconPath = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    AccentColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    ShortDescAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ShortDescEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescAr = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    IconImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    AccentColor = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AddressAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AddressEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Phone2 = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkingHoursAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkingHoursEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    MapZoom = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationType = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    ValueAr = table.Column<string>(type: "TEXT", nullable: true),
                    ValueEn = table.Column<string>(type: "TEXT", nullable: true),
                    InputType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HintAr = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    IsLocalized = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInHeader = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInFooter = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatCounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    Prefix = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Suffix = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Decimals = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    GroupKey = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatCounters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticPages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    BodyAr = table.Column<string>(type: "TEXT", nullable: true),
                    BodyEn = table.Column<string>(type: "TEXT", nullable: true),
                    HeroImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowInFooter = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RoleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RoleEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BioAr = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    BioEn = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    PhotoPath = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    TwitterUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    GroupKey = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Testimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    RoleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    RoleEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    QuoteAr = table.Column<string>(type: "TEXT", maxLength: 1200, nullable: true),
                    QuoteEn = table.Column<string>(type: "TEXT", maxLength: 1200, nullable: true),
                    AvatarPath = table.Column<string>(type: "TEXT", nullable: true),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    ValueAr = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ValueEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrlRedirects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FromPath = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ToPath = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    IsPermanent = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    HitCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlRedirects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    CaptionAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    CaptionEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventImages_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Company = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventRegistrations_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GalleryImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlbumId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    CaptionAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    CaptionEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GalleryImages_GalleryAlbums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "GalleryAlbums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    JobType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ExperienceLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LocationAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LocationEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SalaryRange = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Vacancies = table.Column<int>(type: "INTEGER", nullable: false),
                    SummaryAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    SummaryEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescAr = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true),
                    RequirementsAr = table.Column<string>(type: "TEXT", nullable: true),
                    RequirementsEn = table.Column<string>(type: "TEXT", nullable: true),
                    BenefitsAr = table.Column<string>(type: "TEXT", nullable: true),
                    BenefitsEn = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUrgent = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPostings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPostings_JobCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "JobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Url = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    BadgeTextAr = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    BadgeTextEn = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    OpenInNewTab = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHighlighted = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuItems_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    ExcerptAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ExcerptEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    BodyAr = table.Column<string>(type: "TEXT", nullable: true),
                    BodyEn = table.Column<string>(type: "TEXT", nullable: true),
                    CoverImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    CoverAltAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    CoverAltEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsPosts_NewsCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "NewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductionLineSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductionLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ValueAr = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ValueEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLineSpecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionLineSpecs_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductionLineId = table.Column<int>(type: "INTEGER", nullable: true),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: true),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Sku = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ModelNumber = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    ShortDescAr = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    ShortDescEn = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    DescAr = table.Column<string>(type: "TEXT", nullable: true),
                    DescEn = table.Column<string>(type: "TEXT", nullable: true),
                    MainImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalShopUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    DatasheetPath = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ShowPrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    InStock = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDesc = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "QuoteRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Company = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Country = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    ProductionLineId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductType = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetBudget = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Timeline = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    AttachmentPath = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdminNotes = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Language = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteRequests_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFeatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    TextAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    TextEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFeatures_ServiceItems_ServiceItemId",
                        column: x => x.ServiceItemId,
                        principalTable: "ServiceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobId = table.Column<int>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "INTEGER", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    CoverLetter = table.Column<string>(type: "TEXT", nullable: true),
                    CvPath = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdminNotes = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_JobPostings_JobId",
                        column: x => x.JobId,
                        principalTable: "JobPostings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    AltAr = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    AltEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    LabelAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LabelEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ValueAr = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    ValueEn = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slug",
                table: "Brands",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_EventId",
                table: "EventImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_EventId",
                table: "EventRegistrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Slug",
                table: "Events",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaqItems_GroupKey",
                table: "FaqItems",
                column: "GroupKey");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryAlbums_Slug",
                table: "GalleryAlbums",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImages_AlbumId",
                table: "GalleryImages",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_InfoCards_GroupKey",
                table: "InfoCards",
                column: "GroupKey");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId",
                table: "JobApplications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_ParentId",
                table: "JobCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCategories_Slug",
                table: "JobCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CategoryId",
                table: "JobPostings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_Slug",
                table: "JobPostings",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_Folder",
                table: "MediaAssets",
                column: "Folder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_MenuId",
                table: "MenuItems",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_MenuKey",
                table: "Menus",
                column: "MenuKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsCategories_Slug",
                table: "NewsCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubscribers_Email",
                table: "NewsletterSubscribers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_CategoryId",
                table: "NewsPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsPosts_Slug",
                table: "NewsPosts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageSections_PageKey_SectionKey",
                table: "PageSections",
                columns: new[] { "PageKey", "SectionKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentId",
                table: "ProductCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Slug",
                table: "ProductCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLines_Slug",
                table: "ProductionLines",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionLineSpecs_ProductionLineId",
                table: "ProductionLineSpecs",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductionLineId",
                table: "Products",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecs_ProductId",
                table: "ProductSpecs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteRequests_ProductionLineId",
                table: "QuoteRequests",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFeatures_ServiceItemId",
                table: "ServiceFeatures",
                column: "ServiceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceItems_Slug",
                table: "ServiceItems",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteSettings_Key",
                table: "SiteSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatCounters_GroupKey",
                table: "StatCounters",
                column: "GroupKey");

            migrationBuilder.CreateIndex(
                name: "IX_StaticPages_Slug",
                table: "StaticPages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Key",
                table: "Translations",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UrlRedirects_FromPath",
                table: "UrlRedirects",
                column: "FromPath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Certifications");

            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "EventImages");

            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "FaqItems");

            migrationBuilder.DropTable(
                name: "GalleryImages");

            migrationBuilder.DropTable(
                name: "HeroSlides");

            migrationBuilder.DropTable(
                name: "InfoCards");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "MediaAssets");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "NewsletterSubscribers");

            migrationBuilder.DropTable(
                name: "NewsPosts");

            migrationBuilder.DropTable(
                name: "PageSections");

            migrationBuilder.DropTable(
                name: "PartnerRequests");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductionLineSpecs");

            migrationBuilder.DropTable(
                name: "ProductSpecs");

            migrationBuilder.DropTable(
                name: "QuoteRequests");

            migrationBuilder.DropTable(
                name: "ServiceFeatures");

            migrationBuilder.DropTable(
                name: "SiteLocations");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "SocialLinks");

            migrationBuilder.DropTable(
                name: "StatCounters");

            migrationBuilder.DropTable(
                name: "StaticPages");

            migrationBuilder.DropTable(
                name: "TeamMembers");

            migrationBuilder.DropTable(
                name: "Testimonials");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "UrlRedirects");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "GalleryAlbums");

            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "NewsCategories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ServiceItems");

            migrationBuilder.DropTable(
                name: "JobCategories");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "ProductionLines");
        }
    }
}
