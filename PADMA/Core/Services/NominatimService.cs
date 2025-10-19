using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using PADMA.Core.Models;

namespace PADMA.Core.Services;

public class NominatimService
{
    private readonly HttpClient _http;

    public NominatimService()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri("https://nominatim.openstreetmap.org/")
        };
        // ВАЖНО: укажите читаемый User-Agent (политика Nominatim)
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("PADMA/mobile-beta (contact: serhii.sheremet@gmail.com)");

    }

    public async Task<List<AppLocation>> SearchAsync(string query, string languageCode = "en", int limit = 15)
    {
        // format=jsonv2: современный формат
        // addressdetails=1: чтобы достать город/регион/страну
        var url = $"search?format=jsonv2&q={Uri.EscapeDataString(query)}&addressdetails=1&limit={limit}&accept-language={languageCode}";

        using var resp = await _http.GetAsync(url);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<NominatimResult>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new();

        var result = new List<AppLocation>();
        foreach (var it in items)
        {
            // выбираем удобные поля для UI/сохранения
            var addr = it.Address ?? new Dictionary<string, string>();

            string locality =
                FirstNonEmpty(addr, "city", "town", "village", "hamlet", "municipality", "locality", "county") 
                ?? it.DisplayName ?? "";

            string region = FirstNonEmpty(addr, "region", "state_district", "county") ?? "";
            string state = FirstNonEmpty(addr, "state") ?? "";
            string country = FirstNonEmpty(addr, "country") ?? "";
            string countryCode = FirstNonEmpty(addr, "country_code")?.ToUpperInvariant() ?? "";

            result.Add(new AppLocation
            {
                // Id = 0 (ещё нет в локальной БД)
                Locality = locality,
                Latitude = it.Lat ?? "",
                Longitude = it.Lon ?? "",
                Region = region,
                State = state,
                Country = country,
                CountryCode = countryCode,
                LanguageCode = languageCode
            });
        }
        await Task.Delay(1000);

        return result;
    }

    private static string? FirstNonEmpty(Dictionary<string, string> dict, params string[] keys)
    {
        foreach (var k in keys)
            if (dict.TryGetValue(k, out var v) && !string.IsNullOrWhiteSpace(v))
                return v;
        return null;
    }

    private class NominatimResult
    {
        [JsonPropertyName("display_name")] public string? DisplayName { get; set; }
        [JsonPropertyName("lat")] public string? Lat { get; set; }
        [JsonPropertyName("lon")] public string? Lon { get; set; }
        [JsonPropertyName("address")] public Dictionary<string, string>? Address { get; set; }
    }
}
