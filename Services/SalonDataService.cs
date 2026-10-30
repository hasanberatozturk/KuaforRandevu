using System.Net.Http.Json;
using KuaforRandevu.Models;

namespace KuaforRandevu.Services;

// HttpClient ile JSON’u asenkron okur. ??= sayesinde aynı dosya bir kez indirilir.

public class SalonDataService
{
    private readonly HttpClient _http;
    private List<SalonService>? _services;
    private List<Employee>? _employees;
    private List<BusySlot>? _busySlots;

    public SalonDataService(HttpClient http) => _http = http;

    public async Task<List<SalonService>> GetServicesAsync() => _services ??= await _http.GetFromJsonAsync<List<SalonService>>("data/services.json") ?? new();
    public async Task<List<Employee>> GetEmployeesAsync() =>
        _employees ??= await _http.GetFromJsonAsync<List<Employee>>("data/employees.json") ?? new();

    public async Task<List<BusySlot>> GetBusySlotsAsync() =>
        _busySlots ??= await _http.GetFromJsonAsync<List<BusySlot>>("data/busy-slots.json") ?? new();

}