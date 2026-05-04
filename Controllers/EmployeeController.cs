using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing employee records.
    /// </summary>
    [Authorize]
    public class EmployeeController : BaseController
    {
        public EmployeeController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a select list of departments.
        /// </summary>
        /// <returns>A SelectList of departments.</returns>
        private async Task<SelectList> GetDepartmentsAsync()
        {
            var departments = await _context.Departments.OrderBy(d => d.DepName).ToListAsync();
            return new SelectList(departments, "DepId", "DepName");
        }

        /// <summary>
        /// Displays the list of all employees.
        /// </summary>
        /// <returns>The index view with a list of employees.</returns>
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.Include(e => e.Department).OrderBy(e => e.EName).ToListAsync();
            return View(employees);
        }

        /// <summary>
        /// Displays the employee creation page.
        /// </summary>
        /// <returns>The create view.</returns>
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await GetDepartmentsAsync();
            return View();
        }

        /// <summary>
        /// Processes the employee creation request.
        /// </summary>
        /// <param name="employee">The employee data to create.</param>
        /// <returns>A redirect to index or the create view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EName,EAge,ECity,DepId")] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(employee);
            }

            // Explicitly generate ID if database doesn't auto-generate it
            int maxId = await _context.Employees.AnyAsync() ? await _context.Employees.MaxAsync(e => e.Eid) : 0;
            employee.Eid = maxId + 1;

            _context.Add(employee);
            RecordLog("Employee", $"Created employee: {employee.EName}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the employee edit page.
        /// </summary>
        /// <param name="id">The ID of the employee to edit.</param>
        /// <returns>The edit view or Not Found.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments = await GetDepartmentsAsync();
            return View(employee);
        }

        /// <summary>
        /// Processes the employee edit request.
        /// </summary>
        /// <param name="id">The ID of the employee to edit.</param>
        /// <param name="employee">The updated employee data.</param>
        /// <returns>A redirect to index or the edit view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Eid,EName,EAge,ECity,DepId")] Employee employee)
        {
            if (id != employee.Eid)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentsAsync();
                return View(employee);
            }

            try
            {
                var existingEmployee = await _context.Employees.FindAsync(id);
                if (existingEmployee == null) return NotFound();

                existingEmployee.EName = employee.EName;
                existingEmployee.EAge = employee.EAge;
                existingEmployee.ECity = employee.ECity;
                existingEmployee.DepId = employee.DepId;

                RecordLog("Employee", $"Updated employee: {employee.EName}");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(e => e.Eid == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the employee deletion confirmation page.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>The delete view or Not Found.</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Eid == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        /// <summary>
        /// Processes the employee deletion request.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>A redirect to index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Attendances)
                .Include(e => e.LeaveRequests)
                .Include(e => e.SalaryRecords)
                .FirstOrDefaultAsync(e => e.Eid == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Remove related records first to satisfy FK constraints
            _context.Attendances.RemoveRange(employee.Attendances);
            _context.LeaveTables.RemoveRange(employee.LeaveRequests);
            _context.Salaries.RemoveRange(employee.SalaryRecords);

            // Now remove the employee
            _context.Employees.Remove(employee);
            
            RecordLog("Employee", $"Deleted employee ID: {id}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
