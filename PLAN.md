# SVEI — Silicon Valley for Electronic Industries
## خطة العمل التفصيلية — موقع المصنع + لوحة تحكم كاملة

> **الوضع الحالي:** الموقع القديم `https://svei.tech` قالب HTML ثابت (Megan Industrial Template) — بدون باك اند، بدون CMS، محتوى مكتوب داخل الـ HTML.
> **الهدف:** موقع ASP.NET Core MVC عصري بالكامل + لوحة تحكم عربية تتحكم في **كل حرف وكل صورة** في الموقع.

---

# الجزء الأول — تحليل الموقع القديم

## 1.1 معلومات الشركة المستخرجة

| البند | القيمة |
|---|---|
| **الاسم الكامل** | Silicon Valley for Electronic Industries (SVEI) |
| **الاسم المختصر** | SVEI / Silicon Valley |
| **سنة التأسيس** | 2023 |
| **النشاط** | تصنيع إلكترونيات — هواتف ذكية وإكسسوارات موبايل للعلامات الصينية |
| **الموقع** | مصر، السويس — العين السخنة، **TEDA — المنطقة الاقتصادية لقناة السويس** |
| **الإحداثيات** | `29.6725549, 32.30928` |
| **البريد** | info@svei.tech |
| **الهاتف / الدعم** | (+20) 109 888 7606 — دعم 24/7 |
| **مواعيد العمل** | السبت – الخميس: 8:00 – 17:00 (إجازة نهاية الأسبوع) |
| **المتجر الإلكتروني** | https://yallatager.com/ar/Silicon-Valley-Product.html (خارجي) |
| **فيديو تعريفي** | https://www.youtube.com/watch?v=VngRX0yj_iE |
| **السوشيال** | facebook / twitter / vimeo / linkedin / skype — **كلها روابط وهمية `//facebook.com`** ⚠️ محتاجين الروابط الحقيقية |

## 1.2 الهوية البصرية

**اللوجو:** مربع أحمر على اليسار فيه `SV` / `Ei` بالأبيض (شبكة 2×2)، وعلى اليمين "Silicon Valley" بخط سميك هندسي + "Electronic Industries" بخط أرفع.

```
اللون الأساسي (أحمر اللوجو)  #E31B23
الأسود / النص الداكن          #1F2022  →  #0A0D14
الرمادي الثانوي               #5A5C5E
الأصفر (accent القالب القديم) #FDB714  ←  لون الكروت والأزرار في القالب
السماوي (توهج الرندر 3D)      #22C6E8  ←  من صورة الشريحة في الهيرو
الخلفية الفاتحة               #F5F6F8
```

> ⚠️ **مشكلة في القالب القديم:** الأصفر (`#FDB714`) لون افتراضي من قالب "Megan" الصناعي — مالوش علاقة بهوية SVEI (أحمر/أسود). في التصميم الجديد **هنستبدله بالأحمر + السماوي التقني** اللي بيطلع من صورة الشريحة نفسها، ونخلي الأصفر accent ثانوي بس.

## 1.3 خريطة أقسام الصفحة الرئيسية (القديمة)

| # | القسم | المحتوى الحالي | ملاحظة |
|---|---|---|---|
| 0 | Preloader | GIF | نستبدله بأنيميشن SVG للوجو |
| 1 | Top bar | إيميل + مواعيد العمل + مبدّل لغة (EN/FR/DE **وهمي — مش شغال**) | ⚠️ محتاجين **عربي/إنجليزي** بدل FR/DE |
| 2 | Navbar | لوجو + HOME / CONTACT / SHOP + تليفون + 5 أيقونات سوشيال | 3 صفحات بس! فقير جداً |
| 3 | Hero Slider | Revolution Slider — صورتين: رندر شريحة SVEI + صورة المصنع الحقيقي. النص: "Empowering Tommorow." / "Empowering Lives, Products and Employees" + زرار "Our Products" | ⚠️ خطأ إملائي `Tommorow` |
| 4 | About / Intro | "Innovating Tomorrow's Technology, Today" + فقرة ترحيب + زرار فيديو يوتيوب | |
| 5 | 3 كروت | Foundation / Our Mission / Our Vision + 3 صور | الصور **stock عامة** (لحام، مبنى، عامل) — مش من المصنع |
| 6 | Industry Slider | 4 تبويبات: Cell Phones / Power Storage / Wearables / Audio Peripherals — كل واحد وصف + خلفية | ⭐ أهم قسم — ده خطوط الإنتاج |
| 7 | Brands Slider | 5 علامات: Oraimo / Infinix / Itel / Gtide / UniTronics + روابط مواقعهم | |
| 8 | Counters | 2000 موبايل/يوم · 1000 باور بانك/يوم · 1500 ساعة & TWS/يوم + صورة خط الإنتاج | ⭐ أرقام قوية |
| 9 | خريطة جوجل | إحداثيات العين السخنة | مفتاح API مكشوف في الـ HTML ⚠️ |
| 10 | شريط دعم | إيميل + تليفون (خلفية صفراء) | |
| 11 | Footer | لوجو + وصف + سوشيال + Privacy / Terms / Site Map | الروابط دي **مش موجودة أصلاً** ⚠️ |

## 1.4 صفحة CONTACT
خريطة → 3 كروت (Location / Call Center / Email Us) → فورم (اسم، إيميل، تليفون، رسالة) بيبعت لـ `assets/php/mail.php`.

## 1.5 المشاكل الحرجة في الموقع القديم

| # | المشكلة | الخطورة |
|---|---|---|
| 1 | **مفيش باك اند خالص** — كل حرف hard-coded في HTML | 🔴 حرجة |
| 2 | مفتاح Google Maps API **مكشوف** في الكود | 🔴 أمنية |
| 3 | فورم التواصل PHP — الرسايل **مش بتتخزن في أي مكان** | 🔴 حرجة |
| 4 | مبدّل اللغة **ديكور فقط** (EN/FR/DE مش شغالين) — **ومفيش عربي!** | 🔴 حرجة |
| 5 | كل روابط السوشيال وهمية (`//facebook.com`) | 🟠 عالية |
| 6 | Privacy / Terms / Site Map — روابط `#` فاضية | 🟠 عالية |
| 7 | Revolution Slider (مدفوع، تقيل، jQuery) + jQuery + Bootstrap 4 + 6 مكتبات أيقونات | 🟠 أداء |
| 8 | صورة هيرو واحدة **664 KB** JPG بدون تحسين | 🟠 أداء |
| 9 | صفحة CONTACT لسه فيها `<title>` بتاع القالب: "Megan - Industrial Template" | 🟡 SEO |
| 10 | `<html lang="zxx">` + `<meta description>` فاضي + مفيش OG tags / sitemap / robots | 🟡 SEO |
| 11 | خطأ إملائي "Tommorow" في الهيرو | 🟡 |
| 12 | مفيش صفحات: منتجات، خدمات، شهادات، أخبار، وظائف، معرض صور | 🟠 |
| 13 | صور Foundation/Mission/Vision **stock** مش من المصنع | 🟡 |

---

# الجزء الثاني — معمارية الحل الجديد

## 2.1 قرار المعمارية

**مشروع منفصل تماماً** عن Uni-Group — قاعدة بيانات مستقلة، admin مستقل، ريبو مستقلة.
**السبب:** شركتين مختلفتين (هولدنج تجارية vs مصنع)، جمهور مختلف، دورة حياة نشر مختلفة.
**لكن** نعيد استخدام نفس الـ **patterns** المجربة من Uni-Group (Ar/En، Slug، Upload helper، Admin RTL) عشان نمشي بسرعة.

## 2.2 الـ Stack

```
Backend    ASP.NET Core MVC 8.0  (net8.0)  ·  namespace SVEI.Web
ORM        EF Core 8.0.x  —  SQLite (dev)  /  SQL Server (prod)  — switch من appsettings
Auth       ASP.NET Identity  —  أدوار: Admin, Editor
Views      Razor (.cshtml)  —  server-rendered  +  جزر تفاعلية Vanilla JS
Front-end  ❌ لا React — بدل ما نستنى bundle build.
           ✅ CSS حديث (Grid/Flex/clamp/CSS vars) + Vanilla JS (IntersectionObserver, Web Animations)
Slider     Swiper 11 (self-hosted, 40KB) بدل Revolution Slider التقيل
Map        Leaflet + OpenStreetMap  — مجاني، **بدون مفتاح API مكشوف**
Icons      Lucide SVG inline (بدل 6 مكتبات أيقونات)
Fonts      Inter (EN) + IBM Plex Sans Arabic (AR) — self-hosted
Images     WebP + <picture> + lazy loading + responsive srcset
Editor     Quill.js أو TinyMCE للمحتوى الغني في الأدمن
```

**ليه مش React؟** موقع تعريفي محتواه من قاعدة بيانات → Razor server-rendered = SEO أفضل بكتير + أسرع + مفيش خطوة build. (اتعلمنا من Uni-Group إن الـ JSX bundle كان بيتعب في كل تعديل.)

## 2.3 هيكل المجلدات

```
svei.web/
├── SVEI.Web.csproj
├── Program.cs
├── appsettings.json / appsettings.Development.json
├── Models/
│   ├── Identity/         AppUser.cs
│   ├── Content/          HeroSlide, PageSection, InfoCard, StatCounter
│   ├── Catalog/          ProductCategory, Product, ProductImage, ProductSpec, Brand
│   ├── Company/          Certification, Timeline, TeamMember, GalleryAlbum, GalleryImage
│   ├── Community/        NewsPost, JobCategory, JobPosting, JobApplication
│   ├── Inbox/            ContactMessage, QuoteRequest, NewsletterSub
│   └── Settings/         SiteSetting, Menu, MenuItem, SiteLocation, SocialLink, MediaFile
├── Data/
│   ├── AppDbContext.cs
│   ├── Seed/             SeedRunner.cs  +  seed-*.json  (محتوى الموقع القديم)
│   └── Migrations/
├── Services/
│   ├── ISettingsService     كاش لإعدادات الموقع
│   ├── IMediaService        رفع + تحويل WebP + thumbnails
│   ├── ISlugService
│   ├── IEmailService        SMTP — إشعار عند رسالة/طلب سعر جديد
│   └── ILocalizationService لغة الطلب الحالي
├── Controllers/
│   ├── HomeController, AboutController, ProductsController, ServicesController
│   ├── NewsController, CareersController, GalleryController, ContactController
│   ├── QuoteController, SitemapController
│   └── Admin/  (12 كنترولر — تحت)
├── ViewComponents/
│   ├── HeaderVC, FooterVC, HeroSliderVC, StatCounterVC, BrandStripVC, CtaBarVC
├── Views/
│   ├── Shared/  _Layout, _AdminLayout, _Partials/
│   ├── Home, About, Products, News, Careers, Gallery, Contact
│   └── Admin/   (شاشة لكل موديول)
├── wwwroot/
│   ├── css/     tokens.css, base.css, components.css, sections.css, admin.css
│   ├── js/      site.js, slider.js, map.js, admin.js
│   ├── lib/     swiper/, leaflet/, quill/
│   ├── fonts/   inter/, ibm-plex-arabic/
│   ├── img/     brand/, hero/, sections/, brands/, icons/
│   └── uploads/ (gitignored) products/ news/ gallery/ certs/ cvs/
└── Resources/   SharedResource.ar.resx / .en.resx  (نصوص الواجهة الثابتة)
```

---

# الجزء الثالث — نموذج البيانات (Data Model)

> **قاعدة ذهبية:** كل حقل نصي مزدوج `Ar` + `En`. كل كيان فيه `IsActive/IsPublished` + `SortOrder` + `CreatedAt` + `UpdatedAt`.

## المجموعة 1 — محتوى الصفحات (يخلي كل حرف قابل للتعديل)

### `SiteSetting`  — مفتاح/قيمة عام
```
Id · Key(unique) · ValueAr · ValueEn · Group · Type(text|html|image|bool|number|color|url) · Label · SortOrder
```
**الجروبات:** `general` (اسم الشركة، الشعار، سنة التأسيس) · `contact` (إيميل، تليفونات، عنوان، مواعيد) · `social` (روابط) · `seo` (عناوين، أوصاف، OG) · `appearance` (ألوان، لوجو، فافيكون) · `integrations` (رابط المتجر، رابط فيديو، GA ID)

### `HeroSlide` — سلايدر الهيرو (كنترول كامل)
```
Id · TitleAr/En · SubtitleAr/En · ButtonTextAr/En · ButtonUrl
ImagePath · MobileImagePath · VideoUrl · OverlayOpacity · TextPosition(left|center|right)
IsActive · SortOrder
```

### `PageSection` — أي قسم نصي في أي صفحة
```
Id · PageKey(home|about|products|contact...) · SectionKey(intro|mission|cta...)
KickerAr/En · TitleAr/En · SubtitleAr/En · BodyAr/En(HTML)
ImagePath · VideoUrl · ButtonTextAr/En · ButtonUrl · BackgroundType · IsVisible · SortOrder
```
> ⭐ ده اللي بيخلي الأدمن يقدر يغيّر أي عنوان أو فقرة في أي صفحة بدون مبرمج.

### `InfoCard` — كروت (Foundation/Mission/Vision وأي مجموعة كروت)
```
Id · GroupKey · TitleAr/En · BodyAr/En · Icon(lucide) · ImagePath · LinkUrl · IsActive · SortOrder
```

### `StatCounter` — عدادات الإنتاج
```
Id · Value(2000) · Suffix(+/K) · LabelAr/En · DescAr/En · Icon · IsActive · SortOrder
```

## المجموعة 2 — المنتجات وخطوط الإنتاج ⭐ القلب

### `ProductCategory` — خطوط الإنتاج (Cell Phones / Power Storage / Wearables / Audio)
```
Id · NameAr/En · Slug(unique) · ShortDescAr/En · FullDescAr/En(HTML)
Icon · CoverImagePath · BannerImagePath · ParentId(nullable→ أقسام فرعية)
DailyCapacity · CapacityUnitAr/En · MetaTitle/MetaDesc · IsActive · SortOrder
```

### `Product` — منتج فعلي
```
Id · CategoryId · BrandId(nullable) · NameAr/En · Slug(unique) · ModelNumber · SKU
ShortDescAr/En · FullDescAr/En(HTML) · CoverImagePath · DatasheetPath(PDF)
IsFeatured · IsActive · SortOrder · ViewCount · ExternalShopUrl · MetaTitle/MetaDesc
```

### `ProductImage`
```
Id · ProductId · ImagePath · AltAr/En · IsCover · SortOrder
```

### `ProductSpec` — المواصفات الفنية
```
Id · ProductId · KeyAr/En(الشاشة) · ValueAr/En(6.7" AMOLED) · Group · SortOrder
```

### `Brand` — العلامات اللي بنصنّعها
```
Id · NameAr/En · Slug · LogoPath · LogoDarkPath · WebsiteUrl
DescAr/En · Since · IsFeatured · IsActive · SortOrder
```
> Seed: Oraimo · Infinix · Itel · Gtide · UniTronics

## المجموعة 3 — الشركة (صفحات جديدة مش موجودة في القديم)

### `Certification` — شهادات الجودة ⭐ مهمة جداً لمصنع
```
Id · NameAr/En · IssuerAr/En · CertNumber · ImagePath · PdfPath
IssuedDate · ExpiryDate · DescAr/En · IsActive · SortOrder
```
> ISO 9001 · ISO 14001 · شهادات NTRA · موافقات جهاز تنظيم الاتصالات...

### `TimelineEvent` — رحلة المصنع
```
Id · Year(2023) · TitleAr/En · DescAr/En · ImagePath · Icon · IsActive · SortOrder
```

### `TeamMember` — القيادة
```
Id · NameAr/En · PositionAr/En · BioAr/En · PhotoPath · LinkedInUrl · Email · IsActive · SortOrder
```

### `GalleryAlbum` + `GalleryImage` — معرض المصنع ⭐
```
Album: Id · TitleAr/En · Slug · DescAr/En · CoverImagePath · Category(factory|line|events|products) · IsPublished · SortOrder
Image: Id · AlbumId · ImagePath · CaptionAr/En · SortOrder
```

### `ProductionLine` — خطوط الإنتاج بالتفصيل (اختياري - المرحلة 3)
```
Id · NameAr/En · DescAr/En · Capacity · Equipment · ImagePath · VideoUrl · IsActive · SortOrder
```

## المجموعة 4 — المجتمع

### `NewsPost` — أخبار (نسخة من Uni-Group)
```
Id · TitleAr/En · Slug · ExcerptAr/En · BodyAr/En(HTML) · CoverImagePath
Category · AuthorName · IsPublished · IsFeatured · ViewCount · PublishedAt · MetaTitle/MetaDesc
```

### `JobCategory` / `JobPosting` / `JobApplication` — وظائف
> نفس موديل Uni-Group بالظبط (مجرّب وشغال) + رفع CV

## المجموعة 5 — صندوق الوارد (كل ما يصل من الزوار)

### `ContactMessage`
```
Id · Name · Email · Phone · Company · Subject · Message · Source · IpAddress · IsRead · IsArchived · AdminNote · CreatedAt
```

### `QuoteRequest` — طلب عرض سعر / تصنيع ⭐ الأهم تجارياً لمصنع
```
Id · CompanyName · ContactName · JobTitle · Email · Phone · Country · Website
ProductCategoryId · ProductType · EstimatedQuantity · TargetPrice · Timeline
Requirements · AttachmentPath
Status(new|contacted|quoted|won|lost) · AdminNote · IsRead · CreatedAt
```

### `NewsletterSub`
```
Id · Email · Lang · IsConfirmed · IsActive · CreatedAt
```

## المجموعة 6 — إعدادات وبنية

### `SiteLocation` — نقاط الخريطة
```
Id · NameAr/En · Lat · Lng · Type(factory|office|warehouse) · AddressAr/En · Phone · Email · IsActive · SortOrder
```
> Seed: المصنع — العين السخنة `29.6725549, 32.30928`

### `SocialLink`
```
Id · Platform · Url · Icon · IsActive · SortOrder
```

### `Menu` + `MenuItem` — القوائم قابلة للتعديل من الأدمن ⭐
```
Menu:     Id · Key(main|footer-1|footer-2|topbar) · NameAr/En
MenuItem: Id · MenuId · ParentId · LabelAr/En · Url · Icon · OpenInNewTab · IsActive · SortOrder
```

### `MediaFile` — مكتبة وسائط مركزية
```
Id · FileName · Path · ThumbPath · MimeType · SizeBytes · Width · Height · AltAr/En · Folder · UploadedAt
```

### `AuditLog`
```
Id · UserId · UserName · Action · EntityType · EntityId · Changes(JSON) · IpAddress · CreatedAt
```

**الإجمالي: 28 جدول**

---

# الجزء الرابع — خريطة الموقع الجديد (Public)

| المسار | الصفحة | مصدر البيانات |
|---|---|---|
| `/` | الرئيسية | HeroSlide, PageSection, InfoCard, ProductCategory, Brand, StatCounter |
| `/about` | من نحن | PageSection, TimelineEvent, InfoCard, TeamMember |
| `/about/certifications` | الشهادات والاعتمادات | Certification |
| `/products` | خطوط الإنتاج | ProductCategory |
| `/products/{slug}` | خط إنتاج + منتجاته | ProductCategory + Product |
| `/product/{slug}` | صفحة منتج | Product + Images + Specs |
| `/brands` | العلامات اللي بنصنّعها | Brand |
| `/services` | خدمات التصنيع (OEM/ODM/EMS) | PageSection, InfoCard |
| `/gallery` | معرض المصنع | GalleryAlbum |
| `/gallery/{slug}` | ألبوم | GalleryImage (lightbox) |
| `/news` · `/news/{slug}` | الأخبار | NewsPost |
| `/careers` · `/careers/{slug}` | الوظائف + التقديم | JobPosting |
| `/contact` | تواصل + خريطة + فورم | SiteLocation, SiteSetting |
| `/quote` | **اطلب عرض سعر** ⭐ | QuoteRequest |
| `/privacy` · `/terms` | صفحات قانونية | PageSection |
| `/sitemap.xml` · `/robots.txt` | SEO | مولّد ديناميكياً |

**اللغة:** `/ar/...` و `/en/...` — العربي هو الافتراضي (شركة مصرية) مع تبديل حقيقي شغّال (مش زي القديم).

---

# الجزء الخامس — لوحة التحكم (نطاق التحكم)

> **المبدأ: صفر hard-coding. أي شيء يظهر للزائر = يتحكم فيه الأدمن.**

## 5.1 الشاشات

| # | الموديول | القدرات |
|---|---|---|
| 1 | **Dashboard** | إحصائيات، رسائل جديدة، طلبات أسعار، آخر النشاطات، رسم بياني للزيارات |
| 2 | **Hero Slider** | إضافة/حذف/ترتيب سحب-وإفلات، صورة desktop+mobile، نص، زرار، شفافية |
| 3 | **أقسام الصفحات** | تعديل كل عنوان/فقرة/صورة في كل صفحة — Rich text bilingual |
| 4 | **الكروت والعدادات** | Foundation/Mission/Vision + عدادات الإنتاج |
| 5 | **خطوط الإنتاج** | CRUD + أقسام فرعية + بانر + طاقة إنتاجية |
| 6 | **المنتجات** | CRUD + معرض صور + مواصفات فنية + PDF datasheet + مميز |
| 7 | **العلامات التجارية** | CRUD + لوجو فاتح/غامق + رابط |
| 8 | **الشهادات** | CRUD + صورة + PDF + تواريخ (تنبيه قبل الانتهاء) |
| 9 | **المعرض** | ألبومات + رفع متعدد + سحب للترتيب |
| 10 | **الأخبار** | CRUD + محرر غني + جدولة نشر + SEO |
| 11 | **الوظائف** | أقسام + وظائف + طلبات التقديم + تحميل CVs |
| 12 | **صندوق الوارد** | رسائل + طلبات أسعار (مع pipeline: جديد→تم التواصل→عرض مُرسل→فوز/خسارة) + تصدير Excel |
| 13 | **القوائم** | بناء قائمة النافبار والفوتر بالسحب والإفلات |
| 14 | **مكتبة الوسائط** | كل الصور في مكان واحد + بحث + حذف الغير مستخدم |
| 15 | **الإعدادات** | تبويبات: عام · تواصل · سوشيال · SEO · مظهر (ألوان!) · تكاملات |
| 16 | **المستخدمون** | Admin / Editor + سجل التدقيق |

## 5.2 مميزات الأدمن
- عربي RTL كامل (نفس نمط Uni-Group المجرّب) + Dark/Light
- **معاينة مباشرة** — زر "شوف الصفحة" جنب كل قسم
- سحب وإفلات للترتيب في كل الجداول
- رفع صور بالسحب + قص + تحويل WebP تلقائي
- محرر غني (Quill) للحقول HTML
- Autosave للمسودات
- بحث شامل + فلاتر + Pagination
- تنبيهات بادچ للجديد (زي Uni-Group)

---

# الجزء السادس — نظام التصميم الجديد

## 6.1 لوحة الألوان — مستخرجة من الهوية الحقيقية

```css
:root{
  /* الأساسي — أحمر اللوجو */
  --sv-red:        #E31B23;
  --sv-red-dark:   #B8151C;
  --sv-red-soft:   rgba(227,27,35,.08);

  /* التقني — من توهج الشريحة في صورة الهيرو */
  --sv-cyan:       #22C6E8;
  --sv-cyan-deep:  #0E7C99;
  --sv-cyan-soft:  rgba(34,198,232,.10);

  /* الحيادي */
  --sv-ink:        #0A0D14;   /* الأسود الأساسي */
  --sv-ink-2:      #1F2022;
  --sv-slate:      #5A5C5E;
  --sv-line:       rgba(10,13,20,.10);
  --sv-bg:         #FFFFFF;
  --sv-bg-alt:     #F5F6F8;
  --sv-bg-dark:    #0A0D14;

  /* accent ثانوي (بقايا الأصفر) */
  --sv-amber:      #FDB714;

  /* تدرجات مميزة */
  --sv-grad-tech:  linear-gradient(135deg,#0A0D14 0%,#12233F 55%,#0E7C99 100%);
  --sv-grad-red:   linear-gradient(120deg,#E31B23 0%,#FF5A3C 100%);
}
```

## 6.2 اتجاه التصميم — **"Precision Tech"**

| العنصر | القرار |
|---|---|
| **الشخصية** | دقة صناعية + توهج تقني. مساحات بيضاء واسعة، خطوط حادة، تفاصيل دقيقة كأنها PCB |
| **الخطوط** | EN: Inter (700/800 للعناوين) · AR: IBM Plex Sans Arabic. عناوين `clamp()` responsive |
| **الشبكة** | 12 عمود، حاوية 1320px، حواف `clamp(20px,5vw,80px)` |
| **الزوايا** | 4px–8px بس — حادة وصناعية (مش دائرية) |
| **الظلال** | خفيفة جداً `0 1px 3px` + `0 12px 40px` عند hover |
| **الحركة** | IntersectionObserver reveal، عدادات تتحرك عند الظهور، parallax خفيف. **`prefers-reduced-motion` محترم** |
| **اللمسة المميزة** | ⭐ نمط **دوائر ومسارات PCB** بالـ SVG كخلفية خفيفة في الأقسام + توهج سماوي عند hover على الكروت — يربط التصميم بصورة الشريحة في اللوجو |
| **الصور** | WebP + `<picture>` + srcset + lazy + blur placeholder |
| **الهيرو** | فيديو خط الإنتاج (أو صورة) + طبقة تدرج داكن + عنوان ضخم + عدادات مصغرة تحت |
| **الوضع الداكن** | اختياري — CSS vars جاهزة من الأول |

## 6.3 مكتبة المكونات
`Button` (primary أحمر / ghost / link مع سهم) · `Card` (صورة + محتوى + توهج) · `StatCounter` · `SectionHeader` (kicker + title + rule أحمر) · `Tabs` (خطوط الإنتاج) · `Accordion` (FAQ) · `Lightbox` · `Breadcrumb` · `Pagination` · `Form controls` · `Toast` · `Badge`

---

# الجزء السابع — التنفيذ على مراحل

## 🔹 المرحلة 0 — التجهيز (يوم 1)
- [x] تحليل الموقع القديم كاملاً (index.html 75KB + contact.html + كل الأصول)
- [x] تثبيت .NET 8 SDK — **8.0.423**
- [x] تثبيت dotnet-ef CLI — **8.0.29**
- [x] تحميل مكتبات الفرونت self-hosted → `_svei_lib/` (Swiper 11, Leaflet 1.9.4, GLightbox 3.3.1, SortableJS 1.15.6, Quill 2.0.3)
- [x] تحميل الخطوط self-hosted → `_svei_lib/fonts/` (Inter + IBM Plex Sans Arabic, 24 face, 436KB)
- [x] سحب أصول الموقع القديم + تحويل WebP → `_svei_assets/` (توفير 63% في الهيرو)
- [ ] إنشاء `SVEI.Web` + هيكل المجلدات
- [ ] نقل `_svei_lib` و `_svei_assets` جوه `wwwroot`
- [ ] Git init + الدفع للريبو الجديدة ← **مستني الرابط منك**

### ✅ البيئة جاهزة ومتحقَّق منها
```
dotnet SDK   8.0.423        dotnet-ef  8.0.29
node         v22.23.2       npm        10.9.8
python       3.13.13        Pillow     12.2.0   (تحويل WebP)
ImageMagick  7.1.1-43       git-lfs    3.6.1
مساحة فاضية  19 GB
```
> ملاحظة: الـ SDK متثبت في `$HOME/.dotnet` و`DOTNET_ROOT` + `PATH` متسجلين في `~/.bashrc`.

## 🔹 المرحلة 1 — الأساس (أيام 2-4)
- [ ] كل الـ 28 موديل + `AppDbContext` + Migration أولى
- [ ] Identity + الأدوار + seed للأدمن
- [ ] `SeedRunner` — يحقن **كل محتوى الموقع القديم** في القاعدة
- [ ] الخدمات: Settings(cached) · Media(WebP) · Slug · Email · Localization
- [ ] Localization middleware (ar/en) + ملفات resx

## 🔹 المرحلة 2 — نظام التصميم (أيام 5-6)
- [ ] `tokens.css` + `base.css` + `components.css`
- [ ] `_Layout` جديد: topbar + navbar (لوجو، قائمة من DB، تبديل لغة حقيقي، CTA "اطلب عرض سعر") + footer
- [ ] ViewComponents: Header, Footer, HeroSlider, StatCounter, BrandStrip
- [ ] نمط PCB بالـ SVG + مكتبة الأنيميشن

## 🔹 المرحلة 3 — الصفحات العامة (أيام 7-11)
- [ ] الرئيسية (كل أقسامها من DB)
- [ ] من نحن + الشهادات
- [ ] خطوط الإنتاج + المنتجات + صفحة المنتج
- [ ] العلامات + الخدمات
- [ ] المعرض + Lightbox
- [ ] الأخبار + الوظائف
- [ ] تواصل + خريطة Leaflet + **اطلب عرض سعر**
- [ ] صفحات قانونية + 404 + sitemap.xml + robots.txt

## 🔹 المرحلة 4 — لوحة التحكم (أيام 12-18)
- [ ] لايوت الأدمن + الدخول + الداشبورد
- [ ] الـ 16 موديول (CRUD + سحب وإفلات + رفع)
- [ ] مكتبة الوسائط + المحرر الغني
- [ ] Pipeline طلبات الأسعار + تصدير Excel
- [ ] بناء القوائم + الإعدادات + سجل التدقيق

## 🔹 المرحلة 5 — الصقل (أيام 19-21)
- [ ] SEO: OG/Twitter cards · JSON-LD (Organization + Product) · canonical · hreflang
- [ ] الأداء: WebP, lazy, minify, cache headers → هدف Lighthouse **90+**
- [ ] الأمان: CSP, HSTS, antiforgery, rate limiting, input sanitization, **إخفاء أي مفاتيح**
- [ ] الوصولية: WCAG AA, تنقل بالكيبورد, ARIA, تباين ألوان
- [ ] اختبار responsive (320 → 2560px) + اختبار RTL كامل
- [ ] تعليمات النشر

---

# الجزء الثامن — المكتبات المطلوبة

## NuGet
```
Microsoft.EntityFrameworkCore.SqlServer        8.0.x   قاعدة الإنتاج
Microsoft.EntityFrameworkCore.Sqlite           8.0.x   قاعدة التطوير
Microsoft.EntityFrameworkCore.Design           8.0.x   migrations
Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.x
SixLabors.ImageSharp                           3.x     تحويل WebP + thumbnails
ClosedXML                                      0.104   تصدير Excel
HtmlSanitizer                                  8.x     تنظيف مخرجات المحرر  🔒
Serilog.AspNetCore                             8.x     اللوجات
AspNetCoreRateLimit                            5.x     حماية الفورمات  🔒
```

## Front-end (self-hosted — بدون CDN)
```
Swiper 11        سلايدرات        ~40KB
Leaflet 1.9      خريطة           ~42KB   (بديل مجاني لجوجل — مفيش مفتاح مكشوف)
Quill 2          محرر (أدمن فقط) ~45KB
SortableJS       سحب وإفلات      ~12KB
GLightbox        معرض الصور      ~10KB
Lucide           أيقونات SVG     inline
Inter + IBM Plex Sans Arabic     خطوط self-hosted
```
**إجمالي JS للزائر: ~105KB** مقابل **~800KB+** في القالب القديم (jQuery + Revolution + Bootstrap + 6 مكتبات أيقونات).

---

# الجزء التاسع — نقل الأصول

| الأصل | المصدر | الوجهة |
|---|---|---|
| اللوجو (غامق/فاتح) | `assets/img/logo/*.png` | `wwwroot/img/brand/` + **إعادة رسم SVG** |
| صورة رندر الشريحة | `slider/one/home_00_01.jpg` (664KB) | ضغط → WebP → `img/hero/` |
| صورة خط الإنتاج | `slider/one/home_00_15.jpg` | ⭐ **أهم صورة** — حقيقية من المصنع → WebP |
| لوجوهات العلامات ×5 | `img/projects/1-5.jpg` | `img/brands/` (يفضّل PNG شفاف من مواقعهم) |
| أيقونات العدادات ×3 | `img/icons/*-White.png` | → Lucide SVG |
| صور Foundation/Mission/Vision | `img/banner/feature1-3.jpg` | ⚠️ **stock — نستبدلها بصور المصنع الحقيقية** |
| الفافيكون | `img/favicon.png` | → SVG + ICO |

---

# الجزء العاشر — ❓ محتاج منك

## 🔴 حرج — يوقّف الشغل
1. **رابط الريبو الجديدة** (لسه الريموت الحالي بتاع uni.com)
2. **صور حقيقية من المصنع** — الأهم على الإطلاق. خطوط الإنتاج، الغرف النظيفة، معدات SMT، الفريق، المبنى من بره، منطقة الجودة، المخازن
3. **المحتوى العربي** — الموقع القديم إنجليزي بس. عايزين الترجمة العربية لكل النصوص (أو أكتبها أنا كمسودة وتراجعها)

## 🟠 مهم
4. **الشهادات** — ISO 9001؟ 14001؟ موافقات NTRA؟ (صور/PDF) — دي بتبني ثقة ضخمة لمصنع
5. **روابط السوشيال الحقيقية** (Facebook / LinkedIn / YouTube / Instagram)
6. **بيانات المصنع:** المساحة (م²) · عدد الموظفين · عدد خطوط SMT · الطاقة السنوية
7. **قائمة المنتجات** — موديلات فعلية لكل خط إنتاج + مواصفات + صور

## 🟡 يخلي الموقع أحسن
8. **رحلة المصنع** — إنجازات 2023 → 2026
9. **القيادة** — أسماء وصور ومناصب
10. **قصص نجاح / عملاء** — شهادات العملاء
11. **خدمات التصنيع** — بتقدموا OEM؟ ODM؟ EMS كامل؟ تجميع بس؟
12. **الفيديو** — الفيديو الحالي `VngRX0yj_iE` لسه صالح؟

## ⚙️ قرارات محتاج ردك عليها
| # | السؤال | ترشيحي |
|---|---|---|
| 1 | العربي ولا الإنجليزي هو الافتراضي؟ | **العربي** (شركة مصرية) مع تبديل سهل |
| 2 | متجر داخلي ولا نفضل رابط yallatager؟ | **رابط خارجي** دلوقتي + كتالوج داخلي بـ"اطلب عرض سعر" |
| 3 | قاعدة الإنتاج SQL Server ولا PostgreSQL؟ | **SQL Server** (زي Uni-Group) |
| 4 | الاستضافة فين؟ | يحدد إعدادات النشر |
| 5 | نجيب صور AI مؤقتة لحد ما تبعت الحقيقية؟ | **آه** — عشان نشتغل بدون توقف |

---

# ملحق — مقارنة سريعة

| | القديم | الجديد |
|---|---|---|
| الصفحات | 2 | 16+ |
| المحتوى | Hard-coded | 100% من DB |
| اللغات | EN فقط (مبدّل وهمي) | AR + EN حقيقي |
| لوحة تحكم | ❌ | ✅ 16 موديول |
| الرسائل | تضيع | تتخزن + إشعار إيميل |
| طلبات الأسعار | ❌ | ✅ مع pipeline |
| JS للزائر | ~800KB | ~105KB |
| الخريطة | Google (مفتاح مكشوف) | Leaflet (مجاني وآمن) |
| SEO | ضعيف جداً | كامل |
| الجوال | Bootstrap افتراضي | mobile-first |

---

*آخر تحديث: 2026-08-11 · تحليل مبني على قراءة كاملة لـ `svei.tech/index.html` (75KB) و `contact.html` + فحص كل الأصول*
