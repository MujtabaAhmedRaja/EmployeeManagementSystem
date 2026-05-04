using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for generating various system reports and dashboards.
    /// </summary>
    [Authorize]
    public class ReportController : BaseController
    {
        public ReportController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Displays the administrator dashboard with system-wide statistics.
        /// </summary>
        /// <returns>The dashboard view.</returns>
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
        /// Generates a detailed report of all employees.
        /// </summary>
        /// <returns>The employee details report view.</returns>
        public async Task<IActionResult> EmployeeDetails()
        {
            var employees = await _context.Employees.Include(e => e.Department).ToListAsync();
            var salaryLookup = await _context.Salaries.Include(s => s.Role).ToListAsync();

            var report = employees.Select(employee => new EmployeeReportModel
            {
                Id = employee.Eid,
                Name = employee.EName,
                Department = employee.Department?.DepName,
                Role = salaryLookup.FirstOrDefault(s => s.Eid == employee.Eid)?.Role?.RoleName ?? "Unassigned",
                Age = employee.EAge,
                City = employee.ECity,
                Status = "Active"
            }).ToList();

            return View(report);
        }

        /// <summary>
        /// Generates an attendance summary report for all employees.
        /// </summary>
        /// <returns>The attendance reports view.</returns>
        public async Task<IActionResult> AttendanceReports()
        {
            var attendanceHistory = await _context.Attendances.Include(a => a.Employee).ToListAsync();
            var leaves = await _context.LeaveTables.Include(l => l.Employee).ToListAsync();
            var employees = await _context.Employees.OrderBy(e => e.EName).ToListAsync();

            var report = employees.Select(emp => new AttendanceReportModel
            {
                EmployeeName = emp.EName,
                PresentDays = attendanceHistory.Count(a => a.Eid == emp.Eid && a.Status == "Present"),
                AbsentDays = attendanceHistory.Count(a => a.Eid == emp.Eid && a.Status == "Absent"),
                LeaveDays = leaves.Count(l => l.Eid == emp.Eid && l.Status == "Approved"),
                AttendancePercentage = CalculateAttendancePercentage(
                    attendanceHistory.Count(a => a.Eid == emp.Eid),
                    attendanceHistory.Count(a => a.Eid == emp.Eid && a.Status == "Present"))
            }).ToList();

            return View(report);
        }

        /// <summary>
        /// Generates a payroll and salary summary report.
        /// </summary>
        /// <returns>The salary reports view.</returns>
        public async Task<IActionResult> SalaryReports()
        {
            var salaryRecords = await _context.Salaries
                .Include(s => s.Employee)
                .Include(s => s.Role)
                .ToListAsync();

            var report = salaryRecords.Select(s => new SalaryReportModel
            {
                EmployeeName = s.Employee?.EName,
                RoleName = s.Role?.RoleName,
                BaseSalary = s.Amount,
                Allowances = 0,
                Deductions = 0,
                NetSalary = s.Amount
            }).ToList();

            ViewBag.TotalPayroll = report.Sum(r => r.NetSalary);
            return View(report);
        }

        /// <summary>
        /// Calculates the attendance percentage based on total days and days present.
        /// </summary>
        /// <param name="totalDays">Total number of days recorded.</param>
        /// <param name="presentDays">Number of days present.</param>
        /// <returns>A formatted percentage string.</returns>
        private static string CalculateAttendancePercentage(int totalDays, int presentDays)
        {
            if (totalDays == 0)
            {
                return "0%";
            }

            return Math.Round((presentDays / (double)totalDays) * 100, 0) + "%";
        }
    }
}
