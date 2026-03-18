using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using BTTemplate.Models;
using System.Text.Json;

namespace BTTemplate.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly AppSettingsModel _settings;

        public DashboardsController(IConfiguration config,
                                    IOptions<AppSettingsModel> settings)
        {
            _config   = config;
            _settings = settings.Value;
        }

        // ═════════════════════════════════════════════════════════════════
        //  GET /Dashboards/Index
        //
        //  SSO redirects back here with query params:
        //    ?id=GUID&user=DOMAIN\SAM&email=...&fname=...&depart=...
        //
        //  Flow:
        //    1. No session + no SSO params → redirect to SSO login
        //    2. SSO params present → create session → show dashboard
        //    3. Session exists (revisit) → show dashboard directly
        // ═════════════════════════════════════════════════════════════════
        public IActionResult Index(string? id,   string? user,
                                   string? email, string? fname,
                                   string? depart)
        {
            var existingSession = HttpContext.Session.GetString("UserSession");
            var authenUrl       = _config["TBCorApiServices:AuthenUrl"] ?? "/";

            // No session and no SSO params → go to SSO
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(existingSession))
                return Redirect(authenUrl);

            // SSO callback — create new session
            if (!string.IsNullOrEmpty(id))
            {
                var samAcc = UserSessionModel.ParseSamAcc(user ?? "");

                var session = new UserSessionModel
                {
                    Id         = id,
                    UserLogon  = user   ?? "",
                    SamAcc     = samAcc,
                    Email      = email  ?? "",
                    FullName   = fname  ?? "",
                    Department = depart ?? "",
                    IsUser     = true
                };

                // Load additional roles from DB (optional)
                LoadUserRoles(session, samAcc);

                HttpContext.Session.SetString("UserSession",
                    JsonSerializer.Serialize(session));
            }

            return View();
        }

        // ─────────────────────────────────────────────────────────────────
        //  GET /Dashboards/Logout
        // ─────────────────────────────────────────────────────────────────
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            var authenUrl = _config["TBCorApiServices:AuthenUrl"] ?? "/";
            return Redirect(authenUrl);
        }

        // ═════════════════════════════════════════════════════════════════
        //  LoadUserRoles — query roles/permissions from DB
        //
        //  Extend this method to populate role flags on UserSessionModel.
        //  By default it does nothing — all users get IsUser = true only.
        //
        //  Example: query a TBUserFunction or TBUserRole table
        // ═════════════════════════════════════════════════════════════════
        private void LoadUserRoles(UserSessionModel session, string samAcc)
        {
            try
            {
                var connStr = _config.GetConnectionString("MainConn");
                if (string.IsNullOrEmpty(connStr)) return;

                using var conn = new SqlConnection(connStr);
                conn.Open();

                // ── EXAMPLE: read from a role table ──────────────────
                // Uncomment and adapt to your actual schema:
                //
                // const string sql = @"
                //     SELECT RoleCode
                //     FROM   dbo.TBUserRole
                //     WHERE  UserLogon = @sam AND IsActive = 1";
                //
                // using var cmd = new SqlCommand(sql, conn);
                // cmd.Parameters.AddWithValue("@sam", samAcc);
                // using var reader = cmd.ExecuteReader();
                // while (reader.Read())
                // {
                //     switch (reader.GetString(0))
                //     {
                //         case "ADMIN":   session.IsAdmin   = true; break;
                //         // Add more cases as needed
                //     }
                // }

                // Fallback: every user is at minimum IsUser = true
                session.IsUser = true;
            }
            catch
            {
                // DB unavailable → grant basic user access
                session.IsUser = true;
            }
        }
    }
}
