using PADMA.Core.Analysis;       
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Utilities;
using System.Globalization;

namespace PADMA.Core.Services;

public interface IProfileContextService
{
    ProfileTransitContext? Current { get; }

    /// <summary>Recompute context for currently active profile (or passed profile id).</summary>
    Task<ProfileTransitContext> RebuildAsync(CancellationToken ct = default);
}

public sealed class ProfileContextService : IProfileContextService
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    public ProfileTransitContext? Current { get; private set; }

    public async Task<ProfileTransitContext> RebuildAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var profile = DataCache.Instance.ActiveProfile
                ?? throw new InvalidOperationException("ActiveProfile is null.");

            var birthLoc = DataCache.Instance.LocationList.Where(loc => loc.Id == profile.PlaceOfBirthId).FirstOrDefault()
                ?? throw new InvalidOperationException("Birth location not found.");

            var livingLoc = DataCache.Instance.LocationList.Where(loc => loc.Id == profile.PlaceOfLivingId).FirstOrDefault()
                ?? throw new InvalidOperationException("Living location not found.");

            // Active Node settings
            var nodeSetting = DataCache.Instance.AppSettingsList
                .FirstOrDefault(s => s.GroupCode == "NODE" && s.Active == 1);
            var nodeMode = (EAppSetting)(nodeSetting?.Id ?? (int)EAppSetting.NODEMEAN);

            // Data by birth location
            int birthZodiacMoonId = 1, birthNakshatraMoonId = 1, birthPadaMoonId = 1, lagnaId = 1;
            double birthLat = 0, birthLon = 0, acendent = 0;
            double livingLat = 0, livingLon = 0;
            string LivingTzId = string.Empty;
            DateTime localBirthDate = profile.DateOfBirth;
            TimeZoneInfo LivingTzInfo = TimeZoneInfo.Utc;
            List<PlanetData> bdPlanetData = new List<PlanetData>();
            
            if (birthLoc != null  &&
                double.TryParse(birthLoc.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out birthLat) &&
                double.TryParse(birthLoc.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out birthLon))
            {
                // Birth reference calculations
                double offset = TimeZoneService.GetUtcOffsetHours(profile.DateOfBirth, birthLat, birthLon);
                localBirthDate = profile.DateOfBirth.AddHours(-offset);

                char hsys = 'O'; // Placidus
                bdPlanetData = SwissAnalysis.CalculatePlanetPositionsForDate(localBirthDate, birthLat, birthLon, nodeMode);
                acendent = SwissService.CalculateAscendantForDate(localBirthDate, birthLat, birthLon, 0, hsys);
            }

            // Timezone by living location
            if (livingLoc != null &&
                double.TryParse(livingLoc.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out livingLat) &&
                double.TryParse(livingLoc.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out livingLon))
            {
                LivingTzId = TimeZoneService.GetDotNetTimeZoneId(livingLat, livingLon);
                LivingTzInfo = TimeZoneInfo.FindSystemTimeZoneById(LivingTzId);
            }

            if (bdPlanetData.Count > 0)
            {
                var moon = bdPlanetData.FirstOrDefault(p => p.PlanetId == (int)EPlanet.MOON);

                birthZodiacMoonId = moon?.ZodiacId ?? 1;
                birthNakshatraMoonId = moon?.NakshatraId ?? 1;
                birthPadaMoonId = moon?.PadaId ?? 1;
                lagnaId = SwissUtility.GetZodiacIdFromDegree(acendent);
            }

            EAppSetting nodeType = (EAppSetting)(DataCache.Instance.AppSettingsList.FirstOrDefault(i => i.GroupCode.Equals("NODE") && i.Active == 1)?.Id ?? 18);

            var ctx = new ProfileTransitContext(
                ProfileId: profile.Id,
                BirthDateUtc: localBirthDate,
                BirthLat: birthLat,
                BirthLon: birthLon,
                LivingLat: livingLat,
                LivingLon: livingLon,
                DotNetTimeZoneId: LivingTzId,
                TimeZoneInfo: LivingTzInfo,
                BirthZodiacMoonId: birthZodiacMoonId,
                BirthNakshatraMoonId: birthNakshatraMoonId,
                BirthPadaMoonId: birthPadaMoonId,
                BirthLagnaId: lagnaId,
                BirthPlanetDataList: bdPlanetData,
                nodeType
            );

            Current = ctx;
            return ctx;
        }
        finally
        {
            _lock.Release();
        }
    }
}
