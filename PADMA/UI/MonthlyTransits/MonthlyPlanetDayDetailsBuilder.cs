using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.TransitBuilder;

namespace PADMA.UI.MonthlyTransits;

public static class MonthlyPlanetDayDetailsBuilder
{
    public static MonthlyPlanetDayDetailsModel Build(
    MonthlyPlanetTransitsData data,
    EPlanet planet,
    DateTime selectedDayLocal)
    {
        var rangeCache = new Dictionary<string, (DateTime StartLocal, DateTime EndLocal)>();
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
                PadaPeriodBlocks = [],
                ExtraBlocks = []
            };
        }

        var padaPeriodBlocks = BuildPadaPeriodBlocks(
            group,
            planet,
            dayStart,
            dayEnd,
            rangeCache);

        var extraBlocks = new List<MonthlyPlanetDayDetailsBlock>();

        AddVedhaBlocks(
            extraBlocks,
            data,
            group,
            planet,
            dayStart,
            dayEnd);

        AddMrityuBhagaBlock(
            extraBlocks,
            planet,
            dayStart,
            dayEnd,
            data);
        

        return new MonthlyPlanetDayDetailsModel
        {
            Planet = planet,
            SelectedDayLocal = selectedDate,
            Title = GetPlanetName(planet),
            Subtitle = selectedDate.ToString("dd.MM.yyyy"),
            PadaPeriodBlocks = padaPeriodBlocks,
            ExtraBlocks = extraBlocks
        };
    }

    private static IReadOnlyList<MonthlyPlanetDayPadaPeriodBlock> BuildPadaPeriodBlocks(
        MonthlyPlanetGroup group,
        EPlanet planet,
        DateTime dayStart,
        DateTime dayEnd,
        Dictionary<string, (DateTime StartLocal, DateTime EndLocal)> rangeCache)
    {
        var padaLane = group.Lanes
            .FirstOrDefault(x => x.Kind == MonthlyTransitLaneKind.Pada);

        if (padaLane == null)
            return [];

        var result = new List<MonthlyPlanetDayPadaPeriodBlock>();

        var padaSegments = padaLane.Segments
            .Where(x => Intersects(GetRealStartLocal(x), GetRealEndLocal(x), dayStart, dayEnd))
            .OrderBy(x => GetRealStartLocal(x))
            .ToList();

        foreach (var segment in padaSegments)
        {
            var slice = segment.SourceSlice;
            if (slice == null)
                continue;

            var start = GetRealStartLocal(segment);
            var end = GetRealEndLocal(segment);

            var lines = BuildPadaPeriodLines(group, planet, segment, rangeCache);

            if (lines.Count == 0)
                continue;

            result.Add(new MonthlyPlanetDayPadaPeriodBlock
            {
                StartLocal = start,
                EndLocal = end,
                Title = BuildPadaPeriodTitle(planet, slice),
                Lines = lines
            });
        }

        return result;
    }

    private static (DateTime StartLocal, DateTime EndLocal) GetRealZodiacRangeLocal(
        EPlanet planet,
        PlanetSlice anchorSlice,
        TimeZoneInfo tzInfo)
    {
        var nodeMode = DataCache.Instance.GetActiveNodeSetting();
        var anchorUtc = GetAnchorUtc(anchorSlice);

        var (startUtc, endUtc) = SwissAnalysis.GetZodiacBoundariesCached(
            (int)planet,
            anchorSlice.ZodiacId,
            anchorUtc,
            nodeMode);

        return (
            TimeZoneInfo.ConvertTimeFromUtc(AsUtc(startUtc), tzInfo),
            TimeZoneInfo.ConvertTimeFromUtc(AsUtc(endUtc), tzInfo));
    }

    private static (DateTime StartLocal, DateTime EndLocal) GetRealNakshatraRangeLocal(
        EPlanet planet,
        PlanetSlice anchorSlice,
        TimeZoneInfo tzInfo)
    {
        var nodeMode = DataCache.Instance.GetActiveNodeSetting();
        var anchorUtc = GetAnchorUtc(anchorSlice);

        var (startUtc, endUtc) = SwissAnalysis.GetNakshatraBoundariesCached(
            (int)planet,
            anchorSlice.NakshatraId,
            anchorUtc,
            nodeMode);

        return (
            TimeZoneInfo.ConvertTimeFromUtc(AsUtc(startUtc), tzInfo),
            TimeZoneInfo.ConvertTimeFromUtc(AsUtc(endUtc), tzInfo));
    }

    private static DateTime GetAnchorUtc(PlanetSlice slice)
    {
        return slice.StartUtc + TimeSpan.FromTicks(
            Math.Max(1, (slice.EndUtc - slice.StartUtc).Ticks / 2));
    }

    private static TimeSpan GetInitialSearchStep(EPlanet planet)
    {
        return planet switch
        {
            EPlanet.MOON => TimeSpan.FromHours(2),
            EPlanet.SUN => TimeSpan.FromDays(1),
            EPlanet.MERCURY => TimeSpan.FromDays(1),
            EPlanet.VENUS => TimeSpan.FromDays(1),
            EPlanet.MARS => TimeSpan.FromDays(2),
            EPlanet.JUPITER => TimeSpan.FromDays(7),
            EPlanet.SATURN => TimeSpan.FromDays(14),
            EPlanet.RAHU => TimeSpan.FromDays(7),
            EPlanet.KETU => TimeSpan.FromDays(7),
            _ => TimeSpan.FromDays(1)
        };
    }

    private static DateTime BinarySearchFirstTargetUtc(
    DateTime outsideUtc,
    DateTime insideUtc,
    Func<DateTime, bool> isTarget)
    {
        var low = outsideUtc; // not target
        var high = insideUtc; // target

        while ((high - low).TotalSeconds > 1)
        {
            var mid = low + TimeSpan.FromTicks((high - low).Ticks / 2);

            if (isTarget(mid))
                high = mid;
            else
                low = mid;
        }

        return high;
    }

    private static (DateTime StartUtc, DateTime EndUtc) FindContinuousStateRangeUtc(
        EPlanet planet,
        DateTime anchorUtc,
        Func<PlanetSlice, int> stateSelector,
        int targetState,
        TimeSpan initialStep,
        TimeSpan maxSearchSpan)
    {
        var ctx = DataCache.Instance.ProfileContextService?.Current;
        if (ctx == null)
            return (anchorUtc, anchorUtc);

        var nodeMode = DataCache.Instance.GetActiveNodeSetting();

        bool IsTarget(DateTime utc)
        {
            var s = GetPlanetSliceAtUtc(planet, utc, ctx, nodeMode);
            return s != null && stateSelector(s) == targetState;
        }

        var startUtc = FindBackwardBoundaryUtc(
            anchorUtc,
            IsTarget,
            initialStep,
            maxSearchSpan);

        var endUtc = FindForwardBoundaryUtc(
            anchorUtc,
            IsTarget,
            initialStep,
            maxSearchSpan);

        return (startUtc, endUtc);
    }

    private static DateTime FindBackwardBoundaryUtc(
        DateTime anchorUtc,
        Func<DateTime, bool> isTarget,
        TimeSpan initialStep,
        TimeSpan maxSearchSpan)
    {
        var inside = anchorUtc;
        var step = initialStep;

        var minUtc = anchorUtc - maxSearchSpan;
        var outside = inside - step;

        while (outside > minUtc && isTarget(outside))
        {
            inside = outside;
            step = TimeSpan.FromTicks(step.Ticks * 2);
            outside = inside - step;
        }

        if (outside < minUtc)
            outside = minUtc;

        return BinarySearchFirstTargetUtc(outside, inside, isTarget);
    }

    private static DateTime FindForwardBoundaryUtc(
        DateTime anchorUtc,
        Func<DateTime, bool> isTarget,
        TimeSpan initialStep,
        TimeSpan maxSearchSpan)
    {
        var inside = anchorUtc;
        var step = initialStep;

        var maxUtc = anchorUtc + maxSearchSpan;
        var outside = inside + step;

        while (outside < maxUtc && isTarget(outside))
        {
            inside = outside;
            step = TimeSpan.FromTicks(step.Ticks * 2);
            outside = inside + step;
        }

        if (outside > maxUtc)
            outside = maxUtc;

        return BinarySearchFirstNonTargetUtc(inside, outside, isTarget);
    }

    private static DateTime BinarySearchFirstNonTargetUtc(
        DateTime insideUtc,
        DateTime outsideUtc,
        Func<DateTime, bool> isTarget)
    {
        var low = insideUtc; // target
        var high = outsideUtc; // not target

        while ((high - low).TotalSeconds > 1)
        {
            var mid = low + TimeSpan.FromTicks((high - low).Ticks / 2);

            if (isTarget(mid))
                low = mid;
            else
                high = mid;
        }

        return high;
    }

    private static PlanetSlice? GetPlanetSliceAtUtc(
        EPlanet planet,
        DateTime utc,
        ProfileTransitContext ctx,
        EAppSetting nodeMode)
    {
        var startUtc = utc.AddDays(-2);
        var endUtc = utc.AddDays(2);

        List<PlanetData> planetData;

        if (planet == EPlanet.KETU)
        {
            var rahuData = SwissAnalysis.CalculatePlanetDataList_London(
                (int)EPlanet.RAHU,
                startUtc,
                endUtc,
                nodeMode,
                true);

            planetData = rahuData
                .Select(SwissAnalysis.CalculateKetuData)
                .ToList();
        }
        else
        {
            planetData = SwissAnalysis.CalculatePlanetDataList_London(
                (int)planet,
                startUtc,
                endUtc,
                nodeMode,
                true);
        }

        var slices = PlanetTransitBuilder.BuildPlanetSlices(
                planetData,
                ctx.BirthNakshatraMoonId,
                ctx.BirthZodiacMoonId,
                ctx.BirthLagnaId,
                nodeMode)
            .OrderBy(x => x.StartUtc)
            .ToList();

        return slices.FirstOrDefault(x => x.StartUtc <= utc && x.EndUtc > utc)
            ?? slices.OrderBy(x => Math.Abs((x.StartUtc - utc).TotalSeconds)).FirstOrDefault();
    }

    private static IReadOnlyList<MonthlyPlanetDayDetailsLine> BuildPadaPeriodLines(
        MonthlyPlanetGroup group,
        EPlanet planet,
        MonthlyTransitSegment padaSegment,
        Dictionary<string, (DateTime StartLocal, DateTime EndLocal)> rangeCache)
    {
        var lines = new List<MonthlyPlanetDayDetailsLine>();

        var slice = padaSegment.SourceSlice;
        if (slice == null)
            return lines;

        var padaStart = GetRealStartLocal(padaSegment);
        var padaEnd = GetRealEndLocal(padaSegment);

        var ctx = DataCache.Instance.ProfileContextService?.Current;

        (DateTime StartLocal, DateTime EndLocal)? zodiacRange = null;
        (DateTime StartLocal, DateTime EndLocal)? nakshatraRange = null;

        if (ctx != null && ShouldUseRealRangeCalculation(planet))
        {
            zodiacRange = GetCachedRealZodiacRangeLocal(
                planet,
                slice,
                ctx.TimeZoneInfo,
                rangeCache);

            nakshatraRange = GetCachedRealNakshatraRangeLocal(
                planet,
                slice,
                ctx.TimeZoneInfo,
                rangeCache);
        }
        else
        {
            var zodiacSegment = FindRelatedSegment(
                group,
                MonthlyTransitLaneKind.Zodiac,
                padaStart,
                padaEnd);

            if (zodiacSegment != null)
            {
                zodiacRange = (
                    GetRealStartLocal(zodiacSegment),
                    GetRealEndLocal(zodiacSegment));
            }

            var nakshatraSegment = FindRelatedSegment(
                group,
                MonthlyTransitLaneKind.Nakshatra,
                padaStart,
                padaEnd);

            if (nakshatraSegment != null)
            {
                nakshatraRange = (
                    GetRealStartLocal(nakshatraSegment),
                    GetRealEndLocal(nakshatraSegment));
            }
        }

        if (zodiacRange.HasValue)
        {
            AddLine(
                lines,
                "Zodiac Sign",
                BuildZodiacText(planet, slice),
                zodiacRange.Value.StartLocal,
                zodiacRange.Value.EndLocal);
        }
        else
        {
            AddLine(lines, "Zodiac Sign", BuildZodiacText(planet, slice));
        }

        if (nakshatraRange.HasValue)
        {
            AddLine(
                lines,
                "Nakshatra",
                BuildNakshatraText(planet, slice),
                nakshatraRange.Value.StartLocal,
                nakshatraRange.Value.EndLocal);
        }
        else
        {
            AddLine(lines, "Nakshatra", BuildNakshatraText(planet, slice));
        }

        AddLine(
            lines,
            "Pada",
            BuildPadaNumberText(slice),
            padaStart,
            padaEnd);

        if (nakshatraRange.HasValue)
        {
            AddLine(
                lines,
                "Tara Bala",
                BuildTaraBalaText(planet, slice),
                nakshatraRange.Value.StartLocal,
                nakshatraRange.Value.EndLocal);
        }
        else
        {
            AddLine(lines, "Tara Bala", BuildTaraBalaText(planet, slice));
        }

        AddLine(
            lines,
            "Navamsa",
            BuildNavamshaText(planet, slice),
            padaStart,
            padaEnd);

        AddLine(lines, "Special Navamsa", BuildSpecialNavamshaText(slice));
        AddLine(lines, "Malefic Navamsa", BuildMaleficNavamshaText(slice));
        AddLine(lines, "Drekkana", BuildDrekkanaText(slice));

        return lines;
    }

    private static (DateTime StartLocal, DateTime EndLocal) GetCachedRealZodiacRangeLocal(
        EPlanet planet,
        PlanetSlice slice,
        TimeZoneInfo tzInfo,
        Dictionary<string, (DateTime StartLocal, DateTime EndLocal)> cache)
    {
        var anchorUtc = GetAnchorUtc(slice);
        var key = $"{planet}|zodiac|{slice.ZodiacId}|{anchorUtc:yyyyMMddHHmmss}";

        if (cache.TryGetValue(key, out var cached))
            return cached;

        var value = GetRealZodiacRangeLocal(planet, slice, tzInfo);
        cache[key] = value;
        return value;
    }

    private static (DateTime StartLocal, DateTime EndLocal) GetCachedRealNakshatraRangeLocal(
        EPlanet planet,
        PlanetSlice slice,
        TimeZoneInfo tzInfo,
        Dictionary<string, (DateTime StartLocal, DateTime EndLocal)> cache)
    {
        var anchorUtc = GetAnchorUtc(slice);
        var key = $"{planet}|nakshatra|{slice.NakshatraId}|{anchorUtc:yyyyMMddHHmmss}";

        if (cache.TryGetValue(key, out var cached))
            return cached;

        var value = GetRealNakshatraRangeLocal(planet, slice, tzInfo);
        cache[key] = value;
        return value;
    }

    private static MonthlyTransitSegment? FindRelatedSegment(
        MonthlyPlanetGroup group,
        MonthlyTransitLaneKind laneKind,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var lane = group.Lanes.FirstOrDefault(x => x.Kind == laneKind);
        if (lane == null)
            return null;

        return lane.Segments
            .Where(x => Intersects(
                GetRealStartLocal(x),
                GetRealEndLocal(x),
                periodStart,
                periodEnd))
            .OrderByDescending(x =>
            {
                var start = Max(GetRealStartLocal(x), periodStart);
                var end = Min(GetRealEndLocal(x), periodEnd);
                return (end - start).Ticks;
            })
            .FirstOrDefault();
    }

    private static string BuildPadaPeriodTitle(EPlanet planet, PlanetSlice slice)
    {
        var zodiacName = GetZodiacName(slice.ZodiacId);
        var nakshatraName = GetNakshatraShortName(slice.NakshatraId);

        var padaNumber = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);
        var navamsaZodiacId = slice.NavamsaZodiacId;

        var navamsaMarker = BuildNavamshaExaltationMarker(planet, navamsaZodiacId);

        var nakshatraPart = string.IsNullOrWhiteSpace(nakshatraName)
            ? slice.NakshatraId.ToString()
            : nakshatraName;

        return $"{zodiacName} - {nakshatraPart} - {padaNumber} - {navamsaZodiacId}{navamsaMarker}";
    }

    private static string BuildZodiacText(EPlanet planet, PlanetSlice slice)
    {
        var zodiac = GetZodiacName(slice.ZodiacId);
        var marker = BuildZodiacStateMarker(planet, slice);

        return string.IsNullOrWhiteSpace(marker)
            ? zodiac
            : $"{zodiac}{marker}";
    }

    private static string BuildZodiacStateMarker(EPlanet planet, PlanetSlice slice)
    {
        if (planet == EPlanet.RAHU || planet == EPlanet.KETU)
            return string.Empty;

        if (slice.IsRetrograde)
            return ".R";

        var ex = ExaltationUtility.GetPlanetExaltation(
            planet,
            (EZodiac)slice.ZodiacId);

        return ex switch
        {
            EExaltation.EXALTATION => "↑",
            EExaltation.DEBILITATION => "↓",
            _ => string.Empty
        };
    }

    private static string BuildNakshatraText(EPlanet planet, PlanetSlice slice)
    {
        _ = planet;

        var name = GetNakshatraShortName(slice.NakshatraId);

        return string.IsNullOrWhiteSpace(name)
            ? slice.NakshatraId.ToString()
            : $"{slice.NakshatraId}.{name}";
    }

    private static string GetNakshatraShortName(int nakshatraId)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        var desc = DataCache.Instance.NakshatraDescList
            .FirstOrDefault(x =>
                x.NakshatraId == nakshatraId &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase));

        if (desc == null)
            return string.Empty;

        return !string.IsNullOrWhiteSpace(desc.ShortName)
            ? desc.ShortName
            : desc.Name ?? string.Empty;
    }

    private static string BuildPadaNumberText(PlanetSlice slice)
    {
        var padaNumber = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);
        return padaNumber > 0 ? padaNumber.ToString() : string.Empty;
    }

    private static string BuildTaraBalaText(EPlanet planet, PlanetSlice slice)
    {
        _ = planet;

        var name = GetTaraBalaName(slice.TaraBalaId);

        var prefix = string.IsNullOrWhiteSpace(name)
            ? slice.TaraBalaId.ToString()
            : $"{slice.TaraBalaId}.{name}";

        return $"{prefix} {slice.TaraBalaPercent}%";
    }

    private static string GetTaraBalaName(int taraBalaId)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        var desc = DataCache.Instance.TaraBalaDescList
            .FirstOrDefault(x =>
                x.TaraBalaId == taraBalaId &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase));

        if (desc == null)
            return string.Empty;

        return !string.IsNullOrWhiteSpace(desc.ShortName)
            ? desc.ShortName
            : desc.Name ?? string.Empty;
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

    private static void AddLine(
        List<MonthlyPlanetDayDetailsLine> lines,
        string labelNative,
        string value,
        DateTime? startLocal = null,
        DateTime? endLocal = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        lines.Add(new MonthlyPlanetDayDetailsLine
        {
            Label = Localize(labelNative),
            Value = value,
            StartLocal = startLocal,
            EndLocal = endLocal
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

    private static string BuildSpecialNavamshaText(PlanetSlice slice)
    {
        var pada = DataCache.Instance.PadaList
            .FirstOrDefault(x => x.Id == slice.PadaId);

        if (pada == null)
            return string.Empty;

        var text = PlanetTooltipUtility.GetSpecNavamsha(pada);

        return CleanCommaText(text);
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

    private static DateTime Max(DateTime a, DateTime b)
    {
        return a > b ? a : b;
    }

    private static DateTime Min(DateTime a, DateTime b)
    {
        return a < b ? a : b;
    }

    private static bool ShouldUseRealRangeCalculation(EPlanet planet)
    {
        // Moon is fast enough; the monthly -7/+7 buffer always covers
        // zodiac/nakshatra/tara-bala boundaries for selected day tooltip.
        return planet != EPlanet.MOON;
    }


}