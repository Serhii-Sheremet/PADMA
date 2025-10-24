using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace PADMA.Pages;

[QueryProperty(nameof(ProfileId), nameof(ProfileId))]
public partial class ProfileDetailPage : ContentPage
{
    private readonly DatabaseService _database;
    private Profile? _profile;
    private static Profile? _tempProfile;
    public int ProfileId { get; set; }

    private bool _isEditing = false;
    private bool _hasChanges = false;
    private bool _isInitializing = false;
    private string? _snapshotJson;   // снимок состояния для проверки реальных изменений
    private bool _isClosingByButton = false;

    public ProfileDetailPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;

        // подписка на выбор локации
        MessagingCenter.Unsubscribe<LocationSearchPage, (string, AppLocation)>(this, "LocationSelected");
        MessagingCenter.Subscribe<LocationSearchPage, (string, AppLocation)>(
            this, "LocationSelected", OnLocationSelected);

        // отслеживаем реальные изменения только после инициализации
        entryProfileName.TextChanged += (_, _) => MarkChanged();
        entryPersonName.TextChanged += (_, _) => MarkChanged();
        entryPersonSurname.TextChanged += (_, _) => MarkChanged();
        entryMessage.TextChanged += (_, _) => MarkChanged();
        dateOfBirthDate.DateSelected += (_, _) => MarkChanged();
        timeOfBirthTime.PropertyChanged += (_, _) => MarkChanged();
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

    private void MarkChanged()
    {
        if (_isInitializing) return;
        _hasChanges = true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _isInitializing = true;

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

        RefreshLocationLabels();

        dateOfBirthDate.Date = _profile.DateOfBirth.Date;
        timeOfBirthTime.Time = _profile.DateOfBirth.TimeOfDay;

        ApplyLocalization();

        if (!_isEditing)
            SetEditMode(ProfileId == 0);

        _isInitializing = false;
        _hasChanges = false;

        // сохраняем "снимок" модели для последующего сравнения
        _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);
    }

    private void RefreshLocationLabels()
    {
        lblPlaceOfBirthValue.Text = string.IsNullOrWhiteSpace(_profile?.PlaceOfBirthLocality)
            ? Localization.GetLocalizedText("Select location...", _database.GetActiveLanguageCode())
            : _profile.PlaceOfBirthLocality;

        lblPlaceOfLivingValue.Text = string.IsNullOrWhiteSpace(_profile?.PlaceOfLivingLocality)
            ? Localization.GetLocalizedText("Select location...", _database.GetActiveLanguageCode())
            : _profile.PlaceOfLivingLocality;
    }

    // общий метод — показывает диалог Yes/No и обрабатывает всё
    private async Task ConfirmExitAsync()
    {
        string lang = _database.GetActiveLanguageCode();

        if (HasRealChanges()) // есть реальные изменения?
        {
            bool save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes", lang),
                Localization.GetLocalizedText("Do you want to save changes before exit?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang)
            );

            if (save)
            {
                bool saved = await SaveProfileAsync();
                if (!saved)
                {
                    // валидация не прошла — остаёмся на странице
                    return;
                }
            }
            // если выбрал "No" — просто выходим
        }

        // нет изменений или успешно сохранили — выходим
        await Shell.Current.GoToAsync("//profiles", true);
    }

    // обработка крестика — просто ставим флаг и вызываем ConfirmExitAsync()
    private async void OnCloseClicked(object sender, EventArgs e)
    {
        _isClosingByButton = true;
        await ConfirmExitAsync();
    }

    // обработка стрелки "Назад" (или свайпа) через OnNavigatingFrom
    protected override async void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

        // если выходим через крестик — уже обработано
        if (_isClosingByButton)
            return;

        // проверяем изменения
        if (HasRealChanges())
        {
            // "имитируем отмену" — возвращаем пользователя обратно на текущую страницу
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // показываем тот же диалог
                await ConfirmExitAsync();
            });
        }
    }


    // новая логика сравнения текущего состояния с сохранённым снимком
    private bool HasRealChanges()
    {
        if (_profile == null) return false;
        try
        {
            var current = System.Text.Json.JsonSerializer.Serialize(_profile);
            return current != _snapshotJson;
        }
        catch { return _hasChanges; }
    }

    private async Task<bool> SaveProfileAsync()
    {
        if (_profile == null) return false;

        string lang = _database.GetActiveLanguageCode();

        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(entryProfileName.Text))
        {
            await DisplayAlert(Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Profile name is required.", lang), "OK");
            return false;
        }

        if (_profile.DateOfBirth == default)
        {
            await DisplayAlert(Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Date of birth is required.", lang), "OK");
            return false;
        }

        if (_profile.PlaceOfBirthId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfBirthLocality))
        {
            await DisplayAlert(Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of birth is required.", lang), "OK");
            return false;
        }

        if (_profile.PlaceOfLivingId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfLivingLocality))
        {
            await DisplayAlert(Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of living is required.", lang), "OK");
            return false;
        }

        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;

        try
        {
            if (_profile.Id > 0)
                _database.UpdateProfile(_profile);
            else
                _database.AddProfile(_profile);

            _hasChanges = false;
            _isEditing = false;
            SetEditMode(false);

            // обновляем снимок после сохранения
            _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);

            await DisplayAlert(Localization.GetLocalizedText("Saved", lang),
                Localization.GetLocalizedText("Profile saved successfully.", lang), "OK");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PADMA] Save profile error: {ex.Message}");
            await DisplayAlert(Localization.GetLocalizedText("Error", lang),
                Localization.GetLocalizedText("Failed to save profile. Please try again.", lang), "OK");
            return false;
        }
    }

    private void OnLocationSelected(LocationSearchPage sender, (string Mode, AppLocation Loc) payload)
    {
        var (mode, loc) = payload;

        System.Diagnostics.Debug.WriteLine($"[ProfileDetail] LocationSelected: mode='{mode}', loc='{loc?.Locality}', id={loc?.Id}");

        if (_profile == null) return;

        if (mode.Equals("birth", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfBirthId = loc.Id;
            _profile.PlaceOfBirthLocality = loc.Locality;
        }
        else if (mode.Equals("living", StringComparison.OrdinalIgnoreCase))
        {
            _profile.PlaceOfLivingId = loc.Id;
            _profile.PlaceOfLivingLocality = loc.Locality;
        }
        else
        {
            // fallback: если Mode пустой или неизвестный
            _profile.PlaceOfBirthId = loc.Id;
            _profile.PlaceOfBirthLocality = loc.Locality;
        }

        _tempProfile = _profile;
        _hasChanges = true;

        MainThread.BeginInvokeOnMainThread(RefreshLocationLabels);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string lang = _database.GetActiveLanguageCode();

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Save", lang),
            Localization.GetLocalizedText("Save changes to profile?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang)
        );

        if (!confirm) return;

        bool result = await SaveProfileAsync();

        if (result)
            await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        _tempProfile = _profile;
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?Mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        _tempProfile = _profile;
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?Mode=living", true);
    }

    private void SetEditMode(bool isEdit)
    {
        _isEditing = isEdit;

        entryProfileName.IsEnabled = isEdit;
        entryPersonName.IsEnabled = isEdit;
        entryPersonSurname.IsEnabled = isEdit;
        dateOfBirthDate.IsEnabled = isEdit;
        timeOfBirthTime.IsEnabled = isEdit;
        entryMessage.IsEnabled = isEdit;

        // блокируем выбор локаций при просмотре
        btnPlaceOfBirth.IsEnabled = isEdit;
        btnPlaceOfLiving.IsEnabled = isEdit;
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
