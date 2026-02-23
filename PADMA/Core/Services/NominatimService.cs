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

    public async Task<List<AppLocation>> SearchAsync(string query, string langCode, string? countryCode = null)
    {
        var ccFilter = string.IsNullOrWhiteSpace(countryCode)
                ? null
                : countryCode.Trim().ToLowerInvariant();

        var url =
            $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&addressdetails=1&accept-language={langCode}&limit=20";

        if (!string.IsNullOrWhiteSpace(ccFilter))
            url += $"&countrycodes={Uri.EscapeDataString(ccFilter)}";
        
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<JsonElement>>(json);

        var list = new List<AppLocation>();

        foreach (var item in results)
        {
            try
            {
                var clazz = item.TryGetProperty("class", out var cl) ? cl.GetString() ?? "" : "";
                var type = item.TryGetProperty("type", out var tp) ? tp.GetString() ?? "" : "";

                var address = item.GetProperty("address");

                string locality =
                    address.TryGetProperty("city", out var c1) ? c1.GetString() :
                    address.TryGetProperty("town", out var c2) ? c2.GetString() :
                    address.TryGetProperty("village", out var c3) ? c3.GetString() :
                    address.TryGetProperty("hamlet", out var c4) ? c4.GetString() :
                    address.TryGetProperty("municipality", out var c5) ? c5.GetString() :
                    address.TryGetProperty("locality", out var c6) ? c6.GetString() :
                    null;

                // Пропускаем если вообще нет признаков населённого пункта
                if (string.IsNullOrWhiteSpace(locality))
                    continue;

                // теперь берём регион и страну
                string region =
                    address.TryGetProperty("state", out var s1) ? s1.GetString() :
                    address.TryGetProperty("region", out var s2) ? s2.GetString() :
                    address.TryGetProperty("province", out var s3) ? s3.GetString() :
                    address.TryGetProperty("county", out var s4) ? s4.GetString() :
                    null;

                string country =
                    address.TryGetProperty("country", out var cn) ? cn.GetString() : "";

                string countryCodeFromApi =
                    address.TryGetProperty("country_code", out var cc) ? (cc.GetString() ?? "").ToUpperInvariant() : "";

                string lat = item.GetProperty("lat").GetString() ?? "0";
                string lon = item.GetProperty("lon").GetString() ?? "0";

                // --- формируем читаемое имя ---
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(locality)) parts.Add(locality);
                if (!string.IsNullOrWhiteSpace(region) && !parts.Contains(region)) parts.Add(region);
                if (!string.IsNullOrWhiteSpace(country) && !parts.Contains(country)) parts.Add(country);
                string displayName = string.Join(", ", parts);

                // --- фильтрация дубликатов ---
                if (list.Any(l =>
                        string.Equals(l.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)))
                    continue;

                list.Add(new AppLocation
                {
                    Locality = locality ?? "",
                    Region = region,
                    Country = country,
                    CountryCode = countryCodeFromApi,
                    LanguageCode = langCode,
                    Latitude = lat,
                    Longitude = lon,
                    DisplayName = displayName,
                    CoordinatesString = $"Lat: {lat}, Lon: {lon}"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] Nominatim parse error: {ex.Message}");
            }
        }

        return list;
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
