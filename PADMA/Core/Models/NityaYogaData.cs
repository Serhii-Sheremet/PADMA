namespace PADMA.Core.Models
{
    public class NityaYogaData
    {
        public DateTime DateTimeUtc { get; set; }   // момент перехода
        public double Longitude { get; set; }       // сидерическая долгота йоги (0..360)
        public int YogaId { get; set; }             // 1..27 (ENityaYoga)

        public override string ToString() =>
            $"{DateTimeUtc:yyyy-MM-dd HH:mm:ss}  |  YogaId={YogaId}  |  Lon={Longitude:F6}";
    }
}
