namespace EMS.Models;

/// <summary>
/// Represents an activity log entry in the system.
/// </summary>
public class Log
{
    /// <summary>
    /// Unique identifier for the log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the database table affected.
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// Description of the action performed.
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// Date and time of the action.
    /// </summary>
    public DateTime ActionDate { get; set; }
}
