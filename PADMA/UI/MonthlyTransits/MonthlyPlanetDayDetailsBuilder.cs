using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.UI.MonthlyTransits;

public static class MonthlyPlanetDayDetailsBuilder
{
    public static MonthlyPlanetDayDetailsModel Build(
        MonthlyPlanetTransitsData data,
        EPlanet planet,
        DateTime selectedDayLocal)
    {
        var selectedDate = selectedDayLocal.Date;
        var dayStart = selectedDate;
        var dayEnd = selectedDate.AddDays(1);

        var group = data.PlanetGroups
            .FirstOrDefault(x => x.Planet == planet);

        if (group == null)
        {
            return new MonthlyPlanetDayDetailsModel
            {
                Planet = planet,
                SelectedDayLocal = selectedDate,
                Title = GetPlanetName(planet),
                Subtitle = selectedDate.ToString("dd.MM.yyyy"),
                Blocks = []
            };
        }

        var blocks = new List<MonthlyPlanetDayDetailsBlock>();

        AddLaneBlock(
            blocks,
            "Zodiac Sign",
            group,
            MonthlyTransitLaneKind.Zodiac,
            dayStart,
            dayEnd,
            x => x.Text);

        AddLaneBlock(
            blocks,
            "Nakshatra",
            group,
            MonthlyTransitLaneKind.Nakshatra,
            dayStart,
            dayEnd,
            x => x.Text);

        AddLaneBlock(
            blocks,
            "Pada",
            group,
            MonthlyTransitLaneKind.Pada,
            dayStart,
            dayEnd,
            x => x.Text);

        AddLaneBlock(
            blocks,
            "Tara Bala",
            group,
            MonthlyTransitLaneKind.TaraBala,
            dayStart,
            dayEnd,
            x => x.Text);

        AddNavamshaBlock(
            blocks,
            group,
            planet,
            dayStart,
            dayEnd);

        AddSpecialNavamshaBlock(
            blocks,
            group,
            dayStart,
            dayEnd);

        AddMaleficNavamshaBlock(
            blocks,
            group,
            dayStart,
            dayEnd);

        AddDrekkanaBlock(
            blocks,
            group,
            dayStart,
            dayEnd);

        AddMrityuBhagaBlock(
            blocks,
            planet,
            dayStart,
            dayEnd,
            data);

        AddVedhaBlocks(
            blocks,
            data,
            group,
            planet,
            dayStart,
            dayEnd);

        return new MonthlyPlanetDayDetailsModel
        {
            Planet = planet,
            SelectedDayLocal = selectedDate,
            Title = GetPlanetName(planet),
            Subtitle = selectedDate.ToString("dd.MM.yyyy"),
            Blocks = blocks
        };
    }

    private static void AddLaneBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        string title,
        MonthlyPlanetGroup group,
        MonthlyTransitLaneKind laneKind,
        DateTime dayStart,
        DateTime dayEnd,
        Func<MonthlyTransitSegment, string> valueSelector)
    {
        var lane = group.Lanes.FirstOrDefault(x => x.Kind == laneKind);
        if (lane == null)
            return;

        var rows = lane.Segments
            .Where(x => Intersects(GetRealStartLocal(x), GetRealEndLocal(x), dayStart, dayEnd))
            .Select(x => new MonthlyPlanetDayDetailsRow
            {
                StartLocal = GetRealStartLocal(x),
                EndLocal = GetRealEndLocal(x),
                Value = valueSelector(x)
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .OrderBy(x => x.StartLocal)
            .ToList();

        if (rows.Count == 0)
            return;

        blocks.Add(new MonthlyPlanetDayDetailsBlock
        {
            Title = Localize(title),
            Rows = rows
        });
    }

    private static void AddNavamshaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetGroup group,
        EPlanet planet,
        DateTime dayStart,
        DateTime dayEnd)
    {
        var padaLane = group.Lanes.FirstOrDefault(x => x.Kind == MonthlyTransitLaneKind.Pada);
        if (padaLane == null)
            return;

        var rows = new List<MonthlyPlanetDayDetailsRow>();

        foreach (var segment in padaLane.Segments)
        {
            var start = GetRealStartLocal(segment);
            var end = GetRealEndLocal(segment);

            if (!Intersects(start, end, dayStart, dayEnd))
                continue;

            var slice = segment.SourceSlice;
            if (slice == null)
                continue;

            var navamsaText = BuildNavamshaText(planet, slice);
            if (string.IsNullOrWhiteSpace(navamsaText))
                continue;

            rows.Add(new MonthlyPlanetDayDetailsRow
            {
                StartLocal = start,
                EndLocal = end,
                Value = navamsaText
            });
        }

        rows = MergeAdjacentRows(rows);

        if (rows.Count == 0)
            return;

        blocks.Add(new MonthlyPlanetDayDetailsBlock
        {
            Title = Localize("Navamsa"),
            Rows = rows
        });
    }

    private static List<MonthlyPlanetDayDetailsRow> MergeAdjacentRows(
        IReadOnlyList<MonthlyPlanetDayDetailsRow> rows)
    {
        var ordered = rows
            .OrderBy(x => x.StartLocal)
            .ToList();

        if (ordered.Count <= 1)
            return ordered;

        var result = new List<MonthlyPlanetDayDetailsRow>();

        foreach (var row in ordered)
        {
            if (result.Count == 0)
            {
                result.Add(row);
                continue;
            }

            var prev = result[^1];

            var isSameValue = string.Equals(prev.Value, row.Value, StringComparison.Ordinal);
            var isTouching = Math.Abs((row.StartLocal - prev.EndLocal).TotalSeconds) <= 1;

            if (isSameValue && isTouching)
            {
                result[^1] = new MonthlyPlanetDayDetailsRow
                {
                    StartLocal = prev.StartLocal,
                    EndLocal = row.EndLocal,
                    Value = prev.Value
                };
            }
            else
            {
                result.Add(row);
            }
        }

        return result;
    }

    private static string BuildNavamshaText(EPlanet planet, PlanetSlice slice)
    {
        var navamsaZodiacId = slice.NavamsaZodiacId;

        if (navamsaZodiacId < 1 || navamsaZodiacId > 12)
            return string.Empty;

        var zodiacName = GetZodiacName(navamsaZodiacId);
        var marker = BuildNavamshaExaltationMarker(planet, navamsaZodiacId);

        return string.IsNullOrWhiteSpace(marker)
            ? zodiacName
            : $"{zodiacName}{marker}";
    }

    private static string BuildNavamshaExaltationMarker(EPlanet planet, int navamsaZodiacId)
    {
        if (planet == EPlanet.RAHU || planet == EPlanet.KETU)
            return string.Empty;

        var ex = ExaltationUtility.GetPlanetExaltation(
            planet,
            (EZodiac)navamsaZodiacId);

        return ex switch
        {
            EExaltation.EXALTATION => "↑",
            EExaltation.DEBILITATION => "↓",
            _ => string.Empty
        };
    }

    private static bool Intersects(
        DateTime periodStart,
        DateTime periodEnd,
        DateTime dayStart,
        DateTime dayEnd)
    {
        return periodEnd > dayStart && periodStart < dayEnd;
    }

    private static DateTime GetRealStartLocal(MonthlyTransitSegment segment)
    {
        return segment.RealStartLocal == default
            ? segment.StartLocal
            : segment.RealStartLocal;
    }

    private static DateTime GetRealEndLocal(MonthlyTransitSegment segment)
    {
        return segment.RealEndLocal == default
            ? segment.EndLocal
            : segment.RealEndLocal;
    }

    private static string GetPlanetName(EPlanet planet)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        return DataCache.Instance.PlanetDescList
            .FirstOrDefault(x =>
                x.PlanetId == (int)planet &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase))
            ?.Name
            ?? planet.ToString();
    }

    private static string GetZodiacName(int zodiacId)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        return DataCache.Instance.ZodiacDescList
            .FirstOrDefault(x =>
                x.ZodiacId == zodiacId &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase))
            ?.Name
            ?? zodiacId.ToString();
    }

    private static string Localize(string native)
    {
        return Localization.GetLocalizedText(
            native,
            DataCache.Instance.CurrentLanguageCode);
    }

    private static void AddPadaDerivedBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        string title,
        MonthlyPlanetGroup group,
        DateTime dayStart,
        DateTime dayEnd,
        Func<PlanetSlice, string> valueSelector)
    {
        var padaLane = group.Lanes.FirstOrDefault(x => x.Kind == MonthlyTransitLaneKind.Pada);
        if (padaLane == null)
            return;

        var rows = new List<MonthlyPlanetDayDetailsRow>();

        foreach (var segment in padaLane.Segments)
        {
            var start = GetRealStartLocal(segment);
            var end = GetRealEndLocal(segment);

            if (!Intersects(start, end, dayStart, dayEnd))
                continue;

            var slice = segment.SourceSlice;
            if (slice == null)
                continue;

            var value = valueSelector(slice);
            if (string.IsNullOrWhiteSpace(value))
                continue;

            rows.Add(new MonthlyPlanetDayDetailsRow
            {
                StartLocal = start,
                EndLocal = end,
                Value = value
            });
        }

        rows = MergeAdjacentRows(rows);

        if (rows.Count == 0)
            return;

        blocks.Add(new MonthlyPlanetDayDetailsBlock
        {
            Title = Localize(title),
            Rows = rows
        });
    }

    private static void AddSpecialNavamshaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetGroup group,
        DateTime dayStart,
        DateTime dayEnd)
    {
        AddPadaDerivedBlock(
            blocks,
            "Special Navamsa",
            group,
            dayStart,
            dayEnd,
            BuildSpecialNavamshaText);
    }

    private static string BuildSpecialNavamshaText(PlanetSlice slice)
    {
        var pada = DataCache.Instance.PadaList
            .FirstOrDefault(x => x.Id == slice.PadaId);

        if (pada == null)
            return string.Empty;

        var text = PlanetTooltipUtility.GetSpecNavamsha(pada);

        return CleanCommaText(text);
    }

    private static void AddMaleficNavamshaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetGroup group,
        DateTime dayStart,
        DateTime dayEnd)
    {
        AddPadaDerivedBlock(
            blocks,
            "Malefic Navamsa",
            group,
            dayStart,
            dayEnd,
            BuildMaleficNavamshaText);
    }

    private static string BuildMaleficNavamshaText(PlanetSlice slice)
    {
        var ctx = DataCache.Instance.ProfileContextService?.Current;
        if (ctx == null)
            return string.Empty;

        Func<string, string> L = key =>
            Localization.GetLocalizedText(
                key,
                DataCache.Instance.CurrentLanguageCode);

        var text = PlanetTooltipUtility.GetBadNavamsha(
            slice.PadaId,
            ctx.BirthPadaMoonId,
            ctx.BirthPadaLagnaId,
            L);

        return CleanCommaText(text);
    }

    private static void AddDrekkanaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetGroup group,
        DateTime dayStart,
        DateTime dayEnd)
    {
        AddPadaDerivedBlock(
            blocks,
            "Drekkana",
            group,
            dayStart,
            dayEnd,
            BuildDrekkanaText);
    }

    private static string BuildDrekkanaText(PlanetSlice slice)
    {
        var ctx = DataCache.Instance.ProfileContextService?.Current;
        if (ctx == null)
            return string.Empty;

        var lang = DataCache.Instance.CurrentLanguageCode;

        var dList = PlanetTooltipUtility.GetBadDrekkanaList(
            slice.PadaId,
            ctx.BirthPadaMoonId,
            ctx.BirthPadaLagnaId);

        if (dList == null || dList.Count == 0)
            return string.Empty;

        var parts = new List<string>();

        foreach (var de in dList)
        {
            var suffix = de.IsLagna
                ? Localization.GetLocalizedText("Drekkana from Lagna", lang)
                : Localization.GetLocalizedText("Drekkana from Natal Moon", lang);

            parts.Add($"{de.Drekkana} {suffix}");
        }

        return string.Join(", ", parts);
    }

    private static string CleanCommaText(string? text)
    {
        return (text ?? string.Empty)
            .Trim()
            .TrimStart(',')
            .TrimEnd(',')
            .Trim();
    }

    private static void AddMrityuBhagaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        EPlanet planet,
        DateTime dayStartLocal,
        DateTime dayEndLocal,
        MonthlyPlanetTransitsData data)
    {
        var ctx = DataCache.Instance.ProfileContextService?.Current;
        if (ctx == null)
            return;

        var tzInfo = ctx.TimeZoneInfo;
        var nodeMode = DataCache.Instance.GetActiveNodeSetting();

        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);
        var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);

        // Use a wider window than one day so refined MB periods crossing the day are found safely.
        var queryStartUtc = dayStartUtc.AddDays(-14);
        var queryEndUtc = dayEndUtc.AddDays(14);

        var mbList = SwissAnalysis.CalculateMrityuBhagaDataList_London(
            (int)planet,
            queryStartUtc,
            queryEndUtc,
            nodeMode);

        var rows = mbList
            .Select(mb => new
            {
                StartUtc = AsUtc(mb.DateFromUtc),
                EndUtc = AsUtc(mb.DateToUtc)
            })
            .Where(x => x.EndUtc > dayStartUtc && x.StartUtc < dayEndUtc)
            .OrderBy(x => x.StartUtc)
            .Select(x => new MonthlyPlanetDayDetailsRow
            {
                StartLocal = TimeZoneInfo.ConvertTimeFromUtc(x.StartUtc, tzInfo),
                EndLocal = TimeZoneInfo.ConvertTimeFromUtc(x.EndUtc, tzInfo),
                Value = GetPlanetName(planet)
            })
            .ToList();

        if (rows.Count == 0)
            return;

        blocks.Add(new MonthlyPlanetDayDetailsBlock
        {
            Title = Localize("Mrityu Bhaga"),
            Rows = rows
        });
    }

    private static DateTime AsUtc(DateTime dt)
    {
        return dt.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    private static void AddVedhaBlocks(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetTransitsData data,
        MonthlyPlanetGroup group,
        EPlanet planet,
        DateTime dayStartLocal,
        DateTime dayEndLocal)
    {
        var transitSetting = DataCache.Instance.GetActiveTransitSettings();

        if (transitSetting == EAppSetting.TRANZITMOONANDLAGNA)
        {
            AddVedhaBlock(
                blocks,
                data,
                group,
                planet,
                dayStartLocal,
                dayEndLocal,
                forLagna: false);

            AddVedhaBlock(
                blocks,
                data,
                group,
                planet,
                dayStartLocal,
                dayEndLocal,
                forLagna: true);

            return;
        }

        AddVedhaBlock(
            blocks,
            data,
            group,
            planet,
            dayStartLocal,
            dayEndLocal,
            forLagna: transitSetting == EAppSetting.TRANZITLAGNA);
    }

    private static void AddVedhaBlock(
        List<MonthlyPlanetDayDetailsBlock> blocks,
        MonthlyPlanetTransitsData data,
        MonthlyPlanetGroup group,
        EPlanet planet,
        DateTime dayStartLocal,
        DateTime dayEndLocal,
        bool forLagna)
    {
        var ctx = DataCache.Instance.ProfileContextService?.Current;
        if (ctx == null)
            return;

        if (data.TransitPack.Count == 0)
            return;

        if (!data.TransitPack.TryGetValue(planet, out var targetSlices) ||
            targetSlices == null ||
            targetSlices.Count == 0)
            return;

        var tzInfo = ctx.TimeZoneInfo;
        var nodeMode = DataCache.Instance.GetActiveNodeSetting();

        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayStartLocal, tzInfo);
        var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayEndLocal, tzInfo);

        var activeTargetSlices = targetSlices
            .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
            .OrderBy(s => s.StartUtc)
            .ToList();

        if (activeTargetSlices.Count == 0)
            return;

        var allVedhas = new List<VedhaEntity>();

        foreach (var slice in activeTargetSlices)
        {
            int dom = forLagna ? slice.HouseFromLagna : slice.HouseFromMoon;

            var tr = DataCache.Instance.TransitList
                .FirstOrDefault(t => t.PlanetId == slice.PlanetId && t.House == dom);

            if (tr == null)
                continue;

            if (string.IsNullOrWhiteSpace(tr.Vedha) ||
                !int.TryParse(tr.Vedha, out int vedhaDom))
                continue;

            var anchorUtc = MaxUtc(slice.StartUtc, dayStartUtc);

            var (rangeStartUtc, rangeEndUtc) =
                PlanetTooltipUtility.GetContinuousHouseRangeUtc(
                    targetSlices,
                    anchorUtc,
                    isLagna: forLagna);

            if (rangeEndUtc <= rangeStartUtc)
                continue;

            var vedhaList = PlanetTooltipUtility.PrepareVedhaPlanetList(
                targetPlanetId: slice.PlanetId,
                targetStartUtc: rangeStartUtc,
                targetEndUtc: rangeEndUtc,
                transitPack: data.TransitPack,
                vedhaDom: vedhaDom,
                isLagna: forLagna,
                nodeType: nodeMode);

            allVedhas.AddRange(vedhaList);
        }

        if (allVedhas.Count == 0)
            return;

        var merged = PlanetTooltipUtility.MergeVedhaIntervals(allVedhas)
            .Where(v =>
            {
                var s = AsUtc(v.DateStart);
                var e = AsUtc(v.DateEnd);
                return e > dayStartUtc && s < dayEndUtc;
            })
            .OrderBy(v => AsUtc(v.DateStart))
            .ToList();

        if (merged.Count == 0)
            return;

        var rows = merged
            .Select(v =>
            {
                var startUtc = AsUtc(v.DateStart);
                var endUtc = AsUtc(v.DateEnd);

                return new MonthlyPlanetDayDetailsRow
                {
                    StartLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tzInfo),
                    EndLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tzInfo),
                    Value = GetPlanetName(v.PlanetCode)
                };
            })
            .ToList();

        blocks.Add(new MonthlyPlanetDayDetailsBlock
        {
            Title = forLagna
                ? Localize("Vedha from Lagna")
                : Localize("Vedha from Moon"),
            Rows = rows
        });
    }

    private static DateTime MaxUtc(DateTime a, DateTime b)
    {
        return a > b ? a : b;
    }


}