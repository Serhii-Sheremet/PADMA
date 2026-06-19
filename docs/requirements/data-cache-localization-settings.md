# Data Cache, Localization, and Settings

## 1. Purpose

Configuration UI, `APPSETTING`, settings messages, cache refresh, shared visual standards, localization rules, and color-settings requirements.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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
|Notifications|NOTEREMINDER|OFF, MIN5, MIN15, MIN30|Choose notifications method|

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
| USER_EVENTS | Stores user notes linked to profile|

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
