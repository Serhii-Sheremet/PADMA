using PADMA.Core.Analysis;
using PADMA.Core.Models;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using PADMA.Pages;
using System.Text.Json;

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

    private enum ProfileUiMode
    {
        View,   // просмотр существующего
        Edit,   // редактирование существующего
        Create  // создание нового
    }

    private ProfileUiMode _mode;

    public ProfileDetailPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;

        // подписка на выбор локации
        MessagingCenter.Unsubscribe<LocationSearchPage, (string, AppLocation)>(this, "LocationSelected");
        MessagingCenter.Subscribe<LocationSearchPage, (string, AppLocation)>(
            this, "LocationSelected", OnLocationSelected);

        // подписка на тюнинг даты/времени рождения из превью карты
        MessagingCenter.Unsubscribe<BirthChartPreviewPage, DateTime>(this, "BirthDateTimeSelected");
        MessagingCenter.Subscribe<BirthChartPreviewPage, DateTime>(
            this, "BirthDateTimeSelected", OnBirthDateTimeSelected);

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
        dateOfBirthDate.DateSelected += OnDobDateSelected;
        timeOfBirthTime.PropertyChanged += OnDobTimeChanged;
    }

    private void OnDobDateSelected(object? sender, DateChangedEventArgs e)
    {
        if (_isInitializing || _profile == null) return;

        // сохраняем время, меняем только дату
        var t = timeOfBirthTime.Time;
        _profile.DateOfBirth = e.NewDate.Date + t;

        MarkChanged();
    }

    private void OnDobTimeChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != TimePicker.TimeProperty.PropertyName) return;
        if (_isInitializing || _profile == null) return;

        // сохраняем дату, меняем только время
        var d = dateOfBirthDate.Date;
        var t = timeOfBirthTime.Time;
        _profile.DateOfBirth = d.Date + t;

        MarkChanged();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.Current.Navigating += OnShellNavigating;
        _isInitializing = true;

        // 1) Получить/создать профиль
        if (_tempProfile != null)
        {
            // Возврат со страницы поиска локации или после отмены выхода — используем кеш
            _profile = _tempProfile;
            _skipSnapshotOnce = true;
        }
        else if (ProfileId > 0)
        {
            // Открыли существующий профиль
            _profile = _database.GetProfileById(ProfileId);

            // Подтянуть локации из БД (и обновить Locality в профиле, чтобы UI видел)
            if (_profile?.PlaceOfBirthId is int pbId)
            {
                _birthLocation = _database.GetLocationById(pbId);
                _profile.PlaceOfBirthLocality = _birthLocation?.Locality ?? "";
            }
            else
            {
                _birthLocation = null;
                if (_profile != null) _profile.PlaceOfBirthLocality = "";
            }

            if (_profile?.PlaceOfLivingId is int plId)
            {
                _livingLocation = _database.GetLocationById(plId);
                _profile.PlaceOfLivingLocality = _livingLocation?.Locality ?? "";
            }
            else
            {
                _livingLocation = null;
                if (_profile != null) _profile.PlaceOfLivingLocality = "";
            }
        }
        else
        {
            // Новый профиль
            _profile = new Profile
            {
                DateOfBirth = DateTime.Now,
                ProfileName = "",
                PersonName = "",
                PersonSurname = "",
                Message = ""
            };

            _birthLocation = null;
            _livingLocation = null;
        }

        if (_profile == null)
        {
            // На всякий случай: если БД вернула null — создаём новый
            _profile = new Profile
            {
                DateOfBirth = DateTime.Now,
                ProfileName = "",
                PersonName = "",
                PersonSurname = "",
                Message = ""
            };
            _birthLocation = null;
            _livingLocation = null;
        }

        // 2) BindingContext (после того, как _profile окончательно определён)
        BindingContext = _profile;

        // 3) Обновить контролы из модели (важно: чтобы DatePicker/TimePicker не ломали время)
        RefreshLocationLabels();
        dateOfBirthDate.Date = _profile.DateOfBirth.Date;
        timeOfBirthTime.Time = _profile.DateOfBirth.TimeOfDay;

        // 4) Локализация (зависит от языка, не от режима)
        ApplyLocalization();

        // 5) Режим: новый профиль -> Create, существующий -> View, если уже редактируем -> Edit
        if (_isEditing)
        {
            ApplyMode(_mode == ProfileUiMode.Create ? ProfileUiMode.Create : ProfileUiMode.Edit);
        }
        else
        {
            ApplyMode(_profile.Id == 0 ? ProfileUiMode.Create : ProfileUiMode.View);
        }

        // 6) Флаги изменений
        _isInitializing = false;
        _hasChanges = false;

        // 7) Snapshot (только если не просили пропустить один раз)
        if (!_skipSnapshotOnce)
            _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);

        _skipSnapshotOnce = false;
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
        btnBirthChartPreview.Text = Localization.GetLocalizedText("Birth Chart", langCode);
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

        // do not intercept internal lookup-style navigation
        if (e.Target?.Location.OriginalString?.Contains(nameof(LocationSearchPage)) == true ||
            e.Target?.Location.OriginalString?.Contains(nameof(BirthChartPreviewPage)) == true)
            return;

        // when leaving to other pages, intercept and ask about saving
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

        Profile? before = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(_snapshotJson))
                before = System.Text.Json.JsonSerializer.Deserialize<Profile>(_snapshotJson);
        }
        catch
        {
            // если что-то пошло не так — просто считаем, что before неизвестен
        }

        // синхронизуем UI -> модель
        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;

        // проверки
        if (string.IsNullOrWhiteSpace(entryProfileName.Text))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Profile name is required.", lang), 
                "OK"
            );
            return false;
        }

        // требуем явного выбора даты (а не «сегодня по умолчанию»)
        if (_profile.DateOfBirth.Date == DateTime.Now.Date)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Date of birth is required.", lang), 
                "OK"
            );
            return false;
        }

        if (_birthLocation == null)
        //if (_profile.PlaceOfBirthId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfBirthLocality))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of birth is required.", lang), 
                "OK"
            );
            return false;
        }

        if (_livingLocation == null)
        //f (_profile.PlaceOfLivingId <= 0 || string.IsNullOrWhiteSpace(_profile.PlaceOfLivingLocality))
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Validation", lang),
                Localization.GetLocalizedText("Place of living is required.", lang), 
                "OK"
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

            bool isActiveProfile = DataCache.Instance.ActiveProfile?.Id == _profile.Id && _profile.Id > 0;

            bool calcRelevantChanged = false;
            if (before != null)
            {
                calcRelevantChanged =
                    before.DateOfBirth != _profile.DateOfBirth ||
                    before.PlaceOfBirthId != _profile.PlaceOfBirthId ||
                    before.PlaceOfLivingId != _profile.PlaceOfLivingId;
            }

            bool saveAsNew = false;
            // if editing an existing profile and key calculation-related fields changed,
            // let the user choose whether to update current profile or save as new
            if (_mode == ProfileUiMode.Edit && calcRelevantChanged)
            {
                saveAsNew = await DisplayAlert(
                    Localization.GetLocalizedText("Saving profile", lang), // title
                    Localization.GetLocalizedText("Update profile or save as new?", lang), // body
                    Localization.GetLocalizedText("Save as new", lang), // accept (справа)
                    Localization.GetLocalizedText("Update", lang)       // cancel (слева)
                );

                if (saveAsNew)
                {
                    string originalName = before?.ProfileName?.Trim() ?? string.Empty;
                    string initialName = _profile.ProfileName;

                    while (true)
                    {
                        string? newName = await DisplayPromptAsync(
                            Localization.GetLocalizedText("Profile name", lang),
                            Localization.GetLocalizedText("Enter new profile name", lang),
                            accept: Localization.GetLocalizedText("OK", lang),
                            cancel: Localization.GetLocalizedText("Cancel", lang),
                            initialValue: initialName
                        );

                        // user explicitly cancelled save-as-new flow
                        if (newName == null)
                            return false;

                        newName = newName.Trim();
                        initialName = newName; // keep last entered value for the next loop

                        if (string.IsNullOrWhiteSpace(newName))
                        {
                            await DisplayAlert(
                                Localization.GetLocalizedText("Validation", lang),
                                Localization.GetLocalizedText("Profile name is required.", lang),
                                Localization.GetLocalizedText("OK", lang)
                            );
                            continue;
                        }

                        if (string.Equals(newName, originalName, StringComparison.OrdinalIgnoreCase))
                        {
                            await DisplayAlert(
                                Localization.GetLocalizedText("Validation", lang),
                                Localization.GetLocalizedText("New profile name must be different from the current one.", lang),
                                Localization.GetLocalizedText("OK", lang)
                            );
                            continue;
                        }

                        bool nameAlreadyExists = DataCache.Instance.ProfileList?
                            .Any(p => p.Id != _profile.Id &&
                                      string.Equals(p.ProfileName?.Trim(), newName, StringComparison.OrdinalIgnoreCase)) == true;

                        if (nameAlreadyExists)
                        {
                            await DisplayAlert(
                                Localization.GetLocalizedText("Validation", lang),
                                Localization.GetLocalizedText("A profile with this name already exists.", lang),
                                Localization.GetLocalizedText("OK", lang)
                            );
                            continue;
                        }

                        _profile.ProfileName = newName;
                        entryProfileName.Text = newName;
                        break;
                    }
                }
            }

            // FIX: if editing an existing profile but Id was lost somewhere, restore it
            if (_profile.Id == 0 && _mode != ProfileUiMode.Create && ProfileId > 0 && !saveAsNew)
            {
                _profile.Id = ProfileId;
            }

            if (saveAsNew)
            {
                _profile.Id = 0;
                _profile.Checked = false;
                _database.AddProfile(_profile);
                ProfileId = _profile.Id;
            }
            else if (_mode == ProfileUiMode.Create)
            {
                _database.AddProfile(_profile);
                ProfileId = _profile.Id;
            }
            else
            {
                _database.UpdateProfile(_profile);
            }

            // если редактировали активный профиль и изменились ключевые поля для расчётов — пересобрать контекст и уведомить UI
            if (!saveAsNew && isActiveProfile && calcRelevantChanged)
            {
                // обновим ActiveProfile на свежий объект из БД (чтобы не держать старую ссылку)
                DataCache.Instance.ActiveProfile = _database.GetProfileById(_profile.Id);

                DataCache.Instance.ReloadLocations(_database);
                await DataCache.Instance.ProfileContextService.RebuildAsync();

                MessagingCenter.Send<object>(this, "ProfileChanged");
                SwissAnalysis.ClearZodiacBoundaryCache();
            }

            _hasChanges = false;
            _isEditing = false;
            ApplyMode(ProfileUiMode.View);

            // обновляем снимок — теперь страница чистая
            _snapshotJson = System.Text.Json.JsonSerializer.Serialize(_profile);
            _tempProfile = null;

            await DisplayAlert(
                Localization.GetLocalizedText("Saved", lang),
                Localization.GetLocalizedText("Profile saved successfully.", lang), 
                "OK"
            );
            return true;
        }
        catch (Exception ex)
        {
            //System.Diagnostics.Debug.WriteLine($"[PADMA] Save profile error: {ex.Message}");
            await DisplayAlert(
                Localization.GetLocalizedText("Error", lang),
                Localization.GetLocalizedText("Failed to save profile. Please try again.", lang), 
                "OK"
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

    private void OnBirthDateTimeSelected(BirthChartPreviewPage sender, DateTime newBirthDateTime)
    {
        dateOfBirthDate.Date = newBirthDateTime.Date;
        timeOfBirthTime.Time = newBirthDateTime.TimeOfDay;

        if (_profile != null)
        {
            _profile.DateOfBirth = newBirthDateTime;
            _tempProfile = _profile;
        }
    }

    private async void OnPlaceOfBirthClicked(object sender, EventArgs e)
    {
        if (!_isEditing) return;

        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;
        _tempProfile = _profile;

        _skipSnapshotOnce = true;  // вернёмся — не перезаписывать снапшот
        await Shell.Current.GoToAsync($"{nameof(LocationSearchPage)}?Mode=birth", true);
    }

    private async void OnPlaceOfLivingClicked(object sender, EventArgs e)
    {
        if (!_isEditing) return;

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

    private void ApplyMode(ProfileUiMode mode)
    {
        _mode = mode;

        bool isView = mode == ProfileUiMode.View;
        bool isEdit = mode == ProfileUiMode.Edit;
        bool isCreate = mode == ProfileUiMode.Create;

        // синхронизируем старый флаг с новым режимом
        _isEditing = isEdit || isCreate;

        // Вариант 1: скрывать лишние
        //btnSave.IsVisible = isEdit || isCreate;
        //btnEdit.IsVisible = isView;
        //btnDelete.IsVisible = isView;

        // Вариант 2: кнопки "серенькие" (IsVisible - IsEnabled+Opacity)
        btnSave.IsEnabled = isEdit || isCreate;
        btnSave.Opacity = btnSave.IsEnabled ? 1.0 : 0.35;

        btnEdit.IsEnabled = isView;
        btnEdit.Opacity = btnEdit.IsEnabled ? 1.0 : 0.35;

        btnDelete.IsEnabled = isView;
        btnDelete.Opacity = btnDelete.IsEnabled ? 1.0 : 0.35;

        btnBirthChartPreview.IsEnabled = isEdit || isCreate;
        btnBirthChartPreview.Opacity = btnBirthChartPreview.IsEnabled ? 1.0 : 0.35;

        SetFieldsEditable(isEdit || isCreate);
    }

    private void SetFieldsEditable(bool editable)
    {
        entryProfileName.IsEnabled = editable;
        entryPersonName.IsEnabled = editable;
        entryPersonSurname.IsEnabled = editable;
        entryMessage.IsEnabled = editable;
        dateOfBirthDate.IsEnabled = editable;
        timeOfBirthTime.IsEnabled = editable;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_profile == null)
            return;

        // если изменений нет — просто выйти из режима редактирования
        if (!HasRealChanges())
        {
            ApplyMode(ProfileUiMode.View);
            return;
        }

        bool result = await SaveProfileAsync();
        if (!result)
            return;

        ApplyMode(ProfileUiMode.View);
        _snapshotJson = JsonSerializer.Serialize(_profile);
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        ApplyMode(ProfileUiMode.Edit);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        string lang = DataCache.Instance.CurrentLanguageCode;

        if (_profile == null || _profile.Id <= 0)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Delete", lang),
                Localization.GetLocalizedText("Nothing to delete.", lang),
                "OK"
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
            "OK"
        );

        await GoBackAsync();
    }

    private async void OnBirthChartPreviewClicked(object sender, EventArgs e)
    {
        if (!_isEditing)
            return;

        if (_profile == null)
            return;

        var lang = DataCache.Instance.CurrentLanguageCode;  
        // Save current draft state before navigation
        _profile.DateOfBirth = dateOfBirthDate.Date + timeOfBirthTime.Time;
        _tempProfile = _profile;
        _skipSnapshotOnce = true;

        if (_birthLocation == null)
        {
            await DisplayAlert(
                Localization.GetLocalizedText("Warning", lang),
                Localization.GetLocalizedText("Please select place of birth first.", lang),
                "OK");
            return;
        }

        await Shell.Current.GoToAsync(
                nameof(BirthChartPreviewPage),
                true,
                new Dictionary<string, object>
                {
                    ["BirthDateTime"] = _profile.DateOfBirth,
                    ["BirthLocation"] = _birthLocation
                });
    }


}
