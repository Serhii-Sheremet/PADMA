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
────────────────────────────────────
| Profile name: John Doe            |
| Date of birth: 12.05.1988         |
| Place of birth: Kyiv ⯈           |
| Place of living: Warsaw ⯈        |
────────────────────────────────────
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

> _End of PADMA requirements document_
