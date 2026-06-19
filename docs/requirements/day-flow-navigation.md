# Day Flow and Navigation

## 1. Purpose

Progressive calendar-to-day computation, token-based navigation, `NavigationDataStore`, `DayNavBundle`, `DayWindowContext`, and DayOverview horizontal paging.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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
