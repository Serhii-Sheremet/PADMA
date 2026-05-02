using static PADMA.UI.Services.DayComputationService;

namespace PADMA.UI;

public sealed class YogaOverviewStripe
{
    public int YogaId { get; set; }
    public int ColorId { get; set; }
    public string Title { get; set; } = string.Empty;

    public DayOfWeek Vara { get; set; }
    public int NakshatraId { get; set; }
    public int TithiId { get; set; }

    public DateTime DayStartLocal { get; set; }   // 00:00 local
    public DateTime DayEndLocal { get; set; }     // 24:00 local

    public Color SegmentColor { get; set; } = Colors.Transparent;

    public List<YogaTimeSegment> Segments { get; } = new();
}

public sealed class YogaTimeSegment
{
    public DateTime StartLocal { get; set; }      // clamped to civil day
    public DateTime EndLocal { get; set; }        // clamped to civil day

    public bool ShowStartBoundary { get; set; }   // Start > dayStart
    public bool ShowEndBoundary { get; set; }     // End < dayEnd

    public string StartText { get; set; } = "";   // "HH:mm" or ""
    public string EndText { get; set; } = "";     // "HH:mm" or ""

    public List<YogaSegmentDetail> Details { get; } = new();
}

public sealed class YogaSegmentDetail
{
    public DayOfWeek Vara { get; set; }
    public int NakshatraId { get; set; }
    public int TithiId { get; set; }
    public DateTime StartLocal { get; set; }
    public DateTime EndLocal { get; set; }
}

