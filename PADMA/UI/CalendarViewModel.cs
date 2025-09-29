using System;
using System.Collections.ObjectModel;
using PADMA.UI.Models;

namespace PADMA.UI.Models
{
    /// <summary>
    /// ViewModel для календаря (отвечает за список дней).
    /// </summary>
    public class CalendarViewModel
    {
        public ObservableCollection<DayItem> Days { get; set; } = new();

        public int Year { get; private set; }
        public int Month { get; private set; }

        public DayItem? SelectedDay { get; set; }

        public CalendarViewModel()
        {
            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;
            RefreshCalendar();
        }

        /// <summary>
        /// Перестроить календарь на текущий месяц.
        /// </summary>
        public void RefreshCalendar()
        {
            Days.Clear();

            var firstDayOfMonth = new DateTime(Year, Month, 1);
            int offset = (int)firstDayOfMonth.DayOfWeek;
            if (offset == 0) offset = 7; // Воскресенье как последний день недели

            var startDate = firstDayOfMonth.AddDays(-offset);

            for (int i = 0; i < 42; i++) // всегда 6 недель = 42 дня
            {
                var date = startDate.AddDays(i);

                Days.Add(new DayItem
                {
                    DayNumber = date.Day,
                    Date = date,
                    IsCurrentMonth = date.Month == Month,
                    IsToday = date.Date == DateTime.Today
                });
            }
        }

        /// <summary>
        /// Переключение месяца (смещение на ±N месяцев).
        /// </summary>
        public void MoveMonth(int delta)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(delta);
            Year = newDate.Year;
            Month = newDate.Month;
            RefreshCalendar();
        }
    }
}
