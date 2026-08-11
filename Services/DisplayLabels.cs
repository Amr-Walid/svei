namespace SVEI.Web.Services
{
    /// <summary>Bilingual labels + icons for the fixed Event type enum.</summary>
    public static class EventLabels
    {
        public static readonly string[] Types =
            { "exhibition", "conference", "training", "ceremony", "visit", "launch", "other" };

        public static string Type(string? key, bool isAr) => (key ?? "other") switch
        {
            "exhibition" => isAr ? "معرض"            : "Exhibition",
            "conference" => isAr ? "مؤتمر"           : "Conference",
            "training"   => isAr ? "تدريب"           : "Training",
            "ceremony"   => isAr ? "حفل"             : "Ceremony",
            "visit"      => isAr ? "زيارة"           : "Visit",
            "launch"     => isAr ? "إطلاق منتج"      : "Launch",
            _            => isAr ? "فعالية"          : "Event"
        };

        public static string Icon(string? key) => (key ?? "other") switch
        {
            "exhibition" => "fa-solid fa-store",
            "conference" => "fa-solid fa-microphone-lines",
            "training"   => "fa-solid fa-chalkboard-user",
            "ceremony"   => "fa-solid fa-award",
            "visit"      => "fa-solid fa-people-group",
            "launch"     => "fa-solid fa-rocket",
            _            => "fa-solid fa-calendar-days"
        };

        public static string Status(string? key, bool isAr) => (key ?? "past") switch
        {
            "upcoming" => isAr ? "قادمة" : "Upcoming",
            "ongoing"  => isAr ? "جارية" : "Ongoing",
            _          => isAr ? "منتهية" : "Past"
        };
    }

    /// <summary>Bilingual labels for job type / experience level.</summary>
    public static class JobLabels
    {
        public static readonly string[] Types =
            { "Full-time", "Part-time", "Shift", "Contract", "Internship", "Remote" };

        public static string Type(string? key, bool isAr) => (key ?? "Full-time") switch
        {
            "Full-time"  => isAr ? "دوام كامل"  : "Full-time",
            "Part-time"  => isAr ? "دوام جزئي"  : "Part-time",
            "Shift"      => isAr ? "ورديات"     : "Shift",
            "Contract"   => isAr ? "عقد مؤقت"   : "Contract",
            "Internship" => isAr ? "تدريب"      : "Internship",
            "Remote"     => isAr ? "عن بُعد"    : "Remote",
            _            => key ?? ""
        };

        public static string Level(string? key, bool isAr) => (key ?? "") switch
        {
            "Entry"   => isAr ? "مبتدئ"       : "Entry level",
            "Junior"  => isAr ? "خبرة بسيطة"  : "Junior",
            "Mid"     => isAr ? "خبرة متوسطة" : "Mid level",
            "Senior"  => isAr ? "خبرة عالية"  : "Senior",
            "Manager" => isAr ? "إداري"       : "Manager",
            _         => key ?? ""
        };

        public static string Icon(string? key) => (key ?? "Full-time") switch
        {
            "Remote"     => "fa-solid fa-house-laptop",
            "Internship" => "fa-solid fa-graduation-cap",
            "Shift"      => "fa-solid fa-clock-rotate-left",
            "Contract"   => "fa-solid fa-file-signature",
            "Part-time"  => "fa-regular fa-clock",
            _            => "fa-solid fa-briefcase"
        };
    }
}
