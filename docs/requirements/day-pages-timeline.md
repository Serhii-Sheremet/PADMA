# Day Overview and DayPage Timeline

## 1. Purpose

DayOverviewPage and DayPage presentation: bars, planet blocks, Muhurtas, Yogas, eclipses, timeline layout, dynamic lanes, sticky labels, icons, and tooltip interaction.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

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

All transit lanes are implemented.


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
