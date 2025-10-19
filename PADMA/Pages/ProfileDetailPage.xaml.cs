using CloudKit;
using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PADMA.Pages;

[QueryProperty(nameof(ProfileId), nameof(ProfileId))]
public partial class ProfileDetailPage : ContentPage
{
    private readonly DatabaseService _database;
    private Profile? _profile;
    public int ProfileId { get; set; }

    public ProfileDetailPage()
    {
        InitializeComponent();
        
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ProfileId > 0)
        {
            // Загрузка существующего профайла
            _profile = await DatabaseService.Instance.GetProfileByIdAsync(ProfileId);
        }
        else
        {
            // Новый профиль
            _profile = new Profile
            {
                DateOfBirth = DateTime.Now,
                Message = "",
                PersonName = "",
                PersonSurname = "",
                ProfileName = ""
            };
        }

        BindingContext = _profile;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Save",
            "Save changes to profile?",
            "Yes", "Cancel");

        if (!confirm) return;

        if (_profile == null) return;

        if (_profile.Id > 0)
            await DatabaseService.Instance.UpdateProfileAsync(_profile);
        else
            await DatabaseService.Instance.AddProfileAsync(_profile);

        await DisplayAlert("Saved", "Profile saved successfully.", "OK");
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        entryProfileName.IsEnabled = true;
        entryPersonName.IsEnabled = true;
        entryPersonSurname.IsEnabled = true;
        dateOfBirth.IsEnabled = true;
        btnPlaceOfBirth.IsEnabled = true;
        btnPlaceOfLiving.IsEnabled = true;
        entryMessage.IsEnabled = true;
    }

    private async void OnSetDefaultClicked(object sender, EventArgs e)
    {
        if (_profile == null) return;

        bool confirm = await DisplayAlert("Default profile", "Set this profile as default?", "Yes", "Cancel");
        if (!confirm) return;

        await DatabaseService.Instance.SetDefaultProfileAsync(_profile.Id);
        await DisplayAlert("Done", "Profile marked as default.", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_profile == null || _profile.Id == 0) return;

        bool confirm = await DisplayAlert("Delete", "Delete this profile?", "Yes", "Cancel");
        if (!confirm) return;

        await DatabaseService.Instance.DeleteProfileAsync(_profile.Id);
        await DisplayAlert("Deleted", "Profile deleted.", "OK");
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=living", true);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profiles", true);
    }
}
