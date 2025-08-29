using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using DotNetCoreCalendar.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DotNetCoreCalendar.Controllers.ActionFilters;

namespace DotNetCoreCalendar.Controllers
{
    /// <summary>
    /// Authenticated CRUD operations for <see cref="Event"/>.
    /// </summary>
    [Authorize]
    public class EventController : Controller
    {
        private readonly IDAL _dal;
        private readonly UserManager<ApplicationUser> _usermanager;

        public EventController(IDAL dal, UserManager<ApplicationUser> usermanager)
        {
            _dal = dal;
            _usermanager = usermanager;
        }

        /// <summary>
        /// List events for the current user.
        /// </summary>
        public IActionResult Index()
        {
            if (TempData["Alert"] != null)
            {
                ViewData["Alert"] = TempData["Alert"];
            }
            return View(_dal.GetMyEvents(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        /// <summary>
        /// Show event details (by id).
        /// </summary>
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = _dal.GetEvent((int)id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        /// <summary>
        /// Render create form with locations and current user id.
        /// </summary>
        public IActionResult Create()
        {
            return View(new EventViewModel(_dal.GetLocations(), User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        /// <summary>
        /// Create a new event from posted form values.
        /// </summary>
        /// <remarks>
        /// NOTE: This action reads raw <see cref="IFormCollection"/> and delegates creation to the DAL.
        /// This bypasses MVC model binding validation. Consider also validating ModelState (e.g., required fields)
        /// and/or using strongly-typed binding for robustness. Anti-forgery is enabled.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel vm, IFormCollection form)
        {
            try
            {
                _dal.CreateEvent(form);
                TempData["Alert"] = "Success! You created a new event for: " + form["Event.Name"];
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(vm);
            }
        }

        /// <summary>
        /// Render edit form for a specific event.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="UserAccessOnly"/> to ensure the current user owns the event.
        /// </remarks>
        [UserAccessOnly]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = _dal.GetEvent((int)id);
            if (@event == null)
            {
                return NotFound();
            }
            var vm = new EventViewModel(@event, _dal.GetLocations(), User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(vm);
        }

        /// <summary>
        /// Update an existing event using posted form values.
        /// </summary>
        /// <remarks>
        /// NOTE: Similar to Create, this reads <see cref="IFormCollection"/> and delegates to the DAL,
        /// avoiding model binding validation. Anti-forgery is enabled.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            try
            {
                _dal.UpdateEvent(form);
                TempData["Alert"] = "Success! You modified an event for: " + form["Event.Name"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["Alert"] = "An error occurred: " + ex.Message;
                var vm = new EventViewModel(_dal.GetEvent(id), _dal.GetLocations(), User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(vm);
            }
        }

        /// <summary>
        /// Confirm delete page for an event.
        /// </summary>
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var @event = _dal.GetEvent((int)id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        /// <summary>
        /// Permanently delete the event.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _dal.DeleteEvent(id);
            TempData["Alert"] = "You deleted an event.";
            return RedirectToAction(nameof(Index));
        }
    }
}