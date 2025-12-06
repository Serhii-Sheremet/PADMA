using PADMA.Core.Models;
using PADMA.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // === ★ Добавлено: поддержка CultureCode ===
        private string _cultureCode;
        public string CultureCode
        {
            get => _cultureCode;
            private set
            {
                if (_cultureCode != value)
                {
                    _cultureCode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
        }

        public CultureInfo CurrentCulture =>
            !string.IsNullOrWhiteSpace(CultureCode)
                ? new CultureInfo(CultureCode)
                : CultureInfo.CurrentUICulture;

        // === Инициализация культуры ===
        public void InitializeCulture()
        {
            ReloadCultureAndRefresh();
        }

        public void ReloadCultureAndRefresh()
        {
            try
            {
                var code = DataCache.Instance.CurrentLanguageCode;
                string newCulture = code switch
                {
                    "en" => "en-US",
                    "uk" => "uk-UA",
                    "pl" => "pl-PL",
                    "ru" => "ru-RU",
                    _ => CultureInfo.CurrentUICulture.Name
                };

                if (!string.Equals(CultureCode, newCulture, StringComparison.OrdinalIgnoreCase))
                {
                    CultureCode = newCulture;
                    OnPropertyChanged(nameof(CurrentCulture));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PADMA] ReloadCultureAndRefresh failed: {ex.Message}");
                CultureCode = CultureInfo.CurrentUICulture.Name;
                OnPropertyChanged(nameof(CurrentCulture));
            }

            RefreshCalendar();
        }


        // === Опционально: обновлённый заголовок месяца (если где-то нужен)
        private string _monthTitle;
        public string MonthTitle
        {
            get => _monthTitle;
            private set
            {
                if (_monthTitle != value)
                {
                    _monthTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateMonthTitle()
        {
            try
            {
                MonthTitle = new DateTime(Year, Month, 1).ToString("MMMM yyyy", CurrentCulture);
            }
            catch
            {
                MonthTitle = $"{Year}-{Month:00}";
            }
        }
        // === Конец блока культуры ===


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
            UpdateMonthTitle(); // добавлено, чтобы заголовок обновлялся по культуре
        }

        /// <summary>
        /// Core: build a 6x7 grid (42 days) based on the selected first day of week.
        /// </summary>
        private void GenerateDays(int year, int month)
        {
            Days.Clear();

            var firstOfMonth = new DateTime(Year, Month, 1);
            int shift = ((7 + (int)firstOfMonth.DayOfWeek - (int)DataCache.Instance.DayOfWeek) % 7);
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

                Profile? aProfile = DataCache.Instance.ActiveProfile;
                if (aProfile != null)
                {
                    
                    //// 1. определяем таймзону по месту проживания профиля
                    //var tz = TimeZoneService.GetTimeZoneInfoFromCoordinates(
                    //    aProfile.LivingLatitude,
                    //    aProfile.LivingLongitude);
                    //
                    //// 2. получаем окно видимости + буфер (локальное время)
                    //var (visibleStartLocal, visibleEndLocal,
                    //     bufferStartLocal, bufferEndLocal,
                    //     visibleDays) = CalendarWindowService.GetVisibleWindow(
                    //                        year, month, _firstDayOfWeek, tz);
                    //
                    //// 3. переводим буфер в UTC
                    //var bufferStartUtc = bufferStartLocal.UtcDateTime;
                    //var bufferEndUtc = bufferEndLocal.UtcDateTime;
                    //
                    //// 4. здесь ты вызываешь свою связку SwissAnalysis + TithiTransitBuilder
                    ////    Ниже просто схематично!
                    //List<TithiData> tithiData = _swissAnalysis.BuildTithiData(bufferStartUtc, bufferEndUtc /*, profile, tz, ... */);
                    //List<TithiSlice> tithiSlices = TithiTransitBuilder.BuildTithiSlices(tithiData);
                    //
                    //
                    //foreach (var day in Days) // Days — твоя ObservableCollection<DayItem> для 42 дней
                    //{
                    //    // тут важно: day.Date — это локальная дата
                    //    day.TithiSegments = PanchangaHelper.BuildSegmentsForDay(
                    //        tithiSlices,            // все слайсы Tithi в UTC
                    //        day.Date,               // дата дня в локальном времени
                    //        tz,                     // таймзона профиля
                    //        _cache,                 // для GetColor
                    //        slice => slice.ColorCode // здесь подставь реальное поле у TithiSlice/CalendarSlice
                    //    );
                    //}
                    
                }

                /*
                // Example: add dummy Panchanga segments for demonstration
                if (Days[i].Date.Day == 15 && Days[i].IsCurrentMonth)
                {
                    Days[i].TithiSegments.Add(new PanchangaSegment
                    {
                        Start = Days[i].Date.AddHours(0),
                        End = Days[i].Date.AddHours(12),
                        Color = Colors.Red
                    });

                    Days[i].TithiSegments.Add(new PanchangaSegment
                    {
                        Start = Days[i].Date.AddHours(12),
                        End = Days[i].Date.AddHours(24),
                        Color = Colors.Blue
                    });
                }*/

            }

            OnPropertyChanged(nameof(Days));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
