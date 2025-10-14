> Context: This document is used by ChatGPT (GPT-5) for project PADMA continuation.  
> Always load this file first in a new session to resume context.

# 🪶 PADMA — Project Requirements & Current Implementation  
> _Version: October 2025_  

---

## 🗓️ Overview  

**PADMA** — cross-platform application built with **.NET 8 (MAUI)** and **SQLite (sqlite-net-pcl)**.  
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
  - (future) Hora, Muhurta, MrityuBhaga, Sunrise
- Toolbar with close icon (returns to `MainPage`).

**Behavior:**
- Subscribes to `"SettingsChanged"` messages from all child pages.  
- On receiving the event → refreshes cache and localized texts.  
- If no changes occurred → closes silently.

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

### 🔹 Localization Consistency  

All visible UI elements (titles, labels, dialogs) must use:  
```csharp
Localization.GetLocalizedText("NativeText", DataCache.Instance.CurrentLanguageCode);
```

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

## 🧮 Project Utilities  

General helper classes used throughout the project.

### 🔹 DateTimeExtensions  
Located in `PADMA/Core/Utilities/Extensions.cs`

**Purpose:**  
Provides reusable date and time helper methods.

**Available methods:**
```csharp
public static bool Between(this DateTime date, DateTime startDate, DateTime endDate)
public static bool StrictBetween(this DateTime date, DateTime startDate, DateTime endDate)
public static DateTime ShiftByUtcOffset(this DateTime date, TimeSpan baseUtcOffset)
public static DateTime ShiftByDaylightDelta(this DateTime date, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
public static float ConvertHoursToPixels(float width, DateTime date)
```

---

## 🚀 Future Enhancements  

- Add “Today” button on MainPage to quickly return to current month.  
- Implement new configuration pages:
  - HoraPage  
  - MuhurtaPage  
  - MrityuBhagaPage  
  - SunrisePage  
- Add event management and astrological calculations for DayPage.  
- Introduce light/dark theme switching via future `ThemePage`.  
- Optimize database initialization and migration tracking.

---

> _End of PADMA requirements document_
