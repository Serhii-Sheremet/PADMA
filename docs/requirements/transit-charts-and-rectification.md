# Transit Charts and Birth Time Rectification

## 1. Purpose

TransitChartsPage requirements and Birth Time Rectification Preview scope, navigation, data flow, controls, and deferred extensions.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

# PADMA -- TransitChartsPage

## Overview

`TransitChartsPage` provides visualization and analysis of planetary
transits using two complementary modes:

1.  **Current Transits**
2.  **Transits from Natal Reference**

The page allows users to inspect current planetary positions and their
relationship to a natal chart using the North Indian chart layout.

This page integrates with the existing PADMA architecture including: -
Swiss Ephemeris calculation modules - `DataCache` -
`ProfileContextService` - `TransitChartDataService` - existing transit
aspect logic

# 1. Page Purpose

The page allows the user to:

-   View **current planetary transits**
-   Analyze **current transits relative to a natal reference point**
-   Inspect **planetary aspects**
-   Navigate through time (date/time controls)
-   Compare transit positions visually and in table format

The page uses the **North Indian chart representation** for the main
chart and a separate **Navamsa chart**.

# 2. Page Structure

The page contains two logical tabs:

## Tab 1 -- Current Transits

Displays the current planetary transit chart based on the **current
Ascendant**.

### Components

-   North Indian transit chart
-   Transit positions table
-   Transit Navamsa chart
-   Aspect controls
-   Date and time controls

### Data Source

Planetary positions are calculated dynamically using:
`SwissAnalysis.CalculatePlanetPositionsForDate()`

The chart is calculated relative to the **current Ascendant**.

### Behaviour

When time changes (via arrows or date/time pickers):

-   Transit chart updates
-   Transit table updates
-   Navamsa chart updates
-   Aspects are recalculated

## Tab 2 -- Transits from Natal Reference

Displays **current planetary transits relative to a natal reference
point**.

Possible references:
-   Lagna
-   Sun
-   Moon
-   Mars
-   Mercury
-   Jupiter
-   Venus
-   Saturn
-   Rahu
-   Ketu

### Chart Construction

1.  Natal reference zodiac is determined.
2.  Zodiac list is **rotated (swapped)** so that the reference zodiac
    becomes **House 1**.
3.  Natal planets are placed in houses according to their natal zodiac.
4.  Current transit planets are placed in houses according to their
    **current zodiac**, using the swapped zodiac structure.

No recalculation of natal positions occurs.

### Planet Types in Chart

Three visual categories exist:

* Natal Planets (black)
-   Fixed positions
-   Derived from natal chart
-   Do not move when time changes

*Transit Planets (colored)
-   Current planetary positions
-   Color determined by house relationship using existing transit color
    logic
-   Move when time changes

* Aspects (grey markers)
-   Represent aspects between **transit planets only**
-   Displayed when aspect filters are enabled

# 3. Static Panels in Natal Mode

In **Natal Reference mode**, two panels remain static:

### Planet Table

Displays **natal planetary positions** only.
Header text changes to:
 `Transits from period ruler <Reference>`

### Navamsa Chart

Displays the **natal Navamsa chart**.

Header text:
 `Natal navamsa`

These panels are **not recalculated when time changes**.

# 4. Time Navigation

The page provides multiple mechanisms to change time:
-   Time shift arrows
-   Date picker
-   Time picker

Behaviour depends on the active tab.

### Current Transits Tab

Time change triggers:
-   Recalculation of planetary positions
-   Chart redraw
-   Table update
-   Navamsa update
-   Aspect recalculation

### Natal Reference Tab

Time change triggers:
-   Recalculation of **transit planets only**
-   Chart redraw
-   Aspect recalculation

Static panels remain unchanged.

# 5. Aspect Controls

Aspect filters allow enabling/disabling aspects for:
-   Sun
-   Moon
-   Mars
-   Mercury
-   Jupiter
-   Venus
-   Saturn
-   Rahu

Aspect logic is applied only to **transit planets**.

When aspect selection changes:
-   The active chart is recalculated
-   Aspect markers are redrawn

Natal planets are not used for aspect calculations.

# 6. Localization

The page supports localization through the existing localization system.

# 7. Core Services Used

The implementation relies on existing PADMA services:

** Swiss Ephemeris
	Planetary positions: - SwissAnalysis - SwissService

** Context Data
-   ProfileContextService
-   DataCache

### Chart Construction

`TransitChartDataService.BuildNatalChartHousesByRuler()`

This method builds the house structure by:
1.  Using the swapped zodiac list
2.  Adding natal planets
3.  Adding current transit planets
4.  Applying transit aspects

# 8. Rendering

Charts are rendered using:
-   NorthIndianChartView
-   NorthIndianChartDrawable

Each house contains:
    ChartHouseData
        HouseNumber
        ZodiacNumber
        Planets

Planets are represented by:
 `ChartPlanetItem`

Planet type determines rendering color and behaviour.

# 9. Summary

`TransitChartsPage` provides a dual mode transit analysis tool
combining:
-   dynamic transit analysis
-   natal relative transit interpretation

** Key design principles:
-   reuse of existing transit logic
-   separation of static natal data and dynamic transit data
-   minimal recalculation when navigating time
-   compatibility with legacy PAD logic

This page mirrors the analytical behaviour of the legacy PAD application
while adapting it to the mobile PADMA architecture.


# PADMA -- Birth Time Rectification Preview

## Overview

A new feature must be added to the profile creation and profile editing
workflow to support **birth time rectification**.

The purpose of this feature is to allow the user to preview a natal
chart **before saving the profile**, adjust the birth time, and then
apply the selected time back to the profile form.

This feature is intended to support situations where the exact birth
time is uncertain and must be tuned manually by visually analyzing the
chart and related data.

The implementation should reuse the existing PADMA chart infrastructure
and as much of the already completed `TransitChartsPage` functionality
as possible, while keeping the user flow focused on **natal chart
rectification**.

## 1. Main Goal

The feature must allow the user to:

-   open a separate preview page from profile creation/editing,
-   build a natal chart from the currently entered profile birth data,
-   adjust the birth time using compact time shift controls,
-   immediately see the chart update,
-   inspect related natal data,
-   optionally inspect transit-based supporting views,
-   apply the selected birth time back to the profile page.

This feature is not a replacement for normal profile editing.
It is an auxiliary tool used during profile creation or profile
correction.

## 2. Entry Point

The feature must be available from the existing profile create/edit
page.

### Source page
-   profile details page used for creating a new profile,
-   the same page in edit mode for an existing profile.

### Trigger

A dedicated UI control must be added on the profile page.
At this stage the exact final control type is not fixed yet.
Possible options may include:
-   button,
-   icon button,
-   tappable row,
-   compact action element near birth date/time controls.

The final control type will be decided later during UI implementation.

### Preconditions

The preview page should open only if the minimum required birth data is
available:
-   date of birth,
-   time of birth,
-   place of birth.

If required data is missing, the page should not open and the user
should receive a clear validation message.

## 3. Navigation Model

The rectification tool must open as a **separate lookup-style page**,
similar in interaction concept to the existing place lookup pages.

### Behaviour

-   open from the profile page,
-   work independently as a temporary editor/preview tool,
-   return the selected adjusted birth time back to the profile page,
-   not save the profile automatically,
-   not modify database data directly from the preview page.

The preview page returns only the selected birth time (and, if needed
internally, the resulting adjusted local birth datetime) back to the
profile form.

Actual profile saving remains the responsibility of the profile page.

## 4. Core Use Case

Typical flow:
1.  User opens profile creation or editing.
2.  User enters or reviews:
    -   birth date,
    -   birth time,
    -   place of birth.
3.  User opens the birth time rectification preview.
4.  Preview page builds the natal chart from the current entered values.
5.  User adjusts time using time shift controls.
6.  Chart and related panels update accordingly.
7.  User confirms the selected time.
8.  Preview page closes.
9.  Selected time is written back to the profile page birth time
    control.
10. User continues editing or saves profile.

## 5. Preview Page Scope

The new preview page is conceptually similar to `TransitChartsPage`, but
specialized for birth time rectification.

### It should include:
-   natal chart,
-   natal planetary positions table,
-   Navamsa chart,
-   aspect controls,
-   time shift controls,
-   current selected birth time display,
-   apply/confirm action,
-   cancel/back action.

### It should not include, for the first implementation:

-   full date picker workflow from `TransitChartsPage`,
-   full transit tab switching,
-   unrelated profile editing fields,
-   automatic profile save.

## 6. Chart Behaviour

The main chart on the rectification preview page must represent the
**natal chart** for the currently selected birth moment.

### Recalculation trigger

Whenever the user changes the time using the time shift controls:
-   natal chart must recalculate,
-   natal planetary positions table must update,
-   Navamsa chart must update,
-   aspect display must update if enabled.

### Data basis

The chart must be calculated from:
-   currently selected local birth date/time,
-   selected place of birth,
-   the correct historical timezone for that place and date.

The implementation must use the same corrected UTC conversion logic
already established in the project, including correct `DateTimeKind.Utc`
handling before Swiss planetary calculations.

## 7. Time Adjustment Controls

The page must provide compact time tuning controls intended specifically
for birth time rectification.

### Supported step units

Only the following units are required:
-   seconds,
-   minutes,
-   hours.

### Not required for this page

At this stage, the following step units are not required:
-   days,
-   months,
-   years.

### Interaction

The user must be able to:
-   decrease time by selected unit,
-   increase time by selected unit,
-   switch the active step unit.

The controls should behave similarly to the already implemented step
navigation logic on `TransitChartsPage`, but in a simplified form.

## 8. Date and Time Presentation

On entering the preview page, the initial time must match the currently
selected birth date/time from the profile page.

For the first implementation:
-   the page must display the currently selected birth date/time,
-   time changes must be driven by step controls,
-   dedicated date picker and time picker controls are not required
    initially.

This is intentional to keep the page focused on rectification workflow
and reduce UI complexity.

Date/time pickers may be considered later if needed.

## 9. Natal Planetary Positions Table

The page must include a table showing natal planetary positions for the
currently selected birth moment.

### Purpose

This table helps the user inspect how the chart changes while adjusting
birth time.

### Behaviour

The table must update whenever the selected birth time changes.

### Expected content

The exact columns will match the already established chart table pattern
in the project, including data such as:

-   Planet (2 first chars),
-   Degree (degrees, minutes, seconds),
-   Rasi,
-   Nakshatra,
-   Pada,
-   Navamsa

## 10. Navamsa Chart

The page must include a Navamsa chart for the currently selected birth
moment.

### Behaviour

The Navamsa chart must update together with the natal chart and table
whenever the time changes.

### Purpose

This supports rectification analysis by showing how divisional placement
changes with time adjustments.

## 11. Aspect Controls

The page must include aspect controls.

### Behaviour

-   user can enable/disable selected aspect groups,
-   chart must update accordingly,
-   aspect markers must be recalculated for the currently selected birth
    chart state.

The exact aspect control UI may reuse patterns from `TransitChartsPage`.

## 12. Data Flow

The preview page must work from temporary input values supplied by the
profile page.

### Input data from profile page

-   local birth date/time,
-   place of birth,
-   any additional required calculation settings already used globally
    in the app.

### Output back to profile page

-   adjusted local birth time,
-   or adjusted local birth datetime if that is more convenient for the
    receiving page.

### Important rule

The preview page must **not** become the owner of profile persistence.

It only previews and returns the selected value.

## 13. Reuse of Existing Implementation

The feature should reuse as much existing infrastructure as practical.

### Expected reusable parts

-   `NorthIndianChartView`
-   `NorthIndianChartDrawable`
-   chart house building logic from current chart services
-   natal and chart table building logic already used in
    `TransitChartsPage`
-   step navigation logic pattern
-   aspect selection logic pattern
-   localization approach already used on chart pages
-   corrected timezone/UTC conversion logic already validated against
    legacy PAD

### Simplifications compared to `TransitChartsPage`

-   no dual-tab mode,
-   no current-transit-only mode,
-   no natal reference dropdown,
-   no transit-color chart mode,
-   no date/time picker requirement in first version.

## 14. Return and Apply Logic

The preview page must provide an explicit user action to apply the
selected time.

### Apply action

When the user confirms:

-   the selected birth time must be returned to the profile page,
-   the profile page must update its displayed birth time accordingly.

### Cancel / Back action

If the user leaves without applying:

-   no changes should be written back,
-   the profile page must remain unchanged.

The exact technical return mechanism may be decided during
implementation, for example: - messaging, - callback, - navigation
result pattern, - shared temporary state.

This will be chosen based on the project's existing navigation
practices.

## 15. Modes of Use

The same feature must work in both scenarios:

### Create mode

Used before a new profile is saved for the first time.

### Edit mode

Used when correcting the birth time of an already existing profile.
The workflow and page behaviour should remain consistent in both cases.

## 16. Validation and Stability

The implementation must preserve the already validated calculation
chain:

-   local birth time,
-   historical timezone detection,
-   UTC conversion,
-   Swiss planetary calculation,
-   ascendant calculation,
-   chart rendering.

This is especially important because previous testing already confirmed
that correct `DateTimeKind.Utc` handling is required for planetary
positions to match the legacy PAD application.

The rectification preview must use the same corrected approach.

## 17. Future Extensions

The first version should remain focused and practical.

Possible future extensions may include:
-   date/time pickers if needed,
-   extra divisional charts,
-   comparison tools

These are not part of the initial implementation scope.

## 18. Summary

A new lookup-style preview page must be added to the profile workflow to
support **birth time rectification**.

The page should:

-   open from profile create/edit,
-   build a natal chart using current entered birth data,
-   allow time tuning with seconds/minutes/hours,
-   update natal chart, table, Navamsa and aspects,
-   return the selected time back to the profile page,
-   avoid direct profile saving.

This feature is intended to provide a practical and domain-correct tool
for adjusting uncertain birth time before saving or updating a profile.
