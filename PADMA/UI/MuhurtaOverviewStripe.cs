namespace PADMA.UI;

public sealed class MuhurtaOverviewStripe
{
    public string Title { get; set; } = string.Empty;      // ShortName (+ " не образуется")
    public bool IsFormed { get; set; } = true;

    public DateTime DayStartLocal { get; set; }
    public DateTime DayEndLocal { get; set; }

    public DateTime? StartLocal { get; set; }              // null если не образуется
    public DateTime? EndLocal { get; set; }

    public string StartText { get; set; } = string.Empty;  // "HH:mm"
    public string EndText { get; set; } = string.Empty;

    public Color SegmentColor { get; set; } = Colors.Transparent;

}
