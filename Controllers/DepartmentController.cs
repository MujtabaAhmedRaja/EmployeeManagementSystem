using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing organizational departments.
    /// </summary>
    [Authorize]
    public class DepartmentController : BaseController
    {
        public DepartmentController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Displays the list of all departments.
        /// </summary>
        /// <returns>The index view with a list of departments.</returns>
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.OrderBy(d => d.DepName).ToListAsync();
            return View(departments);
        }

        /// <summary>
        /// Displays the department creation page.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the department creation request.
        /// </summary>
        /// <param name="department">The department data to create.</param>
        /// <returns>A redirect to index or the create view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepName")] Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            _context.Departments.Add(department);
            RecordLog("Department", $"Created department: {department.DepName}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the department edit page.
        /// </summary>
        /// <param name="id">The ID of the department to edit.</param>
        /// <returns>The edit view or Not Found.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        /// <summary>
        /// Processes the department edit request.
        /// </summary>
        /// <param name="id">The ID of the department to edit.</param>
        /// <param name="department">The updated department data.</param>
        /// <returns>A redirect to index or the edit view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepId,DepName")] Department department)
        {
            if (id != department.DepId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                var existingDep = await _context.Departments.FindAsync(id);
                if (existingDep == null) return NotFound();

                existingDep.DepName = department.DepName;
                
                RecordLog("Department", $"Updated department: {department.DepName}");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Departments.Any(e => e.DepId == id)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the department deletion confirmation page.
        /// </summary>
        /// <param name="id">The ID of the department to delete.</param>
        /// <returns>The delete view or Not Found.</returns>
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        /// <summary>
        /// Processes the department deletion request.
        /// </summary>
        /// <param name="id">The ID of the department to delete.</param>
        /// <returns>A redirect to index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepId == id);

            if (department == null)
            {
                return NotFound();
            }

            if (department.Employees.Any())
            {
                // We shouldn't delete a department that still has employees.
                // We could reassign them, but for now we prevent deletion.
                TempData["Error"] = "Cannot delete department because it still has employees assigned to it. Please reassign the employees first.";
                return RedirectToAction(nameof(Index));
            }

            var name = department.DepName;
            _context.Departments.Remove(department);
            RecordLog("Department", $"Deleted department: {name}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
