using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Core.Analysis;

namespace PADMA.Pages
{
    public partial class ProfilesPage : ContentPage
    {
        private readonly DatabaseService _database;
        private ProfileViewItem? _selectedProfile;
        public ObservableCollection<ProfileViewItem> Profiles { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged(); // ContentPage это BindableObject Ч работает
            }
        }

        private string _busyText = "Please waitЕ";
        public string BusyText
        {
            get => _busyText;
            set
            {
                if (_busyText == value) return;
                _busyText = value;
                OnPropertyChanged();
            }
        }

        private async Task RunBusyAsync(string text, Func<Task> action)
        {
            BusyText = text;
            IsBusy = true;

            // важно: дать UI шанс отрисовать overlay
            await Task.Yield();

            try
            {
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Command<ProfileViewItem> OpenProfileCommand { get; }

        public ProfilesPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
            BindingContext = this;

            OpenProfileCommand = new Command<ProfileViewItem>(async p => await NavigateToProfile(p));

            // локализаци€ заголовка и кнопки
            string lang = DataCache.Instance.CurrentLanguageCode;
            Title = Localization.GetLocalizedText("Profiles", lang);
            btnAddProfile.Text = Localization.GetLocalizedText("Add new profile", lang);
            btnDetails.Text = Localization.GetLocalizedText("Details", lang);
            btnSetDefault.Text = Localization.GetLocalizedText("Set default", lang);
            btnChoose.Text = Localization.GetLocalizedText("Choose", lang);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProfiles();
            ClearSelection();
        }

        private void LoadProfiles()
        {
            int? selectedId = _selectedProfile?.Id;

            Profiles.Clear();
            var profiles = DataCache.Instance.GetProfiles(_database);

            foreach (var p in profiles)
            {
                Profiles.Add(new ProfileViewItem
                {
                    Id = p.Id,
                    ProfileName = p.ProfileName,
                    IsDefault = p.Checked
                });
            }

            if (selectedId.HasValue)
                _selectedProfile = Profiles.FirstOrDefault(p => p.Id == selectedId.Value);

            UpdateActionsVisibility();
        }

        private void OnProfileSelected(object sender, SelectionChangedEventArgs e)
        {
            _selectedProfile = e.CurrentSelection?.FirstOrDefault() as ProfileViewItem;
            UpdateActionsVisibility();
        }

        private void UpdateActionsVisibility()
        {
            bool has = _selectedProfile != null;
            actionsPanel.IsVisible = has;

            btnDetails.IsEnabled = has;
            btnSetDefault.IsEnabled = has && !(_selectedProfile?.IsDefault ?? false);

            // disable Choose if already active
            var activeId = DataCache.Instance.ActiveProfile?.Id;
            bool isAlreadyActive = has && activeId.HasValue && _selectedProfile!.Id == activeId.Value;

            btnChoose.IsEnabled = has && !isAlreadyActive;
        }

        private async void OnDetailsClicked(object sender, EventArgs e)
        {
            if (_selectedProfile == null) return;
            await NavigateToProfile(_selectedProfile);
        }

        private async void OnSetDefaultClicked(object sender, EventArgs e)
        {
            if (_selectedProfile == null) return;

            string lang = DataCache.Instance.CurrentLanguageCode;

            bool confirm = await DisplayAlert(
                Localization.GetLocalizedText("Default profile", lang),
                Localization.GetLocalizedText("Set this profile as default?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (!confirm) return;

            _database.SetDefaultProfile(_selectedProfile.Id);

            // обновить список и сохранить выбор
            int keepId = _selectedProfile.Id;
            LoadProfiles();

            _selectedProfile = Profiles.FirstOrDefault(p => p.Id == keepId);
            UpdateActionsVisibility();

            await DisplayAlert(
                Localization.GetLocalizedText("Done", lang),
                Localization.GetLocalizedText("Profile marked as default.", lang),
                "OK"
            );
        }

        private async void OnChooseClicked(object sender, EventArgs e)
        {
            if (_selectedProfile == null) return;

            var profile = DataCache.Instance.GetProfiles(_database)
                .FirstOrDefault(p => p.Id == _selectedProfile.Id);

            if (profile == null) return;

            var lang = DataCache.Instance.CurrentLanguageCode;
            await RunBusyAsync(Localization.GetLocalizedText("Switching profileЕ", lang), async () =>
            {
                DataCache.Instance.ActiveProfile = profile;
                DataCache.Instance.ReloadLocations(_database);
                await DataCache.Instance.ProfileContextService.RebuildAsync();

                MessagingCenter.Send<object>(this, "ProfileChanged");
                
                SwissAnalysis.ClearZodiacBoundaryCache();
                ClearSelection();
                
                await Shell.Current.GoToAsync("//main", true);

                // ƒать UI шанс показать MainPage overlay
                await Task.Yield();
            });
        }

        private async Task NavigateToProfile(ProfileViewItem? profile)
        {
            if (profile == null) return;
            var route = $"{nameof(ProfileDetailPage)}?ProfileId={profile.Id}";
            await Shell.Current.GoToAsync(route, true);
        }

        private async void OnAddProfileClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(ProfileDetailPage), true);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            ClearSelection();
            await Shell.Current.GoToAsync("//main", true);
        }

        private void ClearSelection()
        {
            _selectedProfile = null;

            if (profilesView != null)
                profilesView.SelectedItem = null;

            UpdateActionsVisibility();
        }




    }

    public class ProfileViewItem
    {
        public int Id { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }
    }
}
