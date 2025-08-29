using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Helpers
{
    /// <summary>
    /// Utilities to convert domain models (Event/Location) to lightweight DTOs that FullCalendar expects,
    /// then serialize them to JSON for embedding in views.
    /// </summary>
    public static class JSONListHelper
    {
        /// <summary>
        /// Serializes a list of <see cref="Models.Event"/> to the minimal event DTO JSON for FullCalendar.
        /// </summary>
        public static string GetEventListJSONString(List<Models.Event> events)
        {
            var eventlist = new List<Event>(); // NOTE: refers to the DTO type below, not Models.Event.
            foreach (var model in events)
            {
                var myevent = new Event()
                {
                    id = model.Id,
                    start = model.StartTime,
                    end = model.EndTime,
                    resourceId = model.Location.Id,
                    description = model.Description,
                    title = model.Name
                };
                eventlist.Add(myevent);
            }
            return System.Text.Json.JsonSerializer.Serialize(eventlist);
        }

        /// <summary>
        /// Serializes a list of <see cref="Models.Location"/> to the minimal resource DTO JSON for FullCalendar.
        /// </summary>
        public static string GetResourceListJSONString(List<Models.Location> locations)
        {
            var resourcelist = new List<Resource>();

            foreach (var loc in locations)
            {
                var resource = new Resource()
                {
                    id = loc.Id,
                    title = loc.Name
                };
                resourcelist.Add(resource);
            }
            return System.Text.Json.JsonSerializer.Serialize(resourcelist);
        }
    }

    /// <summary>
    /// Lightweight event DTO for client-side consumption (avoids sending the full EF entity).
    /// </summary>
    /// <remarks>
    /// Name intentionally matches FullCalendar fields. Avoid confusion with DotNetCoreCalendar.Models.Event.
    /// </remarks>
    public class Event
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int resourceId { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// Lightweight resource DTO for FullCalendar.
    /// </summary>
    public class Resource
    {
        public int id { get; set; }
        public string title { get; set; }
    }
}