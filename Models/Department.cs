namespace EMS.Models;

/// <summary>
/// Represents an organizational department.
/// </summary>
public class Department
{
    /// <summary>
    /// Unique identifier for the department.
    /// </summary>
    public int DepId { get; set; }

    /// <summary>
    /// Name of the department.
    /// </summary>
    public string? DepName { get; set; }

    /// <summary>
    /// Collection of employees belonging to this department.
    /// </summary>
    public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
}
