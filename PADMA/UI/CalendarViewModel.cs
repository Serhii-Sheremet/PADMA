using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PADMA.Core.Services;

namespace PADMA.UI
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DayItem> Days { get; } = new ObservableCollection<DayItem>();

        private int _year;
        public int Year
        {
            get => _year;
            private set
            {
                if (_year != value)
                {
                    _year = value;
                    OnPropertyChanged(nameof(Year));
                }
            }
        }

        private int _month;
        public int Month
        {
            get => _month;
            private set
            {
                if (_month != value)
                {
                    _month = value;
                    OnPropertyChanged(nameof(Month));
                }
            }
        }

        public CalendarViewModel()
        {
            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;
            GenerateDays(Year, Month);
        }

        /// <summary>
        /// Rebuild current month grid (apply first-day-of-week changes, etc.).
        /// </summary>
        public void RefreshCalendar()
        {
            GenerateDays(Year, Month);
        }

        /// <summary>
        /// Move month by offset (e.g., -1 = previous, +1 = next).
        /// </summary>
        public void MoveMonth(int offset)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(offset);
            Year = newDate.Year;
            Month = newDate.Month;
            GenerateDays(Year, Month);
        }

        

        /// <summary>
        /// Core: build a 6x7 grid (42 days) based on the selected first day of week.
        /// </summary>
        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            var db = ServiceLocator.Services.GetService<DatabaseService>();
            var firstDay = db.GetFirstDayOfWeekFromDb(); // Sunday или Monday

            var firstOfMonth = new DateTime(Year, Month, 1);
            int shift = ((7 + (int)firstOfMonth.DayOfWeek - (int)firstDay) % 7);
            var startDate = firstOfMonth.AddDays(-shift);

            for (int i = 0; i < 42; i++)
            {
                DateTime date = startDate.AddDays(i);
                bool isCurrentMonth = (date.Month == month && date.Year == year);
                bool isToday = date.Date == DateTime.Today;

                Days.Add(new DayItem
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday
                });
            }

            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
