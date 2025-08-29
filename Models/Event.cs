using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Models
{
    /// <summary>
    /// Calendar event entity owned by a user and optionally tied to a Location.
    /// </summary>
    public class Event
    {
        /// <summary>Primary key (identity).</summary>
        [Key]
        public int Id { get; set; }

        /// <summary>Event title shown in lists/calendars.</summary>
        public string Name { get; set; }

        /// <summary>Optional longer description/details.</summary>
        public string Description { get; set; }

        /// <summary>Start date/time (local or UTC depending on app policy).</summary>
        public DateTime StartTime { get; set; }

        /// <summary>End date/time (should be ≥ <see cref="StartTime"/>).</summary>
        public DateTime EndTime { get; set; }

        // ---------------- Navigation properties ----------------

        /// <summary>
        /// Optional location/resource assigned to the event.
        /// EF creates a shadow FK "LocationId" since there is no explicit LocationId property.
        /// </summary>
        public virtual Location Location { get; set; }

        /// <summary>Owner of the event.</summary>
        public virtual ApplicationUser User { get; set; }

        // ---------------- Convenience constructors / updaters ----------------

        /// <summary>
        /// Constructs an event by reading values from an MVC form post.
        /// </summary>
        /// <param name="form">Posted form with keys: "Event.Name", "Event.Description", "Event.StartTime", "Event.EndTime".</param>
        /// <param name="location">Resolved Location entity (may be null).</param>
        /// <param name="user">Resolved ApplicationUser owner.</param>
        /// <remarks>
        /// Uses DateTime.Parse on raw strings; in production consider using TryParse with culture/time-zone handling and validation.
        /// </remarks>
        public Event(IFormCollection form, Location location, ApplicationUser user)
        {
            User = user;
            Name = form["Event.Name"].ToString();
            Description = form["Event.Description"].ToString();
            StartTime = DateTime.Parse(form["Event.StartTime"].ToString());
            EndTime = DateTime.Parse(form["Event.EndTime"].ToString());
            Location = location;
        }

        /// <summary>
        /// Updates current event fields using posted form values.
        /// </summary>
        public void UpdateEvent(IFormCollection form, Location location, ApplicationUser user)
        {
            User = user;
            Name = form["Event.Name"].ToString();
            Description = form["Event.Description"].ToString();
            StartTime = DateTime.Parse(form["Event.StartTime"].ToString());
            EndTime = DateTime.Parse(form["Event.EndTime"].ToString());
            Location = location;
        }

        /// <summary>Parameterless constructor for EF Core.</summary>
        public Event()
        {
        }
    }
}