using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;
using PADMA.Core.Utilities;
using PADMA.UI.Services;

namespace PADMA.UI.MonthlyTransits;

public sealed class MonthlyDayNavBundleBuilder
{
    private readonly IDayComputationService _dayService;

    public MonthlyDayNavBundleBuilder(IDayComputationService dayService)
    {
        _dayService = dayService;
    }

    public async Task<DayNavBundle> BuildAsync(
        MonthlyPlanetTransitsData monthlyData,
        DateTime selectedDayLocal,
        CancellationToken ct = default)
    {
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        var profile = DataCache.Instance.ActiveProfile;
        var ctx = DataCache.Instance.ProfileContextService.Current
            ?? await DataCache.Instance.ProfileContextService.RebuildAsync(ct);

        if (profile == null || ctx == null)
            throw new InvalidOperationException("Active profile context is not available.");

        var tzInfo = ctx.TimeZoneInfo;
        var lang = DataCache.Instance.CurrentLanguageCode;
        var nodeMode = DataCache.Instance.GetActiveNodeSetting();

        var dayLocal = selectedDayLocal.Date;
        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayLocal, tzInfo);
        var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayLocal.AddDays(1), tzInfo);

        var moonData = SwissAnalysis.CalculatePlanetDataList_London(
            (int)EPlanet.MOON,
            dayStartUtc.AddDays(-2),
            dayEndUtc.AddDays(2),
            nodeMode,
            true);

        var nakshatraSlices = NakshatraTransitBuilder.BuildNakshatraSlices(moonData);
        var taraBalaSlices = TaraBalaTransitBuilder.BuildTaraBalaSlices(
            nakshatraSlices,
            ctx.BirthNakshatraMoonId);

        var tithiData = SwissAnalysis.CalculateTithiDataList_London(
            dayStartUtc.AddDays(-2),
            dayEndUtc.AddDays(2),
            nodeMode);

        var tithiSlices = TithiTransitBuilder.BuildTithiSlices(
            tithiData,
            dayEndUtc.AddDays(2));

        var karanaSlices = KaranaTransitBuilder.BuildKaranaSlices(tithiSlices);

        var nityaYogaData = SwissAnalysis.CalculateNityaYogaDataList_London(
            dayStartUtc.AddDays(-2),
            dayEndUtc.AddDays(2),
            nodeMode);

        var nityaYogaSlices = NityaYogaTransitBuilder.BuildNityaYogaSlices(
            nityaYogaData,
            dayEndUtc.AddDays(2));

        var chandraBalaSlices = ChandraBalaTransitBuilder.BuildChandraBalaSlices(
            moonData,
            ctx.BirthZodiacMoonId);

        var lagnaData = SwissAnalysis.CalculateLagnaDataList(
            dayStartUtc.AddHours(-3),
            dayEndUtc.AddHours(6),
            ctx.LivingLat,
            ctx.LivingLon,
            0,
            'O');

        var lagnaSlices = LagnaTransitBuilder.BuildLagnaSlices(lagnaData)
            .Where(s => s.EndUtc > dayStartUtc && s.StartUtc < dayEndUtc)
            .ToList();

        var nakById = DataCache.Instance.NakshatraDescList
            .Where(x => x.LanguageCode == lang)
            .ToDictionary(x => x.NakshatraId, x => x.Name);

        var taraById = DataCache.Instance.TaraBalaDescList
            .Where(x => x.LanguageCode == lang)
            .ToDictionary(x => x.TaraBalaId, x => x.Name);

        var tithiById = DataCache.Instance.TithiDescList
            .Where(x => x.LanguageCode == lang)
            .ToDictionary(x => x.TithiId, x => x.Name);

        var karanaById = DataCache.Instance.KaranaDescList
            .Where(x => x.LanguageCode == lang)
            .ToDictionary(x => x.KaranaId, x => x.Name);

        var nityaYogaById = DataCache.Instance.NityaYogaDescList
            .Where(x => x.LanguageCode == lang)
            .ToDictionary(x => x.NityaYogaId, x => x.Name);

        var tplHouse = Localization.GetLocalizedText("Moon in {0} house", lang);
        var tplHouseSign = Localization.GetLocalizedText("Moon in {0} house, {1}", lang);

        var eclipse = monthlyData.EclipseDays
            .FirstOrDefault(x => x.DayLocal.Date == dayLocal.Date);

        string? eclipseIcon = null;
        if (eclipse != null)
        {
            eclipseIcon = eclipse.EclipseId switch
            {
                (int)EEclipse.SUNECLIPSE => "solar_eclipse_24.png",
                (int)EEclipse.MOONECLIPSE => "lunar_eclipse_24.png",
                _ => null
            };
        }

        var cache = DataCache.Instance.UserEventsWindowCache;
        if (!cache.IsLoadedFor(ctx.ProfileId, dayLocal, dayLocal.AddDays(1)))
            cache.LoadWindow(ctx.ProfileId, dayLocal, dayLocal.AddDays(1));

        var day = new DayItem
        {
            Date = dayLocal,
            DayNumber = dayLocal.Day,
            IsCurrentMonth = true,
            IsToday = dayLocal.Date == DateTime.Today,
            HasUserEvents = cache.HasEvents(dayLocal),
            NoteMarkerColor = DataCache.Instance.GetColor(EColor.NOTEMARKER),

            EclipseId = eclipse?.EclipseId,
            EclipseDate = eclipse?.EclipseDateUtc,
            EclipseIcon = eclipseIcon,

            TransitPack = monthlyData.TransitPack.ToDictionary(
                kv => kv.Key,
                kv => kv.Value),

            LagnaSlices = lagnaSlices,

            NakshatraSegments = PanchangaHelper.BuildSegmentsForDay(
                nakshatraSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice => $"{slice.NakshatraId}.{nakById.GetValueOrDefault(slice.NakshatraId, "")}",
                slice => ETransitKind.Nakshatra,
                slice => slice.NakshatraId),

            TaraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                taraBalaSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice => $"{taraById.GetValueOrDefault(slice.TaraBalaId, "")} {slice.TaraBalaPercent}%",
                slice => ETransitKind.TaraBala,
                slice => slice.TaraBalaId),

            TithiSegments = PanchangaHelper.BuildSegmentsForDay(
                tithiSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice => $"{slice.TithiId}.{tithiById.GetValueOrDefault(slice.TithiId, "")}",
                slice => ETransitKind.Tithi,
                slice => slice.TithiId),

            KaranaSegments = PanchangaHelper.BuildSegmentsForDay(
                karanaSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice => $"{karanaById.GetValueOrDefault(slice.KaranaId, "")}",
                slice => ETransitKind.Karana,
                slice => slice.KaranaId),

            NityaYogaSegments = PanchangaHelper.BuildSegmentsForDay(
                nityaYogaSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice => $"{slice.NityaYogaId}.{nityaYogaById.GetValueOrDefault(slice.NityaYogaId, "")}",
                slice => ETransitKind.NityaYoga,
                slice => slice.NityaYogaId),

            ChandraBalaSegments = PanchangaHelper.BuildSegmentsForDay(
                chandraBalaSlices,
                dayLocal,
                tzInfo,
                DataCache.Instance,
                slice => (EColor)slice.ColorId,
                slice =>
                {
                    var house = slice.HouseNumber;

                    if (slice.ZodiacCode == EZodiac.SCO)
                    {
                        var zodiacName = GetZodiacName((int)slice.ZodiacCode);
                        return string.Format(tplHouseSign, house, zodiacName);
                    }

                    return string.Format(tplHouse, house);
                },
                slice => ETransitKind.ChandraBala,
                slice => slice.HouseNumber)
        };

        var key = new DayKey(profile.Id, DateOnly.FromDateTime(dayLocal));
        var overview = await _dayService.GetOverviewAsync(key, day, ct);

        return new DayNavBundle
        {
            Day = day,
            Overview = overview,
            Window = null
        };
    }

    private static string GetZodiacName(int zodiacId)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        return DataCache.Instance.ZodiacDescList
            .FirstOrDefault(x => x.LanguageCode == lang && x.ZodiacId == zodiacId)
            ?.Name ?? string.Empty;
    }
}