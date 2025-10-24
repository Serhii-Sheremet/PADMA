using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages
{
    public partial class ProfilesPage : ContentPage
    {
        private readonly DatabaseService _database;
        public ObservableCollection<ProfileViewItem> Profiles { get; } = new();
        public Command<ProfileViewItem> OpenProfileCommand { get; }

        public ProfilesPage(DatabaseService database)
        {
            InitializeComponent();
            _database = database;
            BindingContext = this;

            OpenProfileCommand = new Command<ProfileViewItem>(async p => await NavigateToProfile(p));

            // локализация заголовка и кнопки
            Title = Localization.GetLocalizedText("Profiles", _database.GetActiveLanguageCode());
            btnAddProfile.Text = Localization.GetLocalizedText("Add new profile", _database.GetActiveLanguageCode());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            Profiles.Clear();
            var profiles = _database.GetProfiles();

            foreach (var p in profiles)
            {
                Profiles.Add(new ProfileViewItem
                {
                    Id = p.Id,
                    ProfileName = p.ProfileName,
                    IsDefault = p.Checked
                });
            }
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
            await Shell.Current.GoToAsync("//main", true);
        }
    }

    public class ProfileViewItem
    {
        public int Id { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
