using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PADMA.Core.Models
{
    [Table("LOCATION")]
    public class AppLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("LOCALITY")]
        public string Locality { get; set; } = "";

        [Column("REGION")]
        public string? Region { get; set; }

        [Column("STATE")]
        public string? State { get; set; }

        [Column("COUNTRY")]
        public string Country { get; set; } = "";

        [Column("COUNTRYCODE")]
        public string? CountryCode { get; set; }

        [Column("LANGUAGECODE")]
        public string? LanguageCode { get; set; }

        [Column("LATITUDE")]
        public string Latitude { get; set; } = "0";

        [Column("LONGITUDE")]
        public string Longitude { get; set; } = "0";

        /// <summary>
        /// Отображаемое имя для UI: "Город, Регион, Страна".
        /// Формируется динамически, но может быть перезаписано при создании объекта.
        /// </summary>
        [Ignore]
        public string DisplayName
        {
            get
            {
                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(Locality))
                    parts.Add(Locality);

                var region = !string.IsNullOrWhiteSpace(Region) ? Region : State;
                if (!string.IsNullOrWhiteSpace(region) && !parts.Contains(region))
                    parts.Add(region!);

                if (!string.IsNullOrWhiteSpace(Country))
                    parts.Add(Country);

                return string.Join(", ", parts);
            }
            set
            {
                // нужен сеттер, чтобы можно было присваивать из NominatimService
                _displayName = value;
            }
        }

        private string? _displayName;

        /// <summary>
        /// Строка координат в удобном для UI виде.
        /// </summary>
        [Ignore]
        public string CoordinatesString
        {
            get
            {
                if (double.TryParse(Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    return $"Lat: {lat:F4}, Lon: {lon:F4}";
                }

                return $"Lat: {Latitude}, Lon: {Longitude}";
            }
            set
            {
                _coordinatesString = value;
            }
        }

        private string? _coordinatesString;
    }
}
