using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using StudyWithChatgpt.Services;

namespace StudyWithChatgpt.Controllers
{
    [Route("language")]
    public class LanguageController : Controller
    {
        [HttpGet("set")]
        public IActionResult Set(string? culture, string? returnUrl)
        {
            var normalized = CultureConfig.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase)
                ? culture!
                : CultureConfig.DefaultCulture;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(normalized)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax,
                    Secure = Request.IsHttps
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
