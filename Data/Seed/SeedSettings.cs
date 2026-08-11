using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>
    /// Global settings + UI translations. Data taken from the live svei.tech site.
    /// </summary>
    public static class SeedSettings
    {
        public static async Task RunAsync(AppDbContext db)
        {
            await SettingsAsync(db);
            await TranslationsAsync(db);
            await SocialAsync(db);
            await LocationsAsync(db);
        }

        private static async Task SettingsAsync(AppDbContext db)
        {
            var existing = await db.SiteSettings.Select(s => s.Key).ToListAsync();
            var rows = Defaults().Where(s => !existing.Contains(s.Key)).ToList();
            if (rows.Count == 0) return;
            db.SiteSettings.AddRange(rows);
            await db.SaveChangesAsync();
        }

        private static IEnumerable<SiteSetting> Defaults()
        {
            SiteSetting S(string key, string group, string? ar, string? en, string type = "text",
                          string? labelAr = null, string? labelEn = null, bool localized = true, int sort = 0)
                => new()
                {
                    Key = key,
                    Group = group,
                    ValueAr = ar,
                    ValueEn = en,
                    InputType = type,
                    LabelAr = labelAr,
                    LabelEn = labelEn,
                    IsLocalized = localized,
                    SortOrder = sort
                };

            // ── general ───────────────────────────────────────────────────────
            yield return S("site.name", "general", "سيليكون فالي للصناعات الإلكترونية",
                "Silicon Valley for Electronic Industries", "text", "اسم الموقع", "Site name", true, 1);
            yield return S("site.short_name", "general", "SVEI", "SVEI", "text", "الاسم المختصر", "Short name", true, 2);
            yield return S("site.tagline", "general",
                "نصنع تكنولوجيا الغد في مصر",
                "Empowering Tomorrow's Technology, Made in Egypt", "text", "الشعار النصي", "Tagline", true, 3);
            yield return S("site.description", "general",
                "أول مصنع متكامل للإلكترونيات في المنطقة الاقتصادية لقناة السويس، متخصص في تصنيع الهواتف المحمولة وأجهزة تخزين الطاقة والأجهزة القابلة للارتداء وملحقات الصوت.",
                "An integrated electronics manufacturing facility in the Suez Canal Economic Zone, specialising in mobile phones, power storage, wearables and audio peripherals.",
                "textarea", "وصف الشركة", "Company description", true, 4);
            yield return S("site.founded_year", "general", "2023", "2023", "text", "سنة التأسيس", "Founded", false, 5);
            yield return S("site.logo_dark", "general", "/img/brand/logo-dark.svg", null, "image", "الشعار (خلفية فاتحة)", "Logo (light bg)", false, 6);
            yield return S("site.logo_light", "general", "/img/brand/logo-light.svg", null, "image", "الشعار (خلفية داكنة)", "Logo (dark bg)", false, 7);
            yield return S("site.favicon", "general", "/img/brand/favicon.svg", null, "image", "الأيقونة المفضلة", "Favicon", false, 8);

            // ── contact ───────────────────────────────────────────────────────
            yield return S("contact.email", "contact", "info@svei.tech", "info@svei.tech", "email", "البريد الإلكتروني", "Email", false, 1);
            yield return S("contact.email2", "contact", "", "", "email", "بريد إضافي", "Secondary email", false, 2);
            yield return S("contact.phone", "contact", "(+20)1098887606", "(+20)1098887606", "text", "رقم الهاتف", "Phone", false, 3);
            yield return S("contact.whatsapp", "contact", "201098887606", "201098887606", "text", "واتساب", "WhatsApp", false, 4);
            yield return S("contact.address", "contact",
                "مصر، السويس – العين السخنة، تيدا – المنطقة الاقتصادية لقناة السويس",
                "EGYPT, Suez – Ain Sokhna, TEDA – Suez Canal Economic Zone",
                "textarea", "العنوان", "Address", true, 5);
            yield return S("contact.hours", "contact",
                "السبت – الخميس: 8:00 ص – 5:00 م",
                "Sat – Thu: 8:00 AM – 5:00 PM", "text", "مواعيد العمل", "Working hours", true, 6);
            yield return S("contact.support_note", "contact", "دعم فني 24/7", "24/7 Technical Support", "text", "ملاحظة الدعم", "Support note", true, 7);
            yield return S("contact.lat", "contact", "29.6725549", null, "text", "خط العرض", "Latitude", false, 8);
            yield return S("contact.lng", "contact", "32.30928", null, "text", "خط الطول", "Longitude", false, 9);
            yield return S("contact.map_zoom", "contact", "13", null, "number", "تقريب الخريطة", "Map zoom", false, 10);
            yield return S("contact.shop_url", "contact", "https://yallatager.com/ar/Silicon-Valley-Product.html", null, "url", "رابط المتجر", "Shop URL", false, 11);

            // ── seo ───────────────────────────────────────────────────────────
            yield return S("seo.title", "seo",
                "SVEI | سيليكون فالي للصناعات الإلكترونية",
                "SVEI | Silicon Valley for Electronic Industries", "text", "عنوان SEO", "SEO title", true, 1);
            yield return S("seo.description", "seo",
                "مصنع سيليكون فالي للصناعات الإلكترونية — تصنيع الهواتف المحمولة، بنوك الطاقة، الأجهزة القابلة للارتداء وسماعات الصوت في المنطقة الاقتصادية لقناة السويس.",
                "SVEI — mobile phone, power bank, wearable and audio device manufacturing in the Suez Canal Economic Zone, Egypt.",
                "textarea", "وصف SEO", "SEO description", true, 2);
            yield return S("seo.keywords", "seo",
                "تصنيع إلكترونيات، مصنع موبايلات مصر، السخنة، المنطقة الاقتصادية، OEM، ODM",
                "electronics manufacturing, Egypt phone factory, Ain Sokhna, SCZONE, OEM, ODM", "textarea", "الكلمات المفتاحية", "Keywords", true, 3);
            yield return S("seo.og_image", "seo", "/img/hero/hero-factory.webp", null, "image", "صورة المشاركة", "OG image", false, 4);

            // ── theme ─────────────────────────────────────────────────────────
            yield return S("theme.primary", "theme", "#E31B23", null, "color", "اللون الأساسي", "Primary colour", false, 1);
            yield return S("theme.accent", "theme", "#22C6E8", null, "color", "لون التمييز", "Accent colour", false, 2);
            yield return S("theme.ink", "theme", "#0A0D14", null, "color", "لون النص الداكن", "Ink colour", false, 3);

            // ── features / toggles ────────────────────────────────────────────
            yield return S("feature.news", "features", "1", null, "bool", "تفعيل الأخبار", "Enable News", false, 1);
            yield return S("feature.events", "features", "1", null, "bool", "تفعيل الفعاليات", "Enable Events", false, 2);
            yield return S("feature.careers", "features", "1", null, "bool", "تفعيل الوظائف", "Enable Careers", false, 3);
            yield return S("feature.products", "features", "1", null, "bool", "تفعيل المنتجات", "Enable Products", false, 4);
            yield return S("feature.gallery", "features", "1", null, "bool", "تفعيل معرض الصور", "Enable Gallery", false, 5);
            yield return S("feature.newsletter", "features", "1", null, "bool", "تفعيل النشرة البريدية", "Enable Newsletter", false, 6);
            yield return S("feature.quote", "features", "1", null, "bool", "تفعيل طلب عرض سعر", "Enable RFQ", false, 7);

            // ── footer ────────────────────────────────────────────────────────
            yield return S("footer.about", "footer",
                "سيليكون فالي للصناعات الإلكترونية — شريكك الموثوق في التصنيع الإلكتروني المتكامل داخل مصر.",
                "Silicon Valley for Electronic Industries — your trusted partner for integrated electronics manufacturing in Egypt.",
                "textarea", "نبذة الفوتر", "Footer about", true, 1);
            yield return S("footer.copyright", "footer",
                "جميع الحقوق محفوظة © {year} سيليكون فالي للصناعات الإلكترونية",
                "All rights reserved © {year} Silicon Valley for Electronic Industries",
                "text", "حقوق النشر", "Copyright", true, 2);
            yield return S("footer.newsletter_title", "footer",
                "اشترك في نشرتنا البريدية",
                "Subscribe to our newsletter", "text", "عنوان النشرة", "Newsletter title", true, 3);
            yield return S("footer.newsletter_text", "footer",
                "كن أول من يعرف بأخبارنا ومنتجاتنا الجديدة.",
                "Be the first to hear about our news and new products.", "textarea", "نص النشرة", "Newsletter text", true, 4);
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task TranslationsAsync(AppDbContext db)
        {
            var existing = await db.Translations.Select(t => t.Key).ToListAsync();
            var rows = UiStrings().Where(t => !existing.Contains(t.Key)).ToList();
            if (rows.Count == 0) return;
            db.Translations.AddRange(rows);
            await db.SaveChangesAsync();
        }

        private static IEnumerable<Translation> UiStrings()
        {
            Translation T(string key, string ar, string en, string group = "common")
                => new() { Key = key, Group = group, ValueAr = ar, ValueEn = en };

            // nav
            yield return T("nav.home", "الرئيسية", "Home", "nav");
            yield return T("nav.about", "من نحن", "About", "nav");
            yield return T("nav.services", "خدماتنا", "Services", "nav");
            yield return T("nav.products", "المنتجات", "Products", "nav");
            yield return T("nav.production_lines", "خطوط الإنتاج", "Production Lines", "nav");
            yield return T("nav.brands", "علاماتنا", "Brands", "nav");
            yield return T("nav.news", "الأخبار", "News", "nav");
            yield return T("nav.events", "الفعاليات", "Events", "nav");
            yield return T("nav.careers", "الوظائف", "Careers", "nav");
            yield return T("nav.gallery", "معرض الصور", "Gallery", "nav");
            yield return T("nav.contact", "اتصل بنا", "Contact", "nav");
            yield return T("nav.quote", "اطلب عرض سعر", "Request a Quote", "nav");

            // buttons
            yield return T("btn.read_more", "اقرأ المزيد", "Read more", "btn");
            yield return T("btn.view_all", "عرض الكل", "View all", "btn");
            yield return T("btn.learn_more", "اعرف المزيد", "Learn more", "btn");
            yield return T("btn.send", "إرسال", "Send", "btn");
            yield return T("btn.submit", "إرسال الطلب", "Submit", "btn");
            yield return T("btn.apply_now", "قدّم الآن", "Apply now", "btn");
            yield return T("btn.register", "سجّل الآن", "Register", "btn");
            yield return T("btn.subscribe", "اشترك", "Subscribe", "btn");
            yield return T("btn.download", "تحميل", "Download", "btn");
            yield return T("btn.back", "رجوع", "Back", "btn");
            yield return T("btn.shop_now", "تسوق الآن", "Shop now", "btn");
            yield return T("btn.get_directions", "الاتجاهات", "Get directions", "btn");

            // labels
            yield return T("label.name", "الاسم", "Full name", "form");
            yield return T("label.email", "البريد الإلكتروني", "Email", "form");
            yield return T("label.phone", "رقم الهاتف", "Phone", "form");
            yield return T("label.company", "الشركة", "Company", "form");
            yield return T("label.subject", "الموضوع", "Subject", "form");
            yield return T("label.message", "الرسالة", "Message", "form");
            yield return T("label.city", "المدينة", "City", "form");
            yield return T("label.country", "الدولة", "Country", "form");
            yield return T("label.cv", "السيرة الذاتية", "CV / Résumé", "form");
            yield return T("label.cover_letter", "خطاب التقديم", "Cover letter", "form");
            yield return T("label.quantity", "الكمية", "Quantity", "form");
            yield return T("label.details", "التفاصيل", "Details", "form");
            yield return T("label.experience_years", "سنوات الخبرة", "Years of experience", "form");
            yield return T("label.optional", "اختياري", "optional", "form");

            // messages
            yield return T("msg.form_success", "تم استلام رسالتك بنجاح، سنتواصل معك قريباً.",
                "Your message has been received. We'll be in touch shortly.", "msg");
            yield return T("msg.form_error", "حدث خطأ، برجاء المحاولة مرة أخرى.",
                "Something went wrong. Please try again.", "msg");
            yield return T("msg.subscribed", "تم الاشتراك بنجاح!", "Subscribed successfully!", "msg");
            yield return T("msg.already_subscribed", "هذا البريد مشترك بالفعل.", "This email is already subscribed.", "msg");
            yield return T("msg.applied", "تم إرسال طلبك بنجاح.", "Your application has been submitted.", "msg");
            yield return T("msg.registered", "تم تسجيلك في الفعالية.", "You are registered for this event.", "msg");
            yield return T("msg.no_results", "لا توجد نتائج.", "No results found.", "msg");
            yield return T("msg.coming_soon", "قريباً", "Coming soon", "msg");

            // misc
            yield return T("word.published_on", "نُشر في", "Published on", "word");
            yield return T("word.share", "مشاركة", "Share", "word");
            yield return T("word.related", "ذات صلة", "Related", "word");
            yield return T("word.category", "التصنيف", "Category", "word");
            yield return T("word.location", "الموقع", "Location", "word");
            yield return T("word.date", "التاريخ", "Date", "word");
            yield return T("word.deadline", "آخر موعد", "Deadline", "word");
            yield return T("word.vacancies", "عدد الشواغر", "Vacancies", "word");
            yield return T("word.job_type", "نوع الوظيفة", "Job type", "word");
            yield return T("word.upcoming", "قادمة", "Upcoming", "word");
            yield return T("word.ongoing", "جارية", "Ongoing", "word");
            yield return T("word.past", "منتهية", "Past", "word");
            yield return T("word.urgent", "عاجل", "Urgent", "word");
            yield return T("word.units_per_day", "وحدة / يوم", "units / day", "word");
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task SocialAsync(AppDbContext db)
        {
            if (await db.SocialLinks.AnyAsync()) return;

            db.SocialLinks.AddRange(
                new SocialLink { Platform = "facebook", Url = "https://www.facebook.com/", Icon = "fa-brands fa-facebook-f", LabelAr = "فيسبوك", LabelEn = "Facebook", Color = "#1877F2", SortOrder = 1, IsActive = false },
                new SocialLink { Platform = "linkedin", Url = "https://www.linkedin.com/", Icon = "fa-brands fa-linkedin-in", LabelAr = "لينكدإن", LabelEn = "LinkedIn", Color = "#0A66C2", SortOrder = 2, IsActive = false },
                new SocialLink { Platform = "instagram", Url = "https://www.instagram.com/", Icon = "fa-brands fa-instagram", LabelAr = "إنستجرام", LabelEn = "Instagram", Color = "#E4405F", SortOrder = 3, IsActive = false },
                new SocialLink { Platform = "youtube", Url = "https://www.youtube.com/watch?v=VngRX0yj_iE", Icon = "fa-brands fa-youtube", LabelAr = "يوتيوب", LabelEn = "YouTube", Color = "#FF0000", SortOrder = 4, IsActive = true },
                new SocialLink { Platform = "whatsapp", Url = "https://wa.me/201098887606", Icon = "fa-brands fa-whatsapp", LabelAr = "واتساب", LabelEn = "WhatsApp", Color = "#25D366", SortOrder = 5, IsActive = true, ShowInHeader = true }
            );
            await db.SaveChangesAsync();
        }

        private static async Task LocationsAsync(AppDbContext db)
        {
            if (await db.SiteLocations.AnyAsync()) return;

            db.SiteLocations.Add(new SiteLocation
            {
                NameAr = "المصنع الرئيسي – العين السخنة",
                NameEn = "Main Factory – Ain Sokhna",
                AddressAr = "مصر، السويس – العين السخنة، تيدا – المنطقة الاقتصادية لقناة السويس",
                AddressEn = "EGYPT, Suez – Ain Sokhna, TEDA – Suez Canal Economic Zone",
                Phone = "(+20)1098887606",
                Email = "info@svei.tech",
                WorkingHoursAr = "السبت – الخميس: 8:00 ص – 5:00 م",
                WorkingHoursEn = "Sat – Thu: 8:00 AM – 5:00 PM",
                Latitude = 29.6725549,
                Longitude = 32.30928,
                MapZoom = 13,
                LocationType = "factory",
                IsPrimary = true,
                SortOrder = 1
            });
            await db.SaveChangesAsync();
        }
    }
}
