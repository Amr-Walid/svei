using Microsoft.EntityFrameworkCore;
using SVEI.Web.Models;

namespace SVEI.Web.Data.Seed
{
    /// <summary>Production lines, services, brands — all taken from svei.tech.</summary>
    public static class SeedCatalog
    {
        public static async Task RunAsync(AppDbContext db)
        {
            await ProductionLinesAsync(db);
            await ServicesAsync(db);
            await BrandsAsync(db);
            await CategoriesAsync(db);
            await GalleryAsync(db);
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task ProductionLinesAsync(AppDbContext db)
        {
            if (await db.ProductionLines.AnyAsync()) return;

            var lines = new List<ProductionLine>
            {
                new()
                {
                    Slug = "cell-phones", SortOrder = 1, Icon = "fa-solid fa-mobile-screen",
                    AccentColor = "#E31B23",
                    NameAr = "خط إنتاج الهواتف المحمولة", NameEn = "Cell Phones Production Line",
                    ShortDescAr = "طاقة إنتاجية 2000 هاتف يومياً بخطوط تجميع سطحي حديثة.",
                    ShortDescEn = "2,000 handsets per day on modern SMT assembly lines.",
                    DescAr = "خط متكامل لتصنيع وتجميع الهواتف المحمولة يشمل تركيب اللوحات الإلكترونية بتقنية SMT، تجميع الشاشات والبطاريات، تحميل أنظمة التشغيل، الاختبار الوظيفي الشامل، ثم التغليف النهائي بهوية العميل التجارية.",
                    DescEn = "A complete line for manufacturing and assembling mobile phones: SMT board population, screen and battery assembly, OS flashing, full functional testing, and final packaging in the client's brand identity.",
                    CapacityPerDay = 2000,
                    CapacityUnitAr = "هاتف / يوم", CapacityUnitEn = "handsets / day",
                    ImagePath = "/img/lines/phones.webp",
                    Specs =
                    {
                        new ProductionLineSpec { LabelAr = "الطاقة اليومية", LabelEn = "Daily capacity", ValueAr = "2000 وحدة", ValueEn = "2,000 units", Icon = "fa-solid fa-gauge-high", SortOrder = 1 },
                        new ProductionLineSpec { LabelAr = "تقنية التجميع", LabelEn = "Assembly technology", ValueAr = "SMT + تجميع يدوي دقيق", ValueEn = "SMT + precision manual assembly", Icon = "fa-solid fa-microchip", SortOrder = 2 },
                        new ProductionLineSpec { LabelAr = "الاختبار", LabelEn = "Testing", ValueAr = "اختبار وظيفي آلي 100%", ValueEn = "100% automated functional test", Icon = "fa-solid fa-vial-circle-check", SortOrder = 3 },
                        new ProductionLineSpec { LabelAr = "التغليف", LabelEn = "Packaging", ValueAr = "تغليف بهوية العميل", ValueEn = "Client-branded packaging", Icon = "fa-solid fa-box", SortOrder = 4 }
                    }
                },
                new()
                {
                    Slug = "power-storage", SortOrder = 2, Icon = "fa-solid fa-battery-full",
                    AccentColor = "#22C6E8",
                    NameAr = "خط إنتاج أجهزة تخزين الطاقة", NameEn = "Power Storage Production Line",
                    ShortDescAr = "1000 وحدة يومياً من بنوك الطاقة وأجهزة الشحن.",
                    ShortDescEn = "1,000 power banks and charging devices per day.",
                    DescAr = "خط مخصص لتصنيع بنوك الطاقة وأجهزة الشحن المحمولة، يشمل تجميع خلايا الليثيوم، لحام دوائر الحماية، اختبار السعة الفعلية ودورات الشحن والتفريغ للتأكد من السلامة والأداء.",
                    DescEn = "A dedicated line for power banks and portable charging devices: lithium cell assembly, protection-circuit soldering, real capacity testing, and charge/discharge cycling to verify safety and performance.",
                    CapacityPerDay = 1000,
                    CapacityUnitAr = "وحدة / يوم", CapacityUnitEn = "units / day",
                    ImagePath = "/img/lines/power.webp",
                    Specs =
                    {
                        new ProductionLineSpec { LabelAr = "الطاقة اليومية", LabelEn = "Daily capacity", ValueAr = "1000 وحدة", ValueEn = "1,000 units", Icon = "fa-solid fa-gauge-high", SortOrder = 1 },
                        new ProductionLineSpec { LabelAr = "نوع الخلايا", LabelEn = "Cell type", ValueAr = "ليثيوم أيون / بوليمر", ValueEn = "Li-ion / Li-polymer", Icon = "fa-solid fa-bolt", SortOrder = 2 },
                        new ProductionLineSpec { LabelAr = "اختبار السلامة", LabelEn = "Safety testing", ValueAr = "دوائر حماية + دورات شحن", ValueEn = "Protection circuits + cycle testing", Icon = "fa-solid fa-shield-halved", SortOrder = 3 }
                    }
                },
                new()
                {
                    Slug = "wearables", SortOrder = 3, Icon = "fa-solid fa-clock",
                    AccentColor = "#FDB714",
                    NameAr = "خط إنتاج الأجهزة القابلة للارتداء", NameEn = "Wearables Production Line",
                    ShortDescAr = "1500 ساعة ذكية وجهاز قابل للارتداء يومياً.",
                    ShortDescEn = "1,500 smartwatches and wearable devices per day.",
                    DescAr = "خط متخصص في تصنيع الساعات الذكية وأساور اللياقة، يشمل تجميع الحساسات الدقيقة، الشاشات، وحدات البلوتوث، مع اختبارات مقاومة الماء والغبار ومعايرة الحساسات.",
                    DescEn = "Specialised in smartwatches and fitness bands: precision sensor assembly, displays, Bluetooth modules, plus water and dust resistance testing and sensor calibration.",
                    CapacityPerDay = 1500,
                    CapacityUnitAr = "جهاز / يوم", CapacityUnitEn = "devices / day",
                    ImagePath = "/img/lines/wearables.webp",
                    Specs =
                    {
                        new ProductionLineSpec { LabelAr = "الطاقة اليومية", LabelEn = "Daily capacity", ValueAr = "1500 جهاز", ValueEn = "1,500 devices", Icon = "fa-solid fa-gauge-high", SortOrder = 1 },
                        new ProductionLineSpec { LabelAr = "المنتجات", LabelEn = "Products", ValueAr = "ساعات ذكية، أساور لياقة", ValueEn = "Smartwatches, fitness bands", Icon = "fa-solid fa-heart-pulse", SortOrder = 2 },
                        new ProductionLineSpec { LabelAr = "الاختبارات", LabelEn = "Testing", ValueAr = "مقاومة الماء والغبار، معايرة الحساسات", ValueEn = "Water/dust resistance, sensor calibration", Icon = "fa-solid fa-droplet", SortOrder = 3 }
                    }
                },
                new()
                {
                    Slug = "audio-peripherals", SortOrder = 4, Icon = "fa-solid fa-headphones",
                    AccentColor = "#0E7C99",
                    NameAr = "خط إنتاج ملحقات الصوت", NameEn = "Audio Peripherals Production Line",
                    ShortDescAr = "سماعات أذن لاسلكية ومكبرات صوت بجودة عالية.",
                    ShortDescEn = "Wireless earbuds and speakers built to a high audio standard.",
                    DescAr = "خط تصنيع سماعات الأذن اللاسلكية، السماعات السلكية ومكبرات الصوت المحمولة، مع غرفة اختبار صوتي معزولة لضمان جودة الصوت ومطابقة المواصفات.",
                    DescEn = "Manufacturing wireless earbuds, wired headsets and portable speakers, with an isolated acoustic test chamber to guarantee sound quality and specification compliance.",
                    CapacityUnitAr = "وحدة / يوم", CapacityUnitEn = "units / day",
                    ImagePath = "/img/lines/audio.webp",
                    Specs =
                    {
                        new ProductionLineSpec { LabelAr = "المنتجات", LabelEn = "Products", ValueAr = "سماعات لاسلكية، سلكية، مكبرات صوت", ValueEn = "Wireless earbuds, wired headsets, speakers", Icon = "fa-solid fa-headphones", SortOrder = 1 },
                        new ProductionLineSpec { LabelAr = "اختبار الصوت", LabelEn = "Acoustic testing", ValueAr = "غرفة اختبار معزولة", ValueEn = "Isolated test chamber", Icon = "fa-solid fa-wave-square", SortOrder = 2 }
                    }
                }
            };

            db.ProductionLines.AddRange(lines);
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task ServicesAsync(AppDbContext db)
        {
            if (await db.ServiceItems.AnyAsync()) return;

            db.ServiceItems.AddRange(
                new ServiceItem
                {
                    Slug = "oem-odm", ImagePath = "/img/services/oem.webp", SortOrder = 1, IsFeatured = true, Icon = "fa-solid fa-industry",
                    AccentColor = "#E31B23",
                    TitleAr = "تصنيع OEM و ODM", TitleEn = "OEM & ODM Manufacturing",
                    ShortDescAr = "نصنع منتجك بمواصفاتك وهويتك التجارية، أو نطوّر لك التصميم من الصفر.",
                    ShortDescEn = "We build to your specification and brand, or develop the design for you from scratch.",
                    DescAr = "سواء كان لديك تصميم جاهز تريد تصنيعه (OEM) أو فكرة تحتاج لتحويلها لمنتج كامل (ODM)، فريقنا الهندسي يرافقك من دراسة الجدوى الفنية حتى الإنتاج الكمي.",
                    DescEn = "Whether you have a finished design to manufacture (OEM) or an idea that needs turning into a full product (ODM), our engineering team supports you from technical feasibility through to volume production.",
                    Features =
                    {
                        new ServiceFeature { TextAr = "دراسة جدوى فنية مجانية", TextEn = "Free technical feasibility study", SortOrder = 1 },
                        new ServiceFeature { TextAr = "تطوير العينات الأولية", TextEn = "Prototype development", SortOrder = 2 },
                        new ServiceFeature { TextAr = "تصنيع بهوية العميل", TextEn = "Client-branded manufacturing", SortOrder = 3 }
                    }
                },
                new ServiceItem
                {
                    Slug = "smt-assembly", ImagePath = "/img/services/smt.webp", SortOrder = 2, IsFeatured = true, Icon = "fa-solid fa-microchip",
                    AccentColor = "#22C6E8",
                    TitleAr = "التجميع السطحي SMT", TitleEn = "SMT Assembly",
                    ShortDescAr = "تركيب المكونات الإلكترونية على اللوحات بدقة عالية وسرعة إنتاجية.",
                    ShortDescEn = "High-precision, high-throughput placement of components on PCBs.",
                    DescAr = "خطوط SMT مجهزة بماكينات وضع مكونات آلية وأفران إعادة تدفق مضبوطة الحرارة، مع فحص بصري آلي AOI لضمان سلامة كل نقطة لحام.",
                    DescEn = "SMT lines equipped with automated pick-and-place machines and temperature-profiled reflow ovens, with automated optical inspection (AOI) validating every solder joint.",
                    Features =
                    {
                        new ServiceFeature { TextAr = "وضع مكونات آلي", TextEn = "Automated pick-and-place", SortOrder = 1 },
                        new ServiceFeature { TextAr = "فحص بصري آلي AOI", TextEn = "Automated optical inspection (AOI)", SortOrder = 2 },
                        new ServiceFeature { TextAr = "أفران إعادة تدفق مضبوطة", TextEn = "Profiled reflow ovens", SortOrder = 3 }
                    }
                },
                new ServiceItem
                {
                    Slug = "quality-control", ImagePath = "/img/services/qc.webp", SortOrder = 3, IsFeatured = true, Icon = "fa-solid fa-vial-circle-check",
                    AccentColor = "#FDB714",
                    TitleAr = "مراقبة الجودة والاختبار", TitleEn = "Quality Control & Testing",
                    ShortDescAr = "نظام جودة متعدد المراحل من استلام المكونات حتى المنتج النهائي.",
                    ShortDescEn = "Multi-stage quality system from incoming components to finished goods.",
                    DescAr = "نطبّق فحص المكونات الواردة (IQC)، مراقبة أثناء التصنيع (IPQC)، وفحص المنتج النهائي (OQC)، مدعومة بمعامل اختبار مجهزة وتوثيق كامل لكل دفعة إنتاج.",
                    DescEn = "We apply incoming quality control (IQC), in-process control (IPQC) and outgoing inspection (OQC), backed by equipped test labs and full batch documentation.",
                    Features =
                    {
                        new ServiceFeature { TextAr = "فحص المكونات الواردة IQC", TextEn = "Incoming quality control (IQC)", SortOrder = 1 },
                        new ServiceFeature { TextAr = "مراقبة أثناء التصنيع IPQC", TextEn = "In-process control (IPQC)", SortOrder = 2 },
                        new ServiceFeature { TextAr = "توثيق كامل لكل دفعة", TextEn = "Full batch traceability", SortOrder = 3 }
                    }
                },
                new ServiceItem
                {
                    Slug = "packaging-logistics", ImagePath = "/img/services/packaging.webp", SortOrder = 4, Icon = "fa-solid fa-boxes-packing",
                    AccentColor = "#0E7C99",
                    TitleAr = "التغليف والخدمات اللوجستية", TitleEn = "Packaging & Logistics",
                    ShortDescAr = "تغليف احترافي بهويتك التجارية وشحن من ميناء السخنة.",
                    ShortDescEn = "Professional branded packaging and shipping from Sokhna Port.",
                    DescAr = "نوفر تصميم وتنفيذ عبوات المنتج بهوية علامتك، مع خدمات التخزين والشحن مستفيدين من قربنا لميناء السخنة والمزايا الجمركية للمنطقة الاقتصادية.",
                    DescEn = "We design and produce product packaging in your brand identity, with warehousing and shipping services leveraging our proximity to Sokhna Port and the economic zone's customs advantages.",
                    Features =
                    {
                        new ServiceFeature { TextAr = "تصميم عبوات مخصصة", TextEn = "Custom packaging design", SortOrder = 1 },
                        new ServiceFeature { TextAr = "تخزين آمن", TextEn = "Secure warehousing", SortOrder = 2 },
                        new ServiceFeature { TextAr = "شحن دولي من ميناء السخنة", TextEn = "International shipping from Sokhna Port", SortOrder = 3 }
                    }
                },
                new ServiceItem
                {
                    Slug = "after-sales", ImagePath = "/img/services/support.webp", SortOrder = 5, Icon = "fa-solid fa-screwdriver-wrench",
                    AccentColor = "#5A5C5E",
                    TitleAr = "خدمات ما بعد البيع", TitleEn = "After-Sales Services",
                    ShortDescAr = "دعم فني وصيانة وقطع غيار على مدار الساعة.",
                    ShortDescEn = "Round-the-clock technical support, maintenance and spare parts.",
                    DescAr = "مركز خدمة متكامل يقدم الدعم الفني، الصيانة داخل الضمان وخارجه، وتوفير قطع الغيار الأصلية للعملاء والموزعين.",
                    DescEn = "A full service centre providing technical support, in- and out-of-warranty maintenance, and genuine spare parts for clients and distributors.",
                    Features =
                    {
                        new ServiceFeature { TextAr = "دعم فني 24/7", TextEn = "24/7 technical support", SortOrder = 1 },
                        new ServiceFeature { TextAr = "صيانة داخل وخارج الضمان", TextEn = "In- and out-of-warranty repair", SortOrder = 2 },
                        new ServiceFeature { TextAr = "قطع غيار أصلية", TextEn = "Genuine spare parts", SortOrder = 3 }
                    }
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task BrandsAsync(AppDbContext db)
        {
            if (await db.Brands.AnyAsync()) return;

            db.Brands.AddRange(
                new Brand
                {
                    Slug = "oraimo", NameAr = "أوريمو", NameEn = "Oraimo", SortOrder = 1,
                    LogoPath = "/img/brands/1.webp", BrandType = "client",
                    DescAr = "علامة عالمية في ملحقات الهواتف والأجهزة الذكية.",
                    DescEn = "A global brand in phone accessories and smart devices."
                },
                new Brand
                {
                    Slug = "infinix", NameAr = "إنفينكس", NameEn = "Infinix", SortOrder = 2,
                    LogoPath = "/img/brands/2.webp", BrandType = "client",
                    DescAr = "علامة رائدة في الهواتف الذكية بالأسواق الناشئة.",
                    DescEn = "A leading smartphone brand across emerging markets."
                },
                new Brand
                {
                    Slug = "itel", NameAr = "آيتل", NameEn = "Itel", SortOrder = 3,
                    LogoPath = "/img/brands/3.webp", BrandType = "client",
                    DescAr = "هواتف ذكية اقتصادية بجودة موثوقة.",
                    DescEn = "Affordable smartphones with dependable quality."
                },
                new Brand
                {
                    Slug = "gtide", NameAr = "جي تايد", NameEn = "Gtide", SortOrder = 4,
                    LogoPath = "/img/brands/4.webp", BrandType = "client",
                    DescAr = "ملحقات إلكترونية وحلول طاقة محمولة.",
                    DescEn = "Electronic accessories and portable power solutions."
                },
                new Brand
                {
                    Slug = "unitronics", NameAr = "يوني ترونيكس", NameEn = "UniTronics", SortOrder = 5,
                    LogoPath = "/img/brands/5.webp", BrandType = "partner",
                    DescAr = "شريك في التوزيع والحلول التقنية.",
                    DescEn = "A distribution and technology solutions partner."
                }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task CategoriesAsync(AppDbContext db)
        {
            if (await db.ProductCategories.AnyAsync()) return;

            db.ProductCategories.AddRange(
                new ProductCategory { Slug = "smartphones", ImagePath = "/img/products/smartphones.webp", NameAr = "هواتف ذكية", NameEn = "Smartphones", Icon = "fa-solid fa-mobile-screen", SortOrder = 1 },
                new ProductCategory { Slug = "power-banks", ImagePath = "/img/products/powerbanks.webp", NameAr = "بنوك طاقة", NameEn = "Power Banks", Icon = "fa-solid fa-battery-full", SortOrder = 2 },
                new ProductCategory { Slug = "smartwatches", ImagePath = "/img/products/smartwatches.webp", NameAr = "ساعات ذكية", NameEn = "Smartwatches", Icon = "fa-solid fa-clock", SortOrder = 3 },
                new ProductCategory { Slug = "audio", ImagePath = "/img/products/audio.webp", NameAr = "سماعات وصوتيات", NameEn = "Audio & Headphones", Icon = "fa-solid fa-headphones", SortOrder = 4 },
                new ProductCategory { Slug = "accessories", ImagePath = "/img/products/accessories.webp", NameAr = "إكسسوارات", NameEn = "Accessories", Icon = "fa-solid fa-plug", SortOrder = 5 }
            );
            await db.SaveChangesAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        private static async Task GalleryAsync(AppDbContext db)
        {
            if (await db.GalleryAlbums.AnyAsync()) return;

            db.GalleryAlbums.AddRange(
                new GalleryAlbum
                {
                    Slug = "factory-tour", SortOrder = 1,
                    TitleAr = "جولة داخل المصنع", TitleEn = "Factory Tour",
                    DescAr = "صور من داخل خطوط الإنتاج ومعامل الاختبار.",
                    DescEn = "Photos from inside our production lines and test labs.",
                    CoverImagePath = "/img/hero/hero-factory.webp"
                },
                new GalleryAlbum
                {
                    Slug = "production-lines", SortOrder = 2,
                    TitleAr = "خطوط الإنتاج", TitleEn = "Production Lines",
                    DescAr = "معدات وخطوط التجميع السطحي والاختبار.",
                    DescEn = "SMT assembly and testing equipment.",
                    CoverImagePath = "/img/hero/hero-chip.webp"
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
