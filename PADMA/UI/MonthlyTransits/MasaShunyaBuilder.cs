using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;

namespace PADMA.UI.MonthlyTransits;

public static class MasaShunyaBuilder
{
    public static MonthlyMasaShunyaBand Build(
        List<PlanetData> moonData,
        List<TithiData> tithiData,
        DateTime bufferStartUtc,
        DateTime bufferEndUtc,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var moonSlices = PlanetTransitBuilder.BuildPlanetSlices(
            moonData,
            DataCache.Instance.ProfileContextService.Current.BirthNakshatraMoonId,
            DataCache.Instance.ProfileContextService.Current.BirthZodiacMoonId,
            DataCache.Instance.ProfileContextService.Current.BirthLagnaId,
            DataCache.Instance.GetActiveNodeSetting());

        var tithiSlices = TithiTransitBuilder.BuildTithiSlices(tithiData, bufferEndUtc);
        var nakshatraSlices = NakshatraTransitBuilder.BuildNakshatraSlices(moonData);

        var masaPeriods = BuildMasaPeriods(
            moonSlices,
            tithiData,
            bufferStartUtc,
            bufferEndUtc);

        var masaSegments = BuildMasaSegments(
            masaPeriods,
            moonSlices,
            monthStartLocal,
            monthEndLocal,
            tzInfo);

        var shunyaNakshatra = BuildShunyaNakshatraOverlays(
            masaPeriods,
            nakshatraSlices,
            monthStartLocal,
            monthEndLocal,
            tzInfo);

        var shunyaTithi = BuildShunyaTithiOverlays(
            masaPeriods,
            tithiSlices,
            monthStartLocal,
            monthEndLocal,
            tzInfo);

        return new MonthlyMasaShunyaBand
        {
            MasaSegments = masaSegments,
            ShunyaNakshatraOverlays = shunyaNakshatra,
            ShunyaTithiOverlays = shunyaTithi
        };
    }

    private sealed class MasaPeriod
    {
        public DateTime StartUtc { get; init; }
        public DateTime EndUtc { get; init; }
        public int MasaId { get; init; }
        public DateTime? FullMoonUtc { get; init; }
    }

    private static List<MasaPeriod> BuildMasaPeriods(
        IReadOnlyList<PlanetSlice> moonSlices,
        IReadOnlyList<TithiData> tithiData,
        DateTime bufferStartUtc,
        DateTime bufferEndUtc)
    {
        var result = new List<MasaPeriod>();

        var points = tithiData
            .OrderBy(x => x.DateTimeUtc)
            .ToList();

        if (points.Count == 0)
            return result;

        var firstTithiOne = points.FirstOrDefault(x => x.TithiId == 1);

        DateTime currentStart = bufferStartUtc;
        DateTime? currentFullMoonUtc = null;

        int currentMasaId;

        if (firstTithiOne != null)
        {
            currentMasaId = GetPreviousMasaId(
                GetMasaIdByMoonZodiac(moonSlices, firstTithiOne.DateTimeUtc));
        }
        else
        {
            currentMasaId = GetMasaIdByMoonZodiac(moonSlices, bufferStartUtc);
        }

        foreach (var point in points)
        {
            var utc = point.DateTimeUtc;

            if (utc < bufferStartUtc || utc > bufferEndUtc)
                continue;

            if (point.TithiId == 16)
                currentFullMoonUtc = utc;

            if (point.TithiId != 1)
                continue;

            if (utc > currentStart && currentMasaId != 0)
            {
                result.Add(new MasaPeriod
                {
                    StartUtc = currentStart,
                    EndUtc = utc,
                    MasaId = currentMasaId,
                    FullMoonUtc = currentFullMoonUtc
                });
            }

            currentStart = utc;
            currentMasaId = GetMasaIdByMoonZodiac(moonSlices, utc);
            currentFullMoonUtc = null;
        }

        if (currentStart < bufferEndUtc && currentMasaId != 0)
        {
            result.Add(new MasaPeriod
            {
                StartUtc = currentStart,
                EndUtc = bufferEndUtc,
                MasaId = currentMasaId,
                FullMoonUtc = currentFullMoonUtc
            });
        }

        return result;
    }


    private static int GetMasaIdByMoonZodiac(
        IReadOnlyList<PlanetSlice> moonSlices,
        DateTime utc)
    {
        var slice = moonSlices
            .FirstOrDefault(x => x.StartUtc <= utc && x.EndUtc > utc)
            ?? moonSlices.LastOrDefault(x => x.StartUtc <= utc)
            ?? moonSlices.FirstOrDefault();

        if (slice == null)
            return 0;

        return DataCache.Instance.MasaList
            .FirstOrDefault(x => x.ZodiacId == slice.ZodiacId)
            ?.Id ?? 0;
    }

    private static int GetPreviousMasaId(int masaId)
    {
        if (masaId <= 0)
            return 0;

        return masaId == 1 ? 12 : masaId - 1;
    }

    private static IReadOnlyList<MonthlyTransitSegment> BuildMasaSegments(
        IReadOnlyList<MasaPeriod> masaPeriods,
        IReadOnlyList<PlanetSlice> moonSlices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var result = new List<MonthlyTransitSegment>();

        foreach (var masa in masaPeriods)
        {
            var startLocal = TimeZoneInfo.ConvertTimeFromUtc(masa.StartUtc, tzInfo);
            var endLocal = TimeZoneInfo.ConvertTimeFromUtc(masa.EndUtc, tzInfo);

            if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
                continue;

            var visibleStart = Max(startLocal, monthStartLocal);
            var visibleEnd = Min(endLocal, monthEndLocal);

            if (visibleEnd <= visibleStart)
                continue;

            result.Add(new MonthlyTransitSegment
            {
                StartLocal = visibleStart,
                EndLocal = visibleEnd,
                Text = BuildMasaText(masa.MasaId, masa.FullMoonUtc, moonSlices),
                Color = DataCache.Instance.GetColor(EColor.LIGHTGREEN),
                IsSplitColor = false
            });
        }

        return result;
    }

    private static IReadOnlyList<MonthlyTransitOverlaySegment> BuildShunyaNakshatraOverlays(
        IReadOnlyList<MasaPeriod> masaPeriods,
        IReadOnlyList<NakshatraSlice> nakshatraSlices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var result = new List<MonthlyTransitOverlaySegment>();
        var color = DataCache.Instance.GetColor(EColor.SHUNYANAKSHATRA);

        foreach (var masa in masaPeriods)
        {
            var masaDef = DataCache.Instance.MasaList.FirstOrDefault(x => x.Id == masa.MasaId);
            if (masaDef == null || masaDef.ShunyaNakshatraIdArray.Length == 0)
                continue;

            foreach (var nak in nakshatraSlices)
            {
                if (!masaDef.ShunyaNakshatraIdArray.Contains(nak.NakshatraId))
                    continue;

                AddIntersectionOverlay(
                    result,
                    masa.StartUtc,
                    masa.EndUtc,
                    nak.StartUtc,
                    nak.EndUtc,
                    monthStartLocal,
                    monthEndLocal,
                    tzInfo,
                    color);
            }
        }

        return result;
    }

    private static IReadOnlyList<MonthlyTransitOverlaySegment> BuildShunyaTithiOverlays(
        IReadOnlyList<MasaPeriod> masaPeriods,
        IReadOnlyList<TithiSlice> tithiSlices,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo)
    {
        var result = new List<MonthlyTransitOverlaySegment>();
        var color = DataCache.Instance.GetColor(EColor.SHUNIATITHI);

        foreach (var masa in masaPeriods)
        {
            var masaDef = DataCache.Instance.MasaList.FirstOrDefault(x => x.Id == masa.MasaId);
            if (masaDef == null || masaDef.ShunyaTithiIdArray.Length == 0)
                continue;

            foreach (var tithi in tithiSlices)
            {
                if (!masaDef.ShunyaTithiIdArray.Contains(tithi.TithiId))
                    continue;

                AddIntersectionOverlay(
                    result,
                    masa.StartUtc,
                    masa.EndUtc,
                    tithi.StartUtc,
                    tithi.EndUtc,
                    monthStartLocal,
                    monthEndLocal,
                    tzInfo,
                    color);
            }
        }

        return result;
    }

    private static void AddIntersectionOverlay(
        List<MonthlyTransitOverlaySegment> result,
        DateTime aStartUtc,
        DateTime aEndUtc,
        DateTime bStartUtc,
        DateTime bEndUtc,
        DateTime monthStartLocal,
        DateTime monthEndLocal,
        TimeZoneInfo tzInfo,
        Color color)
    {
        var startUtc = Max(aStartUtc, bStartUtc);
        var endUtc = Min(aEndUtc, bEndUtc);

        if (endUtc <= startUtc)
            return;

        var startLocal = TimeZoneInfo.ConvertTimeFromUtc(startUtc, tzInfo);
        var endLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, tzInfo);

        if (endLocal <= monthStartLocal || startLocal >= monthEndLocal)
            return;

        var visibleStart = Max(startLocal, monthStartLocal);
        var visibleEnd = Min(endLocal, monthEndLocal);

        if (visibleEnd <= visibleStart)
            return;

        result.Add(new MonthlyTransitOverlaySegment
        {
            StartLocal = visibleStart,
            EndLocal = visibleEnd,
            Color = color
        });
    }

    private static string BuildMasaText(
    int masaId,
    DateTime? fullMoonUtc,
    IReadOnlyList<PlanetSlice> moonSlices)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        var masaName = DataCache.Instance.MasaDescList
            .FirstOrDefault(x =>
                x.MasaId == masaId &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase))
            ?.Name
            ?? masaId.ToString();

        if (!fullMoonUtc.HasValue)
            return masaName;

        var fullMoonNakshatraId = GetMoonNakshatraIdAt(
            moonSlices,
            fullMoonUtc.Value);

        if (fullMoonNakshatraId == 0)
            return masaName;

        var ruler = GetNakshatraRulerName(fullMoonNakshatraId, lang);

        return string.IsNullOrWhiteSpace(ruler)
            ? masaName
            : $"{masaName} ({ruler})";
    }

    private static int GetMoonNakshatraIdAt(
        IReadOnlyList<PlanetSlice> moonSlices,
        DateTime utc)
    {
        var slice = moonSlices
            .FirstOrDefault(x => x.StartUtc <= utc && x.EndUtc > utc)
            ?? moonSlices.LastOrDefault(x => x.StartUtc <= utc)
            ?? moonSlices.FirstOrDefault();

        return slice?.NakshatraId ?? 0;
    }

    private static string GetNakshatraRulerName(int nakshatraId, string lang)
    {
        var fullRuler = DataCache.Instance.NakshatraDescList
            .FirstOrDefault(x =>
                x.NakshatraId == nakshatraId &&
                string.Equals(x.LanguageCode, lang, StringComparison.OrdinalIgnoreCase))
            ?.Ruler ?? string.Empty;

        if (string.IsNullOrWhiteSpace(fullRuler))
            return string.Empty;

        return fullRuler
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault()
            ?? string.Empty;
    }

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;


}