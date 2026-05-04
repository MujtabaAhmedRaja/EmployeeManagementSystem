using System.ComponentModel.DataAnnotations;

namespace EMS.Models;

/// <summary>
/// Represents a salary record linking an employee to a role and an amount.
/// </summary>
public class Salary
{
    /// <summary>
    /// Employee identifier (Part of composite key).
    /// </summary>
    public int Eid { get; set; }

    /// <summary>
    /// Role identifier (Part of composite key).
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Base salary amount.
    /// </summary>
    [Range(10000, int.MaxValue, ErrorMessage = "Salary amount must be at least 10000.")]
    public int Amount { get; set; }

    /// <summary>
    /// Navigation property for the employee.
    /// </summary>
    public Employee? Employee { get; set; }

    /// <summary>
    /// Navigation property for the role.
    /// </summary>
    public Role? Role { get; set; }
}
