namespace PADMA.Core.Models;

public class ChartHouseData
{
    public int HouseNumber { get; set; }          // 1..12
    public int ZodiacNumber { get; set; }         // знак в доме
    public List<ChartPlanetItem> Planets { get; set; } = new();
}