# 🪶 PADMA — Project Requirements & Current Implementation  

---

## 🗓️ Overview  

**PADMA** — cross-platform application built with **.NET9 / MAUI ** and **SQLite (sqlite-net-pcl)**.  
It displays a localized astrological calendar, user configuration pages, and other utilities.  
All settings, interface texts, and reference data are stored in the embedded SQLite database  
**`PADMADB.db3`**, which is cached in memory at runtime.

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

### 🔸 ServiceLocator  

**Purpose:**  
Provides a simple global entry point for dependency-injected services (e.g. `DatabaseService`).  

**Usage example:**
```csharp
var db = ServiceLocator.Services.GetService<DatabaseService>();
```

------

## 🧩 UI Templates & Layout Standards  

The following templates define visual and structural consistency across all pages.

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

### 🔹 ConfigurationPage  

**Purpose:**  
Acts as a hub for accessing all configuration pages.  

**Layout:**
- Inherits from `ContentPage`.
- Localized title `"Settings"`.
- Vertical list of localized navigation buttons:
  - `Language`
  - `First day of week`
  - `Planetary transits`
  - `Nodes (Rahu and Ketu)`
  - `Hora`
  - `30 Muhurtas (60 Ghati)`
  - `Mrityu Bhaga` 
  - `Sunrise calculation`
  - `Notification`
  - `Color settings`
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

### 🔹 MainPage  

**Purpose:**  
Main calendar view of the application.  

**Layout:**
- Toolbar — localized month title and navigation buttons (`left_arrow.png`, `right_arrow.png`).
- Weekday header row — localized 3-letter abbreviations.
- Main grid — 6×7 bordered day cells.  
  Each cell includes:
  - Day number (top-left).
  - 6 colored bars.

**Behavior:**
- Loads current language and first-day-of-week from cache.
- Reacts to `"SettingsChanged"` messages.
- Rebuilds layout when:
  - Language changes,
  - First day of week changes,
  - Month navigation occurs.
- Uses `ReloadCultureAndRefresh()` for culture updates.
- Title capitalization follows current culture (`ToTitleCase()`).

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

### 🔹 Future Reuse  

- All new configuration pages must inherit from `ConfigBasePage`.  
- Non-configuration pages (e.g. reports or charts) must follow the same style and spacing.  
- Common color and typography palette must remain consistent.

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

### 🔄 Global Behavior  

**Shared logic across pages:**
- All configuration pages trigger `"SettingsChanged"` via `MessagingCenter` after saving changes.
- `ConfigurationPage` listens for these messages and refreshes texts.
- `MainPage` listens for the same message and rebuilds its layout.
- If user navigates back without changes — no message or refresh occurs.
- Localization applies dynamically to all visible elements on appearance.

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

## 🧩 Architecture Notes

This section summarizes key technical and behavioral conventions that define PADMA’s internal consistency across all MAUI components.

### 🔹 Unified Messaging Contract  
All configuration pages communicate updates using a single `MessagingCenter` event pattern:

```csharp
MessagingCenter.Send<object>(this, "SettingsChanged");
MessagingCenter.Subscribe<object>(this, "SettingsChanged", async _ => { ... });
```
- Ensures consistent message delivery regardless of page type.  
- Prevents common MAUI issues with mismatched sender types.  
- Allows `ConfigurationPage` to listen universally to all child updates.

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

### 🔹 Centralized Cache Refresh  
`DataCache.Instance.Refresh(db)` is invoked **only after confirmed configuration updates**.  
This avoids unnecessary reloads and ensures the user immediately sees updated texts, settings, or localization changes.

### 🔹 Defensive UI Updates  
`MainPage` and `ConfigurationPage` both use internal flags (e.g. `_hasConfigChanges`) to determine whether UI refreshes are required after returning from a configuration page.

- If no settings were changed, navigation returns instantly without rebuilding the calendar.  
- If changes exist, the calendar and localized interface are refreshed.  
This optimization significantly improves perceived performance on all platforms.

### 🔹 Localization Flow  
All text localization uses:
```csharp
Localization.GetLocalizedText("Native English Text", DataCache.Instance.CurrentLanguageCode);
```

- English text entries are **mandatory** in `APP_TEXTS` (as base keys).  
- Each localized record must include English, Ukrainian, Polish, and Russian variants.  
- Dynamic UI elements (titles, labels, buttons) must have `x:Name` assigned for runtime localization updates.

### 🔹 Database Versioning  
The table `APP_META` stores database version info.  
On app startup, `DatabaseService` compares the deployed and local DB versions and automatically replaces outdated copies from `/Resources/Raw/PADMADB.db3`.  
This guarantees schema and localization updates propagate without manual intervention.

### 🔹 Extension Methods  
Utility extensions defined in `PADMA/Core/Utilities/Extensions.cs` provide reusable helpers for date/time operations:

```csharp
date.Between(start, end);
date.StrictBetween(start, end);
date.ShiftByUtcOffset(offset);
date.ShiftByDaylightDelta(adjustmentRules);
```
These methods standardize temporal logic across astronomical and calendar-related calculations.

## 👤 Profiles feature

### 🎯 Purpose  

The **Profiles** feature stores personal data for individual users  
(name, surname, birth date, birth and living locations).  
It enables PADMA to perform automatic calculations and display  
personalized data (e.g., location-based information) upon app startup.  

Each profile may be marked as **default**,  
which determines which profile data is automatically used by the **Calendar** page.  

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

## 🧭 Profiles — Navigation & UI Behavior  

### 🧱 Overview  

This section defines the navigation flow, toolbar layout, and user interaction logic  
for all pages within the **Profiles** feature.  
It ensures full consistency with the existing PADMA user interface conventions  
used by **ConfigurationPage** and other modules.  

The Profiles module manages user profiles and defines how profile data is selected, activated,
and used by the Calendar and calculation engine.

The design clearly separates three different concepts:
- Selected profile — temporarily selected in the list UI.
- Active profile — currently used by the Calendar and calculations.
- Default profile — profile automatically loaded on app startup.

This separation avoids implicit side effects and makes profile behavior predictable.

### 🏠 ProfilesPage — Main Hub  

**Toolbar layout:**  
```
☰  Profiles                              ❌
```
**Action Bar (below toolbar):**  
```
[➕ Add new profile]
```

**Profiles list:
- Displays all profiles.
- Default profile is marked with a ⭐ icon in the left column.
- Tap on a profile:
	- Selects the profile (visual highlight).
	- Does not navigate or trigger calculations.
- Re-tapping an already selected profile removes selection.
- Tap “Add new profile” → opens `ProfileDetailPage` in **Create mode**.  
- ❌ Close → returns to **MainPage**.  
- On return from a child page, the list refreshes to reflect changes

## 🔽 Context Action Panel (Bottom)

Appears only when a profile is selected.
```
[ Details ]   [ Set default ]   [ Choose ]
```
### Button behavior:

| Button          | Behavior                                                                                                                            |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| **Details**     | Opens `ProfileDetailPage` in **View mode** for the selected profile.                                                                |
| **Set default** | Marks the selected profile as default (`CHECKED = 1`). Disabled if the profile is already default.                               	|
| **Choose**      | Makes the selected profile the **active profile** and returns to `MainPage`. Disabled if the selected profile is already active. 	|

**Visibility rules:
- The action panel is hidden when:
	- no profile is selected,
	- Choose is executed,
	- Cancel / Close (❌) is executed.
- The panel remains visible after Set default, allowing further actions.

### 👤 ProfileDetailPage — Profile Card  

**Toolbar layout:**  
```
←  [Profile name / New profile]           ❌
```
**Action Bar (below toolbar):**  
```
[ 💾 Save ] [ ✏️ Edit ] [ 🗑 Delete ]
```
**Button order confirmed:** Save first.

#### 🔹 Modes of operation  

| Mode | Description |
|------|--------------|
| **View** | All input fields are read-only. Only action buttons are active. |
| **Edit** | All input fields become editable, including name, surname, date, and locations. |
| **New** | Empty form, all fields editable, title = “New profile”. |

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

#### 🔹 Button behavior  

| Button | Action | Confirmation |
|---------|---------|---------------|
| 💾 **Save** | Writes changes to `PROFILE` and new `LOCATION` entries if needed. Updates cache and returns to `ProfilesPage`. | ✅ “Save changes to profile?” |
| ✏️ **Edit** | Enables editable mode for all fields. | — |
| 🗑 **Delete** | Deletes current profile and returns to list. | ✅ “Delete this profile?” |

The **❌ Close** icon and **← Back arrow** both trigger the same method `HandleBackAsync()`.
Both must show the same confirmation dialog sequence when unsaved changes exist.  
If no changes exist, they immediately return to the profiles list without prompts. 

### 🔹 Confirmation Dialogs and Validation Rules
| Event | Dialog | Conditions |
|--------|---------|-------------|
| Leaving the page with unsaved changes | “Do you want to save changes before exit?” | Triggered if `HasRealChanges()` returns true. |
| Manual save via 💾 button | “Save changes to profile?” | Always displayed before database write. |
| Successful save | “Profile saved successfully.” | Shown after profile insert or update succeeds. |
| Save error | “Failed to save profile. Please try again.” | Shown when a database or validation exception occurs. |
| Validation alerts | Localized messages per field | Required fields: **Profile Name**, **Date of Birth**, **Place of Birth**, **Place of Living** |

Dialogs follow the same visual and logical pattern as those used in `ConfigBasePage`.  

### 🌍 LocationPage — Location Lookup  

**Purpose:**  
Searches and selects geographic locations using **Nominatim API**. 
Hybrid search using local DB first, Nominatim as fallback, optional country filter via popup. 

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

## Key Points

- Country selection popup
- Normalized columns: LOCALITY_NORM, REGION_NORM, STATE_NORM, COUNTRY_NORM
- Local DB search uses normalized columns
- External search uses Nominatim with countrycodes and limit=20
- Offline-first behavior

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

### 🔹 Behavior When Returning from Location Search
- `MessagingCenter` sends `"LocationSelected"` with `(Mode, AppLocation)` payload.  
- The appropriate field (`Place of Birth` or `Place of Living`) updates immediately.  
- `_snapshotJson` is **not refreshed** after location selection — this ensures the “unsaved changes” dialog remains functional.  
- The `_skipSnapshotOnce` flag prevents snapshot regeneration upon re-entering the page.

### 🔄 Profile Selection & Activation Flow

Choosing a profile:
1. User selects a profile in ProfilesPage.
2. Presses Choose.
3. Application:
	- Updates DataCache.ActiveProfile.
	- Reloads LocationList cache.
	- Rebuilds ProfileContextService.
	- Sends "ProfileChanged" message.
4. Navigates back to MainPage.
5. Calendar recalculates and redraws using the new active profile.

**Important rules:
- Selecting a profile does not trigger calculations.
- Calculations occur only after pressing Choose.
- Profiles without a valid birth location cannot be chosen.

### ⭐ Default Profile Logic

- Only one profile may be marked as default.
- Default profile:
	- Is loaded automatically on app startup.
	- Does not automatically become active during runtime.
- Default flag affects startup behavior only.

### ❌ Exit & Cancel Behavior

- ❌ Close icon:
	- Clears selection.
	- Hides action panel.
	- Returns to MainPage.
- No profile state is changed on cancel.

### ⚙️ Integration rules  

- Navigation consistency follows the same convention as configuration pages.  
- Confirm dialogs are reused from existing shared logic.  
- Changes in profiles triggers `"ProfileChanged"` messaging events,  
  to notify the Calendar or other pages of an active profile switch.  
- Data saving logic ensures that new locations are committed only when  
  a profile save operation is confirmed.  
- On app startup, **DataCache** loads the current default profile  
  (`CHECKED = 1`) together with cached settings, texts, and references.  
- Default profile data will later be used by **Calendar** or  
  other computational modules.  
- Changes in profiles do **not** send `"SettingsChanged"` messages,  
  unless language or calendar configuration are directly affected.  

### 🌐 Localization  

All pages in the Profiles feature follow the same localization contract  
as the rest of PADMA:  
```csharp
Localization.GetLocalizedText("Native English Text", DataCache.Instance.CurrentLanguageCode);
```

All UI text entries must exist in the `APP_TEXTS` table  
with four translations: **English, Ukrainian, Polish, Russian**.  

### 🎯 UX Summary

- Profile selection is explicit and reversible.
- No hidden recalculations.
- Default profile logic is predictable.
- Profile activation is intentional and user-controlled.

This architecture ensures clarity, stability, and future extensibility
(e.g., natal chart pages, profile-based reports).

### 🗺️ Nominatim (OpenStreetMap)

To find GPS coordinates for locations the Nominatim API are used
🔗 https://nominatim.org/release-docs/latest/api/Search/

------

# 🌠 Swiss Module — SwissService, SwissAnalysis, SwissUtility, SweConst

**Purpose:**  
Implements high-precision astronomical and astrological computations using the Swiss Ephemeris native library.
All calculations are performed in **UTC (GMT-0) ** and the **sidereal Lahiri** mode by default.
This module provides the foundation for Tithi, Nitya Yoga, Mrityu Bhaga, and Eclipse analyses used in PADMA.

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Analysis/SwissAnalysis.cs` | High-level astrological analysis and event detection (transitions, yoga, eclipses) |
| `Core/Services/SwissService.cs` | Low-level computation services wrapping native Swiss Ephemeris |
| `Core/Utilities/SwissUtility.cs` | Utility helpers for normalization, mapping, and conversions |
| `Core/Native/SwissEphemerisNative.cs` | P/Invoke bindings to native SWE API functions |
| `Core/Models/SwissParameters.cs` | Input parameters for computation |
| `Core/Models/SwissResult.cs` | Output structure with planetary results |
| `Core/Constants/SweConst.cs` | Constants and flags mirroring Swiss Ephemeris definitions |

## ⚙️ Initialization & Platform Integration

- Ephemeris data files (`*.se1`) are stored inside `/Resources/Raw/ephe.zip`.
- On first run, the archive is extracted to `FileSystem.AppDataDirectory/ephe/`.
- SwissService automatically sets the path and sidereal mode (Lahiri).
- Platform-specific paths:  
  - **Windows:** via `swedll64.dll` with `AppContext.BaseDirectory/Resources/Raw/ephe/`  
  - **Android:** via `libswe.so` (custom-built from source) with archived `ephe.zip` in `AppContext.BaseDirectory/Resources/Raw/`  
  - **iOS:** to be integrated later with static `libswe.a` (planned)

## 🧮 Implemented Calculations

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

**Notes:**  
- `PlanetId` represents PADMA’s internal model and database linkage.  
- `SwissPlanetConst` is generated dynamically and never stored in the database.  
- Ketu (both Mean and True) is not directly computed — its position is derived geometrically as the opposite point of Rahu.  
- This approach ensures consistent and efficient calculations, aligning with both **Jyotish tradition** and **Swiss Ephemeris standards**.

### 🪐 Planet Positions

`GetPlanetPosition()` — computes geocentric longitude, latitude, distance, and speed for any planet.

- Uses Swiss Ephemeris (`swe_calc_ut`)
- Returns `SwissResult` object with:
  - `CalculationValues[6]`
  - `Sign`, `IsRetrograde`, `UtcSecondsOfDay`

**Retrograde detection:**  
`IsRetrograde = (speed < 0)`

### 🌗 Tithi Calculation

`CalculateTithiDataList_London(DateTime startUtc, DateTime endUtc)`

Computes the sequence of lunar day transitions within a range.

Formula:
```
Tithi = floor( Normalize(moonLon - sunLon) / 12° ) + 1
```
- Range: 1–30  
- Each Tithi corresponds to 12° separation between Sun and Moon.  
- Output: List of (TithiIndex, StartUtc, EndUtc)

### 🌞 Nitya Yoga Calculation

`CalculateNityaYogaDataList_London(DateTime startUtc, DateTime endUtc)`

Formula:
```
YogaAngle = Normalize(moonLon + sunLon)
YogaIndex = floor( YogaAngle / 13°20′ ) + 1
```
- Range: 1–27  
- Uses `NityaYogaTithiResults` (paired Sun/Moon positions).  
- Each yoga occupies a 13°20′ arc (13.3333°).

### ☠️ Mrityu Bhaga Detection Engine

## Overview

Mrityu Bhaga detection identifies time intervals when a planet’s sidereal longitude falls into its predefined “critical / danger” degree zone (Mrityu Bhaga) for the current zodiac sign.

The engine reuses:
* Swiss Ephemeris sidereal positions (Lahiri)
* Reference Mrityu Bhaga degree table loaded in DataCache (MRITYUBHAGA)
* Existing PADMA time handling conventions (UTC internally, local only for UI)

It produces **intervals** (periods), not just flags.

## Inputs

Function:
```
CalculateMrityuBhagaDataList_London(
* planetId,
* fromUtc,
* toUtc,
* nodeType)
``` 

Data sources:
* DataCache.Instance.MrityuBhagaList (table MRITYUBHAGA)
  * PlanetId
  * ZodiacId
  * Degree (sidereal, 0–360)
* Active system setting: Mrityu Bhaga mode
  * MRITYUBHAGANEQUAL
  * MRITYUBHAGANLESS
  * MRITYUBHAGANMORE
  * MRITYUBHAGAERNST

## Zone Definition (per active setting)

For each planet and current zodiac sign, the critical zone is built from the reference degree `D` and tolerance `tol`:
* MRITYUBHAGANEQUAL: [D - tol, D + tol]
* MRITYUBHAGANLESS:  [D - tol, D]
* MRITYUBHAGANMORE:  [D, D + tol]
* MRITYUBHAGAERNST:  [D - tol, D + tol]

Tolerance values are defined by settings (current implementation):
* EQUAL: 0.5°
* LESS/MORE/ERNST: 1.0°

Zone comparison uses normalized degrees and supports wrap-around over 0°.

## Core Algorithm

For the requested window (fromUtc..toUtc):

1. Iterate time samples and compute planet sidereal longitude and retrograde flag.
2. Resolve current zodiac sign by longitude.
3. Load Mrityu Bhaga reference record for (planetId, zodiacId).
4. Build zone boundaries (fromDeg..toDeg) per active setting.
5. Determine `inside = IsWithinDegrees(lon, fromDeg, toDeg)`.
6. Detect transitions:
   * Entry: inside == true after being outside
   * Exit: inside == false after being inside
7. Record intervals as MrityuBhagaData objects:
   * DateFromUtc
   * DateToUtc
   * PlanetId
   * ZodiacId
   * Degree
   * MrityuBhagaSetting
   * LongitudeFrom / LongitudeTo

## Real Boundary Expansion (No Window Clipping)

The engine MUST return **real** interval boundaries and must not clip periods to the requested window:
* If the planet is already inside the Mrityu Bhaga zone at fromUtc:
  * Expand backward in time until the last moment outside the zone.
  * Refine the entry moment to obtain a real DateFromUtc.

* If the planet is still inside the zone at toUtc:
  * Expand forward in time until the first moment outside the zone.
  * Refine the exit moment to obtain a real DateToUtc.

This prevents artificial truncation and supports cases where a Mrityu Bhaga period starts before or ends after the monthly calculation window.
Protective limits are used for expansion (e.g., up to N days backward/forward).

## Sampling Strategy

To avoid missing short passages through the zone (especially for fast planets), the engine uses adaptive sampling:

* Outside zone: coarse step (recommended: ~15 minutes)
* Inside zone: fine step (~1 minute)

Optional refinement:

* When an entry/exit is detected between two samples, a binary search refinement may be used to improve boundary precision.

## Time Handling

* All calculations and stored interval boundaries are in UTC.
* UI layers convert to profile local time for display.

## Output
```
List<MrityuBhagaData>
```
Properties:
* Chronologically ordered
* Non-overlapping for the same planet/sign segment
* Real (expanded) boundaries; not clipped by fromUtc/toUtc

## Consumers

Mrityu Bhaga intervals are intended for later UI and monthly views, including:
* Planetary monthly transit overview
* Warning markers / special annotations

## Status

Implemented.
Boundary expansion logic ensures correct intervals for both slow and fast planets without clipping to calculation window.

### 🌑 Eclipse Computation

Implements both **lunar** and **solar** eclipse search.

- `CalculateEclipses_London(DateTime fromUtc, DateTime toUtc)` → the list of eclipses (UTC time) within a range (both - lunar and solar).  

Returned structure `EclipseInfo`:
```
Type (Partial, Total, Annular, Hybrid, Penumbral)
BeginUtc
MaximumUtc
EndUtc
Magnitude
```
Uses Swiss Ephemeris functions:
- `swe_lun_eclipse_when`
- `swe_sol_eclipse_when_glob`

### 🧩 Higher-Level Analysis (SwissAnalysis)

`CalculatePlanetDataList_London(startUtc, endUtc)` —
builds list of `PlanetData` entries for all transitions.

Features:
- 1-hour stepping (`3600s`)
- Detects changes in **Sign**, **Nakshatra**, **Pada**, **Retrograde**
- Performs binary search via `FindTransitionEpoch()` for exact UTC time
- Default coordinates: London (`Lon = -0.17, Lat = 51.5`)

-------

## 🧱 Data Models

### `SwissParameters`
| Field | Type | Description |
|--------|------|-------------|
| PlanetId | int | Internal PADMA ID |
| PlanetCode | string | Identifier (e.g. "SUN") |
| Longitude | double | Geographic longitude |
| Latitude | double | Geographic latitude |
| Altitude | double | Altitude in meters |
| UtcDateTime | DateTime | UTC moment of calculation |

### `SwissResult for Planet transits calculations`
| Field | Type | Description |
|--------|------|-------------|
| CalculationValues | double[6] | Raw Swiss Ephemeris result |
| Sign | int | Zodiac sign 1–12 |
| IsRetrograde | bool | Motion flag |
| UtcSecondsOfDay | int | Seconds since UTC midnight |
| IsCalculationFailed | bool | Error flag |

## 🧰 Utilities

`SwissUtility` provides:
- `NormalizeDegrees(double)` — ensures [0,360]
- `GetPlanetSWEConstByPlanetId(int)` — mapping PADMA → Swiss constants
- `AdjustForKetu(double)` — adds 180°, wraps to 360°
- `GetZodiakIdFromDegree()`, `GetNakshatraIdFromDegree()`, `GetPadaIdFromDegree()`
- `GetNavamsaByNakshatraAndPada()` — database lookup via `DataCache.Padas`

## 🧠 Constants (`SweConst`)

Main constants used:
| Name | Description |
|------|--------------|
| `SEFLG_SWIEPH` | Swiss Ephemeris mode |
| `SEFLG_SPEED` | Include planetary speed |
| `SEFLG_SIDEREAL` | Sidereal zodiac |
| `SE_SIDM_LAHIRI` | Lahiri Ayanamsha |
| `SE_GREG_CAL` | Gregorian calendar |

Also defines IDs for:
- Planets (`SE_SUN`..`SE_SATURN`, `SE_MEAN_NODE`, `SE_TRUE_NODE`)
- Flags for eclipse modes and computation masks.

## 🕒 Time Handling

- All computations are in **UTC (GMT+0, London coordinates)**.
- UI layer performs local time conversions.
- `.NET TimeZoneInfo` and `AdjustmentRules` handle DST.  
- Date/time strings stored in DB as `"yyyy-MM-dd HH:mm:ss"`.

# 🌄 Ascendant Calculation — SwissService & SwissUtility

## 📘 Overview

The Ascendant (Lagna) calculation feature has been implemented using the Swiss Ephemeris engine integrated through the SwissService.  
This module computes the **Ascendant longitude** for any date/time and geographic location, including proper handling of **historical time zones**.

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Services/SwissService.cs` | Contains the low-level calculation `CalculateAscendantForDate` (core Ascendant computation in UTC). |
| `Core/Utilities/SwissUtility.cs` | Provides `CalculateAscendantWithTimeZone` for high-level usage including local time zone conversion. |
| `Core/Services/TimeZoneService.cs` | Handles historical timezone detection using GeoTimeZone, TimeZoneConverter, and NodaTime. |

## ⚙️ External Libraries

| Package | Version | Purpose |
|----------|----------|----------|
| `GeoTimeZone` | 6.1.0 | Determines IANA timezone ID by latitude/longitude (offline). |
| `TimeZoneConverter` | 7.2.0 | Converts between IANA and Windows (.NET) timezone formats. |
| `NodaTime` | 3.2.2 | Provides historical timezone offsets and date-time conversions. |

## 🧩 Calculation Flow

### ️⃣  Ascendant Core Calculation (SwissService)

Method:  
```csharp
public static double CalculateAscendantForDate(
    DateTime dateTimeUtc,
    double latitude,
    double longitude,
    double altitude,
    char hsys = 'O')
```
- Inputs are in **UTC**.
- Performs conversion to Julian Day (`swe_julday`).
- Activates sidereal Lahiri mode (`swe_set_sid_mode`).
- Sets topocentric coordinates (`swe_set_topo`).
- Calls Swiss Ephemeris native function `swe_houses_ex()`.

Result:  
- Returns **Ascendant ecliptic longitude** in degrees [0–360].
- Default house system: **‘O’ (Placidus)**.

### ️⃣  Ascendant with TimeZone Adjustment (SwissUtility)

Method:  
```csharp
public static double CalculateAscendantWithTimeZone(
    DateTime dateUtc,
    double latitude,
    double longitude,
    double altitude,
    char hsys = 'O')
```
- Uses `TimeZoneService` to get historical UTC offset for the coordinates.
- Converts to local time via NodaTime’s `DateTimeZoneProviders.Tzdb`.
- Calls `CalculateAscendantForDate` with the corrected UTC time.
- Returns Ascendant longitude (sidereal, Lahiri).

## 🕒 Historical Time Zone Logic

### TimeZoneService Methods

| Method | Description |
|---------|--------------|
| `GetIanaTimeZoneId(lat, lon)` | Returns IANA zone ID (e.g., "Europe/Kyiv"). |
| `GetDotNetTimeZoneId(lat, lon)` | Returns equivalent Windows ID. |
| `GetUtcOffsetHours(date, lat, lon)` | Returns UTC offset (historical) in hours using NodaTime tzdb. |

## 🔍 Notes
- Calculation fully respects historical DST and UTC offsets.  
- Works identically on Windows, Android, and iOS.  
- Uses sidereal mode **Lahiri** by default.  
- Returns absolute ecliptic longitude (0–360°), compatible with all PADMA models.  
- Formatting into degrees/minutes/seconds handled in `FormatDegrees(double degrees)` function (`Core/Utilities/SwissUtility.cs`).

-------

### 🌅 Sunrise and Sunset Calculation

#### **Purpose**
This module calculates the sunrise and sunset times for a given geographic location and date, respecting user-defined configuration (calculation type: by disc edge or by disc center).

#### **Main Files**
- `SwissService.cs` — functions to calculate sunrise and sunset times in UTC.
- `SwissEphemerisNative.cs` — P/Invoke declaration for the `swe_rise_trans` function.
- `SweConst.cs` — contains constant definitions used for rise/set calculations (`SE_SUNRISE_TIP`, `SE_SUNRISE_CENTER`, `SE_SUNSET_TIP`, `SE_SUNSET_CENTER`).
- `TimeZoneService.cs` — universal time conversion service (UTC ↔ Local) based on `.NET TimeZoneInfo` and `AdjustmentRules`.

#### **SwissService Functions**

** ️⃣  Sunrise Calculation:**
```csharp
public static DateTime CalculateSunriseForDateAndLocation(DateTime date, double latitude, double longitude, double altitude)
```
Calculates the UTC time of sunrise for a given date and coordinates.  
The calculation type is determined by the active configuration:
- `SUNRISETIP` — lower limb (disc edge),
- `SUNRISECENTER` — disc center.

** ️⃣  Sunset Calculation:**
```csharp
public static DateTime CalculateSunsetForDateAndLocation(DateTime date, double latitude, double longitude, double altitude)
```
Calculates the UTC time of sunset for a given date and coordinates.

#### **TimeZoneService and Time Conversion**

For converting UTC results to local time, the following function is used:

```csharp
public static DateTime ConvertUtcToLocalSmart(DateTime utc, double latitude, double longitude)
```

This function:
- Determines the .NET timezone based on coordinates (GeoTimeZone + TZConvert);
- Applies the base `UtcOffset` and adjusts using `AdjustmentRules`;
- Considers possible Daylight Saving Time (DST) transitions;
- Works without external libraries and is applicable for any region.

Additional helper functions in `TimeZoneService`:
- `ShiftDateByDaylightDelta()` — applies DST offset when active;
- `GetAdjustmentDate()` — computes actual transition dates for DST.

#### **Notes**
- All Swiss Ephemeris calculations are performed in UTC.  
- Conversion to local time is handled via `.NET TimeZoneInfo`, ensuring compatibility with system settings on all platforms.  
- Minor discrepancies (up to ±1 day or even more) may occur for future years due to known limitations of the Windows time zone database.  
- For historical calculations (e.g., natal charts), `NodaTime` is used — relying on the IANA time zone database for full historical accuracy.

--------

### 🧩 Transit Engine Architecture (PADMA)

#### **Purpose**
This section defines the new unified data structure for handling all astrological events (transits) within PADMA.  
The goal is to simplify, optimize, and standardize how all computed Swiss-based entities are stored and rendered in the calendar.

#### **Background**
The legacy PAD project used a `Day` class containing ~200 lists of transit-specific calendar objects (e.g., `List<TithiCalendar>`, `List<KaranaCalendar>`, etc.).  
While this approach provided structural clarity, it also caused:
- High memory overhead (due to object duplication and list allocation);
- Complex data binding for UI;
- Inefficient cloning between derived and base calendar classes;
- Difficult extension when adding new transit types.

## 1. Overview

The Transit Engine is the core analytical layer of PADMA responsible for transforming raw Swiss Ephemeris calculations into structured, chronologically ordered, semantically rich astrological intervals (“Slices”).  
This architecture replaces legacy patterns with a unified slice-based timeline model.

The Engine supports all major Vedic astrological domains:
- Planetary transits (including nodes, retrograde, nakshatra, pada, navamsa, tara bala, houses)
- Tithi
- NityaYoga
- MrityuBhaga
- Eclipses
- Sunrise / Sunset cycles

All output slices are:
- Time bounded (`StartUtc`, `EndUtc`)
- Typed (`ETransitKind`)
- Normalized through a unified interface for use in calendar views (Day, Month, Timeline)

## 2. Core Concepts

### 2.1 Slice Architecture
Every astrological phenomenon is represented as a **Slice**:
- It has a beginning and an end.
- It contains metadata specific to its type.
- It can span minutes, hours, days, or weeks.
- It is type-identifiable via `ETransitKind`.

Slices are stored in `Timeline` structures which represent the full computed period (e.g., 46 days).

### 2.2 Types of Transits (`ETransitKind`)
- **Planet**  
- **Tithi**  
- **NityaYoga**  
- **MrityuBhaga**  
- **Eclipse**  
- **Sunrise / Sunset**  
- **CustomUserTransit**
- ** etc **

This keeps the system extensible.

### 2.3 Data Sources
The Transit Engine uses:
- Swiss ephemeris raw calculations (`SwissAnalysis`)
- Cached domain dictionaries (`DataCache`)
- User profile information (birth nakshatra, node mode, location)
- Local sunset/sunrise rules (TimeZoneInfo)

## 3. Unified Transit Engine Flow

### 3.1 Steps
1. **SwissAnalysis** generates raw chronological lists:
   - PlanetData x 11
   - TithiData
   - YogaData
   - MrityuBhagaData
   - EclipseData
   - Sunrise/Sunset

2. **TransitBuilders** convert raw data into typed slices.

3. **TimelineAssembler** merges all slices:
   - Sorted by StartUtc
   - Grouped by ETransitKind if needed

4. **CalendarLayer** (-2+42+2 = 46 day grid) filters slices by day.

5. **DayPage & MainPage** consume filtered slices.

## 4. Why Slices?

### Benefits:
- Unified rendering pipeline
- No explosion of Day properties
- Easy filtering in UI
- Infinite extensibility
- Clean decoupling of “calculation” vs “interpretation”

## 5. Integration With PADMA

- All domain tables (`Nakshatra`, `Pada`, `TaraBala`, `Zodiac`, etc.) are loaded into `DataCache` at startup.
- Transit Engine reads only Ids (never names).
- All formatting (names, localized strings) happens at the UI/ViewModel layer.

## 6. Conclusion

The Transit Engine provides:
- A consistent and powerful method to compute all Vedic astrological timelines.
- A unified format for consumption in UI.
- Clear separation between Swiss calculations, business logic, cached dictionary data, and interface rendering.

--------

## 7. Planet Transit Builder

The Planet Transit Builder is responsible for converting raw Swiss ephemeris planet state samples into continuous, chronologically ordered slices (PlanetSlice) representing stable astrological states over time. 
These slices are later consumed by CalendarViewModel, DayOverviewPage, DayPage, tooltips, and Vedha calculations.

## 7.1 Input

* PlanetData list for a given planet generated by SwissAnalysis
* PlanetId (internal enum EPlanet)
* BirthMoonNakshatraId (from active profile)
* BirthZodiacId (from active profile)
* BirthLagnaId (from active profile)
* Transit calculation mode (Moon / Lagna / MoonAndLagna)

## 7.2 Time Window and Boundary Expansion

Transit builder is invoked with a requested calculation window:

* bufferStartUtc
* bufferEndUtc

To correctly support slow planets and long-lasting states, the builder MUST expand this window so that the first and last slices reflect real state boundaries rather than artificial buffer edges.

### Backward Expansion

Starting from bufferStartUtc:

* Repeatedly step backward in time
* Detect the previous moment when any tracked state field changes
* Continue until a state change is found

Result:

* realStartUtc = moment when current state begins

### Forward Expansion

Starting from bufferEndUtc:

* Repeatedly step forward in time
* Detect the next moment when any tracked state field changes
* Continue until a state change is found

Result:

* realEndUtc = moment when current state ends

All PlanetData sampling for slice building must be performed over:

realStartUtc → realEndUtc

This guarantees:

* No zero-length slices
* Correct periods for slow planets (Saturn, Jupiter, nodes, etc.)
* Accurate slice boundaries for tooltips and Vedha

## 7.3 Change Detection

A new slice begins whenever ANY of the following fields changes between two consecutive PlanetData samples:

* ZodiacId
* NakshatraId
* PadaId
* IsRetrograde

Algorithm:
Reuse SwissAnalysis.HasStateChanged(a, b)

## 7.4 Slice Construction

Slices are built by iterating PlanetData chronologically and grouping consecutive records with identical state.

Each PlanetSlice contains:

* StartUtc
* EndUtc
* PlanetId
* ZodiacId
* NakshatraId
* PadaId
* NavamsaZodiacId
* IsRetrograde
* TaraBalaId
* TaraBalaPercent
* HouseFromMoon
* HouseFromLagna
* PlanetColorFromMoon
* PlanetColorFromLagna

Slices must be strictly ordered and non-overlapping.

## 7.5 Tara Bala Logic

Dependent on:

* BirthMoonNakshatraId
* Swapped Nakshatra sequence relative to birth Moon
* 9 × 3 Tara Bala matrix

Each Nakshatra maps to:

* Column 0 → 100% (favorable)
* Column 1 → 50%
* Column 2 → 25%

TaraBalaId = rowIndex + 1

## 7.6 Navamsa Logic

Navamsa zodiac is resolved using cached Pada table:

GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber)

## 7.7 House and Planet Color Logic

Computed using:

* BirthZodiacId
* BirthLagnaId
* Swapped Zodiac sequences relative to:

  * Natal Moon
  * Lagna

Both house systems are stored inside each slice.

## 7.8 Output

For each planet:

List<PlanetSlice>

Properties:

* Chronologically ordered
* Cover full realStartUtc → realEndUtc range
* No zero-length slices

These slice lists are grouped into TransitPack:

Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>

## 7.9 Consumers

Planet Transit Builder output is used by:

* CalendarViewModel (42-day grid)
* DayOverviewPage
* DayPage timeline
* Planet tooltips
* Vedha calculations

All higher-level logic assumes slice boundaries already represent true state changes.

## 7.10 Status

Completed and stabilized.
Supports slow planets and long-duration states via boundary expansion.

-------

## 8. Nakshatra Slice — Transit Engine Specification (PADMA)

### 8.1. Overview

NakshatraSlice represents the interval during which the Moon remains inside a specific
nakshatra sector (13°20'). It is derived from SwissAnalysis → PlanetData (Moon only).
Slices are built by detecting changes in NakshatraId over time.

This slice carries only identifiers. Descriptive text and attributes are resolved from
NAKSHATRA_DESC via DataCache.

### 8.2. Input Data (SwissAnalysis → PlanetData for Moon)

Relevant PlanetData fields:

- DateTimeUtc
- NakshatraId (1..27)

SwissAnalysis provides a chronological list for the Moon with exact transition moments.

### 8.3. DataCache Tables Used

### NAKSHATRA
- ID (1..27)
- NAKSHATRACODE (string/enum-friendly)
- COLORID (FK → COLOR)

### NAKSHATRA_DESC
Localized descriptive fields:
NAME, SHORTNAME, RULER, NATURE, DESCRIPTION, GOODFOR, BADFOR, LANGUAGECODE.

The slice does not store this – UI retrieves text through DataCache.

### 8.4. NakshatraSlice Model

public class NakshatraSlice : CalendarSlice

Notes:
- NakshatraCode is (ENakshatra)NakshatraId.
- ColorId is resolved from NAKSHATRA table.

### 8.5. NakshatraTransitBuilder

public static class NakshatraTransitBuilder

Logic Summary:
- Input list must be the Moon-only PlanetData sequence.
- A new slice is created whenever NakshatraId changes.
- EndUtc is defined as next transition (or +1 day fallback).
- Color and enum code are resolved from DataCache.

### 8.6. UI Interaction

UI retrieves descriptions via:
cache.GetNakshatraDesc(nakshatraId)
NakshatraSlice remains minimal and computation-oriented.

### 8.7 Output
List<NakshatraSlice> chronologically ordered.

### 8.8. Status: Completed

--------

## 9. Tara-Bala Slice — Transit Engine Specification (PADMA)

### 9.1. Overview

Tara-Bala Slice represents the quality of the Moon's current nakshatra relative to the 
native's birth nakshatra. It is derived from:
- NakshatraSlice (current Moon nakshatra transitions)
- BirthMoonNakshatraId (from user profile)
- Tara-Bala matrix (9×3 logic)

This slice contains only identifiers and computed values. All descriptive text (name, 
shortname, description) comes from TARABALA_DESC via DataCache.

### 9.2. Input Data

#### 9.2.1 NakshatraSlice list (Moon only)
Each NakshatraSlice provides:
- StartUtc / EndUtc
- NakshatraId (1..27)

#### 9.2.2 BirthMoonNakshatraId
Provided by user profile; used to rotate the nakshatra list and form the Tara-Bala matrix.

#### 9.2.3 TransitBuilderUtility
Contains reusable methods:
- SwapNakshatras()
- MakeTaraBalaMatrix()
- ComputeTaraBalaFromMatrix()

### 9.3. Database Tables Used

### TARABALA
| Column | Description |
|--------|-------------|
| ID | 1..9 (Janma, Sampat, Vipat, etc.) |
| COLORID | FK → COLOR table |

### TARABALA_DESC
Localized descriptive fields:
NAME, SHORTNAME, DESCRIPTION, LANGUAGECODE.

Slices do not store description — UI resolves via DataCache.

### 9.4. TaraBalaSlice Model

public class TaraBalaSlice : CalendarSlice

Notes:
- Only essential computational data is stored.
- UI will obtain localized text from TaraBalaDescList.
- ETransitType is not used.

### 9.5. TaraBalaTransitBuilder

public static class TaraBalaTransitBuilder

Logic Summary:
- Input = NakshatraSlice sequence + birth nakshatra.
- Tara-Bala matrix generated via SwapNakshatras + MakeTaraBalaMatrix.
- Tara-Bala index and percent computed per slice.
- Color fetched from TARABALA table.
- Descriptions fetched by UI via DataCache.

### 9.6. UI Interaction

UI retrieves localized text via:
cache.TaraBalaDescList.FirstOrDefault(d => d.TaraBalaId == slice.TaraBalaId)
Slice remains computational and minimal.

### 9.7. Output
List<TaraBalaSlice>

## 9.8. Status: Completed

----------

### 10. Tithi Slice — Transit Engine Specification (PADMA)

### 10.1. Overview

TithiSlice represents the lunar day interval determined by the angular separation
between the Moon and the Sun. PADMA computes all Tithi transitions using Swiss
Ephemeris via SwissAnalysis and converts them into chronologically ordered slices.

### 10.2. Input Data (SwissAnalysis → TithiData)

public class TithiData {
    public DateTime DateTimeUtc { get; set; }
    public double MoonSunDifference { get; set; }
    public int TithiId { get; set; }
}

### 10.3. DataCache Tables Used

### TITHI
- ID (1..30)
- COLORID (FK → COLOR)

### TITHI_DESC
Localized UI text fields:
NAME, SHORTNAME, RULER, TYPE, GOODFOR, BADFOR, LANGUAGECODE.

### 10.4. TithiSlice Model

public class TithiSlice : CalendarSlice 

### 10.5. TithiTransitBuilder

public static List<TithiSlice> BuildTithiSlices(List<TithiData> list)

- StartUtc = moment Tithi starts
- EndUtc = next Tithi start (or +1 day fallback)
- Color fetched via GetTithiColorId

### 10.6. UI Interaction

UI uses DataCache.Instance.GetTithiDesc(tithiId) for localized text.

### 10.7 Output
List<TithiSlice> chronologically ordered.

### 10.8. Status: Completed

---------

## 11. Karana Slice — Transit Engine Specification (PADMA)

### 11.1. Overview

KaranaSlice represents half-segments of each TithiSlice.  
Every Tithi is divided into **exactly two equal Karana slices**, matching the PAD logic and the PADMA database schema:
- 30 Tithis → 60 Karanas  
- Karana identity and color are taken from the KARANA and KARANA_DESC tables.

KaranaSlice stores only identifiers and computed intervals.  
All descriptive text (name, good/bad, ruler) is retrieved via DataCache from KARANA_DESC.

### 11.2. Input Data

#### 11.2.1 TithiSlice list  
Each TithiSlice provides:
- StartUtc / EndUtc
- TithiId (1..30)

#### 11.2.2 Karana definitions from database
`KARANA` table contains:
- ID (primary key)
- TITHIID (FK → Tithi)
- POSITION (1 or 2)
- COLORID

`POSITION` defines:
- 1 → first half of tithi  
- 2 → second half of tithi  

#### 11.2.3 DataCache fields

```
IReadOnlyList<Karana> KaranaList
IReadOnlyList<KaranaDesc> KaranaDescList
```

### 11.3. KaranaSlice Model

public class KaranaSlice : CalendarSlice

Notes:  
- Slice stores only IDs and essential values.  
- Text descriptions are loaded via KaranaDescList at UI level.

### 11.4. KaranaTransitBuilder

public static class KaranaTransitBuilder

Logic Summary:
- Each TithiSlice is divided into two equal halves.  
- For each half, a corresponding Karana entry (POSITION=1 or POSITION=2) is selected.  
- ColorId is resolved from the KARANA table.  
- Descriptions resolved later from KARANA_DESC.

## 11.5. UI Interaction

UI retrieves localized text using:
cache.KaranaDescList.FirstOrDefault(d => d.KaranaId == slice.KaranaId)
Slice remains purely computational and minimal.

### 11.6 Output
List<KaranaSlice> chronologically ordered.

## 11.7. Status: Completed

------

## 12. Nitya Yoga Slice — Transit Engine Specification (PADMA)

### 12.1. Overview

NityaYogaSlice represents the transitions of the 27 Nitya Yogas based on the Moon+Sun
combined longitude.  
SwissAnalysis already provides a list of **NityaYogaData** points where each Yoga
begins.  
A Slice is created for each Yoga interval:

- Start = current Yoga start time  
- End = next Yoga start  
- NityaYogaId = 1..27  
- ColorId is resolved from the NITYAYOGA table via DataCache  

All textual and descriptive information is taken from NITYAYOGA_DESC.

### 12.2. Input Data

#### 12.2.1 NityaYogaData list  
Each entry contains:
- DateTimeUtc — start moment of the Yoga  
- NityaYogaId — enum-compatible yoga id (1..27)

#### 12.2.2 DataCache fields

```
IReadOnlyList<NityaYoga>      NityaYogaList
IReadOnlyList<NityaYogaDesc>  NityaYogaDescList
```

`NITYAYOGA` table provides:
- COLORID  
- NAKSHATRAID  
- YOGIPLANETID  
- AVAYOGIPLANETID  

Slice stores only ID + Color.  
All extended attributes are resolved by UI via DataCache.

### 12.3. NityaYogaSlice Model

public class NityaYogaSlice : CalendarSlice

Notes:  
- Minimal slice: only IDs and computed intervals.  
- Text and detailed attributes come from NityaYogaDescList.

### 12.4. NityaYogaTransitBuilder

public static class NityaYogaTransitBuilder

Logic Summary:  
- Directly converts NityaYogaData list into continuous slices.  
- Computation model identical to Tithi and Nakshatra slices.  
- ColorId resolved through DataCache.  
- Descriptions fetched later from NityaYogaDescList.

### 12.5. UI Interaction

UI retrieves localized text via:
cache.NityaYogaDescList.FirstOrDefault(d => d.NityaYogaId == slice.NityaYogaId)
Slice remains minimal and computational.

### 12.6 Output
List<NityaYogaSlice> chronologically ordered.

### 12.7. Status: Completed

-------

## 13. Chandra Bala Slice — Transit Engine Specification (PADMA)

### 13.1. Overview

Chandra Bala reflects in which **house from the natal Moon** the *transiting Moon* is located.  
It evaluates the Moon’s current zodiac position relative to the natal Moon zodiac.

A ChandraBalaSlice corresponds to each Moon PlanetSlice interval.  
Start/End come directly from PlanetSlice.

Color rules:
- RED  → Houses **6, 8, 12**  
- RED  → If Moon is in **Scorpio**  
- GREEN → All other houses  

Slice stores only computational values (house/zodiac/color).  
UI handles all text.

### 13.2. Input Data

#### 13.2.1 Moon PlanetSlice list  
Each Moon PlanetSlice provides:
- StartUtc / EndUtc  
- ZodiacId (1..12)  
- ZodiacCode (enum EZodiac)

#### 13.2.2 Birth Moon zodiac  
`birthZodiacMoonId` — obtained from the profile or natal chart.  
Used to rotate zodiac list so that natal Moon becomes “house 1”.

#### 13.2.3 TransitBuilderUtility  
Provides a helper:
```
SwapZodiacs(List<Zodiac> zList, int birthZodiacMoonId)
```
Returns a list of 12 elements with the order cyclically rotated.

### 13.3. ChandraBalaSlice Model

public class ChandraBalaSlice : CalendarSlice

Notes:
- Slice stores only minimal required data.
- No DB lookup needed for Chandra Bala.

### 13.4. ChandraBalaTransitBuilder

public static class ChandraBalaTransitBuilder

Summary:
- Rotate zodiac list so natal Moon becomes position 1.
- Compute house number by index.
- Apply house-based and sign-based color rules.
- Use PlanetSlice timing unchanged.

### 13.5. UI Interaction

UI may use:
- HouseNumber  
- ZodiacCode  
- ColorId (segment coloring)  
- Optional localization (“Moon in X house”)

Slice is strictly computational.

### 13.6 Output
List<ChandraBalaSlice> chronologically ordered.

### 13.7. Status: Completed

-------

## 14. YogaSlice Requirements

### 14.1. Overview
This document summarizes the architecture and approach used to implement Yoga calculations within the PADMA Transit Engine.

### 14.2. YogaSlice Structure
A YogaSlice represents a continuous time interval during which a specific Yoga is active. It includes:

- `YogaId`: Numeric ID from YOGA table
- `YogaCode`: Enum `EYoga`
- `StartUtc` / `EndUtc`: Interval boundaries
- `Vara`: Day of the week
- `NakshatraCode`: ENakshatra or 0 when not applicable
- `TithiId`: Tithi number or 0 if not used
- `ColorId`: Color from DB via YOGA table

### 14.3. YogaTransitBuilder
YogaTransitBuilder constructs YogaSlices for a given day using:
- NakshatraSlices
- TithiSlices
- Vara (DayOfWeek)
- periodStartUtc / periodEndUtc (day boundaries)

It routes each YogaCode to appropriate builder:
- Generic rule-based: Dvipushkar, Tripushkar
- Specialized: Amritasiddha, Sarvartha, Siddha (basic), Siddha (large)

### 14.4. YogaRules
Contains definitions for:
- Vara
- Allowed Nakshatras
- Allowed Tithis
- Multi-result allowed flag
- Large Siddha specific combined rules

### 14.5. Implemented Yogas

#### 14.5.1. Dwipushkar
- Vara: Monday, Wednesday, Thursday
- Tithis: {2,7,12,17,22,27}
- Nakshatras: MRIGASHIRA, CHITRA, DHANISHTA
- Generic rule-based

#### 14.5.2. Tripushkar
- Vara: Tuesday, Saturday, Sunday
- Same Tithis: {2,7,12,17,22,27}
- Nakshatras: KRITTIKA, PUNARVASU, UTTARAPHALGUNI, VISAKHA, UTTARAASHADHA, PURVABHADRAPADA
- Generic rule-based

#### 14.5.3. Amritasiddha
- Each Vara has:
  - 1 required Nakshatra
  - 2 forbidden Tithis
- Yoga occurs on interval intersection
- Uses dedicated builder

#### 14.5.4. Sarvartha Siddha
- Pure Nakshatra logic
- Vara-specific lists of allowed Nakshatras
- Dedicated builder

#### 14.5.5. Siddha (basic)
- Tithi-only Yoga
- Vara-dependent groups of allowed Tithis
- Built via `BuildSiddha`

#### 14.5.6. Siddha (large)
- Vara determines:
  - allowed Nakshatras
  - allowed Tithis
- Tuesday: Nakshatra-only
- Other days: Intersection of Nakshatra+Tithi intervals
- Interval clipped to day boundaries
- Built via `BuildLargeSiddha`

#### 14.5.7. MRITYU Yoga  
**Type:** Tithi-based, day-of-week dependent.  
For each Vara (DayOfWeek), MRITYU activates on specific **TithiIds**:

| Vara | TithiIds |
|------|----------|
| Monday | 2, 7, 12, 17, 22, 27 |
| Tuesday | 1, 6, 11, 16, 21, 26 |
| Wednesday | 3, 8, 13, 18, 23, 28 |
| Thursday | 4, 9, 14, 19, 24, 29 |
| Friday | 2, 7, 12, 17, 22, 27 |
| Saturday | 5, 10, 15, 20, 25, 30 |
| Sunday | 1, 6, 11, 16, 21, 26 |

- No Nakshatra involvement  
- Slice = intersection of TithiSlice and daily period  

#### 14.5.8. ADHAM Yoga  
**Type:** Tithi-based, day-of-week dependent.  

| Vara | TithiIds |
|------|----------|
| Monday | 11, 21 |
| Tuesday | 10, 25 |
| Wednesday | 1, 9, 16, 24 |
| Thursday | 8, 23 |
| Friday | 7, 22 |
| Saturday | 6, 21 |
| Sunday | 7, 12, 22, 27 |

- No Nakshatra involvement  
- Slice = TithiSlice intersect daily period  

#### 14.5.9. YAMAGHATA Yoga  
**Type:** Nakshatra-based, day-of-week dependent.  

| Vara | Nakshatra |
|------|-----------|
| Monday | VISAKHA |
| Tuesday | ARDRA |
| Wednesday | MULA |
| Thursday | KRITTIKA |
| Friday | ROHINI |
| Saturday | HASTA |
| Sunday | MAGHA |

- Tithi does not matter  
- Slice = NakshatraSlice intersect daily period  

#### 14.5.10. DAGDHA Yoga  
**Type:** Nakshatra-based, day-of-week dependent.  

| Vara | Nakshatra |
|------|-----------|
| Monday | CHITRA |
| Tuesday | UTTARAASHADHA |
| Wednesday | DHANISHTA |
| Thursday | UTTARAPHALGUNI |
| Friday | JYESHTHA |
| Saturday | REVATI |
| Sunday | BHARANI |
 
- Tithi does not matter  
- Slice = NakshatraSlice intersect daily period  

#### 14.5.11. UNFAVORABLE Yoga  
**Type:** Tithi-based, day-of-week dependent.  

| Vara | TithiIds |
|------|----------|
| Monday | 6, 26 |
| Tuesday | 5, 7, 20, 22 |
| Wednesday | 3, 8, 18, 23 |
| Thursday | 6, 9, 21, 24 |
| Friday | 8, 9, 10, 23, 24, 25 |
| Saturday | 7, 9, 11, 22, 24, 26 |
| Sunday | 4, 9 |

- No Nakshatra  
- Slice = TithiSlice intersect daily period  

### 14.7. Output
YogaTransitBuilder returns all YogaSlices sorted by StartUtc.

### 14.8. Status: Completed

----------

# 15. SunriseSlice Requirements

This section extends PADMA Transit Engine requirements with the Sunrise/Sunset subsystem used by Yoga, Muhurta, etc.

## 15.1. SunriseSlice

A SunriseSlice represents all key solar transition points for a specific UTC date.

### 15.2. Model

```
SunriseSlice
{
    DateTime PreviousSunriseUtc   // sunrise of previous day
    DateTime SunriseUtc           // sunrise of current day
    DateTime SunsetUtc            // sunset of current day
    DateTime NextSunriseUtc       // sunrise of next day
}
```

#### Notes
- All values are UTC.
- Values are produced using Swiss Ephemeris functions swe_rise_trans.
- Sunset is stored as part of the same slice and is **not** a separate slice type.

## 15.3. SunriseTransitBuilder

SunriseTransitBuilder constructs SunriseSlice objects for arbitrary time ranges.

### 15.4. Methods

#### `Build(DateTime dateUtc, double lat, double lon, double alt = 0)`
Builds a SunriseSlice for a specific day.

Internally:
- PreviousSunriseUtc = sunrise(dateUtc - 1 day)
- SunriseUtc = sunrise(dateUtc)
- SunsetUtc = sunset(dateUtc)
- NextSunriseUtc = sunrise(dateUtc + 1 day)

Sunset fallback: if sunset is null, use `SunriseUtc + 12 hours`.

#### `BuildRange(DateTime startUtc, DateTime endUtc, double lat, double lon, double alt = 0)`
Constructs slices for all UTC calendar days from `startUtc` to `endUtc` inclusive.
Iteration is done day-by-day.

## 15.5. Purpose in Transit Engine

Sunrise slices are foundational for:

### ✓ YogaTransitBuilder  
PeriodStart = SunriseUtc  
PeriodEnd   = NextSunriseUtc  

### ✓ MuhurtaTransitBuilder  
- Rahu Kala: sunrise → sunset  
- Yama Ghanda: sunrise → sunset  
- Gulika: sunrise → sunset  
- Abhijit: midpoint(sunrise, sunset)  
- Brahma Muhurta: previous sunrise → sunrise

## 15.6. Behavior Summary

- Always calculated in UTC.
- Handles long ranges (e.g., yearly transit generation).
- Supports buffer days (e.g., -2 / +2 days) for timezone & DST shifts.
- Fully compatible with existing TransitBuilder architecture.

## 15.7. Output
SunriseTransitBuilder returns all SunriseSlices sorted by SunriseUtc (sunrise of current day).

## 15.8. Status: Completed

---------

# 16. MuhurtaSlice Requirements

This section extends PADMA Transit Engine requirements with the **five classical Muhurtas**:
- Abhijit
- Rahu Kala
- Gulika Kala
- Yamaganda
- Brahma Muhurta

All Muhurta calculations are based on **Sunrise/Sunset** times obtained from Swiss Ephemeris.

## 16.1.  MuhurtaSlice

All muhurtas are represented by a unified slice model.
```
MuhurtaSlice : CalendarSlice
{
    EMuhurta MuhurtaCode
    EMuhurta OverlappedMuhurtaCode
    int MuhurtaId        // (int)MuhurtaCode
    int ColorId          // from DataCache.MuhurtaList
}
```

### Notes
- `Kind = ETransitKind.Muhurta` is set in the constructor.
- Colors are resolved through DataCache → MUHURTA table.
- `OverlappedMuhurtaCode` is preserved for UI overlay handling.
- Time interval fields (`StartUtc`, `EndUtc`) are inherited from CalendarSlice.

## 16.2. MuhurtaTransitBuilder

The builder constructs all five muhurtas for a given range of days, using **SunriseSlice** as input.

## 16.3. Methods

### `List<MuhurtaSlice> BuildRange(List<SunriseSlice> sunriseSlices)`
Builds a flat sorted list of all muhurtas for the entire date range.

Internally for each SunriseSlice:
- sunrise = SunriseUtc  
- previous sunrise = PreviousSunriseUtc  
- sunset = SunsetUtc  
- daytime = sunset - sunrise  

Then the five muhurtas are computed.

## 16.4. Muhurta Rules

### 16.4.1. Abhijit (except Wednesday)
- Day is divided into **15 equal parts**
- Abhijit = part 7  
- Start = sunrise + 7 * part  
- End = start + part  
- Skipped on Wednesdays

### 16.4.2. Rahu Kala
Day is divided into **8 equal parts**.

Fixed part index by day:
```
Mon=1 Tue=6 Wed=4 Thu=5 Fri=3 Sat=2 Sun=7
```

### 16.4.3. Gulika Kala
Day divided into **8 parts**.

Part index:
```
Mon=5 Tue=6 Wed=4 Thu=3 Fri=2 Sat=1 Sun=7
```

### 16.4.4. Yamaganda
Day divided into **8 parts**.

Part index:
```
Mon=7 Tue=5 Wed=6 Thu=4 Fri=2 Sat=3 Sun=1
```

### 16.4.5. Brahma Muhurta
Uses the interval **previousSunrise → sunrise**.

- This night section is divided into **30 parts**
- Brahma Muhurta = part 28  
- Start = previousSunrise + 28 * part  
- End = start + part  

## 16.5. Output Behavior

- Returns a **flat list** of MuhurtaSlice.
- Sorted by StartUtc.
- TransitEngine will later group muhurtas per calendar day.
- All times are stored strictly in UTC.

## 16.6. Dependencies

- Requires SunriseSlice (PreviousSunriseUtc, SunriseUtc, SunsetUtc).
- Uses EMuhurta enum.
- Uses MUHURTA table for color resolution.
- Integrated into the same TransitBuilder architecture as:
  - PlanetTransitBuilder  
  - YogaTransitBuilder  
  - NakshatraTransitBuilder  

## 16.7. Status: Completed

----------------

# PADMA Transit Rendering – PanchangaBar & CalendarViewModel Integration

## Overview
This document describes the architecture and implementation of the Panchanga rendering system in the PADMA application, focusing on:

- **PanchangaBar** — the lightweight graphical control for rendering Panchanga segments using `GraphicsView`.
- **CalendarViewModel pipeline** — how daily slices (Tithi for now) are calculated and bound to UI.
- **Segment ↔ UI mapping** — how SwissEphemeris outputs, transit slices, and Panchanga bars interact.
- **Performance principles** ensuring smooth rendering across all 42 calendar cells.

This document expands the official project requirements with the functionality implemented as of the latest development session.

# 1. Data Flow Overview

SwissAnalysis → TransitBuilder → DayItem.TithiSegments → PanchangaBar → GraphicsView

## 1.1. **SwissAnalysis**  
   Produces raw astronomical event lists in UTC (London baseline).

## 1.2. **Transit Builders (e.g., TithiTransitBuilder)**  
   Converts Swiss events into continuous interval slices (TithiSlice), each with:
   - StartUtc, EndUtc,
   - SliceKind / Value,
   - Assigned EColor → AppColor.

## 1.3. **CalendarWindowService**  
   Produces:
   - visibleDays (42 days shown on calendar),  
   - bufferStart / bufferEnd for Swiss calculations  
     (slightly extended range to avoid gaps),
   - visibleStart / visibleEnd for UI region.

## 1.4. **CalendarViewModel.GenerateDays**  
   For each DateOnly in visibleDays:
   - Convert to local timezone of active profile.
   - Build Tithi segments via BuildTithiSegmentsForDay(...).
   - Attach resulting IList<PanchangaSegment> to the DayItem.

## 1.5. **PanchangaBar**  
   - Receives Segments + DayDate via bindings.
   - Renders segments in a GraphicsView using single-pass drawing.

# 2. CalendarViewModel Implementation Summary

## 2.1 Responsibilities
- Construct correct visible calendar window (42 cells).
- Retrieve active profile and associated timezone.
- Request Swiss slices for expanded buffer window.
- Build daily Panchanga segments (currently Tithi).
- Attach segments to DayItem before UI binding.
- Ensure no asynchronous updates after UI virtualization.

## 2.2 Key Architectural Points

### A. Visible Day Grid Alignment
All days in the calendar are built from:
```
(visibleStart, visibleEnd, bufferStart, bufferEnd, visibleDays)
= CalendarWindowService.BuildWindow(...)
```

This ensures UI dates and Swiss calculation dates are always in the same coordinate system, preventing earlier issues where entire weekday columns failed to render.

### B. Segment Creation Before UI Binding
Each DayItem receives its TithiSegments before being added to the ObservableCollection:

```csharp
var segments = BuildTithiSegmentsForDay(tithiSlices, date, tzInfo, DataCache.Instance);

Days.Add(new DayItem {
    Date = date,
    DayNumber = date.Day,
    IsCurrentMonth = isCurrentMonth,
    IsToday = isToday,
    TithiSegments = segments
});
```
This avoids the need for INotifyPropertyChanged inside DayItem and ensures virtualization does not suppress segment redraws.

### C. Timezone Alignment

Swiss data is UTC-based. Each slice is mapped to local time of the active profile:
```
var sliceStartLocal = new DateTimeOffset(slice.StartUtc, TimeSpan.Zero).ToOffset(offset);
var sliceEndLocal   = new DateTimeOffset(slice.EndUtc,   TimeSpan.Zero).ToOffset(offset);
```
Next, segments are trimmed to:
```
(dayLocalStart; dayLocalEnd)
```
to produce exact per-day segment lists.

# 3. PanchangaBar – Graphics-Based Rendering
## 3.1 Why GraphicsView

The initial approach (multiple BoxView per segment) caused:
- Heavy MAUI layout calculations,
- UI freezes,
- Missing columns due to virtualization timing.
Switching to a single GraphicsView per PanchangaBar eliminated all performance problems.

The bar now:
- Does NOT create child controls,
- Does NOT use AbsoluteLayout,
- Draws everything in one GPU-accelerated pass.

## 3.2 Structure

PanchangaBar
  └── GraphicsView
        └── PanchangaBarDrawable (IDrawable)

Key Properties:
- DayDate — date of the calendar cell.
- Segments — list of PanchangaSegment:
	- Start (local DateTime)
	- End (local DateTime)
	- Color (MAUI Color)

## 3.3 Rendering Logic

Inside Draw(ICanvas canvas, RectF rect):
- Normalize segment bounds to the current day.
- Compute minute offset → pixel offset mapping.
- Fill colored rectangles for each segment.
- Draw thin black separator lines.

Redrawing happens only when:
- The bar receives a final size,
- DayDate or Segments change.
This results in instant UI response even with hundreds of segments.

# 4. Segment Model – PanchangaSegment
```
class PanchangaSegment {
    public DateTime Start;  // local time
    public DateTime End;    // local time
    public Color Color;     // segment color from AppColor
}
```
Segments define time intervals, not values.
This is essential for rendering continuous bars across day boundaries.

#5. Performance Considerations
✔ One GraphicsView per row
	Removes thousands of MAUI view elements.
✔ No child controls
	Avoids excessive layout calculations.
✔ Precomputed segments
	All computation occurs once in ViewModel.
✔ Single redraw
	Minimizes overhead when binding occurs.
✔ Proper timezone mapping
	Offsets are computed once per day.
#6. Summary

This module defines a high-performance Panchanga rendering subsystem for the PADMA application.
We now have:
- Timezone-correct slice generation,
- GraphicsView-based rendering,
- Instant calendar performance,
- A scalable design for all six Panchanga layers.

-----

# Profile Context Service

## Overview

`ProfileContextService` is a dedicated application-level service responsible for
building, caching, and providing **precomputed profile-dependent astrological data**
used across the PADMA application.

Its main purpose is to **centralize all expensive and profile-specific calculations**
that were previously duplicated in UI layers (e.g. `CalendarViewModel`) and to ensure
that these calculations are performed **once per active profile**, then reused
consistently throughout the app.

This service is a key architectural component that decouples:
- profile data
- Swiss Ephemeris calculations
- transit-related baseline values

from UI logic and calendar rendering.

## High-Level Responsibilities

`ProfileContextService` is responsible for:

- Building a **ProfileTransitContext** for the active profile
- Performing all required Swiss Ephemeris calculations for the profile
- Resolving and caching:
  - profile timezone
  - birth Moon Nakshatra
  - birth Moon Zodiac (Rashi)
  - birth planetary positions
- Providing a stable, reusable context object to all consumers
- Notifying the application when the context has been rebuilt

## Lifecycle and Initialization

### Creation

- `ProfileContextService` is instantiated once as part of `DataCache`
- It exists for the entire lifetime of the application

### Rebuild

The context is built via:

```csharp
await ProfileContextService.RebuildAsync();
```

This method:
- Reads the active profile from DataCache
- Resolves profile locations and timezone
- Converts birth datetime to UTC
- Performs Swiss Ephemeris calculations
- Constructs a new ProfileTransitContext
- Assigns it to Current

### Timing

RebuildAsync() is called:
- on application startup (App.OnStart)
- whenever the active profile changes (future extension)
The context is not rebuilt implicitly inside view models or UI code.

### Access Pattern

The current context is accessed synchronously:
```
var ctx = DataCache.Instance.ProfileContextService.Current;
```
Consumers must handle the possibility that Current is null
(e.g. during early startup before RebuildAsync completes).

### Update Notification

To avoid UI race conditions, DataCache exposes an event:

```
event Action ProfileContextUpdated;
```
This event is raised after RebuildAsync() completes successfully.

Typical usage:
- CalendarViewModel subscribes to this event
- Upon notification, it triggers calendar regeneration

This pattern ensures:
- no async logic inside view models
- no blocking UI initialization
- deterministic redraw after context availability

### Design Notes

- The service is intentionally stateful, but with a narrow and controlled scope
- Context rebuilding is explicit and observable
- The design favors predictability over implicit magic
- All calculations are traceable and reproducible
This approach closely mirrors the architectural evolution from the legacy PAD
application while adapting it to modern MAUI patterns.

-----

# Month / Year Picker Popup (Calendar Popup)

## Purpose
The **Month / Year Picker Popup** provides a dedicated UI for selecting a target **month and year** used to rebuild the main 42day calendar window in PADMA.  
It is optimized for **clarity, localization, and predictable behavior**, while still giving users a visual overview of the month.

The popup is implemented using **Plugin.Maui.Calendar (v2.0.12)** and **CommunityToolkit.Maui.Popup**.

## Invocation
The popup is opened from **MainPage** when the user taps the month/year title or uses the toolbar controls.

**Input parameters:**
- `CultureInfo culture` — active UI culture
- `int year` — current calendar year
- `int month` — current calendar month

```csharp
new MonthPickerPopup(CurrentCulture, Year, Month)
```

## Functional Behavior

### Selection Model
- The popup is a **month/year selector**, not a full date selector.
- Tapping individual days is allowed **for visual feedback only**.
- The confirmed result **never depends on the selected day**.

### Confirmation Rules
| Action | Result |
|------|--------|
| **OK** | Returns `new DateTime(Year, Month, 1)` |
| **Today** | Returns `DateTime.Today` |
| **Cancel** | Returns `null` |
| Tap outside popup | Ignored (no change applied) |

The caller always extracts **Year / Month** and rebuilds the calendar window accordingly.

## Calendar Control Configuration

### Core Settings
- `Culture` — bound to active UI culture
- `FirstDayOfWeek` — taken from `DataCache.Instance.DayOfWeek`
- `Year`, `Month` — controlled explicitly
- `Day = 1` — fixed technical value

### Disabled Features
The popup intentionally disables features that are not relevant to month selection:

- Event list / footer panel
- Swipe up hide gesture
- Footer arrow indicator

This ensures a clean, predictable UI focused on month navigation.

## Weekday Header Styling

To avoid multiline wrapping on Android (especially for Slavic languages), weekday headers are customized:

- Reduced font size
- Forced single line rendering
- Centered alignment

This prevents visual artifacts such as duplicated “second rows” of weekday letters.

## Footer Buttons Layout

### Buttons
- **Cancel** — localized
- **Today** — localized
- **OK** — fixed short label

### Layout Strategy
- Footer uses a **Grid** with columns: `*, *, Auto`
- `Cancel` and `Today` share available width
- `OK` uses minimal width and is right aligned

This prevents long localized labels (e.g. Ukrainian) from pushing buttons outside the popup frame.

## Localization

All user visible text is localized via `APP_TEXTS` and `Localization.GetLocalizedText()`.

Supported languages:
- English (`en`)
- Ukrainian (`uk`)
- Polish (`pl`)
- Russian (`ru`)

## Design Rationale

- Keeps **month navigation fast** via arrows
- Avoids accidental date based logic
- Works consistently across Android / Windows
- Fully respects application settings (culture, first day of week)
- Scales correctly for long localized labels

This popup is considered the **canonical month/year selection mechanism** in PADMA.

## Status
**Implemented and stable**  

-----

# PADMA – Calendar Day Interaction & Day Overview UX Specification

## Purpose

This chapter defines the user interaction model for selecting a day in the main calendar,
previewing day information, and navigating to the full Day page (daily diary)
in the PADMA (Personal Astrological Diary Mobile App).

The goal is to provide a **mobile-friendly, intuitive, and scalable UX**
that replaces mouse-based desktop interactions with touch-based patterns.

## Context & Constraints

- The main calendar displays **42 days** (6×7 grid).
- Each day cell is visually small and contains only **colored bars (Panchanga segments)**.
- Displaying text directly inside calendar cells is not viable on mobile screens.
- The application targets **mobile platforms (Android, iOS)** using .NET MAUI.
- Existing UX patterns from Configuration pages should be reused where possible.

## High-Level Interaction Model

The interaction with calendar days follows a **three-level depth model**:

1. **Selection** – choose a day in the calendar.
2. **Overview** – preview detailed information for the selected day.
3. **Details** – open the full Day page (daily diary).

## Step 1: Day Selection (Calendar Grid)

### Interaction
- **Tap on a day cell**:
  - Selects the day.
  - Visually highlights the selected day (e.g. light orange background).
  - If another day was previously selected, selection moves to the new day.

### Notes
- This action does **not** open a popup.
- Selection is lightweight and reversible.

## Step 2: Day Overview (Preview / Popup Page)

### Trigger
- **Tap again on the already selected day**.

This is **not a system double-tap**, but a logical:
> *tap on selected item → open overview*

### Result
- A **near full-screen page** opens, visually behaving like a popup.
- The user remains conceptually “inside the calendar context”.

### Content of Day Overview
The overview page displays:
- Large, clearly visible **Panchanga bars** for the day.
- Additional bars (e.g. Yogas) not shown in the calendar grid.
- Key textual information for the day (to be defined later).
- A single primary action button:
  - **“Open Day Details”** (or equivalent localized text).

## Navigation & Closing Behavior (Day Overview)

The Day Overview page uses the **same header UX pattern as Configuration pages**.

### Toolbar Layout
```
[ ← Back ] Title of the Day [ ✕ Close ]
```

### Closing Options
Both of the following close the overview and return to the calendar:
- **Back arrow (←)** on the left (Shell navigation).
- **Close (✕)** button on the right.

### System Navigation
- System Back button / gesture must also close the overview.
- No background tap-to-dismiss behavior is used.

### Explicit Non-Goals
- No closing by tapping outside the content area.
- No “Close” button inside the content body.

## Step 3: Navigation to Day Page (Daily Diary)

### Trigger
- Tap on **“Open Day Details”** button in the Day Overview.

### Result
- The Day Overview closes.
- Navigation proceeds to the dedicated **DayPage**.
- The DayPage represents the full daily diary / working view.

### Rationale
- Prevents accidental deep navigation.
- Keeps the overview page lightweight and informational.
- Clearly separates *preview* vs *work* modes.

## Architectural Notes

- The Day Overview is implemented as a **regular Shell page**, not a modal dialog.
- It reuses the existing **ConfigBasePage / configuration hub header pattern**:
  - Back arrow provided by Shell.
  - Close (✕) button provided by shared template logic.
- This ensures:
  - UX consistency across the app.
  - Minimal new infrastructure.
  - Predictable navigation behavior.

## UX Principles Followed

- Mobile-first interaction (tap-based, no hover, no mouse assumptions).
- Progressive disclosure of information.
- Explicit user control over navigation depth.
- Reuse of established application UX patterns.
- Avoidance of hidden or non-discoverable gestures.

## Future Extensions (Out of Scope)

- Exact list of textual data shown in Day Overview.
- Editing or interaction inside the overview page.
- Animations or transitions.
- Gesture-based shortcuts (e.g. swipe to open DayPage).

These can be defined in follow-up specifications.

## Summary

The final interaction flow is:

**Tap day → Select**  
**Tap selected day → Day Overview**  
**Tap “Open Day Details” → DayPage**

This model provides clarity, scalability, and a high-quality mobile UX
while staying fully aligned with existing PADMA architecture.

-----

# PADMA – Day-Level Progressive Computation Strategy

## Purpose

This document defines the architectural approach for calculating astrological data
at different UI levels (Calendar, Day Overview, Day Details) in PADMA.

The goal is to:
- minimize unnecessary calculations,
- improve performance on mobile devices,
- avoid oversized Day objects,
- and ensure consistent results across all UI levels.

## Background

In the legacy PAD desktop application, all calculations were performed upfront
for each day, resulting in very large Day objects (200+ properties),
even though only a small subset of this data was used in most UI scenarios.

For PADMA (mobile-first), this approach is not optimal due to:
- limited screen size,
- performance constraints,
- and typical user behavior (few days explored deeply).

## Core Principle

**Progressive computation with in-memory caching.**

Astrological data is calculated:
- only when needed,
- at the appropriate UI depth,
- and cached per day to avoid recomputation.

## Computation Levels

### Level 1 – Calendar (MainPage)

**Scope:**
- 42-day grid (6×7).

**Calculations:**
- Panchanga segments required for the calendar view:
  - Nakshatra
  - Tara Bala
  - Tithi
  - Karana
  - Nitya Yoga
  - Chandra Bala

**Result:**
- Lightweight `DayItem` objects.
- No heavy transit or timeline calculations.

### Level 2 – Day Overview (DayOverviewPage)

**Scope:**
- Single selected day.

**Calculations (in addition to Level 1):**
- Expanded Panchanga presentation (same segments, with textual context).
- Planets transit through Zodiac signs segments.
- Muhurta segments.
- Day-level Yogas (if applicable).

**Result:**
- `DayOverviewData` model.
- Designed for quick, near-instant rendering.

### Level 3 – Day Details (DayPage)

**Scope:**
- Single selected day (deep analysis).

**Calculations:**
- Full transit and event data, including:
  - Planetary transits
  - Muhurta timelines
  - Yogas
  - Ghati-based Panchanga
  - Eclipses
  - Additional astrological events
- Vertical time-scale with 20+ stacked segments.
- Preparation for user-defined events and notifications.

**Result:**
- `DayDetailsData` model.
- Heavy computation, executed lazily.

## Caching Strategy

### Cache Type
- **In-memory only**.
- No persistence to SQLite.

### Cache Key
```
(ProfileId, Date)
```
- `ProfileId` uniquely defines:
  - birth data,
  - living data
- Language and localization are **not part of the cache key**.

## Localization & Timezone Handling

- All cached data stores **structural / numeric results only**:
  - enums,
  - IDs,
  - time intervals,
  - segment definitions.
- Localized texts are generated at the UI level using:
```
DataCache.Instance.CurrentLanguageCode
```
- Timezone is derived from `ProfileId`.

Changing language:
- does NOT invalidate the cache.

Changing profile or profile timezone:
- invalidates cached data for that profile (or all cache initially).

## Day Computation Service

A dedicated service coordinates calculations and caching.

### Responsibilities
- Build and cache `DayOverviewData` and `DayDetailsData`.
- Reuse already computed results across UI levels.
- Ensure single computation per day using lazy execution.

### Conceptual Interface
```
Task<DayOverviewData> GetOverviewAsync(DayKey key, DayItem baseDay);
Task<DayDetailsData> GetDetailsAsync(DayKey key, DayItem baseDay);

void InvalidateProfile(int profileId);
void InvalidateAll();
```

## Cache Invalidation Rules

Cache is invalidated when:
- Active profile changes.
- Configuration affecting calculations changes (timezone, ayanamsa, etc.).

Initial implementation may use:
```
InvalidateAll()
```
for simplicity and safety.

## Benefits

- Fast calendar rendering.
- Minimal memory usage.
- Heavy calculations only when explicitly requested.
- Clean separation of concerns between UI levels.
- Scalable foundation for future features (events, notifications, analytics).

##Summary

PADMA uses a three-level progressive computation model:
- Light data for many days.
- Medium data for a selected day overview.
- Heavy data only for deep day analysis.
All calculations are cached in memory per (ProfileId, Date)
and reused across UI layers whenever possible.

------

# PADMA – DayOverview Panchanga Bars: Text-on-Segments Architecture

Scope: MainPage calendar + DayOverviewPage (Panchanga bars)

## Goal

Display localized, compact labels **directly on Panchanga timeline bars** (segments), aligned to the **left edge of each segment**, without adding extra UI labels beside/under the bars.
This improves readability on mobile screens and prepares the same rendering approach for future blocks (Muhurta, Yogas, transits, DayPage timeline).

## High-level approach

### 1) Segment carries text
`PanchangaSegment` is extended with an optional text field:
- `Text` (string?, optional) — already localized and formatted for display (e.g. `4.Rohini`, or `10:37 4.Rohini`)
This field is **UI-facing** and is **not** part of heavy computations. It is built at the UI/ViewModel layer when segments are created for display.

### 2) Text is produced during segment building (UI layer)
The common helper `PanchangaHelper.BuildSegmentsForDay(...)` is extended to accept an **optional** delegate that can produce text per slice:
- `getText` optional callback; if not provided, `PanchangaSegment.Text` remains null.
This keeps the helper generic while enabling text injection for any bar type.

Text creation (ids, names, time prefix) is performed at the point where segments are created (e.g., `CalendarViewModel.GenerateDays(...)`), using:
- dictionaries from `DataCache` for localized names (ShortName/Name),
- `DataCache.Instance.CurrentLanguageCode` for current language.

### 3) Rendering is done in the bar drawable
`PanchangaBarDrawable` renders:
- segment rectangles,
- vertical separators,
- optional `seg.Text` overlay.

Rendering rules:
- text is **left-aligned** within the segment with a small padding (~1px),
- text is drawn only if the segment is large enough (height/width thresholds),
- text is clipped to the segment rectangle to avoid overdraw.

**Note:** wrapping control is being actively refined; the current goal is “no wrapping; if not fitting – do not show”.

## Text formatting rules

### Alignment
- Label is drawn at the **start** of each segment (left edge), not centered.

### Time prefix
- If a segment starts at the day boundary (00:00) — display only entity text:  
  `Id.Name`
- If a segment starts inside the day — prefix with local time (24h):  
  `HH:mm Id.Name`

Examples:
- `4.Rohini`
- `10:37 5.Mrigashira`

### Compact name policy
- For entities that have `ShortName`, prefer it for on-bar display.
- If `ShortName` is missing (e.g., Karana, NityaYoga), use full `Name`.

No extra whitespace inside the entity token: `Id.Name` (no space after dot).  
Time and entity are separated by a single space: `HH:mm Id.Name`.

## Localization policy

### Names (Nakshatra, TaraBala, Tithi, etc.)
Localized names are obtained via dictionaries built from `DataCache` for the current `LanguageCode`.

### Template phrases (ChandraBala)
ChandraBala uses localized templates stored in `APP_TEXTS`.

Added keys (NATIVETEXT) and translations (FOREIGNTEXT):
- `Moon in {0} house`
- `Moon in {0} house, {1}`

Formatting is done via `string.Format(localizedTemplate, ...)`.

### Zodiac code
Zodiac 3-letter codes are taken from table `ZODIAC.ZODIACCODE` and formatted for display as `TitleCase` (e.g., `SCO` → `Sco`).  
This is structural data (not localized).

### Special case – Scorpio highlighting
For ChandraBala, the “with sign” template is used only for **Scorpio** (SCO).  
This case is additionally highlighted by a special color rule (red override) in the ChandraBala builder.

## Files / components involved

### Data model
- `PanchangaSegment` — added `Text` property.

### Segment builder
- `PanchangaHelper.BuildSegmentsForDay(...)` — added optional `getText` delegate and assigned `PanchangaSegment.Text` accordingly.
- Wrapper overloads (non-generic) are updated to accept the same optional delegate and pass it through.

### Rendering
- `PanchangaBarDrawable` — draws `seg.Text` left-aligned and clipped per segment.

### ViewModel integration
- `CalendarViewModel.GenerateDays(...)` — builds per-language dictionaries and supplies `getText` for each Panchanga bar:
  - Nakshatra
  - TaraBala
  - Tithi
  - Karana
  - NityaYoga
  - ChandraBala (template-based)

### Page usage
- `DayOverviewPage.xaml` — displays the 6 Panchanga bars as a single “monolithic” block (no external labels), relying on on-segment text.

## UX layout note (future blocks)

DayOverviewPage will later include multiple blocks:
- Panchanga
- Planet-sign transits (9 planets)
- Muhurta (5 bars)
- Yogas (which are present at this day)

Between these blocks, a small vertical spacing (~2px) will be used, while **within** each block the bars remain tightly stacked (Spacing=0).

---------

# PADMA – Planet Markers & Planet Transit Overview

(MainPage Calendar + DayOverviewPage)

This document finalizes the **planet marker and planetary transit overview architecture**
after full implementation and validation.

Scope:
- **MainPage** – 42-day calendar grid (compact markers)
- **DayOverviewPage** – detailed planetary transit block (striped bars)

## Goal

Provide a **clear, compact, and astrologically correct visualization**
of important planetary changes without heavy per-day recomputation.

The system highlights:
- retrograde state,
- zodiac ingress (sign change),
- exaltation / debilitation,
- with full localization support,
- and correct timezone handling.

## Symbol semantics (strict)

### Base marker (default)
- `Pl` — planet is in normal/direct motion by default.
- **No `D` symbol** is used anywhere.

### Retrograde (state)
- `.R` suffix means **retrograde state**:
  - `Pl.R`

### Zodiac ingress (event)
- `→` indicates **zodiac ingress event** (planet changes sign during that day):
  - `Pl→`

### Combined retrograde + ingress
If ingress happens while the planet is retrograde:
- `Pl.R→`

### Exaltation / Debilitation (state)
- `↑` indicates exaltation state
- `↓` indicates debilitation state

These are displayed **next to the planet marker**:
- `Pl↑`
- `Pl↓`

**Rules:**
- If a planet is retrograde, **do not show** `↑` or `↓`.
- Exaltation/debilitation itself implies sign ingress → no extra `→` is shown.

### Exclusion (calendar)
- Moon is **excluded** from markers on MainPage (too fast).
- Rahu and Ketu are **excluded** rom markers on MainPage as well.
- Moon, Rahu and Ketu details are available on **DayOverviewPage** and **DayPage**.

## Acceptable & Non-Acceptable markers
Acceptable markers are final calendar tokens per planet per day (after applying precedence rules)

### Acceptable:
Pl
Pl.R
Pl→
Pl.R→
Pl↑
Pl↓

### Non-Acceptable (should not shown):
❌ Pl→↑
❌ Pl→↓
❌ Pl.R↑
❌ Pl.R↓
❌ Pl.R→↑
❌ Pl.R→↓

### Marker precedence (single final token per planet)
1. If `Retrograde == true`:
   - show `Pl.R→` if `Ingress == true`
   - else show `Pl.R`
   - (do not show `↑/↓` while retrograde)
2. Else (direct motion):
   - if `Exaltation == true` → show `Pl↑` (do not add `→`)
   - else if `Debilitation == true` → show `Pl↓` (do not add `→`)
   - else if `Ingress == true` → show `Pl→`
   - else → show `Pl`
   
### Retrograde exit
- No special “end” marker.
- Marker returns to `Pl`, `Pl↑`, or `Pl↓` depending on current state.

## Special Rule: Rahu & Ketu

For **Rahu and Ketu**:

- **Never show**:
  - `.R`
  - `↑` / `↓`
- **Allowed only**:
  - base label: `Ra`, `Ke`
  - zodiac ingress: `Ra→`, `Ke→`

This applies to:
- DayOverviewPage transit blocks 

Internally, calculations may use TRUE/MEAN nodes, but UI always displays
Rahu/Ketu as a single entity.

## Localization policy (planet abbreviations)

### Planet abbreviations
- Planet names are localized using:
  - `DataCache.Instance.PlanetDescList`
- Marker prefix uses **first 2 characters** of localized name.

Examples:
- `Ju`, `Ma`, `Ve`
- `Со`, `Ма` (non-Latin languages supported)

### Zodiac names (DayOverviewPage)
- Zodiac names are **fully localized**.
- Retrieved from:
  - `DataCache.Instance.ZodiacDescList`
- Displayed as:
```
Ju.R→, Capricorn
14:32 Ju→, Aquarius
```
(No zodiac codes are used in UI.)

## 7. MainPage (Calendar) – Display Rules

- One compact line per day, under day number.
- Show **only days where an event occurs**.
- Markers separated by space:
`Ju.R→ Me↑`

### Ordering (recommended)
1. Ingress events (`→`, including `R→`)
2. Retrograde (`.R`)
3. Exaltation / Debilitation (`↑/↓`)

## 8. DayOverviewPage – Planet Transit Block

### Visual structure
- Dedicated **Planetary Transit Block**
- 9 horizontal stripes:
- Sun, Moon, Mercury, Venus, Mars, Jupiter, Saturn, Rahu, Ketu
- Positioned below Panchanga block (with spacing)

### Stripe content
- Stripe starts at local `00:00` with base label:
`Ju.R, Capricorn`

- Each event creates a vertical split and new segment:
`14:32 Ju→, Aquarius`

### Color logic
- Colors derived from Transit Engine:
- Moon-based
- Lagna-based
- or split (top/bottom) depending on `EAppSetting`:
  - `TRANZITMOON`
  - `TRANZITLAGNA`
  - `TRANZITMOONANDLAGNA`

## Data Flow & Technical Architecture

### PlanetSlice (core)
- All planetary calculations are done in **UTC**.
- `PlanetSlice.StartUtc` is authoritative.
- Contains:
- ZodiacId
- Retrograde flag
- Color codes (Moon/Lagna)
- Other transit metadata

### TransitPack (shared)
- Built once in `CalendarViewModel`.
- Stored in `DayItem` and reused everywhere.

`Dictionary<EPlanet, IReadOnlyList<PlanetSlice>> TransitPack`

**Keys:**
- Sun..Saturn
- Rahu (8)
- Ketu (9)

TRUE/MEAN node choice is resolved during computation;

## DayOverview computation

- Uses TransitPack
- Converts:
	- Local day boundaries → UTC
	- UTC event times → Local (TimeZoneInfo from ProfileContextService)
- Aggregates slices to sign/retro-level (pads ignored here)

## Performance Principles

- No recomputation on DayOverviewPage.
- Calendar and Overview reuse the same TransitPack.
- Heavy Swiss calculations are done once per window.

## Current Status

✔ MainPage planet markers implemented
✔ DayOverview planetary transit block implemented
✔ Localization complete
✔ Timezone handling corrected
✔ Rahu/Ketu exceptions enforced
✔ Color logic validated

## Notes / Future Extensions

- DayPage will reuse the same TransitPack for pad-level visualization.

------

## DayOverviewPage — Daily Muhurtas

### Purpose
The **Daily Muhurtas** block visualizes key Jyotish time windows of the day as clear, intuitive time segments within a single daily timeline.  
The block is designed for quick visual orientation and practical day planning.

### UI Placement
- The Muhurtas block is located **below the Planet Transits block** on `DayOverviewPage`
- Vertical spacing from the Planet Transits block: **2 px**
- All Muhurtas are displayed as **one compact visual block**, without external headers or spacing between stripes

### Displayed Muhurtas
The following Muhurtas are displayed:
1. **Brahma Muhurta**
2. **Abhijit Muhurta**
3. **Rahu Kala**
4. **Yamaganda**
5. **Gulika Kala**

The number of stripes is fixed to **5**.

### Ordering Rules
- Muhurta stripes are ordered **by their start time** (ascending)
- If a Muhurta does not form on a given day (e.g. *Abhijit Muhurta* on Wednesdays), it:
  - is displayed **as the last stripe**
  - has no time segment drawn
  - shows a localized suffix **“does not occur”**

### Visual Representation
Each Muhurta is rendered as **one horizontal stripe with fixed height (24 px)**:

- Transparent outline — represents the full calendar day (00:00–24:00, local time)
- Colored segment — the active interval of the Muhurta
  - the color is taken from `DataCache` via `Muhurta.ColorId`
- Vertical markers:
  - start of the Muhurta
  - end of the Muhurta
- All elements are drawn **inside a single stripe**, without additional UI elements above or below

### Text Rendering Inside the Stripe
All text is rendered directly inside the stripe using custom drawing logic:

- **Left side**: localized short name of the Muhurta (`ShortName`)
- **Start time**:
  - displayed **to the left of the start marker**
  - horizontal position is calculated using actual text width (`HH:mm`) to ensure the full text fits before the marker
- **End time**:
  - displayed **to the right of the end marker**
- Time format: `HH:mm`
- Font and sizing are consistent with other DayOverview stripes (no per-element font sizing in XAML)

### Localization
- Muhurta names are taken from `MuhurtaDescList` using localized `ShortName`
- The suffix for a non-forming Muhurta is resolved via `APP_TEXTS`:
  - key: `" does not occur"` (leading space included)
  - supported languages: `en`, `uk`, `pl`, `ru`

### Technical Implementation
- Rendering is implemented via `GraphicsView` with a custom `MuhurtaBarDrawable`
- Text positioning uses string measurement (`GetStringSize`) to avoid overlaps
- All calculations are performed in UTC and converted to the active profile’s local time
- `DayItem` does not store Muhurta data — Muhurtas are computed only in `DayOverviewData`

### Status
✅ Implemented  
✅ Visually verified  
✅ Aligned with legacy PAD behavior  

------

## Day Overview – Yogas of the Day

### Purpose
The **Yogas of the Day** block displays all applicable *non-Nitya* yogas occurring during the selected day.
This block provides a compact, visual overview of yogas based on Panchanga rules, calculated according to **vara defined from sunrise to sunrise**, while being displayed within the civil day (00:00–24:00).

### Placement in UI
- The Yogas block is displayed on **DayOverviewPage**
- Positioned **below the Muhurtas block**
- Vertical spacing between blocks: **2 px**
- The block participates in vertical scrolling together with other overview stripes
- The bottom action button (“Day Details”) must always remain visible

### Data Source
- Yogas are calculated using:
  - `YogaTransitBuilder`
  - `Tithi` and `Nakshatra` slices
  - Vara determined by **SunriseSlice** (`PreviousSunrise → Sunrise → NextSunrise`)
- Yoga descriptions (ShortName) are loaded from cache:
  - `DataCache.YogaDescList`
- Colors are resolved via:
  - `YogaSlice.GetYogaColorId()` → App color cache

### Vara Definition
- Vara is defined **astronomically**, from **sunrise to the next sunrise**
- Two vara windows may intersect the civil day:
  1. Tail of the previous vara (before sunrise)
  2. Current vara (from sunrise onward)
- Yogas from both windows are considered if they intersect the civil day

### Display Rules

#### General
- Each **distinct yoga type** is displayed as **one horizontal bar**
- If the same yoga occurs multiple times during the day, its periods are:
  - **Merged into a single bar**
  - Displayed as **multiple colored segments** on that bar
- Segments are sorted by start time

#### Segment Rendering
- Each yoga segment is drawn as:
  - A colored rectangle over a transparent base bar
  - Vertical boundary lines at start and/or end (if inside the day)
- Time labels:
  - Start time (`HH:mm`) is shown **only if the segment starts after 00:00**
  - End time (`HH:mm`) is shown **only if the segment ends before 24:00**
- If a yoga:
  - Started before the day → rendered from `00:00` without start time
  - Ends after the day → rendered until `24:00` without end time

#### Title
- Yoga ShortName is rendered:
  - Inside the bar
  - Left-aligned
  - On top of all segments (never hidden by fill)
- Only one title per yoga bar, regardless of number of segments

### Ordering
- Yoga bars are ordered by:
  - The start time of their **first segment** within the day
- No fixed limit on the number of yogas per day

### Edge Cases
- Days may contain:
  - Zero yogas
  - One yoga
  - Multiple yogas with multiple segments
- Overlapping or adjacent segments of the same yoga are merged
- All calculations respect the active profile’s timezone

### Visual Consistency
- Bar height matches other overview stripes (Panchanga, Transits, Muhurtas)
- No additional labels outside the bar
- Default font size and styling consistent across DayOverviewPage

### Status
- **Implemented**
- Verified with legacy PAD behavior
- Subject to data rule adjustments (tithi/nakshatra mappings) without UI changes

----------

## 🌑🌞 Eclipses

### General
- The application displays **only total and partial solar and lunar eclipses**.
- The number of eclipses is small (usually **4–5 per year**), but they are considered **high importance events** in Jyotish.
- All eclipse calculations are performed **once** using Swiss Ephemeris:
  - `SwissAnalysis.CalculateEclipses_London(fromUtc, toUtc)`
- This method returns an **already filtered list** of required eclipses (`List<EclipseData>`).
- All calculations are based on **London time (GMT-0)**; the eclipse moment is then converted to the **local time zone of the active profile** for display.

### Data and Models
- The `EclipseData` model contains:
  - `EclipseId` — eclipse type  
    - `1` — Lunar eclipse  
    - `2` — Solar eclipse
  - `Date` — exact eclipse moment (GMT-0 / UTC)
- Eclipse type is defined via enum:
  ```csharp
  public enum EEclipse
  {
      MOONECLIPSE = 1,
      SUNECLIPSE = 2
  }
  ```
- Localized eclipse names are loaded from `DataCache.EclipseDescList`
  (database table `ECLIPSE_DESC`).

### Calendar (MainPage)
- In the calendar grid (42 days + buffers), eclipses are displayed as **icons** inside the day cell:
  - `sun_eclipse.png` — solar eclipse
  - `moon_eclipse.png` — lunar eclipse
- Icon placement:
  - positioned **to the right of the day number**
  - size: **16×16 px**
  - additional right margin is reserved for future user events (corner triangles)
- The day model `DayItem` is extended with:
  ```csharp
  public int? EclipseId { get; set; }
  public DateTime? EclipseDate { get; set; } // eclipse moment (UTC / GMT-0)
  public string? EclipseIcon { get; set; }
  ```
- Eclipse-to-day mapping logic:
  - `EclipseDate` is converted to the profile’s local time zone
  - the local date is matched against `DayItem.Date.Date`

### DayOverviewPage
- If an eclipse occurs on the selected day, a **dedicated Eclipse block** is shown.
- The block:
  - is displayed **only when an eclipse exists** (`HasEclipse = true`)
  - **does not occupy layout space** when hidden (no visual gaps)
- Placement:
  - the Eclipse block is located **at the very top of the page**,
    above the Sunrise/Sunset block
- Block content:
  - eclipse icon (24×24 px)
  - localized eclipse name
  - exact local time of the eclipse
- Example display:
  ```
  14:23:05 Solar Eclipse
  ```
- Data is prepared in `DayComputationService` based on `DayItem`
  **without any additional Swiss Ephemeris calculations**.

### Architectural Notes
- Swiss Ephemeris is used **only at the calendar calculation stage**.
- UI pages (Calendar and DayOverview) operate exclusively on **precomputed data**.
- Localization logic is encapsulated in helper methods and `DataCache`.
- The implementation is fully compatible with future extensions:
  - DayPage (daily time scale)
  - user-defined events (appointments)

----------

## DayPage – Timeline-Based Daily View

### Overview
**DayPage** represents a detailed daily view based on a vertical time timeline from **00:00 to 24:00**.  
The page is designed as an interactive astrological diary, combining time scale, user events, and multiple transit lanes in a single synchronized layout.
Each transit lane represents a specific astrological factor (Nakshatra, Tithi, Yoga, etc.) and is rendered dynamically based on precomputed daily data.

This implementation focuses on:
- Reusing already calculated data (no heavy recalculations on DayPage)
- High visual clarity on mobile devices
- Smooth synchronization between timeline, transit lanes, headers, and sticky labels

The page follows the hierarchy:
**MainPage → DayOverview → DayPage**

## Data Flow

1. **MainPage**
   - Performs heavy ephemeris and transit calculations
   - Builds `DayItem` objects for each day

2. **DayOverviewPage**
   - Receives `DayItem`
   - Displays summary blocks
   - Navigates to DayPage, passing the same `DayItem`

3. **DayPage**
   - Receives `DayItem` via `QueryProperty`
   - Reuses:
     - Panchanga segments (Nakshatra, Tithi, etc.)
     - Transit slices
   - No recalculation of ephemeris data is performed

### General Layout
The DayPage layout consists of the following main columns (from left to right):

1. **Icons Column**
   - Narrow column for time-bound icons.
   - Examples:
     - Sunrise / Sunset
     - Solar and Lunar Eclipses
     - Other significant daily markers
   - Icons are vertically positioned according to their exact time.

2. **Time Scale Column**
   - Displays the vertical daily time scale.
   - Time flows from top (00:00) to bottom (24:00).
   - Used as the primary visual reference for all other columns.

3. **User Events Column**
   - Interactive column for creating and displaying user-defined events.
   - Taps are aligned to time grid resolution.
   - Visual grid assists precise time selection.

4. **Transit Lanes Area**
   - Contains multiple parallel vertical lanes for astrological transits.
   - Lanes are horizontally scrollable.
   - Each lane represents one type of transit (e.g. Lagna, Nakshatra, Tithi, etc.).
   - Number of lanes is **not fixed** and can be extended in the future.

### Scrolling Behavior
- **Vertical scrolling**
  - Applies to the entire timeline (icons, time scale, events, and transit lanes).
  - Header row remains fixed.

- **Horizontal scrolling**
  - Applies only to the Transit Lanes area.
  - Transit header, sticky labels, and transit body are horizontally synchronized.

### Time Scale
- Default time resolution: **15 minutes**
- Planned configurable resolutions:
  - 10 minutes
  - 15 minutes
  - 30 minutes

#### Visual Rules
- **15 minutes** – short tick
- **30 minutes** – medium-length tick
- **60 minutes (full hour)**:
  - Full-width tick
  - Time label displayed as `HH:mm`

#### Scale Direction
- Time scale ticks are rendered **from right to left**.
- Right edge of ticks visually connects with the User Events grid.
- This creates a continuous visual time grid between Time Scale and Events column.

#### Pixel Mapping
- **1 minute = 1 pixel**
- Total daily height: **1440 pixels**

### Events Time Grid
- The User Events column displays horizontal grid lines aligned with the Time Scale.
- Grid lines correspond to:
  - 15-minute intervals
  - 30-minute intervals
  - Full hours
- Grid lines extend across the entire Events column width.
- No time text is shown inside the Events column (time labels exist only in the Time Scale column).

## Auto-Centering on Current Time

When opening DayPage:
- If the displayed day is **today (in profile time zone)**, the timeline automatically scrolls so that:
  - The current time is vertically centered when possible
  - Near day start or end, scrolling is clamped to valid bounds

This improves usability and allows immediate orientation around “now”.

#### Purpose
- Visual guidance for user interaction.
- Accurate time-slot selection for creating events.
- Basis for future event hit-testing logic.

### Transit Lanes (Dynamic Columns)
- Each transit lane occupies a fixed-width vertical column.
- Lanes are displayed side by side and scrolled horizontally.
- Transit segments are drawn according to their time spans.
- Segment boundaries are visually separated when a transit changes during the day.

- The number of transit lanes is **dynamic** and driven by the `EDVLineName` enum (IDs `< 100`).
- Lane metadata (code, localized name, short name) is loaded from database tables:
  - `DVLINENAME`
  - `DVLINENAME_DESC`
- Lanes are rendered using a horizontal `ScrollView` with a dynamic `BindableLayout`.

Each lane:
- Has a fixed width (currently `80px`)
- Contains an `AbsoluteLayout` for vertical placement of transit segments
- Uses the same vertical time scale as the main timeline

## Transit Segments Rendering

- Transit segments are rendered as vertical blocks positioned relative to the daily timeline:
  - `Y = minutesFromDayStart * PixelsPerMinute`
  - `Height = durationInMinutes * PixelsPerMinute`
- Segments are **clipped to the current day** (00:00–24:00).
- Support for split-colored segments is included (top/bottom color rendering).

### Segment Separators & Labels
- A thin horizontal separator line is drawn at each segment boundary.
- A compact text label is rendered **just below the separator**, indicating the new segment.
- Labels intentionally omit start time (time is already visible in the left time scale).
- Example label format:
  ```
  NakshatraName
  ```
This prevents visual merging of consecutive segments with the same color and improves readability.

### Sticky Labels for Transit Lanes
Each transit lane supports an optional **sticky label**:
- Sticky labels indicate the **current active transit** at the top of the visible viewport.
- Sticky labels are:
  - **Not part of the header**
  - It is rendered in a transparent overlay above the timeline.
  - Sticky labels:
    - Scroll **horizontally** together with transit lanes
    - Remain **vertically fixed** at the top of the page

### Update Logic
- Sticky labels are updated on **vertical scrolling** of the timeline.
- Current time is derived from:
  ```
  currentMinute = ScrollY / PixelsPerMinute
  ```
- The active segment is determined by checking which segment contains the corresponding time.
- The sticky label text is updated dynamically.

#### Behavior
- Sticky label remains visible while its transit is active.
- When a transit change enters the visible area, the sticky label updates accordingly.
- Sticky labels are **optional per lane**:
  - Visibility is controlled dynamically from code
  - If no label text is provided, the sticky label is hidden
  - Some lanes (e.g. daily Muhurtas) may not use sticky labels at all

#### Visual Style
- Transparent background
- Text only (typically black)
- Does not obscure underlying transit colors

### Visual Styling
- **Day background**: very light blue
- **Night background**: slightly darker tone (planned)
- Timeline ticks, grid lines, and separators use a unified timeline color.
- Opacity levels are used to differentiate:
  - Hour lines
  - Half-hour lines
  - Minor interval lines
- Visual contrast is kept subtle to avoid heavy or aggressive appearance.

### Extensibility
- Transit lanes are not limited to a fixed number.
- Additional lanes can be added without layout changes.
- Time resolution is planned to be configurable via application settings.
- User events functionality will be expanded with detailed editing and navigation flows.

## Current Status

Implemented:
- Dynamic transit lanes
- Implemented several transit lanes
- Segment separators and labels
- Sticky labels synchronized with scroll
- Auto-centering on current time

## Implemented Transit Lanes

-   Nakshatra
-   Tara Bala
-   Tithi
-   Karana
-   Nitya Yoga
-   Chandra Bala

Next steps:
- Add remaining Panchanga and transit lanes
- User-defined event lane (add events feature)

---------

# DayPage – Segment Selection & Tooltip Overlay

## Purpose
This section describes the interaction model and UI implementation for **segment selection** and **tooltip display** on the DayPage transit lanes.
The goal is to provide rich contextual information for any transit segment without breaking the timeline layout or scroll behavior.

## Segment Selection

### Interaction Model
Each transit segment supports a two-step tap interaction:

1. **First tap**
   - Selects the segment
   - Visually highlights the entire segment
   - Closes any open tooltip

2. **Second tap on the same segment**
   - Opens a tooltip with detailed information for that segment

Tapping on a different segment switches selection to the new segment.

## Visual Highlighting

### Highlight Requirements
- The selected segment must be highlighted **without changing its size or position**
- No visual gaps or layout shifts are allowed
- The highlight must cover the **entire visible segment area**

### Implementation
Each segment is rendered inside a wrapper container. The structure is:
```
Wrapper (AbsoluteLayout child)
 ├─ Segment background (BoxView or Grid)
 └─ Highlight overlay (Border)
```
- The segment background fills the entire wrapper
- The highlight overlay is a `Border` rendered **above** the segment
- The border uses `Stroke` only (no background) and is normally hidden
- Highlight visibility is controlled by changing `Opacity`

Because the border is an overlay and not a layout container, it does **not** affect segment geometry or spacing.

## Tooltip Overlay

### General Behavior
- The tooltip is displayed as a centered overlay panel above the DayPage
- The underlying page is dimmed using a semi-transparent backdrop
- The tooltip does not open a new page and does not affect navigation state

### Opening & Closing
- Opened by a **second tap** on the currently selected segment
- Closed by tapping anywhere outside the tooltip panel
- When the tooltip is closed, the segment selection may either remain or be cleared (implementation choice)

## Tooltip Overlay Structure

The tooltip overlay consists of:

- A full-page overlay grid
- A semi-transparent backdrop that captures tap events
- A centered panel containing scrollable content

Key characteristics:
- The overlay is fully hidden and input-transparent when not visible
- When visible, it intercepts all input to prevent interaction with the timeline beneath
- The tooltip panel itself allows vertical scrolling for long content

## Scroll Interaction Compatibility

Special care is taken to ensure compatibility with existing scroll behavior:

- **Vertical scrolling** (timeline):
  - Used to update sticky labels
  - Disabled when tooltip is open

- **Horizontal scrolling** (transit lanes):
  - Remains synchronized across header, body, and sticky layers
  - Disabled when tooltip is open

- **Tap gestures**:
  - Attached only to segment wrappers
  - Do not interfere with scroll gestures

## Data Handling

- Each segment retains its full domain data (`PanchangaSegment`, transit slice, etc.)
- Tooltip content is derived from segment IDs and domain models
- Localized descriptions are resolved at display time using IDs, not preformatted text

This design avoids duplication and ensures consistency across UI components.

## Design Principles

- Overlay-based UI for complex contextual information
- No layout mutation during interaction
- Clear separation between rendering, selection state, and data resolution
- Fully reusable interaction pattern for all transit lanes

## Current Status

Implemented:
- Segment selection by tap
- Full-segment highlight overlay
- Two-step tap interaction model
- Centered tooltip overlay with backdrop
- Safe coexistence with vertical and horizontal scrolling

Planned:
- Structured tooltip content (sections, icons, formatting)
- Reuse of the same selection/tooltip mechanism for all transit lanes

---------

# PADMA – Navigation Data Flow & DayOverview Swipe

## Overview

This document describes the recent architectural changes made to PADMA to improve:

- End-to-end data flow between pages (MainPage → DayOverviewPage → DayPage)
- Reuse of already calculated data (no recomputation)
- Support for horizontal day-to-day navigation (swipe) in DayOverviewPage

These changes are infrastructure-level and do not alter existing astrological algorithms or calculation logic.

## 1. Problem Statement

Before these changes:

- Heavy domain objects (DayItem, transit data, overview results) were partially passed through Shell navigation.
- Some data was lost during navigation, forcing UI pages (especially DayPage) to rely on display text instead of structured models/IDs.
- DayOverviewPage showed a single day only, without the ability to move forward/backward through the already calculated 42-day window.

## 2. NavigationDataStore

### Purpose

`NavigationDataStore` is an in-memory storage used to pass complex navigation payloads between pages without serializing them into Shell query parameters.

This allows:

- Passing rich domain models safely
- Avoiding recalculation
- Keeping navigation lightweight (only a token is passed)

### Key Characteristics

- Singleton service (registered in DI)
- Stores objects by generated string token (GUID)
- Supports Put / Get / Remove operations
- Lifetime: application session (in-memory)

### Conceptual API
```csharp
string token = store.Put(object);
T payload = store.GetRequired<T>(token);
store.Remove(token);
```
## 3. DayNavBundle

### Purpose

`DayNavBundle` is a unified navigation payload that represents all data needed by DayPage (and future extensions).

### Structure

- `DayItem Day`
  - The selected calendar day
  - Contains Panchanga segments and TransitPack (already calculated on MainPage)
- `DayOverviewData Overview`
  - Sunrise/Sunset
  - Overview stripes and other computed day-level summaries
- `DayWindowContext Window` (optional)
  - Full 42-day window
  - Index of the currently selected day
  - Enables day-to-day navigation without recomputation

### Design Goal

DayPage becomes a pure consumer of prepared data, not a recalculation point.

## 4. Updated Navigation Flow

### MainPage → DayOverviewPage

- MainPage builds the 42-day window once (CalendarViewModel).
- A `DayWindowContext` (days + selected index) is stored in NavigationDataStore.
- Only the selected `DayItem` and a `WindowToken` are passed via navigation.

### DayOverviewPage

- Receives:
  - Selected `DayItem`
  - Optional `WindowToken`
- Retrieves `DayWindowContext` from NavigationDataStore.
- Loads `DayOverviewData` via DayComputationService (cached, no recomputation).
- On navigation to DayPage:
  - Builds a `DayNavBundle`
  - Stores it in NavigationDataStore
  - Navigates using a single bundle token.

### DayPage

- Receives only a bundle token.
- Restores full context (`DayItem`, `OverviewData`, window context).
- No recalculation is required.

## 5. DayOverview Horizontal Paging

### Motivation

The 42-day window is already calculated on **MainPage** and kept in memory.

Horizontal paging on **DayOverviewPage** allows:

- Smooth, natural day-to-day navigation (true paging, not gesture-triggered refresh)
- Visual continuity of astrological stripes across adjacent days
- Full reuse of cached overview data (no recomputation)
- Clear temporal perception of how transits evolve from day to day

This behavior closely matches the legacy PAD desktop application and expected calendar UX.

### Implementation

- **`CarouselView`** is used as the core paging mechanism (horizontal orientation).
- Each carousel item represents **one day** and is bound to a lightweight view-model (`DayOverviewItemVm`):
  - `Day` (`DayItem`)
  - `Overview` (`DayOverviewData`, loaded lazily)
- The initial carousel position is set from `DayWindowContext.SelectedIndex`.

#### Data Loading Strategy

- Overview data is loaded **on demand** when a day becomes current.
- Neighboring days (±1) are **preloaded asynchronously** to eliminate visual gaps during swipe.
- All overview requests go through `DayComputationService`, which already provides in-memory caching.

#### Boundary Handling

- Paging is **not circular**.
- When the user reaches the first or last day of the 42-day window, further swipes in that direction are ignored.
- Internal safeguards prevent re-entrant layout updates and UI freezes at boundaries.

### UX Characteristics

- True page-to-page movement (neighboring day visibly slides in).
- No empty or white placeholder screens during swipe.
- No visual "reveal" artifacts typical for action-based swipe controls.
- Buttons and fixed UI elements are placed outside the carousel and remain stationary.

### Notes

- `CarouselView` was chosen specifically to model **time-based paging**, not gesture-triggered commands.
- The solution is stable on desktop emulators and expected to feel even more natural on real touch devices.

## 6. Design Principles Followed

- No changes to Swiss Ephemeris or astrological logic
- No recalculation of already prepared data
- Strict reuse of cached results
- Clear separation between:
  - Calculation
  - Navigation
  - Presentation
- Architecture aligned with legacy PAD behavior
- Solid foundation for:
  - Deep DayPage lanes (planet stripes, Panchanga, Muhurta, etc.)
  - Tooltip rendering based on IDs instead of display text
  - Possible future paging on DayPage

## 7. Current Status

- Navigation data flow stabilized
- DayOverview supports smooth horizontal paging via CarouselView
- Visual continuity between days achieved
- DayPage receives full structured context and is ready for further development

---------

## DayPage Timeline Icons

### Overview

DayPage includes a dedicated **icons column** aligned with the vertical time scale.
Icons are positioned according to exact event times and rendered on the same
day/night background as the timeline.
The icons provide quick visual markers for key daily events without recalculation.

### Implemented Icons

#### 1. Sunrise / Sunset

- Icons:
  - `sunrise.png`
  - `sunset.png`
- Size: **24×24 px**
- Source of data:
  - `SunriseUtc`
  - `SunsetUtc`
- Positioning:
  - Converted from UTC to vertical Y-position using the day start anchor.
  - Centered horizontally within the icons column.
  - Centered vertically on the exact event time.
These icons visually mark the beginning and end of daylight on the timeline.

#### 2. Eclipse Icons (Solar / Lunar)

- Icons:
  - `sun_eclipse.png`
  - `moon_eclipse.png`
- Data source:
  - `DayItem.EclipseId`
  - `DayItem.EclipseDate`
  - `DayItem.EclipseIcon`
- Rendering conditions:
  - Icon is rendered only if an eclipse exists for the day.
- Size policy:
  - Solar eclipse: **20×20 px**
  - Lunar eclipse: **18×18 px**
- Positioning:
  - Vertical position based on `EclipseDate`.
  - Horizontally centered in the icons column.
  - Shares the same day/night background logic as sunrise/sunset.
If multiple icons occur close in time, visual overlap is allowed.
Detailed timing remains available in DayOverview and tooltips.

### Time-to-Position Mapping

All timeline icons use a unified vertical mapping strategy:
- Timeline represents **local day time (00:00 – 24:00)**.
- Conversion flow:
  1. Local day start (`Day.Date.Date`) is converted to UTC.
  2. Event UTC time is mapped to minutes relative to this anchor.
  3. Minutes are converted to pixels using the full timeline height.
This guarantees consistent placement for all time-based elements.

### Design Notes

- Icons are rendered in a dedicated overlay layer (`IconsLayer`).
- Background rendering is shared with the time and events columns.
- Icons do not intercept user input (`InputTransparent = true`).
- No dynamic resizing or animation is required.
- The system is extensible for future icons (custom events, planetary markers, etc.).

### Status

- DayPage icons fully implemented.
- No additional calculations required.
- Visual consistency with DayOverview ensured.

---------


## DayPage Panchanga Labels and Tooltip (ID-based)

### Segment Metadata in `PanchangaSegment`

`PanchangaSegment` was extended with structured references:

- `TransitKind : ETransitKind`
- `TransitId   : int`

These fields identify *what* the segment represents (Nakshatra, Tithi, etc.) and *which* entity instance it refers to.

### Populating `TransitKind` and `TransitId` in `PanchangaHelper`

`PanchangaHelper.BuildSegmentsForDay(...)` was updated to accept optional selectors for segment identity:

- `getKind : Func<TSlice, ETransitKind>?`
- `getId   : Func<TSlice, int>?`

During segment creation, the builder assigns:

- `segment.TransitKind = getKind(slice)` (or `ETransitKind.Unknown` if not provided)
- `segment.TransitId   = getId(slice)` (or `0` if not provided)

Both the generic and non-generic overloads support these parameters, ensuring all Panchanga segment sources can pass IDs without changing existing callers.

### PanchangaSegment Time Model

PanchangaSegment uses full local DateTime boundaries: - TransitStart -
TransitEnd

### Calendar Construction: Panchanga (6 lanes)

When building the 42-day window, all six Panchanga lanes were updated to provide:

- the correct `ETransitKind` per slice type,
- the correct entity ID (e.g., `NakshatraId`, `TithiId`, etc.).

This enables DayPage to render labels and tooltips consistently for all Panchanga lanes.

### DayPage Short Labels

DayPage label rendering was changed to use segment identity:

- Labels are resolved from `seg.TransitKind` + `seg.TransitId`
- Localized entities are pulled from `DataCache` (e.g., `NakshatraDescList` filtered by `CurrentLanguageCode`)
- `seg.Text` is no longer parsed to build labels

Example policy:
- Nakshatra label uses `"{Id}.{Name}"`
- (Other Panchanga lanes follow the same ID-based approach.)
- In some cases it is easier to parse `seg.Text` to get necessary values 

### Unified Tooltip Layout (Title + Range + Blocks)

A unified tooltip layout was introduced on DayPage:

- **Title** (larger font): required
- **Range** (smaller font): required
- **Blocks** (0..N lines): optional, shown only for non-empty fields

The tooltip height adapts to content. Long descriptions are displayed inside a `ScrollView`.

#### Nakshatra Tooltip (first implemented)

For Nakshatra segments:

- **TooltipTitle**: `"{NakshatraId}.{Name}"`
- **TooltipRange**: `"{TransitStart:yyyy-MM-dd HH:mm:ss} – {TransitEnd:yyyy-MM-dd HH:mm:ss}"`
- **TooltipBlocks**: values from `NakshatraDesc` (excluding `LanguageCode` and excluding empty fields)

### Binding / UI Update Notes

`TooltipTitle` and `TooltipRange` were implemented as properties with backing fields that call `OnPropertyChanged()`,
ensuring UI updates correctly when the tooltip is shown.

`TooltipBlocks` uses `ObservableCollection<string>` to update dynamically.

### Current Status

- Panchanga lane labels on DayPage are ID-based and stable.
- Tooltip is implemented using `...Desc` localized data.
- Tooltip is implemented for 6 transit lanes (Nakshatra, TaraBala, Tithi, Karana, NityaYoga, ChandraBala)
- Architecture is ready to extend tooltips to remaining transit kinds .

-----------

# PADMA – DayPage Yoga Lane

This document fixes and describes the final behaviour of **Yoga Day Lane** on **DayPage**. It reflects the current implemented state and should be merged into the main `requirements.md`.

## 1. Purpose

The Yoga Day Lane represents **all Yogas of the day combined into a single timeline lane**.
Unlike DayOverview (where Yogas may appear as separate stripes), on DayPage Yogas are:

- merged into **one lane**
- may **overlap in time**
- visually **interact with each other**

This lane is intended for **visual analysis**, not for raw calculation.

## 2. Input Data

Yoga data is **pre-calculated** and passed to DayPage via navigation bundle. DayPage:

- does **not** perform Swiss calculations
- does **not** recompute Yogas

Each Yoga segment contains:

- YogaId
- StartLocal / EndLocal
- ColorId (semantic meaning: positive / negative / neutral)
- Vara (weekday)
- NakshatraId
- TithiId

## 3. Segment Construction Logic

The Yoga lane is built by **splitting the day timeline into minimal continuous segments**.

### Algorithm

1. Collect all Yoga start/end timestamps within the day
2. Add day start and day end boundaries
3. Sort all time points
4. For each adjacent interval:
   - Determine the set of active Yogas
   - If no Yogas are active → no segment
   - If at least one Yoga is active → create a segment

This supports any number of overlaps (0…N).

## 4. Color Resolution Rules

For each resulting segment:

- If **all active Yogas have the same ColorId** → use that color
- If **different ColorIds are present** → use `EColor.LIGHTGREEN`

Meaning:
> Positive and negative Yogas neutralize each other when overlapping.
No color blending is used.

## 5. Sticky Label Behaviour

Yoga lane uses **sticky labels** with special rules:

- Sticky label text is derived from **all active Yoga names**
- Names are concatenated with spaces
- Sticky label:
  - **appears** when a Yoga segment reaches the top of the visible viewport
  - **disappears immediately** when the timeline enters a gap (no Yoga segment)

This is critical because Yoga lane may contain gaps.

## 6. Tooltip Architecture

Yoga tooltip is built **entirely dynamically** and does **not** rely on the generic header (Title/Range) used by other transit types.

### Key Principles

- Tooltip is composed of **multiple independent Yoga blocks**
- No global title or range is shown
- Each Yoga is rendered as a self-contained block

## 7. Tooltip Block Structure (per Yoga)

Each Yoga block consists of:

1. **Yoga Name** (header)
2. **Exact time interval**
3. **Inner divider** directly under the time interval
4. Body fields:
   - Vara (localized weekday)
   - Nakshatra
   - Tithi
   - Additional Yoga-specific description fields

### Inner Divider Rule

- Divider width equals **exact width of the time interval text**
- Implemented via layout using `Auto` column width
- Divider visually belongs to the Yoga block and does not span the full tooltip width

## 8. Multiple Yoga Blocks

When multiple Yogas are present in one segment:

- Each Yoga is rendered as a separate block
- Blocks are separated by a **full-width divider** and optional spacer

This visually distinguishes different Yogas while keeping them in one tooltip.

## 9. Design Notes

- DayPage UI never recalculates astrological data
- Yoga lane behaviour mirrors legacy PAD logic
- Visualization logic is deterministic and data-driven
- Tooltip rendering is extensible for future Panchanga entities

## 10. Summary

The Yoga Day Lane on DayPage:

- merges all Yogas into a single interactive timeline
- correctly handles arbitrary overlaps
- uses semantic color neutralization
- provides precise, readable, and structured tooltips
- faithfully preserves legacy PAD analytical behaviour

This specification reflects the **final implemented behaviour**.

-----------

# PADMA --- Muhurta Lane Implementation

## 1. Purpose

This document describes the implementation of the Muhurta lane on
DayPage. The Muhurta lane visualizes auspicious and inauspicious
intraday time periods, including correct handling of overlapping
Muhurtas.

## 2. Data Source

Muhurtas are calculated once in DayComputationService and exposed
through DayOverviewData.MuhurtaStripes.

Each MuhurtaOverviewStripe contains: - MuhurtaId - ColorId -
DayStartLocal / DayEndLocal - StartLocal / EndLocal

Stripes with MuhurtaId = 0 (Abhijit on Wednesdays) are filtered out
before use on DayPage.

## 3. Data Transfer

DayOverviewData is transferred to DayPage via DayNavBundle. DayPage
extracts and filters MuhurtaStripes without recalculation.

## 4. Segment Construction (Intersection)

Overlapping Muhurtas are transformed into non-overlapping
PanchangaSegments by: - collecting all time boundaries, - splitting the
day into minimal intervals, - determining active Muhurtas per interval.

Color rules: - single Muhurta → its own color, - overlapping Muhurtas →
mixed (pink) color.

Each segment stores: - TransitStart / TransitEnd, - TransitId, - Text =
"id" or "id1,id2", - resolved Color.

## 5. Rendering

The Muhurta lane uses the existing RenderPanchangaLane pipeline: -
colored blocks, - start and end separator lines, - support for short
intraday segments.

Sticky labels are disabled for Muhurta lane.

## 6. Tooltip

Tooltips are minimal: - title + time range for a single Muhurta, -
multiple formatted sections for overlapping Muhurtas, ordered by actual
start time.

FormattedString is used to render bold headers and secondary range text.

## 7. Design Decisions

-   No new builders or recalculation.
-   Reuse of MuhurtaOverviewStripe.
-   Local intersection logic on DayPage.
-   Visual clarity prioritized.

## 8. Status

Muhurta lane implementation is complete and ready for extension.

---------

# Planets Tooltip (DayPage)

## Overview

The **Planets Tooltip** is displayed on **DayPage** when a user taps a planet transit segment.
It provides detailed contextual information about the current planetary transit.

The implementation **does not introduce new astrological algorithms** and strictly reuses:
- Swiss Ephemeris–based calculations already present in PADMA
- Precomputed `PlanetSlice` data from `TransitPack`
- Cached reference data loaded at application startup (`DataCache`)

The tooltip is assembled dynamically in `DayPage.xaml.cs` and rendered using the same
UI mechanism as other Panchanga tooltips (Nakshatra, Tithi, Yoga, etc.).

## Trigger Conditions

The Planet Pada Tooltip is shown when:
- A user taps on a **planet transit segment** on `DayPage`
- The tapped segment has `TransitKind = Planet`
- The same segment is tapped twice (selection + tooltip pattern)

Entry point:
- `ShowPlanetPadaTooltip(PanchangaSegment seg)`

## Data Sources

### Primary Data
- `PanchangaSegment`
  - `TransitId` → PlanetId
  - `TransitStart`, `TransitEnd` (UTC)
- `PlanetSlice` (from `Day.TransitPack`)
  - `PlanetId`
  - `ZodiacId`
  - `NakshatraId`
  - `PadaId` (**Pada.Id from 108 padas**)
  - `NavamsaZodiacId`
  - `HouseFromMoon`
  - `HouseFromLagna`
  - `StartUtc`, `EndUtc`
  - `NodeType`

### Cached Reference Data (DataCache)
- `PlanetDescList`
- `ZodiacDescList`
- `NakshatraDescList`
- `PadaList`
- `SpecialNavamsaDescList`
- `TransitList`
- `TransitDescList`

### Profile Context
- `BirthPadaMoonId`
- Active timezone
- Active transit mode:
  - `TRANZITMOON`
  - `TRANZITLAGNA`
  - `TRANZITMOONANDLAGNA`

## Time Handling

- All `PlanetSlice` and `TransitPack` times are stored in **UTC**
- Tooltip display times are converted to **profile local time**
- Conversion is performed explicitly in `DayPage` using `TimeZoneInfo.ConvertTimeFromUtc`

## Tooltip Structure

The tooltip consists of **multiple logical blocks**, rendered sequentially.

### 1. Header
- **Title**: Planet name
- **Range**: Local time range of the current planet slice

### 2. Position Block
Displays the current planetary position:

- Zodiac sign
- Nakshatra (formatted as `Id.Name`)
- Pada number (1–4)
- Navamsa:
  - Zodiac name
  - Exaltation / debilitation marker (via `ExaltationUtility`)

### 3. Navamsa Qualities Block
Derived from the current `Pada`:

- **Special Navamsa**
  - Parsed from `Pada.SpecialNavamsa`
  - Names resolved via `SpecialNavamsaDescList`
- **Bad Navamsa**
  - Calculated positionally relative to:
    - Natal Moon Pada
    - Natal Lagna Pada
- **Drekkana**
  - Calculated positionally (Moon and Lagna based)
  - Uses `Pada.Drekkana` and cyclic 108-pada ordering

All calculations reuse legacy PAD algorithms, ported as utility helpers.

### 4. House & Transit Description Block

Transit interpretation depends on the active transit setting:

#### TRANZITMOON
- House from Moon
- Transit description for `(PlanetId, HouseFromMoon)`

#### TRANZITLAGNA
- House from Lagna
- Transit description for `(PlanetId, HouseFromLagna)`

#### TRANZITMOONANDLAGNA
- House from Moon + corresponding transit description
- House from Lagna + corresponding transit description

Both descriptions are shown sequentially.

### 5. Vedha Block 

Vedha is calculated **only for Moon-based transits**
- If active transit mode = TRANZITLAGNA, Vedha block is skipped
- If active transit mode = TRANZITMOONANDLAGNA, Vedha is calculated using Moon houses

Vedha intervals are not limited to the visible day.
For each candidate planet:

- The continuous house range of the target planet is determined
- Zodiac boundaries of the vedha planet are expanded backward and forward
  to real sign-change moments
- Final Vedha intervals are intersections of these extended ranges
- Uses:
  - `Transit.Vedha` value
  - Precomputed `PlanetSlice` house positions
- Intersections are calculated in UTC and displayed in local time
- Vedha is rendered **at the end of the tooltip**

## Design Principles

- No recalculation of ephemeris data at tooltip time
- All heavy computations are done during transit building
- Tooltip logic is purely compositional and read-only
- Strict adherence to legacy PAD behavior
- No duplication of domain logic in UI layer
- Tooltip may show a Busy overlay while assembling heavy blocks (e.g., Vedha),
  but final tooltip is rendered as a single fully built structure

## Implementation Notes

- Helper logic is located in `PlanetTooltipUtility`
- Domain entities (`VedhaEntity`, `DrekkanaEntity`) are placed in `PADMA.Core.Models`
- Localization keys are passed as string identifiers and resolved at UI level
- Tooltip blocks are added using existing `AddTooltipBlock` mechanism
- Zodiac boundary expansion uses SwissAnalysis.FindPreviousZodiacChangeUtc /
  FindNextZodiacChangeUtc with caching
- Ketu zodiac boundaries are resolved via Rahu

## Status

- Planet Tooltip is fully functional

------

# Vedha Calculation Engine

## Overview

The Vedha Calculation Engine determines periods when transiting planets create Vedha (obstruction) for another planet’s transit, according to classical Jyotish rules and legacy PAD behavior.

The engine does not introduce new astrological logic. It strictly reuses:

* PlanetSlice data produced by Planet Transit Builder
* Swiss Ephemeris–based zodiac change detection
* Legacy PAD vedha rules and mappings

Vedha is calculated on demand (e.g., for Planet Tooltip) and is not part of daily Panchanga segment generation.

## Conceptual Model

For a given **target planet** and its active transit period:

1. Determine the house occupied by the target planet (from Natal Moon)
2. Obtain the Vedha house number from Transit reference data
3. Find other planets occupying that Vedha house
4. For each such planet, compute the real zodiac-sign interval during which the Vedha is active
5. Merge overlapping intervals per planet

Vedha represents **periods of time**, not instantaneous flags.

## Scope Rules

* Vedha is calculated **only for Moon-based transits**
* If active transit mode = TRANZITLAGNA → Vedha is skipped
* If active transit mode = TRANZITMOONANDLAGNA → Vedha is calculated using Moon houses

This matches legacy PAD behavior.

## Inputs

* targetPlanetId
* targetStartUtc, targetEndUtc (continuous house range of target planet)
* TransitPack: Dictionary<EPlanet, IReadOnlyList<PlanetSlice>>
* vedhaDom (house number causing vedha)
* nodeType (Rahu/Ketu mode)

## Core Algorithm

Entry function:

`PrepareVedhaPlanetList(...)`

For each planet in TransitPack:

1. Skip if planet == targetPlanet
2. Check if Vedha rule exists for (targetPlanet, candidatePlanet)
3. Iterate candidatePlanet PlanetSlices
4. Use HouseFromMoon
5. If slice.HouseFromMoon != vedhaDom → continue
6. Compute intersection between:

   * targetStartUtc..targetEndUtc
   * slice.StartUtc..slice.EndUtc
7. If no intersection → continue
8. Use a moment inside slice as anchorUtc
9. Determine zodiac boundaries of candidate planet:

   * FindPreviousZodiacChangeUtc
   * FindNextZodiacChangeUtc
10. Create VedhaEntity with:

    * PlanetCode
    * DateStart
    * DateEnd

## Zodiac Boundary Expansion

Vedha duration is defined by **zodiac sign occupancy**, not by nakshatra or pada changes.

For each candidate planet:

* anchorUtc is taken from slice.StartUtc
* Boundaries are expanded to nearest zodiac sign change moments

Helper:
`GetZodiacBoundariesCached(planetId, zodiacId, anchorUtc, nodeType)`

Caching key includes:
planetId : zodiacId : nodeType : anchorDate(yyyyMMdd)
This prevents incorrect reuse of zodiac ranges for fast-moving planets (Moon).

Special case:

* Ketu boundaries are resolved via Rahu
* ZodiacId is shifted by 180° accordingly

## Interval Merging

After collecting all VedhaEntity records:

`MergeVedhaIntervals(...)`

Rules:

* Intervals are grouped by PlanetCode
* Overlapping or touching intervals for the same planet are merged
* Intervals of different planets are never merged together

## Time Handling

* All internal calculations use UTC
* UI converts VedhaEntity.DateStart / DateEnd to profile local time

## Output

List<VedhaEntity>

Each entity:

* PlanetCode
* DateStart (UTC)
* DateEnd (UTC)

## Design Principles

* No Swiss ephemeris calls in UI layer
* No recomputation of planetary states
* Uses only PlanetSlice and SwissAnalysis helpers
* Deterministic and reproducible
* Matches legacy PAD results

## Status

Completed and stabilized.
Vedha correctly supports slow and fast planets, including Moon.

------

# Hora (Planetary Hours)

## Overview
Hora represents planetary hours calculated for a given local day and displayed as a dedicated transit lane on DayPage.  
Each Hora segment is rendered as a colored block corresponding to its ruling planet and labeled with the localized planet name.

PADMA supports three Hora calculation modes, controlled by AppSettings (GroupCode = "HORA"):

- HORADAYNIGHT – 12 daylight horas + 12 night horas (based on sunrise and sunset)
- HORAEQUAL – 24 equal horas between sunrise and next sunrise
- HORAFROM6 – 24 equal horas starting from 06:00 local time

Hora is implemented as a first-class transit kind:
`ETransitKind.Hora = 23`

## Core Model

### HoraSlice
File: Core/Models/Calendar/HoraSlice.cs
```
public sealed class HoraSlice : CalendarSlice
{
    public EHoraPlanet PlanetCode { get; set; }
    public EColor ColorCode { get; set; }
    public bool IsDayLightHora { get; set; }

    public HoraSlice()
    {
        Kind = ETransitKind.Hora;
    }
}
```
## Planet Order

SUN → VENUS → MERCURY → MOON → SATURN → JUPITER → MARS

Starting planet by weekday:

Sunday    → Sun  
Monday    → Moon  
Tuesday   → Mars  
Wednesday → Mercury  
Thursday  → Jupiter  
Friday    → Venus  
Saturday  → Saturn  

## Builder

File: Core/TransitBuilder/HoraTransitBuilder.cs
Main entry point:
```
BuildForLocalDay(
    DateTime dayLocal,
    TimeZoneInfo tzInfo,
    double latitude,
    double longitude,
    double altitude
)
```
Responsibilities:
- Convert local day boundaries to UTC
- Obtain SunriseSlice
- Read active HORA AppSetting
- Dispatch to proper calculation method
- Return List<HoraSlice> in UTC
- Filter slices to those intersecting local civil day

## Calculation Modes

### HORADAYNIGHT (12+12)
- Day = sunrise → sunset / 12
- Night = sunset → next sunrise / 12
- Previous and current astro-days generated (48 slices)
- Filtered to civil day

### HORAEQUAL
- sunrise → next sunrise / 24
- Previous and current periods generated
- Filtered to civil day

### HORAFROM6
- From 06:00 local, step = 1 hour
- Previous and current days generated
- Converted to UTC and filtered

## Localization

HoraPlanet → EPlanet mapping via HoraPlanetMapper  
PlanetId resolved from DataCache.PlanetList  
Localized name via PanchangaHelper.GetPlanetDescEntity

## DayPage Integration

- HoraTransitBuilder.BuildForLocalDay(...)
- PanchangaHelper.BuildSegmentsForDay(...)
- includeStartTimeInText = false
- Assigned to HoraSegments

## Rendering

RenderPanchangaLane(EDVLineName.HORA, HoraSegments)
Sticky label enabled

## Tooltip

Title: Planet name  
Range: dd.MM.yyyy HH:mm:ss – dd.MM.yyyy HH:mm:ss

## Result

Hora is profile-aware, timezone-aware and fully configurable via AppSettings.

---------


# Muhurta30 and Ghati60

## Overview
Muhurta30 and Ghati60 are time-division based Panchanga entities displayed as dedicated transit lanes on DayPage.

Both systems use the same configuration group:

GroupCode = "MUHURTAGHATI"

and support three calculation modes:

- MUHURTAGHATIDAYNIGHT  – based on sunrise/sunset (day + night)
- MUHURTAGHATIEQUAL     – equal divisions from sunrise to next sunrise
- MUHURTAGHATIFROM6    – equal divisions from 06:00 local time

Muhurta30 divides a period into **30 parts**.  
Ghati60 divides a period into **60 parts**.

Both follow identical architectural patterns and differ only by the number of divisions and source tables.

## Transit Kinds
```
ETransitKind.Muhurta30 = 24
ETransitKind.Ghati60  = 25
```

## Core Slice Models

### Muhurta30Slice
File: Core/Models/Calendar/Muhurta30Slice.cs
```
public sealed class Muhurta30Slice : CalendarSlice
{
    public int Muhurta30Id { get; set; }
    public int ColorId { get; set; }

    public Muhurta30Slice()
    {
        Kind = ETransitKind.Muhurta30;
    }
}
```

### Ghati60Slice
File: Core/Models/Calendar/Ghati60Slice.cs
```
public sealed class Ghati60Slice : CalendarSlice
{
    public int Ghati60Id { get; set; }
    public int ColorId { get; set; }
    public bool IsDayLightGhati { get; set; }

    public Ghati60Slice()
    {
        Kind = ETransitKind.Ghati60;
    }
}
```
## Data Sources

Loaded into DataCache:

- Muhurta30List
- Muhurta30DescList
- Ghati60List
- Ghati60DescList

Localization helpers:
```
PanchangaHelper.GetMuhurta30DescEntity(int muhurta30Id)
PanchangaHelper.GetGhati60DescDescEntity(int ghati60Id)
```

## Transit Builders

Files:

- Core/TransitBuilder/Muhurta30TransitBuilder.cs
- Core/TransitBuilder/Ghati60TransitBuilder.cs

Main entry point (both):
```
BuildForLocalDay(
    DateTime dayLocal,
    TimeZoneInfo tzInfo,
    double latitude,
    double longitude,
    double altitude
)
```
Responsibilities:
- Convert local day boundaries to UTC
- Obtain SunriseSlice via SunriseTransitBuilder
- Read active MUHURTAGHATI AppSetting
- Build slices according to selected mode
- Filter slices intersecting local civil day
- Return List of slices in UTC

## Calculation Modes

### 1) MUHURTAGHATIDAYNIGHT

Muhurta30:
- Night before sunrise divided into 15 parts (IDs 16–30)
- Day (sunrise → sunset) divided into 15 parts (IDs 1–15)
- Night after sunset divided into 15 parts (IDs 16–30)

Ghati60:
- Night before sunrise divided into 30 parts (IDs 31–60)
- Day divided into 30 parts (IDs 1–30)
- Night after sunset divided into 30 parts (IDs 31–60)

Slices for previous + current astro-periods are generated and then filtered to civil day.

### 2) MUHURTAGHATIEQUAL

Muhurta30:
- sunrise → next sunrise divided into 30 equal parts

Ghati60:
- sunrise → next sunrise divided into 60 equal parts

Previous and current periods are both generated (buffer) and filtered to civil day.

### 3) MUHURTAGHATIFROM6

Muhurta30:
- 06:00 local → 06:00 next day divided into 30 equal parts

Ghati60:
- 06:00 local → 06:00 next day divided into 60 equal parts

Previous and current periods are generated and filtered to civil day.

This mode does not depend on sunrise.

## Integration into DayPage

1) Call builder:
```
Muhurta30TransitBuilder.BuildForLocalDay(...)
Ghati60TransitBuilder.BuildForLocalDay(...)
```

2) Convert slices to segments:
```
PanchangaHelper.BuildSegmentsForDay(
    slices,
    dayLocal.Date,
    tz,
    DataCache.Instance,
    getColorCode,
    getText,
    getKind,
    includeStartTimeInText:false,
    getId
)
```
Where:
- getText for Muhurta30 → ShortName
- getText for Ghati60   → ShortName
- getId returns Muhurta30Id or Ghati60Id

3) Assign to:
```
Muhurta30Segments
Ghati60Segments
```

## Rendering
```
RenderPanchangaLane(EDVLineName.MUHURTA30, Muhurta30Segments);
RenderPanchangaLane(EDVLineName.GHTATI60, Ghati60Segments);
```
Sticky labels enabled for both.

## Tooltip

Muhurta30 & Ghati60:

- Title: Name
- Range: dd.MM.yyyy HH:mm:ss – dd.MM.yyyy HH:mm:ss
- Body: Description

## Result

Muhurta30 and Ghati60 are fully profile-aware, timezone-aware, configurable via AppSettings, and implemented using the same Slice → Builder → Segment → Lane architecture as other PADMA transit systems.

--------

# User Notes (Events) – Model, Storage & DayPage Interaction Specification

This section describes the data model, storage rules, caching strategy, and DayPage UI interaction
for **User Notes (Events)** in PADMA.

User Notes are personal time-based records created by the user directly on the DayPage timeline.
They originate from legacy PAD behavior and are adapted for mobile interaction.

## 1. Purpose

User Notes represent personal observations, notes, or reminders attached to a specific date/time.

They:

- Are attached to a specific profile
- Have start and end date/time
- Are displayed as colored segments in the Events column on DayPage
- Can later be used for:
  - DayPage visualization
  - Calendar day indicators (triangle in month cell)
  - Tooltip preview of daily notes
  - Notification / reminder scheduling

## 2. Database Table

Existing legacy-compatible table (no structural changes):

```sql
CREATE TABLE IF NOT EXISTS "USER_EVENTS" (
    "ID"        INTEGER,
    "PROFILEID" INTEGER,
    "DATESTART" TEXT,
    "DATEEND"   TEXT,
    "NAME"      TEXT,
    "MESSAGE"   TEXT,
    "ARGBVALUE" INTEGER,
    PRIMARY KEY("ID" AUTOINCREMENT),
    FOREIGN KEY("PROFILEID") REFERENCES "PROFILE"("ID")
);
```

## 3. Date/Time Storage Format

DATESTART and DATEEND are stored as strings using format:
```
yyyy-MM-dd HH:mm:ss
```
Characteristics:
- Exactly 19 characters
- Culture-invariant
- Lexicographically sortable
- Represents **local time of active profile**
- No UTC conversion at DB level

Timezone conversion will be applied later only for notification scheduling.

## 4. Conceptual Model (C#)

```csharp
public sealed class UserEvent
{
    public int Id { get; set; }
    public int ProfileId { get; set; }

    public string DateStart { get; set; } = "";
    public string DateEnd   { get; set; } = "";

    public string Name { get; set; } = "";
    public string Message { get; set; } = "";
    public int ArgbValue { get; set; }

    public DateTime StartLocal => UserEventDateHelper.Parse(DateStart);
    public DateTime EndLocal   => UserEventDateHelper.Parse(DateEnd);
}
```

Helper:

```csharp
public static class UserEventDateHelper
{
    public const string DbFormat = "yyyy-MM-dd HH:mm:ss";

    public static string ToDbString(DateTime dt) =>
        dt.ToString(DbFormat, CultureInfo.InvariantCulture);

    public static DateTime Parse(string s) =>
        DateTime.ParseExact(s, DbFormat, CultureInfo.InvariantCulture);
}
```

## 5. Window Cache

User Notes are loaded through a window-based cache:

`UserEventsWindowCache`

Responsibilities:

- Load all events for:
  - profileId
  - windowStartLocal
  - windowEndExclusiveLocal
- Build index: DayStart → List<UserEvent>
- Provide fast access:

```csharp
bool HasEvents(DateTime dayLocal)
List<UserEvent> GetEventsForDay(DateTime dayLocal)
```

Cache lifecycle:

- After insert/update/delete:
  - Invalidate()
  - ReloadLastWindow()

## 6. Relation to Calendar

CalendarViewModel queries:
```
UserEventsWindowCache.HasEvents(day)
```
If true → draw triangle marker inside month cell.
Calendar is refreshed via MessagingCenter event:
```
"UserEventsChanged"
```

## 7. DayPage Visualization

- Notes displayed in **Events column**
- Vertical position based on:
  - minutes from day start
  - PixelsPerMinute
- Height based on duration
- Color from ARGBVALUE
- Title displayed if space allows

Overlapping events:

- Column-based layout
- UserEventOverlapHelper assigns:
  - ColumnIndex
  - ColumnCount

## 8. DayPage Interaction Rules

### Tap on empty Events area
- Computes minute position from Y
- Rounds to 15 minutes
- Selects slot
- Second tap on same slot → Create Note

### Tap on existing note
- First tap → Select
- Second tap → Edit Note

## 9. Note Editor Overlay

Overlay contains:

- Header: New note / Edit note
- Start TimePicker
- End TimePicker
- Title Entry
- Description Editor
- Color Palette bar
- Buttons: Delete, Cancel, OK

Behavior:

- Create → Insert
- Edit → Update
- Delete → Delete
- If no changes on Edit + OK → close only

Safety rule:

- If End <= Start → End = Start + 15 minutes

## 10. Color Selection

- Palette bar shows current color
- Stored as ARGB int

## 11. Localization

Overlay texts resolved via:
```
Localization.GetLocalizedText(nativeText, langCode)
```

## 12. Messaging & Refresh

After any change:

- Reload cache window
- Rebuild Events column
- Send:
```
MessagingCenter.Send("UserEventsChanged")
```

## 13. Future Extensions

- Reminders / notifications
- Multi-day notes
- Drag & resize
- Repeating notes
- Tooltip preview from calendar

------

# User Notes – Local Notifications (Reminder Service)

This section defines the **Stage 1 implementation** of local notifications for **User Notes** (USER_EVENTS).
The goal is to provide reliable “remind me before start time” behavior even when PADMA is not open,
using **OS-scheduled local notifications**.

This is a **global (app-wide / profile-wide)** reminder setting.  
Per-note overrides are out of scope for this stage.

## 1. Goal and Scope (Stage 1)

### Goal
- Schedule local notifications for upcoming User Notes based on a **global reminder offset**
  (e.g., 5 / 15 / 30 minutes before note start).

### Scope (included)
- Global reminder setting stored in `APPSETTING` under a dedicated `GROUPCODE`.
- OS-level scheduling (no polling, no background timers).
- Refresh scheduling on “convenient occasions”.
- Horizon window scheduling: **7 days ahead**.
- Safety limit: **max 64 notifications** per active profile inside the horizon.
- Simple settings UI page for choosing reminder mode.
- Notification message includes **time range and note name**.

### Out of scope (later)
- Per-note reminder toggle/offset.
- Repeating events.
- Advanced notification channel / sound / vibration customization UI.
- Cross-device sync.

## 2. Global Reminder Setting (APPSETTING)

### 2.1 Database table
Existing table (no structural changes):
```
CREATE TABLE IF NOT EXISTS APPSETTING
(
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    GROUPCODE TEXT,
    SETTINGCODE TEXT,
    ACTIVE SMALLINT
);
```
### 2.2 Setting group
- GROUPCODE = 'NOTEREMINDER'

### 2.3 Options
- OFF
- MIN5
- MIN15
- MIN30

Default:
- OFF is active.

### 2.4 Effective reminder minutes
- OFF   → null  
- MIN5  → 5  
- MIN15 → 15  
- MIN30 → 30  

## 3. Scheduling Strategy

### 3.1 Why OS scheduling
Notifications must work even when PADMA is closed or suspended.

### 3.2 Horizon and limits
- Horizon: 7 days
- Max scheduled per profile: 64

### 3.3 Refresh triggers
- App start (after profile ready)
- "UserEventsChanged"
- "ProfileChanged"
- Reminder setting changed

## 4. Service Design

### Interface
```
public interface IUserNoteReminderService
{
    Task RefreshAsync(CancellationToken ct = default);
    Task CancelAllAsync(CancellationToken ct = default);
}
```
## 5. Notification Provider
```
public interface ILocalNotificationProvider
{
    Task<bool> EnsurePermissionsAsync();
    Task ScheduleAsync(int notificationId, DateTime fireTimeLocal, string title, string body);
    Task CancelAsync(int notificationId);
    Task CancelManyAsync(IEnumerable<int> notificationIds);
}
```
## 6. Scheduling Rules

fireTimeLocal = note.StartLocal - reminderMinutes

Before scheduling:
- If DateTime.Kind == Unspecified → treat as Local
- Else → convert to Local

Skip if:
- reminder OFF
- fireTimeLocal <= now
- outside horizon
- limit reached

NotificationId = note.Id

## 7. Message Content

Title: Personal Astrological Diary  
Body: HH:mm–HH:mm • NoteName

## 8. Status

Stage 1 implemented and verified (Android emulator).

## 9. Future

- Per-note reminders
- Repeat rules
- Channel/sound settings

-------

# Lagna (Ascendant) Transit Support --- DayPage

This section describes the implementation of **Lagna (Ascendant) transit
support** in PADMA, including data models, Swiss calculations, slice
generation, calendar integration, and DayPage rendering.

The Lagna transit is treated as a planet-like dynamic entity whose
longitude changes continuously and produces time slices based on **Pada
changes**.\
Average density is \~108 slices per day (one per Pada).

## 1. Purpose

The Lagna transit stripe provides a continuous timeline of:

-   Zodiac sign of Ascendant
-   Nakshatra of Ascendant
-   Pada of Ascendant
-   Navamsa of Ascendant

This information is displayed as the **first transit stripe on DayPage**
and behaves similarly to planetary stripes.

## 2. Data Model

### LagnaData

Represents a single time point of Lagna state.

Fields:
-   DateTimeUtc : DateTime\
-   Longitude : double\
-   ZodiacId : int\
-   NakshatraId : int\
-   PadaId : int\
-   NavamsaZodiacId : int

Location:
-   PADMA/Core/Models/LagnaData.cs

### LagnaSlice

Calendar slice derived from LagnaData.

Fields:
-   StartUtc : DateTime\
-   EndUtc : DateTime\
-   ZodiacId : int\
-   NakshatraId : int\
-   PadaId : int\
-   NavamsaZodiacId : int\
-   Kind = ETransitKind.Lagna

Location:
-   PADMA/Core/Models/Calendar/LagnaSlice.cs

## 3. Swiss Calculation (Ascendant)

Ascendant longitude is calculated strictly in **UTC** using Swiss
Ephemeris:

SwissService.CalculateAscendantForDate(\
DateTime utcDate,\
double latitude,\
double longitude,\
double altitude,\
char hsys\
)

Important rule:
-   No timezone conversion is applied inside Swiss calculations.
-   Timezone is applied only at UI level (UTC → local).

## 4. LagnaData Calculation

Method:
SwissAnalysis.CalculateLagnaDataList(\
DateTime startUtc,\
DateTime endUtc,\
double latitude,\
double longitude,\
double altitude,\
char hsys\
)

Algorithm:
1.  Iterate from startUtc to endUtc with step = 120 seconds.
2.  For each step:
    -   Calculate Ascendant longitude.
    -   Derive ZodiacId, NakshatraId, PadaId.
    -   Derive NavamsaZodiacId from Nakshatra + Pada.
3.  When state change detected (Zodiac/Nakshatra/Pada):
    -   Binary-search transition moment to 1-second precision.
4.  Add LagnaData entry at each transition.

Result:
Ordered list of LagnaData transition points.

Location:
-   PADMA/Core/Analysis/SwissAnalysis.cs

## 5. LagnaSlice Builder

Method:
LagnaTransitBuilder.BuildLagnaSlices(List`<LagnaData>`)

Logic:
For each LagnaData\[i\]:

-   StartUtc = LagnaData\[i\].DateTimeUtc\
-   EndUtc = LagnaData\[i+1\].DateTimeUtc (or same if last)

Produces ordered list of LagnaSlice.

Location:
-   PADMA/Core/TransitBuilder/LagnaTransitBuilder.cs

## 6. Calendar Integration

During calendar window calculation:
1.  LagnaData is calculated for full buffer window.
2.  LagnaSlices are built once.
3.  For each DayItem, only slices intersecting local day are assigned:

Condition:
slice.EndUtc \> dayStartUtc\
AND\
slice.StartUtc \< dayEndUtc

Stored in:
DayItem.LagnaSlices

## 7. DayPage Rendering

### Stripe

-   Lagna is rendered as the **first transit stripe**.
-   Uses ETransitKind.Lagna.
-   Sticky labels enabled.

### Segment Construction

Each LagnaSlice produces one PanchangaSegment:

-   Start = local(StartUtc)
-   End = local(EndUtc)
-   TransitKind = Lagna
-   TransitId = PadaId
-   Color = derived from NakshatraId + PadaId

Segment Text Format:
`<ZodiacId>`-`<NakShort5>`-`<PadaNumber>`-`<NavamsaZodiacId>`

Example:
3-MRIGA-2-7

## 8. Tooltip (Lagna Segment)

Displayed fields:
-   Zodiac Sign
-   Nakshatra
-   Pada
-   Navamsa
-   Special Navamsa (if any)
-   Malefic Navamsa (relative to natal Moon and natal Lagna)
-   Drekkana information

## 9. Architectural Notes

-   Lagna is NOT part of EPlanet.
-   Lagna is a standalone transit type: ETransitKind.Lagna.
-   Lagna slices are passed separately from TransitPack (planet
    dictionary).
-   All Swiss math is UTC-only.
-   All timezone handling is UI-level only.


## 10. Result

DayPage now provides a complete dynamic Ascendant timeline alongside all
planetary and Panchanga stripes, completing the functional scope of the
page.

------------

# PADMA – Color Settings Page (Syncfusion) Requirements

> Target: `PADMA/Pages/ColorSettingsPage` (inherits `ConfigBasePage`)

## 1. Goal

Provide a configuration page that lets the user fine-tune **application color palette** stored in database tables `COLOR` and `COLOR_DESC`, using **Syncfusion Color Picker** for precise selection.

The page must follow the same UX and behavioral patterns as existing configuration pages (e.g., `HoraPage`, `MuhurtasPage`):
- load current values on page open
- track changes vs original values
- on exit, if changes exist → ask to save / discard
- if saved → persist to DB, refresh cache, notify the app to redraw calendar/UI

## 2. Data model and sources

### 2.1 Tables
- `COLOR` (`AppColor`): `ID`, `CODE`, `ARGBVALUE`
- `COLOR_DESC` (`AppColorDesc`): localized color names: `COLORID`, `NAME`, `LANGUAGECODE`

### 2.2 Enum mapping
- `EColor` values correspond to `COLOR.ID`.

### 2.3 Cache
Already loaded into:
- `DataCache.Instance.ColorList`
- `DataCache.Instance.ColorDescList`

### 2.4 ARGB conversions
Use existing helper methods (do not duplicate logic):
- `CalendarDrawingHelper.ColorFromArgbInt(int argb)`
- `CalendarDrawingHelper.ColorToArgbInt(Color color)`

## 3. UI requirements

### 3.1 General
- Page must inherit `ConfigBasePage` and use the same layout/styling conventions as other config pages.
- All static texts must be localized via `Localization.GetLocalizedText(...)` using `DataCache.Instance.CurrentLanguageCode`.

### 3.2 Layout
- **Top panel**: list of available color entries (localized name with small colored swatch).
- **Bottom area**: buttons **"System default" and "Change"**

> Note: existing config pages apply changes on `OnDisappearing()`.  

## 4. Behavior requirements

### 4.1 Load current colors
On page creation:
1. Read current color values from DB (preferred) or from `DataCache` (acceptable if it reflects DB).
2. Build a view list ordered by `COLOR.ID` (or a defined ordering).
3. Resolve display name from `COLOR_DESC` for current language; fallback to `COLOR.CODE`.

### 4.2 Selection behavior
- Selecting an item in the left list makes it **current selection**.
- Tapping the "Change" button opens a lookup page ColorLookupPage with inline SfColorPicker.
- Changing the color in the picker updates the selected item’s pending value (in-memory).

### 4.3 Change tracking
- The page must track changes relative to the original values loaded at entry.
- If no values changed → leaving the page closes silently (no prompts).
- If any values changed → leaving the page triggers a **Save changes?** dialog, consistent with `HoraPage` / `MuhurtasPage`.

### 4.4 Save prompt on exit (OnDisappearing)
Follow same pattern as other config pages:
- On `OnDisappearing()`:
  - if unchanged → return
  - else show `DisplayAlert(“Save changes?”, “…”, “Yes”, “No”)`
  - if user selects **Yes** → persist changes (see 4.5)
  - if **No** → discard in-memory changes (no DB update)

### 4.5 Persist changes
When saving:
1. Update `COLOR.ARGBVALUE` for each changed row.
2. Refresh cache:
   - `DataCache.Instance.Refresh(db)` (not refreshes colors as for now)
3. Notify app/UI to redraw:
   - `MessagingCenter.Send<object>(this, "SettingsChanged");`

> This message is already used by other config pages and is expected to trigger calendar redraw / UI refresh.

## 5. “System settings” default palette

### 5.1 Purpose
Allow user to restore the **system default** palette for all colors (or a defined subset).

### 5.2 Defaults definition
Defaults must be a constant set defined in code (hardcoded), stable across runs:
- Implemented as `Dictionary<EColor, int /*ARGB*/>` or equivalent.
- Must cover at least all values present in `EColor` (except optional `NOCOLOR`).

### 5.3 Action flow
When user taps **System settings**:
1. Ask confirmation (localized): “Apply system default colors?”
2. If confirmed:
   - set each color’s pending value to its default ARGB
   - mark page as changed (so exiting will trigger save prompt unless user applies immediately)
3. Update picker/preview for current selection accordingly.

## 6. Non-goals / constraints
- Do not introduce a brand-new settings storage mechanism; color values remain in `COLOR` table.
- Keep UX consistent with existing config pages (no unique navigation patterns only for this page).
- Do not refactor unrelated modules while implementing this page.

## 7. Acceptance criteria

1. Opening `ColorSettingsPage` shows list of colors with correct localized names.
2. By default first item in a list is selected.
3. Taping a "Change" button -> open ColorLookupPage with inline sfugion palette selection control (the picker's color is set to selected one).
3. Picker changes update selected item’s swatch and are tracked as unsaved changes.
4. Leaving the page:
   - no changes → no prompt
   - with changes → prompt appears; **Yes** saves to DB, **No** discards
5. After saving:
   - `COLOR.ARGBVALUE` values are updated
   - caches are refreshed
   - `"SettingsChanged"` message is sent
   - calendar/UI redraws using new colors
6. “System default” applies default palette in-memory and participates in the same save/discard logic.

---------

# PADMA -- TransitChartsPage

## Overview

`TransitChartsPage` provides visualization and analysis of planetary
transits using two complementary modes:

1.  **Current Transits**
2.  **Transits from Natal Reference**

The page allows users to inspect current planetary positions and their
relationship to a natal chart using the North Indian chart layout.

This page integrates with the existing PADMA architecture including: -
Swiss Ephemeris calculation modules - `DataCache` -
`ProfileContextService` - `TransitChartDataService` - existing transit
aspect logic

# 1. Page Purpose

The page allows the user to:

-   View **current planetary transits**
-   Analyze **current transits relative to a natal reference point**
-   Inspect **planetary aspects**
-   Navigate through time (date/time controls)
-   Compare transit positions visually and in table format

The page uses the **North Indian chart representation** for the main
chart and a separate **Navamsa chart**.

# 2. Page Structure

The page contains two logical tabs:

## Tab 1 -- Current Transits

Displays the current planetary transit chart based on the **current
Ascendant**.

### Components

-   North Indian transit chart
-   Transit positions table
-   Transit Navamsa chart
-   Aspect controls
-   Date and time controls

### Data Source

Planetary positions are calculated dynamically using:
`SwissAnalysis.CalculatePlanetPositionsForDate()`

The chart is calculated relative to the **current Ascendant**.

### Behaviour

When time changes (via arrows or date/time pickers):

-   Transit chart updates
-   Transit table updates
-   Navamsa chart updates
-   Aspects are recalculated

## Tab 2 -- Transits from Natal Reference

Displays **current planetary transits relative to a natal reference
point**.

Possible references:
-   Lagna
-   Sun
-   Moon
-   Mars
-   Mercury
-   Jupiter
-   Venus
-   Saturn
-   Rahu
-   Ketu

### Chart Construction

1.  Natal reference zodiac is determined.
2.  Zodiac list is **rotated (swapped)** so that the reference zodiac
    becomes **House 1**.
3.  Natal planets are placed in houses according to their natal zodiac.
4.  Current transit planets are placed in houses according to their
    **current zodiac**, using the swapped zodiac structure.

No recalculation of natal positions occurs.

### Planet Types in Chart

Three visual categories exist:

* Natal Planets (black)
-   Fixed positions
-   Derived from natal chart
-   Do not move when time changes

*Transit Planets (colored)
-   Current planetary positions
-   Color determined by house relationship using existing transit color
    logic
-   Move when time changes

* Aspects (grey markers)
-   Represent aspects between **transit planets only**
-   Displayed when aspect filters are enabled

# 3. Static Panels in Natal Mode

In **Natal Reference mode**, two panels remain static:

### Planet Table

Displays **natal planetary positions** only.
Header text changes to:
 `Transits from period ruler <Reference>`

### Navamsa Chart

Displays the **natal Navamsa chart**.

Header text:
 `Natal navamsa`

These panels are **not recalculated when time changes**.

# 4. Time Navigation

The page provides multiple mechanisms to change time:
-   Time shift arrows
-   Date picker
-   Time picker

Behaviour depends on the active tab.

### Current Transits Tab

Time change triggers:
-   Recalculation of planetary positions
-   Chart redraw
-   Table update
-   Navamsa update
-   Aspect recalculation

### Natal Reference Tab

Time change triggers:
-   Recalculation of **transit planets only**
-   Chart redraw
-   Aspect recalculation

Static panels remain unchanged.

# 5. Aspect Controls

Aspect filters allow enabling/disabling aspects for:
-   Sun
-   Moon
-   Mars
-   Mercury
-   Jupiter
-   Venus
-   Saturn
-   Rahu

Aspect logic is applied only to **transit planets**.

When aspect selection changes:
-   The active chart is recalculated
-   Aspect markers are redrawn

Natal planets are not used for aspect calculations.

# 6. Localization

The page supports localization through the existing localization system.

# 7. Core Services Used

The implementation relies on existing PADMA services:

** Swiss Ephemeris
	Planetary positions: - SwissAnalysis - SwissService

** Context Data
-   ProfileContextService
-   DataCache

### Chart Construction

`TransitChartDataService.BuildNatalChartHousesByRuler()`

This method builds the house structure by:
1.  Using the swapped zodiac list
2.  Adding natal planets
3.  Adding current transit planets
4.  Applying transit aspects

# 8. Rendering

Charts are rendered using:
-   NorthIndianChartView
-   NorthIndianChartDrawable

Each house contains:
    ChartHouseData
        HouseNumber
        ZodiacNumber
        Planets

Planets are represented by:
 `ChartPlanetItem`

Planet type determines rendering color and behaviour.

# 9. Summary

`TransitChartsPage` provides a dual mode transit analysis tool
combining:
-   dynamic transit analysis
-   natal relative transit interpretation

** Key design principles:
-   reuse of existing transit logic
-   separation of static natal data and dynamic transit data
-   minimal recalculation when navigating time
-   compatibility with legacy PAD logic

This page mirrors the analytical behaviour of the legacy PAD application
while adapting it to the mobile PADMA architecture.

-------
