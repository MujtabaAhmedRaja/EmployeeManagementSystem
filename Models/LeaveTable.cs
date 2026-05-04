using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models;

/// <summary>
/// Represents a leave request submitted by an employee.
/// </summary>
public class LeaveTable
{
    /// <summary>
    /// Unique identifier for the leave request.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LeaveId { get; set; }

    /// <summary>
    /// Employee identifier.
    /// </summary>
    public int Eid { get; set; }

    /// <summary>
    /// Reason for the leave request.
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Status of the leave request (e.g., Pending, Approved, Rejected).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Navigation property for the employee.
    /// </summary>
    public Employee? Employee { get; set; }
}
