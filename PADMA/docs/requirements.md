# 🪶 PADMA — Requirements & Architecture Overview

> *Version: October 2025*
> *This document defines the official project requirements and describes the implemented architecture of the PADMA .NET MAUI application.*

---

## **Part I — Official Functional & UI Requirements**

### 🗓 Overview

**PADMA** is a cross-platform .NET MAUI application focused on calendar visualization, astrological calculations, and configurable user preferences.
All data (settings, UI texts, reference tables, profiles) are stored in a **SQLite** database `PADMADB.db3` and cached in memory for fast access.

---

### 🧭 Navigation Structure

| Section     | Route      | Description             |
| ----------- | ---------- | ----------------------- |
| Calendar    | `//main`   | Main calendar interface |
| Settings    | `//config` | Configuration hub       |
| Day details | `//day`    | Detailed day view       |
| Exit        | `//exit`   | Closes the app          |

The burger (fly-out) menu provides quick access to **Calendar**, **Settings**, and **Exit**.
When a settings page is opened, the fly-out automatically closes.

---

### 🏠 **MainPage** (Calendar)

**Purpose:**
The central user interface showing a monthly grid calendar.

#### Functional Requirements

1. Display a **6 × 7 calendar grid** (42 days) representing the selected month.
2. Header elements:

   * Toolbar title → month + year, localized and capitalized.
   * Navigation arrows (`left_arrow.png`, `right_arrow.png`) to move months.
3. Above the day grid, display a **weekday header row**:

   * 7 cells (`MON TUE … SUN`), localized to current culture.
   * Each cell has a thin border (0.5 px) to form a seamless grid with the days below.
   * Font size = 13 pt, bold, centered.
4. Each day cell shows:

   * Day number in top-left corner.
   * 6 colored bars as placeholders for future transit visualization.
   * Highlight for today (light blue background).
5. Calendar respects:

   * Active culture (`DataCache.Instance.CurrentLanguageCode`).
   * First day of week from `APPSETTING` (`WEEKMONDAY` / `WEEKSUNDAY`).
6. On month navigation or configuration change:

   * Calendar and header are rebuilt.
   * Title and weekday names are re-localized.
7. On day tap:

   * Navigate to `DayPage` using
     `await Shell.Current.GoToAsync($"day?Date={selected.Date:yyyy-MM-dd}");`
8. On app resume or return from settings (`OnAppearing`):

   * Refresh DataCache and rebuild the calendar (ensures latest language and settings).

#### Visual Style

| Element           | Style                                                   |
| ----------------- | ------------------------------------------------------- |
| Toolbar title     | Font 22 pt Bold, center-aligned, localized month + year |
| Weekday header    | Font 13 pt Bold, black text, thin grid borders          |
| Day number        | Bold, small margin, dark text                           |
| Grid lines        | Stroke 0.5 px #000                                      |
| Background        | White                                                   |
| Today highlight   | Light blue background                                   |
| Responsive layout | Columns distributed evenly across device width          |

#### Behavioral Notes

* All localization uses culture from **DataCache** → **LanguagePage** → **DatabaseService**.
* Calendar automatically rebuilds on:

  * Language change
  * First-day-of-week change
  * Month navigation
* Implemented for Android / iOS / Windows platforms.

---

### 🌅 **DayPage**

Placeholder for future astrological details.

| Feature      | Description                                           |
| ------------ | ----------------------------------------------------- |
| Navigation   | Opened by selecting a day on MainPage                 |
| Content      | Detailed daily data (currently placeholder)           |
| Localization | All texts localized                                   |
| Return       | Back navigation returns to the same month on MainPage |

---

### 🧩 UI Templates & Layout Standards

PADMA UI follows a unified design system to ensure visual consistency, reusability, and straightforward localization across all pages.

---

### 🔹 ConfigBasePage

**Purpose:**  
Provides a common visual template for all configuration pages.

**Features:**
- Inherits from `ContentPage`.
- Defines standard padding, margins, and font styles.
- Includes toolbar with close icon (`close_icon.png`).
- Title automatically localized via `Localization.GetLocalizedText()`.
- Shared styles loaded from `Resources/Styles`.

**Common Elements:**
| Element | Style | Description |
|----------|--------|-------------|
| Page title | `PageTitleStyle` | Bold, centered, localized |
| Instruction text | `InstructionLabelStyle` | Regular weight, medium size |
| Option labels | `LabelTextStyle` | Used for radio button captions |
| Radio buttons | Grouped logically per setting |

All configuration pages (Language, FirstDayOfWeek, Transits, Nodes, etc.) inherit from this template.

---

### 🔹 ConfigurationPage

**Purpose:**  
Acts as a hub connecting all configuration pages.

**Layout:**
- Inherits from `ContentPage`.
- Contains a localized title: `"Settings"`.
- Displays a vertical list of buttons for navigating to each config page.
- Uses localized button labels (via `Localization.GetLocalizedText()`).
- Toolbar contains the close icon for returning to MainPage.

**Behavior:**
- Subscribes to `MessagingCenter` messages from child pages (`"SettingsChanged"`).
- When receiving changes, refreshes cache and updates localized texts.
- If no changes occurred — simply closes without refresh.
- Fully localized, including confirmation dialogs and toolbar texts.

---

### 🔹 MainPage

**Purpose:**  
Primary calendar view of the application.

**Layout:**
- Toolbar: localized month title + navigation buttons (using `left_arrow.png`, `right_arrow.png`).
- Header row: localized weekday abbreviations (Mon/Tue/…).
- Main grid: 6 rows × 7 columns with bordered day cells.
- Each day cell displays:
  - Date number in top-left corner.
  - 6 color bars below (transit placeholders).
- Adaptive sizing — fits all screen sizes equally.

**Behavior:**
- Loads active language and first-day-of-week settings from cache.
- Reacts to `"SettingsChanged"` messages (from any configuration page).
- Rebuilds layout when:
  - Language changes,
  - Week start changes,
  - Month navigation occurs.
- Title capitalization handled automatically via `ToTitleCase()` for current culture.

---

### 🔹 Common Visual Standards

| Element | Property | Value |
|----------|-----------|-------|
| Font | Default | *OpenSans* |
| Font size | Page titles | 22sp |
| Font size | Labels | 14–16sp |
| Colors | Primary background | `#FFFFFF` |
| Colors | Text color | `#333333` |
| Colors | Grid borders | `#CCCCCC` |
| Accent | Current day highlight | Light blue |
| Layout spacing | Default padding | 16px |
| Layout spacing | Default spacing | 12px |

---

### 🔹 Localization Consistency

All visible UI elements (titles, labels, button texts, and dialogs) must use:
```csharp
Localization.GetLocalizedText("NativeText", DataCache.Instance.CurrentLanguageCode);```


### ⚙️ **ConfigurationPage** (Settings Hub)

**Purpose:**
Acts as the central entry point for all configuration sections.

| Property | Description                                  |
| -------- | -------------------------------------------- |
| Route    | `//config`                                   |
| Access   | From Settings in the burger menu             |
| Layout   | Inherits from `ConfigBasePage`               |
| Behavior | Automatically closes burger menu when opened |

Contains navigation buttons to sub-pages:

* **LanguagePage**
* **FirstDayOfWeekPage**
* **TransitsPage** 
* **NodesPage** 
* **HoraPage** 
* **MuhurtaPage** 
* **MrityuBhagaPage** 
* **SunrisePage**

All configuration pages send a unified **`MessagingCenter.Send(this, "SettingsChanged")`** message after saving settings.
MainPage listens once to this message and updates itself (title, culture, header grid).
Additionally, **OnAppearing()** ensures the calendar refreshes even if the event is missed.

### ⚙️ Event Handling and Localization Refresh

The ConfigurationPage acts as a hub for all configuration sub-pages (Language, FirstDayOfWeek, Transits, etc.)
and handles configuration change notifications through a unified messaging system.

###🔸 Unified Messaging Pattern

* Each configuration sub-page, after applying and saving its settings, sends a single global message:

MessagingCenter.Send<object>(this, "SettingsChanged");

This message type (object) ensures compatibility across all pages.

*The ConfigurationPage subscribes once and handles all such notifications:

MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ =>
{
    ApplyLocalization();
    await ShowSettingsUpdatedMessage();
});

###🔸 Refresh Behavior

* ApplyLocalization() updates the page title and all button texts using

Localization.GetLocalizedText(nativeText, DataCache.Instance.CurrentLanguageCode);

* ShowSettingsUpdatedMessage() displays a localized confirmation alert:

await DisplayAlert(
    Localization.GetLocalizedText("Configuration Updated", langCode),
    Localization.GetLocalizedText("Settings have been successfully applied.", langCode),
    Localization.GetLocalizedText("OK", langCode)
);

###🔸 OnAppear Synchronization

To ensure consistency even when returning via navigation gestures,
ApplyLocalization() is also invoked in OnAppearing():

protected override void OnAppearing()
{
    base.OnAppearing();
    Shell.Current.FlyoutIsPresented = false;
    ApplyLocalization();
}

### ✅ Benefits

* One single message for all configuration pages — minimal code duplication.
* Instant localization refresh on language change.
* Scalable design — new settings pages integrate automatically.
* Predictable and fully localized user feedback.

---

### 🔸 Conditional Messaging Behavior (Hub + Sub-pages)

The configuration system in **PADMA** uses an event-driven model to refresh data and UI only when **real configuration changes** occur.  
Both the **ConfigurationPage** (hub) and its sub-pages (`LanguagePage`, `FirstDayOfWeekPage`, etc.) must follow this conditional update rule.

###🔸 Unified Save Behavior

All configuration subpages (LanguagePage, FirstDayOfWeekPage, TransitsPage, etc.)
follow a delayed save model:

* The user may change radio buttons or options freely.
* No confirmation dialog or database update occurs immediately.
* When the user closes the page (via back arrow or close icon): If settings were changed (_currentSettingCode != _originalSettingCode), a confirmation dialog “Save changes?” is displayed.
* If confirmed → new settings are saved, cache refreshed, and "SettingsChanged" is sent to ConfigurationPage.
* If declined or no changes → page simply closes silently.

This ensures consistency across all configuration pages and avoids redundant UI dialogs.

---

#### 🧭 Sub-page behavior

Each sub-page sends the `"SettingsChanged"` message **only if a real modification occurs**.

**Pattern (example from `LanguagePage`):**

```csharp
private string _originalSettingCode;
private string _currentSettingCode;

// Load
_originalSettingCode = db.GetActiveLanguageCode();
_currentSettingCode = _originalSettingCode;

// On change
_currentSettingCode = selectedLangCode;

// On save
if (_currentSettingCode != _originalSettingCode)
{
    db.SetLanguage(_currentSettingCode);
    DataCache.Instance.Refresh(db);
    MessagingCenter.Send<object>(this, "SettingsChanged");
}```

| User action                      | Message Sent | Result                                 |
| -------------------------------- | ------------ | -------------------------------------- |
| Setting changed and saved        | ✅ Yes        | ConfigurationPage updates localization |
| Opened and closed without change | ❌ No         | No events triggered                    |

### 🧩 Hub (ConfigurationPage) behavior

The ConfigurationPage tracks whether any child page triggered updates during its lifetime.

* When "SettingsChanged" is received, a local flag is set:

```csharp
private bool _hasConfigChanges = false;

MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ =>
{
    _hasConfigChanges = true;
    ApplyLocalization();
    await ShowSettingsUpdatedMessage();
});````

* When the user exits the hub (via close button or navigation back), the page checks this flag before triggering a full refresh:

```csharp
private async void OnCloseClicked(object sender, EventArgs e)
{
    if (_hasConfigChanges)
    {
        // trigger calendar refresh and apply cache updates
        MessagingCenter.Send<object>(this, "ConfigurationHubClosedWithChanges");
    }

    await Shell.Current.GoToAsync("//main", true);
}
````

* The MainPage (calendar) listens for "ConfigurationHubClosedWithChanges" and refreshes only if this message is received.

| Behavior                                               | Effect                                             |
| ------------------------------------------------------ | -------------------------------------------------- |
| User opens ConfigurationPage and exits without changes | No cache refresh, no calendar redraw               |
| User modifies settings in any sub-page                 | UI updates, cache reloads, calendar refreshed once |
| Consistent event logic across all settings pages       | Efficient performance and predictable UX           |

---

### 🔲 **ConfigBasePage**

Shared base layout for all configuration pages.
Provides:

* Consistent background, margins, and typography
* Toolbar with close icon (`close_icon.png`)
* Standard text styles (`SectionLabelStyle`, `InstructionLabelStyle`)
* Unified page structure inherited by all config pages

---

### 🍔 **Burger Menu (AppShell Flyout)**

| Item     | Page              | Notes                |
| -------- | ----------------- | -------------------- |
| Calendar | MainPage          | Default start page   |
| Settings | ConfigurationPage | Central settings hub |
| Exit     | ExitPage          | Closes the app       |

Menu items are localized and automatically close the fly-out upon selection.

---

### ⚙️ Configuration Pages

All configuration pages in PADMA follow a unified design and behavior pattern.

Each page:
- Inherits from `ConfigBasePage` (located in `PADMA/UI/Templates`).
- Contains:
  - Localized **title** (shown on toolbar),
  - **Instruction label** under the title,
  - One or more **radio button groups** for choosing a setting,
  - Standardized navigation via **close icon (X)** and **back arrow (←)**.
- All text elements use the localization system:
  `Localization.GetLocalizedText("...", DataCache.Instance.CurrentLanguageCode)`
- All settings are persisted in the SQLite `APPSETTING` table and refreshed via `DataCache`.

### Common Behavior

| Action | Description |
|--------|-------------|
| Opening | Page loads current setting from DB (`DatabaseService.GetAppSettingsList()` or specific query). |
| Changing option | Updates internal variable but **does not** immediately modify DB. |
| Exiting (back or X) | If the user changed a setting → shows localized confirmation dialog: <br>`"Apply new settings for [page purpose]?"` |
| Confirmation (Yes) | Calls `DatabaseService.SetAppSettingActive(group, settingCode)`, refreshes cache, and sends a `MessagingCenter` message `"SettingsChanged"`. |
| No changes or cancel | Closes silently without any refresh. |

---

### Implemented Pages

| Page | Group | Options | Title (EN) | Description |
|------|--------|----------|-------------|--------------|
| **LanguagePage** | `LANG` | English / Українська / Polski / Русский | *Language* | Select interface language |
| **FirstDayOfWeekPage** | `WEEK` | Monday / Sunday | *First day of week* | Choose which day starts the week |
| **TransitsPage** | `TRANZIT` | From natal Moon / From Lagna / From both Moon and Lagna | *Planetary transits* | Choose planetary transits display mode |
| **NodesPage** | `NODE` | Mean / True | *Nodes (Rahu and Ketu)* | Choose node calculation method |

---

🧩 Notes

- All configuration pages reuse shared styles:
  - `PageTitleStyle` for titles  
  - `InstructionLabelStyle` for instructions  
  - `LabelTextStyle` for option labels  
- The `ConfigurationPage` acts as a **hub**, linking to all configuration pages.
- `MessagingCenter` is the central notification system used for all `"SettingsChanged"` events.
- Adding a new configuration page only requires:
  1. Creating a new `XAML`/`code-behind` pair inheriting from `ConfigBasePage`
  2. Adding localized texts to `APP_TEXTS`
  3. Defining new settings in `APPSETTING`
  4. Adding a navigation button on `ConfigurationPage`

---

### 🧾 **Database Schema Summary**

| Table                    | Purpose                                                           |
| ------------------------ | ----------------------------------------------------------------- |
| APPSETTING               | Application settings (LANGUAGE, WEEK, etc.)                       |
| APP_TEXTS                | Localized UI strings for all supported languages (en, uk, pl, ru) |
| LANGUAGE / LANGUAGE_DESC | Supported interface languages                                     |
| COLOR / COLOR_DESC       | Color definitions                                                 |
| PLANET / PLANET_DESC     | Planet reference data                                             |
| LOCATION, PROFILE        | Geographic and user data                                          |
| Other *_DESC             | Localized reference descriptions                                  |

---

## **Part II — Implemented MAUI Architecture**

### 🧩 Core Components

#### 🔹 DatabaseService

* Manages direct SQLite access.
* Handles copying default DB from resources if missing.
* Provides CRUD for app settings and look-up tables.

**Key Methods**

* Method: SetAppSettingActive(string groupCode, string settingCode)
* Purpose: Deactivates all existing settings in a given group and activates the specified one.
* Used by configuration pages (Language, FirstDayOfWeek, Transits, Nodes, etc.)

**Other methods:**
```csharp
GetAppSettingsList()
GetFirstDayOfWeekFromDb()
GetActiveLanguageCode()
GetAppTextsList(string languageCode)
GetLanguages()
GetColors(), GetColorDescs()
GetPlanets(), GetPlanetDescs()
```

* Includes automatic database version synchronization via APP_META (see “Database Versioning & Auto-Refresh System”).

---

#### 🔹 DataCache

Central in-memory cache initialized at startup.

**Responsibilities**

* Loads all reference and localized data (`LoadAll`).
* Refreshes localized data and app settings on configuration change (`Refresh`).
* Stores `CurrentLanguageCode` for the active UI culture.

---

#### 🔹 ServiceLocator

Simple DI container wrapper for accessing shared services globally:
`ServiceLocator.Services.GetService<DatabaseService>()`

---

### 🧰 Project Utilities

Folder: PADMA/Core/Utilities/
Common reusable helpers shared across all modules of PADMA.
Each class provides standalone functionality with no dependency on UI or database code.
Used by pages, services, and future calculation modules.

### 🈴 Localization

File: Localization.cs
Provides unified access to localized interface texts stored in the APP_TEXTS table.

Features:
Method	Description
GetLocalizedText(string nativeText, string langCode)	
Returns localized version of a given text. If translation is missing — returns nativeText.

#### 🔤 Localization System 

All user-facing texts in PADMA must support full multilingual localization across **four languages**:
🇬🇧 English (`en`), 🇺🇦 Ukrainian (`uk`), 🇵🇱 Polish (`pl`), 🇷🇺 Russian (`ru`).
Localized texts are stored in the `APP_TEXTS` table and can be dynamically refreshed from the database.

### 🧱 Localization Data Management

Localized texts are added incrementally per feature or page, not globally.
Before adding new texts to APP_TEXTS, always check for existing NATIVETEXT entries to avoid duplicates.
Each new text must include all four language variants (en, uk, pl, ru).
Use simple INSERT (not REPLACE) — to prevent overwriting existing translations.
The current database damp file (for checking tables structure and content) is always stored at this location: PADMA/docs/sql/padma_tables.sql

##### 🧩 Implementation Rules

1. **Localization Function (Required)**
   All UI strings must be localized using the utility method:

   ```csharp
   Localization.GetLocalizedText(nativeText, DataCache.Instance.CurrentLanguageCode);
   ```

   * `nativeText` — English base string (the canonical key).
   * `CurrentLanguageCode` — one of the four language codes (`en`, `uk`, `pl`, `ru`).
   * This method retrieves the correct translation from the `APP_TEXTS` table.
   * If translation for the selected language does not exist, the English version (`en`) is used as fallback.
   * **All four languages must be present in the database** for every UI text — even English (as a reference row).

2. **Named UI Elements (Mandatory)**
   Every UI element that displays or updates localized text (e.g. `Label`, `Button`, `Entry.Placeholder`, `Title`)
   must have an explicit `x:Name` in XAML, so that the element can be accessed and updated in runtime:

   ```xml
   <Button x:Name="btnLanguage" Text="Language" />
   ```

   Example usage in code-behind:

   ```csharp
   btnLanguage.Text = Localization.GetLocalizedText("Language", langCode);
   ```

3. **Auto-Refresh on Re-Appear**
   Each page that uses localized text must call a localization refresh (e.g. `ApplyLocalization()`) from `OnAppearing()`
   to ensure that texts update immediately after the language setting changes.

4. **Database Consistency**
   The `APP_TEXTS` table must contain entries for each language, following this format:

   ```
   NATIVETEXT | LANGUAGECODE | FOREIGNTEXT
   ```

   Example:

   | NATIVETEXT | LANGUAGECODE | FOREIGNTEXT |
   | ---------- | ------------ | ----------- |
   | Language   | en           | Language    |
   | Language   | uk           | Мова        |
   | Language   | pl           | Język       |
   | Language   | ru           | Язык        |

5. **Fallback Logic**
   If no translation exists for the selected language, `GetLocalizedText()` must safely return the English text.

---

### 🕒 DateTimeExtensions

File: DateTimeExtensions.cs
Provides reusable helper methods for DateTime operations — range checks and timezone corrections.
Used across future calculation modules (Sunrise, Muhurta, Transits).

**Methods:**
| Method                                                       | Description                                                |
| ------------------------------------------------------------ | ---------------------------------------------------------- |
| `Between(DateTime date, DateTime start, DateTime end)`       | Checks if `date` is within inclusive range `[start, end]`. |
| `StrictBetween(DateTime date, DateTime start, DateTime end)` | Checks if `date` is within exclusive range `(start, end)`. |
| `ShiftByUtcOffset(TimeSpan baseUtcOffset)`                   | Shifts `DateTime` by a given UTC offset.                   |
| `ShiftByDaylightDelta(TimeZoneInfo.AdjustmentRule[] rules)`  | Adjusts date by daylight saving time delta.                |

**Example:**
```if (now.Between(sunrise, sunset))
    Console.WriteLine("Daytime period");```

---

### 🧠 ViewModels

#### CalendarViewModel

* Holds current year, month, culture, and collection of day items.
* Generates a 6 × 7 grid of days respecting first-day-of-week setting.
* Supports localization via `CultureCode` → `CultureInfo`.

** Methods:
  * `InitializeCulture()`
  * `ReloadCultureAndRefresh()`
  * `MoveMonth(int delta)`
  * `RefreshCalendar()`
  * `GenerateDays(year, month)`

---

### 🧩 Messaging & Refresh Flow

| Event               | Trigger                | Result                                                                    |
| ------------------- | ---------------------- | ------------------------------------------------------------------------- |
| `"SettingsChanged"` | Any configuration page | MainPage → ReloadCultureAndRefresh() → UpdateTitle() + UpdateDaysHeader() |
| OnAppearing()       | Returning to MainPage  | Ensures DataCache.Refresh() and UI rebuild                                |

---

### 🧱 Shared UI & Styles

* All configuration pages inherit **ConfigBasePage**.
* Shared font styles defined in `Resources/Styles`.
* Icon resources in `Resources/Images`:

  * `left_arrow.png`, `right_arrow.png`, `close_icon.png`, `flags/*.png`
* Uniform spacing and padding throughout all MAUI pages.

---

### 🗄️ Data Flow Summary

```
SQLite  ⇄  DatabaseService  ⇄  DataCache
                              ⇓
                        ViewModels (Calendar, Config)
                              ⇓
                           XAML Pages
```

* On app launch → Database copied (if missing) → DatabaseService initialized → DataCache.LoadAll().
* All pages access reference data and localized texts via DataCache.
* Configuration changes trigger DataCache.Refresh() and UI rebuilds through MessagingCenter + OnAppearing().

---

### 🌐 Localization Pipeline

1. Language selected in **LanguagePage** → `APPSETTING` updated.
2. **DataCache.Refresh()** reloads localized texts.
3. `"SettingsChanged"` event sent.
4. **MainPage** updates culture, title, and weekday headers.

---

### 📱 Platform Notes

* Responsive grid for all screen sizes.
* Works on Android / iOS / Windows.
* Uses only built-in .NET MAUI controls and SQLite-net.
* Light theme (white background, dark text).
* Extensible structure — future pages (ThemePage, NotificationPage, etc.) can reuse ConfigBasePage template.

---

### 🗃️ Database Versioning & Auto-Refresh System

PADMA maintains synchronization between the embedded database (Resources/Raw/PADMADB.db3)
and the runtime user copy stored in the local app folder.

### ⚙ Version Control Mechanism

Each database file contains an entry in the APP_META table:

APP_META
├── ID (int)
├── KEY (string)
└── VALUE (string)

The version key is stored under KEY = 'DB_VERSION'.

### 🔁 Auto-Update Logic

During app startup, DatabaseService compares the value of DB_VERSION in:
The resource database (Resources/Raw/PADMADB.db3)
The active user database (FileSystem.AppDataDirectory/PADMADB.db3)

* If the versions differ:
* The local database file is deleted.
* A new copy of the updated database is written from resources.
* The new connection is initialized automatically.

---

✅ **End of Document**

> Next revision planned after implementation of ThemePage and Transits visualization module.
