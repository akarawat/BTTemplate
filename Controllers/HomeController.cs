using Microsoft.AspNetCore.Mvc;
using BTTemplate.Models;

namespace BTTemplate.Controllers
{
    /// <summary>
    /// Example controller showing how to use BaseController.
    /// Rename / replace with your actual feature controllers.
    /// </summary>
    public class HomeController : BaseController
    {
        public HomeController(IConfiguration config) : base(config) { }

        // ── Public landing page (no auth required) ─────────────────────
        public IActionResult Index()
        {
            // If already logged in, go to dashboard
            if (GetSession() != null)
                return RedirectToAction("Index", "Dashboards");

            return View();
        }

        // ── Example protected page ─────────────────────────────────────
        public IActionResult Protected()
        {
            var session = RequireLogin(out var redirect);
            if (redirect != null) return redirect;

            // Use session.FullName, session.Department, etc.
            ViewBag.UserName = session!.FullName;
            return View();
        }

        // ── Example: Admin-only page ───────────────────────────────────
        public IActionResult AdminOnly()
        {
            var session = RequireLogin(out var redirect);
            if (redirect != null) return redirect;

            if (!session!.IsAdmin)
                return RedirectToAction("Index", "Dashboards");

            return View();
        }
    }
}
