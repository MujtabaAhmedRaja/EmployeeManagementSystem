using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing employee attendance and leave requests.
    /// </summary>
    [Authorize]
    public class AttendanceController : BaseController
    {
        public AttendanceController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a select list of employees.
        /// </summary>
        /// <returns>A SelectList of employees.</returns>
        private async Task<SelectList> GetEmployeeListAsync()
        {
            var employees = await _context.Employees.OrderBy(e => e.EName).ToListAsync();
            return new SelectList(employees, "Eid", "EName");
        }

        /// <summary>
        /// Displays the attendance history.
        /// </summary>
        /// <returns>The index view with attendance records.</returns>
        public async Task<IActionResult> Index()
        {
            var attendanceHistory = await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AttDate)
                .ToListAsync();

            return View(attendanceHistory);
        }

        /// <summary>
        /// Displays the attendance recording page.
        /// </summary>
        /// <returns>The record view.</returns>
        public async Task<IActionResult> Record()
        {
            var model = new AttendanceRecordViewModel
            {
                Employees = await GetEmployeeListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Processes the attendance recording request.
        /// </summary>
        /// <param name="model">The attendance data to record.</param>
        /// <returns>A redirect to index or the record view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Record(AttendanceRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = await GetEmployeeListAsync();
                return View(model);
            }

            var attendance = new Attendance
            {
                Eid = model.EmployeeId,
                Status = model.Status,
                AttDate = DateTime.Now
            };

            _context.Attendances.Add(attendance);
            RecordLog("Attendance", $"Recorded attendance for employee ID: {model.EmployeeId}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the leave application page.
        /// </summary>
        /// <returns>The apply leave view.</returns>
        public async Task<IActionResult> ApplyLeave()
        {
            var model = new ApplyLeaveViewModel
            {
                Employees = await GetEmployeeListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Processes the leave application request.
        /// </summary>
        /// <param name="model">The leave request data.</param>
        /// <returns>A redirect to index or the apply leave view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyLeave(ApplyLeaveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = await GetEmployeeListAsync();
                return View(model);
            }

            var leave = new LeaveTable
            {
                Eid = model.EmployeeId,
                Reason = model.Reason,
                Status = "Pending"
            };

            _context.LeaveTables.Add(leave);
            RecordLog("LeaveTable", $"Applied for leave for employee ID: {model.EmployeeId}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the leave management page for administrators.
        /// </summary>
        /// <returns>The manage leaves view.</returns>
        public async Task<IActionResult> ManageLeaves()
        {
            var leaves = await _context.LeaveTables.Include(l => l.Employee)
                .OrderByDescending(l => l.LeaveId)
                .ToListAsync();
            return View(leaves);
        }

        /// <summary>
        /// Processes a leave request (Approve/Reject).
        /// </summary>
        /// <param name="leaveId">The ID of the leave request.</param>
        /// <param name="action">The action to perform (Approve/Reject).</param>
        /// <returns>A redirect to manage leaves.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessLeave(int leaveId, string action)
        {
            var leave = await _context.LeaveTables.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            string status = action.Equals("Approve", StringComparison.OrdinalIgnoreCase)
                ? "Approved"
                : "Rejected";
            
            leave.Status = status;
            
            RecordLog("LeaveTable", $"Processed leave ID: {leaveId} as {status}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageLeaves));
        }
    }
}
