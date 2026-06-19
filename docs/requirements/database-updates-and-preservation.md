# Database Updates and User-Data Preservation

## 1. Purpose

Embedded database versioning, replacement behavior, preservation/restoration of user-owned tables, SQLite sequence synchronization, and retention-related configuration handling.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

# PADMA - Data Persistence and Retention Enhancements

## 1. Database Update Mechanism with User Data Preservation

### Overview

A robust database update mechanism has been implemented to ensure that
user-generated data is preserved during application updates delivered
via Google Play.

### Problem

Previously, when the application database version (`DB_VERSION`)
changed, the local database file was fully replaced with the bundled
version. This caused loss of user data such as profiles, events, and
settings.

### Solution

A migration strategy was introduced:

1.  Before replacing the database:

    -   Read and preserve user data from:
        -   `LOCATION`
        -   `PROFILE`
        -   `USER_EVENTS`
        -   `APPSETTING`
        -   `COLOR`

2.  Replace the database file with the new version.

3.  Restore preserved data:

    -   `LOCATION`, `PROFILE`, `USER_EVENTS`:
        -   Fully restored with original IDs.
    -   `APPSETTING`:
        -   Merged using `GROUPCODE + SETTINGCODE`
        -   Only `ACTIVE` field is updated.
    -   `COLOR`:
        -   Merged using `CODE`
        -   Only `ARGBVALUE` is updated.

4.  Synchronize SQLite auto-increment sequences using the maximum
    existing IDs.

### Result

-   User data is fully preserved across updates.
-   New database schema and reference data are applied safely.
-   No data loss during upgrades.


## 2. SQLite Sequence Synchronization

### Overview

After restoring user data with explicit IDs, SQLite auto-increment
counters are synchronized.

### Implementation

For each table: - `LOCATION` - `PROFILE` - `USER_EVENTS`

The following logic is applied:

-   Determine `MAX(ID)`
-   Update or insert corresponding entry in `sqlite_sequence`

### Result

-   New records continue with correct IDs.
-   No conflicts or duplication.
-   Gaps in IDs are allowed and handled correctly.


## 3. Retention Policy for User Events

### Overview

A retention policy system was introduced to manage long-term growth of
user event data.

### Configuration

New setting group: `RETENTION`

Available options:
- `OFF` --- Keep all records
- `DAY30` --- Keep last 30 days
- `MONTH3` --- Keep last 3 months
- `MONTH6` --- Keep last 6 months

### Behavior

-   Applies only to `USER_EVENTS`
-   Deletes only completed events:
    -   `DATEEND < cutoff`
-   Future and active events are never removed

### Execution

-   Runs asynchronously after application startup
-   Does not block UI
-   Does not trigger calendar recalculation

### Integration

-   After cleanup:
    -   Notification reminders are refreshed
-   UI remains responsive

### Result

-   Prevents uncontrolled database growth
-   Maintains performance over time
-   Fully transparent to user


## 4. Configuration Handling Improvements

### Problem

All configuration changes previously triggered full calendar
recalculation.

### Solution

Configuration updates are now categorized:

-   `SettingsChanged`:
    -   Triggers calendar recalculation
-   `NotificationSettingsChanged`:
    -   Refreshes reminders only
-   `RetentionPolicyChanged`:
    -   Applies retention logic only

### Result

-   Eliminates unnecessary recalculations
-   Improves performance
-   Cleaner separation of concerns


## Summary

These enhancements provide:

-   Safe database upgrades
-   Reliable user data persistence
-   Controlled data growth
-   Improved performance and UX
