namespace EMS.Models;

public class EmployeeReportModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Department { get; set; }
    public string? Role { get; set; }
    public int Age { get; set; }
    public string? City { get; set; }
    public string Status { get; set; } = "Active";
}

public class AttendanceReportModel
{
    public string? EmployeeName { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public string AttendancePercentage { get; set; } = "0%";
}

public class SalaryReportModel
{
    public string? EmployeeName { get; set; }
    public string? RoleName { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class DashboardViewModel
{
    public int EmployeeCount { get; set; }
    public int DepartmentCount { get; set; }
    public int AttendanceCount { get; set; }
    public int LeaveRequestCount { get; set; }
    public int SalaryEntryCount { get; set; }
    public int RoleCount { get; set; }
}
