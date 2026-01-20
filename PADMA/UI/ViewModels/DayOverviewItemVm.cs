using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PADMA.UI.ViewModels
{
    public sealed class DayOverviewItemVm : INotifyPropertyChanged
    {
        public DayItem Day { get; }

        private DayOverviewData? _overview;
        public DayOverviewData? Overview
        {
            get => _overview;
            set { _overview = value; OnPropertyChanged(); }
        }

        public DayOverviewItemVm(DayItem day) => Day = day;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
