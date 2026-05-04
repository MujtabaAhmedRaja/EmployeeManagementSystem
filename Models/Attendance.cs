using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models;

/// <summary>
/// Represents a daily attendance record for an employee.
/// </summary>
public class Attendance
{
    /// <summary>
    /// Unique identifier for the attendance record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AttId { get; set; }

    /// <summary>
    /// Employee identifier.
    /// </summary>
    public int Eid { get; set; }

    /// <summary>
    /// Date of attendance.
    /// </summary>
    public DateTime AttDate { get; set; }

    /// <summary>
    /// Attendance status (e.g., Present, Absent).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Navigation property for the employee.
    /// </summary>
    public Employee? Employee { get; set; }
}
