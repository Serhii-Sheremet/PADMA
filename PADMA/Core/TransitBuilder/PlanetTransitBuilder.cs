using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Enums;
using PADMA.Core.Utilities;

namespace PADMA.Core.Services.TransitBuilder
{
    public static class PlanetTransitBuilder
    {
        public static List<PlanetSlice> BuildPlanetSlices(
            List<PlanetData> list, 
            int birthMoonNakshatraId,
            int birthZodiacMoonId,
            int birthLagnaId,
            ENodeType nodeType)
        {
            var result = new List<PlanetSlice>();

            if (list.Count == 0)
                return result;

            var swappedZodiacLagna = SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthLagnaId);
            var swappedZodiacMoon = SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthZodiacMoonId);

            var swappedNakshatras = SwapNakshatras(DataCache.Instance.NakshatraList.ToList(), birthMoonNakshatraId);
            var taraMatrix = MakeTaraBalaMatrix(swappedNakshatras);

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
                
                (var tbId, var tbPct) = ComputeTaraBalaFromMatrix(d.NakshatraId, taraMatrix);
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

        public static List<Nakshatra> SwapNakshatras(List<Nakshatra> nList, int birthNakshatraId)
        {
            return nList
                .Where(n => n.Id >= birthNakshatraId)
                .Concat(nList.Where(n => n.Id < birthNakshatraId))
                .ToList();
        }

        public static List<Zodiac> SwapZodiacs(List<Zodiac> zList, int id)
        {
            return zList
                .Where(z => z.Id >= id)
                .Concat(zList.Where(z => z.Id < id))
                .ToList();
        }

        public static int[,] MakeTaraBalaMatrix(List<Nakshatra> swapped)
        {
            int[,] arr = new int[9, 3];
            int index = 0;

            for (int col = 0; col < 3; col++)
                for (int row = 0; row < 9; row++)
                    arr[row, col] = swapped[index++].Id;

            return arr;
        }

        /// <summary>
        /// ¬озвращает (TaraBalaId 1..9, Percent 100/50/25) из уже предсобранной матрицы.
        /// </summary>
        private static (int taraBalaId, int percent) ComputeTaraBalaFromMatrix(int nakshatraId, int[,] matrix)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (matrix[row, col] == nakshatraId)
                    {
                        int id = row + 1;
                        int pct = (col == 0) ? 100 : (col == 1 ? 50 : 25);
                        return (id, pct);
                    }
                }
            }
            return (0, 0);
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
            return DataCache.Instance.TransitList.Where(i => i.PlanetId == (int)pCode && i.Dom == pHouse).FirstOrDefault()?.ColorId ?? 0;
        }


    }
}
