namespace PADMA.Core.Models
{
    /// <summary>
    /// Single Tithi change event (exact moment the tithi starts).
    /// </summary>
    public class TithiData
    {
        public DateTime DateTimeUtc { get; set; }
        public double MoonSunDifference { get; set; } // normalized to [0,360)
        public int TithiId { get; set; }              // 1..30

        public override string ToString()
            => $"{DateTimeUtc:yyyy-MM-dd HH:mm:ss} | ΔMS={MoonSunDifference:F4}° | Tithi={TithiId}";
    }
}
