using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Helpers;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Controllers
{
    /// <summary>
    /// Public landing pages and user calendar view.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDAL _idal;
        private readonly UserManager<ApplicationUser> _usermanager;

        public HomeController(ILogger<HomeController> logger, IDAL idal, UserManager<ApplicationUser> usermanager)
        {
            _logger = logger;
            _idal = idal;
            _usermanager = usermanager;
        }

        /// <summary>
        /// Landing page with example events/resources (all events).
        /// </summary>
        public IActionResult Index()
        {
            // Convert entities to lightweight DTO JSON for client-side FullCalendar.
            ViewData["Resources"] = JSONListHelper.GetResourceListJSONString(_idal.GetLocations());
            ViewData["Events"] = JSONListHelper.GetEventListJSONString(_idal.GetEvents());
            return View();
        }

        /// <summary>
        /// Authenticated user’s personal calendar page (their events only).
        /// </summary>
        [Authorize]
        public IActionResult MyCalendar()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["Resources"] = JSONListHelper.GetResourceListJSONString(_idal.GetLocations());
            ViewData["Events"] = JSONListHelper.GetEventListJSONString(_idal.GetMyEvents(userid));
            return View();
        }

        /// <summary>Static privacy page.</summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error page with correlation id for logs/troubleshooting.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 404 page. Sets response status to 404 and renders a friendly view.
        /// </summary>
        public ViewResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}