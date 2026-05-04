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
