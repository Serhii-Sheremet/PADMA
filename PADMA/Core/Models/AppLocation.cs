// PADMA/Core/Models/AppLocation.cs
using SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PADMA.Core.Models
{
    [Table("LOCATION")]
    public class AppLocation : INotifyPropertyChanged
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

        // ------- UI-only поля -------
        private bool _isSelected;
        [Ignore]
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private string? _displayNameOverride;
        [Ignore]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_displayNameOverride))
                    return _displayNameOverride;

                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Locality)) parts.Add(Locality);

                var region = !string.IsNullOrWhiteSpace(Region) ? Region : State;
                if (!string.IsNullOrWhiteSpace(region)) parts.Add(region!);

                if (!string.IsNullOrWhiteSpace(Country)) parts.Add(Country);
                return string.Join(", ", parts);
            }
            set { _displayNameOverride = value; OnPropertyChanged(); }
        }

        private string? _coordsOverride;
        [Ignore]
        public string CoordinatesString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_coordsOverride))
                    return _coordsOverride;

                if (double.TryParse(Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                    double.TryParse(Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                    return $"Lat: {lat:F4}, Lon: {lon:F4}";

                return $"Lat: {Latitude}, Lon: {Longitude}";
            }
            set { _coordsOverride = value; OnPropertyChanged(); }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
