using PADMA.Core.Enums;
using PADMA.Core.Models;

namespace PADMA.Core.Services;

public sealed record ProfileTransitContext(
    int ProfileId,

    //Date of birth
    DateTime BirthDateUtc,

    // Locations 
    double BirthLat,
    double BirthLon,
    double LivingLat,
    double LivingLon,

    // Timezone for living location (основная для календаря)
    string DotNetTimeZoneId,
    TimeZoneInfo TimeZoneInfo,

    // Birth reference values used by TransitEngine
    int BirthZodiacMoonId,
    int BirthNakshatraMoonId,
    int BirthPadaMoonId,
    int BirthLagnaId,

    // Planets data at birth
    List<PlanetData> BirthPlanetDataList,

    // Node type (True/Mean etc.)
    EAppSetting NodeType
);
