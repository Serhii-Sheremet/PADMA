# Main Calendar

## 1. Purpose

MainPage and its month-grid behavior: buffered calendar data preparation, Panchanga bars, month/year picker, day interaction, and planet/eclipses markers.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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
