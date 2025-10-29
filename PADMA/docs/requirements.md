> Context: This document is used by ChatGPT (GPT-5) for project PADMA continuation.  
> Always load this file first in a new session to resume context.

# 🪶 PADMA — Project Requirements & Current Implementation  
> _Version: October 2025_  

---

## 🗓️ Overview  

**PADMA** — cross-platform application built with **.NET9 / MAUI ** and **SQLite (sqlite-net-pcl)**.  
It displays a localized astrological calendar, user configuration pages, and other utilities.  
All settings, interface texts, and reference data are stored in the embedded SQLite database  
**`PADMADB.db3`**, which is cached in memory at runtime.

---

## 🧱 Architecture  

### 📂 Project Structure

| Folder | Description |
|---------|-------------|
| `PADMA/Core` | Business logic, models, services, and utilities |
| `PADMA/Pages` | UI pages (XAML + code-behind) |
| `PADMA/UI/Templates` | Common page templates (e.g. `ConfigBasePage`) |
| `PADMA/Resources/Raw` | Embedded SQLite database and static assets |
| `PADMA/Resources/Styles` | Shared UI styles (fonts, colors, margins) |
| `PADMA/docs` | Documentation and requirements |

### ⚙️ Startup Process
1. On application start:
   - The bundled database `PADMADB.db3` from `/Resources/Raw` is copied to the local app directory.
   - `DatabaseService` loads the content.
   - `DataCache.Instance.LoadAll()` caches all required data (languages, settings, texts, references).
2. If the database version in table `APP_META` differs from the existing one —  
   → the local database is automatically replaced with the new version.

---

## ⚙️ Core Services

### 🔸 DatabaseService
Handles all database operations via SQLite.

**Key responsibilities:**
- Provides unified CRUD access for application settings and reference tables.  
- Automatically replaces outdated database versions (via `APP_META` table check).  

**Main methods:**
```csharp
GetAppSettingsList()
GetAppTextsList(string languageCode)
GetActiveSetting(string groupCode)
GetActiveSettingCode(string groupCode)
GetActiveLanguageCode()
GetFirstDayOfWeekFromDb()
SetLanguage(string code)
SetFirstDayOfWeek(string code)
SetAppSettingActive(string groupCode, string settingCode) // universal for any config page
```


## ⚙️ AppSettingsService  

**Purpose:**  
Provides centralized logic for loading and managing all application settings from the database.  
It acts as a higher-level abstraction over `DatabaseService`, offering grouped access to configuration options.

**Responsibilities:**
- Loads all configuration groups from the `APPSETTING` table.  
- Retrieves currently active setting for each group.  
- Activates specific settings and updates their state in the database.  
- Notifies the system (via `MessagingCenter`) when a configuration change occurs.

**Main methods:**
```csharp
LoadAllSettings()
GetActiveSetting(string groupCode)
ActivateSetting(string groupCode, string settingCode)
```

**Behavior:**
- When a setting is activated, all others in the same group are automatically deactivated.  
- Ensures data consistency between database and in-memory cache (`DataCache`).  
- Often used by configuration pages derived from `ConfigBasePage`.  


## 🧰 KeyboardHelper  

**Purpose:**  
Provides utility methods to control the virtual keyboard behavior on mobile devices.  
Ensures a clean UI experience by automatically hiding the keyboard after certain actions (e.g. tapping a button, navigating away).

**Methods:**
```csharp
HideKeyboard(Page page)
HideKeyboard(View control)
```

**Behavior:**
- Detects the current focused element and dismisses the soft keyboard.  
- Typically invoked after text entry or form submission within `ProfileDetailPage` and similar pages.  
- Supports both Android and iOS platforms.  


## 🎨 XAML Converters  

**Purpose:**  
Provide reusable value converters for binding logic in XAML.  
They translate application data into visual UI states such as colors, sizes, or visibility.

**Defined converters:**

| Converter | Description |
|------------|--------------|
| `BoolToColorConverter` | Returns a color depending on a boolean value (e.g. highlight selected items). |
| `DateTimeToTimeSpanConverter` | Converts `DateTime` objects to `TimeSpan` or formatted time strings for display. |
| `HeightToCellHeightConverter` | Dynamically adjusts element height based on grid layout size. |
| `TodayBackgroundConverter` | Highlights the current day cell with accent color in the calendar grid. |

**Usage Example (XAML):**
```xml
<Label Text="{Binding IsToday, Converter={StaticResource TodayBackgroundConverter}}" />
```

All converters are declared as resources in XAML and shared across calendar and configuration pages.


## ⏱️ DateTime Format and Precision

**Purpose:**  
To ensure consistent handling of dates and times across the application and database.

**Standard Format:**  
All date and time values use the standard .NET `DateTime` structure and are stored in SQLite in the format:

```
yyyy-MM-dd HH:mm:ss
```

**Rules:**
- Precision is up to **seconds**.  
- `System.Globalization.CultureInfo.InvariantCulture` is always used when converting to or from text.  
- All data persisted to the database must follow this invariant format to ensure cross-platform consistency.  
- Example (from `Profile` module):
  ```csharp
  var dateText = profile.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
  ```
- This convention applies to all future modules, including ephemeris and time-based computations.

---

### 🔸 DataCache  

**Purpose:**  
Central in-memory cache loaded at startup and refreshed after configuration changes.  

**Responsibilities:**  
- Loads all reference data and localized interface texts.  
- Stores current language code (`CurrentLanguageCode`).  
- Provides instant access to data without repeated DB queries.

**Main methods:**
```csharp
LoadAll(DatabaseService db, string? preferredUiLang = null) // called at startup
Refresh(DatabaseService db) // called after configuration changes
```

---

### 🔸 Localization System  

**Purpose:**  
Provides localized UI texts using translations from the `APP_TEXTS` table.  

**Usage:**
```csharp
Localization.GetLocalizedText("NativeText", DataCache.Instance.CurrentLanguageCode);
```

**`APP_TEXTS` structure:**  
Each entry contains a native text and up to four translations (`en`, `uk`, `pl`, `ru`).  
If a translation is missing — returns the native English string.

---

### 🔸 ServiceLocator  

**Purpose:**  
Provides a simple global entry point for dependency-injected services (e.g. `DatabaseService`).  

**Usage example:**
```csharp
var db = ServiceLocator.Services.GetService<DatabaseService>();
```

---

## 🧩 UI Templates & Layout Standards  

The following templates define visual and structural consistency across all pages.

---

### 🔹 ConfigBasePage  

**Purpose:**  
Provides a common layout for all configuration pages.

**Features:**
- Inherits from `ContentPage`.
- Unified padding, margins, and font styles.
- Toolbar with close icon (`close_icon.png`).
- Title automatically localized.
- Uses shared styles from `/Resources/Styles`.

| Element | Style | Description |
|----------|--------|-------------|
| Page title | `PageTitleStyle` | Bold, centered, localized |
| Instruction text | `InstructionLabelStyle` | Medium font, standard margin |
| Option labels | `LabelTextStyle` | Used beside radio buttons |
| Radio buttons | Grouped per configuration option |

---

### 🔹 ConfigurationPage  

**Purpose:**  
Acts as a hub for accessing all configuration pages.  

**Layout:**
- Inherits from `ContentPage`.
- Localized title `"Settings"`.
- Vertical list of localized navigation buttons:
  - `LanguagePage`
  - `FirstDayOfWeekPage`
  - `TransitsPage`
  - `NodesPage`
  - `Hora`
  - `Muhurtas`
  - `MrityuBhaga` 
  - `Sunrise`
- Toolbar with close icon (returns to `MainPage`).

**Behavior:**
- Subscribes to `"SettingsChanged"` messages from all child pages.  
- On receiving the event → refreshes cache and localized texts.  
- If no changes occurred → closes silently.

### 🔹 MessagingCenter — unified contract

* Subscribers use a single subscription:
```
MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ => { ... });
```

* All child pages must send with TSender = object:
```
MessagingCenter.Send<object>(this, "SettingsChanged");
```

Rationale: Xamarin/MAUI MessagingCenter matches by generic sender type.
Mismatched types (e.g., Send<NodesPage>, Subscribe<object>) won’t deliver the message.

---

### 🔹 MainPage  

**Purpose:**  
Main calendar view of the application.  

**Layout:**
- Toolbar — localized month title and navigation buttons (`left_arrow.png`, `right_arrow.png`).
- Weekday header row — localized 3-letter abbreviations.
- Main grid — 6×7 bordered day cells.  
  Each cell includes:
  - Day number (top-left).
  - 6 color bars (transit placeholders).

**Behavior:**
- Loads current language and first-day-of-week from cache.
- Reacts to `"SettingsChanged"` messages.
- Rebuilds layout when:
  - Language changes,
  - First day of week changes,
  - Month navigation occurs.
- Uses `ReloadCultureAndRefresh()` for culture updates.
- Title capitalization follows current culture (`ToTitleCase()`).

---

### 🔹 Common Visual Standards  

| Element | Property | Value |
|----------|-----------|-------|
| Font | Default | *OpenSans* |
| Title size | 22sp |
| Label size | 14–16sp |
| Background | `#FFFFFF` |
| Text color | `#333333` |
| Grid borders | `#CCCCCC` |
| Accent color (today) | Light blue |
| Default padding | 16px |
| Default spacing | 12px |

---

### 🔹 Future Reuse  

- All new configuration pages must inherit from `ConfigBasePage`.  
- Non-configuration pages (e.g. reports or charts) must follow the same style and spacing.  
- Common color and typography palette must remain consistent.

---

## ⚙️ Configuration Pages  

All configuration pages share a unified structure and behavior pattern, defined by `ConfigBasePage`.  
Each page includes:
- Localized title and instruction label.  
- One or more radio button groups.  
- Confirmation dialog when exiting with changes.  
- Silent exit if no changes occurred.  
- Message broadcast `"SettingsChanged"` when changes are saved.  
- Cache refresh through `DataCache.Instance.Refresh(db)`.  

**Common confirmation dialog:**
> “Apply new settings for [setting name]?”

---

### Implemented configuration pages

| Page | GroupCode | Options | Description |
|-------|------------|----------|-------------|
| LanguagePage | LANGUAGE | ENGLISH, UKRAINIAN, POLISH, RUSSIAN | Selects UI language |
| FirstDayOfWeekPage | WEEK | MONDAY, SUNDAY | Sets first day of week |
| TransitsPage | TRANSIT | MOON, LAGNA, MOONANDLAGNA | Selects planetary transits mode |
| NodesPage | NODE | MEAN, TRUE | Chooses Rahu/Ketu calculation method |
| HoraPage | HORA | HORADAYNIGHT, HORAEQUAL, HORAFROM6 | Chooses Hora calculation method |
| MuhurtasPage |MUHURTAGHATI | MUHURTAGHATIDAYNIGHT, MUHURTAGHATIEQUAL, MUHURTAGHATIFROM6 | Chooses Muhurta & Ghati calculation method |
| MrityuBhaga| MRITYUBHAGA | NEQUAL, NLESS, NMORE, NERNST | Chooses Mrityu Bhaga calculation method |
| Sunrise | SUNRISE | TIP, CENTER | Chooses Sunrise calculation method |

---

### 🔄 Global Behavior  

**Shared logic across pages:**
- All configuration pages trigger `"SettingsChanged"` via `MessagingCenter` after saving changes.
- `ConfigurationPage` listens for these messages and refreshes texts.
- `MainPage` listens for the same message and rebuilds its layout.
- If user navigates back without changes — no message or refresh occurs.
- Localization applies dynamically to all visible elements on appearance.

---

## 🧾 Database Schema  

| Table | Purpose |
|--------|----------|
| APPSETTING | Stores all app configuration settings |
| APP_TEXTS | Localized UI texts |
| APP_META | Stores database version number for replacement logic |
| LANGUAGE, COLOR, PLANET | Reference tables |
| *_DESC | Language-specific descriptions for reference tables |
| LOCATION | Geographic data (limited to predefined entries) |
| PROFILE | User profiles linked to LOCATION |

---

## 🧩 Architecture Notes

This section summarizes key technical and behavioral conventions that define PADMA’s internal consistency across all MAUI components.

---

### 🔹 Unified Messaging Contract  
All configuration pages communicate updates using a single `MessagingCenter` event pattern:

```csharp
MessagingCenter.Send<object>(this, "SettingsChanged");
MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ => { ... });
```

- Ensures consistent message delivery regardless of page type.  
- Prevents common MAUI issues with mismatched sender types.  
- Allows `ConfigurationPage` to listen universally to all child updates.

---

### 🔹 Config Pages Consistency  
Every configuration page inherits from `ConfigBasePage` and adheres to a unified structure:

- Localized **title** and **instruction label**.  
- A group of `RadioButton` options linked to persistent settings in `APPSETTING`.  
- Confirmation dialog on exit (only when changes are detected).  
- Updates the database using  
  ```csharp
  DatabaseService.SetAppSettingActive(group, code);
  ```  
- Refreshes cache via  
  ```csharp
  DataCache.Instance.Refresh(db);
  ```  
- Notifies the main interface through  
  ```csharp
  MessagingCenter.Send<object>(this, "SettingsChanged");
  ```

---

### 🔹 Centralized Cache Refresh  
`DataCache.Instance.Refresh(db)` is invoked **only after confirmed configuration updates**.  
This avoids unnecessary reloads and ensures the user immediately sees updated texts, settings, or localization changes.

---

### 🔹 Defensive UI Updates  
`MainPage` and `ConfigurationPage` both use internal flags (e.g. `_hasConfigChanges`) to determine whether UI refreshes are required after returning from a configuration page.

- If no settings were changed, navigation returns instantly without rebuilding the calendar.  
- If changes exist, the calendar and localized interface are refreshed.  
This optimization significantly improves perceived performance on all platforms.

---

### 🔹 Localization Flow  
All text localization uses:
```csharp
Localization.GetLocalizedText("Native English Text", DataCache.Instance.CurrentLanguageCode);
```

- English text entries are **mandatory** in `APP_TEXTS` (as base keys).  
- Each localized record must include English, Ukrainian, Polish, and Russian variants.  
- Dynamic UI elements (titles, labels, buttons) must have `x:Name` assigned for runtime localization updates.

---

### 🔹 Database Versioning  
The table `APP_META` stores database version info.  
On app startup, `DatabaseService` compares the deployed and local DB versions and automatically replaces outdated copies from `/Resources/Raw/PADMADB.db3`.  
This guarantees schema and localization updates propagate without manual intervention.

---

### 🔹 Extension Methods  
Utility extensions defined in `PADMA/Core/Utilities/Extensions.cs` provide reusable helpers for date/time operations:

```csharp
date.Between(start, end);
date.StrictBetween(start, end);
date.ShiftByUtcOffset(offset);
date.ShiftByDaylightDelta(adjustmentRules);
```

These methods standardize temporal logic across astronomical and calendar-related calculations.

### 👤 Profiles feature

---

### 🎯 Purpose  

The **Profiles** feature stores personal data for individual users  
(name, surname, birth date, birth and living locations).  
It enables PADMA to perform automatic calculations and display  
personalized data (e.g., location-based information) upon app startup.  

Each profile may be marked as **default**,  
which determines which profile data is automatically used by the **Calendar** page.  

---

### 🧱 Entry Point  

The feature is accessible from the main **Shell** menu (burger menu).  

**Current menu structure:**  
```
Calendar  
Profiles   ← new section  
Settings  
Exit
```

**File:** `AppShell.xaml`  
The route `Profiles` is registered in `AppShell.xaml.cs`.  

---

## 🧭 Profiles — Navigation & UI Behavior  

### 🧱 Overview  

This section defines the navigation flow, toolbar layout, and user interaction logic  
for all pages within the **Profiles** feature.  
It ensures full consistency with the existing PADMA user interface conventions  
used by **ConfigurationPage** and other modules.  

---

### 🏠 ProfilesPage — Main Hub  

**Toolbar layout:**  
```
☰  Profiles                              ❌
```

**Action Bar (below toolbar):**  
```
[➕ Add new profile]
```

**Behavior:**  
- Tap on a profile → opens `ProfileDetailPage` in **View mode**.  
- Tap “Add new profile” → opens `ProfileDetailPage` in **Create mode**.  
- ❌ Close → returns to **MainPage**.  
- On return from a child page, the list refreshes to reflect changes.  

---

### 👤 ProfileDetailPage — Profile Card  

**Toolbar layout:**  
```
←  [Profile name / New profile]           ❌
```

**Action Bar (below toolbar):**  
```
[ 💾 Save ] [ ✏️ Edit ] [ 🌟 Set default ] [ 🗑 Delete ]
```

**Button order confirmed:** Save first.

---

#### 🔹 Modes of operation  

| Mode | Description |
|------|--------------|
| **View** | All input fields are read-only. Only action buttons are active. |
| **Edit** | All input fields become editable, including name, surname, date, and locations. |
| **New** | Empty form, all fields editable, title = “New profile”. |

---

#### 🔹 Field structure  

| Field | Description |
|--------|-------------|
| Profile name | Editable text field |
| Person name | Editable text field |
| Person surname | Editable text field |
| Date of birth | Editable date picker |
| Place of birth | Button → opens `LocationPage` |
| Place of living | Button → opens `LocationPage` |
| Message / notes | Multiline entry (optional) |

---

#### 🔹 Button behavior  

| Button | Action | Confirmation |
|---------|---------|---------------|
| 💾 **Save** | Writes changes to `PROFILE` and new `LOCATION` entries if needed. Updates cache and returns to `ProfilesPage`. | ✅ “Save changes to profile?” |
| ✏️ **Edit** | Enables editable mode for all fields. | — |
| 🌟 **Set default** | Sets current profile as default (`CHECKED = 1`), unsets others. | — |
| 🗑 **Delete** | Deletes current profile and returns to list. | ✅ “Delete this profile?” |

The **❌ Close** icon and **← Back arrow** both trigger the same method `HandleBackAsync()`.
Both must show the same confirmation dialog sequence when unsaved changes exist.  
If no changes exist, they immediately return to the profiles list without prompts. 

---

### 🔹 Confirmation Dialogs and Validation Rules
| Event | Dialog | Conditions |
|--------|---------|-------------|
| Leaving the page with unsaved changes | “Do you want to save changes before exit?” | Triggered if `HasRealChanges()` returns true. |
| Manual save via 💾 button | “Save changes to profile?” | Always displayed before database write. |
| Successful save | “Profile saved successfully.” | Shown after profile insert or update succeeds. |
| Save error | “Failed to save profile. Please try again.” | Shown when a database or validation exception occurs. |
| Validation alerts | Localized messages per field | Required fields: **Profile Name**, **Date of Birth**, **Place of Birth**, **Place of Living** |


Dialogs follow the same visual and logical pattern as those used in `ConfigBasePage`.  

---

### 🌍 LocationPage — Location Lookup  

**Purpose:**  
Searches and selects geographic locations using **Nominatim API**.  

**Toolbar layout:**  
```
←  Location search
```

**Behavior:**  
- User searches a place name (Nominatim API).  
- Search results show locality, region, country, coordinates.  
- Selecting a result returns to `ProfileDetailPage`,  
  filling only the **LOCALITY** field in the form.  

**Persistence logic:**  
- Selected locations are **not immediately saved** into `LOCATION`.  
- Database update occurs only after the entire profile is saved.  
- Therefore, this page acts as a **lookup-only** component, not a data editor.  

---

### 🧩 Navigation hierarchy  

```
AppShell
 ├── MainPage (Calendar)
 ├── ProfilesPage (burger + close)
 │    └── ProfileDetailPage (back + close)
 │         └── LocationPage (back only)
 ├── ConfigurationPage (burger + close)
 └── ExitPage
```

---

### 🔹 Navigation and State Persistence
- Navigating to `LocationSearchPage` **must not reset** or discard current profile form data.  
- Temporary state of the current profile is stored in `_tempProfile`.  
- When returning from the search page, all entered values remain intact.  
- Page exit logic checks for actual modifications using `HasRealChanges()` which compares serialized state `_snapshotJson`.

```csharp
private static Profile? _tempProfile;  // Holds unsaved form data
private string? _snapshotJson;         // Serialized snapshot of initial state
private bool HasRealChanges() => JsonSerializer.Serialize(_profile) != _snapshotJson;
```

---

### 🔹 Behavior When Returning from Location Search
- `MessagingCenter` sends `"LocationSelected"` with `(Mode, AppLocation)` payload.  
- The appropriate field (`Place of Birth` or `Place of Living`) updates immediately.  
- `_snapshotJson` is **not refreshed** after location selection — this ensures the “unsaved changes” dialog remains functional.  
- The `_skipSnapshotOnce` flag prevents snapshot regeneration upon re-entering the page.

---

### ⚙️ Integration rules  

- Navigation consistency follows the same convention as configuration pages.  
- Confirm dialogs are reused from existing shared logic.  
- Changes in profiles may trigger `"ProfileChanged"` messaging events,  
  to notify the Calendar or other pages of an active profile switch.  
- Data saving logic ensures that new locations are committed only when  
  a profile save operation is confirmed.  
- On app startup, **DataCache** loads the current default profile  
  (`CHECKED = 1`) together with cached settings, texts, and references.  
- Default profile data will later be used by **Calendar** or  
  other computational modules.  
- Changes in profiles do **not** send `"SettingsChanged"` messages,  
  unless language or calendar configuration are directly affected.  
- In future development, a dedicated `"ProfileChanged"` event may be added. 

---

### 🌐 Localization  

All pages in the Profiles feature follow the same localization contract  
as the rest of PADMA:  

```csharp
Localization.GetLocalizedText("Native English Text", DataCache.Instance.CurrentLanguageCode);
```

All UI text entries must exist in the `APP_TEXTS` table  
with four translations: **English, Ukrainian, Polish, Russian**.  

---

### 🎨 UX summary  

```
╔════════════════════════════╗
║ ☰ Profiles                ❌ ║
╚════════════════════════════╝
[ ➕ Add new profile ]
───────────────
• John Doe
• Mary Smith
───────────────
      │
      ▼
╔════════════════════════════╗
║ ← John Doe                ❌ ║
╚════════════════════════════╝
[ 💾 Save ] [ ✏️ Edit ] [ 🌟 Set default ] [ 🗑 Delete ]
────────────────────────────────────---------
| Profile name: John Doe            		|
| Date of birth: 12.05.1988 12:48:00        |
| Place of birth: Kyiv ⯈           			|
| Place of living: Warsaw ⯈        			|
────────────────────────────────────---------
      │
      ▼
╔════════════════════════════╗
║ ← Location search           ║
╚════════════════════════════╝
| Search: [Kyiv] (🔍)         |
| Results:                   |
|  - Київ, Україна           |
|  - Kyiv, Ukraine           |
────────────────────────────────────
```

**Data source:**  
Location data structure is defined in  
[`docs/sql/padma_tables.sql`](https://github.com/Serhii-Sheremet/PADMA/blob/main/PADMA/docs/sql/padma_tables.sql). 

### 🗺️ Nominatim (OpenStreetMap)

To find GPS coordinates for locations the Nominatim API are used
🔗 https://nominatim.org/release-docs/latest/api/Search/

---

---

## 🌠 SwissService — Swiss Ephemeris Integration

**Purpose:**  
Provides high-precision astronomical and astrological calculations using the **Swiss Ephemeris** native library  
integrated directly (not via `SwissEphNet`). All computations are executed in **UTC (GMT-0)**  
with the sidereal Lahiri mode by default.

---

### 🧭 Core Principles

- All calculations are performed relative to **UTC / GMT-0**.  
- Input dates are converted to UTC before processing.  
- Output values remain in UTC and are only converted to local time in the UI layer.  
- DST adjustments rely on .NET’s `TimeZoneInfo` and `AdjustmentRules`.  
- Default flags:  
  - `SEFLG_SWIEPH | SEFLG_SPEED | SEFLG_SIDEREAL`  
  - `SE_SIDM_LAHIRI` (sidereal mode).  
- Internal precision — double (native Swiss Ephemeris).

---

### 🧬 Implementation Overview

#### 🦪 Architecture and File Layout
| Component | Role |
|------------|------|
| `Core/Native/SwissEphemerisNative.cs` | P/Invoke bindings for native SWE methods (`swe_julday`, `swe_calc_ut`, `swe_set_ephe_path`, `swe_get_ayanamsa_ut`, `swe_set_sid_mode`, `swe_close`) |
| `Core/Services/SwissService.cs` | Initializes path, sets sidereal mode, computes positions, handles ephe.zip unpacking |
| `Core/Services/SwissAnalysis.cs` | Performs iterative analysis (e.g. for Moon states) and derives higher-level astrological data |
| `Core/Utilities/SwissUtility.cs` | Provides helper conversions (planet mapping, zodiac, nakshatra, pada, navamsa) |
| `Resources/Raw/ephe.zip` | Embedded archive with Swiss Ephemeris data files (`.se1`) — *flat archive, no subfolder* |

---

### ⚙️ Initialization Logic

1. **`ephe.zip` unpacking**  
   - On Android, the archive is extracted at startup to  
     `FileSystem.AppDataDirectory/ephe/`.  
   - Re-extraction is skipped if files already exist.  
   - Proper disposal ensures no file-in-use errors.

2. **Path setup and sidereal mode**  
   ```csharp
   await SwissService.InitializeEphemerisPathAsync();
   SwissService.SetSiderealMode(SweConst.SE_SIDM_LAHIRI);
   ```

3. **Verification test**  
   A smoke test (`CalculatePlanetDataList_London`) prints a sequence of Moon positions,  
   confirming valid ephemeris loading and sidereal mode activation.

---

### 🧱 Data and Cache Integration

#### PADA table
| Field | Type | Note |
|--------|------|------|
| `ID` | INTEGER | Primary key |
| `ZODIAKID` | INTEGER | Zodiac reference |
| `NAKSHATRAID` | INTEGER | Nakshatra reference |
| `PADANUMBER` | INTEGER | Local pada (1–4) |
| `DREKKANA` | INTEGER | Drekkana ID |
| `SPECIALNAVAMSA` | TEXT | Additional attributes |
| `NAVAMSA` | INTEGER | Navamsa zodiac ID (1–12) |
| `COLORID` | INTEGER | Color mapping |

**Corrections applied:**  
- `NAVAMSHA` → `NAVAMSA`  
- `SPECIALNAVAMSHA` → `SPECIALNAVAMSA`

#### Cache loading
```csharp
Padas = db.GetPadas().ToList();
Console.WriteLine($"[CACHE] Loaded {Padas.Count} Pada records");
```

#### Access helpers
```csharp
public static int GetPadaNumberByPadaId(int padaId)
    => DataCache.Instance.Padas.FirstOrDefault(i => i.Id == padaId)?.PadaNumber ?? 0;

public static int GetNavamsaByNakshatraAndPada(int nakshatraId, int padaNumber)
    => DataCache.Instance.Padas.FirstOrDefault(i =>
           i.NakshatraId == nakshatraId && i.PadaNumber == padaNumber)?.Navamsa ?? 0;
```

Used in `SwissAnalysis` during planet state construction:
```csharp
int navamsaId = Utility.GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber);
```

---

### 💫 Planet Calculation

Each `PlanetData` record includes:
```
DateTimeUtc | Longitude | ZodiakId | NakshatraId | PadaId |
NavamsaZodiakId | SpeedInLongitude | IsRetrograde
```

Example output (Moon, Lahiri):
```
2025-10-27 00:00:00 | L=249.3377° | Z=9 | N=19 | P=75 | Nav=3 | Speed=0.00835 | Retro=D
```

---

### 🧮 Current Functions

| Function | Description | Status |
|-----------|--------------|--------|
| `InitializeEphemerisPathAsync()` | Prepares and registers ephemeris path | ✅ Implemented |
| `SetSiderealMode()` | Sets sidereal ayanamsha (default Lahiri) | ✅ Implemented |
| `GetAyanamsa()` | Returns ayanamsha value for UTC date | ✅ Implemented |
| `GetPlanetPosition()` | Returns position, speed, distance | ✅ Implemented |
| `CalculatePlanetDataList_London()` | Generates sequence of planet states with transitions | ✅ Implemented |
| `GetZodiakIdFromDegree()` / `GetNakshatraIdFromDegree()` / `GetPadaIdFromDegree()` | Degree mapping | ✅ Implemented |
| `GetNavamsaByNakshatraAndPada()` | From DB cache | ✅ Implemented |
| `GetSunriseSunset()` | Basic version exists — needs refinement | ⚠️ Under review |
| `GetTithiSequence()` / `GetNityaYogaSequence()` / `GetMrityuBhagaPeriods()` | Lunar/solar relations | ⏸️ Planned (next phase) |
| `GetEclipses()` | Solar/lunar eclipses | ⏸️ Planned |
| `GetAscendant()` | Houses and ascendant | ⏸️ Planned |

---

### 🧩 Data Structures

#### 🪐 `SwissParameters`
Encapsulates all input parameters for Swiss Ephemeris calculations.

| Field | Type | Description |
|--------|------|-------------|
| `PlanetCode` | string | Internal planet code (matches `PLANET.PLANETCODE` in the database and the `EPlanet` enum). Used for computation and identification. |
| `PlanetId` | int | Internal planet Id (matches `PLANET` in the database and the `EPlanet` enum). |
| `Longitude` | double | Geographic longitude (East positive). |
| `Latitude` | double | Geographic latitude (North positive). |
| `Altitude` | double | Altitude above sea level (meters). |
| `UtcDateTime` | DateTime | UTC timestamp used as the base reference for calculation. |

*Note:* `PlanetCode` is not localized — localized names are stored in `PLANET_DESC` in the database.

---

#### 🧮 `SwissResult`
Represents calculation results returned by SwissService methods.

| Field | Type | Description |
|--------|------|-------------|
| `CalculationValues` | double[] | Array of values returned by Swiss Ephemeris (`longitude`, `latitude`, `distance`, `speedLon`, `speedLat`, `speedDist`). |
| `UtcSecondsOfDay` | int | Time of the calculation in seconds from the start of the UTC day. |
| `Sign` | int | Zodiac sign number (1–12). |
| `IsRetrograde` | bool | Indicates if the planet is retrograde. |
| `IsCalculationFailed` | bool | True if the calculation returned invalid or missing data. |

Used by:  
- `GetPlanetPositions()`  
- `GetTithiSequence()`  
- `GetNityaYogaSequence()`  
- `GetMrityuBhagaPeriods()`  

---

#### ⚰️ `MrityuBhagaParameters`
Defines angular limits used to detect Mrityu Bhaga (critical degrees) periods for each planet.

| Field | Type | Description |
|--------|------|-------------|
| `PlanetLongitude` | double | Current ecliptic longitude of the planet being analyzed. |
| `StartDegree` | double | Start of the critical sector (in degrees). |
| `EndDegree` | double | End of the critical sector (in degrees). |
| `IsForwardCalculation` | bool | Direction of calculation; if true, search starts from the lower boundary. |

Used by:  
- `GetMrityuBhagaPeriods()`  

---

#### 🌗 `NityaYogaTithiResults`
Stores paired ephemeris data of the Sun and Moon for computing Tithi and Nitya Yoga transitions.

| Field | Type | Description |
|--------|------|-------------|
| `SunResults` | double[] | Swiss Ephemeris output for the Sun (`longitude`, `latitude`, `distance`, `speedLon`, `speedLat`, `speedDist`). |
| `MoonResults` | double[] | Swiss Ephemeris output for the Moon (same format). |
| `UtcSecondsOfDay` | int | UTC time in seconds from the start of the day corresponding to this calculation step. |

Used by:  
- `GetTithiSequence()`  
- `GetNityaYogaSequence()`  

---

#### 🪩 `PlanetParameters`
Stores calculated astrological parameters for a planet at a given moment.

| Field | Type | Description |
|--------|------|-------------|
| `CurrentSign` | int | Zodiac sign number (1–12). |
| `CurrentNakshatra` | int | Nakshatra number (1–27) derived from the planet’s longitude. |
| `CurrentPada` | int | Pada number (1–4) within the current Nakshatra. |
| `RetrogradeStatus` | string | Retrograde motion state: `"R"` (retrograde), `"D"` (direct), `"S"` (stationary). |

Used by:  
- `GetPlanetPositions()`  
- `GetMrityuBhagaPeriods()`  
- `GetTithiSequence()`  

---

#### 🪙 `EPlanet` (Enum)
Defines internal identifiers for celestial bodies used in PADMA.  
Maps directly to the `PLANET` table in the database.

| Enum | Code | Description |
|-------|------|-------------|
| `Sun` | `"SUN"` | The Sun |
| `Moon` | `"MOON"` | The Moon |
| `Mars` | `"MARS"` | Mars |
| `Mercury` | `"MERCURY"` | Mercury |
| `Jupiter` | `"JUPITER"` | Jupiter |
| `Venus` | `"VENUS"` | Venus |
| `Saturn` | `"SATURN"` | Saturn |
| `Rahu` | `"RAHU"` | North Node |
| `Ketu` | `"KETU"` | South Node |
| ... | ... | (others as defined in DB) |


### 🔧 Planet ID Mapping

SwissService operates with two distinct but related identifiers for planets:

1. **PlanetId** — Internal PADMA identifier  
   - Matches IDs from the `EPlanet` enum and the `PLANET` table in the PADMA database.  
   - Used throughout PADMA logic and data caching.  
   - Represents the logical planet used in calculations and UI.  

2. **SwissPlanetConst** — Swiss Ephemeris constant  
   - Numeric identifier defined in the Swiss Ephemeris library (`SweConst` class).  
   - Used only internally during Swiss Ephemeris calls.  
   - Derived from `PlanetId` via a utility mapping.

Before performing calculations, `PlanetId` is converted into its Swiss Ephemeris equivalent using:

```csharp
SwissUtility.GetPlanetSWEConstByPlanetId(int planetId)
```

Example implementation:
```csharp
public static int GetPlanetSWEConstByPlanetId(int planetId)
{
    return planetId switch
    {
        1 => SweConst.SE_SUN,
        2 => SweConst.SE_MOON,
        3 => SweConst.SE_MARS,
        4 => SweConst.SE_MERCURY,
        5 => SweConst.SE_JUPITER,
        6 => SweConst.SE_VENUS,
        7 => SweConst.SE_SATURN,
        8 => SweConst.SE_MEAN_NODE,   // Rahu (Mean)
        10 => SweConst.SE_TRUE_NODE,  // Rahu (True)
        // 9 and 11 correspond to Ketu (Mean / True) — computed as 180° opposite of Rahu
        _ => -1
    };
}
```

In cases where **Ketu** is required:
```csharp
public static double AdjustForKetu(double rahuLongitude)
{
    // Ketu = Rahu + 180°, normalized to 360°
    double ketuLongitude = rahuLongitude + 180.0;
    if (ketuLongitude >= 360.0)
        ketuLongitude -= 360.0;
    return ketuLongitude;
}
```

---

## 🪐 Mapping Table

| EPlanet ID | Planet | Swiss Ephemeris Constant | Notes |
|-------------|---------|--------------------------|--------|
| 1 | Sun | `SE_SUN` |  |
| 2 | Moon | `SE_MOON` |  |
| 3 | Mars | `SE_MARS` |  |
| 4 | Mercury | `SE_MERCURY` |  |
| 5 | Jupiter | `SE_JUPITER` |  |
| 6 | Venus | `SE_VENUS` |  |
| 7 | Saturn | `SE_SATURN` |  |
| 8 | Rahu (Mean Node) | `SE_MEAN_NODE` | Direct Swiss Ephemeris calculation |
| 9 | Ketu (Mean Node) | — | Derived as Rahu + 180° |
| 10 | Rahu (True Node) | `SE_TRUE_NODE` | Direct Swiss Ephemeris calculation |
| 11 | Ketu (True Node) | — | Derived as Rahu + 180° |

---

**Notes:**  
- `PlanetId` represents PADMA’s internal model and database linkage.  
- `SwissPlanetConst` is generated dynamically and never stored in the database.  
- Ketu (both Mean and True) is not directly computed — its position is derived geometrically as the opposite point of Rahu.  
- This approach ensures consistent and efficient calculations, aligning with both **Jyotish tradition** and **Swiss Ephemeris standards**.

---

### ⚙️ Constants & Flags

SwissService uses predefined constants from the SwissEphNet library (`SweConst`).  
Typical defaults:

| Constant | Description |
|-----------|-------------|
| `SEFLG_SWIEPH` | Use Swiss Ephemeris computation mode. |
| `SEFLG_SPEED` | Include planet speed in results. |
| `SEFLG_SIDEREAL` | Enable sidereal zodiac. |
| `SE_SIDM_LAHIRI` | Use Lahiri ayanamsha. |
| `SE_GREG_CAL` | Gregorian calendar mode. |

Custom constants are not duplicated — the service uses SwissEphNet definitions directly.

---

### 🧮 Planned Functions (Implementation Phase 1)

| Function | Description | Status |
|-----------|--------------|--------|
| `GetPlanetPositions()` | Returns planetary coordinates (longitude, latitude, speed, distance). | ✅ Planned |
| `GetTithiSequence()` | Calculates lunar day (Tithi) transitions. | ✅ Planned |
| `GetNityaYogaSequence()` | Calculates Nitya Yoga sequence for the given date range. | ✅ Planned |
| `GetMrityuBhagaPeriods()` | Detects critical degree periods for each planet. | ✅ Planned |
| `GetEclipses()` | Computes solar and lunar eclipses. | ⚙️ Planned |
| `GetAscendant()` | Calculates ascendant and house cusps. | ✅ Planned |
| `GetSunriseSunset()` | Calculates sunrise and sunset times (requires validation). | ⚠️ Under review |

---

### 🕒 Time Handling Policy

- All internal calculations use **UTC (GMT-0)** as the base reference.  
- Conversion to local time happens only in the UI layer.  
- Time zone and DST adjustments use `.NET TimeZoneInfo` APIs.  
- This ensures cross-platform accuracy and prevents offset drift errors.  

---

## 📘 Naming Recommendations

To ensure clear, consistent English naming and cross-platform maintainability,  
the following conventions apply to all `SwissService` structures and methods.

| Old Name | New Name | Notes |
|-----------|-----------|-------|
| `EpheParameters` | `SwissParameters` | Unified calculation input model |
| `EpheResults` | `SwissResult` | Standard calculation result model |
| `CurrentZnak` | `CurrentSign` | "Sign" = Zodiac sign |
| `CurrentRetro` | `RetrogradeStatus` | Textual retrograde status |
| `Znak` | `Sign` | in result models |
| `Degree` | `PlanetLongitude` | Explicit meaning |
| `DegreeFrom` | `StartDegree` | Clarified naming |
| `DegreeTo` | `EndDegree` | Clarified naming |
| `IsCalcFrom` | `IsForwardCalculation` | Direction flag |
| `DateInSeconds` | `UtcSecondsOfDay` | Time relative to UTC |
| `DateTimeUtc` | `UtcDateTime` | C# idiomatic form |
| `DateNotFound` | `IsCalculationFailed` | Boolean-friendly |
| `RetrogradeStatus` | `IsRetrograde` (optional bool form) | For flags or visual indicators |
| `CalcResults` | `CalculationValues` | Clearer and descriptive |

**Method naming guidelines:**
- Use PascalCase verbs (`Get`, `Calculate`, `Convert`).  
- Avoid suffixes tied to location (e.g., `_London`).  
- Example methods:
  - `GetPlanetPositions(SwissParameters parameters)`
  - `CalculateTithiSequence(DateTime startDateUtc, DateTime endDateUtc)`
  - `GetMrityuBhagaPeriods(SwissParameters parameters)`
  - `GetEclipses(DateTime startDateUtc, DateTime endDateUtc)`
  - `GetAscendant(SwissParameters parameters)`
  - `GetSunriseSunset(SwissParameters parameters)`

### 🧩 Future iOS Support (Planned)

The Swiss Ephemeris integration is currently implemented for:
- **Windows** — via `swedll64.dll`
- **Android** — via `libswe.so` (custom-built from source)

#### iOS version (pending)
For iOS, a static library (`libswe.a`) will be required, since iOS does not permit dynamic linking of `.dll` or `.so` files.  
This library can be built later using either:
- **macOS + Xcode toolchain**, via `clang` and `ar`, or  
- **GitHub Actions**, using a macOS build runner to produce `libswe.a` automatically.

When ready, it will be added to:
`Platforms/iOS/libs/libswe.a`


and referenced in `PADMA.csproj` as:

```xml
<ItemGroup Condition="'$(TargetFramework)' == 'net9.0-ios'">
  <NativeReference Include="Platforms/iOS/libs/libswe.a">
    <Kind>Static</Kind>
  </NativeReference>
</ItemGroup>
```

Once integrated, the SwissService initialization logic will extend with a new conditional branch:
```
#elif IOS
    string path = NSBundle.MainBundle.BundlePath + "/ephe";
    SwissEphemerisNative.swe_set_ephe_path(path);
```
### ✅ Until then, the iOS build will reuse the same managed code,
and only require addition of the native library at the final release stage.


---

> _End of PADMA requirements document_
