using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using System.Collections.ObjectModel;
using PADMA.Core.Services;
using PADMA.Core.Utilities;

namespace PADMA.Pages;

public partial class ProfilesPage : ContentPage
{
    public ObservableCollection<ProfileViewItem> Profiles { get; set; }

    // Добавляем команду для открытия профайла
    public Command<ProfileViewItem> OpenProfileCommand { get; }

    public ProfilesPage()
    {
        InitializeComponent();
        Profiles = new ObservableCollection<ProfileViewItem>();

        // Инициализируем команду
        OpenProfileCommand = new Command<ProfileViewItem>(async (item) => await OpenProfile(item));

        BindingContext = this;

        LoadDemoProfiles();
    }

    private void LoadDemoProfiles()
    {
        Profiles.Add(new ProfileViewItem { Id = 1, ProfileName = "John Doe", IsDefault = true });
        Profiles.Add(new ProfileViewItem { Id = 2, ProfileName = "Mary Smith", IsDefault = false });
    }

    private async Task OpenProfile(ProfileViewItem? profile)
    {
        if (profile == null) return;
        string route = $"{nameof(ProfileDetailPage)}?profileId={profile.Id}";
        await Shell.Current.GoToAsync(route, true);
    }

    private async void OnAddProfileClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ProfileDetailPage), true);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        // Возврат к MainPage (аналогично ConfigurationPage)
        await Shell.Current.GoToAsync("//main", true);
    }
}

public class ProfileViewItem
{
    public int Id { get; set; }
    public string ProfileName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
