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
* *(future)* ThemePage, NotificationPage, TransitsPage, NodesPage, HoraPage, MuhurtaPage, MrityuBhagaPage, SunrisePage.

All configuration pages send a unified **`MessagingCenter.Send(this, "SettingsChanged")`** message after saving settings.
MainPage listens once to this message and updates itself (title, culture, header grid).
Additionally, **OnAppearing()** ensures the calendar refreshes even if the event is missed.

---

### 🈸 **LanguagePage**

| Feature     | Description                                                                               |
| ----------- | ----------------------------------------------------------------------------------------- |
| Purpose     | Select UI language                                                                        |
| Languages   | English (en), Українська (uk), Polski (pl), Русский (ru)                                  |
| Layout      | List with radio buttons, flag icons, language names                                       |
| Behavior    | Updates `APPSETTING` (`LANGUAGE` group), refreshes `DataCache`, sends `"SettingsChanged"` |
| Persistence | Language persists after restart                                                           |
| Dialogs     | Localized confirmation (“Save changes?” / “Yes / No”)                                     |
| Text        | Instruction label: “Choose application language:”                                         |

---

### 📅 **FirstDayOfWeekPage**

| Feature       | Description                            |
| ------------- | -------------------------------------- |
| Options       | Monday / Sunday                        |
| Updates       | `APPSETTING` (`WEEK` group)            |
| Confirmation  | “Save changes?” dialog                 |
| On Change     | Updates DB + cache + notifies MainPage |
| Localization  | All texts localized                    |
| Current value | Read from DB on load                   |

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

```csharp
GetAppSettingsList()
SetFirstDayOfWeek(string code)
SetLanguage(string code)
GetFirstDayOfWeekFromDb()
GetActiveLanguageCode()
GetAppTextsList(string languageCode)
GetLanguages()
GetColors(), GetColorDescs()
GetPlanets(), GetPlanetDescs()
```

---

#### 🔹 DataCache

Central in-memory cache initialized at startup.

**Responsibilities**

* Loads all reference and localized data (`LoadAll`).
* Refreshes localized data and app settings on configuration change (`Refresh`).
* Stores `CurrentLanguageCode` for the active UI culture.

---

#### 🔤 Localization System *(updated October 2025)*

All user-facing texts in PADMA must support full multilingual localization across **four languages**:
🇬🇧 English (`en`), 🇺🇦 Ukrainian (`uk`), 🇵🇱 Polish (`pl`), 🇷🇺 Russian (`ru`).
Localized texts are stored in the `APP_TEXTS` table and can be dynamically refreshed from the database.

🧱 Localization Data Management

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

#### 🔹 ServiceLocator

Simple DI container wrapper for accessing shared services globally:
`ServiceLocator.Services.GetService<DatabaseService>()`

---

### 🧠 ViewModels

#### CalendarViewModel

* Holds current year, month, culture, and collection of day items.
* Generates a 6 × 7 grid of days respecting first-day-of-week setting.
* Supports localization via `CultureCode` → `CultureInfo`.
* Methods:

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

✅ **End of Document**

> Next revision planned after implementation of ThemePage and Transits visualization module.
