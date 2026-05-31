using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.TransitBuilder;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyPlanetTransitsDataService
{
    private static readonly EPlanet[] PlanetOrder =
    [
        EPlanet.SUN,
        EPlanet.MOON,
        EPlanet.MARS,
        EPlanet.MERCURY,
        EPlanet.JUPITER,
        EPlanet.VENUS,
        EPlanet.SATURN,
        EPlanet.RAHU,
        EPlanet.KETU
    ];

    public async Task<MonthlyPlanetTransitsData> BuildAsync(
        int year,
        int month,
        CancellationToken ct = default)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        var ctx = DataCache.Instance.ProfileContextService.Current
            ?? await DataCache.Instance.ProfileContextService.RebuildAsync(ct);

        var tzInfo = ctx.TimeZoneInfo;

        var monthStartLocal = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var monthEndLocal = monthStartLocal.AddMonths(1);

        // Masa shunya calculation requires a larger buffer to correctly determine the lunar month for intervals that start/end near month boundaries.
        var masaBufferStartLocal = monthStartLocal.AddDays(-45);
        var masaBufferEndLocal = monthEndLocal.AddDays(45);

        var masaBufferStartUtc = TimeZoneInfo.ConvertTimeToUtc(masaBufferStartLocal, tzInfo);
        var masaBufferEndUtc = TimeZoneInfo.ConvertTimeToUtc(masaBufferEndLocal, tzInfo);

        // Buffer is required to correctly detect intervals that started before the visible month
        // or end after the visible month.
        var bufferStartLocal = monthStartLocal.AddDays(-7);
        var bufferEndLocal = monthEndLocal.AddDays(7);

        var bufferStartUtc = TimeZoneInfo.ConvertTimeToUtc(bufferStartLocal, tzInfo);
        var bufferEndUtc = TimeZoneInfo.ConvertTimeToUtc(bufferEndLocal, tzInfo);

        var nodeMode = DataCache.Instance.GetActiveNodeSetting();
        var transitMode = DataCache.Instance.GetActiveTransitSettings();

        var moonDataForMasa = SwissAnalysis.CalculatePlanetDataList_London(
            (int)EPlanet.MOON,
            masaBufferStartUtc,
            masaBufferEndUtc,
            nodeMode,
            true);
        var tithiDataForMasa = SwissAnalysis.CalculateTithiDataList_London(
            masaBufferStartUtc,
            masaBufferEndUtc,
            nodeMode);

        var masaShunya = MasaShunyaBuilder.Build(
            moonDataForMasa,
            tithiDataForMasa,
            masaBufferStartUtc,
            masaBufferEndUtc,
            monthStartLocal,
            monthEndLocal,
            tzInfo);

        var groups = new List<MonthlyPlanetGroup>();

        List<PlanetData>? rahuDataCache = null;

        foreach (var planet in PlanetOrder)
        {
            ct.ThrowIfCancellationRequested();

            List<PlanetData> planetData;

            if (planet == EPlanet.KETU)
            {
                rahuDataCache ??= SwissAnalysis.CalculatePlanetDataList_London(
                    (int)EPlanet.RAHU,
                    bufferStartUtc,
                    bufferEndUtc,
                    nodeMode,
                    true);

                planetData = rahuDataCache
                    .Select(SwissAnalysis.CalculateKetuData)
                    .ToList();
            }
            else
            {
                planetData = SwissAnalysis.CalculatePlanetDataList_London(
                    (int)planet,
                    bufferStartUtc,
                    bufferEndUtc,
                    nodeMode,
                    true);

                if (planet == EPlanet.RAHU)
                    rahuDataCache = planetData;
            }

            var slices = PlanetTransitBuilder.BuildPlanetSlices(
                planetData,
                ctx.BirthNakshatraMoonId,
                ctx.BirthZodiacMoonId,
                ctx.BirthLagnaId,
                nodeMode);

            var clippedSlices = slices
                .Where(s => s.EndUtc > bufferStartUtc && s.StartUtc < bufferEndUtc)
                .OrderBy(s => s.StartUtc)
                .ToList();

            var mrityuBhagaSegments = BuildMrityuBhagaOverlays(
                planet,
                bufferStartUtc,
                bufferEndUtc,
                monthStartLocal,
                monthEndLocal,
                tzInfo,
                nodeMode);

            var group = new MonthlyPlanetGroup
            {
                Planet = planet,
                PlanetName = GetPlanetName(planet),
                Lanes =
                [
                    BuildZodiacLane(
                        planet,
                        clippedSlices,
                        mrityuBhagaSegments,
                        monthStartLocal,
                        monthEndLocal,
                        tzInfo,
                        transitMode),

                    BuildNakshatraLane(
                        planet,
                        clippedSlices,
                        monthStartLocal,
                        monthEndLocal,
                        tzInfo,
                        transitMode),

                    BuildPadaLane(
                        planet,
                        clippedSlices,
                        monthStartLocal,
                        monthEndLocal,
                        tzInfo),

                    BuildTaraBalaLane(
                        planet,
                        clippedSlices,
                        monthStartLocal,
                        monthEndLocal,
                        tzInfo)

                ]
            };

            groups.Add(group);
        }

        return new MonthlyPlanetTransitsData
        {
            Year = year,
            Month = month,
            MonthStartLocal = monthStartLocal,
            MonthEndLocal = monthEndLocal,
            MasaShunya = masaShunya,
            PlanetGroups = groups
        };
    }

    private static MonthlyTransitLane BuildZodiacLane(
        EPlanet planet,
        IReadOnlyList<PlanetSlice> slices,
        IReadOnlyList<MonthlyTransitOverlaySegment> mrityuBhagaOverlays,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo,
        EAppSetting transitMode)
    {
        var segments = new List<MonthlyTransitSegment>();

        PlanetSlice? currentSlice = null;

        foreach (var slice in slices.OrderBy(s => s.StartUtc))
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            // Zodiac lane changes only when zodiac or retrograde state changes.
            // Pada/nakshatra-only changes are ignored here.
            if (currentSlice != null &&
                currentSlice.ZodiacId == slice.ZodiacId &&
                currentSlice.IsRetrograde == slice.IsRetrograde)
            {
                continue;
            }

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            var color = TransitColorResolver.Resolve(slice, transitMode);

            segments.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildZodiacText(planet, slice),
                IsSplitColor = color.IsSplitColor,
                Color = color.Color,
                ColorTop = color.ColorTop,
                ColorBottom = color.ColorBottom,
                SourceSlice = slice
            });

            currentSlice = slice;
        }

        // Important: the loop above may miss the active slice at month start if no zodiac change
        // happens exactly inside the visible month. Fill gaps using the nearest previous slice.
        segments = NormalizeSegmentsForMonth(
            planet,
            slices,
            segments,
            monthStartLocal,
            monthEndLocal,
            tzInfo,
            transitMode);

        return new MonthlyTransitLane
        {
            Kind = MonthlyTransitLaneKind.Zodiac,
            Title = "Zodiac",
            Segments = segments,
            Overlays = mrityuBhagaOverlays
        };
    }

    private static IReadOnlyList<MonthlyTransitOverlaySegment> BuildMrityuBhagaOverlays(
        EPlanet planet,
        DateTime bufferStartUtc,
        DateTime bufferEndUtc,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo,
        EAppSetting nodeMode)
    {
        var result = new List<MonthlyTransitOverlaySegment>();

        var mbList = SwissAnalysis.CalculateMrityuBhagaDataList_London(
            (int)planet,
            bufferStartUtc,
            bufferEndUtc,
            nodeMode);

        if (mbList.Count == 0)
            return result;

        var mbColor = DataCache.Instance.GetColor(EColor.MRITYUBHAGA);

        foreach (var mb in mbList.OrderBy(x => x.DateFromUtc))
        {
            var startUtc = AsUtc(mb.DateFromUtc);
            var endUtc = AsUtc(mb.DateToUtc);

            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            result.Add(new MonthlyTransitOverlaySegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Color = mbColor
            });
        }

        return result;
    }

    private static DateTime AsUtc(DateTime dt)
    {
        return dt.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    private static List<MonthlyTransitSegment> NormalizeSegmentsForMonth(
        EPlanet planet,
        IReadOnlyList<PlanetSlice> slices,
        List<MonthlyTransitSegment> rawSegments,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo,
        EAppSetting transitMode)
    {
        // For the first implementation, rebuild zodiac segments in a simpler and safer way:
        // scan all slices, convert each zodiac state interval to local time, clip to month,
        // then merge adjacent visible segments with the same zodiac/retrograde state.

        var result = new List<MonthlyTransitSegment>();

        foreach (var slice in slices.OrderBy(s => s.StartUtc))
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            if (result.Count > 0)
            {
                var prev = result[^1].SourceSlice;

                if (prev != null &&
                    prev.ZodiacId == slice.ZodiacId &&
                    prev.IsRetrograde == slice.IsRetrograde)
                {
                    var previous = result[^1];

                    result[^1] = new MonthlyTransitSegment
                    {
                        StartLocal = previous.StartLocal,
                        EndLocal = visibleEnd,
                        Text = previous.Text,
                        Color = previous.Color,
                        ColorTop = previous.ColorTop,
                        ColorBottom = previous.ColorBottom,
                        IsSplitColor = previous.IsSplitColor,
                        SourceSlice = previous.SourceSlice
                    };

                    continue;
                }
            }

            var color = TransitColorResolver.Resolve(slice, transitMode);

            result.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildZodiacText(planet, slice),
                Color = color.Color,
                ColorTop = color.ColorTop,
                ColorBottom = color.ColorBottom,
                IsSplitColor = color.IsSplitColor,
                SourceSlice = slice
            });
        }

        return result;
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
        if (IsNodePlanet(planet))
            return string.Empty;

        if (slice.IsRetrograde)
            return ".R";

        var ex = ExaltationUtility.GetPlanetExaltation(planet, (EZodiac)slice.ZodiacId);

        return ex switch
        {
            EExaltation.EXALTATION => "↑",
            EExaltation.DEBILITATION => "↓",
            _ => string.Empty
        };
    }

    private static bool IsNodePlanet(EPlanet planet)
    {
        return planet == EPlanet.RAHU || planet == EPlanet.KETU;
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

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

    private static MonthlyTransitLane BuildNakshatraLane(
        EPlanet planet,
        IReadOnlyList<PlanetSlice> slices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo,
        EAppSetting transitMode)
    {
        var segments = new List<MonthlyTransitSegment>();

        foreach (var slice in slices.OrderBy(s => s.StartUtc))
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            if (segments.Count > 0)
            {
                var prev = segments[^1].SourceSlice;

                if (prev != null &&
                    prev.NakshatraId == slice.NakshatraId &&
                    prev.IsRetrograde == slice.IsRetrograde)
                {
                    var previous = segments[^1];

                    
                    segments[^1] = new MonthlyTransitSegment
                    {
                        StartLocal = previous.StartLocal,
                        EndLocal = visibleEnd,
                        Text = previous.Text,
                        Color = previous.Color,
                        ColorTop = previous.ColorTop,
                        ColorBottom = previous.ColorBottom,
                        IsSplitColor = previous.IsSplitColor,
                        SourceSlice = previous.SourceSlice
                    };

                    continue;
                }
            }

            var color = GetNakshatraColor(slice);

            segments.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildNakshatraText(planet, slice),
                Color = color,
                ColorTop = null,
                ColorBottom = null,
                IsSplitColor = false,
                SourceSlice = slice
            });
        }

        return new MonthlyTransitLane
        {
            Kind = MonthlyTransitLaneKind.Nakshatra,
            Title = "Nakshatra",
            Segments = segments
        };
    }

    private static string BuildNakshatraText(EPlanet planet, PlanetSlice slice)
    {
        if (planet == EPlanet.MOON)
            return slice.NakshatraId.ToString();

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

    private static Color GetNakshatraColor(PlanetSlice slice)
    {
        var nakshatra = DataCache.Instance.NakshatraList
            .FirstOrDefault(x => x.Id == slice.NakshatraId);

        if (nakshatra == null)
            return Colors.Transparent;

        return DataCache.Instance.GetColor((EColor)nakshatra.ColorId);
    }

    private static MonthlyTransitLane BuildPadaLane(
        EPlanet planet,
        IReadOnlyList<PlanetSlice> slices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var segments = new List<MonthlyTransitSegment>();

        foreach (var slice in slices.OrderBy(s => s.StartUtc))
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            if (segments.Count > 0)
            {
                var prev = segments[^1].SourceSlice;

                if (prev != null &&
                    prev.PadaId == slice.PadaId &&
                    prev.NavamsaZodiacId == slice.NavamsaZodiacId)
                {
                    var previous = segments[^1];

                    segments[^1] = new MonthlyTransitSegment
                    {
                        StartLocal = previous.StartLocal,
                        EndLocal = visibleEnd,
                        Text = previous.Text,
                        Color = previous.Color,
                        ColorTop = previous.ColorTop,
                        ColorBottom = previous.ColorBottom,
                        IsSplitColor = previous.IsSplitColor,
                        SourceSlice = previous.SourceSlice
                    };

                    continue;
                }
            }

            var color = GetPadaColor(slice);

            segments.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildPadaText(planet, slice),
                Color = color,
                ColorTop = null,
                ColorBottom = null,
                IsSplitColor = false,
                SourceSlice = slice
            });
        }

        return new MonthlyTransitLane
        {
            Kind = MonthlyTransitLaneKind.Pada,
            Title = "Pada",
            Segments = segments
        };
    }

    private static string BuildPadaText(EPlanet planet, PlanetSlice slice)
    {
        var padaNumber = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);
        var navamsaZodiacId = slice.NavamsaZodiacId;

        var marker = BuildNavamsaExaltationMarker(planet, navamsaZodiacId);

        return $"{padaNumber}-{navamsaZodiacId}{marker}";
    }

    private static string BuildNavamsaExaltationMarker(EPlanet planet, int navamsaZodiacId)
    {
        if (planet == EPlanet.RAHU || planet == EPlanet.KETU)
            return string.Empty;

        if (navamsaZodiacId < 1 || navamsaZodiacId > 12)
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

    private static Color GetPadaColor(PlanetSlice slice)
    {
        var padaNumber = SwissUtility.GetPadaNumberByPadaId(slice.PadaId);

        var pada = DataCache.Instance.PadaList
            .FirstOrDefault(x =>
                x.NakshatraId == slice.NakshatraId &&
                x.PadaNumber == padaNumber);

        if (pada == null || pada.ColorId == 0)
            return Colors.White;

        return DataCache.Instance.GetColor((EColor)pada.ColorId);
    }

    private static MonthlyTransitLane BuildTaraBalaLane(
        EPlanet planet,
        IReadOnlyList<PlanetSlice> slices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var segments = new List<MonthlyTransitSegment>();

        foreach (var slice in slices.OrderBy(s => s.StartUtc))
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(slice.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            if (segments.Count > 0)
            {
                var prev = segments[^1].SourceSlice;

                if (prev != null &&
                    prev.TaraBalaId == slice.TaraBalaId &&
                    prev.TaraBalaPercent == slice.TaraBalaPercent)
                {
                    var previous = segments[^1];

                    segments[^1] = new MonthlyTransitSegment
                    {
                        StartLocal = previous.StartLocal,
                        EndLocal = visibleEnd,
                        Text = previous.Text,
                        Color = previous.Color,
                        ColorTop = previous.ColorTop,
                        ColorBottom = previous.ColorBottom,
                        IsSplitColor = previous.IsSplitColor,
                        SourceSlice = previous.SourceSlice
                    };

                    continue;
                }
            }

            var color = DataCache.Instance.GetColor((EColor)TaraBalaSlice.GetTaraBalaColorId(slice.TaraBalaId, slice.TaraBalaPercent));

            segments.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildTaraBalaText(planet, slice),
                Color = color,
                ColorTop = null,
                ColorBottom = null,
                IsSplitColor = false,
                SourceSlice = slice
            });
        }

        return new MonthlyTransitLane
        {
            Kind = MonthlyTransitLaneKind.TaraBala,
            Title = "Tara Bala",
            Segments = segments
        };
    }

    private static string BuildTaraBalaText(EPlanet planet, PlanetSlice slice)
    {
        if (planet == EPlanet.MOON)
            return slice.TaraBalaId.ToString();

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



}