using HaberPortali2.Data;                     // AppDbContext için doðru namespace
using HaberPortali2.Repositories;             // Repository ve IRepository için doðru namespace
using Microsoft.AspNetCore.Authentication.Cookies; // Cookie authentication için
using Microsoft.EntityFrameworkCore;          // UseSqlServer için

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1) SQL Server Baðlantýsý (server = ".")
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// ------------------------------------------------------------
// 2) Generic Repository için Dependency Injection
// ------------------------------------------------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ------------------------------------------------------------
// 3) Cookie bazlý kimlik doðrulama
// ------------------------------------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";             // Giriþ yapýlmazsa yönleneceði sayfa
        options.AccessDeniedPath = "/Auth/Denied";     // Yetki yoksa yönleneceði sayfa
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.Cookie.Name = "HaberPortaliAuth";      // Cookie adý
    });

// MVC'yi aktif et
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ------------------------------------------------------------
// 4) Hata yönetimi
// ------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ------------------------------------------------------------
// 5) Middleware Pipeline
// ------------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ------------------------------------------------------------
// 6) ***AREA Route (Admin Paneli için þart)***
// ------------------------------------------------------------
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// ------------------------------------------------------------
// 7) Varsayýlan Route
// ------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
