// NOTE: .NET 6+ / .NET 9 minimal hosting Program.cs.
// Wires up EF Core, Identity, MVC/Razor Pages, and registers the DAL (IDAL -> DAL).

using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------- Services (DI container) -----------------------------

// EF Core DbContext (SQL Server) + Lazy Loading
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies());

// ASP.NET Core Identity with ApplicationUser
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // You can tighten password rules here, e.g.:
        // options.Password.RequiredLength = 8;
        // options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register DAL for app data operations (fixes the "no argument for 'db'" ctor error)
builder.Services.AddScoped<IDAL, DAL>();

// MVC + Razor Pages
builder.Services.AddControllersWithViews()
#if DEBUG
    .AddRazorRuntimeCompilation()
#endif
    ;
builder.Services.AddRazorPages();

var app = builder.Build();

// ----------------------------- HTTP request pipeline -----------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------- Endpoint mapping -----------------------------

// Optional convenience route: /Privacy -> HomeController.Privacy()
app.MapControllerRoute(
    name: "HomeActionOnly",
    pattern: "{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" });

// Default conventional route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();