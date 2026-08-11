using SVEI.Web.Models;

namespace SVEI.Web.ViewModels
{
    public class MenuVm
    {
        public string MenuKey { get; set; } = "";
        public string? Title { get; set; }
        public List<MenuItemVm> Items { get; set; } = new();
    }

    public class MenuItemVm
    {
        public string Label { get; set; } = "";
        public string Url { get; set; } = "#";
        public string? Icon { get; set; }
        public string? Badge { get; set; }
        public bool OpenInNewTab { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsActive { get; set; }
        public List<MenuItemVm> Children { get; set; } = new();
    }

    public class FooterVm
    {
        public string? AboutText { get; set; }
        public string? Copyright { get; set; }
        public string? LogoPath { get; set; }
        public SiteLocation? PrimaryLocation { get; set; }
        public List<SocialLink> Social { get; set; } = new();
        public List<MenuVm> Menus { get; set; } = new();
        public bool ShowNewsletter { get; set; } = true;
        public string? NewsletterTitle { get; set; }
        public string? NewsletterText { get; set; }
    }

    /// <summary>Shared shape for a paged listing page.</summary>
    public class PagedVm<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalItems { get; set; }
        public int TotalPages => PageSize <= 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;
        public string? Query { get; set; }
        public string? Filter { get; set; }
    }
}
