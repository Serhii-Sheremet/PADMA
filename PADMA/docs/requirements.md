> Context: This document is used by ChatGPT (GPT-5) for project PADMA continuation.  
> Always load this file first in a new session to resume context.

# 🪶 PADMA — Project Requirements & Current Implementation  

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


# 🌠 Swiss Module — SwissService, SwissAnalysis, SwissUtility, SweConst

**Purpose:**  
Implements high-precision astronomical and astrological computations using the Swiss Ephemeris native library.
All calculations are performed in **UTC (GMT-0) ** and the **sidereal Lahiri** mode by default.
This module provides the foundation for Tithi, Nitya Yoga, Mrityu Bhaga, and Eclipse analyses used in PADMA.

---

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

---

## ⚙️ Initialization & Platform Integration

- Ephemeris data files (`*.se1`) are stored inside `/Resources/Raw/ephe.zip`.
- On first run, the archive is extracted to `FileSystem.AppDataDirectory/ephe/`.
- SwissService automatically sets the path and sidereal mode (Lahiri).
- Platform-specific paths:  
  - **Windows:** via `swedll64.dll` with `AppContext.BaseDirectory/Resources/Raw/ephe/`  
  - **Android:** via `libswe.so` (custom-built from source) with archived `ephe.zip` in `AppContext.BaseDirectory/Resources/Raw/`  
  - **iOS:** to be integrated later with static `libswe.a` (planned)

---

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

### 🪐 Planet Positions

`GetPlanetPosition()` — computes geocentric longitude, latitude, distance, and speed for any planet.

- Uses Swiss Ephemeris (`swe_calc_ut`)
- Returns `SwissResult` object with:
  - `CalculationValues[6]`
  - `Sign`, `IsRetrograde`, `UtcSecondsOfDay`

**Retrograde detection:**  
`IsRetrograde = (speed < 0)`

---

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

---

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

---

### ☠️ Mrityu Bhaga Detection

`CalculateMrityuBhagaDataList_London(int planetId, DateTime fromUtc, DateTime toUtc)` — determines critical zones for each planet within a range.

- Uses pre-defined angular ranges per planet (degrees or sign+degree).  
- Flags `PlanetData.IsInMrityuBhaga = true` when longitude falls within danger range.  
- Output: `List<MrityuBhagaData>` per planet.

---

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

---

### 🧩 Higher-Level Analysis (SwissAnalysis)

`CalculatePlanetDataList_London(startUtc, endUtc)` —
builds list of `PlanetData` entries for all transitions.

Features:
- 1-hour stepping (`3600s`)
- Detects changes in **Sign**, **Nakshatra**, **Pada**, **Retrograde**
- Performs binary search via `FindTransitionEpoch()` for exact UTC time
- Default coordinates: London (`Lon = -0.17, Lat = 51.5`)

---

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

---

## 🧰 Utilities

`SwissUtility` provides:
- `NormalizeDegrees(double)` — ensures [0,360]
- `GetPlanetSWEConstByPlanetId(int)` — mapping PADMA → Swiss constants
- `AdjustForKetu(double)` — adds 180°, wraps to 360°
- `GetZodiakIdFromDegree()`, `GetNakshatraIdFromDegree()`, `GetPadaIdFromDegree()`
- `GetNavamsaByNakshatraAndPada()` — database lookup via `DataCache.Padas`

---

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

---

## 🕒 Time Handling

- All computations are in **UTC (GMT+0, London coordinates)**.
- UI layer performs local time conversions.
- `.NET TimeZoneInfo` and `AdjustmentRules` handle DST.  
- Date/time strings stored in DB as `"yyyy-MM-dd HH:mm:ss"`.

---


# 🌄 Ascendant Calculation — SwissService & SwissUtility

## 📘 Overview

The Ascendant (Lagna) calculation feature has been implemented using the Swiss Ephemeris engine integrated through the SwissService.  
This module computes the **Ascendant longitude** for any date/time and geographic location, including proper handling of **historical time zones**.

---

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Services/SwissService.cs` | Contains the low-level calculation `CalculateAscendantForDate` (core Ascendant computation in UTC). |
| `Core/Utilities/SwissUtility.cs` | Provides `CalculateAscendantWithTimeZone` for high-level usage including local time zone conversion. |
| `Core/Services/TimeZoneService.cs` | Handles historical timezone detection using GeoTimeZone, TimeZoneConverter, and NodaTime. |

---

## ⚙️ External Libraries

| Package | Version | Purpose |
|----------|----------|----------|
| `GeoTimeZone` | 6.1.0 | Determines IANA timezone ID by latitude/longitude (offline). |
| `TimeZoneConverter` | 7.2.0 | Converts between IANA and Windows (.NET) timezone formats. |
| `NodaTime` | 3.2.2 | Provides historical timezone offsets and date-time conversions. |

---

## 🧩 Calculation Flow

### ️⃣ Ascendant Core Calculation (SwissService)

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

---

### ️⃣ Ascendant with TimeZone Adjustment (SwissUtility)

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

---

## 🕒 Historical Time Zone Logic

### TimeZoneService Methods

| Method | Description |
|---------|--------------|
| `GetIanaTimeZoneId(lat, lon)` | Returns IANA zone ID (e.g., "Europe/Kyiv"). |
| `GetDotNetTimeZoneId(lat, lon)` | Returns equivalent Windows ID. |
| `GetUtcOffsetHours(date, lat, lon)` | Returns UTC offset (historical) in hours using NodaTime tzdb. |

---

## 🔍 Notes
- Calculation fully respects historical DST and UTC offsets.  
- Works identically on Windows, Android, and iOS.  
- Uses sidereal mode **Lahiri** by default.  
- Returns absolute ecliptic longitude (0–360°), compatible with all PADMA models.  
- Formatting into degrees/minutes/seconds handled in `FormatDegrees(double degrees)` function (`Core/Utilities/SwissUtility.cs`).

---


# 🌄 Ascendant Calculation — SwissService & SwissUtility

## 📘 Overview

The Ascendant (Lagna) calculation feature has been implemented using the Swiss Ephemeris engine integrated through the SwissService.  
This module computes the **Ascendant longitude** for any date/time and geographic location, including proper handling of **historical time zones**.

---

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Services/SwissService.cs` | Contains the low-level calculation `CalculateAscendantForDate` (core Ascendant computation in UTC). |
| `Core/Utilities/SwissUtility.cs` | Provides `CalculateAscendantWithTimeZone` for high-level usage including local time zone conversion. |
| `Core/Services/TimeZoneService.cs` | Handles historical timezone detection using GeoTimeZone, TimeZoneConverter, and NodaTime. |

---

## ⚙️ External Libraries

| Package | Version | Purpose |
|----------|----------|----------|
| `GeoTimeZone` | 6.1.0 | Determines IANA timezone ID by latitude/longitude (offline). |
| `TimeZoneConverter` | 7.2.0 | Converts between IANA and Windows (.NET) timezone formats. |
| `NodaTime` | 3.2.2 | Provides historical timezone offsets and date-time conversions. |

---

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

---

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

---

## 🕒 Historical Time Zone Logic

### TimeZoneService Methods

| Method | Description |
|---------|--------------|
| `GetIanaTimeZoneId(lat, lon)` | Returns IANA zone ID (e.g., "Europe/Kyiv"). |
| `GetDotNetTimeZoneId(lat, lon)` | Returns equivalent Windows ID. |
| `GetUtcOffsetHours(date, lat, lon)` | Returns UTC offset (historical) in hours using NodaTime tzdb. |

---

## 🔍 Notes
- Calculation fully respects historical DST and UTC offsets.  
- Works identically on Windows, Android, and iOS.  
- Uses sidereal mode **Lahiri** by default.  
- Returns absolute ecliptic longitude (0–360°), compatible with all PADMA models.  
- Formatting into degrees/minutes/seconds handled in `FormatDegrees(double degrees)` function (`Core/Utilities/SwissUtility.cs`).

---

### 🌅 Sunrise and Sunset Calculation

#### **Purpose**
This module calculates the sunrise and sunset times for a given geographic location and date, respecting user-defined configuration (calculation type: by disc edge or by disc center).

---

#### **Main Files**
- `SwissService.cs` — functions to calculate sunrise and sunset times in UTC.
- `SwissEphemerisNative.cs` — P/Invoke declaration for the `swe_rise_trans` function.
- `SweConst.cs` — contains constant definitions used for rise/set calculations (`SE_SUNRISE_TIP`, `SE_SUNRISE_CENTER`, `SE_SUNSET_TIP`, `SE_SUNSET_CENTER`).
- `TimeZoneService.cs` — universal time conversion service (UTC ↔ Local) based on `.NET TimeZoneInfo` and `AdjustmentRules`.

---

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

---

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

---

#### **Notes**
- All Swiss Ephemeris calculations are performed in UTC.  
- Conversion to local time is handled via `.NET TimeZoneInfo`, ensuring compatibility with system settings on all platforms.  
- Minor discrepancies (up to ±1 day or even more) may occur for future years due to known limitations of the Windows time zone database.  
- For historical calculations (e.g., natal charts), `NodaTime` is used — relying on the IANA time zone database for full historical accuracy.

---

### 🧩 Transit Engine Architecture (PADMA)

#### **Purpose**
This section defines the new unified data structure for handling all astrological events (transits) within PADMA.  
The goal is to simplify, optimize, and standardize how all computed Swiss-based entities are stored and rendered in the calendar.

---

#### **Background**
The legacy PAD project used a `Day` class containing ~200 lists of transit-specific calendar objects (e.g., `List<TithiCalendar>`, `List<KaranaCalendar>`, etc.).  
While this approach provided structural clarity, it also caused:
- High memory overhead (due to object duplication and list allocation);
- Complex data binding for UI;
- Inefficient cloning between derived and base calendar classes;
- Difficult extension when adding new transit types.

---

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

---

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

This keeps the system extensible.

### 2.3 Data Sources
The Transit Engine uses:
- Swiss ephemeris raw calculations (`SwissAnalysis`)
- Cached domain dictionaries (`DataCache`)
- User profile information (birth nakshatra, node mode, location)
- Local sunset/sunrise rules (TimeZoneInfo)

---

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

---

## 4. Why Slices?

### Benefits:
- Unified rendering pipeline
- No explosion of Day properties
- Easy filtering in UI
- Infinite extensibility
- Clean decoupling of “calculation” vs “interpretation”

---

## 5. Integration With PADMA

- All domain tables (`Nakshatra`, `Pada`, `TaraBala`, `Zodiac`, etc.) are loaded into `DataCache` at startup.
- Transit Engine reads only Ids (never names).
- All formatting (names, localized strings) happens at the UI/ViewModel layer.

---

## 6. Conclusion

The Transit Engine provides:
- A consistent and powerful method to compute all Vedic astrological timelines.
- A unified format for consumption in UI.
- Clear separation between Swiss calculations, business logic, cached dictionary data, and interface rendering.

---

## 7. Planet Transit Builder

### 7.1 Input
- PlanetData list for a given planet generated by SwissAnalysis
- PlanetId (internal)
- BirthMoonNakshatraId (from profile)

### 7.2 Change Detection
Slices are created whenever one of these fields changes:
- ZodiacId  
- NakshatraId  
- PadaId  
- IsRetrograde

Algorithm: reuse `SwissAnalysis.HasStateChanged(a,b)`.

### 7.3 Slice Contents
Each PlanetSlice contains:
- `StartUtc`, `EndUtc`
- `PlanetId`
- `ZodiacId`
- `NakshatraId`
- `PadaId`
- `NavamsaId` (computed)
- `IsRetrograde`
- `TaraBalaId`, `TaraBalaPercent`
- `Houses (moon and lagna)`
- `Planet Color (moon and lagna)`

### 7.4 Tara Bala Logic
Dependent on:
- BirthMoonNakshatraId
- Swapped nakshatra list (relative to birth)
- 9×3 TaraBala matrix

Each nakshatra falls into:
- Column 0 → 100% (favorable)  
- Column 1 → 50%  
- Column 2 → 25%  

TaraBalaId = rowNumber + 1.

### 7.5 Navamsa Logic
Navamsa = lookup from cached Pada table:
`GetNavamsaByNakshatraAndPada(nakshatraId, padaNumber)`

### 7.6 House and Planet Color Logic
Depend on:
- BirthZodiacId
- BirthLagnaId
- Swapped Zodiac list (both - relative to birth Natal Moon and Lagna)

### 7.7 Output
List<PlanetSlice> chronologically ordered.

### 7.8. Status: Completed

---

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

---

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
List\<TaraBalaSlice>

## 9.8. Status: Completed

---

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

---

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

---

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

---

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

---

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

---