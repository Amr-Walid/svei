using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SVEI.Web.Controllers.Admin
{
    /// <summary>Switches the admin UI language via the shared svei_lang cookie.</summary>
    [Authorize(Roles = "Admin,Editor")]
    [Route("Admin/Lang")]
    public class LangController : Controller
    {
        [HttpGet("{code}")]
        public IActionResult Set(string code)
        {
            var lang = code == "en" ? "en" : "ar";

            Response.Cookies.Append("svei_lang", lang, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                HttpOnly = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            var back = Request.Headers.Referer.ToString();
            return Redirect(!string.IsNullOrWhiteSpace(back) && Url.IsLocalUrl(back) ? back : "/Admin");
        }
    }
}
