# BTTemplate — Bernina Thailand Web Application Template

ASP.NET Core 8 MVC template with SSO Login + Session Management built-in.

## Tech Stack
- ASP.NET Core 8 MVC (net8.0)
- Bootstrap 5.3 + Bootstrap Icons
- SQL Server (Microsoft.Data.SqlClient 5.2.2)
- BT SSO (btauthen.berninathailand.com)
- IIS Self-Contained Publish

## Quick Start — New Project Checklist

1. **Rename**: BTTemplate → YourProjectName (csproj, namespace, Constants.cs)
2. **Configure appsettings.json**: AuthenUrl, URLSITE, DB connection string
3. **Customize UserSessionModel**: Add your role flags
4. **Customize LoadUserRoles()**: Map DB roles to session flags
5. **Add your Controllers/Views**
6. **Publish**: `dotnet publish -c Release -o ./publish`

## SSO Flow
```
Visit /Dashboards/Index (no session)
  → Redirect to btauthen.berninathailand.com
  → SSO authenticates user
  → Redirect back with ?id=...&user=...&email=...&fname=...&depart=...
  → Save UserSession + load roles from DB
  → Show Dashboard
```

## Session Helper (use in every Controller)
```csharp
private UserSessionModel? GetSession()
{
    var json = HttpContext.Session.GetString("UserSession");
    if (string.IsNullOrEmpty(json)) return null;
    return JsonSerializer.Deserialize<UserSessionModel>(json);
}
```

## Known Tips
- SQL collation mismatch → add COLLATE THAI_CI_AS to JOIN
- SqlClient error on old Windows Server → use SqlClient 5.2.2
- @ in Razor CSS → use @@keyframes, @@media
- IIS 500.31 → publish Self-Contained or install .NET Hosting Bundle
