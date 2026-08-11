using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>
    /// Every visible heading / paragraph / button label on the public site.
    /// Sourced from svei.tech and rewritten bilingually (typos fixed).
    /// </summary>
    public static class SeedContent
    {
        public static async Task RunAsync(AppDbContext db)
        {
            await HeroAsync(db);
            await SectionsAsync(db);
            await StatsAsync(db);
            await CardsAsync(db);
            await FaqAsync(db);
            await MilestonesAsync(db);
            await StaticPagesAsync(db);
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task HeroAsync(AppDbContext db)
        {
            if (await db.HeroSlides.AnyAsync()) return;

            db.HeroSlides.AddRange(
                new HeroSlide
                {
                    EyebrowAr = "سيليكون فالي للصناعات الإلكترونية",
                    EyebrowEn = "Silicon Valley for Electronic Industries",
                    TitleAr = "نصنع تكنولوجيا الغد في مصر",
                    TitleEn = "Empowering Tomorrow's Technology, Made in Egypt",
                    SubtitleAr = "مصنع متكامل داخل المنطقة الاقتصادية لقناة السويس، يجمع بين خطوط إنتاج عالمية وكوادر مصرية مدرَّبة لتقديم إلكترونيات بمعايير دولية.",
                    SubtitleEn = "An integrated facility inside the Suez Canal Economic Zone, combining world-class production lines with skilled Egyptian talent to deliver electronics to international standards.",
                    ImagePath = "/img/hero/hero-factory.webp",
                    ButtonTextAr = "اكتشف خطوط الإنتاج",
                    ButtonTextEn = "Explore our production lines",
                    ButtonUrl = "/production-lines",
                    Button2TextAr = "تواصل معنا",
                    Button2TextEn = "Contact us",
                    Button2Url = "/contact",
                    Align = "center",
                    OverlayOpacity = 60,
                    SortOrder = 1
                },
                new HeroSlide
                {
                    EyebrowAr = "تصنيع دقيق",
                    EyebrowEn = "Precision Manufacturing",
                    TitleAr = "من التصميم إلى المنتج النهائي",
                    TitleEn = "From design to finished product",
                    SubtitleAr = "خدمات OEM و ODM متكاملة تشمل التجميع السطحي SMT، الاختبار، التغليف والشحن — تحت سقف واحد.",
                    SubtitleEn = "End-to-end OEM & ODM services covering SMT assembly, testing, packaging and shipping — all under one roof.",
                    ImagePath = "/img/hero/hero-chip.webp",
                    ButtonTextAr = "خدماتنا",
                    ButtonTextEn = "Our services",
                    ButtonUrl = "/services",
                    Button2TextAr = "اطلب عرض سعر",
                    Button2TextEn = "Request a quote",
                    Button2Url = "/quote",
                    Align = "center",
                    OverlayOpacity = 55,
                    SortOrder = 2
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task SectionsAsync(AppDbContext db)
        {
            var existing = await db.PageSections
                .Select(s => s.PageKey + "|" + s.SectionKey).ToListAsync();

            var rows = Sections()
                .Where(s => !existing.Contains(s.PageKey + "|" + s.SectionKey))
                .ToList();

            if (rows.Count == 0) return;
            db.PageSections.AddRange(rows);
            await db.SaveChangesAsync();
        }

        private static IEnumerable<PageSection> Sections()
        {
            // ── HOME ──────────────────────────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "home", SectionKey = "intro", SortOrder = 1,
                EyebrowAr = "من نحن", EyebrowEn = "Who we are",
                TitleAr = "شريكك في التصنيع الإلكتروني داخل مصر",
                TitleEn = "Your electronics manufacturing partner in Egypt",
                SubtitleAr = "تأسست عام 2023 داخل المنطقة الاقتصادية لقناة السويس",
                SubtitleEn = "Founded in 2023 inside the Suez Canal Economic Zone",
                BodyAr = "نمتلك خطوط إنتاج حديثة لتصنيع الهواتف المحمولة، أجهزة تخزين الطاقة، الأجهزة القابلة للارتداء وملحقات الصوت. نعمل وفق أنظمة جودة صارمة وفريق هندسي مؤهل لخدمة السوق المحلي والتصدير.",
                BodyEn = "We operate modern production lines for mobile phones, power storage devices, wearables and audio peripherals. Strict quality systems and a qualified engineering team serve both the local market and export.",
                ButtonTextAr = "اعرف المزيد عنا", ButtonTextEn = "More about us", ButtonUrl = "/about",
                ImagePath = "/img/hero/hero-factory.webp"
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "production_lines", SortOrder = 2,
                EyebrowAr = "قدراتنا الإنتاجية", EyebrowEn = "Our capabilities",
                TitleAr = "خطوط الإنتاج", TitleEn = "Production Lines",
                SubtitleAr = "أربعة خطوط متخصصة تغطي أهم فئات الإلكترونيات الاستهلاكية.",
                SubtitleEn = "Four specialised lines covering the key consumer electronics categories.",
                ButtonTextAr = "كل خطوط الإنتاج", ButtonTextEn = "All production lines", ButtonUrl = "/production-lines"
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "features", SortOrder = 3,
                EyebrowAr = "لماذا SVEI", EyebrowEn = "Why SVEI",
                TitleAr = "لماذا تختار سيليكون فالي؟", TitleEn = "Why choose Silicon Valley?",
                SubtitleAr = "موقع استراتيجي، تكنولوجيا حديثة، وكوادر مصرية مدرَّبة.",
                SubtitleEn = "Strategic location, modern technology, and skilled Egyptian talent."
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "stats", SortOrder = 4,
                TitleAr = "أرقام تتحدث عنا", TitleEn = "Numbers that speak for us",
                SubtitleAr = "طاقة إنتاجية حقيقية داخل المنطقة الاقتصادية لقناة السويس.",
                SubtitleEn = "Real production capacity inside the Suez Canal Economic Zone."
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "video", SortOrder = 5,
                EyebrowAr = "جولة داخل المصنع", EyebrowEn = "Inside the factory",
                TitleAr = "شاهد كيف نصنع", TitleEn = "See how we build",
                SubtitleAr = "جولة مصورة داخل خطوط الإنتاج ومعامل الاختبار.",
                SubtitleEn = "A guided tour through our production lines and test labs.",
                VideoUrl = "https://www.youtube.com/watch?v=VngRX0yj_iE",
                ImagePath = "/img/lines/phones.webp"
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "brands", SortOrder = 6,
                EyebrowAr = "شركاء النجاح", EyebrowEn = "Trusted by",
                TitleAr = "علامات تجارية نصنع لها", TitleEn = "Brands we manufacture for",
                SubtitleAr = "نتشرف بثقة كبرى العلامات في السوق المصري والإقليمي.",
                SubtitleEn = "Trusted by leading brands across the Egyptian and regional market."
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "news", SortOrder = 7,
                EyebrowAr = "آخر المستجدات", EyebrowEn = "Latest updates",
                TitleAr = "الأخبار", TitleEn = "News",
                SubtitleAr = "تابع آخر أخبار المصنع وإنجازاتنا.",
                SubtitleEn = "Follow the factory's latest news and milestones.",
                ButtonTextAr = "كل الأخبار", ButtonTextEn = "All news", ButtonUrl = "/news"
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "events", SortOrder = 8,
                EyebrowAr = "فعالياتنا", EyebrowEn = "Our events",
                TitleAr = "المعارض والفعاليات", TitleEn = "Exhibitions & Events",
                SubtitleAr = "قابلنا في أهم المعارض الصناعية والتكنولوجية.",
                SubtitleEn = "Meet us at the leading industrial and technology exhibitions.",
                ButtonTextAr = "كل الفعاليات", ButtonTextEn = "All events", ButtonUrl = "/events"
            };

            yield return new PageSection
            {
                PageKey = "home", SectionKey = "cta", SortOrder = 9,
                TitleAr = "جاهز لبدء مشروعك التصنيعي؟",
                TitleEn = "Ready to start your manufacturing project?",
                SubtitleAr = "أرسل لنا متطلباتك وسيتواصل فريقنا الهندسي معك خلال 48 ساعة.",
                SubtitleEn = "Send us your requirements and our engineering team will respond within 48 hours.",
                ButtonTextAr = "اطلب عرض سعر", ButtonTextEn = "Request a quote", ButtonUrl = "/quote",
                Button2TextAr = "اتصل بنا", Button2TextEn = "Contact us", Button2Url = "/contact"
            };

            // ── ABOUT ─────────────────────────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "about", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "من نحن", EyebrowEn = "About us",
                TitleAr = "سيليكون فالي للصناعات الإلكترونية",
                TitleEn = "Silicon Valley for Electronic Industries",
                SubtitleAr = "نبني قاعدة صناعية إلكترونية مصرية بمعايير عالمية.",
                SubtitleEn = "Building an Egyptian electronics industrial base to global standards.",
                ImagePath = "/img/about/exterior.webp"
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "story", SortOrder = 2,
                EyebrowAr = "قصتنا", EyebrowEn = "Our story",
                TitleAr = "بدأنا في 2023 برؤية واضحة", TitleEn = "We started in 2023 with a clear vision",
                BodyAr = "انطلق مصنع سيليكون فالي للصناعات الإلكترونية عام 2023 داخل منطقة تيدا بالعين السخنة، ضمن المنطقة الاقتصادية لقناة السويس. اخترنا هذا الموقع لقربه من ميناء السخنة وسهولة الوصول للأسواق الإقليمية.\n\nمنذ اليوم الأول والهدف واحد: توطين صناعة الإلكترونيات في مصر، ونقل المعرفة التقنية لكوادر مصرية قادرة على المنافسة عالمياً.",
                BodyEn = "Silicon Valley for Electronic Industries launched in 2023 within the TEDA zone in Ain Sokhna, inside the Suez Canal Economic Zone. The location was chosen for its proximity to Sokhna Port and easy access to regional markets.\n\nThe goal has been the same since day one: localise electronics manufacturing in Egypt and transfer technical know-how to Egyptian talent able to compete globally.",
                ImagePath = "/img/careers/team.webp"
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "mission", SortOrder = 3,
                TitleAr = "رسالتنا", TitleEn = "Our Mission",
                BodyAr = "تقديم حلول تصنيع إلكتروني متكاملة وموثوقة، بجودة عالمية وتكلفة تنافسية، مع الالتزام الكامل بمواعيد التسليم ومعايير السلامة.",
                BodyEn = "To deliver integrated, reliable electronics manufacturing solutions with world-class quality and competitive cost, with full commitment to delivery schedules and safety standards.",
                Icon = "fa-solid fa-bullseye"
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "vision", SortOrder = 4,
                TitleAr = "رؤيتنا", TitleEn = "Our Vision",
                BodyAr = "أن نكون المركز الصناعي الإلكتروني الأول في الشرق الأوسط وأفريقيا، ونقطة انطلاق للمنتج المصري نحو العالم.",
                BodyEn = "To be the leading electronics manufacturing hub in the Middle East and Africa, and the launchpad for Egyptian products to the world.",
                Icon = "fa-solid fa-eye"
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "values", SortOrder = 5,
                EyebrowAr = "مبادئنا", EyebrowEn = "Our principles",
                TitleAr = "قيمنا", TitleEn = "Our Values",
                SubtitleAr = "المبادئ التي توجّه كل قرار داخل المصنع.",
                SubtitleEn = "The principles guiding every decision inside the factory."
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "timeline", SortOrder = 6,
                EyebrowAr = "مسيرتنا", EyebrowEn = "Our journey",
                TitleAr = "محطات فارقة", TitleEn = "Key milestones"
            };

            yield return new PageSection
            {
                PageKey = "about", SectionKey = "team", SortOrder = 7,
                EyebrowAr = "فريق العمل", EyebrowEn = "Our people",
                TitleAr = "القيادة", TitleEn = "Leadership",
                SubtitleAr = "خبرات صناعية وهندسية تقود المصنع.",
                SubtitleEn = "Industrial and engineering expertise leading the factory."
            };

            // ── SERVICES ──────────────────────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "services", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "ماذا نقدم", EyebrowEn = "What we offer",
                TitleAr = "خدماتنا", TitleEn = "Our Services",
                SubtitleAr = "حلول تصنيع متكاملة من الفكرة حتى التسليم.",
                SubtitleEn = "Integrated manufacturing solutions from concept to delivery."
            };

            yield return new PageSection
            {
                PageKey = "services", SectionKey = "process", SortOrder = 2,
                EyebrowAr = "كيف نعمل", EyebrowEn = "How we work",
                TitleAr = "مراحل العمل", TitleEn = "Our Process",
                SubtitleAr = "منهجية واضحة تضمن الجودة في كل خطوة.",
                SubtitleEn = "A clear methodology that guarantees quality at every step."
            };

            // ── PRODUCTS ──────────────────────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "products", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "صنع في مصر", EyebrowEn = "Made in Egypt",
                TitleAr = "منتجاتنا", TitleEn = "Our Products",
                SubtitleAr = "أجهزة إلكترونية بمعايير جودة عالمية.",
                SubtitleEn = "Electronic devices built to international quality standards.",
                ButtonTextAr = "تسوق الآن", ButtonTextEn = "Shop now",
                ButtonUrl = "https://yallatager.com/ar/Silicon-Valley-Product.html"
            };

            // ── PRODUCTION LINES ──────────────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "production-lines", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "قدراتنا", EyebrowEn = "Our capabilities",
                TitleAr = "خطوط الإنتاج", TitleEn = "Production Lines",
                SubtitleAr = "طاقة إنتاجية تتجاوز 4500 وحدة يومياً عبر أربعة خطوط متخصصة.",
                SubtitleEn = "Over 4,500 units per day across four specialised lines."
            };

            // ── NEWS / EVENTS / CAREERS / GALLERY ─────────────────────────────
            yield return new PageSection
            {
                PageKey = "news", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "المركز الإعلامي", EyebrowEn = "Newsroom",
                TitleAr = "الأخبار", TitleEn = "News",
                SubtitleAr = "آخر أخبار المصنع وإنجازاتنا وشراكاتنا.",
                SubtitleEn = "The factory's latest news, achievements and partnerships."
            };

            yield return new PageSection
            {
                PageKey = "events", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "قابلنا", EyebrowEn = "Meet us",
                TitleAr = "المعارض والفعاليات", TitleEn = "Exhibitions & Events",
                SubtitleAr = "معارض، مؤتمرات، وأيام تدريبية ننظمها أو نشارك فيها.",
                SubtitleEn = "Exhibitions, conferences and training days we host or take part in."
            };

            yield return new PageSection
            {
                PageKey = "careers", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "انضم إلينا", EyebrowEn = "Join us",
                TitleAr = "الوظائف المتاحة", TitleEn = "Careers",
                SubtitleAr = "ابنِ مستقبلك المهني في قلب الصناعة الإلكترونية المصرية.",
                SubtitleEn = "Build your career at the heart of Egypt's electronics industry."
            };

            yield return new PageSection
            {
                PageKey = "careers", SectionKey = "why_us", SortOrder = 2,
                EyebrowAr = "بيئة العمل", EyebrowEn = "Life at SVEI",
                TitleAr = "لماذا تعمل معنا؟", TitleEn = "Why work with us?",
                SubtitleAr = "تدريب مستمر، مسار وظيفي واضح، وبيئة عمل آمنة.",
                SubtitleEn = "Continuous training, a clear career path, and a safe workplace."
            };

            yield return new PageSection
            {
                PageKey = "gallery", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "بالصور", EyebrowEn = "In pictures",
                TitleAr = "معرض الصور", TitleEn = "Gallery",
                SubtitleAr = "جولة بصرية داخل المصنع وخطوط الإنتاج.",
                SubtitleEn = "A visual tour inside the factory and production lines."
            };

            // ── CONTACT / QUOTE / BRANDS ──────────────────────────────────────
            yield return new PageSection
            {
                PageKey = "contact", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "نحن هنا", EyebrowEn = "We're here",
                TitleAr = "اتصل بنا", TitleEn = "Contact Us",
                SubtitleAr = "فريقنا جاهز للرد على استفساراتك على مدار الساعة.",
                SubtitleEn = "Our team is ready to answer your enquiries around the clock."
            };

            yield return new PageSection
            {
                PageKey = "contact", SectionKey = "form", SortOrder = 2,
                TitleAr = "أرسل لنا رسالة", TitleEn = "Send us a message",
                SubtitleAr = "املأ النموذج وسنعاود الاتصال بك في أقرب وقت.",
                SubtitleEn = "Fill in the form and we'll get back to you shortly.",
                ButtonTextAr = "إرسال الرسالة", ButtonTextEn = "Send message"
            };

            yield return new PageSection
            {
                PageKey = "contact", SectionKey = "map", SortOrder = 3,
                TitleAr = "موقعنا على الخريطة", TitleEn = "Find us on the map",
                SubtitleAr = "تيدا – العين السخنة، المنطقة الاقتصادية لقناة السويس.",
                SubtitleEn = "TEDA – Ain Sokhna, Suez Canal Economic Zone."
            };

            yield return new PageSection
            {
                PageKey = "quote", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "ابدأ مشروعك", EyebrowEn = "Start your project",
                TitleAr = "اطلب عرض سعر", TitleEn = "Request a Quote",
                SubtitleAr = "شاركنا تفاصيل مشروعك وسنعود إليك بعرض مفصل خلال 48 ساعة.",
                SubtitleEn = "Share your project details and we'll return a detailed quote within 48 hours."
            };

            yield return new PageSection
            {
                PageKey = "brands", SectionKey = "hero", SortOrder = 1,
                EyebrowAr = "شركاؤنا", EyebrowEn = "Our partners",
                TitleAr = "العلامات التجارية", TitleEn = "Brands",
                SubtitleAr = "علامات عالمية ومحلية تثق في خطوط إنتاجنا.",
                SubtitleEn = "Global and local brands that trust our production lines."
            };
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task StatsAsync(AppDbContext db)
        {
            if (await db.StatCounters.AnyAsync()) return;

            db.StatCounters.AddRange(
                new StatCounter { LabelAr = "هاتف محمول يومياً", LabelEn = "Mobile phones per day", Value = 2000, Suffix = "+", Icon = "fa-solid fa-mobile-screen", GroupKey = "home", SortOrder = 1 },
                new StatCounter { LabelAr = "جهاز تخزين طاقة يومياً", LabelEn = "Power storage units per day", Value = 1000, Suffix = "+", Icon = "fa-solid fa-battery-full", GroupKey = "home", SortOrder = 2 },
                new StatCounter { LabelAr = "جهاز قابل للارتداء يومياً", LabelEn = "Wearables per day", Value = 1500, Suffix = "+", Icon = "fa-solid fa-clock", GroupKey = "home", SortOrder = 3 },
                new StatCounter { LabelAr = "خطوط إنتاج متخصصة", LabelEn = "Specialised production lines", Value = 4, Icon = "fa-solid fa-industry", GroupKey = "home", SortOrder = 4 },
                new StatCounter { LabelAr = "علامات تجارية شريكة", LabelEn = "Partner brands", Value = 5, Suffix = "+", Icon = "fa-solid fa-handshake", GroupKey = "home", SortOrder = 5 },
                new StatCounter { LabelAr = "سنة التأسيس", LabelEn = "Established", Value = 2023, Icon = "fa-solid fa-calendar", GroupKey = "about", SortOrder = 1 }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task CardsAsync(AppDbContext db)
        {
            if (await db.InfoCards.AnyAsync()) return;

            db.InfoCards.AddRange(
                // home.features — why SVEI
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 1, Icon = "fa-solid fa-location-dot",
                    TitleAr = "موقع استراتيجي", TitleEn = "Strategic location",
                    TextAr = "داخل المنطقة الاقتصادية لقناة السويس وعلى بُعد دقائق من ميناء السخنة، ما يختصر زمن وتكلفة الشحن للأسواق الإقليمية.",
                    TextEn = "Inside the Suez Canal Economic Zone, minutes from Sokhna Port — cutting shipping time and cost to regional markets."
                },
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 2, Icon = "fa-solid fa-microchip",
                    TitleAr = "تكنولوجيا حديثة", TitleEn = "Modern technology",
                    TextAr = "خطوط تجميع سطحي SMT وأجهزة اختبار آلية تضمن دقة التصنيع وثبات الجودة على مدار الإنتاج.",
                    TextEn = "SMT assembly lines and automated test equipment ensure manufacturing precision and consistent quality."
                },
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 3, Icon = "fa-solid fa-users-gear",
                    TitleAr = "كوادر مصرية مدرَّبة", TitleEn = "Skilled Egyptian talent",
                    TextAr = "فريق هندسي وفني يخضع لبرامج تدريب مستمرة على أحدث تقنيات التصنيع الإلكتروني.",
                    TextEn = "Engineering and technical teams enrolled in continuous training on the latest electronics manufacturing techniques."
                },
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 4, Icon = "fa-solid fa-shield-halved",
                    TitleAr = "جودة مضمونة", TitleEn = "Assured quality",
                    TextAr = "نظام مراقبة جودة متعدد المراحل من استلام المكونات حتى التغليف النهائي.",
                    TextEn = "Multi-stage quality control from incoming components through to final packaging."
                },
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 5, Icon = "fa-solid fa-truck-fast",
                    TitleAr = "التزام بالمواعيد", TitleEn = "On-time delivery",
                    TextAr = "تخطيط إنتاج دقيق وسلسلة إمداد مرنة تضمن تسليم الطلبات في مواعيدها.",
                    TextEn = "Precise production planning and a flexible supply chain keep orders on schedule."
                },
                new InfoCard
                {
                    GroupKey = "home.features", SortOrder = 6, Icon = "fa-solid fa-sliders",
                    TitleAr = "مرونة في التخصيص", TitleEn = "Customisation flexibility",
                    TextAr = "خدمات OEM و ODM تتيح تصنيع منتجك بمواصفاتك وهويتك التجارية.",
                    TextEn = "OEM and ODM services let you manufacture to your own specification and brand identity."
                },

                // about.values
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 1, Icon = "fa-solid fa-gem",
                    TitleAr = "الجودة أولاً", TitleEn = "Quality first",
                    TextAr = "لا تنازل عن معايير الجودة مهما كانت ضغوط الإنتاج.",
                    TextEn = "No compromise on quality standards, whatever the production pressure."
                },
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 2, Icon = "fa-solid fa-lightbulb",
                    TitleAr = "الابتكار", TitleEn = "Innovation",
                    TextAr = "نستثمر في التقنيات الجديدة ونشجّع فرقنا على تطوير حلول أفضل.",
                    TextEn = "We invest in new technologies and encourage our teams to build better solutions."
                },
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 3, Icon = "fa-solid fa-handshake-angle",
                    TitleAr = "الشراكة", TitleEn = "Partnership",
                    TextAr = "ننظر لعملائنا كشركاء طويلي الأمد لا كصفقات عابرة.",
                    TextEn = "We treat clients as long-term partners, not one-off transactions."
                },
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 4, Icon = "fa-solid fa-leaf",
                    TitleAr = "الاستدامة", TitleEn = "Sustainability",
                    TextAr = "ترشيد استهلاك الطاقة وإدارة مسؤولة للمخلفات الصناعية.",
                    TextEn = "Energy-conscious operations and responsible industrial waste management."
                },
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 5, Icon = "fa-solid fa-user-shield",
                    TitleAr = "السلامة", TitleEn = "Safety",
                    TextAr = "بيئة عمل آمنة تلتزم بمعايير السلامة المهنية.",
                    TextEn = "A safe workplace committed to occupational health and safety standards."
                },
                new InfoCard
                {
                    GroupKey = "about.values", SortOrder = 6, Icon = "fa-solid fa-flag",
                    TitleAr = "الانتماء", TitleEn = "National impact",
                    TextAr = "فخورون بالمساهمة في توطين الصناعة الإلكترونية المصرية.",
                    TextEn = "Proud to contribute to localising Egypt's electronics industry."
                },

                // services.process
                new InfoCard
                {
                    GroupKey = "services.process", SortOrder = 1, Icon = "fa-solid fa-comments",
                    TitleAr = "1. الاستشارة", TitleEn = "1. Consultation",
                    TextAr = "نستمع لمتطلباتك ونحلل جدوى التصنيع فنياً واقتصادياً.",
                    TextEn = "We listen to your requirements and assess technical and commercial feasibility."
                },
                new InfoCard
                {
                    GroupKey = "services.process", SortOrder = 2, Icon = "fa-solid fa-pen-ruler",
                    TitleAr = "2. التصميم والعينة", TitleEn = "2. Design & prototype",
                    TextAr = "نطوّر التصميم وننتج عينة أولية للمراجعة والاعتماد.",
                    TextEn = "We develop the design and produce a prototype for review and approval."
                },
                new InfoCard
                {
                    GroupKey = "services.process", SortOrder = 3, Icon = "fa-solid fa-industry",
                    TitleAr = "3. الإنتاج", TitleEn = "3. Production",
                    TextAr = "تشغيل خط الإنتاج مع مراقبة جودة لحظية في كل مرحلة.",
                    TextEn = "The line runs with real-time quality control at every stage."
                },
                new InfoCard
                {
                    GroupKey = "services.process", SortOrder = 4, Icon = "fa-solid fa-box-open",
                    TitleAr = "4. الاختبار والتغليف", TitleEn = "4. Testing & packaging",
                    TextAr = "اختبارات وظيفية شاملة ثم تغليف بهويتك التجارية.",
                    TextEn = "Full functional testing, then packaging in your brand identity."
                },
                new InfoCard
                {
                    GroupKey = "services.process", SortOrder = 5, Icon = "fa-solid fa-truck",
                    TitleAr = "5. التسليم", TitleEn = "5. Delivery",
                    TextAr = "شحن من ميناء السخنة أو تسليم محلي حسب الاتفاق.",
                    TextEn = "Shipping from Sokhna Port or local delivery, as agreed."
                },

                // careers.why_us
                new InfoCard
                {
                    GroupKey = "careers.why_us", SortOrder = 1, Icon = "fa-solid fa-graduation-cap",
                    TitleAr = "تدريب مستمر", TitleEn = "Continuous training",
                    TextAr = "برامج تدريب فنية وإدارية على مدار العام.",
                    TextEn = "Technical and management training programmes year-round."
                },
                new InfoCard
                {
                    GroupKey = "careers.why_us", SortOrder = 2, Icon = "fa-solid fa-chart-line",
                    TitleAr = "مسار وظيفي واضح", TitleEn = "Clear career path",
                    TextAr = "فرص ترقّي محددة مبنية على الأداء والكفاءة.",
                    TextEn = "Defined progression opportunities based on performance and competence."
                },
                new InfoCard
                {
                    GroupKey = "careers.why_us", SortOrder = 3, Icon = "fa-solid fa-heart-pulse",
                    TitleAr = "تأمين ورعاية", TitleEn = "Insurance & care",
                    TextAr = "تأمين صحي واجتماعي شامل لجميع العاملين.",
                    TextEn = "Comprehensive health and social insurance for all staff."
                },
                new InfoCard
                {
                    GroupKey = "careers.why_us", SortOrder = 4, Icon = "fa-solid fa-bus",
                    TitleAr = "انتقالات ووجبات", TitleEn = "Transport & meals",
                    TextAr = "خطوط انتقالات يومية ووجبة ساخنة داخل المصنع.",
                    TextEn = "Daily transport routes and a hot meal on site."
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task FaqAsync(AppDbContext db)
        {
            if (await db.FaqItems.AnyAsync()) return;

            db.FaqItems.AddRange(
                new FaqItem
                {
                    GroupKey = "general", SortOrder = 1,
                    QuestionAr = "ما هو الحد الأدنى لكمية الطلب (MOQ)؟",
                    QuestionEn = "What is the minimum order quantity (MOQ)?",
                    AnswerAr = "يختلف الحد الأدنى حسب نوع المنتج وخط الإنتاج. تواصل معنا بتفاصيل مشروعك وسنحدد لك الحد الأدنى المناسب.",
                    AnswerEn = "The MOQ varies by product type and production line. Contact us with your project details and we'll confirm the applicable minimum."
                },
                new FaqItem
                {
                    GroupKey = "general", SortOrder = 2,
                    QuestionAr = "هل تقدمون خدمات OEM و ODM؟",
                    QuestionEn = "Do you offer OEM and ODM services?",
                    AnswerAr = "نعم. نصنع بمواصفاتك وهويتك التجارية (OEM)، كما نطوّر تصميمات كاملة نيابةً عنك (ODM).",
                    AnswerEn = "Yes. We manufacture to your specification and brand (OEM), and we can develop complete designs on your behalf (ODM)."
                },
                new FaqItem
                {
                    GroupKey = "general", SortOrder = 3,
                    QuestionAr = "كم يستغرق تنفيذ الطلب؟",
                    QuestionEn = "What is the typical lead time?",
                    AnswerAr = "يعتمد على الكمية وتوافر المكونات. غالباً بين 3 و 8 أسابيع من اعتماد العينة.",
                    AnswerEn = "It depends on volume and component availability — typically 3 to 8 weeks from prototype approval."
                },
                new FaqItem
                {
                    GroupKey = "general", SortOrder = 4,
                    QuestionAr = "هل يمكن زيارة المصنع؟",
                    QuestionEn = "Can we visit the factory?",
                    AnswerAr = "بالتأكيد. نرحّب بزيارات العملاء بموعد مسبق. تواصل معنا لترتيب الزيارة.",
                    AnswerEn = "Absolutely. We welcome client visits by prior appointment — contact us to arrange one."
                },
                new FaqItem
                {
                    GroupKey = "general", SortOrder = 5,
                    QuestionAr = "هل تصدّرون خارج مصر؟",
                    QuestionEn = "Do you export outside Egypt?",
                    AnswerAr = "نعم، موقعنا داخل المنطقة الاقتصادية لقناة السويس يسهّل التصدير للأسواق العربية والأفريقية.",
                    AnswerEn = "Yes — our location inside the Suez Canal Economic Zone facilitates export to Arab and African markets."
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task MilestonesAsync(AppDbContext db)
        {
            if (await db.Milestones.AnyAsync()) return;

            db.Milestones.AddRange(
                new Milestone
                {
                    Year = "2023", SortOrder = 1, Icon = "fa-solid fa-flag",
                    TitleAr = "تأسيس الشركة", TitleEn = "Company founded",
                    TextAr = "تأسيس سيليكون فالي للصناعات الإلكترونية داخل منطقة تيدا بالعين السخنة.",
                    TextEn = "Silicon Valley for Electronic Industries is founded in the TEDA zone, Ain Sokhna."
                },
                new Milestone
                {
                    Year = "2023", SortOrder = 2, Icon = "fa-solid fa-mobile-screen",
                    TitleAr = "تشغيل خط الهواتف المحمولة", TitleEn = "Mobile phone line goes live",
                    TextAr = "بدء الإنتاج التجاري بطاقة 2000 هاتف يومياً.",
                    TextEn = "Commercial production begins at 2,000 handsets per day."
                },
                new Milestone
                {
                    Year = "2024", SortOrder = 3, Icon = "fa-solid fa-battery-full",
                    TitleAr = "التوسع في تخزين الطاقة", TitleEn = "Power storage expansion",
                    TextAr = "إضافة خط إنتاج بنوك الطاقة بطاقة 1000 وحدة يومياً.",
                    TextEn = "A power bank line is added at 1,000 units per day."
                },
                new Milestone
                {
                    Year = "2024", SortOrder = 4, Icon = "fa-solid fa-clock",
                    TitleAr = "الأجهزة القابلة للارتداء", TitleEn = "Wearables line",
                    TextAr = "إطلاق خط الساعات الذكية والأجهزة القابلة للارتداء بطاقة 1500 وحدة يومياً.",
                    TextEn = "Smartwatch and wearables line launches at 1,500 units per day."
                },
                new Milestone
                {
                    Year = "2025", SortOrder = 5, Icon = "fa-solid fa-headphones",
                    TitleAr = "ملحقات الصوت", TitleEn = "Audio peripherals",
                    TextAr = "إضافة خط سماعات الأذن وملحقات الصوت لاستكمال منظومة المنتجات.",
                    TextEn = "Earbuds and audio peripherals line completes the product ecosystem."
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task StaticPagesAsync(AppDbContext db)
        {
            if (await db.StaticPages.AnyAsync()) return;

            db.StaticPages.AddRange(
                new StaticPage
                {
                    Slug = "privacy", SortOrder = 1,
                    TitleAr = "سياسة الخصوصية", TitleEn = "Privacy Policy",
                    BodyAr = "<p>نحترم خصوصيتك ونلتزم بحماية بياناتك الشخصية. توضح هذه السياسة أنواع البيانات التي نجمعها وكيفية استخدامها.</p><h3>البيانات التي نجمعها</h3><p>الاسم، البريد الإلكتروني، رقم الهاتف، واسم الشركة — عند إرسالك لنموذج تواصل أو طلب عرض سعر أو التقديم على وظيفة.</p><h3>كيف نستخدم بياناتك</h3><p>نستخدم البيانات للرد على استفساراتك وتقديم خدماتنا فقط، ولا نبيعها أو نشاركها مع أطراف ثالثة لأغراض تسويقية.</p><h3>حقوقك</h3><p>يمكنك طلب الاطلاع على بياناتك أو تعديلها أو حذفها بمراسلتنا على info@svei.tech.</p>",
                    BodyEn = "<p>We respect your privacy and are committed to protecting your personal data. This policy explains what we collect and how it is used.</p><h3>Data we collect</h3><p>Name, email, phone number and company name — submitted when you use a contact form, request a quote, or apply for a job.</p><h3>How we use it</h3><p>Data is used solely to respond to your enquiries and deliver our services. We do not sell or share it with third parties for marketing.</p><h3>Your rights</h3><p>You may request access to, correction of, or deletion of your data by writing to info@svei.tech.</p>"
                },
                new StaticPage
                {
                    Slug = "terms", SortOrder = 2,
                    TitleAr = "الشروط والأحكام", TitleEn = "Terms & Conditions",
                    BodyAr = "<p>باستخدامك لهذا الموقع فإنك توافق على الشروط التالية.</p><h3>استخدام الموقع</h3><p>محتوى الموقع مُقدَّم لأغراض إعلامية. يُحظر إعادة نشر أو استخدام المحتوى تجارياً دون إذن كتابي مسبق.</p><h3>الملكية الفكرية</h3><p>جميع العلامات التجارية والشعارات والصور المعروضة مملوكة لسيليكون فالي للصناعات الإلكترونية أو لأصحابها المرخّصين.</p><h3>حدود المسؤولية</h3><p>نبذل جهدنا لضمان دقة المعلومات، لكننا لا نضمن خلوّها من الأخطاء ولا نتحمل مسؤولية أي قرار يُتخذ بناءً عليها.</p>",
                    BodyEn = "<p>By using this website you agree to the following terms.</p><h3>Use of the site</h3><p>Content is provided for informational purposes. Republishing or commercial use without prior written permission is prohibited.</p><h3>Intellectual property</h3><p>All trademarks, logos and images shown belong to Silicon Valley for Electronic Industries or their respective licensed owners.</p><h3>Limitation of liability</h3><p>We work to keep information accurate but do not warrant it is error-free, and accept no liability for decisions made in reliance on it.</p>"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
