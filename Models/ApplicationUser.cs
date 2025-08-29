using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Models
{
    /// <summary>
    /// ASP.NET Core Identity user extended with navigation to owned events.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Events created/owned by this user.
        /// Virtual enables lazy-loading with proxies.
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }
    }
}