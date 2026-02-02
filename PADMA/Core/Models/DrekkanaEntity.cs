namespace PADMA.Core.Models
{
    public class DrekkanaEntity
    {
        public int Drekkana { get; set; }
        public int NakshatraId { get; set; }
        public int PadaId { get; set; }     // это Pada.Id (1..108)
        public bool IsLagna { get; set; }
    }
}