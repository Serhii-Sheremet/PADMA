# PADMA — Project Requirements Index

## 1. Purpose

PADMA is the mobile successor to the legacy **Personal Astrological Diary (PAD)** project.
It is a personal Jyotish diary and visual analytical tool designed primarily for the Product Owner's
daily astrological practice.

This file is the entry point for PADMA requirements. Detailed requirements are split into thematic
documents in this `docs` directory.

## 2. Core Product Rules

1. PADMA is not a greenfield product. It inherits business logic and calculation behavior from
   the legacy PAD project wherever applicable.
2. The Product Owner's Jyotish rules and practical workflow decisions have priority over generic
   internet descriptions of similar astrological concepts.
3. Swiss Ephemeris calculations use Lahiri sidereal mode and UTC internally.
4. UI presentation converts UTC calculation data into the active profile's local time zone.
5. The application must preserve implemented behavior when new features are added.
6. DayPage must remain a consumer of prepared navigation data, not a date-only shortcut page.
7. Documentation should describe the current implementation and clearly separate deferred work.

## 3. Current Platform and Technology

- Application framework: .NET MAUI / .NET 9.
- Primary release platform: Android.
- Future platform: iOS.
- Development/test platform: Windows.
- Database: embedded SQLite (`PADMADB.db3`).
- Calculation engine: Swiss Ephemeris with Lahiri sidereal mode.
- UI/reference/localized data source: SQLite tables cached through `DataCache`.
- Supported UI languages: English, Ukrainian, Polish, Russian.

## 4. Documentation Set

| Document | Responsibility |
|---|---|
| [architecture.md](requirements/architecture.md) | Solution foundations, startup, shared services, database and time conventions |
| [data-cache-localization-settings.md](requirements/data-cache-localization-settings.md) | Cache, localization, configuration UI, settings events, colors |
| [profiles-locations-context.md](requirements/profiles-locations-context.md) | Profiles, locations, active/default semantics, profile calculation context |
| [swiss-calculations.md](requirements/swiss-calculations.md) | Swiss Ephemeris integration, planets, ascendant, sunrise/sunset, eclipses, Mrityu Bhaga |
| [transit-engine.md](requirements/transit-engine.md) | Slice architecture, builders, Panchanga, Yoga, Muhurta, Hora, Lagna, Vedha |
| [main-calendar.md](requirements/main-calendar.md) | MainPage, 42-day calendar preparation, picker, bars, markers and calendar rendering |
| [day-flow-navigation.md](requirements/day-flow-navigation.md) | Progressive computation, navigation tokens, DayNavBundle, DayWindowContext, carousel behavior |
| [day-pages-timeline.md](requirements/day-pages-timeline.md) | DayOverviewPage, DayPage timeline, lanes, tooltips and day-level UI |
| [user-events-notifications-retention.md](requirements/user-events-notifications-retention.md) | User events, cache, editor, reminders, retention behavior |
| [transit-charts-and-rectification.md](requirements/transit-charts-and-rectification.md) | Transit charts and Birth Time Rectification Preview |
| [monthly-planet-transits.md](requirements/monthly-planet-transits.md) | Monthly Planet Transits page, details, selections, Monthly → DayPage flow |
| [yearly-planet-transits.md](requirements/yearly-planet-transits.md)| Yearly Planet Transits page, details, selections, Yearly → MonthlyPlanetTransitsPage flow |
| [database-updates-and-preservation.md](requirements/database-updates-and-preservation.md) | Embedded database replacement, user-data preservation, sequence synchronization |
| [backlog.md](requirements/backlog.md) | Deferred or future items only |

## 5. Status Vocabulary

| Term | Meaning |
|---|---|
| **Implemented** | Current working behavior present in the project |
| **Current Rule** | Mandatory behavior that future work must preserve |
| **Deferred** | Planned or possible work that is intentionally not implemented yet |
| **Historical Note** | Previous behavior retained only for context and not a current rule |
| **Needs Verification** | Statement that should be checked against code before it is used as a requirement |

## 6. Documentation Maintenance Rule

When a feature is implemented:

1. Update the relevant thematic document.
2. Move any completed item out of `backlog.md`.
3. Keep this index short.
4. Do not append unrelated chronological notes to this file.
