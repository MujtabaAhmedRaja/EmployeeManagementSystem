using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models;

/// <summary>
/// Represents a job role within the organization.
/// </summary>
public class Role
{
    /// <summary>
    /// Unique identifier for the role.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoleId { get; set; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// Collection of salary records associated with this role.
    /// </summary>
    public ICollection<Salary> SalaryRecords { get; set; } = new HashSet<Salary>();
}
