using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Models
{
    /// <summary>
    /// Represents a physical/virtual place or resource that events can be assigned to.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Primary key (identity).
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Human-friendly name of the location (e.g., "Office", "Gym").
        /// </summary>
        /// <remarks>
        /// Consider marking as [Required] in production to prevent empty names.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Navigation: events scheduled at this location.
        /// Lazy-loaded due to virtual keyword (when proxies are enabled).
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }
    }
}