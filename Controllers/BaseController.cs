using Microsoft.AspNetCore.Mvc;
using BTTemplate.Models;
using System.Text.Json;

namespace BTTemplate.Controllers
{
    /// <summary>
    /// Base controller — provides session helper and auth redirect.
    /// All application controllers should inherit from this.
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly IConfiguration _config;

        public BaseController(IConfiguration config)
        {
            _config = config;
        }

        // ── Read UserSession from HttpContext.Session ──────────────────
        protected UserSessionModel? GetSession()
        {
            var json = HttpContext.Session.GetString("UserSession");
            if (string.IsNullOrEmpty(json)) return null;
            return JsonSerializer.Deserialize<UserSessionModel>(json);
        }

        // ── Redirect to SSO if not logged in ──────────────────────────
        protected string AuthenUrl =>
            _config["TBCorApiServices:AuthenUrl"] ?? "/";

        // ── Return session or redirect (use in action methods) ─────────
        protected UserSessionModel? RequireLogin(out IActionResult? redirect)
        {
            var session = GetSession();
            redirect = session == null ? Redirect(AuthenUrl) : null;
            return session;
        }

        // ── Return JSON error response ─────────────────────────────────
        protected IActionResult JsonError(string message, int statusCode = 400)
            => StatusCode(statusCode, new { ok = false, msg = message });

        protected IActionResult JsonOk(object? data = null, string msg = "")
            => Json(new { ok = true, msg, data });
    }
}
