using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Models.ViewModels
{
    /// <summary>
    /// View model for create/edit Event screens.
    /// Supplies the Event entity, a Location dropdown, and user context.
    /// </summary>
    public class EventViewModel
    {
        /// <summary>The event being created/edited.</summary>
        public Event Event { get; set; }

        /// <summary>
        /// Select list for Locations shown in the UI.
        /// NOTE: This is a field, not an auto-property. MVC can still read it for rendering,
        /// but consider making it a property (public List&lt;SelectListItem&gt; Location { get; set; }) for consistency.
        /// </summary>
        public List<SelectListItem> Location = new List<SelectListItem>();

        /// <summary>Current event's location name (used to pre-select in the dropdown).</summary>
        public string LocationName { get; set; }

        /// <summary>Current user id (posted back on submit).</summary>
        public string UserId { get; set; }

        /// <summary>
        /// Populates the view model and pre-selects the current location by visible text.
        /// </summary>
        public EventViewModel(Event myevent, List<Location> locations, string userid)
        {
            Event = myevent;
            LocationName = myevent.Location.Name;
            UserId = userid;
            foreach (var loc in locations)
            {
                Location.Add(new SelectListItem() { Text = loc.Name });
            }
        }

        /// <summary>
        /// Alternate constructor for "Create" where no existing event location is chosen yet.
        /// </summary>
        public EventViewModel(List<Location> locations, string userid)
        {
            UserId = userid;
            foreach (var loc in locations)
            {
                Location.Add(new SelectListItem() { Text = loc.Name });
            }
        }

        /// <summary>Parameterless for MVC model binding.</summary>
        public EventViewModel()
        {
        }
    }
}