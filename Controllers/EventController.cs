using System;
using System.Security.Claims;
using DotNetCoreCalendar.Controllers.ActionFilters;
using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using DotNetCoreCalendar.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Authenticated CRUD operations for <see cref="Event"/>.
/// </summary>

namespace DotNetCoreCalendar.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IDAL _dal;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventController(IDAL dal, UserManager<ApplicationUser> userManager)
        {
            _dal = dal;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            if (TempData["Alert"] != null)
                ViewData["Alert"] = TempData["Alert"];

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_dal.GetMyEvents(userId));
        }

        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var ev = _dal.GetEvent(id.Value);
            if (ev == null) return NotFound();
            return View(ev);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new EventViewModel(_dal.GetLocations(), userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventViewModel vm, IFormCollection form)
        {
            bool isAjax = string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            try
            {
                _dal.CreateEvent(form);

                if (isAjax) return Ok(new { ok = true });

                TempData["Alert"] = "Success! You created a new event for: " + form["Event.Name"];
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.GetBaseException()?.Message ?? ex.Message;
                if (isAjax) return BadRequest(new { ok = false, error = msg });

                ViewData["Alert"] = "Save failed: " + msg;
                return View(vm);
            }
            catch (Exception ex)
            {
                if (isAjax) return BadRequest(new { ok = false, error = ex.Message });

                ViewData["Alert"] = "An error occurred: " + ex.Message;
                return View(vm);
            }
        }

        [UserAccessOnly]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var ev = _dal.GetEvent(id.Value);
            if (ev == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new EventViewModel(ev, _dal.GetLocations(), userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection form)
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
                var vm = new EventViewModel(_dal.GetEvent(id), _dal.GetLocations(),
                    User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(vm);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var ev = _dal.GetEvent(id.Value);
            if (ev == null) return NotFound();
            return View(ev);
        }

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