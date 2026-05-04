using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models;

/// <summary>
/// Represents an employee in the system.
/// </summary>
public class Employee
{
    /// <summary>
    /// Unique identifier for the employee.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Eid { get; set; }

    /// <summary>
    /// Full name of the employee.
    /// </summary>
    public string? EName { get; set; }

    /// <summary>
    /// Age of the employee.
    /// </summary>
    public int EAge { get; set; }

    /// <summary>
    /// City of residence.
    /// </summary>
    public string? ECity { get; set; }

    /// <summary>
    /// Department identifier.
    /// </summary>
    public int DepId { get; set; }

    /// <summary>
    /// Navigation property for the department.
    /// </summary>
    public Department? Department { get; set; }

    /// <summary>
    /// Collection of attendance records for this employee.
    /// </summary>
    public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();

    /// <summary>
    /// Collection of leave requests for this employee.
    /// </summary>
    public ICollection<LeaveTable> LeaveRequests { get; set; } = new HashSet<LeaveTable>();

    /// <summary>
    /// Collection of salary and role records for this employee.
    /// </summary>
    public ICollection<Salary> SalaryRecords { get; set; } = new HashSet<Salary>();
}
