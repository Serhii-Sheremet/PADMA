# Swiss Calculations

## 1. Purpose

Swiss Ephemeris native integration and low-level astronomical/astrological calculations: planets, nodes, ascendant, time zones, sunrise/sunset, eclipses, and Mrityu Bhaga.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

# 🌠 Swiss Module — SwissService, SwissAnalysis, SwissUtility, SweConst

**Purpose:**
Implements high-precision astronomical and astrological computations using the Swiss Ephemeris native library.
All calculations are performed in **UTC (GMT-0) ** and the **sidereal Lahiri** mode by default.
This module provides the foundation for Tithi, Nitya Yoga, Mrityu Bhaga, and Eclipse analyses used in PADMA.

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Analysis/SwissAnalysis.cs` | High-level astrological analysis and event detection (transitions, yoga, eclipses) |
| `Core/Services/SwissService.cs` | Low-level computation services wrapping native Swiss Ephemeris |
| `Core/Utilities/SwissUtility.cs` | Utility helpers for normalization, mapping, and conversions |
| `Core/Native/SwissEphemerisNative.cs` | P/Invoke bindings to native SWE API functions |
| `Core/Models/SwissParameters.cs` | Input parameters for computation |
| `Core/Models/SwissResult.cs` | Output structure with planetary results |
| `Core/Constants/SweConst.cs` | Constants and flags mirroring Swiss Ephemeris definitions |

## ⚙️ Initialization & Platform Integration

- Ephemeris data files (`*.se1`) are stored inside `/Resources/Raw/ephe.zip`.
- On first run, the archive is extracted to `FileSystem.AppDataDirectory/ephe/`.
- SwissService automatically sets the path and sidereal mode (Lahiri).
- Platform-specific paths:
  - **Windows:** via `swedll64.dll` with `AppContext.BaseDirectory/Resources/Raw/ephe/`
  - **Android:** via `libswe.so` (custom-built from source) with archived `ephe.zip` in `AppContext.BaseDirectory/Resources/Raw/`
  - **iOS:** to be integrated later with static `libswe.a` (planned)

## 🧮 Implemented Calculations

#### 🪙 `EPlanet` (Enum)
Defines internal identifiers for celestial bodies used in PADMA.
Maps directly to the `PLANET` table in the database.

| Enum | Code | Description |
|-------|------|-------------|
| `Sun` | `"SUN"` | The Sun |
| `Moon` | `"MOON"` | The Moon |
| `Mars` | `"MARS"` | Mars |
| `Mercury` | `"MERCURY"` | Mercury |
| `Jupiter` | `"JUPITER"` | Jupiter |
| `Venus` | `"VENUS"` | Venus |
| `Saturn` | `"SATURN"` | Saturn |
| `Rahu` | `"RAHU"` | North Node |
| `Ketu` | `"KETU"` | South Node |

### 🔧 Planet ID Mapping

SwissService operates with two distinct but related identifiers for planets:

1. **PlanetId** — Internal PADMA identifier
   - Matches IDs from the `EPlanet` enum and the `PLANET` table in the PADMA database.
   - Used throughout PADMA logic and data caching.
   - Represents the logical planet used in calculations and UI.

2. **SwissPlanetConst** — Swiss Ephemeris constant
   - Numeric identifier defined in the Swiss Ephemeris library (`SweConst` class).
   - Used only internally during Swiss Ephemeris calls.
   - Derived from `PlanetId` via a utility mapping.

Before performing calculations, `PlanetId` is converted into its Swiss Ephemeris equivalent using:

```csharp
SwissUtility.GetPlanetSWEConstByPlanetId(int planetId)
```

## 🪐 Mapping Table

| EPlanet ID | Planet | Swiss Ephemeris Constant | Notes |
|-------------|---------|--------------------------|--------|
| 1 | Sun | `SE_SUN` |  |
| 2 | Moon | `SE_MOON` |  |
| 3 | Mars | `SE_MARS` |  |
| 4 | Mercury | `SE_MERCURY` |  |
| 5 | Jupiter | `SE_JUPITER` |  |
| 6 | Venus | `SE_VENUS` |  |
| 7 | Saturn | `SE_SATURN` |  |
| 8 | Rahu (Mean/True Node) | `SE_MEAN_NODE/SE_TRUE_NODE` | Direct Swiss Ephemeris calculation |
| 9 | Ketu | — | Derived as Rahu + 180° |

**Notes:**
- `PlanetId` represents PADMA’s internal model and database linkage.
- `SwissPlanetConst` is generated dynamically and never stored in the database.
- Ketu (both Mean and True) is not directly computed — its position is derived geometrically as the opposite point of Rahu.
- This approach ensures consistent and efficient calculations, aligning with both **Jyotish tradition** and **Swiss Ephemeris standards**.

### 🪐 Planet Positions

`GetPlanetPosition()` — computes geocentric longitude, latitude, distance, and speed for any planet.

- Uses Swiss Ephemeris (`swe_calc_ut`)
- Returns `SwissResult` object with:
  - `CalculationValues[6]`
  - `Sign`, `IsRetrograde`, `UtcSecondsOfDay`

**Retrograde detection:**
`IsRetrograde = (speed < 0)`

### 🌗 Tithi Calculation

`CalculateTithiDataList_London(DateTime startUtc, DateTime endUtc)`

Computes the sequence of lunar day transitions within a range.

Formula:
```
Tithi = floor( Normalize(moonLon - sunLon) / 12° ) + 1
```
- Range: 1–30
- Each Tithi corresponds to 12° separation between Sun and Moon.
- Output: List of (TithiIndex, StartUtc, EndUtc)

### 🌞 Nitya Yoga Calculation

`CalculateNityaYogaDataList_London(DateTime startUtc, DateTime endUtc)`

Formula:
```
YogaAngle = Normalize(moonLon + sunLon)
YogaIndex = floor( YogaAngle / 13°20′ ) + 1
```
- Range: 1–27
- Uses `NityaYogaTithiResults` (paired Sun/Moon positions).
- Each yoga occupies a 13°20′ arc (13.3333°).

### ☠️ Mrityu Bhaga Detection Engine

## Overview

Mrityu Bhaga detection identifies time intervals when a planet’s sidereal longitude falls into its predefined “critical / danger” degree zone (Mrityu Bhaga) for the current zodiac sign.

The engine reuses:
* Swiss Ephemeris sidereal positions (Lahiri)
* Reference Mrityu Bhaga degree table loaded in DataCache (MRITYUBHAGA)
* Existing PADMA time handling conventions (UTC internally, local only for UI)

It produces **intervals** (periods), not just flags.

## Inputs

Function:
```
CalculateMrityuBhagaDataList_London(
* planetId,
* fromUtc,
* toUtc,
* nodeType)
```

Data sources:
* DataCache.Instance.MrityuBhagaList (table MRITYUBHAGA)
  * PlanetId
  * ZodiacId
  * Degree (sidereal, 0–360)
* Active system setting: Mrityu Bhaga mode
  * MRITYUBHAGANEQUAL
  * MRITYUBHAGANLESS
  * MRITYUBHAGANMORE
  * MRITYUBHAGAERNST

## Zone Definition (per active setting)

For each planet and current zodiac sign, the critical zone is built from the reference degree `D` and tolerance `tol`:
* MRITYUBHAGANEQUAL: [D - tol, D + tol]
* MRITYUBHAGANLESS:  [D - tol, D]
* MRITYUBHAGANMORE:  [D, D + tol]
* MRITYUBHAGAERNST:  [D - tol, D + tol]

Tolerance values are defined by settings (current implementation):
* EQUAL: 0.5°
* LESS/MORE/ERNST: 1.0°

Zone comparison uses normalized degrees and supports wrap-around over 0°.

## Core Algorithm

For the requested window (fromUtc..toUtc):

1. Iterate time samples and compute planet sidereal longitude and retrograde flag.
2. Resolve current zodiac sign by longitude.
3. Load Mrityu Bhaga reference record for (planetId, zodiacId).
4. Build zone boundaries (fromDeg..toDeg) per active setting.
5. Determine `inside = IsWithinDegrees(lon, fromDeg, toDeg)`.
6. Detect transitions:
   * Entry: inside == true after being outside
   * Exit: inside == false after being inside
7. Record intervals as MrityuBhagaData objects:
   * DateFromUtc
   * DateToUtc
   * PlanetId
   * ZodiacId
   * Degree
   * MrityuBhagaSetting
   * LongitudeFrom / LongitudeTo

## Real Boundary Expansion (No Window Clipping)

The engine MUST return **real** interval boundaries and must not clip periods to the requested window:
* If the planet is already inside the Mrityu Bhaga zone at fromUtc:
  * Expand backward in time until the last moment outside the zone.
  * Refine the entry moment to obtain a real DateFromUtc.

* If the planet is still inside the zone at toUtc:
  * Expand forward in time until the first moment outside the zone.
  * Refine the exit moment to obtain a real DateToUtc.

This prevents artificial truncation and supports cases where a Mrityu Bhaga period starts before or ends after the monthly calculation window.
Protective limits are used for expansion (e.g., up to N days backward/forward).

## Sampling Strategy

To avoid missing short passages through the zone (especially for fast planets), the engine uses adaptive sampling:

* Outside zone: coarse step (recommended: ~15 minutes)
* Inside zone: fine step (~1 minute)

Optional refinement:

* When an entry/exit is detected between two samples, a binary search refinement may be used to improve boundary precision.

## Time Handling

* All calculations and stored interval boundaries are in UTC.
* UI layers convert to profile local time for display.

## Output
```
List<MrityuBhagaData>
```
Properties:
* Chronologically ordered
* Non-overlapping for the same planet/sign segment
* Real (expanded) boundaries; not clipped by fromUtc/toUtc

## Consumers

Mrityu Bhaga intervals are intended for later UI and monthly views, including:
* Planetary monthly transit overview
* Warning markers / special annotations

## Status

Implemented.
Boundary expansion logic ensures correct intervals for both slow and fast planets without clipping to calculation window.

### 🌑 Eclipse Computation

Implements both **lunar** and **solar** eclipse search.

- `CalculateEclipses_London(DateTime fromUtc, DateTime toUtc)` → the list of eclipses (UTC time) within a range (both - lunar and solar).

Returned structure `EclipseInfo`:
```
Type (Partial, Total, Annular, Hybrid, Penumbral)
BeginUtc
MaximumUtc
EndUtc
Magnitude
```
Uses Swiss Ephemeris functions:
- `swe_lun_eclipse_when`
- `swe_sol_eclipse_when_glob`

### 🧩 Higher-Level Analysis (SwissAnalysis)

`CalculatePlanetDataList_London(startUtc, endUtc)` —
builds list of `PlanetData` entries for all transitions.

Features:
- 1-hour stepping (`3600s`)
- Detects changes in **Sign**, **Nakshatra**, **Pada**, **Retrograde**
- Performs binary search via `FindTransitionEpoch()` for exact UTC time
- Default coordinates: London (`Lon = -0.17, Lat = 51.5`)


## 🧱 Data Models

### `SwissParameters`
| Field | Type | Description |
|--------|------|-------------|
| PlanetId | int | Internal PADMA ID |
| PlanetCode | string | Identifier (e.g. "SUN") |
| Longitude | double | Geographic longitude |
| Latitude | double | Geographic latitude |
| Altitude | double | Altitude in meters |
| UtcDateTime | DateTime | UTC moment of calculation |

### `SwissResult for Planet transits calculations`
| Field | Type | Description |
|--------|------|-------------|
| CalculationValues | double[6] | Raw Swiss Ephemeris result |
| Sign | int | Zodiac sign 1–12 |
| IsRetrograde | bool | Motion flag |
| UtcSecondsOfDay | int | Seconds since UTC midnight |
| IsCalculationFailed | bool | Error flag |

## 🧰 Utilities

`SwissUtility` provides:
- `NormalizeDegrees(double)` — ensures [0,360]
- `GetPlanetSWEConstByPlanetId(int)` — mapping PADMA → Swiss constants
- `AdjustForKetu(double)` — adds 180°, wraps to 360°
- `GetZodiakIdFromDegree()`, `GetNakshatraIdFromDegree()`, `GetPadaIdFromDegree()`
- `GetNavamsaByNakshatraAndPada()` — database lookup via `DataCache.Padas`

## 🧠 Constants (`SweConst`)

Main constants used:
| Name | Description |
|------|--------------|
| `SEFLG_SWIEPH` | Swiss Ephemeris mode |
| `SEFLG_SPEED` | Include planetary speed |
| `SEFLG_SIDEREAL` | Sidereal zodiac |
| `SE_SIDM_LAHIRI` | Lahiri Ayanamsha |
| `SE_GREG_CAL` | Gregorian calendar |

Also defines IDs for:
- Planets (`SE_SUN`..`SE_SATURN`, `SE_MEAN_NODE`, `SE_TRUE_NODE`)
- Flags for eclipse modes and computation masks.

## 🕒 Time Handling

- All computations are in **UTC (GMT+0, London coordinates)**.
- UI layer performs local time conversions.
- `.NET TimeZoneInfo` and `AdjustmentRules` handle DST.
- Date/time strings stored in DB as `"yyyy-MM-dd HH:mm:ss"`.

# 🌄 Ascendant Calculation — SwissService & SwissUtility

## 📘 Overview

The Ascendant (Lagna) calculation feature has been implemented using the Swiss Ephemeris engine integrated through the SwissService.
This module computes the **Ascendant longitude** for any date/time and geographic location, including proper handling of **historical time zones**.

## 🧭 Core Components

| File | Description |
|------|--------------|
| `Core/Services/SwissService.cs` | Contains the low-level calculation `CalculateAscendantForDate` (core Ascendant computation in UTC). |
| `Core/Utilities/SwissUtility.cs` | Provides `CalculateAscendantWithTimeZone` for high-level usage including local time zone conversion. |
| `Core/Services/TimeZoneService.cs` | Handles historical timezone detection using GeoTimeZone, TimeZoneConverter, and NodaTime. |

## ⚙️ External Libraries

| Package | Version | Purpose |
|----------|----------|----------|
| `GeoTimeZone` | 6.1.0 | Determines IANA timezone ID by latitude/longitude (offline). |
| `TimeZoneConverter` | 7.2.0 | Converts between IANA and Windows (.NET) timezone formats. |
| `NodaTime` | 3.2.2 | Provides historical timezone offsets and date-time conversions. |

## 🧩 Calculation Flow

### ️⃣  Ascendant Core Calculation (SwissService)

Method:
```csharp
public static double CalculateAscendantForDate(
    DateTime dateTimeUtc,
    double latitude,
    double longitude,
    double altitude,
    char hsys = 'O')
```
- Inputs are in **UTC**.
- Performs conversion to Julian Day (`swe_julday`).
- Activates sidereal Lahiri mode (`swe_set_sid_mode`).
- Sets topocentric coordinates (`swe_set_topo`).
- Calls Swiss Ephemeris native function `swe_houses_ex()`.

Result:
- Returns **Ascendant ecliptic longitude** in degrees [0–360].
- Default house system: **‘O’ (Placidus)**.

### ️⃣  Ascendant with TimeZone Adjustment (SwissUtility)

Method:
```csharp
public static double CalculateAscendantWithTimeZone(
    DateTime dateUtc,
    double latitude,
    double longitude,
    double altitude,
    char hsys = 'O')
```
- Uses `TimeZoneService` to get historical UTC offset for the coordinates.
- Converts to local time via NodaTime’s `DateTimeZoneProviders.Tzdb`.
- Calls `CalculateAscendantForDate` with the corrected UTC time.
- Returns Ascendant longitude (sidereal, Lahiri).

## 🕒 Historical Time Zone Logic

### TimeZoneService Methods

| Method | Description |
|---------|--------------|
| `GetIanaTimeZoneId(lat, lon)` | Returns IANA zone ID (e.g., "Europe/Kyiv"). |
| `GetDotNetTimeZoneId(lat, lon)` | Returns equivalent Windows ID. |
| `GetUtcOffsetHours(date, lat, lon)` | Returns UTC offset (historical) in hours using NodaTime tzdb. |

## 🔍 Notes
- Calculation fully respects historical DST and UTC offsets.
- Works identically on Windows, Android, and iOS.
- Uses sidereal mode **Lahiri** by default.
- Returns absolute ecliptic longitude (0–360°), compatible with all PADMA models.
- Formatting into degrees/minutes/seconds handled in `FormatDegrees(double degrees)` function (`Core/Utilities/SwissUtility.cs`).


### 🌅 Sunrise and Sunset Calculation

#### **Purpose**
This module calculates the sunrise and sunset times for a given geographic location and date, respecting user-defined configuration (calculation type: by disc edge or by disc center).

#### **Main Files**
- `SwissService.cs` — functions to calculate sunrise and sunset times in UTC.
- `SwissEphemerisNative.cs` — P/Invoke declaration for the `swe_rise_trans` function.
- `SweConst.cs` — contains constant definitions used for rise/set calculations (`SE_SUNRISE_TIP`, `SE_SUNRISE_CENTER`, `SE_SUNSET_TIP`, `SE_SUNSET_CENTER`).
- `TimeZoneService.cs` — universal time conversion service (UTC ↔ Local) based on `.NET TimeZoneInfo` and `AdjustmentRules`.

#### **SwissService Functions**

** ️⃣  Sunrise Calculation:**
```csharp
public static DateTime CalculateSunriseForDateAndLocation(DateTime date, double latitude, double longitude, double altitude)
```
Calculates the UTC time of sunrise for a given date and coordinates.
The calculation type is determined by the active configuration:
- `SUNRISETIP` — lower limb (disc edge),
- `SUNRISECENTER` — disc center.

** ️⃣  Sunset Calculation:**
```csharp
public static DateTime CalculateSunsetForDateAndLocation(DateTime date, double latitude, double longitude, double altitude)
```
Calculates the UTC time of sunset for a given date and coordinates.

#### **TimeZoneService and Time Conversion**

For converting UTC results to local time, the following function is used:

```csharp
public static DateTime ConvertUtcToLocalSmart(DateTime utc, double latitude, double longitude)
```

This function:
- Determines the .NET timezone based on coordinates (GeoTimeZone + TZConvert);
- Applies the base `UtcOffset` and adjusts using `AdjustmentRules`;
- Considers possible Daylight Saving Time (DST) transitions;
- Works without external libraries and is applicable for any region.

Additional helper functions in `TimeZoneService`:
- `ShiftDateByDaylightDelta()` — applies DST offset when active;
- `GetAdjustmentDate()` — computes actual transition dates for DST.

#### **Notes**
- All Swiss Ephemeris calculations are performed in UTC.
- Conversion to local time is handled via `.NET TimeZoneInfo`, ensuring compatibility with system settings on all platforms.
- Minor discrepancies (up to ±1 day or even more) may occur for future years due to known limitations of the Windows time zone database.
- For historical calculations (e.g., natal charts), `NodaTime` is used — relying on the IANA time zone database for full historical accuracy.
