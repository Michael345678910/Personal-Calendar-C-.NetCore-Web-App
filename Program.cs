// NOTE: This file uses the .NET 6+ "minimal hosting" model (top-level statements).
// It wires up Entity Framework Core, ASP.NET Core Identity, MVC/Razor Pages, and route mappings.

using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

// ----------------------------- Services (DI container) -----------------------------

// Register the application's EF Core DbContext.
// .UseLazyLoadingProxies() enables EF lazy loading for virtual navigation properties.
// IMPORTANT: Ensure "DefaultConnection" exists in appsettings.* and points to your DB.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        .UseLazyLoadingProxies());

// Configure ASP.NET Core Identity using the custom ApplicationUser entity.
// RequireConfirmedAccount=false allows immediate sign-in after registration.
// If you add email confirmation later, change this to true and configure email sender.
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // TIP: You can harden password rules here (e.g., options.Password.RequiredLength = 8;)
    })
    // Store Identity data in the same ApplicationDbContext registered above.
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register the Data Access Layer abstraction for app-specific data operations.
builder.Services.AddScoped<IDAL, DAL>();

// Add MVC controllers with views.
// In DEBUG, AddRazorRuntimeCompilation() lets you edit .cshtml files and see changes
// without rebuilding (requires the runtime compilation package).
builder.Services.AddControllersWithViews()
#if DEBUG
    .AddRazorRuntimeCompilation()
#endif
    ;

// Add Razor Pages (used by Identity UI and any custom pages under /Areas or /Pages).
builder.Services.AddRazorPages();

var app = builder.Build();

// ----------------------------- HTTP request pipeline -----------------------------

if (!app.Environment.IsDevelopment())
{
    // In production: send unhandled exceptions to /Home/Error view.
    app.UseExceptionHandler("/Home/Error");
    // Enable HSTS to instruct browsers to prefer HTTPS for 30 days by default.
    app.UseHsts();
}

// Static middleware and routing stack.
app.UseHttpsRedirection(); // Redirect HTTP -> HTTPS
app.UseStaticFiles();      // Serve files from wwwroot/

app.UseRouting();          // Match incoming requests to endpoints

app.UseAuthentication();   // Read/validate auth cookies, set HttpContext.User
app.UseAuthorization();    // Enforce [Authorize] on controllers/pages

// ----------------------------- Endpoint mapping -----------------------------

// Route that allows calling Home actions without specifying the controller explicitly.
// Example: GET /Privacy -> HomeController.Privacy()
app.MapControllerRoute(
    name: "HomeActionOnly",
    pattern: "{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" });

// Conventional default route for controllers.
// Example: /{controller=Home}/{action=Index}/{id?}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages endpoints (Identity UI and any custom pages).
app.MapRazorPages();

app.Run();