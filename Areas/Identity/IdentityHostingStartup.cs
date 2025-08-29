using System;
using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DotNetCoreCalendar.Areas.Identity.IdentityHostingStartup))]
namespace DotNetCoreCalendar.Areas.Identity
{
    /// <summary>
    /// Hook for configuring services specifically for the Identity UI Area.
    /// Currently empty; keep to add Identity-specific configuration without touching Program.cs.
    /// </summary>
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                // Add identity options, external providers, or cookie settings here if needed.
            });
        }
    }
}