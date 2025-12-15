using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Enums;
using PADMA.Core.Utilities;

namespace PADMA.Core.TransitBuilder
{
    public static class PlanetTransitBuilder
    {
        public static List<PlanetSlice> BuildPlanetSlices(
            List<PlanetData> list, 
            int birthNakshatraMoonId,
            int birthZodiacMoonId,
            int birthLagnaId,
            EAppSetting nodeType)
        {
            var result = new List<PlanetSlice>();

            if (list.Count == 0)
                return result;

            var swappedZodiacLagna = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthLagnaId);
            var swappedZodiacMoon = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthZodiacMoonId);

            var swappedNakshatras = TransitBuilderUtility.SwapNakshatras(DataCache.Instance.NakshatraList.ToList(), birthNakshatraMoonId);
            var taraMatrix = TransitBuilderUtility.MakeTaraBalaMatrix(swappedNakshatras);

            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];

                var slice = new PlanetSlice
                {
                    Kind = ETransitKind.Planet,
                    PlanetId = d.PlanetId,
                    ZodiacId = d.ZodiacId,
                    NakshatraId = d.NakshatraId,
                    PadaId = d.PadaId,
                    IsRetrograde = d.IsRetrograde,
                    NodeType = nodeType,

                    StartUtc = d.DateTimeUtc,
                    EndUtc = (i < list.Count - 1) ? list[i + 1].DateTimeUtc : d.DateTimeUtc
                };
                
                (var tbId, var tbPct) = TransitBuilderUtility.ComputeTaraBalaFromMatrix(d.NakshatraId, taraMatrix);
                slice.TaraBalaId = tbId;
                slice.TaraBalaPercent = tbPct;

                int padaNumber = SwissUtility.GetPadaNumberByPadaId(d.PadaId);
                slice.NavamsaZodiacId = SwissUtility.GetNavamsaByNakshatraAndPada(d.NakshatraId, padaNumber);

                slice.HouseFromMoon = CalculateHouse(swappedZodiacMoon, birthZodiacMoonId);
                slice.MoonColorCode = (EColor)GetPlanetColorCode((EPlanet)slice.PlanetId, slice.HouseFromMoon);  

                slice.HouseFromLagna = CalculateHouse(swappedZodiacMoon, birthLagnaId);
                slice.LagnaColorCode = (EColor)GetPlanetColorCode((EPlanet)slice.PlanetId, slice.HouseFromLagna);

                result.Add(slice);
            }

            return result;
        }

        private static int CalculateHouse(List<Zodiac> zList, int zodiacId) 
        {
            return (zList.FindIndex(i => i.Id == zodiacId) + 1);
        }

        private static int GetPlanetColorCode(EPlanet pCode, int pHouse)
        {
            if (pCode == EPlanet.RAHUTRUE)
                pCode = EPlanet.RAHUMEAN;
            if (pCode == EPlanet.KETUTRUE)
                pCode = EPlanet.KETUMEAN;
            return DataCache.Instance.TransitList.Where(i => i.PlanetId == (int)pCode && i.House == pHouse).FirstOrDefault()?.ColorId ?? 0;
        }


    }
}
