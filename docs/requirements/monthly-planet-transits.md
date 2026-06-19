# Monthly Planet Transits

## 1. Purpose

Monthly Planet Transits page scope, graphics layout, data preparation, selection and detail behavior, no-profile safety, and direct DayPage navigation.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

## Monthly Planet Transits Page

### Purpose

The Monthly Planet Transits page is a mobile adaptation of the legacy PAD monthly transits screen.
Unlike the full legacy desktop screen, the PADMA version focuses only on monthly planetary transit analysis. It does not aim to reproduce all panchanga rows from the legacy screen in the first implementation.
The page is intended to become the main interactive monthly analysis screen for planetary transits.

### Scope

The first implementation includes:

- month day scale;
- weekday scale;
- Masa/Shunya row;
- monthly transit groups for the 9 planets.

The planet display order is:

1. Sun;
2. Moon;
3. Mars;
4. Mercury;
5. Jupiter;
6. Venus;
7. Saturn;
8. Rahu;
9. Ketu.

This order follows the weekday / classical planetary order.

The first implementation excludes:

- global Nakshatra row;
- Tara Bala row;
- Tithi row;
- Karana row;
- Nitya Yoga row;
- Chandra Bala row;
- BTN / VTN Yoga rows;
- other full panchanga rows from the legacy PAD screen.

### Planet Group Structure

Each planet is displayed as a vertical group of transit lanes.
In the first implementation, each planet group contains the same four lanes as in the legacy PAD screen.
The internal data model must not hard-code the assumption that a planet always has exactly four lanes. It must allow additional lanes to be added later without redesigning the page structure.

### Planet Transit Lanes

The initial lane order inside each planet group is:

1. zodiac transit;
2. nakshatra transit;
3. pada transit;
4. tara bala transit.

These four lanes reproduce the current legacy PAD monthly planet transit structure.
The internal model must support adding more lanes later, because the Product Owner may request additional planet transit components in future versions.

### Transit Color Mode

The monthly planet transit page must respect the existing global transit display setting from `EAppSetting`:

- `TRANZITMOON` — display transit result from the natal Moon;
- `TRANZITLAGNA` — display transit result from Lagna;
- `TRANZITMOONANDLAGNA` — display both results in the same interval.

When `TRANZITMOONANDLAGNA` is active, a transit interval is visually split horizontally into two colored parts:

- top part: result from natal Moon;
- bottom part: result from Lagna.

This behavior must be consistent with the existing implementation used by Day Overview / Day Page transit rendering.

### Horizontal Time Axis

The horizontal axis represents one full calendar month.

The top header displays:

- day of week;
- day number;
- visual separation between days.

Transit intervals are drawn proportionally between their start and end time within the month.

### Day Header Markers

The day header highlights special calendar days inside the currently displayed month.
The current local day is highlighted in the day header with a pastel light-blue background, but only when the displayed month/year matches the current system date.
Days with a solar or lunar eclipse are highlighted in the day header with a pastel light-red background.
The highlight is applied only to the day header cell containing:

* day of week;
* day number.

It must not fill the full vertical timeline column.
If the current day and an eclipse day fall on the same date, the current-day highlight may visually take priority. A combined visual style may be added later if required by the Product Owner.
Eclipse day markers are calculated from the existing Swiss eclipse calculation logic and converted to the active profile living timezone before determining the local calendar day.

### Masa/Shunya Row Interaction and Details

The `Masa/Shunya` row is interactive independently from planet transit groups.

When the user taps a day inside the `Masa/Shunya` row:
* the selected Masa/Shunya day is highlighted by a selection rectangle;
* the selection rectangle covers only the selected day cell inside the Masa/Shunya row;
* the selection does not cover planet transit groups.

The first tap selects the Masa/Shunya day.
The second tap on the same selected Masa/Shunya day opens a mobile tooltip/details panel.

The Masa/Shunya tooltip shows all Masa/Shunya detail segments that intersect the selected day, including:
* Masa period;
* Shunya Nakshatra period;
* Shunya Tithi period.

The selected day is only used as a filter. Tooltip periods must preserve their real full start/end boundaries and must not be clipped to the selected day.

Masa tooltip details include:
* Masa name;
* full Masa period;
* Full Moon Nakshatra;
* Full Moon Nakshatra ruler.

Shunya Nakshatra tooltip details include:
* Shunya Nakshatra name;
* full Shunya Nakshatra period;
* Nakshatra ruler.

Shunya Tithi tooltip details include:
* Shunya Tithi name;
* full Shunya Tithi period.

The visual `Masa/Shunya` row label may be composed from separately localized `Masa` and `Shunya` texts.
Tooltip headers may compose `Shunya Nakshatra` and `Shunya Tithi` from separately localized `Shunya`, `Nakshatra`, and `Tithi` texts.
Phrases that require language-specific grammar, such as `Full Moon Nakshatra` and `Full Moon Nakshatra Ruler`, must be localized as full phrases.

### Scrolling

The page must support:

- horizontal scrolling across the month;
- vertical scrolling across planet groups;
- readable fixed or sticky row labels where technically feasible.

The left planet/group label column should remain visible during horizontal scrolling where technically feasible.
The horizontal scroll position of the day header and the transit timeline body must stay synchronized.
The first implementation should prioritize correctness, full data visibility, and interaction over compact visual optimization.

### Rendering Approach

The monthly timeline body should be rendered using `GraphicsView` / custom drawable logic instead of building the table from many individual MAUI controls.
This is required for performance because the page displays many long horizontal time intervals across a full month.
The implementation may use separate drawables or drawing helpers for:

- day header;
- fixed left labels;
- timeline body;
- Masa/Shunya band;
- planet transit segments;
- selection overlay.

Important structural separators may still be drawn by regular XAML elements when this is more reliable than drawing them inside a `GraphicsView`.

### Page Type and Navigation

The Monthly Planet Transits page is a root-level application page available from the burger menu / Shell flyout.

A new flyout item is added:
- Native text: `Transits for month`

The page must behave like other root Shell pages:

- the burger menu must be available from the page;
- the page must not be opened as a nested detail page from Configuration;
- the page must not require the user to press a close button to return to the main application flow;
- switching to another root page is done through the Shell flyout.

The page should be implemented as a dedicated `ContentPage`, not as a configuration/detail page, because it requires a custom two-dimensional scrollable layout, timeline header, sticky labels, selection overlay, and tooltip/details area.
The page may reuse visual styles, localization patterns, helper services, and shared controls from existing pages, but it should not inherit from a template that imposes a vertical `ScrollView` around the whole page if that interferes with horizontal and vertical timeline scrolling.

### Month Navigation and Page Re-entry Behavior

The page initially opens on the current month.
When the user leaves the Monthly Planet Transits page and later opens it again as a root Shell page, the page resets to the current month.
When the user opens the month/year picker from the toolbar title and then closes it, the page must not treat the popup close event as a real page re-entry.
Closing the month/year picker must not automatically reset the page back to the current month.
If the user selects the same month/year that is already displayed, the page must not recalculate monthly transit data.

If the user selects a different month/year:
* the selected month/year is updated;
* the monthly transit data is recalculated;
* the day header is rebuilt;
* all planet transit lanes are refreshed;
* Masa/Shunya data is refreshed;
* eclipse day markers are refreshed;
* current selections and tooltips are cleared;
* scroll positions are reset to the beginning.

The page must safely handle rapid month/year changes.
If a previous monthly calculation is cancelled because a newer calculation has started, the cancellation must be treated as an expected workflow and must not crash the page.

### Selection Behavior

The Monthly Planet Transits page supports two independent selection types:

1. Planet/day selection;
2. Masa/Shunya day selection.

For planet/day selection:
* the selected day and planet are determined from the tap position;
* the selection rectangle covers the full width of the selected day;
* the selection rectangle covers only the selected planet group;
* the rectangle covers all transit lanes of that planet;
* it must not cover the full monthly timeline height.

For Masa/Shunya day selection:
* the selected day is determined from the tap position inside the Masa/Shunya row;
* the selection rectangle covers only the selected day cell inside the Masa/Shunya row;
* it must not cover planet groups.

Selecting a planet/day clears any existing Masa/Shunya selection.
Selecting a Masa/Shunya day clears any existing planet/day selection.

The first tap changes the selection rectangle only.
The second tap on the same selected target opens the corresponding tooltip/details panel.


### Page Header and Toolbar

The Monthly Planet Transits page uses a toolbar/header layout consistent with the existing Main Page calendar toolbar.

The flyout menu item title is localized as:
- Native text: `Transits for month`

However, the page toolbar title itself does not display this static page name.
Instead, the toolbar title area displays the currently selected month and year, localized according to the current application language, for example:

- `Травень 2026`
- `May 2026`

The toolbar contains:
- burger menu button on the left, because the page is a root Shell flyout page;
- localized month/year selector in the title area;
- previous month toolbar button;
- next month toolbar button.

The month/year selector should reuse the same interaction pattern as the existing Main Page month/year selector.
When the user taps the month/year title component, the page opens the same or equivalent month/year selection component used by the Main Page, allowing the user to jump directly to any month and year.

After the user selects another month/year:
- the selected month is updated;
- the toolbar title is refreshed;
- the monthly transit timeline is recalculated;
- the day header is rebuilt;
- all planet transit lanes are refreshed;
- the current interval selection is cleared.

### Reuse of Month Navigation UI

The implementation should reuse the existing Main Page month navigation UI and month/year picker logic where possible.
If the current Main Page implementation is too tightly coupled to the calendar page, the shared month navigation component may be extracted into a reusable control or helper so that both Main Page and Monthly Planet Transits Page use the same behavior.

### Close and Back Behavior

Because the page is a root Shell flyout page, it should not use the standard `close_icon.png` toolbar button used by nested configuration/detail pages.
The user can leave the page by opening the burger menu and selecting another root page.
The Android back button behavior should follow the normal Shell root page behavior and must not accidentally close the whole app or navigate to an invalid previous page.

### Interaction

The page is interactive.

When the user taps inside a transit interval:

For planet/day selection:
- the selected day and planet are determined from the tap position;
- the selection rectangle covers the full width of the selected day;
- the selection rectangle covers only the selected planet group;
- the rectangle covers all transit lanes of that planet;
- it must not cover the full monthly timeline height.

For Masa/Shunya day selection:
- the selected day is determined from the tap position inside the Masa/Shunya row;
- the selection rectangle covers only the selected day cell inside the Masa/Shunya row;
- it must not cover planet groups.

The mobile details area may be implemented as a tooltip, bottom panel, popup, or another suitable mobile UI pattern. The exact presentation may be refined during implementation.

### Transit Details

The details area may show either the selected interval details or a grouped set of related transit details for the selected planet and selected day, depending on the lane and available legacy logic.

Legacy detail fields include:
- transit type;
- start time;
- end time;
- zodiac sign;
- nakshatra;
- pada;
- description;
- vedha from;
- mrityu bhaga.

The exact set of fields may depend on the selected transit lane and must be generated from the same calculation logic as the visual interval.

### Calculation Rules

The page must reuse existing PADMA transit calculation logic where possible.
The implementation should extend the current transit builder approach used by `DayPage`.
Additional builders may be added only for monthly planet transit lanes that are present in legacy PAD but are not currently produced by existing PADMA builders.
The calculation behavior must follow legacy PAD logic unless explicitly changed by the Product Owner.

### Calculation Range

The monthly calculation range covers the full selected calendar month in the active living location timezone.
The implementation must correctly display intervals that started before the beginning of the month or end after the end of the month by clipping them visually to the visible month range.
Regular planet transit builders may calculate with a small time buffer before and after the selected month when needed to detect boundary-crossing intervals correctly.
The Masa/Shunya calculation requires a larger buffer before and after the visible month, because Masa periods are based on New Moon boundaries and may start in the previous calendar month.
The calculation must search far enough before the selected month to find the active Masa period at the beginning of the visible month.

### Profile and Location Context

The page uses the active profile context from `DataCache`.
All calculations must use the same profile, natal data, living location, timezone, ayanamsa/node settings, and app settings as the existing Day Page and transit calculations.
If no active profile is available, the page should follow the same empty/error behavior pattern as other profile-dependent pages.

### Reuse of Existing Transit Logic

The Monthly Planet Transits page must reuse existing PADMA transit calculation, coloring, and split-color logic wherever possible.
If the current logic is embedded in Day Overview or Day Page specific code, it may be extracted into shared services or utilities so that both the existing daily/month overview screens and the new monthly planet transits page use the same source of truth.
The implementation must avoid duplicating transit color rules, Moon/Lagna transit mode handling, and tooltip/detail generation logic.

### Tooltip and Details Reuse

The Monthly Planet Transits page must reuse the existing Day Page tooltip organization where possible.
Transit details should be presented as structured blocks/sections rather than as a single plain text string.
The implementation should reuse existing tooltip content builders, such as planet transit tooltip utilities, and existing Moon/Lagna detail composition logic.
If the current Day Page tooltip rendering code is too tightly coupled to `DayPage`, the shared tooltip structure or rendering helpers may be extracted into reusable UI services/components.
The monthly page tooltip/details UI may have a different mobile presentation if needed, but its content structure and calculation meaning must remain consistent with `DayPage`.


# Monthly Planet Transits — Planet Day Details Requirements

## Planet Day Details

The Monthly Planet Transits page does not select individual transit segments as the main interaction target.
Instead, the user selects a combination of:

- planet;
- calendar day.

When the user taps a planet transit area, the page determines the selected day and the selected planet from the tap position.
The selected area is highlighted by a selection rectangle covering:

- the full width of the selected day;
- all transit lanes of the selected planet;
- only the selected planet group.

The selection rectangle must not cover the whole monthly timeline height. It must be limited to the selected planet group.

## Details Panel Purpose

The page displays a Planet Day Details panel for the selected planet and selected day.
The panel summarizes all relevant transit periods of the selected planet that intersect the selected calendar day.
The details panel is not limited to the visible part of the selected day.
If a transit period started before the selected day or ends after the selected day, the panel must show the real full transit period start and end.

Example:
`15.05.2026 02:52:02 – 15.06.2026 09:23:01`

not:
`01.06.2026 00:00:00 – 01.06.2026 23:59:59`

The selected day is used only as a filter: show all transit periods that are active at any moment during that day.

## Details Panel Structure

The details panel is organized into structured blocks.
The initial block set includes:

- Zodiac;
- Nakshatra;
- Pada;
- Tara Bala;
- Navamsha;
- Vedha;
- Mrityu Bhaga.

Each block may contain one or more rows.
Each row represents a real transit period that intersects the selected day.

The row format should include:

- full period start date/time;
- full period end date/time;
- value/description for that period.

Date/time values should use the full local format:
`dd.MM.yyyy HH:mm:ss`

The display must use the active profile living timezone.

## Period Handling

The details panel must preserve real transit period boundaries.
It must not clip the displayed period ranges to the selected day.
This rule applies to all long-duration and derived transit data, including:

- zodiac transit;
- nakshatra transit;
- pada transit;
- tara bala;
- navamsha;
- vedha;
- mrityu bhaga.

The selected day is only used to determine whether a period should be included:

A period is included if:

- `period.EndLocal > selectedDayStartLocal`
- and `period.StartLocal < selectedDayEndLocal`

### Vedha Calculation for Monthly and Day Tooltips

Vedha intervals must be calculated from real continuous zodiac/house ranges, not from the visible monthly timeline range and not from buffer-limited `PlanetSlice` boundaries.

For Vedha calculation:
* the real continuous house range of the target planet is determined from the target planet’s real zodiac sign boundaries;
* the real continuous house range of the Vedha candidate planet is determined from the candidate planet’s real zodiac sign boundaries;
* the final Vedha interval is the intersection of these two real ranges.

In PADMA, the transit house from the natal Moon or Lagna is determined by the planet’s zodiac sign. Therefore, the real zodiac sign period is used as the real continuous house range for Vedha purposes.

The final Vedha interval must not be:
* the full period of the Vedha candidate planet alone;
* clipped to the selected day;
* clipped to the visible monthly calculation window;
* clipped to a page-specific buffer range.

The same shared Vedha utility logic must be used by both:
* DayPage tooltip;
* Monthly Planet Transits tooltip.

This ensures that the same planet/date combination shows the same Vedha period on both pages.
For slow-moving planets, Vedha periods may start before the visible month and end after the visible month. The tooltip must still display the real full Vedha interval.
Ketu zodiac boundaries must be resolved consistently with PADMA node logic. When required, Ketu period boundaries are resolved through the Rahu/Ketu calculation path used by the Swiss transit utilities.

### Mrityu Bhaga in Monthly Details

Mrityu Bhaga intervals are displayed as overlay periods on the monthly zodiac lane and as detail rows in the tooltip when they intersect the selected day.
Mrityu Bhaga detail periods must preserve real start/end boundaries and must not be clipped to the selected day.
The visual monthly overlay may be clipped to the visible month for drawing purposes, but the tooltip/details data must use the real calculated period.

## Relationship to DayPage Tooltip

The Monthly Planet Transits details panel must reuse existing DayPage tooltip calculation logic wherever possible.
However, unlike DayPage, the monthly page details are not based on one selected segment.

DayPage tooltip:
- selected object: one exact segment;
- period shown: selected segment period.

Monthly transits details:
- selected object: planet + day;
- periods shown: all relevant periods intersecting the selected day;
- each period keeps its own real start/end boundaries.

The visual organization should remain similar to DayPage tooltips:
- title;
- date/planet context;
- structured blocks;
- rows with labels and values.


# Monthly Planet Transits → DayPage Navigation

## Purpose

The Monthly Planet Transits page now supports opening the detailed DayPage directly from the day header row.
This implementation allows the user to inspect a monthly transit chart, select a specific calendar day in the header, and open the full daily timeline for that date using the same DayPage infrastructure that is already used by the MainPage → DayOverviewPage → DayPage flow.
The implementation must not introduce a separate shortcut navigation model based only on date. DayPage remains a consumer of prepared day data passed through `NavigationDataStore` as a `DayNavBundle`.

## User Interaction

### Header day selection

The day header row in `MonthlyPlanetTransitsPage` supports tap interaction.

Behavior:
1. First tap on a day cell in the header:
   - selects the day;
   - draws a gold selection frame around the selected header day cell;
   - clears any active planet transit selection;
   - clears any active Masa/Shunya selection;
   - hides any active tooltip.

2. Second tap on the same selected day cell:
   - starts preparation of a full `DayNavBundle`;
   - shows the existing busy overlay while daily data is prepared;
   - opens `DayPage` for the selected date.

The wait during second tap preparation is acceptable. The Monthly Planet Transits page must remain lightweight and must not precompute all full DayPage data for the month during initial page load.

## Selection Rules

Only one selection frame may be visible at a time on the Monthly Planet Transits page.

Selection priority rules:
- tapping a header day clears:
  - planet transit selection;
  - Masa/Shunya selection;
  - tooltip state.

- tapping a planet transit cell clears:
  - header day selection;
  - Masa/Shunya selection.

- tapping a Masa/Shunya cell clears:
  - header day selection;
  - planet transit selection.

All selection frames on the page use the standard application gold selection color.

## Navigation Model

Navigation from `MonthlyPlanetTransitsPage` to `DayPage` uses the same token-based approach as the existing DayPage flow.

The page must create a `DayNavBundle`, store it in `NavigationDataStore`, and navigate to DayPage using the generated token.

Required navigation pattern:
```csharp
var token = store.Put(bundle);

await Shell.Current.GoToAsync("//day", true,
    new Dictionary<string, object>
    {
        { "token", token }
    });
```

Important: the route must be absolute (`"//day"`), not relative (`"day"`).

Reason:
- `MonthlyPlanetTransitsPage` is a Flyout branch.
- Relative navigation would push DayPage onto the Monthly Planet Transits branch stack.
- After closing DayPage and returning to MainPage, opening Monthly Planet Transits from the burger menu would restore the old DayPage instance instead of the monthly page.
- Absolute navigation opens DayPage as a root Shell route and avoids polluting the Monthly Planet Transits navigation stack.

## DayNavBundle Preparation

The Monthly Planet Transits page must not open DayPage by passing only a date.

DayPage expects a prepared bundle:
```csharp
public sealed class DayNavBundle
{
    public DayItem Day { get; init; }
    public DayOverviewData? Overview { get; init; }
    public DayWindowContext? Window { get; init; }
}
```

For navigation from Monthly Planet Transits:
```csharp
var bundle = new DayNavBundle
{
    Day = dayItem,
    Overview = overviewData,
    Window = null
};
```

`Window` is intentionally `null`, because this navigation does not originate from the 42-day MainPage calendar window and does not require DayOverview carousel navigation.

## MonthlyDayNavBundleBuilder

A dedicated builder is used to prepare the DayPage-compatible bundle for a selected day.

Recommended class:
```text
PADMA/UI/MonthlyTransits/MonthlyDayNavBundleBuilder.cs
```

Responsibility:
- build a full `DayItem` for the selected local date;
- reuse the already calculated monthly `TransitPack`;
- calculate only the additional daily data required by DayPage;
- call `IDayComputationService.GetOverviewAsync(...)`;
- return a complete `DayNavBundle`.

The builder keeps DayPage preparation logic out of `MonthlyPlanetTransitsPage.xaml.cs` and prevents `MonthlyPlanetTransitsDataService` from becoming a second CalendarViewModel.

## Data Sources Used by the Builder

### Reused from monthly data

The builder reuses data already calculated for the monthly transit chart:
```csharp
monthlyData.TransitPack
```

This provides the planet transit slices required by DayPage.

### Calculated lazily on second tap

The following data is prepared only when the user second-taps a header day:
- Panchanga segments:
  - Nakshatra;
  - TaraBala;
  - Tithi;
  - Karana;
  - NityaYoga;
  - ChandraBala.

- Lagna slices for the selected day.
- User event marker state for the selected day.
- Day overview data through `IDayComputationService`.

This avoids increasing initial load time of `MonthlyPlanetTransitsPage`.

## Panchanga Preparation

The builder prepares all six Panchanga lanes required by DayPage.
This is intentional even though the Monthly Planet Transits page currently focuses mainly on planet transits.

Reason:
- DayPage already expects the full Panchanga lane set.
- Tithi may later be displayed directly on the Monthly Planet Transits page.
- Legacy PAD contained Panchanga information on this screen.
- Preparing the full set for DayPage keeps the navigation result complete and compatible with existing daily timeline behavior.

The Panchanga calculation must reuse existing transit builder logic and `PanchangaHelper.BuildSegmentsForDay(...)`.

## Lagna Preparation

Lagna slices are calculated lazily only for the selected day during second tap navigation.

Reason:
- Lagna is needed by DayPage.
- Calculating Lagna for the full month during initial Monthly Planet Transits load is unnecessary.
- A short busy overlay delay on second tap is acceptable.

## DayOverviewData Preparation

The builder must use the existing day computation service:
```csharp
var overview = await dayService.GetOverviewAsync(dayKey, dayItem, ct);
```

The Monthly Planet Transits page must not separately implement Muhurta or day Yoga calculation.

`IDayComputationService` remains responsible for:
- sunrise;
- sunset;
- eclipse overview data;
- planet overview stripes;
- Muhurta stripes;
- VTN/day Yoga stripes.

This keeps the monthly-to-day navigation aligned with the existing MainPage → DayOverviewPage → DayPage flow.

## Eclipse Data

Monthly eclipse markers must preserve the actual eclipse UTC time, not only the local day and eclipse type.

`MonthlyEclipseDayMarker` includes:
```csharp
public sealed class MonthlyEclipseDayMarker
{
    public DateTime DayLocal { get; init; }
    public int EclipseId { get; init; }
    public DateTime EclipseDateUtc { get; init; }
}
```

Reason:
- DayPage and day overview data need the actual eclipse time.
- A marker with only day and type is not sufficient for detailed day navigation.

When building eclipse markers, `EclipseDateUtc` must be populated from the original eclipse UTC date/time.

## Header Hit Testing

The header row has its own tap handler.

`MonthlyTransitsHitTestHelper` contains a header hit-test method:
```csharp
public static DateTime? HitTestHeaderDay(
    MonthlyTransitsLayout layout,
    double x,
    double y)
```

It maps tap coordinates to a local date in the displayed month.
Only taps inside the header height and valid day range produce a date.

## Header Selection Rendering

`MonthlyTransitsLayout` includes:

```csharp
public DateTime? HeaderSelectedDay { get; init; }
```

`MonthlyPlanetTransitsPage.CreateLayout()` passes the current header selection into the layout:

```csharp
HeaderSelectedDay = _headerSelectedDay,
```

`MonthlyTransitsHeaderDrawable` draws a gold rectangle around the selected day cell when `HeaderSelectedDay` belongs to the displayed year and month.

The selected header day frame must use the same standard gold color as other selection frames on the page.

## Busy Overlay

Opening DayPage from Monthly Planet Transits must happen under the existing busy overlay.
The busy overlay is shown only during second tap preparation and navigation.

Expected behavior:
1. User second-taps selected header day.
2. Busy overlay appears.
3. `MonthlyDayNavBundleBuilder.BuildAsync(...)` prepares the bundle.
4. Bundle is stored in `NavigationDataStore`.
5. App navigates to `//day`.

This keeps the monthly page responsive during normal browsing and only performs expensive daily calculations when they are explicitly needed.

## Current Implementation Status

Implemented:
- header day tap handling;
- first tap selection;
- second tap DayPage navigation;
- lazy full daily bundle preparation;
- reuse of monthly `TransitPack`;
- lazy Panchanga preparation for selected day;
- lazy Lagna preparation for selected day;
- reuse of `IDayComputationService` for overview data;
- absolute Shell navigation to `//day`;
- gold selection frame for selected header day;
- mutually exclusive selection frames between header, planet transits, and Masa/Shunya areas;
- eclipse marker extended with `EclipseDateUtc`.

Deferred / possible future extension:
- precomputing a reusable `MonthlyPanchangaPack` if Panchanga or Tithi needs to be displayed directly on the Monthly Planet Transits page.
- visual display of Tithi or other Panchanga lanes on the Monthly Planet Transits page.
