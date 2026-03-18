using BTTemplate.Models;

var builder = WebApplication.CreateBuilder(args);

// ── MVC + Razor Views ─────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Config binding ────────────────────────────────────────────────────
builder.Services.Configure<AppSettingsModel>(
    builder.Configuration.GetSection("TBCorApiServices"));

// ── HTTP Client (for calling internal APIs / Email sender) ────────────
builder.Services.AddHttpClient();

// ── Session (24-hour sliding expiry) ─────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout        = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name        = ".BTTemplate.Session";
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();           // ← must be before MapControllerRoute
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}");

app.Run();
