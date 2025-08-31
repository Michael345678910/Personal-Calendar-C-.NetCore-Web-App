using DotNetCoreCalendar.Data;
using DotNetCoreCalendar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace DotNetCoreCalendar.Controllers.ActionFilters
{
    /// <summary>
    /// Attribute you can place on controllers/actions: [UserAccessOnly]
    /// Uses TypeFilter to resolve a DI-enabled filter implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class UserAccessOnlyAttribute : TypeFilterAttribute
    {
        public UserAccessOnlyAttribute() : base(typeof(UserAccessOnlyFilter))
        {
            // no args needed; constructor dependencies are resolved by DI
        }
    }

    /// <summary>
    /// Actual filter logic, DI-enabled. Ensures the current user owns the Event (by UserName).
    /// </summary>
    internal sealed class UserAccessOnlyFilter : IAsyncActionFilter
    {
        private readonly IDAL _dal;

        public UserAccessOnlyFilter(IDAL dal)
        {
            _dal = dal;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Try to read the {id} route value; if missing or invalid, just continue.
            if (!context.RouteData.Values.TryGetValue("id", out var idObj) ||
                !int.TryParse(Convert.ToString(idObj), out var id))
            {
                await next();
                return;
            }

            var username = context.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                // Not authenticated → behave like the old filter and redirect.
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "NotFound" }));
                return;
            }

            var myevent = _dal.GetEvent(id);
            // If the event exists and has an owner, enforce access by username (case-insensitive).
            if (myevent?.User?.UserName is string owner &&
                !string.Equals(owner, username, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Home", action = "NotFound" }));
                return;
            }

            // Allowed → continue to the action.
            await next();
        }
    }
}