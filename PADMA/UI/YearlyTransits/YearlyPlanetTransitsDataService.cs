using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;
using PADMA.UI.MonthlyTransits;

namespace PADMA.UI.YearlyTransits;

/// <summary>
/// Builds one continuous, profile-dependent visual transit data set for a local calendar year.
/// It reuses the verified Monthly Transits segment builders so that colors, labels,
/// overlays, clipping, and transit interpretation rules remain identical.
/// </summary>
public sealed class YearlyPlanetTransitsDataService
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

    public async Task<YearlyPlanetTransitsData> BuildAsync(
        int year,
        CancellationToken ct = default)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        // A full selected year needs the exclusive 1 January boundary of the next year.
        if (year < DateTime.MinValue.Year || year >= DateTime.MaxValue.Year)
        {
            throw new ArgumentOutOfRangeException(
                nameof(year),
                "A yearly transit view requires a complete local calendar year.");
        }

        var ctx = DataCache.Instance.ProfileContextService.Current
            ?? await DataCache.Instance.ProfileContextService.RebuildAsync(ct);

        if (ctx == null)
        {
            throw new InvalidOperationException(
                "Yearly transits cannot be calculated without an active profile.");
        }

        var tzInfo = ctx.TimeZoneInfo;

        var yearStartLocal = new DateTime(
            year, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        var yearEndLocal = yearStartLocal.AddYears(1);

        // Masa/Shunya needs a wider lunar-context buffer near the visible boundaries.
        var masaBufferStartLocal = yearStartLocal.AddDays(-45);
        var masaBufferEndLocal = yearEndLocal.AddDays(45);

        var masaBufferStartUtc = TimeZoneInfo.ConvertTimeToUtc(
            masaBufferStartLocal, tzInfo);

        var masaBufferEndUtc = TimeZoneInfo.ConvertTimeToUtc(
            masaBufferEndLocal, tzInfo);

        // Planet slices need outer context so a visible boundary can be clipped correctly.
        var bufferStartLocal = yearStartLocal.AddDays(-7);
        var bufferEndLocal = yearEndLocal.AddDays(7);

        var bufferStartUtc = TimeZoneInfo.ConvertTimeToUtc(
            bufferStartLocal, tzInfo);

        var bufferEndUtc = TimeZoneInfo.ConvertTimeToUtc(
            bufferEndLocal, tzInfo);

        var nodeMode = DataCache.Instance.GetActiveNodeSetting();
        var transitMode = DataCache.Instance.GetActiveTransitSettings();

        var moonDataForMasa = SwissAnalysis.CalculatePlanetDataList_London(
            (int)EPlanet.MOON,
            masaBufferStartUtc,
            masaBufferEndUtc,
            nodeMode,
            includeOuterBoundaries: true);

        var tithiDataForMasa = SwissAnalysis.CalculateTithiDataList_London(
            masaBufferStartUtc,
            masaBufferEndUtc,
            nodeMode);

        // Despite historical parameter names, MasaShunyaBuilder accepts any visible local range.
        var masaShunya = MasaShunyaBuilder.Build(
            moonDataForMasa,
            tithiDataForMasa,
            masaBufferStartUtc,
            masaBufferEndUtc,
            yearStartLocal,
            yearEndLocal,
            tzInfo);

        var groups = new List<MonthlyPlanetGroup>();
        var transitPack = new Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>();

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
                    includeOuterBoundaries: true);

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
                    includeOuterBoundaries: true);

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

            transitPack[planet] = clippedSlices;

            // Reuse verified Monthly builders. They clip to the supplied visible local range,
            // so yearStartLocal/yearEndLocal are valid here.
            var mrityuBhagaOverlays =
                MonthlyPlanetTransitsDataService.BuildMrityuBhagaOverlays(
                    planet,
                    bufferStartUtc,
                    bufferEndUtc,
                    yearStartLocal,
                    yearEndLocal,
                    tzInfo,
                    nodeMode);

            groups.Add(new MonthlyPlanetGroup
            {
                Planet = planet,
                PlanetName = MonthlyPlanetTransitsDataService.GetPlanetName(planet),
                Lanes =
                [
                    MonthlyPlanetTransitsDataService.BuildZodiacLane(
                        planet,
                        clippedSlices,
                        mrityuBhagaOverlays,
                        yearStartLocal,
                        yearEndLocal,
                        tzInfo,
                        transitMode),

                    MonthlyPlanetTransitsDataService.BuildNakshatraLane(
                        planet,
                        clippedSlices,
                        yearStartLocal,
                        yearEndLocal,
                        tzInfo,
                        transitMode),

                    MonthlyPlanetTransitsDataService.BuildPadaLane(
                        planet,
                        clippedSlices,
                        yearStartLocal,
                        yearEndLocal,
                        tzInfo),

                    MonthlyPlanetTransitsDataService.BuildTaraBalaLane(
                        planet,
                        clippedSlices,
                        yearStartLocal,
                        yearEndLocal,
                        tzInfo)
                ]
            });
        }

        return new YearlyPlanetTransitsData
        {
            Year = year,
            YearStartLocal = yearStartLocal,
            YearEndLocal = yearEndLocal,
            MasaShunya = masaShunya,
            PlanetGroups = groups,
            TransitPack = transitPack
        };
    }
}
