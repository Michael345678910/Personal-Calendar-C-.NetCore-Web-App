using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCoreCalendar.Data
{
    /// <summary>
    /// Data access abstraction for Events and Locations.
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
        void UpdateLocation(Location location);
        void DeleteLocation(int id);
    }

    /// <summary>
    /// EF Core implementation of <see cref="IDAL"/> backed by ApplicationDbContext from DI.
    /// </summary>
    public class DAL : IDAL
    {
        private readonly ApplicationDbContext _db;

        public DAL(ApplicationDbContext db) => _db = db;

        // ---------------- Events ----------------

        /// <summary>Returns all events (includes Location & User).</summary>
        public List<Event> GetEvents() =>
            _db.Events
               .Include(e => e.Location)
               .Include(e => e.User)
               .OrderByDescending(e => e.StartTime)
               .ToList();

        /// <summary>Returns events owned by a specific user (includes Location & User).</summary>
        public List<Event> GetMyEvents(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid)) return new List<Event>();

            return _db.Events
                      .Include(e => e.Location)
                      .Include(e => e.User)
                      .Where(e => e.User != null && e.User.Id == userid)
                      .OrderByDescending(e => e.StartTime)
                      .ToList();
        }

        /// <summary>Finds an event by id (or null if not found; includes Location & User).</summary>
        public Event GetEvent(int id) =>
            _db.Events
               .Include(e => e.Location)
               .Include(e => e.User)
               .SingleOrDefault(x => x.Id == id);

        /// <summary>
        /// Creates an event from posted form values.
        /// Expected form keys: UserId, Location (optional), Event.*
        /// NOTE: Location is OPTIONAL. If the form leaves it blank, we persist a NULL FK.
        ///       This requires the model/migration to have a nullable LocationId.
        /// </summary>
        public void CreateEvent(IFormCollection form)
        {
            var userId = form["UserId"].ToString();
            var user = _db.Users.SingleOrDefault(x => x.Id == userId)
                       ?? throw new InvalidOperationException("User not found.");

            // Optional location: null if empty, or use/create by name.
            var location = ResolveLocationOrNull(form["Location"].ToString());

            var newevent = new Event(form, location, user);
            _db.Events.Add(newevent);

            // One SaveChanges: inserts new Location (if created above) + Event in a single transaction.
            _db.SaveChanges();
        }

        /// <summary>
        /// Updates an existing event. Accepts Event.Id or Id in the form.
        /// Also supports optional Location (NULL when blank).
        /// </summary>
        public void UpdateEvent(IFormCollection form)
        {
            var idStr = form["Event.Id"].ToString();
            if (string.IsNullOrWhiteSpace(idStr)) idStr = form["Id"].ToString();
            if (!int.TryParse(idStr, out var eventId))
                throw new InvalidOperationException("Invalid event id.");

            var myevent = _db.Events
                             .Include(e => e.Location)
                             .Include(e => e.User)
                             .SingleOrDefault(x => x.Id == eventId)
                         ?? throw new InvalidOperationException("Event not found.");

            var userId = form["UserId"].ToString();
            var user = _db.Users.SingleOrDefault(x => x.Id == userId)
                       ?? throw new InvalidOperationException("User not found.");

            var location = ResolveLocationOrNull(form["Location"].ToString());

            myevent.UpdateEvent(form, location, user);
            _db.SaveChanges();
        }

        /// <summary>Deletes an event by id (if it exists).</summary>
        public void DeleteEvent(int id)
        {
            var myevent = _db.Events.SingleOrDefault(e => e.Id == id);
            if (myevent != null)
            {
                _db.Events.Remove(myevent);
                _db.SaveChanges();
            }
        }

        // ---------------- Locations ----------------

        /// <summary>Returns all locations ordered by name.</summary>
        public List<Location> GetLocations() =>
            _db.Locations.OrderBy(l => l.Name).ToList();

        /// <summary>Finds a location by id (or null).</summary>
        public Location GetLocation(int id) => _db.Locations.Find(id);

        /// <summary>Creates a new location.</summary>
        public void CreateLocation(Location location)
        {
            _db.Locations.Add(location);
            _db.SaveChanges();
        }

        /// <summary>Updates an existing location's name only.</summary>
        public void UpdateLocation(Location location)
        {
            var existing = _db.Locations.Find(location.Id);
            if (existing == null) return;
            existing.Name = location.Name;
            _db.SaveChanges();
        }

        /// <summary>
        /// Deletes a location and nulls out related event foreign keys (uses shadow FK "LocationId").
        /// With DeleteBehavior.SetNull configured in the model, EF will also handle this automatically,
        /// but keeping it here is safe if you ever change the delete behavior.
        /// </summary>
        public void DeleteLocation(int id)
        {
            var relatedEvents = _db.Events
                .Where(e => EF.Property<int?>(e, "LocationId") == id)
                .ToList();

            foreach (var ev in relatedEvents)
            {
                ev.Location = null;
                _db.Entry(ev).Property("LocationId").CurrentValue = null;
            }

            var loc = _db.Locations.Find(id);
            if (loc == null) return;

            _db.Locations.Remove(loc);
            _db.SaveChanges();
        }

        // ---------------- Helpers ----------------

        /// <summary>
        /// Returns an existing Location by name, or creates one if missing.
        /// If <paramref name="rawName"/> is null/empty, returns NULL (meaning "no location").
        /// </summary>
        private Location ResolveLocationOrNull(string rawName)
        {
            var name = rawName?.Trim();
            if (string.IsNullOrEmpty(name)) return null;

            var loc = _db.Locations.SingleOrDefault(l => l.Name == name);
            if (loc != null) return loc;

            loc = new Location { Name = name };
            _db.Locations.Add(loc); // inserted together with Event on SaveChanges
            return loc;
        }
    }
}