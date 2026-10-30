using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public class AppointmentStateService
{
    public List<SalonService> SelectedServices { get; set; } = new();
    public Employee? SelectedEmployee { get; set; }
    public DateTime? SelectedDate { get; set; }
    public DateTime? SelectedStart { get; set; }

    public int TotalMinutes => SelectedServices.Sum(s => s.DurationMinutes);
    public decimal TotalPrice => SelectedServices.Sum(s => s.Price);
    public DateTime? SelectedEnd => SelectedStart?.AddMinutes(TotalMinutes);

    public event Action? OnChange;

    public void SetServices(List<SalonService> services)
    {
        SelectedServices = services;
        // Hizmetler değişince önceki çalışan/saat seçimi geçersiz olabilir
        SelectedEmployee = null;
        SelectedStart = null;
        NotifyStateChanged();
    }

    public void Reset()
    {
        SelectedServices = new();
        SelectedEmployee = null;
        SelectedDate = null;
        SelectedStart = null;
        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}