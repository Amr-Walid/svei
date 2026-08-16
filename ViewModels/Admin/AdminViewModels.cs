using SVEI.Web.Admin;

namespace SVEI.Web.ViewModels.Admin
{
    /// <summary>Generic list page for any entity in the schema.</summary>
    public class EntityListVm
    {
        public EntityDef Def { get; set; } = default!;
        public List<object> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int Total { get; set; }
        public string? Query { get; set; }
        public string? Group { get; set; }
        public List<string> Groups { get; set; } = new();
        public Dictionary<string, List<(int Id, string Label)>> Lookups { get; set; } = new();

        public int TotalPages => Total == 0 ? 1 : (int)Math.Ceiling(Total / (double)PageSize);
        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;
        public int From => Total == 0 ? 0 : (Page - 1) * PageSize + 1;
        public int To => Math.Min(Page * PageSize, Total);
    }

    /// <summary>Generic create/edit form for any entity in the schema.</summary>
    public class EntityFormVm
    {
        public EntityDef Def { get; set; } = default!;
        public object Entity { get; set; } = default!;
        public bool IsNew { get; set; }
        public Dictionary<string, List<(int Id, string Label)>> Lookups { get; set; } = new();

        /// <summary>Child rows (specs / images / features) rendered under the form.</summary>
        public EntityDef? ChildDef { get; set; }
        public List<object> Children { get; set; } = new();

        public int Id => AdminSchema.GetValue(Entity, "Id") as int? ?? 0;
    }

    /// <summary>Model passed to the generic _Field partial.</summary>
    public class FieldVm
    {
        public Field Field { get; set; } = default!;
        public object Entity { get; set; } = default!;
        public Dictionary<string, List<(int Id, string Label)>> Lookups { get; set; } = new();

        public FieldVm() { }
        public FieldVm(Field f, object entity, Dictionary<string, List<(int Id, string Label)>>? lookups = null)
        { Field = f; Entity = entity; Lookups = lookups ?? new(); }
    }

    /// <summary>Dashboard counters + recent activity.</summary>
    public class DashboardVm
    {
        public int Products { get; set; }
        public int ProductionLines { get; set; }
        public int News { get; set; }
        public int Events { get; set; }
        public int Jobs { get; set; }
        public int Brands { get; set; }
        public int Services { get; set; }
        public int GalleryImages { get; set; }
        public int Pages { get; set; }
        public int Settings { get; set; }
        public int Translations { get; set; }
        public int Sections { get; set; }

        public int UnreadMessages { get; set; }
        public int UnreadQuotes { get; set; }
        public int UnreadApplications { get; set; }
        public int UnreadRegistrations { get; set; }
        public int UnreadPartners { get; set; }
        public int Subscribers { get; set; }

        public List<RecentItem> Recent { get; set; } = new();
        public List<UpcomingItem> Upcoming { get; set; } = new();
        public List<AuditItem> Activity { get; set; } = new();
    }

    public record RecentItem(string Kind, string IconClass, string Title, string Url, DateTime When, bool Unread);
    public record UpcomingItem(string Title, DateTime? When, string Url, string TypeLabel);
    public record AuditItem(string? User, string? Action, string? Entity, string? Summary, DateTime When);

    /// <summary>Media library browser.</summary>
    public class MediaVm
    {
        public List<Models.MediaAsset> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 48;
        public int Total { get; set; }
        public string? Query { get; set; }
        public string? Folder { get; set; }
        public List<string> Folders { get; set; } = new();
        public long TotalBytes { get; set; }

        public int TotalPages => Total == 0 ? 1 : (int)Math.Ceiling(Total / (double)PageSize);
    }

    /// <summary>Admin user management.</summary>
    public class AdminUserVm
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public bool LockedOut { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    /// <summary>Grouped settings editor (tabs by Group).</summary>
    public class SettingsGroupVm
    {
        public string Group { get; set; } = "";
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public string Icon { get; set; } = "fa-solid fa-sliders";
        public List<Models.SiteSetting> Items { get; set; } = new();
    }

    public class SettingsPageVm
    {
        public List<SettingsGroupVm> Groups { get; set; } = new();
        public string Active { get; set; } = "general";
    }
}
