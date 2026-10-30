namespace KuaforRandevu.Models;

public class BusySlot
{
    public string EmployeeId { get; set; } = "";
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}