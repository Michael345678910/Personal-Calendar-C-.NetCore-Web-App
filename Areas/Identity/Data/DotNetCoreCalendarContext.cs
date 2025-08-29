using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreCalendar.Data
{
    /// <summary>
    /// Identity DbContext scaffolded by templates.
    /// NOTE: Your app already uses <see cref="ApplicationDbContext"/> (with ApplicationUser)
    /// in Program.cs. This context appears unused and can likely be removed to avoid confusion,
    /// unless you intentionally keep it for a separate Identity area/database.
    /// </summary>
    public class DotNetCoreCalendarContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Preferred constructor for DI.
        /// </summary>
        public DotNetCoreCalendarContext(DbContextOptions<DotNetCoreCalendarContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Customize ASP.NET Core Identity mappings if needed (table names, keys, indexes, etc.).
        /// Currently uses defaults.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Example (commented): builder.Entity<IdentityUser>(b => b.ToTable("Users"));
        }
    }
}