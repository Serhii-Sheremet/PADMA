using Microsoft.VisualBasic;
using PADMA.Core.Analysis;
using PADMA.Core.Enums;
using PADMA.Core.Models;
using PADMA.Core.Models.Calendar;
using PADMA.Core.Services;
using PADMA.Core.TransitBuilder;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        // === Добавлено: поддержка CultureCode ===
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
                //System.Diagnostics.Debug.WriteLine($"[PADMA] ReloadCultureAndRefresh failed: {ex.Message}");
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

            TimeZoneInfo? tzInfo = null;
            int birthNakshatraId = 0;
            List<NakshatraSlice>? nakshatraSlices = null;
            List<TaraBalaSlice>? taraBalaSlices = null;
            List<TithiSlice>? tithiSlices = null;
            Profile? profile = DataCache.Instance.ActiveProfile;

            AppLocation? birthLocation = null;
            if (profile != null)
            {
                birthLocation = DataCache.Instance.LocationList
                    .FirstOrDefault(l => l.Id == profile.PlaceOfBirthId);
                // Получаем накшатру рождения, если есть дата и место рождения
                if (birthLocation != null)
                {
                    if (double.TryParse(birthLocation.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var latit) &&
                        double.TryParse(birthLocation.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var longi))
                    {
                        double offset = TimeZoneService.GetUtcOffsetHours(profile.DateOfBirth, latit, longi);
                        DateTime localBD = profile.DateOfBirth.AddHours(-offset);

                        var bdPlanetData = SwissAnalysis.CalculatePlanetsPositionForDate(localBD, latit, longi);
                        birthNakshatraId = bdPlanetData.Where(i => i.PlanetId == (int)EPlanet.MOON).FirstOrDefault()?.NakshatraId ?? 1;
                    }
                }
            }

            // Если профиля нет — просто строим календарь по датам, без полосок
            AppLocation? livingLocation = null;
            if (profile != null)
            {
                livingLocation = DataCache.Instance.LocationList
                    .FirstOrDefault(l => l.Id == profile.PlaceOfLivingId);
            }

            // Считаем окно календаря (если есть таймзона)
            DateTimeOffset visibleStart, visibleEnd, bufferStart, bufferEnd;
            IReadOnlyList<DateOnly> visibleDays;

            if (livingLocation != null &&
                double.TryParse(livingLocation.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(livingLocation.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                var tzId = TimeZoneService.GetDotNetTimeZoneId(lat, lon);
                tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);

                (visibleStart, visibleEnd, bufferStart, bufferEnd, visibleDays) =
                    CalendarWindowService.BuildWindow(year, month, DataCache.Instance.DayOfWeek, tzInfo);

                
                var bufferStartUtc = bufferStart.UtcDateTime;
                var bufferEndUtc = bufferEnd.UtcDateTime;

                // ---- Swiss + Nakshatra (Луна) + TaraBala ----
                var moonData = SwissAnalysis.CalculatePlanetDataList_London((int)EPlanet.MOON, bufferStartUtc, bufferEndUtc);
                nakshatraSlices = NakshatraTransitBuilder.BuildNakshatraSlices(moonData);
                if (birthNakshatraId > 0)
                    taraBalaSlices = TaraBalaTransitBuilder.BuildTaraBalaSlices(nakshatraSlices, birthNakshatraId);



                // ---- Swiss + Tithi —---
                var tithiData = SwissAnalysis.CalculateTithiDataList_London(bufferStartUtc, bufferEndUtc);
                tithiSlices = TithiTransitBuilder.BuildTithiSlices(tithiData);

                
            }
            else
            {
                // если профиля/локации/таймзоны нет — строим видимые дни по старой логике
                var firstOfMonth = new DateTime(year, month, 1);
                int shift = ((7 + (int)firstOfMonth.DayOfWeek - (int)DataCache.Instance.DayOfWeek) % 7);
                var startDate = firstOfMonth.AddDays(-shift);

                var tmp = new List<DateOnly>();
                for (int i = 0; i < 42; i++)
                    tmp.Add(DateOnly.FromDateTime(startDate.AddDays(i)));

                visibleDays = tmp;
            }

            // Создаём 42 DayItem строго по visibleDays
            foreach (var d in visibleDays)
            {
                var date = d.ToDateTime(TimeOnly.MinValue);
                bool isCurrentMonth = (date.Month == month && date.Year == year);
                bool isToday = date.Date == DateTime.Today;

                IList<PanchangaSegment> nakshatraSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> taraBalaSegments = new List<PanchangaSegment>();
                IList<PanchangaSegment> tithiSegments = new List<PanchangaSegment>();
                

                if (profile != null && tzInfo != null)
                {
                    if (nakshatraSlices != null)
                    {
                        nakshatraSegments = BuildNakshatraSegmentsForDay(
                        nakshatraSlices,
                        date,
                        tzInfo,
                        DataCache.Instance);
                    }

                    if (taraBalaSlices != null)
                    {
                        taraBalaSegments = BuildTaraBalaSegmentsForDay(
                        taraBalaSlices,
                        date,
                        tzInfo,
                        DataCache.Instance);
                    }

                    if (tithiSlices != null)
                    {
                        tithiSegments = BuildTithiSegmentsForDay(
                        tithiSlices,
                        date,
                        tzInfo,
                        DataCache.Instance);
                    }
                }

                Days.Add(new DayItem
                {
                    Date = date,
                    DayNumber = date.Day,
                    IsCurrentMonth = isCurrentMonth,
                    IsToday = isToday,
                    NakshatraSegments = nakshatraSegments,
                    TaraBalaSegments = taraBalaSegments,
                    TithiSegments = tithiSegments
                });
            }
            
            OnPropertyChanged(nameof(Days));
        }

        private IList<PanchangaSegment> BuildNakshatraSegmentsForDay(
            IEnumerable<NakshatraSlice> nakshatraSlicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache)
        {
            var result = new List<PanchangaSegment>();

            // границы дня в локальном времени
            var offset = tz.GetUtcOffset(dayLocal);
            var dayStartLocal = new DateTimeOffset(dayLocal.Date, offset);
            var dayEndLocal = dayStartLocal.AddDays(1);

            foreach (var slice in nakshatraSlicesUtc)
            {
                // слайс в локальном времени
                var sliceStartLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
                var sliceEndLocal = new DateTimeOffset(slice.EndUtc, TimeSpan.Zero).ToOffset(offset);

                // пересекаем с сутками
                var effStart = sliceStartLocal > dayStartLocal ? sliceStartLocal : dayStartLocal;
                var effEnd = sliceEndLocal < dayEndLocal ? sliceEndLocal : dayEndLocal;

                if (effEnd <= effStart)
                    continue;

                // цвет из ColorId накшатры
                var colorCode = (EColor)slice.ColorId;
                var color = cache.GetColor(colorCode);

                result.Add(new PanchangaSegment
                {
                    Start = effStart.LocalDateTime,
                    End = effEnd.LocalDateTime,
                    Color = color
                });
            }

            return result
                .OrderBy(s => s.Start)
                .ToList();
        }

        private IList<PanchangaSegment> BuildTaraBalaSegmentsForDay(
            IEnumerable<TaraBalaSlice> taraBalaSlicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache)
        {
            var result = new List<PanchangaSegment>();

            // границы дня в локальном времени
            var offset = tz.GetUtcOffset(dayLocal);
            var dayStartLocal = new DateTimeOffset(dayLocal.Date, offset);
            var dayEndLocal = dayStartLocal.AddDays(1);

            foreach (var slice in taraBalaSlicesUtc)
            {
                // слайс в локальном времени
                var sliceStartLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
                var sliceEndLocal = new DateTimeOffset(slice.EndUtc, TimeSpan.Zero).ToOffset(offset);

                // пересекаем с сутками
                var effStart = sliceStartLocal > dayStartLocal ? sliceStartLocal : dayStartLocal;
                var effEnd = sliceEndLocal < dayEndLocal ? sliceEndLocal : dayEndLocal;

                if (effEnd <= effStart)
                    continue;

                // цвет из ColorId накшатры
                var colorCode = (EColor)slice.ColorId;
                var color = cache.GetColor(colorCode);

                result.Add(new PanchangaSegment
                {
                    Start = effStart.LocalDateTime,
                    End = effEnd.LocalDateTime,
                    Color = color
                });
            }

            return result
                .OrderBy(s => s.Start)
                .ToList();
        }

        private IList<PanchangaSegment> BuildTithiSegmentsForDay(
            IEnumerable<TithiSlice> tithiSlicesUtc,
            DateTime dayLocal,
            TimeZoneInfo tz,
            DataCache cache)
        {
            var result = new List<PanchangaSegment>();

            // границы дня в локальном времени
            var offset = tz.GetUtcOffset(dayLocal);
            var dayStartLocal = new DateTimeOffset(dayLocal.Date, offset);
            var dayEndLocal = dayStartLocal.AddDays(1);

            foreach (var slice in tithiSlicesUtc)
            {
                // слайс в локальном времени
                var sliceStartLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
                var sliceEndLocal = new DateTimeOffset(slice.EndUtc, TimeSpan.Zero).ToOffset(offset);

                // пересекаем с сутками
                var effStart = sliceStartLocal > dayStartLocal ? sliceStartLocal : dayStartLocal;
                var effEnd = sliceEndLocal < dayEndLocal ? sliceEndLocal : dayEndLocal;

                if (effEnd <= effStart)
                    continue;

                var colorCode = (EColor)slice.ColorId;
                var color = cache.GetColor(colorCode);

                result.Add(new PanchangaSegment
                {
                    Start = effStart.LocalDateTime,
                    End = effEnd.LocalDateTime,
                    Color = color
                });
            }

            return result
                .OrderBy(s => s.Start)
                .ToList();
        }






        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
