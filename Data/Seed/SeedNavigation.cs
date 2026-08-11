using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>Header + footer menus, fully editable from the admin panel.</summary>
    public static class SeedNavigation
    {
        public static async Task RunAsync(AppDbContext db)
        {
            if (await db.Menus.AnyAsync()) return;

            var header = new Menu
            {
                MenuKey = "header", TitleAr = "القائمة الرئيسية", TitleEn = "Main menu", SortOrder = 1,
                Items =
                {
                    new MenuItem { LabelAr = "الرئيسية", LabelEn = "Home", Url = "/", SortOrder = 1 },
                    new MenuItem { LabelAr = "من نحن", LabelEn = "About", Url = "/about", SortOrder = 2 },
                    new MenuItem { LabelAr = "خطوط الإنتاج", LabelEn = "Production Lines", Url = "/production-lines", SortOrder = 3 },
                    new MenuItem { LabelAr = "خدماتنا", LabelEn = "Services", Url = "/services", SortOrder = 4 },
                    new MenuItem { LabelAr = "المنتجات", LabelEn = "Products", Url = "/products", SortOrder = 5 },
                    new MenuItem { LabelAr = "الأخبار", LabelEn = "News", Url = "/news", SortOrder = 6 },
                    new MenuItem { LabelAr = "الفعاليات", LabelEn = "Events", Url = "/events", SortOrder = 7 },
                    new MenuItem { LabelAr = "الوظائف", LabelEn = "Careers", Url = "/careers", SortOrder = 8 },
                    new MenuItem { LabelAr = "اتصل بنا", LabelEn = "Contact", Url = "/contact", SortOrder = 9 },
                    new MenuItem { LabelAr = "اطلب عرض سعر", LabelEn = "Get a Quote", Url = "/quote", SortOrder = 10, IsHighlighted = true }
                }
            };

            var footerCompany = new Menu
            {
                MenuKey = "footer-company", TitleAr = "الشركة", TitleEn = "Company", SortOrder = 2,
                Items =
                {
                    new MenuItem { LabelAr = "من نحن", LabelEn = "About us", Url = "/about", SortOrder = 1 },
                    new MenuItem { LabelAr = "الأخبار", LabelEn = "News", Url = "/news", SortOrder = 2 },
                    new MenuItem { LabelAr = "الفعاليات", LabelEn = "Events", Url = "/events", SortOrder = 3 },
                    new MenuItem { LabelAr = "الوظائف", LabelEn = "Careers", Url = "/careers", SortOrder = 4 },
                    new MenuItem { LabelAr = "معرض الصور", LabelEn = "Gallery", Url = "/gallery", SortOrder = 5 }
                }
            };

            var footerServices = new Menu
            {
                MenuKey = "footer-services", TitleAr = "ماذا نقدم", TitleEn = "What we do", SortOrder = 3,
                Items =
                {
                    new MenuItem { LabelAr = "تصنيع OEM و ODM", LabelEn = "OEM & ODM", Url = "/services/oem-odm", SortOrder = 1 },
                    new MenuItem { LabelAr = "التجميع السطحي SMT", LabelEn = "SMT Assembly", Url = "/services/smt-assembly", SortOrder = 2 },
                    new MenuItem { LabelAr = "مراقبة الجودة", LabelEn = "Quality Control", Url = "/services/quality-control", SortOrder = 3 },
                    new MenuItem { LabelAr = "التغليف واللوجستيات", LabelEn = "Packaging & Logistics", Url = "/services/packaging-logistics", SortOrder = 4 },
                    new MenuItem { LabelAr = "خدمات ما بعد البيع", LabelEn = "After-Sales", Url = "/services/after-sales", SortOrder = 5 }
                }
            };

            var footerLegal = new Menu
            {
                MenuKey = "footer-legal", TitleAr = "روابط", TitleEn = "Links", SortOrder = 4,
                Items =
                {
                    new MenuItem { LabelAr = "اتصل بنا", LabelEn = "Contact", Url = "/contact", SortOrder = 1 },
                    new MenuItem { LabelAr = "اطلب عرض سعر", LabelEn = "Request a Quote", Url = "/quote", SortOrder = 2 },
                    new MenuItem { LabelAr = "المتجر الإلكتروني", LabelEn = "Online Shop", Url = "https://yallatager.com/ar/Silicon-Valley-Product.html", OpenInNewTab = true, SortOrder = 3 },
                    new MenuItem { LabelAr = "سياسة الخصوصية", LabelEn = "Privacy Policy", Url = "/page/privacy", SortOrder = 4 },
                    new MenuItem { LabelAr = "الشروط والأحكام", LabelEn = "Terms & Conditions", Url = "/page/terms", SortOrder = 5 }
                }
            };

            db.Menus.AddRange(header, footerCompany, footerServices, footerLegal);
            await db.SaveChangesAsync();

            // Redirects preserving the old static site's URLs
            if (!await db.UrlRedirects.AnyAsync())
            {
                db.UrlRedirects.AddRange(
                    new UrlRedirect { FromPath = "/index.html", ToPath = "/" },
                    new UrlRedirect { FromPath = "/contact.html", ToPath = "/contact" },
                    new UrlRedirect { FromPath = "/about.html", ToPath = "/about" },
                    new UrlRedirect { FromPath = "/services.html", ToPath = "/services" },
                    new UrlRedirect { FromPath = "/products.html", ToPath = "/products" }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
