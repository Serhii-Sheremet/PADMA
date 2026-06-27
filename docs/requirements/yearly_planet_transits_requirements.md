# Yearly Planet Transits

## 1. Purpose

The Yearly Planet Transits page provides a compact visual overview of planetary transit
states for all twelve months of a selected calendar year.

The page is a high-level navigation and analysis view. It is intended to help the user
identify notable periods across a full year and then open the existing Monthly Planet
Transits page for a selected month.

The visual approach follows the legacy PAD yearly transit screen while using the current
PADMA transit engine, rendering conventions, and navigation rules.

## 2. Scope

The first implementation includes:

- a selected-year toolbar and year navigation controls;
- a twelve-month horizontal transit matrix;
- Masa/Shunya and planetary transit lanes;
- compact numeric or short transit identifiers inside visual segments;
- synchronized horizontal and vertical scrolling;
- selection of a whole calendar month;
- second-tap navigation to `MonthlyPlanetTransitsPage`;
- safe behavior when no active profile exists.

The first implementation does not include:

- per-segment tooltips;
- a planet-specific details panel;
- selection of individual transit cells;
- direct navigation to DayPage;
- Panchanga display on the yearly page;
- a full date/day header comparable to the monthly page.

## 3. Entry Point

The page is available from the Shell burger menu as:

```text
Transits for year
```

When opened, the page displays the current calendar year by default.

A separate initial modal year-selection dialog is not required. The user must see a
usable yearly overview immediately after opening the page.

## 4. Page Structure

The page follows the established graphics-based structure used by
`MonthlyPlanetTransitsPage`.

Suggested main components:

```text
YearlyPlanetTransitsPage
├── Toolbar
│   ├── selected year title
│   ├── year-picker trigger
│   ├── previous-year button
│   └── next-year button
├── Month header graphics area
├── Fixed labels graphics area
└── Scrollable transit body graphics area
```

The page may use dedicated yearly equivalents of the monthly graphics components, for
example:

```text
YearlyTransitsLayout
YearlyTransitsHeaderDrawable
YearlyTransitsLabelsDrawable
YearlyTransitsBodyDrawable
YearlyTransitsHitTestHelper
```

Existing monthly components may be reused where their assumptions remain valid. The
yearly page must not overload monthly-only classes with year-specific behavior if doing
so would make them difficult to maintain.

## 5. Toolbar and Year Selection

### 5.1 Selected year

The toolbar title displays the selected calendar year.

Example:

```text
☰   2026 ▼                                      ‹   ›
```

The exact visual placement follows the existing page toolbar conventions.

### 5.2 Previous and next year

The toolbar includes previous-year and next-year buttons.

Behavior:

- Previous button selects `selectedYear - 1`.
- Next button selects `selectedYear + 1`.
- A year change clears any selected month.
- A year change starts a new yearly calculation under the standard busy overlay.
- The page refreshes only after the current calculation result is accepted.

### 5.3 Year picker

Tapping the selected year opens the existing `MonthYearPickerPopup` in a year-only mode.

Year-only mode requirements:

- The user selects a year.
- The popup contains `OK` and `Cancel`.
- The popup does not show `Today`.
- The popup does not require month selection.
- `Cancel` leaves the selected year and displayed yearly data unchanged.
- `OK` applies the selected year, clears selection, and recalculates the yearly view.

The picker should follow the same supported-date limits as the rest of the application.

## 6. Yearly Timeline Layout

### 6.1 Month structure

The horizontal timeline represents one continuous calendar year:

```text
1 January selectedYear → 1 January selectedYear + 1
```

The header is divided into twelve month sections.

Each month section:

- has a localized month name;
- occupies a width proportional to its number of calendar days;
- is separated from adjacent months by a clearly visible vertical boundary;
- is the unit of user selection and navigation.

The page does not display a separate day-number header. Days remain an internal
calculation and layout unit used to position transit boundaries correctly.

### 6.2 Transit lanes

The initial yearly view contains:

1. Masa/Shunya lane;
2. Sun;
3. Moon;
4. Mars;
5. Mercury;
6. Jupiter;
7. Venus;
8. Saturn;
9. Rahu;
10. Ketu.

Each planet may contain the same compact sub-lanes already used by the Monthly Planet
Transits page where those sub-lanes are meaningful for the yearly overview.

Examples of displayed compact values include transit identifiers such as zodiac,
nakshatra/pada, house, relative state, or other existing numeric/short labels.

The exact lane composition and label formatting must reuse established PADMA rules from
Monthly Planet Transits and the transit engine. No new astrological interpretation rules
are introduced by this page.

### 6.3 Scrolling

The page supports both directions of scrolling:

- horizontal scrolling across the twelve-month timeline;
- vertical scrolling through all transit lanes.

The month header and body must remain horizontally synchronized.

The fixed left label area must remain aligned with the vertical body lanes.

## 7. Visual Rendering Rules

- Transit segments use the existing PADMA color rules.
- Month boundaries are visually stronger than ordinary internal grid lines.
- The selected month is outlined using the standard PADMA gold selection frame.
- The gold frame covers the selected month across the full interactive body area.
- The page does not display a tooltip, detail panel, or planet-cell-specific highlight.
- The visual design should remain consistent with `MonthlyPlanetTransitsPage`.

## 8. Month Selection and Navigation

### 8.1 First tap

A tap anywhere inside a month body area selects that calendar month.

The selection is not tied to a specific planet, sub-lane, Masa/Shunya segment, or empty
area. The tapped x-coordinate determines the selected month.

On first tap:

1. the selected month is stored;
2. any previous selected-month frame is removed;
3. a gold frame is drawn around the new month;
4. no navigation occurs.

### 8.2 Second tap

A second tap on the same selected month opens `MonthlyPlanetTransitsPage` for that
year and month.

Behavior:

1. the yearly page confirms that the tapped month equals the current selected month;
2. the page navigates to Monthly Planet Transits with the selected `year` and `month`;
3. Monthly Planet Transits performs its normal data preparation and rendering flow.

A second tap on a different month is treated as a new first tap and only changes the
selected month.

### 8.3 Navigation contract

The yearly page passes only the selected calendar values:

```text
year
month
```

The yearly page must not create a `DayNavBundle`, `DayWindowContext`, or DayPage token.

`MonthlyPlanetTransitsPage` remains responsible for:

- accepting the requested year/month;
- updating its own selected month state;
- calculating monthly transit data;
- rendering the existing monthly view;
- handling its normal month-to-DayPage navigation.

The exact transport may use Shell query parameters or another existing simple navigation
contract for scalar navigation values. The implementation must not create a parallel
navigation model unnecessarily.

## 9. Data Preparation

### 9.1 General approach

The yearly page requires one transit data set covering the selected year, with sufficient
buffering to preserve real transit boundaries at the beginning and end of the visual
range.

The calculation range must support:

```text
1 January selectedYear → 1 January selectedYear + 1
```

plus the required builder-specific boundary buffer.

### 9.2 Reuse rules

The yearly implementation must reuse existing PADMA components wherever possible:

- Swiss calculation services;
- profile context;
- active node settings;
- existing transit builders;
- `PlanetSlice` and `TransitPack`;
- Masa/Shunya calculation rules;
- existing color and compact-label rules.

The page must not duplicate legacy or monthly transit calculation logic merely to obtain a
year-shaped visual result.

### 9.3 Service boundary

A dedicated service is expected, for example:

```text
YearlyPlanetTransitsDataService
```

Its responsibility is to prepare data appropriate for the yearly visual model.

The service may share lower-level helpers with `MonthlyPlanetTransitsDataService`. The
two services should not become coupled through page-specific UI assumptions.

The service result should contain only data needed by the yearly page, such as:

- selected year;
- year range metadata;
- planet `TransitPack`;
- Masa/Shunya periods;
- any month-boundary or rendering metadata needed by the layout.

### 9.4 Performance

The yearly calculation may take longer than a monthly calculation.

Requirements:

- calculation runs under the existing busy overlay;
- repeated year changes cancel or supersede obsolete calculations;
- only the latest accepted calculation updates the page;
- the page remains responsive during normal scrolling after data is built;
- no separate calculation is started by first tap selection.

## 10. No-Profile Behavior

The Yearly Planet Transits page must be safe to open when no active profile exists.

Without an active profile:

- no yearly transit calculation is started;
- the page renders a safe skeleton/grid state;
- no exception is thrown;
- year controls may remain visible;
- body/header taps do not navigate.

After a valid active profile is selected, the page must be able to calculate normally on
its next refresh or appearance according to the established profile-change flow.

## 11. Localization

All visible texts follow the existing PADMA localization contract:

```csharp
Localization.GetLocalizedText("Native English Text", DataCache.Instance.CurrentLanguageCode);
```

This includes:

- page title;
- year-picker labels;
- `OK` and `Cancel`;
- busy-overlay text;
- month names;
- any visible lane labels not already resolved through reference data.

## 12. Integration Rules

- Add the page to the Shell burger menu.
- The page must react safely to `ProfileChanged`.
- The page must react safely to relevant `SettingsChanged` events.
- A language change updates localized month names and visible labels.
- A settings/profile update clears stale selection and stale yearly data before refresh.
- Existing MainPage, Monthly Planet Transits, DayOverviewPage, and DayPage navigation
  flows must not be changed by this feature.

## 13. Deferred Extensions

The following are intentionally outside the initial implementation:

- tooltips and details for yearly transit segments;
- direct DayPage navigation from a day-level yearly hit target;
- Panchanga or Tithi lanes on the yearly page;
- special marker overlays beyond the established Masa/Shunya and planet transit display;
- a user-configurable initial year instead of the current year.

## 14. Acceptance Criteria

The first implementation is accepted when:

1. The burger menu opens Yearly Planet Transits.
2. The initial view shows the current year when an active profile exists.
3. The toolbar can move backward/forward by one year.
4. The year-only picker changes the displayed year with `OK` and preserves it with `Cancel`.
5. The view shows twelve localized month sections.
6. The page renders Masa/Shunya and the planned planetary lanes using existing PADMA colors
   and compact transit labels.
7. Horizontal and vertical scrolling work without misalignment.
8. First tap selects a whole month with one gold frame.
9. Second tap on the same month opens Monthly Planet Transits for the correct year/month.
10. Opening the page without an active profile does not throw and displays a safe skeleton state.
11. Existing monthly and daily navigation continues to work unchanged.
