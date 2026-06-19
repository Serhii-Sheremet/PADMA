# Profiles, Locations, and Profile Context

## 1. Purpose

Profiles UI, location search and persistence, profile activation/default behavior, and the profile calculation context lifecycle.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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
Profiles
Transit charts
Settings
About...
FAQs
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
