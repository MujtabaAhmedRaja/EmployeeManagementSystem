using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Models
{
    /// <summary>
    /// ViewModel for recording attendance.
    /// </summary>
    public class AttendanceRecordViewModel
    {
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = "Present";

        public SelectList? Employees { get; set; }
    }

    /// <summary>
    /// ViewModel for applying for leave.
    /// </summary>
    public class ApplyLeaveViewModel
    {
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } = string.Empty;

        public SelectList? Employees { get; set; }
    }

    /// <summary>
    /// ViewModel for assigning a role to an employee.
    /// </summary>
    public class AssignRoleViewModel
    {
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        public SelectList? Employees { get; set; }
        public SelectList? Roles { get; set; }
    }

    /// <summary>
    /// ViewModel for setting or updating employee salary.
    /// </summary>
    public class SetSalaryViewModel
    {
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }

        [Required]
        [Range(10000, 10000000, ErrorMessage = "Salary amount must be at least 10000 according to system policy.")]
        [Display(Name = "Base Salary")]
        public int BaseSalary { get; set; }

        public SelectList? Employees { get; set; }
        public SelectList? Roles { get; set; }
    }
}
