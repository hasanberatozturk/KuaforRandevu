using System.Text.Json;
using Microsoft.JSInterop;

namespace KuaforRandevu.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _js;

    public LocalStorageService(IJSRuntime js) => _js = js;

    public async Task SetAsync<T>(string key, T value) =>
            await _js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", key);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

}