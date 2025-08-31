using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DotNetCoreCalendar.Models
{
    /// <summary>
    /// Calendar event owned by an ApplicationUser, optionally linked to a Location.
    /// </summary>
    public class Event
    {
        [Key]
        public int Id { get; set; }

        // Core fields
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // --------- Relationships ---------

        /// <summary>
        /// Nullable FK column to <see cref="Location"/>. Making this explicit (and nullable)
        /// prevents EF from inventing a shadow property (e.g., LocationId1) and allows
        /// events to be saved without a location.
        /// </summary>
        public int? LocationId { get; set; }

        /// <summary>
        /// Optional navigation to the event's location.
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(LocationId))]
        public virtual Location? Location { get; set; }

        /// <summary>
        /// Optional navigation to the owning user.
        /// (EF will maintain a shadow FK for User unless you also add a UserId string property.)
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        public Event() { }

        /// <summary>
        /// Construct from a POSTed form (+ resolved location/user).
        /// Expects keys:
        ///  - "Event.Name", "Event.Description" (optional)
        ///  - "Event.StartTime", "Event.EndTime" in yyyy-MM-ddTHH:mm (datetime-local)
        /// </summary>
        public Event(IFormCollection form, Location? location, ApplicationUser? user)
        {
            UpdateFromForm(form, location, user);
        }

        /// <summary>
        /// Update existing entity from a POSTed form.
        /// </summary>
        public void UpdateEvent(IFormCollection form, Location? location, ApplicationUser? user)
        {
            UpdateFromForm(form, location, user);
        }

        private void UpdateFromForm(IFormCollection form, Location? location, ApplicationUser? user)
        {
            User = user;

            // Required title
            Name = form["Event.Name"].ToString();

            // Optional description
            Description = form["Event.Description"].ToString();

            // Parse datetime-local inputs (accepts with/without seconds)
            var startStr = form["Event.StartTime"].ToString();
            var endStr = form["Event.EndTime"].ToString();
            string[] formats = { "yyyy-MM-dd'T'HH:mm", "yyyy-MM-dd'T'HH:mm:ss" };

            if (!DateTime.TryParseExact(startStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
                throw new FormatException($"Invalid start time: {startStr}");
            if (!DateTime.TryParseExact(endStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                throw new FormatException($"Invalid end time: {endStr}");

            StartTime = start;
            EndTime = end;

            // Location is optional: assign both the nav and FK consistently
            Location = location;
            LocationId = location?.Id;
        }
    }
}