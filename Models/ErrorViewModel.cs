using System;

namespace DotNetCoreCalendar.Models
{
    /// <summary>
    /// View model used by the global error page to display a request correlation id.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Correlation id (e.g., from Activity.Current?.Id) helpful for tracing in logs.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// True when <see cref="RequestId"/> has a value and can be shown to the user.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}