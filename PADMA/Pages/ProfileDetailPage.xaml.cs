using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using System;

namespace PADMA.Pages;

[QueryProperty(nameof(ProfileId), nameof(ProfileId))]
public partial class ProfileDetailPage : ContentPage
{
    private readonly DatabaseService _database;
    private Profile? _profile;
    public int ProfileId { get; set; }

    public ProfileDetailPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // подписка на выбор локации из дочерней страницы
        MessagingCenter.Subscribe<LocationSearchPage, (string Mode, AppLocation Loc)>(
            this, "LocationSelected", OnLocationSelected);

        if (ProfileId > 0)
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

        // Включаем/отключаем режим редактирования
        SetEditMode(ProfileId == 0);
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

        if (string.Equals(mode, "birth", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfBirthId = loc.Id > 0 ? loc.Id : _profile.PlaceOfBirthId;
            _profile.PlaceOfBirthLocality = loc.Locality;
            btnPlaceOfBirth.Text = loc.Locality; // обновим UI-кнопку
        }
        else if (string.Equals(mode, "living", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfLivingId = loc.Id > 0 ? loc.Id : _profile.PlaceOfLivingId;
            _profile.PlaceOfLivingLocality = loc.Locality;
            btnPlaceOfLiving.Text = loc.Locality;
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        // открываем поиск локации в режиме выбора места рождения
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        // открываем поиск локации в режиме выбора места проживания
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?mode=living", true);
    }

    private void SetEditMode(bool isEdit)
    {
        entryProfileName.IsEnabled = isEdit;
        entryPersonName.IsEnabled = isEdit;
        entryPersonSurname.IsEnabled = isEdit;
        dateOfBirth.IsEnabled = isEdit;
        btnPlaceOfBirth.IsEnabled = isEdit;
        btnPlaceOfLiving.IsEnabled = isEdit;
        entryMessage.IsEnabled = isEdit;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_profile == null) return;

        // простая валидация (минимум)
        if (string.IsNullOrWhiteSpace(_profile.ProfileName))
        {
            await DisplayAlert("Validation", "Profile name is required.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Save", "Save changes to profile?", "Yes", "Cancel");
        if (!confirm) return;

        if (_profile.Id > 0)
            _database.UpdateProfile(_profile);
        else
            _database.AddProfile(_profile);

        await DisplayAlert("Saved", "Profile saved successfully.", "OK");

        // после сохранения — выходим на список
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        // включаем редактирование полей
        SetEditMode(true);
    }

    private async void OnSetDefaultClicked(object sender, EventArgs e)
    {
        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert("Default profile", "Save profile first.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Default profile", "Set this profile as default?", "Yes", "Cancel");
        if (!confirm) return;

        _database.SetDefaultProfile(_profile.Id);
        await DisplayAlert("Done", "Profile marked as default.", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert("Delete", "Nothing to delete.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Delete", "Delete this profile?", "Yes", "Cancel");
        if (!confirm) return;

        _database.DeleteProfile(_profile.Id);
        await DisplayAlert("Deleted", "Profile deleted.", "OK");
        await Shell.Current.GoToAsync("//profiles", true);
    }



}
