using Microsoft.Maui.Controls;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System;
using System.Threading.Tasks;

namespace PADMA.Pages;

[QueryProperty(nameof(ProfileId), nameof(ProfileId))]
public partial class ProfileDetailPage : ContentPage
{
    private readonly DatabaseService _database;
    private Profile? _profile;
    private static Profile? _tempProfile;
    public int ProfileId { get; set; }

    private AppLocation? _birthLocation;
    private AppLocation? _livingLocation;

    private bool _isEditing = false;
    private bool _hasChanges = false;
    private bool _isInitializing = false;
    private string? _snapshotJson;
    private bool _ignoreNextNavigating = false;
    private bool _skipSnapshotOnce = false; // не обновлять снапшот при следующем OnAppearing

    public ProfileDetailPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;

        // подписка на выбор локации
        MessagingCenter.Unsubscribe<LocationSearchPage, (string, AppLocation)>(this, "LocationSelected");
        MessagingCenter.Subscribe<LocationSearchPage, (string, AppLocation)>(
            this, "LocationSelected", OnLocationSelected);

        // перехват системной стрелки "Назад"
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(async () => await HandleBackAsync())
        });

        // отслеживаем изменения полей
        entryProfileName.TextChanged += (_, _) => MarkChanged();
        entryPersonName.TextChanged += (_, _) => MarkChanged();
        entryPersonSurname.TextChanged += (_, _) => MarkChanged();
        entryMessage.TextChanged += (_, _) => MarkChanged();
        dateOfBirthDate.DateSelected += (_, _) => MarkChanged();
        timeOfBirthTime.PropertyChanged += (_, _) => MarkChanged();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.Current.Navigating += OnShellNavigating;
        _isInitializing = true;

        // если есть кэш — используем его и просим не обновлять снапшот один раз
        if (_tempProfile != null)
        {
            _profile = _tempProfile;
            _skipSnapshotOnce = true;
        }
        else if (ProfileId > 0)
        {
            _profile = _database.GetProfileById(ProfileId);

            if (_profile?.PlaceOfBirthId is int pbId)
            {
                var loc = _database.GetLocationById(pbId);
                _birthLocation = loc;
                _profile.PlaceOfBirthLocality = loc?.Locality ?? "";
            }

            if (_profile?.PlaceOfLivingId is int plId)
            {
                var loc = _database.GetLocationById(plId);
                _livingLocation = loc;
                _profile.PlaceOfLivingLocality = loc?.Locality ?? "";
            }
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

        // установить текущие значения в контролы
        RefreshLocationLabels();
        dateOfBirthDate.Date = _profile!.DateOfBirth.Date;
        timeOfBirthTime.Time = _profile!.DateOfBirth.TimeOfDay;

        ApplyLocalization();

        if (!_isEditing)
            SetEditMode(ProfileId == 0);

        _isInitializing = false;
        _hasChanges = false;

        // снапшот создаём только если НЕ просили пропустить
        if (!_skipSnapshotOnce)
            _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);
        _skipSnapshotOnce = false; // сбросить на будущее
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        try { Shell.Current.Navigating -= OnShellNavigating; } catch { }
    }

    private void ApplyLocalization()
    {
        string langCode = DataCache.Instance.CurrentLanguageCode;
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

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (_ignoreNextNavigating)
        {
            _ignoreNextNavigating = false;
            return;
        }

        // не перехватываем переход на страницу поиска локаций
        if (e.Target?.Location.OriginalString?.Contains(nameof(LocationSearchPage)) == true)
            return;

        // при выходе на другие страницы перехватим и спросим про сохранение
        if (HasRealChanges() && e.CanCancel)
        {
            e.Cancel();
            await HandleBackAsync();
        }
    }

    private bool HasRealChanges()
    {
        if (_profile == null || _snapshotJson == null) return false;
        try
        {
            var current = System.Text.Json.JsonSerializer.Serialize(_profile);
            return current != _snapshotJson;
        }
        catch
        {
            return _hasChanges;
        }
    }

    private async Task HandleBackAsync()
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        if (!HasRealChanges())
        {
            _tempProfile = null;
            await GoBackAsync();
            return;
        }

        bool save = await DisplayAlert(
            Localization.GetLocalizedText("Save changes", lang),
            Localization.GetLocalizedText("Do you want to save changes before exit?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang)
        );

        if (save)
        {
            bool saved = await SaveProfileAsync();
            if (!saved) return; // валидация не прошла — остаёмся
        }
        else
        {
            // дискард
            _tempProfile = null;
            _hasChanges = false;
            _isEditing = false;
        }

        await GoBackAsync();
    }

    private async Task GoBackAsync()
    {
        _ignoreNextNavigating = true;
        await Shell.Current.GoToAsync("//profiles", true);
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        // всегда одна и та же логика закрытия
        await HandleBackAsync();
    }

    private async Task<bool> SaveProfileAsync()
    {
        if (_profile == null) return false;

        string lang = DataCache.Instance.CurrentLanguageCode;

        // синхронизуем UI -> модель
        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;

        // проверки
        if (string.IsNullOrWhiteSpace(entryProfileName.Text))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Profile name is required.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return false;
        }

        // требуем явного выбора даты (а не «сегодня по умолчанию»)
        if (_profile.DateOfBirth.Date == DateTime.Now.Date)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Date of birth is required.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return false;
        }

        if (_birthLocation == null)
        //if (_profile.PlaceOfBirthId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfBirthLocality))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of birth is required.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return false;
        }

        if (_livingLocation == null)
        //f (_profile.PlaceOfLivingId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfLivingLocality))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of living is required.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return false;
        }

        try
        {
            // Сохраняем место рождения, если оно новое
            if (_birthLocation.Id == 0)
            {
                var newId = _database.GetOrCreateLocation(_birthLocation);
                if (newId > 0)
                    _birthLocation.Id = newId;

                DataCache.Instance.ReloadLocations(_database);
            }

            // Сохраняем место проживания, если оно новое
            if (_livingLocation.Id == 0)
            {
                var newId = _database.GetOrCreateLocation(_livingLocation);
                if (newId > 0)
                    _livingLocation.Id = newId;

                DataCache.Instance.ReloadLocations(_database);
            }

            // Присваиваем Id-шники в профиль
            _profile.PlaceOfBirthId = _birthLocation.Id;
            _profile.PlaceOfBirthLocality = _birthLocation.Locality;

            _profile.PlaceOfLivingId = _livingLocation.Id;
            _profile.PlaceOfLivingLocality = _livingLocation.Locality;

            if (_profile.Id == 0)
                _database.AddProfile(_profile);
            else
                _database.UpdateProfile(_profile);

            _hasChanges = false;
            _isEditing = false;
            SetEditMode(false);

            // обновляем снимок — теперь страница чистая
            _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);
            _tempProfile = null;

            await DisplayAlert(
                Localization.GetLocalizedText("Saved", lang),
                Localization.GetLocalizedText("Profile saved successfully.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return true;
        }
        catch (Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine($"[PADMA] Save profile error: {ex.Message}");
            await DisplayAlert(
                Localization.GetLocalizedText("Error", lang),
                Localization.GetLocalizedText("Failed to save profile. Please try again.", lang), 
                Localization.GetLocalizedText("OK", lang)
            );
            return false;
        }
    }

    private void OnLocationSelected(LocationSearchPage sender, (string Mode, AppLocation Loc) payload)
    {
        var (mode, loc) = payload;
        if (_profile == null) return;

        if (mode.Equals("birth", StringComparison.OrdinalIgnoreCase))
        {
            _birthLocation = loc;
            _profile.PlaceOfBirthId = loc.Id;
            _profile.PlaceOfBirthLocality = loc.Locality;
        }
        else if (mode.Equals("living", StringComparison.OrdinalIgnoreCase))
        {
            _livingLocation = loc;
            _profile.PlaceOfLivingId = loc.Id;
            _profile.PlaceOfLivingLocality = loc.Locality;
        }

        _tempProfile = _profile;   // держим кэш
        _hasChanges = true;

        MainThread.BeginInvokeOnMainThread(RefreshLocationLabels);
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        if (_profile == null) return;

        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;
        _tempProfile = _profile;

        _skipSnapshotOnce = true;  // вернёмся — не перезаписывать снапшот
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?Mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        if (_profile == null) return;

        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;
        _tempProfile = _profile;

        _skipSnapshotOnce = true;  // вернёмся — не перезаписывать снапшот
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?Mode=living", true);
    }

    private void RefreshLocationLabels()
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        lblPlaceOfBirthValue.Text = string.IsNullOrWhiteSpace(_profile?.PlaceOfBirthLocality)
            ? Localization.GetLocalizedText("Select location...", lang)
            : _profile!.PlaceOfBirthLocality;

        lblPlaceOfLivingValue.Text = string.IsNullOrWhiteSpace(_profile?.PlaceOfLivingLocality)
            ? Localization.GetLocalizedText("Select location...", lang)
            : _profile!.PlaceOfLivingLocality;
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
        btnPlaceOfBirth.IsEnabled = isEdit;
        btnPlaceOfLiving.IsEnabled = isEdit;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Save", lang),
            Localization.GetLocalizedText("Save changes to profile?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang)
        );

        if (!confirm) return;

        bool result = await SaveProfileAsync();
        if (result)
            await GoBackAsync();
    }

    private void OnEditClicked(object sender, EventArgs e) => SetEditMode(true);

    private async void OnSetDefaultClicked(object sender, EventArgs e)
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Default profile", lang),
                Localization.GetLocalizedText("Save profile first.", lang),
                Localization.GetLocalizedText("OK", lang)
            );
            return;
        }

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Default profile", lang),
            Localization.GetLocalizedText("Set this profile as default?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang)
        );

        if (!confirm) return;

        _database.SetDefaultProfile(_profile.Id);
        await DisplayAlert(
            Localization.GetLocalizedText("Done", lang),
            Localization.GetLocalizedText("Profile marked as default.", lang),
            Localization.GetLocalizedText("OK", lang)
        );
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Delete", lang),
                Localization.GetLocalizedText("Nothing to delete.", lang),
                Localization.GetLocalizedText("OK", lang)
            );
            return;
        }

        bool confirm = await DisplayAlert(
            Localization.GetLocalizedText("Delete", lang),
            Localization.GetLocalizedText("Delete this profile?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang)
        );

        if (!confirm) return;

        _database.DeleteProfile(_profile.Id);

        await DisplayAlert(
            Localization.GetLocalizedText("Deleted", lang),
            Localization.GetLocalizedText("Profile deleted.", lang),
            Localization.GetLocalizedText("OK", lang)
        );

        await GoBackAsync();
    }
}
