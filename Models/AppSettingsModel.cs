namespace BTTemplate.Models
{
    // ── Binds to appsettings.json → TBCorApiServices ──────────────────
    public class AppSettingsModel
    {
        public string AppName     { get; set; } = "BTTemplate";
        public string AppVersion  { get; set; } = "1.0";
        public string AuthenUrl   { get; set; } = "";
        public string URLSITE     { get; set; } = "";
        public string EmailSender { get; set; } = "";
        public string MailForm    { get; set; } = "";
        public string MailDebug   { get; set; } = "1";
        public long   MaxFileSize { get; set; } = 2000000;
        public int    Page20      { get; set; } = 20;
    }

    // ── User session — stored in HttpContext.Session as JSON ───────────
    public class UserSessionModel
    {
        // From SSO callback query params
        public string Id         { get; set; } = "";   // SSO GUID
        public string UserLogon  { get; set; } = "";   // DOMAIN\SAM
        public string SamAcc     { get; set; } = "";   // SAM only (lowercase)
        public string Email      { get; set; } = "";
        public string FullName   { get; set; } = "";
        public string Department { get; set; } = "";

        // ── Role flags ────────────────────────────────────────────────
        // Populate in DashboardsController.LoadUserRoles()
        // by querying your own role/permission table
        public bool IsAdmin { get; set; }
        public bool IsUser  { get; set; } = true;  // every logged-in user

        // ── Add more roles here as needed ─────────────────────────────
        // public bool IsManager  { get; set; }
        // public bool IsApprover { get; set; }

        // ── Parse "DOMAIN\SAM" → "sam" (lowercase) ───────────────────
        public static string ParseSamAcc(string userLogon)
        {
            if (string.IsNullOrEmpty(userLogon)) return "";
            var parts = userLogon.Split('\\');
            return (parts.Length > 1 ? parts[1] : parts[0]).ToLower();
        }
    }
}
