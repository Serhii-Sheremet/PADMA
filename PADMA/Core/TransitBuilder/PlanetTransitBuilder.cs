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
            ENodeType nodeType,
            double natalMoonLongitude,
            double lagnaLongitude)
        {
            var result = new List<PlanetSlice>();

            if (list.Count == 0)
                return result;

            for (int i = 0; i < list.Count; i++)
            {
                var d = list[i];

                var slice = new PlanetSlice
                {
                    Kind = ETransitKind.Planet,
                    PlanetId = d.PlanetId,
                    ZodiacId = d.ZodiakId,
                    NakshatraId = d.NakshatraId,
                    PadaId = d.PadaId,
                    IsRetrograde = d.IsRetrograde,
                    NodeType = nodeType,

                    StartUtc = d.DateTimeUtc,
                    EndUtc = (i < list.Count - 1) ? list[i + 1].DateTimeUtc : d.DateTimeUtc
                };

                // Derived values:
                slice.TaraBalaId = CalculateTaraBala(d, natalMoonLongitude);
                slice.TaraBalaPercent = CalculateTaraBalaPercent(d, natalMoonLongitude);

                slice.HouseFromMoon = CalculateHouseFrom(d.Longitude, natalMoonLongitude);
                slice.HouseFromLagna = CalculateHouseFrom(d.Longitude, lagnaLongitude);

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

        public static int[,] MakeTaraBalaMatrix(List<Nakshatra> swapped)
        {
            int[,] arr = new int[9, 3];
            int index = 0;

            for (int col = 0; col < 3; col++)
                for (int row = 0; row < 9; row++)
                    arr[row, col] = swapped[index++].Id;

            return arr;
        }

        public static int GetTaraBalaNumber(int[,] matrix, int nakshatraId)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 3; col++)
                    if (matrix[row, col] == nakshatraId)
                        return row + 1;

            return 0;
        }

        public static int GetTaraBalaPercent(int[,] matrix, int nakshatraId)
        {
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 3; col++)
                    if (matrix[row, col] == nakshatraId)
                        return col switch
                        {
                            0 => 100,
                            1 => 50,
                            _ => 25
                        };

            return 0;
        }


        // TODO: insert your old functions here
        private static int CalculateTaraBala(PlanetData d, double natalMoonLongitude) { /* ... */ return 0; }
        private static double CalculateTaraBalaPercent(PlanetData d, double natalMoonLongitude) { /* ... */ return 0; }
        private static int CalculateHouseFrom(double planetLon, double baseLon) { /* ... */ return 0; }
    }

    public static class PlanetSliceExtensions
    {
        public static int GetPadaNumber(this PlanetSlice slice)
            => SwissUtility.GetPadaNumberByPadaId(slice.PadaId);

        public static int GetNavamsaId(this PlanetSlice slice)
            => SwissUtility.GetNavamsaByNakshatraAndPada(slice.NakshatraId, slice.GetPadaNumber());
    }

}
