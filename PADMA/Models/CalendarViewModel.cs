using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PADMA.Core.Models
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DayItem> Days { get; } = new();

        private int _year;
        public int Year
        {
            get => _year;
            private set { if (_year != value) { _year = value; OnPropertyChanged(nameof(Year)); } }
        }

        private int _month;
        public int Month
        {
            get => _month;
            private set { if (_month != value) { _month = value; OnPropertyChanged(nameof(Month)); } }
        }

        private DayItem _selectedDay;
        public DayItem SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    if (_selectedDay != null) _selectedDay.IsSelected = false;
                    _selectedDay = value;
                    if (_selectedDay != null) _selectedDay.IsSelected = true;
                    OnPropertyChanged(nameof(SelectedDay));
                }
            }
        }

        public CalendarViewModel()
        {
            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;
            GenerateDays(Year, Month);
            // автоселект сегодня
            SelectedDay = Days.FirstOrDefault(d => d.IsCurrentMonth && d.DayNumber == today.Day)
                       ?? Days.First(d => d.IsCurrentMonth && d.DayNumber == 1);
        }

        public void RefreshCalendar()
        {
            // сохраняем выбранный номер дня, чтобы не терять выбор при смене первого дня недели
            int? selectedNumber = SelectedDay?.DayNumber;
            GenerateDays(Year, Month);
            if (selectedNumber.HasValue)
                SelectedDay = Days.FirstOrDefault(d => d.IsCurrentMonth && d.DayNumber == selectedNumber.Value)
                           ?? Days.First(d => d.IsCurrentMonth && d.DayNumber == 1);
        }

        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            int startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; // 0 = Sunday
            if (AppSettings.FirstDayOfWeek == FirstDayOfWeek.Monday)
                startDayOfWeek = startDayOfWeek == 0 ? 6 : startDayOfWeek - 1;

            // previous month lead-in
            DateTime prevMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
            int leading = startDayOfWeek;

            for (int i = leading - 1; i >= 0; i--)
            {
                int dayNum = daysInPrevMonth - i;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, dayNum);
                Days.Add(new DayItem
                {
                    DayNumber = dayNum,
                    IsCurrentMonth = false,
                    Date = date,
                    IsToday = date.Date == DateTime.Today
                });
            }

            // current month
            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(year, month, d);
                Days.Add(new DayItem
                {
                    DayNumber = d,
                    IsCurrentMonth = true,
                    Date = date,
                    IsToday = date.Date == DateTime.Today
                });
            }

            // next month tail to reach 42 cells
            DateTime nextMonth = firstDayOfMonth.AddMonths(1);
            int nextDayNum = 1;
            while (Days.Count < 42)
            {
                var date = new DateTime(nextMonth.Year, nextMonth.Month, nextDayNum++);
                Days.Add(new DayItem
                {
                    DayNumber = date.Day,
                    IsCurrentMonth = false,
                    Date = date,
                    IsToday = date.Date == DateTime.Today
                });
            }
        }


        public void MoveMonth(int offset)
        {
            var newDate = new DateTime(Year, Month, 1).AddMonths(offset);
            Year = newDate.Year;
            Month = newDate.Month;

            GenerateDays(Year, Month);

            // логика выбора: если это текущий месяц — выбираем сегодня, иначе 1 число
            var today = DateTime.Today;
            if (Year == today.Year && Month == today.Month)
                SelectedDay = Days.FirstOrDefault(d => d.IsCurrentMonth && d.DayNumber == today.Day)
                           ?? Days.First(d => d.IsCurrentMonth && d.DayNumber == 1);
            else
                SelectedDay = Days.First(d => d.IsCurrentMonth && d.DayNumber == 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
