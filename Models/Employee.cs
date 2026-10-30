namespace KuaforRandevu.Models;

public class Employee
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<string> ServiceIds { get; set; } = new();
}