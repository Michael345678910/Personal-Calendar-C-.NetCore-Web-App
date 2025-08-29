using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;

namespace DotNetCoreCalendar.Controllers
{
    /// <summary>
    /// CRUD for <see cref="Location"/> entities.
    /// Requires authentication for all actions.
    /// </summary>
    [Authorize]
    public class LocationController : Controller
    {
        private readonly IDAL _dal;
        private readonly UserManager<ApplicationUser> _usermanager;

        /// <summary>
        /// Inject DAL and Identity user manager (manager not used here yet, but kept for parity/extension).
        /// </summary>
        public LocationController(IDAL idal, UserManager<ApplicationUser> usermanager)
        {
            _dal = idal;
            _usermanager = usermanager;
        }

        /// <summary>
        /// List all locations.
        /// </summary>
        public IActionResult Index()
        {
            if (TempData["Alert"] != null)
                ViewData["Alert"] = TempData["Alert"];
            return View(_dal.GetLocations());
        }

        /// <summary>
        /// Show details for a single location.
        /// </summary>
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            return View(location);
        }

        /// <summary>
        /// Render create form.
        /// </summary>
        public IActionResult Create() => View();

        /// <summary>
        /// Create a new location.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Location location)
        {
            // NOTE: In production, consider adding [Required] to Location.Name and re-checking ModelState.
            if (!ModelState.IsValid) return View(location);

            try
            {
                _dal.CreateLocation(location);
                TempData["Alert"] = "Success! You created a location for: " + location.Name;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Bubble up a friendly message; keep details out of UI for security.
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(location);
            }
        }

        // ===== EDIT =====

        /// <summary>
        /// Render edit form for a specific location.
        /// </summary>
        public IActionResult Edit(int? id, string returnUrl = null)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl; // used by the view to navigate back if supplied
            return View(location); // Views/Location/Edit.cshtml
        }

        /// <summary>
        /// Update an existing location (name only).
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name")] Location location, string returnUrl = null)
        {
            if (id != location.Id) return NotFound();
            if (!ModelState.IsValid) return View(location);

            try
            {
                _dal.UpdateLocation(location); // implemented in DAL
                TempData["Alert"] = $"Saved changes to “{location.Name}”.";
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(location);
            }
        }

        // ===== DELETE =====

        /// <summary>
        /// Confirm delete page for a location.
        /// </summary>
        public IActionResult Delete(int? id, string returnUrl = null)
        {
            if (id == null) return NotFound();
            var location = _dal.GetLocation(id.Value);
            if (location == null) return NotFound();
            ViewData["ReturnUrl"] = returnUrl;
            return View(location);
        }

        /// <summary>
        /// Permanently delete the location (and detach related events’ Location).
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, string returnUrl = null)
        {
            try
            {
                _dal.DeleteLocation(id); // implemented in DAL
                TempData["Alert"] = "Location deleted.";
            }
            catch (Exception ex)
            {
                TempData["Alert"] = "An error occurred: " + ex.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }
    }
}