using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Globalization;

namespace PADMA.Pages;

[QueryProperty(nameof(ProfileId), nameof(ProfileId))]
public partial class ProfileDetailPage : ContentPage
{
    private readonly DatabaseService _database;
    private Profile? _profile;
    private static Profile? _tempProfile;
    public int ProfileId { get; set; }

    public ProfileDetailPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        MessagingCenter.Subscribe<LocationSearchPage, (string Mode, AppLocation Loc)>(
            this, "LocationSelected", OnLocationSelected);

        if (_tempProfile != null)
        {
            _profile = _tempProfile;
            _tempProfile = null;
        }
        else if (ProfileId > 0)
        {
            _profile = _database.GetProfileById(ProfileId);
            if (_profile?.PlaceOfBirthId is int pbId)
                _profile.PlaceOfBirthLocality = _database.GetLocationById(pbId)?.Locality ?? "";
            if (_profile?.PlaceOfLivingId is int plId)
                _profile.PlaceOfLivingLocality = _database.GetLocationById(plId)?.Locality ?? "";
        }
        else
        {
            _profile = new Profile
            {
                DateOfBirth = DateTime.Now,
                ProfileName = "",
                PersonName = "",
                PersonSurname = "",
                Message = ""
            };
        }

        BindingContext = _profile;

        dateOfBirthDate.Date = _profile.DateOfBirth.Date;
        timeOfBirthTime.Time = _profile.DateOfBirth.TimeOfDay;

        ApplyLocalization();

        SetEditMode(ProfileId == 0);
    }

    private void ApplyLocalization()
    {
        string langCode = _database.GetActiveLanguageCode();

        Title = Localization.GetLocalizedText("Profile", langCode);
        lblDateTimeOfBirth.Text = Localization.GetLocalizedText("Date and time of birth", langCode);
        lblPlaceOfBirth.Text = Localization.GetLocalizedText("Place of birth", langCode);
        lblPlaceOfLiving.Text = Localization.GetLocalizedText("Place of living", langCode);
        entryProfileName.Placeholder = Localization.GetLocalizedText("Profile name", langCode);
        entryPersonName.Placeholder = Localization.GetLocalizedText("First name", langCode);
        entryPersonSurname.Placeholder = Localization.GetLocalizedText("Last name", langCode);
        entryMessage.Placeholder = Localization.GetLocalizedText("Notes", langCode);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<LocationSearchPage, (string, AppLocation)>(this, "LocationSelected");
    }

    private void OnLocationSelected(LocationSearchPage sender, (string Mode, AppLocation Loc) payload)
    {
        if (_profile == null) return;
        var (mode, loc) = payload;

        if (mode.Equals("birth", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfBirthId = loc.Id;
            _profile.PlaceOfBirthLocality = loc.Locality;
            lblPlaceOfBirthValue.Text = loc.Locality;
        }
        else if (mode.Equals("living", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfLivingId = loc.Id;
            _profile.PlaceOfLivingLocality = loc.Locality;
            lblPlaceOfLivingValue.Text = loc.Locality;
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        bool confirmExit = await DisplayAlert(
            Localization.GetLocalizedText("Exit", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Discard unsaved changes and close?", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Yes", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("No", _database.GetActiveLanguageCode())
        );

        if (!confirmExit) return;
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        _tempProfile = _profile;
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        _tempProfile = _profile;
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=living", true);
    }

    private void SetEditMode(bool isEdit)
    {
        entryProfileName.IsEnabled = isEdit;
        entryPersonName.IsEnabled = isEdit;
        entryPersonSurname.IsEnabled = isEdit;
        dateOfBirthDate.IsEnabled = isEdit;
        timeOfBirthTime.IsEnabled = isEdit;
        entryMessage.IsEnabled = isEdit;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_profile == null) return;

        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;

        string formattedDate = _profile.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Save", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Save changes to profile?", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Yes", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("No", _database.GetActiveLanguageCode())
        );

        if (!confirm) return;

        if (_profile.Id > 0)
            _database.UpdateProfile(_profile);
        else
            _database.AddProfile(_profile);

        await DisplayAlert(
            Localization.GetLocalizedText("Saved", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Profile saved successfully.", _database.GetActiveLanguageCode()),
            "OK"
        );

        await Shell.Current.GoToAsync("//profiles", true);
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        SetEditMode(true);
    }

    private async void OnSetDefaultClicked(object sender, EventArgs e)
    {
        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Default profile", _database.GetActiveLanguageCode()),
                Localization.GetLocalizedText("Save profile first.", _database.GetActiveLanguageCode()),
                "OK"
            );
            return;
        }

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Default profile", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Set this profile as default?", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Yes", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("No", _database.GetActiveLanguageCode())
        );

        if (!confirm) return;

        _database.SetDefaultProfile(_profile.Id);
        await DisplayAlert(
            Localization.GetLocalizedText("Done", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Profile marked as default.", _database.GetActiveLanguageCode()),
            "OK"
        );
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert("Delete", "Nothing to delete.", "OK");
            return;
        }

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Delete", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Delete this profile?", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("Yes", _database.GetActiveLanguageCode()),
            Localization.GetLocalizedText("No", _database.GetActiveLanguageCode())
        );

        if (!confirm) return;

        _database.DeleteProfile(_profile.Id);
        await DisplayAlert("Deleted", "Profile deleted.", "OK");
        await Shell.Current.GoToAsync("//profiles", true);
    }
}
