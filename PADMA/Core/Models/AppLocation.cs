namespace PADMA.Core.Models;

public class AppLocation
{
    public int Id { get; set; }

    public string Locality { get; set; } = string.Empty;

    public string Latitude { get; set; } = string.Empty;

    public string Longitude { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public string LanguageCode { get; set; } = string.Empty;
}
