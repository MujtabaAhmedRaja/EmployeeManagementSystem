using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for viewing system activity logs.
    /// </summary>
    [Authorize]
    public class LogController : BaseController
    {
        public LogController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Displays the system logs.
        /// </summary>
        /// <returns>The log view with activity history.</returns>
        public async Task<IActionResult> Index()
        {
            var logs = await _context.Logs.OrderByDescending(l => l.ActionDate).ToListAsync();
            return View(logs);
        }
    }
}
