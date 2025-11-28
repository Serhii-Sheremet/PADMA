using System;

namespace PADMA.Core.Utilities
{
    public static class CalendarDrawingHelper
    {
        /// <summary>
        /// Конвертирует время в пиксели по ширине суток.
        /// date – реальное время (в рамках или рядом с dayStart..dayStart+1)
        /// dayStart – начало астрологического "дня" (обычно Date.Date).
        /// width – общая ширина ячейки.
        /// </summary>
        public static double ConvertTimeToPixels(double width, DateTime date, DateTime dayStart)
        {
            // смещение от начала дня в минутах
            var delta = date - dayStart;
            var minutes = delta.TotalMinutes;

            // ограничиваем интервалом [0; 1440]
            if (minutes < 0)
                minutes = 0;
            else if (minutes > 1440)
                minutes = 1440;

            return minutes * width / 1440.0;
        }
    }
}
