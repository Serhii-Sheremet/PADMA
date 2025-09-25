using System.ComponentModel;

namespace PADMA
{
    public class DayItem : INotifyPropertyChanged
    {
        public int DayNumber { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
