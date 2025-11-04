namespace PADMA.Core.Models
{
    public class EclipseData
    {
        public DateTime Date { get; set; }
        public int EclipseId { get; set; } // 1 = Moon, 2 = Sun
        public override string ToString() =>
            $"{EclipseId} | {Date:yyyy-MM-dd HH:mm}";
    }
}