using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using PADMA.Core.Enums;
using PADMA.Core.Services;
using PADMA.Core.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PADMA.Pages;

public partial class ColorSettingsPage : UI.Templates.ConfigBasePage
{
    private readonly DatabaseService _db;
    private readonly Dictionary<int, int> _originalColors = new();
    private readonly Dictionary<int, int> _currentColors = new();
    private readonly ObservableCollection<ColorItem> _items = new();
    private ColorItem? _selectedItem;
    private bool _isSavingOrDiscarding;

    public ColorSettingsPage(DatabaseService db)
    {
        InitializeComponent();
        BindingContext = this;
        IsCompact = true;
        _db = db;

        ApplyLocalization();
        LoadColors();
        SelectFirst();
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Color settings", lang);
        ApplySystemButton.Text = Localization.GetLocalizedText("System default", lang);
        ChangeButton.Text = Localization.GetLocalizedText("Change", lang);
    }

    private void LoadColors()
    {
        _items.Clear();
        _originalColors.Clear();
        _currentColors.Clear();

        var lang = DataCache.Instance.CurrentLanguageCode;

        // предпочтительно как в других конфиг-страницах: читаем из DB
        var colors = _db.GetColors().OrderBy(c => c.Id).ToList();

        // desc из кеша (уже загружено)
        var descs = DataCache.Instance.ColorDescList;

        foreach (var c in colors)
        {
            var name = descs.FirstOrDefault(d => d.ColorId == c.Id && d.LanguageCode == lang)?.Name
                       ?? c.Code;

            _originalColors[c.Id] = c.ArgbValue;
            _currentColors[c.Id] = c.ArgbValue;

            _items.Add(new ColorItem(c.Id, name, c.ArgbValue));
        }

        ColorsList.ItemsSource = _items;
    }

    private void SelectFirst()
    {
        if (_items.Count == 0) return;
        ColorsList.SelectedItem = _items[0];
        _selectedItem = _items[0];
    }

    private void OnSelectionChanged(object? sender, Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        _selectedItem = e.CurrentSelection?.FirstOrDefault() as ColorItem;
        ChangeButton.IsEnabled = _selectedItem != null;
    }

    private void OnColorSelected(ColorLookupPage sender, int argb)
    {
        if (_selectedItem == null) return;

        _currentColors[_selectedItem.Id] = argb;
        var isDirty = _originalColors.TryGetValue(_selectedItem.Id, out var orig) && orig != argb;
        _selectedItem.SetArgb(argb, isDirty);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MessagingCenter.Subscribe < ColorLookupPage, int> (this, "ColorSelected", OnColorSelected);
    }

    protected override async void OnDisappearing()
    {
        MessagingCenter.Unsubscribe<ColorLookupPage, int>(this, "ColorSelected");
        base.OnDisappearing();

        if (_isSavingOrDiscarding) return;
        if (!IsDirty()) return;

        _isSavingOrDiscarding = true;

        try
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            var save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Do you want to save changes?", lang),
                Localization.GetLocalizedText("Yes", lang),
                Localization.GetLocalizedText("No", lang));

            if (save)
            {
                SaveToDb();
                DataCache.Instance.Refresh(_db); // ты уже добавил GetColors() сюда
                MessagingCenter.Send<object>(this, "SettingsChanged");
            }
            else
            {
                // discard: восстановим current из original и UI
                foreach (var item in _items)
                {
                    var orig = _originalColors[item.Id];
                    _currentColors[item.Id] = orig;
                    item.SetArgb(orig, isDirty: false);
                }
            }
        }
        finally
        {
            _isSavingOrDiscarding = false;
        }
    }

    private async void OnChangeClicked(object sender, EventArgs e)
    {
        if (_selectedItem == null) return;

        var startArgb = _selectedItem.Argb; 
        await Shell.Current.GoToAsync($"{nameof(ColorLookupPage)}?StartArgb={startArgb}", true);
    }

    
    

    private async void OnApplySystemClicked(object sender, EventArgs e)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        var ok = await DisplayAlert(
            Localization.GetLocalizedText("Confirm", lang),
            Localization.GetLocalizedText("Apply system default colors?", lang),
            Localization.GetLocalizedText("Yes", lang),
            Localization.GetLocalizedText("No", lang));

        if (!ok) return;

        foreach (var item in _items)
        {
            if (!SystemDefaultPalette.TryGetValue(item.Id, out var defArgb))
                continue;

            _currentColors[item.Id] = defArgb;
            item.SetArgb(defArgb, isDirty: defArgb != _originalColors[item.Id]);
        }

    }

    private bool IsDirty()
    {
        foreach (var kv in _currentColors)
        {
            if (_originalColors.TryGetValue(kv.Key, out var orig) && orig != kv.Value)
                return true;
        }
        return false;
    }

    private void SaveToDb()
    {
        foreach (var kv in _currentColors)
        {
            var id = kv.Key;
            var cur = kv.Value;

            if (_originalColors.TryGetValue(id, out var orig) && orig == cur)
                continue;

            _db.UpdateColorArgb(id, cur);
            _originalColors[id] = cur;
        }

        // сброс dirty-флажков
        foreach (var item in _items)
        {
            item.SetArgb(_currentColors[item.Id], isDirty: false);
        }
    }

    // ===== System default palette (ARGB int) =====
    private static readonly Dictionary<int, int> SystemDefaultPalette = new()
    {
        // примерные значения — подставь ваши реальные system defaults
        { (int)EColor.GREEN, unchecked((int)0xFF2E7D32) },
        { (int)EColor.RED, unchecked((int)0xFFC62828) },
        { (int)EColor.LIGHTGREEN, unchecked((int)0xFF81C784) },
        { (int)EColor.LIGHTRED, unchecked((int)0xFFEF9A9A) },
        { (int)EColor.PINK, unchecked((int)0xFFF48FB1) },

        { (int)EColor.GRAY, unchecked((int)0xFF9E9E9E) },
        { (int)EColor.BLACK, unchecked((int)0xFF000000) },
    };
}

// простой item как у тебя в других конфиг страницах, без MVVM
public sealed class ColorItem : INotifyPropertyChanged
{
    public int Id { get; }
    public string Name { get; }

    private int _argb;
    public int Argb
    {
        get => _argb;
        private set { _argb = value; OnPropertyChanged(); OnPropertyChanged(nameof(MauiColor)); }
    }

    public Color MauiColor => CalendarDrawingHelper.ColorFromArgbInt(Argb);

    private bool _isDirty;
    public bool IsDirty
    {
        get => _isDirty;
        private set { _isDirty = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ColorItem(int id, string name, int argb)
    {
        Id = id;
        Name = name;
        _argb = argb;
        _isDirty = false;
    }

    public void SetArgb(int argb, bool isDirty)
    {
        Argb = argb;
        IsDirty = isDirty;
    }

    private void OnPropertyChanged([CallerMemberName] string? n = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    
}