using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing organizational roles.
    /// </summary>
    [Authorize]
    public class RoleController : BaseController
    {
        public RoleController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Displays the list of all roles.
        /// </summary>
        /// <returns>The index view with a list of roles.</returns>
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.OrderBy(r => r.RoleName).ToListAsync();
            return View(roles);
        }

        /// <summary>
        /// Displays the role creation page.
        /// </summary>
        /// <returns>The create view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the role creation request.
        /// </summary>
        /// <param name="role">The role to be created.</param>
        /// <returns>A redirect to the index view or the create view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleName")] Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                RecordLog("Role", $"Created role: {role.RoleName}");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        /// <summary>
        /// Displays the role edit page.
        /// </summary>
        /// <param name="id">The ID of the role to edit.</param>
        /// <returns>The edit view or a Not Found result.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();

            return View(role);
        }

        /// <summary>
        /// Processes the role edit request.
        /// </summary>
        /// <param name="id">The ID of the role to edit.</param>
        /// <param name="role">The updated role data.</param>
        /// <returns>A redirect to the index view or the edit view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoleId,RoleName")] Role role)
        {
            if (id != role.RoleId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _context.Roles.FindAsync(id);
                    if (existingRole == null) return NotFound();

                    existingRole.RoleName = role.RoleName;
                    
                    RecordLog("Role", $"Updated role ID: {role.RoleId} to {role.RoleName}");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Roles.Any(e => e.RoleId == role.RoleId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        /// <summary>
        /// Displays the role deletion confirmation page.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>The delete view or a Not Found result.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var role = await _context.Roles.FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null) return NotFound();

            return View(role);
        }

        /// <summary>
        /// Processes the role deletion request.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>A redirect to the index view.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles
                .Include(r => r.SalaryRecords)
                .FirstOrDefaultAsync(r => r.RoleId == id);

            if (role == null) return NotFound();

            if (role.SalaryRecords.Any())
            {
                TempData["Error"] = "Cannot delete role because it is currently assigned to one or more employees. Please remove those assignments first.";
                return RedirectToAction(nameof(Index));
            }

            var roleName = role.RoleName;
            _context.Roles.Remove(role);
            RecordLog("Role", $"Deleted role: {roleName}");
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
}
