using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Core.TransitBuilder
{
    public static class ChandraBalaTransitBuilder
    {
        public static List<ChandraBalaSlice> BuildChandraBalaSlices(List<PlanetData> mList, int birthZodiacMoonId)
        {
            int houseNumber = 0, colorId = 0;
            EZodiac zodiacCode;
            var result = new List<ChandraBalaSlice>();

            if (mList == null || mList.Count == 0)
                return result;

            // Prepare swapped zodiac list: birth zodiac becomes index 0
            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthZodiacMoonId);

            // стартуем с первой точки
            int currentZodiacId = mList[0].ZodiacId;
            DateTime currentStartUtc = mList[0].DateTimeUtc;

            for (int i = 1; i < mList.Count; i++)
            {
                var item = mList[i];
                var zid = item.ZodiacId;

                // zodiac поменялcя - закрываем предыдущий интервал
                if (zid != currentZodiacId)
                {
                    zodiacCode = (EZodiac)item.ZodiacId;
                    (houseNumber, colorId) = GetChandraBalaColorAndHouse(currentZodiacId, swappedZodiacs);
                    result.Add(new ChandraBalaSlice
                    {
                        StartUtc = currentStartUtc,
                        EndUtc = item.DateTimeUtc, // до момента смены
                        ZodiacCode = zodiacCode,
                        HouseNumber = houseNumber,
                        ColorId = colorId
                    });

                    // начинаем новый интервал
                    currentZodiacId = zid;
                    currentStartUtc = item.DateTimeUtc;
                }
            }

            // закрываем последнюю чандру балу до последней точки ряда
            var last = mList[mList.Count - 1];
            zodiacCode = (EZodiac)last.ZodiacId;
            (houseNumber, colorId) = GetChandraBalaColorAndHouse(currentZodiacId, swappedZodiacs);
            var lastSlice = new ChandraBalaSlice
            {
                StartUtc = currentStartUtc,
                EndUtc = last.DateTimeUtc,
                ZodiacCode = zodiacCode,
                HouseNumber = houseNumber,
                ColorId = colorId
            };
            result.Add(lastSlice);

            return result;
        }

        private static (int houseNumber, int colorId) GetChandraBalaColorAndHouse(int zodiacId, List<Zodiac> swappedZodiacs)
        {
            var zodiacCode = (EZodiac)zodiacId;
            // Determine house number (1..12)
            var index = swappedZodiacs.FindIndex(z => z.Id == zodiacId);
            int houseNumber = index >= 0 ? index + 1 : 0;
            // Determine color
            int colorId;
            if (houseNumber == 6 || houseNumber == 8 || houseNumber == 12 ||
                zodiacCode == EZodiac.SCO)
            {
                colorId = (int)EColor.RED;
            }
            else
            {
                colorId = (int)EColor.GREEN;
            }
            return (houseNumber, colorId);
        }

    }
}
