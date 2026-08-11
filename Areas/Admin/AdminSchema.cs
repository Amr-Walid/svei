using System.Reflection;
using SVEI.Web.Models;

namespace SVEI.Web.Admin
{
    // ═══════════════════════════════════════════════════════════════════════════
    //  FIELD KINDS — drive which editor widget the admin form renders.
    // ═══════════════════════════════════════════════════════════════════════════
    public enum FieldKind
    {
        Text,      // single-line
        TextArea,  // multi-line plain
        Html,      // Quill rich-text
        Number,
        Decimal,
        Bool,
        Date,
        DateTime,
        Image,     // upload -> WebP
        File,      // upload verbatim (pdf/doc)
        Color,
        Url,
        Email,
        Slug,
        Select,    // static option list
        Lookup,    // FK dropdown from another table
        Icon,      // Font Awesome class picker
        Hidden,
        ReadOnly
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  FIELD — one editable property.
    // ═══════════════════════════════════════════════════════════════════════════
    public class Field
    {
        public string Name { get; set; } = "";
        public string LabelAr { get; set; } = "";
        public string LabelEn { get; set; } = "";
        public FieldKind Kind { get; set; } = FieldKind.Text;

        /// <summary>Field belongs to a language pair; UI groups Ar/En side by side.</summary>
        public string? Pair { get; set; }
        /// <summary>"ar" or "en" when this field is one half of a Pair.</summary>
        public string? Lang { get; set; }

        public string? HintAr { get; set; }
        public bool Required { get; set; }
        public bool ShowInList { get; set; }
        public bool Searchable { get; set; }
        /// <summary>Column span in the 12-col admin form grid.</summary>
        public int Span { get; set; } = 12;
        public string? Group { get; set; }
        public string? Placeholder { get; set; }

        /// <summary>Static options for FieldKind.Select — value/labelAr/labelEn.</summary>
        public (string Value, string Ar, string En)[]? Options { get; set; }

        /// <summary>Entity type for FieldKind.Lookup.</summary>
        public Type? LookupType { get; set; }
        /// <summary>Upload sub-folder for Image/File.</summary>
        public string? Folder { get; set; }
        /// <summary>Source field name for Slug auto-generation.</summary>
        public string? SlugFrom { get; set; }

        public Field(string name, string ar, string en, FieldKind kind = FieldKind.Text)
        { Name = name; LabelAr = ar; LabelEn = en; Kind = kind; }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  ENTITY DESCRIPTOR — everything the generic admin needs about a table.
    // ═══════════════════════════════════════════════════════════════════════════
    public class EntityDef
    {
        /// <summary>URL slug, e.g. "products".</summary>
        public string Key { get; set; } = "";
        public Type ClrType { get; set; } = default!;
        public string TitleAr { get; set; } = "";
        public string TitleEn { get; set; } = "";
        public string SingularAr { get; set; } = "";
        public string SingularEn { get; set; } = "";
        public string Icon { get; set; } = "fa-solid fa-table";
        /// <summary>Sidebar group: content | catalog | community | company | inbox | system</summary>
        public string Section { get; set; } = "content";
        public int Order { get; set; } = 100;

        public List<Field> Fields { get; set; } = new();

        /// <summary>Default ordering expression, e.g. "SortOrder" or "CreatedAt desc".</summary>
        public string OrderBy { get; set; } = "Id desc";
        public bool CanCreate { get; set; } = true;
        public bool CanDelete { get; set; } = true;
        public bool CanEdit { get; set; } = true;
        /// <summary>Rows are drag-sortable (entity has SortOrder).</summary>
        public bool Sortable { get; set; }
        /// <summary>Show unread badge in sidebar (inbox tables with IsRead).</summary>
        public bool HasUnread { get; set; }
        /// <summary>Parent-child editing: child entity key shown on the edit page.</summary>
        public string? ChildKey { get; set; }
        public string? ChildFk { get; set; }
        /// <summary>Hidden from sidebar (edited only as a child).</summary>
        public bool HideInNav { get; set; }
        /// <summary>Restrict this table's group filter to these distinct values.</summary>
        public string? GroupField { get; set; }

        public IEnumerable<Field> ListFields => Fields.Where(f => f.ShowInList);
        public IEnumerable<string> SearchFields => Fields.Where(f => f.Searchable).Select(f => f.Name);
        public Field? Get(string name) => Fields.FirstOrDefault(f => f.Name == name);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  REGISTRY — the single source of truth for the whole admin panel.
    // ═══════════════════════════════════════════════════════════════════════════
    public static class AdminSchema
    {
        private static List<EntityDef>? _all;
        public static List<EntityDef> All => _all ??= Build();

        public static EntityDef? Find(string key) =>
            All.FirstOrDefault(e => string.Equals(e.Key, key, StringComparison.OrdinalIgnoreCase));

        public static IEnumerable<EntityDef> Nav(string section) =>
            All.Where(e => e.Section == section && !e.HideInNav).OrderBy(e => e.Order);

        public static readonly (string Key, string Ar, string En, string Icon)[] Sections =
        {
            ("content",   "المحتوى",       "Content",   "fa-solid fa-pen-ruler"),
            ("catalog",   "الكتالوج",      "Catalog",   "fa-solid fa-boxes-stacked"),
            ("community", "الأخبار والوظائف","Community", "fa-solid fa-bullhorn"),
            ("company",   "الشركة",        "Company",   "fa-solid fa-building"),
            ("inbox",     "الرسائل",       "Inbox",     "fa-solid fa-inbox"),
            ("system",    "النظام",        "System",    "fa-solid fa-gears"),
        };

        // ── shared option sets ────────────────────────────────────────────────
        static (string, string, string)[] BrandTypes = {
            ("client","عميل","Client"), ("partner","شريك","Partner"), ("supplier","مورد","Supplier")
        };
        static (string, string, string)[] EventTypes = {
            ("exhibition","معرض","Exhibition"), ("conference","مؤتمر","Conference"),
            ("workshop","ورشة عمل","Workshop"), ("launch","إطلاق منتج","Product Launch"),
            ("visit","زيارة","Visit"), ("ceremony","حفل","Ceremony"), ("other","أخرى","Other")
        };
        static (string, string, string)[] JobTypes = {
            ("full-time","دوام كامل","Full-time"), ("part-time","دوام جزئي","Part-time"),
            ("contract","عقد","Contract"), ("internship","تدريب","Internship"),
            ("remote","عن بعد","Remote"), ("hybrid","هجين","Hybrid")
        };
        static (string, string, string)[] JobLevels = {
            ("entry","مبتدئ","Entry"), ("junior","جونيور","Junior"), ("mid","متوسط","Mid-level"),
            ("senior","سينيور","Senior"), ("lead","قائد فريق","Lead"), ("manager","مدير","Manager")
        };
        static (string, string, string)[] RequestStatus = {
            ("new","جديد","New"), ("in-review","قيد المراجعة","In review"),
            ("contacted","تم التواصل","Contacted"), ("won","مقبول","Accepted"),
            ("lost","مرفوض","Rejected"), ("closed","مغلق","Closed")
        };
        static (string, string, string)[] AppStatus = {
            ("new","جديد","New"), ("screening","فرز","Screening"), ("interview","مقابلة","Interview"),
            ("offer","عرض","Offer"), ("hired","تم التعيين","Hired"), ("rejected","مرفوض","Rejected")
        };
        static (string, string, string)[] AlignOpts = {
            ("start","بداية","Start"), ("center","وسط","Center"), ("end","نهاية","End")
        };
        static (string, string, string)[] InputTypes = {
            ("text","نص","Text"), ("textarea","نص طويل","Textarea"), ("html","محرر","Rich text"),
            ("image","صورة","Image"), ("number","رقم","Number"), ("bool","نعم/لا","Boolean"),
            ("color","لون","Color"), ("url","رابط","URL"), ("email","بريد","Email")
        };
        static (string, string, string)[] LocationTypes = {
            ("factory","مصنع","Factory"), ("office","مكتب","Office"),
            ("warehouse","مستودع","Warehouse"), ("showroom","معرض","Showroom")
        };

        // ── field factory shorthands ──────────────────────────────────────────
        static Field F(string n, string ar, string en, FieldKind k = FieldKind.Text, int span = 12)
            => new Field(n, ar, en, k) { Span = span };

        /// <summary>Creates the Ar/En pair for a localized property base name.</summary>
        static IEnumerable<Field> P(string baseName, string ar, string en,
                                    FieldKind k = FieldKind.Text, bool listAr = false,
                                    bool search = false, bool required = false, string? group = null)
        {
            yield return new Field(baseName + "Ar", ar + " (عربي)", en + " (Arabic)", k)
            { Pair = baseName, Lang = "ar", Span = 6, ShowInList = listAr, Searchable = search, Required = required, Group = group };
            yield return new Field(baseName + "En", ar + " (إنجليزي)", en + " (English)", k)
            { Pair = baseName, Lang = "en", Span = 6, Searchable = search, Required = required, Group = group };
        }

        static Field Sort() => new Field("SortOrder", "الترتيب", "Sort order", FieldKind.Number) { Span = 3, ShowInList = true };
        static Field Flag(string n, string ar, string en, bool list = true)
            => new Field(n, ar, en, FieldKind.Bool) { Span = 3, ShowInList = list };
        static Field Slug(string from = "TitleEn") =>
            new Field("Slug", "الرابط اللطيف", "Slug", FieldKind.Slug) { Span = 6, SlugFrom = from, Searchable = true, Required = true };
        static Field Icon(int span = 6) => new Field("Icon", "الأيقونة", "Icon", FieldKind.Icon) { Span = span };
        static Field Img(string n, string ar, string en, string folder, int span = 6)
            => new Field(n, ar, en, FieldKind.Image) { Folder = folder, Span = span };
        static Field Look(string n, string ar, string en, Type t, int span = 6, bool req = false)
            => new Field(n, ar, en, FieldKind.Lookup) { LookupType = t, Span = span, ShowInList = true, Required = req };
        static Field Sel(string n, string ar, string en, (string, string, string)[] opts, int span = 6)
            => new Field(n, ar, en, FieldKind.Select) { Options = opts, Span = span, ShowInList = true };
        static Field Seo() => new Field("MetaTitle", "عنوان SEO", "SEO title") { Span = 6, Group = "seo" };
        static Field SeoD() => new Field("MetaDesc", "وصف SEO", "SEO description", FieldKind.TextArea) { Span = 6, Group = "seo" };
        static Field Created() => new Field("CreatedAt", "أُنشئ في", "Created", FieldKind.ReadOnly) { Span = 4, ShowInList = true };

        private static List<EntityDef> Build()
        {
            var L = new List<EntityDef>();

            // ═══════════════════ CONTENT ═══════════════════
            L.Add(new EntityDef
            {
                Key = "settings", ClrType = typeof(SiteSetting),
                TitleAr = "إعدادات الموقع", TitleEn = "Site settings",
                SingularAr = "إعداد", SingularEn = "setting",
                Icon = "fa-solid fa-sliders", Section = "content", Order = 1,
                OrderBy = "Group,SortOrder", GroupField = "Group",
                Fields = {
                    new Field("Key","المفتاح","Key"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("Group","المجموعة","Group"){Span=6,ShowInList=true,Searchable=true},
                    new Field("ValueAr","القيمة (عربي)","Value (Arabic)",FieldKind.TextArea){Span=6,Pair="Value",Lang="ar",ShowInList=true,Searchable=true},
                    new Field("ValueEn","القيمة (إنجليزي)","Value (English)",FieldKind.TextArea){Span=6,Pair="Value",Lang="en",Searchable=true},
                    new Field("LabelAr","الوصف (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar"},
                    new Field("LabelEn","الوصف (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en"},
                    new Field("HintAr","ملاحظة","Hint",FieldKind.TextArea){Span=12},
                    Sel("InputType","نوع الحقل","Input type",InputTypes,4),
                    Flag("IsLocalized","مترجم؟","Localized?",false),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "sections", ClrType = typeof(PageSection),
                TitleAr = "أقسام الصفحات", TitleEn = "Page sections",
                SingularAr = "قسم", SingularEn = "section",
                Icon = "fa-solid fa-puzzle-piece", Section = "content", Order = 2,
                OrderBy = "PageKey,SortOrder", GroupField = "PageKey", Sortable = true,
                Fields = {
                    new Field("PageKey","الصفحة","Page"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("SectionKey","مفتاح القسم","Section key"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("EyebrowAr","التمهيد (عربي)","Eyebrow (Arabic)"){Span=6,Pair="Eyebrow",Lang="ar"},
                    new Field("EyebrowEn","التمهيد (إنجليزي)","Eyebrow (English)"){Span=6,Pair="Eyebrow",Lang="en"},
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true},
                    new Field("SubtitleAr","العنوان الفرعي (عربي)","Subtitle (Arabic)",FieldKind.TextArea){Span=6,Pair="Subtitle",Lang="ar"},
                    new Field("SubtitleEn","العنوان الفرعي (إنجليزي)","Subtitle (English)",FieldKind.TextArea){Span=6,Pair="Subtitle",Lang="en"},
                    new Field("BodyAr","المحتوى (عربي)","Body (Arabic)",FieldKind.Html){Span=12,Pair="Body",Lang="ar"},
                    new Field("BodyEn","المحتوى (إنجليزي)","Body (English)",FieldKind.Html){Span=12,Pair="Body",Lang="en"},
                    new Field("ButtonTextAr","نص الزر (عربي)","Button text (Arabic)"){Span=4,Pair="ButtonText",Lang="ar",Group="cta"},
                    new Field("ButtonTextEn","نص الزر (إنجليزي)","Button text (English)"){Span=4,Pair="ButtonText",Lang="en",Group="cta"},
                    new Field("ButtonUrl","رابط الزر","Button URL",FieldKind.Url){Span=4,Group="cta"},
                    new Field("Button2TextAr","نص الزر ٢ (عربي)","Button 2 text (Arabic)"){Span=4,Pair="Button2Text",Lang="ar",Group="cta"},
                    new Field("Button2TextEn","نص الزر ٢ (إنجليزي)","Button 2 text (English)"){Span=4,Pair="Button2Text",Lang="en",Group="cta"},
                    new Field("Button2Url","رابط الزر ٢","Button 2 URL",FieldKind.Url){Span=4,Group="cta"},
                    Img("ImagePath","الصورة","Image","sections"),
                    Img("Image2Path","صورة ٢","Image 2","sections"),
                    new Field("VideoUrl","رابط الفيديو","Video URL",FieldKind.Url){Span=6},
                    Icon(),
                    new Field("ExtraJson","بيانات إضافية (JSON)","Extra data (JSON)",FieldKind.TextArea){Span=12,Group="advanced"},
                    Flag("IsVisible","ظاهر؟","Visible?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "translations", ClrType = typeof(Translation),
                TitleAr = "الكلمات والترجمات", TitleEn = "Translations",
                SingularAr = "ترجمة", SingularEn = "translation",
                Icon = "fa-solid fa-language", Section = "content", Order = 3,
                OrderBy = "Group,Key", GroupField = "Group",
                Fields = {
                    new Field("Key","المفتاح","Key"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("Group","المجموعة","Group"){Span=6,ShowInList=true,Searchable=true},
                    new Field("ValueAr","النص (عربي)","Text (Arabic)"){Span=6,Pair="Value",Lang="ar",ShowInList=true,Searchable=true},
                    new Field("ValueEn","النص (إنجليزي)","Text (English)"){Span=6,Pair="Value",Lang="en",ShowInList=true,Searchable=true},
                    new Field("Note","ملاحظة","Note",FieldKind.TextArea){Span=12},
                }
            });

            L.Add(new EntityDef
            {
                Key = "hero", ClrType = typeof(HeroSlide),
                TitleAr = "شرائح الواجهة", TitleEn = "Hero slides",
                SingularAr = "شريحة", SingularEn = "slide",
                Icon = "fa-solid fa-images", Section = "content", Order = 4,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("EyebrowAr","التمهيد (عربي)","Eyebrow (Arabic)"){Span=6,Pair="Eyebrow",Lang="ar"},
                    new Field("EyebrowEn","التمهيد (إنجليزي)","Eyebrow (English)"){Span=6,Pair="Eyebrow",Lang="en"},
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    new Field("SubtitleAr","الوصف (عربي)","Subtitle (Arabic)",FieldKind.TextArea){Span=6,Pair="Subtitle",Lang="ar"},
                    new Field("SubtitleEn","الوصف (إنجليزي)","Subtitle (English)",FieldKind.TextArea){Span=6,Pair="Subtitle",Lang="en"},
                    Img("ImagePath","صورة الخلفية","Background image","hero"),
                    new Field("VideoUrl","رابط الفيديو","Video URL",FieldKind.Url){Span=6},
                    new Field("ButtonTextAr","نص الزر (عربي)","Button text (Arabic)"){Span=4,Pair="ButtonText",Lang="ar",Group="cta"},
                    new Field("ButtonTextEn","نص الزر (إنجليزي)","Button text (English)"){Span=4,Pair="ButtonText",Lang="en",Group="cta"},
                    new Field("ButtonUrl","رابط الزر","Button URL",FieldKind.Url){Span=4,Group="cta"},
                    new Field("Button2TextAr","نص الزر ٢ (عربي)","Button 2 text (Arabic)"){Span=4,Pair="Button2Text",Lang="ar",Group="cta"},
                    new Field("Button2TextEn","نص الزر ٢ (إنجليزي)","Button 2 text (English)"){Span=4,Pair="Button2Text",Lang="en",Group="cta"},
                    new Field("Button2Url","رابط الزر ٢","Button 2 URL",FieldKind.Url){Span=4,Group="cta"},
                    Sel("Align","المحاذاة","Alignment",AlignOpts,4),
                    new Field("OverlayOpacity","شفافية الطبقة %","Overlay opacity %",FieldKind.Number){Span=4},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "cards", ClrType = typeof(InfoCard),
                TitleAr = "بطاقات المعلومات", TitleEn = "Info cards",
                SingularAr = "بطاقة", SingularEn = "card",
                Icon = "fa-solid fa-address-card", Section = "content", Order = 5,
                OrderBy = "GroupKey,SortOrder", GroupField = "GroupKey", Sortable = true,
                Fields = {
                    new Field("GroupKey","المجموعة","Group"){Span=12,ShowInList=true,Searchable=true,Required=true,
                        HintAr="مثال: home.features أو about.values أو careers.why_us"},
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    new Field("TextAr","النص (عربي)","Text (Arabic)",FieldKind.TextArea){Span=6,Pair="Text",Lang="ar"},
                    new Field("TextEn","النص (إنجليزي)","Text (English)",FieldKind.TextArea){Span=6,Pair="Text",Lang="en"},
                    Icon(), Img("ImagePath","الصورة","Image","cards"),
                    new Field("LinkUrl","الرابط","Link URL",FieldKind.Url){Span=4},
                    new Field("LinkTextAr","نص الرابط (عربي)","Link text (Arabic)"){Span=4,Pair="LinkText",Lang="ar"},
                    new Field("LinkTextEn","نص الرابط (إنجليزي)","Link text (English)"){Span=4,Pair="LinkText",Lang="en"},
                    new Field("AccentColor","اللون المميز","Accent color",FieldKind.Color){Span=4},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "stats", ClrType = typeof(StatCounter),
                TitleAr = "الأرقام والإحصائيات", TitleEn = "Stat counters",
                SingularAr = "رقم", SingularEn = "counter",
                Icon = "fa-solid fa-chart-simple", Section = "content", Order = 6,
                OrderBy = "GroupKey,SortOrder", GroupField = "GroupKey", Sortable = true,
                Fields = {
                    new Field("GroupKey","المجموعة","Group"){Span=6,ShowInList=true,Searchable=true},
                    new Field("LabelAr","التسمية (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("LabelEn","التسمية (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en",Searchable=true,Required=true},
                    new Field("Value","القيمة","Value",FieldKind.Decimal){Span=4,ShowInList=true,Required=true},
                    new Field("Prefix","بادئة","Prefix"){Span=4},
                    new Field("Suffix","لاحقة","Suffix"){Span=4},
                    new Field("Decimals","عدد الخانات العشرية","Decimals",FieldKind.Number){Span=4},
                    Icon(4),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "faq", ClrType = typeof(FaqItem),
                TitleAr = "الأسئلة الشائعة", TitleEn = "FAQ",
                SingularAr = "سؤال", SingularEn = "question",
                Icon = "fa-solid fa-circle-question", Section = "content", Order = 7,
                OrderBy = "GroupKey,SortOrder", GroupField = "GroupKey", Sortable = true,
                Fields = {
                    new Field("GroupKey","المجموعة","Group"){Span=12,ShowInList=true,Searchable=true},
                    new Field("QuestionAr","السؤال (عربي)","Question (Arabic)"){Span=6,Pair="Question",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("QuestionEn","السؤال (إنجليزي)","Question (English)"){Span=6,Pair="Question",Lang="en",Searchable=true,Required=true},
                    new Field("AnswerAr","الإجابة (عربي)","Answer (Arabic)",FieldKind.Html){Span=12,Pair="Answer",Lang="ar"},
                    new Field("AnswerEn","الإجابة (إنجليزي)","Answer (English)",FieldKind.Html){Span=12,Pair="Answer",Lang="en"},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "testimonials", ClrType = typeof(Testimonial),
                TitleAr = "آراء العملاء", TitleEn = "Testimonials",
                SingularAr = "رأي", SingularEn = "testimonial",
                Icon = "fa-solid fa-quote-left", Section = "content", Order = 8,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    new Field("RoleAr","المسمى (عربي)","Role (Arabic)"){Span=6,Pair="Role",Lang="ar",ShowInList=true},
                    new Field("RoleEn","المسمى (إنجليزي)","Role (English)"){Span=6,Pair="Role",Lang="en"},
                    new Field("QuoteAr","الاقتباس (عربي)","Quote (Arabic)",FieldKind.TextArea){Span=6,Pair="Quote",Lang="ar"},
                    new Field("QuoteEn","الاقتباس (إنجليزي)","Quote (English)",FieldKind.TextArea){Span=6,Pair="Quote",Lang="en"},
                    Img("AvatarPath","الصورة الشخصية","Avatar","team"),
                    new Field("Rating","التقييم (1-5)","Rating (1-5)",FieldKind.Number){Span=3,ShowInList=true},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "pages", ClrType = typeof(StaticPage),
                TitleAr = "الصفحات الثابتة", TitleEn = "Static pages",
                SingularAr = "صفحة", SingularEn = "page",
                Icon = "fa-solid fa-file-lines", Section = "content", Order = 9,
                OrderBy = "SortOrder",
                Fields = {
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    Img("HeroImagePath","صورة الغلاف","Hero image","pages"),
                    new Field("BodyAr","المحتوى (عربي)","Body (Arabic)",FieldKind.Html){Span=12,Pair="Body",Lang="ar"},
                    new Field("BodyEn","المحتوى (إنجليزي)","Body (English)",FieldKind.Html){Span=12,Pair="Body",Lang="en"},
                    Seo(), SeoD(),
                    Flag("IsPublished","منشور؟","Published?"),
                    Flag("ShowInFooter","في التذييل؟","In footer?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "menus", ClrType = typeof(Menu),
                TitleAr = "القوائم", TitleEn = "Menus",
                SingularAr = "قائمة", SingularEn = "menu",
                Icon = "fa-solid fa-bars", Section = "content", Order = 10,
                OrderBy = "SortOrder", ChildKey = "menu-items", ChildFk = "MenuId",
                Fields = {
                    new Field("MenuKey","مفتاح القائمة","Menu key"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true},
                    new Field("TitleEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Title",Lang="en"},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "menu-items", ClrType = typeof(MenuItem),
                TitleAr = "عناصر القوائم", TitleEn = "Menu items",
                SingularAr = "عنصر", SingularEn = "item",
                Icon = "fa-solid fa-list", Section = "content", Order = 11,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("MenuId","القائمة","Menu",typeof(Menu),6,true),
                    Look("ParentId","العنصر الأب","Parent item",typeof(MenuItem),6),
                    new Field("LabelAr","النص (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("LabelEn","النص (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en",Searchable=true,Required=true},
                    new Field("Url","الرابط","URL",FieldKind.Url){Span=6,ShowInList=true},
                    Icon(),
                    new Field("BadgeTextAr","الشارة (عربي)","Badge (Arabic)"){Span=6,Pair="BadgeText",Lang="ar"},
                    new Field("BadgeTextEn","الشارة (إنجليزي)","Badge (English)"){Span=6,Pair="BadgeText",Lang="en"},
                    Flag("OpenInNewTab","نافذة جديدة؟","New tab?",false),
                    Flag("IsHighlighted","مميّز؟","Highlighted?",false),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Sort(),
                }
            });

            // ═══════════════════ CATALOG ═══════════════════
            L.Add(new EntityDef
            {
                Key = "production-lines", ClrType = typeof(ProductionLine),
                TitleAr = "خطوط الإنتاج", TitleEn = "Production lines",
                SingularAr = "خط إنتاج", SingularEn = "production line",
                Icon = "fa-solid fa-industry", Section = "catalog", Order = 1,
                OrderBy = "SortOrder", Sortable = true, ChildKey = "line-specs", ChildFk = "ProductionLineId",
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    new Field("CapacityPerDay","الطاقة اليومية","Capacity / day",FieldKind.Number){Span=3,ShowInList=true},
                    new Field("CapacityUnitAr","الوحدة (عربي)","Unit (Arabic)"){Span=3,Pair="CapacityUnit",Lang="ar"},
                    new Field("CapacityUnitEn","الوحدة (إنجليزي)","Unit (English)"){Span=3,Pair="CapacityUnit",Lang="en"},
                    new Field("ShortDescAr","وصف مختصر (عربي)","Short description (Arabic)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="ar"},
                    new Field("ShortDescEn","وصف مختصر (إنجليزي)","Short description (English)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="en"},
                    new Field("DescAr","الوصف الكامل (عربي)","Description (Arabic)",FieldKind.Html){Span=12,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف الكامل (إنجليزي)","Description (English)",FieldKind.Html){Span=12,Pair="Desc",Lang="en"},
                    Img("ImagePath","الصورة","Image","lines"),
                    Img("IconPath","صورة الأيقونة","Icon image","lines"),
                    Icon(4), new Field("AccentColor","اللون","Accent color",FieldKind.Color){Span=4},
                    Seo(), SeoD(),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Flag("IsFeatured","مميّز؟","Featured?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "line-specs", ClrType = typeof(ProductionLineSpec),
                TitleAr = "مواصفات الخطوط", TitleEn = "Line specs",
                SingularAr = "مواصفة", SingularEn = "spec",
                Icon = "fa-solid fa-list-check", Section = "catalog", Order = 2,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("ProductionLineId","خط الإنتاج","Production line",typeof(ProductionLine),12,true),
                    new Field("LabelAr","التسمية (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar",ShowInList=true,Required=true},
                    new Field("LabelEn","التسمية (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en",Required=true},
                    new Field("ValueAr","القيمة (عربي)","Value (Arabic)"){Span=6,Pair="Value",Lang="ar",ShowInList=true},
                    new Field("ValueEn","القيمة (إنجليزي)","Value (English)"){Span=6,Pair="Value",Lang="en"},
                    Icon(4), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "product-categories", ClrType = typeof(ProductCategory),
                TitleAr = "تصنيفات المنتجات", TitleEn = "Product categories",
                SingularAr = "تصنيف", SingularEn = "category",
                Icon = "fa-solid fa-folder-tree", Section = "catalog", Order = 3,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    Look("ParentId","التصنيف الأب","Parent category",typeof(ProductCategory)),
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="en"},
                    Icon(), Img("ImagePath","الصورة","Image","categories"),
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "products", ClrType = typeof(Product),
                TitleAr = "المنتجات", TitleEn = "Products",
                SingularAr = "منتج", SingularEn = "product",
                Icon = "fa-solid fa-mobile-screen", Section = "catalog", Order = 4,
                OrderBy = "SortOrder", Sortable = true, ChildKey = "product-specs", ChildFk = "ProductId",
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    Look("CategoryId","التصنيف","Category",typeof(ProductCategory)),
                    Look("ProductionLineId","خط الإنتاج","Production line",typeof(ProductionLine)),
                    Look("BrandId","العلامة التجارية","Brand",typeof(Brand)),
                    new Field("Sku","كود المنتج","SKU"){Span=4,ShowInList=true,Searchable=true},
                    new Field("ModelNumber","رقم الموديل","Model number"){Span=4,Searchable=true},
                    new Field("ShortDescAr","وصف مختصر (عربي)","Short description (Arabic)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="ar"},
                    new Field("ShortDescEn","وصف مختصر (إنجليزي)","Short description (English)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="en"},
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.Html){Span=12,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.Html){Span=12,Pair="Desc",Lang="en"},
                    Img("MainImagePath","الصورة الرئيسية","Main image","products"),
                    new Field("DatasheetPath","ملف المواصفات","Datasheet",FieldKind.File){Span=6,Folder="datasheets"},
                    new Field("ExternalShopUrl","رابط المتجر","Shop URL",FieldKind.Url){Span=6},
                    new Field("Price","السعر","Price",FieldKind.Decimal){Span=3},
                    new Field("Currency","العملة","Currency"){Span=3},
                    Flag("ShowPrice","إظهار السعر؟","Show price?",false),
                    Flag("InStock","متوفر؟","In stock?"),
                    Seo(), SeoD(),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Flag("IsFeatured","مميّز؟","Featured?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "product-specs", ClrType = typeof(ProductSpec),
                TitleAr = "مواصفات المنتجات", TitleEn = "Product specs",
                SingularAr = "مواصفة", SingularEn = "spec",
                Icon = "fa-solid fa-list-check", Section = "catalog", Order = 5,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("ProductId","المنتج","Product",typeof(Product),12,true),
                    new Field("LabelAr","التسمية (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar",ShowInList=true,Required=true},
                    new Field("LabelEn","التسمية (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en",Required=true},
                    new Field("ValueAr","القيمة (عربي)","Value (Arabic)"){Span=6,Pair="Value",Lang="ar",ShowInList=true},
                    new Field("ValueEn","القيمة (إنجليزي)","Value (English)"){Span=6,Pair="Value",Lang="en"},
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "product-images", ClrType = typeof(ProductImage),
                TitleAr = "صور المنتجات", TitleEn = "Product images",
                SingularAr = "صورة", SingularEn = "image",
                Icon = "fa-solid fa-image", Section = "catalog", Order = 6,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("ProductId","المنتج","Product",typeof(Product),12,true),
                    new Field("ImagePath","الصورة","Image",FieldKind.Image){Span=12,Folder="products",ShowInList=true,Required=true},
                    new Field("AltAr","النص البديل (عربي)","Alt (Arabic)"){Span=6,Pair="Alt",Lang="ar"},
                    new Field("AltEn","النص البديل (إنجليزي)","Alt (English)"){Span=6,Pair="Alt",Lang="en"},
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "brands", ClrType = typeof(Brand),
                TitleAr = "العلامات التجارية", TitleEn = "Brands",
                SingularAr = "علامة", SingularEn = "brand",
                Icon = "fa-solid fa-copyright", Section = "catalog", Order = 7,
                OrderBy = "SortOrder", Sortable = true, GroupField = "BrandType",
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    Sel("BrandType","النوع","Type",BrandTypes),
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="en"},
                    Img("LogoPath","الشعار","Logo","brands"),
                    Img("LogoLightPath","الشعار (فاتح)","Logo (light)","brands"),
                    new Field("WebsiteUrl","الموقع","Website",FieldKind.Url){Span=6},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Flag("ShowInStrip","في الشريط؟","In strip?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "services", ClrType = typeof(ServiceItem),
                TitleAr = "الخدمات", TitleEn = "Services",
                SingularAr = "خدمة", SingularEn = "service",
                Icon = "fa-solid fa-screwdriver-wrench", Section = "catalog", Order = 8,
                OrderBy = "SortOrder", Sortable = true, ChildKey = "service-features", ChildFk = "ServiceItemId",
                Fields = {
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    new Field("ShortDescAr","وصف مختصر (عربي)","Short description (Arabic)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="ar"},
                    new Field("ShortDescEn","وصف مختصر (إنجليزي)","Short description (English)",FieldKind.TextArea){Span=6,Pair="ShortDesc",Lang="en"},
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.Html){Span=12,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.Html){Span=12,Pair="Desc",Lang="en"},
                    Icon(4), new Field("AccentColor","اللون","Accent color",FieldKind.Color){Span=4},
                    Img("ImagePath","الصورة","Image","services"),
                    Img("IconImagePath","صورة الأيقونة","Icon image","services"),
                    Seo(), SeoD(),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Flag("IsFeatured","مميّز؟","Featured?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "service-features", ClrType = typeof(ServiceFeature),
                TitleAr = "مميزات الخدمات", TitleEn = "Service features",
                SingularAr = "ميزة", SingularEn = "feature",
                Icon = "fa-solid fa-check", Section = "catalog", Order = 9,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("ServiceItemId","الخدمة","Service",typeof(ServiceItem),12,true),
                    new Field("TextAr","النص (عربي)","Text (Arabic)"){Span=6,Pair="Text",Lang="ar",ShowInList=true,Required=true},
                    new Field("TextEn","النص (إنجليزي)","Text (English)"){Span=6,Pair="Text",Lang="en",Required=true},
                    Icon(4), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "albums", ClrType = typeof(GalleryAlbum),
                TitleAr = "ألبومات المعرض", TitleEn = "Gallery albums",
                SingularAr = "ألبوم", SingularEn = "album",
                Icon = "fa-solid fa-photo-film", Section = "catalog", Order = 10,
                OrderBy = "SortOrder", Sortable = true, ChildKey = "album-images", ChildFk = "AlbumId",
                Fields = {
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    Img("CoverImagePath","صورة الغلاف","Cover image","gallery"),
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="en"},
                    Flag("IsPublished","منشور؟","Published?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "album-images", ClrType = typeof(GalleryImage),
                TitleAr = "صور المعرض", TitleEn = "Gallery images",
                SingularAr = "صورة", SingularEn = "image",
                Icon = "fa-solid fa-image", Section = "catalog", Order = 11,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("AlbumId","الألبوم","Album",typeof(GalleryAlbum),12,true),
                    new Field("ImagePath","الصورة","Image",FieldKind.Image){Span=12,Folder="gallery",ShowInList=true,Required=true},
                    new Field("CaptionAr","التعليق (عربي)","Caption (Arabic)"){Span=6,Pair="Caption",Lang="ar"},
                    new Field("CaptionEn","التعليق (إنجليزي)","Caption (English)"){Span=6,Pair="Caption",Lang="en"},
                    Sort(),
                }
            });

            // ═══════════════════ COMMUNITY (News / Events / Careers) ═══════════════════
            L.Add(new EntityDef
            {
                Key = "news-categories", ClrType = typeof(NewsCategory),
                TitleAr = "تصنيفات الأخبار", TitleEn = "News categories",
                SingularAr = "تصنيف", SingularEn = "category",
                Icon = "fa-solid fa-tags", Section = "community", Order = 1,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    Icon(3), new Field("Color","اللون","Color",FieldKind.Color){Span=3},
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "news", ClrType = typeof(NewsPost),
                TitleAr = "الأخبار", TitleEn = "News",
                SingularAr = "خبر", SingularEn = "post",
                Icon = "fa-solid fa-newspaper", Section = "community", Order = 2,
                OrderBy = "PublishedAt desc",
                Fields = {
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    Look("CategoryId","التصنيف","Category",typeof(NewsCategory)),
                    Img("CoverImagePath","صورة الغلاف","Cover image","news"),
                    new Field("AuthorName","الكاتب","Author"){Span=6,ShowInList=true},
                    new Field("CoverAltAr","بديل الغلاف (عربي)","Cover alt (Arabic)"){Span=6,Pair="CoverAlt",Lang="ar"},
                    new Field("CoverAltEn","بديل الغلاف (إنجليزي)","Cover alt (English)"){Span=6,Pair="CoverAlt",Lang="en"},
                    new Field("ExcerptAr","المقتطف (عربي)","Excerpt (Arabic)",FieldKind.TextArea){Span=6,Pair="Excerpt",Lang="ar"},
                    new Field("ExcerptEn","المقتطف (إنجليزي)","Excerpt (English)",FieldKind.TextArea){Span=6,Pair="Excerpt",Lang="en"},
                    new Field("BodyAr","المحتوى (عربي)","Body (Arabic)",FieldKind.Html){Span=12,Pair="Body",Lang="ar"},
                    new Field("BodyEn","المحتوى (إنجليزي)","Body (English)",FieldKind.Html){Span=12,Pair="Body",Lang="en"},
                    new Field("Tags","الوسوم (بفاصلة)","Tags (comma separated)"){Span=6,Searchable=true},
                    new Field("PublishedAt","تاريخ النشر","Published at",FieldKind.DateTime){Span=6,ShowInList=true},
                    Seo(), SeoD(),
                    Flag("IsPublished","منشور؟","Published?"),
                    Flag("IsFeatured","مميّز؟","Featured?"),
                    new Field("ViewCount","المشاهدات","Views",FieldKind.ReadOnly){Span=3,ShowInList=true},
                }
            });

            L.Add(new EntityDef
            {
                Key = "events", ClrType = typeof(Event),
                TitleAr = "الفعاليات", TitleEn = "Events",
                SingularAr = "فعالية", SingularEn = "event",
                Icon = "fa-solid fa-calendar-days", Section = "community", Order = 3,
                OrderBy = "StartDate desc", ChildKey = "event-images", ChildFk = "EventId",
                Fields = {
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    Sel("EventType","نوع الفعالية","Event type",EventTypes),
                    new Field("StartDate","تاريخ البداية","Start date",FieldKind.DateTime){Span=6,ShowInList=true},
                    new Field("EndDate","تاريخ النهاية","End date",FieldKind.DateTime){Span=6},
                    new Field("LocationAr","المكان (عربي)","Location (Arabic)"){Span=6,Pair="Location",Lang="ar",ShowInList=true},
                    new Field("LocationEn","المكان (إنجليزي)","Location (English)"){Span=6,Pair="Location",Lang="en"},
                    new Field("SummaryAr","الملخص (عربي)","Summary (Arabic)",FieldKind.TextArea){Span=6,Pair="Summary",Lang="ar"},
                    new Field("SummaryEn","الملخص (إنجليزي)","Summary (English)",FieldKind.TextArea){Span=6,Pair="Summary",Lang="en"},
                    new Field("DescAr","التفاصيل (عربي)","Description (Arabic)",FieldKind.Html){Span=12,Pair="Desc",Lang="ar"},
                    new Field("DescEn","التفاصيل (إنجليزي)","Description (English)",FieldKind.Html){Span=12,Pair="Desc",Lang="en"},
                    Img("CoverImagePath","صورة الغلاف","Cover image","events"),
                    new Field("VideoUrl","رابط الفيديو","Video URL",FieldKind.Url){Span=6},
                    new Field("MapUrl","رابط الخريطة","Map URL",FieldKind.Url){Span=6},
                    new Field("RegistrationUrl","رابط تسجيل خارجي","External registration URL",FieldKind.Url){Span=6},
                    Flag("AllowRegistration","السماح بالتسجيل؟","Allow registration?"),
                    new Field("Capacity","عدد المقاعد","Capacity",FieldKind.Number){Span=3},
                    Seo(), SeoD(),
                    Flag("IsPublished","منشور؟","Published?"),
                    Flag("IsFeatured","مميّز؟","Featured?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "event-images", ClrType = typeof(EventImage),
                TitleAr = "صور الفعاليات", TitleEn = "Event images",
                SingularAr = "صورة", SingularEn = "image",
                Icon = "fa-solid fa-image", Section = "community", Order = 4,
                OrderBy = "SortOrder", HideInNav = true, Sortable = true,
                Fields = {
                    Look("EventId","الفعالية","Event",typeof(Event),12,true),
                    new Field("ImagePath","الصورة","Image",FieldKind.Image){Span=12,Folder="events",ShowInList=true,Required=true},
                    new Field("CaptionAr","التعليق (عربي)","Caption (Arabic)"){Span=6,Pair="Caption",Lang="ar"},
                    new Field("CaptionEn","التعليق (إنجليزي)","Caption (English)"){Span=6,Pair="Caption",Lang="en"},
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "event-registrations", ClrType = typeof(EventRegistration),
                TitleAr = "تسجيلات الفعاليات", TitleEn = "Event registrations",
                SingularAr = "تسجيل", SingularEn = "registration",
                Icon = "fa-solid fa-user-check", Section = "inbox", Order = 4,
                OrderBy = "CreatedAt desc", CanCreate = false, HasUnread = true,
                Fields = {
                    Look("EventId","الفعالية","Event",typeof(Event),6,true),
                    new Field("FullName","الاسم","Full name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true},
                    new Field("Phone","الهاتف","Phone"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Company","الشركة","Company"){Span=6,Searchable=true},
                    new Field("Notes","ملاحظات","Notes",FieldKind.TextArea){Span=12},
                    Flag("IsConfirmed","مؤكّد؟","Confirmed?"),
                    Flag("IsRead","مقروء؟","Read?"),
                    Created(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "job-categories", ClrType = typeof(JobCategory),
                TitleAr = "أقسام الوظائف", TitleEn = "Job categories",
                SingularAr = "قسم", SingularEn = "category",
                Icon = "fa-solid fa-sitemap", Section = "community", Order = 5,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Slug("NameEn"),
                    Look("ParentId","القسم الأب","Parent",typeof(JobCategory)),
                    Icon(4),
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "jobs", ClrType = typeof(JobPosting),
                TitleAr = "الوظائف", TitleEn = "Jobs",
                SingularAr = "وظيفة", SingularEn = "job",
                Icon = "fa-solid fa-briefcase", Section = "community", Order = 6,
                OrderBy = "PostedAt desc",
                Fields = {
                    new Field("TitleAr","المسمى (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","المسمى (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    Slug(),
                    Look("CategoryId","القسم","Category",typeof(JobCategory),6,true),
                    Sel("JobType","نوع الدوام","Job type",JobTypes),
                    Sel("ExperienceLevel","المستوى","Experience level",JobLevels),
                    new Field("LocationAr","الموقع (عربي)","Location (Arabic)"){Span=6,Pair="Location",Lang="ar",ShowInList=true},
                    new Field("LocationEn","الموقع (إنجليزي)","Location (English)"){Span=6,Pair="Location",Lang="en"},
                    new Field("SalaryRange","نطاق الراتب","Salary range"){Span=4},
                    new Field("Vacancies","عدد الشواغر","Vacancies",FieldKind.Number){Span=4,ShowInList=true},
                    new Field("PostedAt","تاريخ النشر","Posted at",FieldKind.Date){Span=4,ShowInList=true},
                    new Field("ClosingDate","تاريخ الإغلاق","Closing date",FieldKind.Date){Span=4,ShowInList=true},
                    new Field("SummaryAr","الملخص (عربي)","Summary (Arabic)",FieldKind.TextArea){Span=6,Pair="Summary",Lang="ar"},
                    new Field("SummaryEn","الملخص (إنجليزي)","Summary (English)",FieldKind.TextArea){Span=6,Pair="Summary",Lang="en"},
                    new Field("DescAr","الوصف الوظيفي (عربي)","Description (Arabic)",FieldKind.Html){Span=12,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف الوظيفي (إنجليزي)","Description (English)",FieldKind.Html){Span=12,Pair="Desc",Lang="en"},
                    new Field("RequirementsAr","المتطلبات (عربي)","Requirements (Arabic)",FieldKind.Html){Span=12,Pair="Requirements",Lang="ar"},
                    new Field("RequirementsEn","المتطلبات (إنجليزي)","Requirements (English)",FieldKind.Html){Span=12,Pair="Requirements",Lang="en"},
                    new Field("BenefitsAr","المزايا (عربي)","Benefits (Arabic)",FieldKind.Html){Span=12,Pair="Benefits",Lang="ar"},
                    new Field("BenefitsEn","المزايا (إنجليزي)","Benefits (English)",FieldKind.Html){Span=12,Pair="Benefits",Lang="en"},
                    Seo(), SeoD(),
                    Flag("IsActive","مفعّل؟","Active?"),
                    Flag("IsUrgent","عاجل؟","Urgent?"),
                    Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "applications", ClrType = typeof(JobApplication),
                TitleAr = "طلبات التوظيف", TitleEn = "Job applications",
                SingularAr = "طلب", SingularEn = "application",
                Icon = "fa-solid fa-file-signature", Section = "inbox", Order = 3,
                OrderBy = "CreatedAt desc", CanCreate = false, HasUnread = true,
                Fields = {
                    Look("JobId","الوظيفة","Job",typeof(JobPosting),6,true),
                    new Field("FullName","الاسم","Full name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true},
                    new Field("Phone","الهاتف","Phone"){Span=6,ShowInList=true,Searchable=true},
                    new Field("City","المدينة","City"){Span=4,Searchable=true},
                    new Field("YearsOfExperience","سنوات الخبرة","Years of experience",FieldKind.Number){Span=4,ShowInList=true},
                    new Field("LinkedInUrl","لينكدإن","LinkedIn",FieldKind.Url){Span=4},
                    new Field("CvPath","السيرة الذاتية","CV",FieldKind.File){Span=6,Folder="cv",ShowInList=true},
                    Sel("Status","الحالة","Status",AppStatus),
                    new Field("CoverLetter","خطاب التقديم","Cover letter",FieldKind.TextArea){Span=12},
                    new Field("AdminNotes","ملاحظات الإدارة","Admin notes",FieldKind.TextArea){Span=12},
                    Flag("IsRead","مقروء؟","Read?"),
                    Created(),
                }
            });

            // ═══════════════════ COMPANY ═══════════════════
            L.Add(new EntityDef
            {
                Key = "milestones", ClrType = typeof(Milestone),
                TitleAr = "المحطات الزمنية", TitleEn = "Milestones",
                SingularAr = "محطة", SingularEn = "milestone",
                Icon = "fa-solid fa-timeline", Section = "company", Order = 1,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("Year","السنة","Year"){Span=4,ShowInList=true,Searchable=true},
                    new Field("MonthAr","الشهر (عربي)","Month (Arabic)"){Span=4,Pair="Month",Lang="ar"},
                    new Field("MonthEn","الشهر (إنجليزي)","Month (English)"){Span=4,Pair="Month",Lang="en"},
                    new Field("TitleAr","العنوان (عربي)","Title (Arabic)"){Span=6,Pair="Title",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("TitleEn","العنوان (إنجليزي)","Title (English)"){Span=6,Pair="Title",Lang="en",Searchable=true,Required=true},
                    new Field("TextAr","النص (عربي)","Text (Arabic)",FieldKind.TextArea){Span=6,Pair="Text",Lang="ar"},
                    new Field("TextEn","النص (إنجليزي)","Text (English)",FieldKind.TextArea){Span=6,Pair="Text",Lang="en"},
                    Img("ImagePath","الصورة","Image","company"), Icon(),
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "team", ClrType = typeof(TeamMember),
                TitleAr = "فريق العمل", TitleEn = "Team",
                SingularAr = "عضو", SingularEn = "member",
                Icon = "fa-solid fa-users", Section = "company", Order = 2,
                OrderBy = "SortOrder", Sortable = true, GroupField = "GroupKey",
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    new Field("RoleAr","المنصب (عربي)","Role (Arabic)"){Span=6,Pair="Role",Lang="ar",ShowInList=true},
                    new Field("RoleEn","المنصب (إنجليزي)","Role (English)"){Span=6,Pair="Role",Lang="en"},
                    new Field("BioAr","نبذة (عربي)","Bio (Arabic)",FieldKind.TextArea){Span=6,Pair="Bio",Lang="ar"},
                    new Field("BioEn","نبذة (إنجليزي)","Bio (English)",FieldKind.TextArea){Span=6,Pair="Bio",Lang="en"},
                    Img("PhotoPath","الصورة","Photo","team"),
                    new Field("GroupKey","المجموعة","Group"){Span=6,ShowInList=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=4},
                    new Field("LinkedInUrl","لينكدإن","LinkedIn",FieldKind.Url){Span=4},
                    new Field("TwitterUrl","تويتر","Twitter",FieldKind.Url){Span=4},
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "certifications", ClrType = typeof(Certification),
                TitleAr = "الشهادات", TitleEn = "Certifications",
                SingularAr = "شهادة", SingularEn = "certification",
                Icon = "fa-solid fa-certificate", Section = "company", Order = 3,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    new Field("DescAr","الوصف (عربي)","Description (Arabic)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="ar"},
                    new Field("DescEn","الوصف (إنجليزي)","Description (English)",FieldKind.TextArea){Span=6,Pair="Desc",Lang="en"},
                    new Field("IssuedBy","جهة الإصدار","Issued by"){Span=6,ShowInList=true,Searchable=true},
                    new Field("CertNumber","رقم الشهادة","Certificate number"){Span=6,Searchable=true},
                    new Field("IssuedAt","تاريخ الإصدار","Issued at",FieldKind.Date){Span=4,ShowInList=true},
                    new Field("ExpiresAt","تاريخ الانتهاء","Expires at",FieldKind.Date){Span=4,ShowInList=true},
                    Img("LogoPath","الشعار","Logo","certs"),
                    new Field("DocumentPath","ملف الشهادة","Document",FieldKind.File){Span=6,Folder="certs"},
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "locations", ClrType = typeof(SiteLocation),
                TitleAr = "الفروع والمواقع", TitleEn = "Locations",
                SingularAr = "موقع", SingularEn = "location",
                Icon = "fa-solid fa-location-dot", Section = "company", Order = 4,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("NameAr","الاسم (عربي)","Name (Arabic)"){Span=6,Pair="Name",Lang="ar",ShowInList=true,Searchable=true,Required=true},
                    new Field("NameEn","الاسم (إنجليزي)","Name (English)"){Span=6,Pair="Name",Lang="en",Searchable=true,Required=true},
                    Sel("LocationType","النوع","Type",LocationTypes),
                    new Field("AddressAr","العنوان (عربي)","Address (Arabic)",FieldKind.TextArea){Span=6,Pair="Address",Lang="ar",ShowInList=true},
                    new Field("AddressEn","العنوان (إنجليزي)","Address (English)",FieldKind.TextArea){Span=6,Pair="Address",Lang="en"},
                    new Field("Phone","الهاتف","Phone"){Span=4,ShowInList=true},
                    new Field("Phone2","هاتف ٢","Phone 2"){Span=4},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=4,ShowInList=true},
                    new Field("WorkingHoursAr","ساعات العمل (عربي)","Working hours (Arabic)"){Span=6,Pair="WorkingHours",Lang="ar"},
                    new Field("WorkingHoursEn","ساعات العمل (إنجليزي)","Working hours (English)"){Span=6,Pair="WorkingHours",Lang="en"},
                    new Field("Latitude","خط العرض","Latitude",FieldKind.Decimal){Span=4},
                    new Field("Longitude","خط الطول","Longitude",FieldKind.Decimal){Span=4},
                    new Field("MapZoom","تقريب الخريطة","Map zoom",FieldKind.Number){Span=4},
                    Img("ImagePath","الصورة","Image","company"),
                    Flag("IsPrimary","رئيسي؟","Primary?"),
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "social", ClrType = typeof(SocialLink),
                TitleAr = "روابط التواصل", TitleEn = "Social links",
                SingularAr = "رابط", SingularEn = "link",
                Icon = "fa-solid fa-share-nodes", Section = "company", Order = 5,
                OrderBy = "SortOrder", Sortable = true,
                Fields = {
                    new Field("Platform","المنصة","Platform"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("Url","الرابط","URL",FieldKind.Url){Span=6,ShowInList=true,Required=true},
                    Icon(4), new Field("Color","اللون","Color",FieldKind.Color){Span=4},
                    new Field("LabelAr","التسمية (عربي)","Label (Arabic)"){Span=6,Pair="Label",Lang="ar"},
                    new Field("LabelEn","التسمية (إنجليزي)","Label (English)"){Span=6,Pair="Label",Lang="en"},
                    Flag("ShowInHeader","في الترويسة؟","In header?"),
                    Flag("ShowInFooter","في التذييل؟","In footer?"),
                    Flag("IsActive","مفعّل؟","Active?"), Sort(),
                }
            });

            // ═══════════════════ INBOX ═══════════════════
            L.Add(new EntityDef
            {
                Key = "messages", ClrType = typeof(ContactMessage),
                TitleAr = "رسائل التواصل", TitleEn = "Contact messages",
                SingularAr = "رسالة", SingularEn = "message",
                Icon = "fa-solid fa-envelope", Section = "inbox", Order = 1,
                OrderBy = "CreatedAt desc", CanCreate = false, HasUnread = true,
                Fields = {
                    new Field("FullName","الاسم","Full name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true},
                    new Field("Phone","الهاتف","Phone"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Company","الشركة","Company"){Span=6,Searchable=true},
                    new Field("Subject","الموضوع","Subject"){Span=12,ShowInList=true,Searchable=true},
                    new Field("Message","الرسالة","Message",FieldKind.TextArea){Span=12,Searchable=true},
                    new Field("AdminReply","الرد","Reply",FieldKind.TextArea){Span=12},
                    Flag("IsRead","مقروء؟","Read?"),
                    Flag("IsStarred","مميّز؟","Starred?"),
                    Flag("IsArchived","مؤرشف؟","Archived?"),
                    Created(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "quotes", ClrType = typeof(QuoteRequest),
                TitleAr = "طلبات عروض الأسعار", TitleEn = "Quote requests",
                SingularAr = "طلب", SingularEn = "request",
                Icon = "fa-solid fa-file-invoice-dollar", Section = "inbox", Order = 2,
                OrderBy = "CreatedAt desc", CanCreate = false, HasUnread = true,
                Fields = {
                    new Field("FullName","الاسم","Full name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true},
                    new Field("Phone","الهاتف","Phone"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Company","الشركة","Company"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Country","الدولة","Country"){Span=4},
                    Look("ProductionLineId","خط الإنتاج","Production line",typeof(ProductionLine),4),
                    new Field("ProductType","نوع المنتج","Product type"){Span=4,Searchable=true},
                    new Field("Quantity","الكمية","Quantity",FieldKind.Number){Span=4,ShowInList=true},
                    new Field("TargetBudget","الميزانية","Budget"){Span=4},
                    new Field("Timeline","الجدول الزمني","Timeline"){Span=4},
                    new Field("Details","التفاصيل","Details",FieldKind.TextArea){Span=12,Searchable=true},
                    new Field("AttachmentPath","المرفق","Attachment",FieldKind.File){Span=6,Folder="quotes"},
                    Sel("Status","الحالة","Status",RequestStatus),
                    new Field("AdminNotes","ملاحظات الإدارة","Admin notes",FieldKind.TextArea){Span=12},
                    Flag("IsRead","مقروء؟","Read?"),
                    Created(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "partners", ClrType = typeof(PartnerRequest),
                TitleAr = "طلبات الشراكة", TitleEn = "Partnership requests",
                SingularAr = "طلب", SingularEn = "request",
                Icon = "fa-solid fa-handshake", Section = "inbox", Order = 5,
                OrderBy = "CreatedAt desc", CanCreate = false, HasUnread = true,
                Fields = {
                    new Field("CompanyName","اسم الشركة","Company name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("ContactName","اسم المسؤول","Contact name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true},
                    new Field("Phone","الهاتف","Phone"){Span=6,ShowInList=true},
                    new Field("Website","الموقع","Website",FieldKind.Url){Span=6},
                    new Field("Country","الدولة","Country"){Span=6},
                    new Field("PartnershipType","نوع الشراكة","Partnership type"){Span=6,ShowInList=true},
                    new Field("Message","الرسالة","Message",FieldKind.TextArea){Span=12,Searchable=true},
                    new Field("AttachmentPath","المرفق","Attachment",FieldKind.File){Span=6,Folder="partners"},
                    Sel("Status","الحالة","Status",RequestStatus),
                    new Field("AdminNotes","ملاحظات الإدارة","Admin notes",FieldKind.TextArea){Span=12},
                    Flag("IsRead","مقروء؟","Read?"),
                    Created(),
                }
            });

            L.Add(new EntityDef
            {
                Key = "subscribers", ClrType = typeof(NewsletterSubscriber),
                TitleAr = "المشتركون في النشرة", TitleEn = "Newsletter subscribers",
                SingularAr = "مشترك", SingularEn = "subscriber",
                Icon = "fa-solid fa-paper-plane", Section = "inbox", Order = 6,
                OrderBy = "CreatedAt desc", CanCreate = false,
                Fields = {
                    new Field("Email","البريد","Email",FieldKind.Email){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("Name","الاسم","Name"){Span=6,ShowInList=true,Searchable=true},
                    new Field("Language","اللغة","Language"){Span=4,ShowInList=true},
                    Flag("IsActive","مفعّل؟","Active?"),
                    Created(),
                }
            });

            // ═══════════════════ SYSTEM ═══════════════════
            L.Add(new EntityDef
            {
                Key = "redirects", ClrType = typeof(UrlRedirect),
                TitleAr = "إعادة التوجيه", TitleEn = "URL redirects",
                SingularAr = "توجيه", SingularEn = "redirect",
                Icon = "fa-solid fa-arrow-right-arrow-left", Section = "system", Order = 2,
                OrderBy = "Id desc",
                Fields = {
                    new Field("FromPath","من المسار","From path"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    new Field("ToPath","إلى المسار","To path"){Span=6,ShowInList=true,Searchable=true,Required=true},
                    Flag("IsPermanent","دائم (301)؟","Permanent (301)?"),
                    Flag("IsActive","مفعّل؟","Active?"),
                    new Field("HitCount","عدد الزيارات","Hits",FieldKind.ReadOnly){Span=3,ShowInList=true},
                }
            });

            L.Add(new EntityDef
            {
                Key = "audit", ClrType = typeof(AuditLog),
                TitleAr = "سجل النشاط", TitleEn = "Audit log",
                SingularAr = "سجل", SingularEn = "entry",
                Icon = "fa-solid fa-clock-rotate-left", Section = "system", Order = 4,
                OrderBy = "CreatedAt desc", CanCreate = false, CanEdit = false,
                Fields = {
                    new Field("UserName","المستخدم","User",FieldKind.ReadOnly){Span=4,ShowInList=true,Searchable=true},
                    new Field("Action","الإجراء","Action",FieldKind.ReadOnly){Span=4,ShowInList=true,Searchable=true},
                    new Field("EntityName","الجدول","Entity",FieldKind.ReadOnly){Span=4,ShowInList=true,Searchable=true},
                    new Field("EntityId","المعرّف","Entity ID",FieldKind.ReadOnly){Span=4,ShowInList=true},
                    new Field("Summary","الملخص","Summary",FieldKind.ReadOnly){Span=12,ShowInList=true,Searchable=true},
                    new Field("IpAddress","عنوان IP","IP address",FieldKind.ReadOnly){Span=4},
                    Created(),
                }
            });

            return L;
        }

        // ── reflection helpers used by controller + views ─────────────────────
        private static readonly Dictionary<Type, PropertyInfo[]> _propCache = new();
        public static PropertyInfo[] Props(Type t)
        {
            lock (_propCache)
            {
                if (!_propCache.TryGetValue(t, out var p))
                { p = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); _propCache[t] = p; }
                return p;
            }
        }
        public static PropertyInfo? Prop(Type t, string name) =>
            Props(t).FirstOrDefault(p => p.Name == name);

        public static object? GetValue(object entity, string name) =>
            Prop(entity.GetType(), name)?.GetValue(entity);

        /// <summary>Best-effort human label for any entity instance (used by Lookup dropdowns).</summary>
        public static string Label(object? entity, bool ar)
        {
            if (entity is null) return "";
            var t = entity.GetType();
            string[] order = ar
                ? new[] { "TitleAr", "NameAr", "LabelAr", "QuestionAr", "Key", "MenuKey", "Platform", "FullName", "CompanyName", "Email", "Slug" }
                : new[] { "TitleEn", "NameEn", "LabelEn", "QuestionEn", "Key", "MenuKey", "Platform", "FullName", "CompanyName", "Email", "Slug" };
            foreach (var n in order)
            {
                var v = Prop(t, n)?.GetValue(entity) as string;
                if (!string.IsNullOrWhiteSpace(v)) return v!;
            }
            return "#" + (Prop(t, "Id")?.GetValue(entity)?.ToString() ?? "?");
        }
    }
}
