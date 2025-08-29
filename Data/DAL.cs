using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreCalendar.Data
{
    /// <summary>
    /// Data access abstraction for events and locations.
    /// </summary>
    public interface IDAL
    {
        // Events
        List<Event> GetEvents();
        List<Event> GetMyEvents(string userid);
        Event GetEvent(int id);
        void CreateEvent(IFormCollection form);
        void UpdateEvent(IFormCollection form);
        void DeleteEvent(int id);

        // Locations
        List<Location> GetLocations();
        Location GetLocation(int id);
        void CreateLocation(Location location);

        // NEW:
        void UpdateLocation(Location location);
        void DeleteLocation(int id);
    }

    /// <summary>
    /// EF Core implementation of <see cref="IDAL"/>.
    /// </summary>
    /// <remarks>
    /// NOTE: This class constructs its own ApplicationDbContext instance (new ApplicationDbContext()).
    /// In ASP.NET Core, prefer injecting ApplicationDbContext via DI for lifetime management,
    /// connection config, and testing. Keeping as-is to avoid behavior changes.
    /// </remarks>
    public class DAL : IDAL
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // ---------------- Events ----------------

        /// <summary>Returns all events.</summary>
        public List<Event> GetEvents()
        {
            return db.Events.ToList();
        }

        /// <summary>Returns events owned by a specific user.</summary>
        public List<Event> GetMyEvents(string userid)
        {
            return db.Events.Where(x => x.User.Id == userid).ToList();
        }

        /// <summary>Finds an event by id (or null if not found).</summary>
        public Event GetEvent(int id)
        {
            return db.Events.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Creates an event from posted form values.
        /// Expects keys: "Location" (name), "UserId", and "Event.*" fields.
        /// </summary>
        public void CreateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            var newevent = new Event(form, db.Locations.FirstOrDefault(x => x.Name == locname), user);
            db.Events.Add(newevent);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates an existing event with posted form values.
        /// </summary>
        public void UpdateEvent(IFormCollection form)
        {
            var locname = form["Location"].ToString();
            var eventid = int.Parse(form["Event.Id"]);
            var myevent = db.Events.FirstOrDefault(x => x.Id == eventid);
            var location = db.Locations.FirstOrDefault(x => x.Name == locname);
            var user = db.Users.FirstOrDefault(x => x.Id == form["UserId"].ToString());
            myevent.UpdateEvent(form, location, user);
            db.Entry(myevent).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>Deletes an event by id.</summary>
        public void DeleteEvent(int id)
        {
            var myevent = db.Events.Find(id);
            db.Events.Remove(myevent);
            db.SaveChanges();
        }

        // ---------------- Locations ----------------

        /// <summary>Returns all locations.</summary>
        public List<Location> GetLocations()
        {
            return db.Locations.ToList();
        }

        /// <summary>Finds a location by id (or null if not found).</summary>
        public Location GetLocation(int id)
        {
            return db.Locations.Find(id);
        }

        /// <summary>Creates a new location.</summary>
        public void CreateLocation(Location location)
        {
            db.Locations.Add(location);
            db.SaveChanges();
        }

        /// <summary>
        /// Updates an existing location's name only.
        /// </summary>
        public void UpdateLocation(Location location)
        {
            var existing = db.Locations.Find(location.Id);
            if (existing == null) return;
            existing.Name = location.Name;
            db.Entry(existing).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes a location and nulls out related event foreign keys.
        /// </summary>
        /// <remarks>
        /// Since Event has no explicit LocationId property, EF uses a shadow FK named "LocationId".
        /// We query/update the shadow FK via EF.Property and Entry().Property("LocationId").
        /// </remarks>
        public void DeleteLocation(int id)
        {
            // Find events referencing this location by the shadow FK.
            var relatedEvents = db.Events
                .Where(e => EF.Property<int?>(e, "LocationId") == id)
                .ToList();

            // Detach the location from each event (set FK to null)
            foreach (var ev in relatedEvents)
            {
                ev.Location = null;
                db.Entry(ev).Property("LocationId").CurrentValue = null;
                db.Entry(ev).State = EntityState.Modified;
            }

            var loc = db.Locations.Find(id);
            if (loc == null) return;

            db.Locations.Remove(loc);
            db.SaveChanges();
        }
    }
}