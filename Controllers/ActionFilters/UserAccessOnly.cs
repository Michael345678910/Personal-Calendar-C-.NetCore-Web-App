using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Controllers.ActionFilters
{
    /// <summary>
    /// Ensures that only the owner of an Event (by username equality) can access the action.
    /// </summary>
    /// <remarks>
    /// NOTE:
    /// - This filter new-ups a DAL directly; consider injecting IDAL via DI for testability and proper lifetime.
    /// - It compares by UserName; comparing by user Id (ClaimTypes.NameIdentifier) is usually safer.
    /// - Redirect target is "Home/NotFound", but your controller exposes "PageNotFound".
    ///   Adjust the route if you later change behavior (kept as-is here to avoid code changes).
    /// </remarks>
    public class UserAccessOnly : Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute, Microsoft.AspNetCore.Mvc.Filters.IActionFilter
    {
        private DAL _dal = new DAL();

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (context.RouteData.Values.ContainsKey("id"))
            {
                int id = int.Parse((string)context.RouteData.Values["id"]);
                if (context.HttpContext.User != null)
                {
                    var username = context.HttpContext.User.Identity.Name;
                    if (username != null)
                    {
                        var myevent = _dal.GetEvent(id);
                        if (myevent.User != null)
                        {
                            if (myevent.User.UserName != username)
                            {
                                // NOTE: HomeController currently has PageNotFound() rather than NotFound().
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "NotFound" }));
                            }
                        }
                    }
                }
            }
        }
    }
}