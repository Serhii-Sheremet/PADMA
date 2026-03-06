using PADMA.Core.Enums;

namespace PADMA.Core.Models;

public class ChartPlanetItem
{
    public EPlanet PlanetCode { get; set; }
    public double Longitude { get; set; }
    public ETransitType TransitType { get; set; }

    public string Retro { get; set; } = string.Empty;
    public string Exaltation { get; set; } = string.Empty;

    public bool IsActiveAspect { get; set; }

    public EColor ColorCode { get; set; }
}