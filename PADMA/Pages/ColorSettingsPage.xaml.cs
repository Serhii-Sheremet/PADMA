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
    private bool _navigatingToLookup;

    public ColorSettingsPage(DatabaseService db)
    {
        InitializeComponent();
        BindingContext = this;
        IsCompact = true;
        _db = db;

        ApplyLocalization();
        LoadColors();
        SelectFirst();

        MessagingCenter.Subscribe<ColorLookupPage, int>(
            this,
            "ColorSelected",
            (sender, argb) => ApplySelectedColor(argb));

    }

    private void ApplySelectedColor(int argb)
    {
        if (_selectedItem == null || _currentColors.TryGetValue(_selectedItem.Id, out var cur) && cur == argb)
            return;

        // dirty если оригинал отсутствует или отличается
        var isDirty = !_originalColors.TryGetValue(_selectedItem.Id, out var orig) || orig != argb;

        // обновляем current dictionary
        _currentColors[_selectedItem.Id] = argb;

        // обновляем item (один раз)
        _selectedItem.SetArgb(argb, isDirty);
    }

    private void ApplyLocalization()
    {
        var lang = DataCache.Instance.CurrentLanguageCode;
        Title = Localization.GetLocalizedText("Color settings", lang);
        ApplySystemButton.Text = Localization.GetLocalizedText("Default color", lang);
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

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selectedItem = e.CurrentSelection?.FirstOrDefault() as ColorItem;
        ChangeButton.IsEnabled = _selectedItem != null;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _navigatingToLookup = false;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        if (_navigatingToLookup) return;
        if (_isSavingOrDiscarding) return;
        if (!IsDirty()) return;

        _isSavingOrDiscarding = true;

        try
        {
            var lang = DataCache.Instance.CurrentLanguageCode;

            var save = await DisplayAlert(
                Localization.GetLocalizedText("Save changes?", lang),
                Localization.GetLocalizedText("Apply new settings for colors?", lang),
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

        _navigatingToLookup = true;
        var startArgb = _selectedItem.Argb; 
        await Shell.Current.GoToAsync($"{nameof(ColorLookupPage)}?StartArgb={startArgb}", true);
    }

    private async void OnApplySystemClicked(object sender, EventArgs e)
    {
        var lang = DataCache.Instance.CurrentLanguageCode;

        var ok = await DisplayAlert(
            Localization.GetLocalizedText("Confirmation", lang),
            Localization.GetLocalizedText("Reset to default colors?", lang),
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
        {(int)EColor.GREEN,             -13631697 },
        {(int)EColor.RED,               -45233 },
        {(int)EColor.LIGHTGREEN,        -4587591 },
        {(int)EColor.LIGHTRED,          -14650 },
        {(int)EColor.PINK,              -16181 },
        {(int)EColor.NOTEMARKER,        -14774017 },
        {(int)EColor.YOGAMERGE,         -3211314 },
        {(int)EColor.MUHURTAMERGE,      -16181 },
        {(int)EColor.MRITYUBHAGA,       -1064908 },
        {(int)EColor.SUN,               -4587591 },
        {(int)EColor.VENUS,             -4587591 },
        {(int)EColor.MERCURY,           -4587591 },
        {(int)EColor.MOON,              -4587591 },
        {(int)EColor.SATURN,            -14650 },
        {(int)EColor.JUPITER,           -4587591 },
        {(int)EColor.MARS,              -14650 },
        {(int)EColor.MASA1,             -4587591 },
        {(int)EColor.MASA2,             -4587591 },
        {(int)EColor.MASA3,             -4587591 },
        {(int)EColor.MASA4,             -4587591 },
        {(int)EColor.MASA5,             -4587591 },
        {(int)EColor.MASA6,             -4587591 },
        {(int)EColor.MASA7,             -4587591 },
        {(int)EColor.MASA8,             -4587591 },
        {(int)EColor.MASA9,             -4587591 },
        {(int)EColor.MASA10,            -4587591 },
        {(int)EColor.MASA11,            -4587591 },
        {(int)EColor.MASA12,            -4587591 },
        {(int)EColor.SHUNYANAKSHATRA,   -16181 },
        {(int)EColor.SHUNIATITHI,       -16181 },
        {(int)EColor.GRAY,              -4144960 },
        {(int)EColor.BLACK,             -16777216 }
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