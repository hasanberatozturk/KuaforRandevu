namespace KuaforRandevu.Models;

public class Appointment
{
    public string Id { get; set; } = "";
    public string EmployeeId { get; set; } = "";
    public List<string> ServiceIds { get; set; } = new();
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public decimal TotalPrice { get; set; }
    public string CustomerName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool IsCancelled { get; set; }
}