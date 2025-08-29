using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotNetCoreCalendar.Data
{
    /// <summary>
    /// EF Core DbContext including Identity tables plus app-specific entities (Events, Locations).
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Preferred constructor when using ASP.NET Core DI (configured in Program.cs).
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Parameterless constructor used by code paths that new-up the context.
        /// </summary>
        public ApplicationDbContext()
        {
        }

        /// <summary>
        /// Configure model customizations and relationships.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Place fluent API config here if needed (e.g., constraints, indexes, relationships).
        }

        /// <summary>
        /// Fallback configuration when the context wasn't configured by DI.
        /// Reads connection string from appsettings.json and enables lazy loading proxies.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder
                    .UseSqlServer(connectionString)
                    .UseLazyLoadingProxies();
            }
        }

        /// <summary>Events table.</summary>
        public DbSet<Event> Events { get; set; }

        /// <summary>Locations table.</summary>
        public DbSet<Location> Locations { get; set; }
    }
}