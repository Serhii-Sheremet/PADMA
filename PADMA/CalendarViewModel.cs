using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PADMA
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DayItem> Days { get; } = new ObservableCollection<DayItem>();

        public int Year { get; private set; }
        public int Month { get; private set; }

        public CalendarViewModel()
        {
            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;

            GenerateDays(Year, Month);
        }

        public void RefreshCalendar()
        {
            // Просто пересоздаём текущий месяц, чтобы применился новый первый день недели
            GenerateDays(Year, Month);
        }

        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Сдвиг в зависимости от AppSettings
            if (AppSettings.FirstDayOfWeek == FirstDayOfWeek.Monday)
                startDayOfWeek = startDayOfWeek == 0 ? 6 : startDayOfWeek - 1;

            // Предыдущий месяц
            DateTime prevMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            for (int i = startDayOfWeek - 1; i >= 0; i--)
            {
                Days.Add(new DayItem
                {
                    DayNumber = daysInPrevMonth - i,
                    IsCurrentMonth = false
                });
            }

            // Дни текущего месяца
            for (int d = 1; d <= daysInMonth; d++)
            {
                Days.Add(new DayItem
                {
                    DayNumber = d,
                    IsCurrentMonth = true
                });
            }

            // Дни следующего месяца
            while (Days.Count < 42)
            {
                int nextDay = Days.Count - (startDayOfWeek + daysInMonth - 1);
                Days.Add(new DayItem
                {
                    DayNumber = nextDay,
                    IsCurrentMonth = false
                });
            }
        }


        public void MoveMonth(int offset)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(offset);
            Year = newDate.Year;
            Month = newDate.Month;
            GenerateDays(Year, Month);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Days)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }




}