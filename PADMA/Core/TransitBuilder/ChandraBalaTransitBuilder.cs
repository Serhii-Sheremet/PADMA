using System.Collections.Generic;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.Enums;

namespace PADMA.Core.TransitBuilder
{
    public static class ChandraBalaTransitBuilder
    {
        public static List<ChandraBalaSlice> BuildChandraBalaSlices(
            List<PlanetSlice> moonSlices,
            int birthZodiacMoonId)
        {
            var result = new List<ChandraBalaSlice>();

            if (moonSlices == null || moonSlices.Count == 0)
                return result;

            // Prepare swapped zodiac list: birth zodiac becomes index 0
            var swappedZodiacs = TransitBuilderUtility.SwapZodiacs(DataCache.Instance.ZodiacList.ToList(), birthZodiacMoonId);

            foreach (var m in moonSlices)
            {
                var zodiacCode = (EZodiac)m.ZodiacId;

                // Determine house number (1..12)
                var index = swappedZodiacs.FindIndex(z => z.Id == m.ZodiacId);
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

                result.Add(new ChandraBalaSlice
                {
                    StartUtc = m.StartUtc,
                    EndUtc = m.EndUtc,
                    ZodiacCode = zodiacCode,
                    HouseNumber = houseNumber,
                    ColorId = colorId
                });
            }

            return result;
        }
    }
}
