using EMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    /// <summary>
    /// Base controller providing common functionality like logging.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly ProjectDbContext _context;

        protected BaseController(ProjectDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds an action record to the system logs (requires SaveChangesAsync to be called afterward).
        /// </summary>
        /// <param name="tableName">The name of the table affected.</param>
        /// <param name="action">The action performed.</param>
        protected void RecordLog(string tableName, string action)
        {
            // TEMPORARY FIX: Trim to 20 characters because the current database column 'action' is too short.
            // You MUST run the SQL command: ALTER TABLE [Log] ALTER COLUMN [action] NVARCHAR(500);
            // After running that command, you can change this back to 500.
            var safeAction = (action ?? "No action").Length > 20 
                ? action!.Substring(0, 17) + "..." 
                : action;

            var log = new Log
            {
                TableName = tableName,
                Action = safeAction,
                ActionDate = DateTime.Now
            };

            _context.Logs.Add(log);
        }
    }
}
