using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>News, Events and Careers starter content — bilingual.</summary>
    public static class SeedCommunity
    {
        public static async Task RunAsync(AppDbContext db)
        {
            await NewsAsync(db);
            await EventsAsync(db);
            await CareersAsync(db);
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task NewsAsync(AppDbContext db)
        {
            if (!await db.NewsCategories.AnyAsync())
            {
                db.NewsCategories.AddRange(
                    new NewsCategory { Slug = "company-news", NameAr = "أخبار الشركة", NameEn = "Company News", Color = "#E31B23", Icon = "fa-solid fa-building", SortOrder = 1 },
                    new NewsCategory { Slug = "production", NameAr = "الإنتاج والتوسعات", NameEn = "Production & Expansion", Color = "#22C6E8", Icon = "fa-solid fa-industry", SortOrder = 2 },
                    new NewsCategory { Slug = "partnerships", NameAr = "الشراكات", NameEn = "Partnerships", Color = "#FDB714", Icon = "fa-solid fa-handshake", SortOrder = 3 },
                    new NewsCategory { Slug = "technology", NameAr = "تكنولوجيا", NameEn = "Technology", Color = "#0E7C99", Icon = "fa-solid fa-microchip", SortOrder = 4 }
                );
                await db.SaveChangesAsync();
            }

            if (await db.NewsPosts.AnyAsync()) return;

            var company = await db.NewsCategories.FirstOrDefaultAsync(c => c.Slug == "company-news");
            var production = await db.NewsCategories.FirstOrDefaultAsync(c => c.Slug == "production");
            var partnerships = await db.NewsCategories.FirstOrDefaultAsync(c => c.Slug == "partnerships");

            db.NewsPosts.AddRange(
                new NewsPost
                {
                    Slug = "svei-opens-electronics-factory-sokhna",
                    CategoryId = company?.Id,
                    IsFeatured = true,
                    PublishedAt = DateTime.UtcNow.AddDays(-30),
                    AuthorName = "SVEI",
                    CoverImagePath = "/img/hero/hero-factory.webp",
                    Tags = "مصنع,العين السخنة,افتتاح",
                    TitleAr = "افتتاح مصنع سيليكون فالي للصناعات الإلكترونية بالعين السخنة",
                    TitleEn = "SVEI opens its electronics factory in Ain Sokhna",
                    ExcerptAr = "انطلاق الإنتاج التجاري داخل منطقة تيدا بالمنطقة الاقتصادية لقناة السويس بطاقة تتجاوز 4500 وحدة يومياً.",
                    ExcerptEn = "Commercial production begins in the TEDA zone of the Suez Canal Economic Zone, with capacity exceeding 4,500 units per day.",
                    BodyAr = "<p>أعلنت شركة سيليكون فالي للصناعات الإلكترونية عن بدء الإنتاج التجاري في مصنعها الجديد داخل منطقة تيدا بالعين السخنة، ضمن المنطقة الاقتصادية لقناة السويس.</p><p>يضم المصنع أربعة خطوط إنتاج متخصصة تغطي الهواتف المحمولة، أجهزة تخزين الطاقة، الأجهزة القابلة للارتداء وملحقات الصوت، بطاقة إنتاجية إجمالية تتجاوز 4500 وحدة يومياً.</p><p>ويأتي اختيار الموقع لقربه من ميناء السخنة، ما يمنح المصنع ميزة لوجستية في التصدير للأسواق العربية والأفريقية.</p>",
                    BodyEn = "<p>Silicon Valley for Electronic Industries has announced the start of commercial production at its new factory in the TEDA zone of Ain Sokhna, inside the Suez Canal Economic Zone.</p><p>The facility houses four specialised production lines covering mobile phones, power storage devices, wearables and audio peripherals, with a combined capacity of over 4,500 units per day.</p><p>The location was selected for its proximity to Sokhna Port, giving the plant a logistics advantage for exports to Arab and African markets.</p>",
                    MetaTitle = "افتتاح مصنع SVEI بالعين السخنة",
                    MetaDesc = "بدء الإنتاج التجاري لمصنع سيليكون فالي للصناعات الإلكترونية بالمنطقة الاقتصادية لقناة السويس."
                },
                new NewsPost
                {
                    Slug = "wearables-line-launch",
                    CategoryId = production?.Id,
                    IsFeatured = true,
                    PublishedAt = DateTime.UtcNow.AddDays(-18),
                    AuthorName = "SVEI",
                    CoverImagePath = "/img/sections/feature3.webp",
                    Tags = "ساعات ذكية,توسع,خط إنتاج",
                    TitleAr = "إطلاق خط إنتاج الأجهزة القابلة للارتداء بطاقة 1500 وحدة يومياً",
                    TitleEn = "Wearables line launches at 1,500 units per day",
                    ExcerptAr = "خط جديد مخصص للساعات الذكية وأساور اللياقة مزود بمعدات معايرة الحساسات واختبار مقاومة الماء.",
                    ExcerptEn = "A new line dedicated to smartwatches and fitness bands, equipped with sensor calibration and water-resistance testing.",
                    BodyAr = "<p>أضاف المصنع خط إنتاج جديداً متخصصاً في الأجهزة القابلة للارتداء، بطاقة إنتاجية تصل إلى 1500 جهاز يومياً.</p><p>يشمل الخط محطات تجميع الحساسات الدقيقة، تركيب الشاشات ووحدات البلوتوث، بالإضافة إلى معامل معايرة الحساسات واختبار مقاومة الماء والغبار.</p><p>ويستهدف الخط تلبية الطلب المتنامي على الساعات الذكية في السوق المصري والإقليمي.</p>",
                    BodyEn = "<p>The factory has added a new production line dedicated to wearable devices, with capacity reaching 1,500 units per day.</p><p>The line includes precision sensor assembly stations, display and Bluetooth module fitting, plus sensor calibration labs and water and dust resistance testing.</p><p>It targets the growing demand for smartwatches across the Egyptian and regional market.</p>"
                },
                new NewsPost
                {
                    Slug = "partnership-with-leading-brands",
                    CategoryId = partnerships?.Id,
                    PublishedAt = DateTime.UtcNow.AddDays(-9),
                    AuthorName = "SVEI",
                    CoverImagePath = "/img/sections/feature2.webp",
                    Tags = "شراكات,علامات تجارية,OEM",
                    TitleAr = "شراكات تصنيع جديدة مع علامات تجارية رائدة",
                    TitleEn = "New manufacturing partnerships with leading brands",
                    ExcerptAr = "توقيع اتفاقيات تصنيع OEM مع عدد من العلامات العالمية العاملة في السوق المصري.",
                    ExcerptEn = "OEM manufacturing agreements signed with several global brands active in the Egyptian market.",
                    BodyAr = "<p>وقّع مصنع سيليكون فالي اتفاقيات تصنيع بنظام OEM مع مجموعة من العلامات التجارية الرائدة في مجال الهواتف والملحقات الإلكترونية.</p><p>وتشمل الاتفاقيات تصنيع وتجميع المنتجات وفق مواصفات كل علامة، مع التغليف بهويتها التجارية، وتسليمها من داخل المنطقة الاقتصادية لقناة السويس.</p>",
                    BodyEn = "<p>Silicon Valley has signed OEM manufacturing agreements with a group of leading brands in phones and electronic accessories.</p><p>The agreements cover manufacturing and assembly to each brand's specification, packaging in their own identity, and delivery from within the Suez Canal Economic Zone.</p>"
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task EventsAsync(AppDbContext db)
        {
            if (await db.Events.AnyAsync()) return;

            db.Events.AddRange(
                new Event
                {
                    Slug = "cairo-ict-expo",
                    EventType = "exhibition",
                    IsFeatured = true,
                    SortOrder = 1,
                    StartDate = DateTime.UtcNow.AddDays(35),
                    EndDate = DateTime.UtcNow.AddDays(38),
                    AllowRegistration = true,
                    CoverImagePath = "/img/hero/hero-chip.webp",
                    TitleAr = "معرض القاهرة الدولي للتكنولوجيا",
                    TitleEn = "Cairo ICT Technology Expo",
                    SummaryAr = "زورونا في جناح سيليكون فالي للاطلاع على أحدث منتجاتنا وخطوط إنتاجنا.",
                    SummaryEn = "Visit the Silicon Valley stand to see our latest products and production capabilities.",
                    LocationAr = "مركز مصر للمعارض الدولية، القاهرة",
                    LocationEn = "Egypt International Exhibition Center, Cairo",
                    DescAr = "<p>يشارك مصنع سيليكون فالي للصناعات الإلكترونية في معرض القاهرة الدولي للتكنولوجيا بجناح متكامل يعرض منتجاتنا من الهواتف المحمولة، بنوك الطاقة، الساعات الذكية وسماعات الأذن.</p><p>سيكون فريقنا الهندسي والتجاري متواجداً طوال أيام المعرض لمناقشة فرص التصنيع بنظام OEM و ODM.</p>",
                    DescEn = "<p>Silicon Valley for Electronic Industries will exhibit at Cairo ICT with a full stand showcasing our mobile phones, power banks, smartwatches and earbuds.</p><p>Our engineering and commercial teams will be present throughout the show to discuss OEM and ODM manufacturing opportunities.</p>"
                },
                new Event
                {
                    Slug = "factory-open-day-2025",
                    EventType = "visit",
                    SortOrder = 2,
                    StartDate = DateTime.UtcNow.AddDays(12),
                    EndDate = DateTime.UtcNow.AddDays(12),
                    AllowRegistration = true,
                    Capacity = 60,
                    CoverImagePath = "/img/hero/hero-factory.webp",
                    TitleAr = "اليوم المفتوح لزيارة المصنع",
                    TitleEn = "Factory Open Day",
                    SummaryAr = "جولة إرشادية داخل خطوط الإنتاج ومعامل الاختبار للعملاء والشركاء.",
                    SummaryEn = "A guided tour of the production lines and test labs for clients and partners.",
                    LocationAr = "المصنع الرئيسي – تيدا، العين السخنة",
                    LocationEn = "Main Factory – TEDA, Ain Sokhna",
                    DescAr = "<p>ندعو عملاءنا وشركاءنا لزيارة المصنع والاطلاع عن قرب على خطوط الإنتاج الأربعة ومعامل مراقبة الجودة.</p><p>تشمل الزيارة عرضاً تقديمياً عن قدراتنا التصنيعية، جولة ميدانية داخل الخطوط، ثم جلسة أسئلة مفتوحة مع الفريق الهندسي.</p><p>عدد المقاعد محدود — يُرجى التسجيل مسبقاً.</p>",
                    DescEn = "<p>We invite our clients and partners to visit the factory and see our four production lines and quality control labs up close.</p><p>The visit includes a presentation on our manufacturing capabilities, a walk-through of the lines, and an open Q&A session with the engineering team.</p><p>Places are limited — please register in advance.</p>"
                },
                new Event
                {
                    Slug = "smt-training-program",
                    EventType = "training",
                    SortOrder = 3,
                    StartDate = DateTime.UtcNow.AddDays(-25),
                    EndDate = DateTime.UtcNow.AddDays(-21),
                    CoverImagePath = "/img/sections/feature1.webp",
                    TitleAr = "برنامج تدريبي على تقنيات التجميع السطحي SMT",
                    TitleEn = "SMT Assembly Techniques Training Programme",
                    SummaryAr = "برنامج تدريبي مكثف لفنيي الإنتاج على تشغيل وصيانة خطوط SMT.",
                    SummaryEn = "An intensive programme for production technicians on operating and maintaining SMT lines.",
                    LocationAr = "مركز التدريب – المصنع الرئيسي",
                    LocationEn = "Training Centre – Main Factory",
                    DescAr = "<p>نظّم المصنع برنامجاً تدريبياً مكثفاً على مدار خمسة أيام لفنيي الإنتاج، تناول تشغيل ماكينات وضع المكونات، ضبط أفران إعادة التدفق، وقراءة نتائج الفحص البصري الآلي AOI.</p><p>يأتي البرنامج ضمن خطة الشركة لنقل المعرفة التقنية وبناء كوادر مصرية مؤهلة.</p>",
                    DescEn = "<p>The factory ran a five-day intensive training programme for production technicians covering pick-and-place machine operation, reflow oven profiling, and interpreting automated optical inspection (AOI) results.</p><p>The programme is part of the company's plan to transfer technical know-how and build qualified Egyptian talent.</p>"
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task CareersAsync(AppDbContext db)
        {
            if (!await db.JobCategories.AnyAsync())
            {
                db.JobCategories.AddRange(
                    new JobCategory { Slug = "production", NameAr = "الإنتاج والتشغيل", NameEn = "Production & Operations", Icon = "fa-solid fa-industry", SortOrder = 1 },
                    new JobCategory { Slug = "engineering", NameAr = "الهندسة", NameEn = "Engineering", Icon = "fa-solid fa-gears", SortOrder = 2 },
                    new JobCategory { Slug = "quality", NameAr = "الجودة", NameEn = "Quality", Icon = "fa-solid fa-vial-circle-check", SortOrder = 3 },
                    new JobCategory { Slug = "supply-chain", NameAr = "سلسلة الإمداد والمشتريات", NameEn = "Supply Chain & Procurement", Icon = "fa-solid fa-truck", SortOrder = 4 },
                    new JobCategory { Slug = "admin-hr", NameAr = "الإدارة والموارد البشرية", NameEn = "Administration & HR", Icon = "fa-solid fa-users", SortOrder = 5 },
                    new JobCategory { Slug = "sales-marketing", NameAr = "المبيعات والتسويق", NameEn = "Sales & Marketing", Icon = "fa-solid fa-chart-line", SortOrder = 6 }
                );
                await db.SaveChangesAsync();
            }

            if (await db.JobPostings.AnyAsync()) return;

            var production = await db.JobCategories.FirstAsync(c => c.Slug == "production");
            var engineering = await db.JobCategories.FirstAsync(c => c.Slug == "engineering");
            var quality = await db.JobCategories.FirstAsync(c => c.Slug == "quality");

            db.JobPostings.AddRange(
                new JobPosting
                {
                    Slug = "smt-production-engineer",
                    CategoryId = engineering.Id,
                    JobType = "Full-time",
                    ExperienceLevel = "Mid",
                    Vacancies = 2,
                    IsUrgent = true,
                    SortOrder = 1,
                    PostedAt = DateTime.UtcNow.AddDays(-6),
                    ClosingDate = DateTime.UtcNow.AddDays(24),
                    LocationAr = "العين السخنة، السويس", LocationEn = "Ain Sokhna, Suez",
                    TitleAr = "مهندس إنتاج – خطوط SMT", TitleEn = "Production Engineer – SMT Lines",
                    SummaryAr = "مسؤول عن تشغيل ومتابعة خطوط التجميع السطحي ورفع كفاءة الإنتاج.",
                    SummaryEn = "Responsible for running and monitoring SMT assembly lines and improving production efficiency.",
                    DescAr = "<p>نبحث عن مهندس إنتاج للانضمام لفريق خطوط التجميع السطحي، يتولى متابعة الأداء اليومي للخط، تحليل أسباب التوقف، وتنفيذ خطط تحسين الكفاءة.</p>",
                    DescEn = "<p>We're looking for a production engineer to join the SMT lines team, monitoring daily line performance, analysing downtime causes, and implementing efficiency improvement plans.</p>",
                    RequirementsAr = "<ul><li>بكالوريوس هندسة إلكترونيات أو ميكاترونكس أو ما يعادلها</li><li>خبرة من 3 إلى 5 سنوات في بيئة تصنيع إلكتروني</li><li>إلمام بماكينات وضع المكونات وأفران إعادة التدفق</li><li>إجادة اللغة الإنجليزية</li><li>القدرة على العمل بنظام الورديات</li></ul>",
                    RequirementsEn = "<ul><li>BSc in Electronics, Mechatronics Engineering or equivalent</li><li>3–5 years' experience in an electronics manufacturing environment</li><li>Familiarity with pick-and-place machines and reflow ovens</li><li>Good command of English</li><li>Willingness to work shifts</li></ul>",
                    BenefitsAr = "<ul><li>تأمين صحي واجتماعي شامل</li><li>انتقالات ووجبة يومية</li><li>برامج تدريب فنية</li></ul>",
                    BenefitsEn = "<ul><li>Comprehensive health and social insurance</li><li>Transport and a daily meal</li><li>Technical training programmes</li></ul>"
                },
                new JobPosting
                {
                    Slug = "quality-control-inspector",
                    CategoryId = quality.Id,
                    JobType = "Full-time",
                    ExperienceLevel = "Junior",
                    Vacancies = 4,
                    SortOrder = 2,
                    PostedAt = DateTime.UtcNow.AddDays(-4),
                    ClosingDate = DateTime.UtcNow.AddDays(26),
                    LocationAr = "العين السخنة، السويس", LocationEn = "Ain Sokhna, Suez",
                    TitleAr = "مفتش مراقبة جودة", TitleEn = "Quality Control Inspector",
                    SummaryAr = "فحص المكونات الواردة والمنتجات النهائية وتوثيق نتائج الاختبار.",
                    SummaryEn = "Inspect incoming components and finished products and document test results.",
                    DescAr = "<p>مسؤول عن تنفيذ خطط الفحص (IQC / IPQC / OQC)، تسجيل حالات عدم المطابقة، ورفع التقارير اليومية لمشرف الجودة.</p>",
                    DescEn = "<p>Responsible for executing inspection plans (IQC / IPQC / OQC), logging non-conformances, and reporting daily to the quality supervisor.</p>",
                    RequirementsAr = "<ul><li>بكالوريوس أو دبلوم فني في تخصص هندسي</li><li>خبرة من سنة إلى 3 سنوات في مراقبة الجودة</li><li>دقة عالية في الملاحظة والتوثيق</li><li>إلمام بأساسيات الحاسب الآلي</li></ul>",
                    RequirementsEn = "<ul><li>BSc or technical diploma in an engineering discipline</li><li>1–3 years' experience in quality control</li><li>High attention to detail and documentation accuracy</li><li>Basic computer literacy</li></ul>"
                },
                new JobPosting
                {
                    Slug = "production-line-operator",
                    CategoryId = production.Id,
                    JobType = "Shift",
                    ExperienceLevel = "Entry",
                    Vacancies = 20,
                    SortOrder = 3,
                    PostedAt = DateTime.UtcNow.AddDays(-2),
                    ClosingDate = DateTime.UtcNow.AddDays(30),
                    LocationAr = "العين السخنة، السويس", LocationEn = "Ain Sokhna, Suez",
                    TitleAr = "عامل خط إنتاج", TitleEn = "Production Line Operator",
                    SummaryAr = "العمل على محطات التجميع والاختبار داخل خطوط الإنتاج.",
                    SummaryEn = "Work on assembly and test stations across the production lines.",
                    DescAr = "<p>فرصة للانضمام لفريق الإنتاج مع تدريب كامل على رأس العمل. لا تُشترط خبرة سابقة.</p>",
                    DescEn = "<p>An opportunity to join the production team with full on-the-job training. No prior experience required.</p>",
                    RequirementsAr = "<ul><li>مؤهل متوسط أو فوق متوسط</li><li>القدرة على العمل بنظام الورديات</li><li>الالتزام والانضباط</li><li>يفضل سكان السويس والعين السخنة</li></ul>",
                    RequirementsEn = "<ul><li>Intermediate or above-intermediate qualification</li><li>Able to work shifts</li><li>Reliable and disciplined</li><li>Suez / Ain Sokhna residents preferred</li></ul>",
                    BenefitsAr = "<ul><li>تأمين صحي واجتماعي</li><li>انتقالات ووجبة ساخنة</li><li>تدريب على رأس العمل</li></ul>",
                    BenefitsEn = "<ul><li>Health and social insurance</li><li>Transport and a hot meal</li><li>On-the-job training</li></ul>"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
