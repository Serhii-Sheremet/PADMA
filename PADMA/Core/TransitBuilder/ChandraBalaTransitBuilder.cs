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
            var result = new List<ChandraBalaSlice>();

            if (mList == null || mList.Count == 0)
                return result;

            // Prepare swapped zodiac list: birth zodiac becomes index 0
            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthZodiacMoonId);

            // initialize with the first entry
            int currentZodiacId = mList[0].ZodiacId;
            DateTime currentStartUtc = mList[0].DateTimeUtc;

            for (int i = 1; i < mList.Count; i++)
            {
                // zodiac changed - close the previous segment
                if (mList[i].ZodiacId != currentZodiacId)
                {
                    (houseNumber, colorId) = GetChandraBalaColorAndHouse(currentZodiacId, swappedZodiacs);
                    result.Add(new ChandraBalaSlice
                    {
                        StartUtc = currentStartUtc,
                        EndUtc = mList[i].DateTimeUtc, // up to this point in time
                        ZodiacCode = (EZodiac)currentZodiacId,
                        HouseNumber = houseNumber,
                        ColorId = colorId
                    });

                    // start tracking the new segment
                    currentZodiacId = mList[i].ZodiacId;
                    currentStartUtc = mList[i].DateTimeUtc;
                }
            }

            // append the last open segment, running to the end of the period
            var last = mList[mList.Count - 1];
            (houseNumber, colorId) = GetChandraBalaColorAndHouse(currentZodiacId, swappedZodiacs);
            var lastSlice = new ChandraBalaSlice
            {
                StartUtc = currentStartUtc,
                EndUtc = last.DateTimeUtc,
                ZodiacCode = (EZodiac)last.ZodiacId,
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
            if (houseNumber == 6 || houseNumber == 8 || houseNumber == 12 || zodiacCode == EZodiac.SCO)
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
