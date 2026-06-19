# PADMA Architecture

## 1. Purpose

Solution structure, startup lifecycle, shared services, core database conventions, and global implementation standards.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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
