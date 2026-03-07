using PADMA.Core.Models;
using PADMA.Core.Enums;
using PADMA.Core.Utilities;
using System.Collections.Generic;

namespace PADMA.Core.Services
{
    public static class TransitChartDataService
    {

        public static bool AreAspectsEnabled(EAspectFilter filter)
        {
            return filter != EAspectFilter.NONE;
        }

        public static List<ChartPlanetItem>[] AddAspects(
            List<ChartPlanetItem>[] planetsList,
            EAspectFilter aspectFilter)
        {
            if (!AreAspectsEnabled(aspectFilter))
                return planetsList;

            for (int houseIndex = 0; houseIndex < planetsList.Length; houseIndex++)
            {
                for (int planetIndex = 0; planetIndex < planetsList[houseIndex].Count; planetIndex++)
                {
                    var sourcePlanet = planetsList[houseIndex][planetIndex];

                    if (sourcePlanet.IsActiveAspect)
                        continue;

                    if (sourcePlanet.TransitType == ETransitType.TRANSITBIRTH ||
                        sourcePlanet.TransitType == ETransitType.NATALNAVAMSA ||
                        sourcePlanet.TransitType == ETransitType.TRANSITNAVAMSA)
                        continue;

                    if (aspectFilter != EAspectFilter.ALL &&
                        (EPlanet)aspectFilter != sourcePlanet.PlanetCode)
                        continue;

                    var aspectOffsets = GetAspectDomsListByPlanet(sourcePlanet.PlanetCode);

                    foreach (var offset in aspectOffsets)
                    {
                        int targetHouse = houseIndex + offset - 1;

                        if (targetHouse >= 12)
                            targetHouse -= 12;

                        var aspectPlanet = new ChartPlanetItem
                        {
                            PlanetCode = sourcePlanet.PlanetCode,
                            Longitude = sourcePlanet.Longitude,
                            TransitType = sourcePlanet.TransitType,
                            Retro = string.Empty,
                            Exaltation = string.Empty,
                            IsActiveAspect = true,
                            ColorCode = EColor.GRAY
                        };

                        planetsList[targetHouse].Add(aspectPlanet);
                    }
                }
            }

            return planetsList;
        }

        private static List<int> GetAspectDomsListByPlanet(EPlanet planet)
        {
            return planet switch
            {
                EPlanet.SUN => new() { 7 },
                EPlanet.MOON => new() { 7 },
                EPlanet.MERCURY => new() { 7 },
                EPlanet.VENUS => new() { 7 },
                EPlanet.MARS => new() { 4, 7, 8 },
                EPlanet.JUPITER => new() { 5, 7, 9 },
                EPlanet.SATURN => new() { 3, 7, 10 },
                EPlanet.RAHU => new() { 5, 7, 9 },
                _ => new()
            };
        }

        public static List<ChartPlanetItem> GetGeneralPlanetsListByZodiac(
            List<PlanetData> pdList,
            int zodiacId,
            int house)
        {
            var planetsList = new List<ChartPlanetItem>();

            for (int i = 0; i < pdList.Count; i++)
            {
                var chartPlanet = GetPlanetIfCurrentZodiac(
                    pdList[i],
                    ETransitType.TRANSITGENERAL,
                    zodiacId,
                    house);

                if (chartPlanet != null)
                {
                    planetsList.Add(chartPlanet);
                }
            }

            return planetsList;
        }

        private static ChartPlanetItem? GetPlanetIfCurrentZodiac(
            PlanetData pd,
            ETransitType transitType,
            int zodiacId,
            int house)
        {
            if (pd.ZodiacId != zodiacId)
                return null;

            var planetCode = (EPlanet)pd.PlanetId;

            var exaltation = string.Empty;
            var exaltState = ExaltationUtility.GetPlanetExaltation(planetCode, (EZodiac)zodiacId);

            if (exaltState == EExaltation.EXALTATION)
            {
                exaltation = "↑";
            }
            else if (exaltState == EExaltation.DEBILITATION)
            {
                exaltation = "↓";
            }

            EColor colorCode = EColor.BLACK;

            if (transitType != ETransitType.TRANSITGENERAL &&
                transitType != ETransitType.TRANSITBIRTH &&
                transitType != ETransitType.NATALNAVAMSA &&
                transitType != ETransitType.TRANSITNAVAMSA)
            {
                colorCode = (EColor)(
                    DataCache.Instance.TransitList
                        .FirstOrDefault(t => t.PlanetId == pd.PlanetId && t.House == house)
                        ?.ColorId ?? (int)EColor.BLACK);
            }

            string retro =
                    pd.IsRetrograde &&
                    planetCode != EPlanet.RAHU &&
                    planetCode != EPlanet.KETU
                        ? "R"
                        : string.Empty;

            return new ChartPlanetItem
            {
                PlanetCode = planetCode,
                Longitude = pd.Longitude,
                TransitType = transitType,
                Retro = retro,
                Exaltation = exaltation,
                IsActiveAspect = false,
                ColorCode = colorCode
            };
        }

        public static List<ChartHouseData> BuildCurrentTransitChartHouses(
            List<PlanetData> pdList,
            List<Zodiac> swappedZodiacs)
        {
            var houses = new List<ChartHouseData>();

            for (int i = 0; i < 12; i++)
            {
                var zodiac = swappedZodiacs[i];

                var planets = GetGeneralPlanetsListByZodiac(
                    pdList,
                    zodiac.Id,
                    i + 1);

                houses.Add(new ChartHouseData
                {
                    HouseNumber = i + 1,
                    ZodiacNumber = zodiac.Id,
                    Planets = planets
                });
            }

            return houses;
        }


    }
}