using System.Diagnostics;
using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers;

/// <summary>
/// Controller for the main landing page and dashboard.
/// </summary>
[Authorize]
public class HomeController : BaseController
{
    public HomeController(ProjectDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Displays the main dashboard for authenticated users.
    /// </summary>
    /// <returns>The dashboard view with summary stats.</returns>
    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            EmployeeCount = _context.Employees.Count(),
            DepartmentCount = _context.Departments.Count(),
            AttendanceCount = _context.Attendances.Count(),
            LeaveRequestCount = _context.LeaveTables.Count(),
            SalaryEntryCount = _context.Salaries.Count(),
            RoleCount = _context.Roles.Count()
        };

        return View(model);
    }

    /// <summary>
    /// Displays the privacy policy page.
    /// </summary>
    /// <returns>The privacy view.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays the error page.
    /// </summary>
    /// <returns>The error view with request ID.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
