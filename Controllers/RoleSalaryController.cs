using EMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    /// <summary>
    /// Controller for managing employee roles and salaries.
    /// </summary>
    [Authorize]
    public class RoleSalaryController : BaseController
    {
        public RoleSalaryController(ProjectDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a select list of employees.
        /// </summary>
        /// <returns>A SelectList of employees.</returns>
        private async Task<SelectList> GetEmployeesAsync()
        {
            var employees = await _context.Employees.OrderBy(e => e.EName).ToListAsync();
            return new SelectList(employees, "Eid", "EName");
        }

        /// <summary>
        /// Retrieves a select list of roles.
        /// </summary>
        /// <returns>A SelectList of roles.</returns>
        private async Task<SelectList> GetRolesAsync()
        {
            var roles = await _context.Roles.OrderBy(r => r.RoleName).ToListAsync();
            return new SelectList(roles, "RoleId", "RoleName");
        }

        /// <summary>
        /// Displays the list of all salary records.
        /// </summary>
        /// <returns>The index view with salary records.</returns>
        public async Task<IActionResult> Index()
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .Include(s => s.Role)
                .OrderBy(s => s.Employee!.EName)
                .ToListAsync();

            return View(salaries);
        }

        /// <summary>
        /// Displays the role assignment page.
        /// </summary>
        /// <returns>The assign role view.</returns>
        public async Task<IActionResult> AssignRole()
        {
            var model = new AssignRoleViewModel
            {
                Employees = await GetEmployeesAsync(),
                Roles = await GetRolesAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Processes the role assignment request.
        /// </summary>
        /// <param name="model">The assignment data.</param>
        /// <returns>A redirect to index or the assign role view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = await GetEmployeesAsync();
                model.Roles = await GetRolesAsync();
                return View(model);
            }

            var existingSalary = await _context.Salaries.FindAsync(model.EmployeeId, model.RoleId);
            if (existingSalary != null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Salaries.Add(new Salary
            {
                Eid = model.EmployeeId,
                RoleId = model.RoleId,
                Amount = 1000 // Ensure this meets the minimum required by the DB CHECK constraint
            });

            RecordLog("Salary", $"Assigned role ID {model.RoleId} to employee ID {model.EmployeeId}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the salary setting page.
        /// </summary>
        /// <returns>The set salary view.</returns>
        public async Task<IActionResult> SetSalary()
        {
            var model = new SetSalaryViewModel
            {
                Employees = await GetEmployeesAsync(),
                Roles = await GetRolesAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Processes the salary setting request.
        /// </summary>
        /// <param name="model">The salary data.</param>
        /// <returns>A redirect to index or the set salary view with errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetSalary(SetSalaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Employees = await GetEmployeesAsync();
                model.Roles = await GetRolesAsync();
                return View(model);
            }

            var salary = await _context.Salaries.FindAsync(model.EmployeeId, model.RoleId);
            if (salary == null)
            {
                salary = new Salary
                {
                    Eid = model.EmployeeId,
                    RoleId = model.RoleId,
                    Amount = model.BaseSalary
                };
                _context.Salaries.Add(salary);
                RecordLog("Salary", $"Set salary for employee ID {model.EmployeeId}, role ID {model.RoleId}");
            }
            else
            {
                salary.Amount = model.BaseSalary;
                
                RecordLog("Salary", $"Updated salary for employee ID {model.EmployeeId}, role ID {model.RoleId}");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the salary update page for a specific record.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="roleId">The ID of the role.</param>
        /// <returns>The update salary view or Not Found.</returns>
        public async Task<IActionResult> UpdateSalary(int employeeId, int roleId)
        {
            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.Eid == employeeId && s.RoleId == roleId);

            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        /// <summary>
        /// Processes the salary update request.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="roleId">The ID of the role.</param>
        /// <param name="baseSalary">The new salary amount.</param>
        /// <returns>A redirect to index or Not Found.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSalary(int employeeId, int roleId, int baseSalary)
        {
            var salary = await _context.Salaries.FindAsync(employeeId, roleId);
            if (salary == null)
            {
                return NotFound();
            }

            salary.Amount = baseSalary;
            
            RecordLog("Salary", $"Updated salary for employee ID {employeeId}, role ID {roleId}");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
