using System.CodeDom.Compiler;
using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

public record TimeSlotOption(DateTime Start, DateTime End, bool IsAvailable, string? Reason);

public class AppointmentService
{
    private const string StorageKey = "kuafor.appointments";
    private static readonly TimeSpan Open = new(9, 0, 0);
    private static readonly TimeSpan Close = new(19, 0, 0);
    private const int StepMinutes = 30;

    private readonly SalonDataService _data;
    private readonly LocalStorageService _storage;
    private List<Appointment> _appointments = new();
    private bool _loaded;

    public AppointmentService(SalonDataService data, LocalStorageService storage)
    {
        _data = data;
        _storage = storage;
    }

    public async Task LoadAsync()
    {
        if (_loaded) return;
        _appointments = await _storage.GetAsync<List<Appointment>>(StorageKey) ?? new();
        _loaded = true;
    }

    public IReadOnlyList<Appointment> GetAll() => _appointments.OrderByDescending(a => a.StartAt).ToList();

    // Seçilen hizmetlerin TAMAMINI verebilen çalışanlar
    public List<Employee> GetEligibleEmployees(List<Employee> all, List<SalonService> selected) =>
            all.Where(e => selected.All(s => e.ServiceIds.Contains(s.Id))).ToList();

    private static bool Overlaps(DateTime newStart, DateTime newEnd, DateTime curStart, DateTime curEnd) => newStart < curEnd && newEnd > curStart;

    public async Task<bool> IsAvailableAsync(string employeeId, DateTime start, DateTime end)
    {
        var busy = await _data.GetBusySlotsAsync();
        bool busyConflict = busy.Any(b => b.EmployeeId == employeeId && Overlaps(start, end, b.StartAt, b.EndAt));
        bool apptConflict = _appointments.Any(a => !a.IsCancelled && a.EmployeeId == employeeId && Overlaps(start, end, a.StartAt, a.EndAt));
        return !busyConflict && !apptConflict;
    }

    public async Task<List<TimeSlotOption>> GetTimeSlotsAsync(string employeeId, DateTime date, int totalMinutes)
    {
        var result = new List<TimeSlotOption>();
        var now = DateTime.Now;

        for (var t = date.Date + Open; t < date.Date + Close; t = t.AddMinutes(StepMinutes))
        {
            var end = t.AddMinutes(totalMinutes);
            string? reason = null;

            if (t <= now) reason = "Geçmiş saat";
            else if (end > date.Date + Close) reason = "Kapanış saatini aşıyor";
            else if (!await IsAvailableAsync(employeeId, t, end)) reason = "Dolu";

            result.Add(new TimeSlotOption(t, end, reason is null, reason));
        }
        return result;
    }

    public async Task<(bool Success, string Message, Appointment? Appointment)> CreateAsync(
        Employee employee, List<SalonService> services, DateTime start, CustomerInfo customer)
    {
        var total = services.Sum(s => s.DurationMinutes);
        var end = start.AddMinutes(total);

        // Onaydan hemen önce uygunluğu YENİDEN kontrol et
        if (start <= DateTime.Now) return (false, "Geçmiş bir saate randevu alınamaz.", null);
        if (end > start.Date + Close) return (false, "Randevu 19.00'u geçemez.", null);
        if (!await IsAvailableAsync(employee.Id, start, end))
            return (false, "Seçilen saat artık uygun değil. Lütfen başka bir saat seçin.", null);

        var appt = new Appointment
        {
            Id = GenerateCode(start.Year),
            EmployeeId = employee.Id,
            ServiceIds = services.Select(s => s.Id).ToList(),
            StartAt = start,
            EndAt = end,
            TotalPrice = services.Sum(s => s.Price),
            CustomerName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone
        };
        _appointments.Add(appt);
        await _storage.SetAsync(StorageKey, _appointments);
        return (true, "Randevunuz oluşturuldu.", appt);
    }

    public async Task<bool> CancelAsync(string id)
    {
        var appt = _appointments.FirstOrDefault(a => a.Id == id);
        if (appt is null || appt.IsCancelled || appt.StartAt <= DateTime.Now) return false;
        appt.IsCancelled = true;
        await _storage.SetAsync(StorageKey, _appointments);
        return true;
    }

    // Benzersiz kod: KFR-2026-101, KFR-2026-102, ...
    private string GenerateCode(int year)
    {
        var prefix = $"KFR-{year}-";
        var max = _appointments.Where(a => a.Id.StartsWith(prefix)).Select(
            a => int.TryParse(a.Id[prefix.Length..], out var n) ? n : 100).DefaultIfEmpty(100).Max();
        return $"{prefix}{max + 1}";
    }
}