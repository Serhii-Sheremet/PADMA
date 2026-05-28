using PADMA.Core.Enums;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;

namespace PADMA.UI.MonthlyTransits;

public sealed class TransitColorInfo
{
    public bool IsSplitColor { get; init; }

    public Color? Color { get; init; }
    public Color? ColorTop { get; init; }
    public Color? ColorBottom { get; init; }
}

public static class TransitColorResolver
{
    public static TransitColorInfo Resolve(PlanetSlice slice, EAppSetting transitMode)
    {
        var moonColor = DataCache.Instance.GetColor(slice.MoonColorCode);
        var lagnaColor = DataCache.Instance.GetColor(slice.LagnaColorCode);

        return transitMode switch
        {
            EAppSetting.TRANZITMOON => new TransitColorInfo
            {
                IsSplitColor = false,
                Color = moonColor,
                ColorTop = null,
                ColorBottom = null
            },

            EAppSetting.TRANZITLAGNA => new TransitColorInfo
            {
                IsSplitColor = false,
                Color = lagnaColor,
                ColorTop = null,
                ColorBottom = null
            },

            EAppSetting.TRANZITMOONANDLAGNA => new TransitColorInfo
            {
                IsSplitColor = true,
                Color = null,
                ColorTop = moonColor,
                ColorBottom = lagnaColor
            },

            _ => new TransitColorInfo
            {
                IsSplitColor = false,
                Color = moonColor,
                ColorTop = null,
                ColorBottom = null
            }
        };
    }
}