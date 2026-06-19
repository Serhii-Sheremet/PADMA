# User Events, Notifications, and Retention

## 1. Purpose

User-note data model, event cache, DayPage interaction, editor, local notification scheduling, and retention-policy behavior.

## 2. Current Status

This document describes the current PADMA implementation and the rules that future changes must preserve.
Deferred work is listed only when it affects the design of the current implementation.

## 3. Requirements and Implementation Details

# User Notes (Events) – Model, Storage & DayPage Interaction Specification

This section describes the data model, storage rules, caching strategy, and DayPage UI interaction
for **User Notes (Events)** in PADMA.

User Notes are personal time-based records created by the user directly on the DayPage timeline.
They originate from legacy PAD behavior and are adapted for mobile interaction.

## 1. Purpose

User Notes represent personal observations, notes, or reminders attached to a specific date/time.

They:

- Are attached to a specific profile
- Have start and end date/time
- Are displayed as colored segments in the Events column on DayPage
- Can later be used for:
  - DayPage visualization
  - Calendar day indicators (triangle in month cell)
  - Tooltip preview of daily notes
  - Notification / reminder scheduling

## 2. Database Table

Existing legacy-compatible table (no structural changes):

```sql
CREATE TABLE IF NOT EXISTS "USER_EVENTS" (
    "ID"        INTEGER,
    "PROFILEID" INTEGER,
    "DATESTART" TEXT,
    "DATEEND"   TEXT,
    "NAME"      TEXT,
    "MESSAGE"   TEXT,
    "ARGBVALUE" INTEGER,
    PRIMARY KEY("ID" AUTOINCREMENT),
    FOREIGN KEY("PROFILEID") REFERENCES "PROFILE"("ID")
);
```

## 3. Date/Time Storage Format

DATESTART and DATEEND are stored as strings using format:
```
yyyy-MM-dd HH:mm:ss
```
Characteristics:
- Exactly 19 characters
- Culture-invariant
- Lexicographically sortable
- Represents **local time of active profile**
- No UTC conversion at DB level

Timezone conversion will be applied later only for notification scheduling.

## 4. Conceptual Model (C#)

```csharp
public sealed class UserEvent
{
    public int Id { get; set; }
    public int ProfileId { get; set; }

    public string DateStart { get; set; } = "";
    public string DateEnd   { get; set; } = "";

    public string Name { get; set; } = "";
    public string Message { get; set; } = "";
    public int ArgbValue { get; set; }

    public DateTime StartLocal => UserEventDateHelper.Parse(DateStart);
    public DateTime EndLocal   => UserEventDateHelper.Parse(DateEnd);
}
```

Helper:

```csharp
public static class UserEventDateHelper
{
    public const string DbFormat = "yyyy-MM-dd HH:mm:ss";

    public static string ToDbString(DateTime dt) =>
        dt.ToString(DbFormat, CultureInfo.InvariantCulture);

    public static DateTime Parse(string s) =>
        DateTime.ParseExact(s, DbFormat, CultureInfo.InvariantCulture);
}
```

## 5. Window Cache

User Notes are loaded through a window-based cache:

`UserEventsWindowCache`

Responsibilities:

- Load all events for:
  - profileId
  - windowStartLocal
  - windowEndExclusiveLocal
- Build index: DayStart → List<UserEvent>
- Provide fast access:

```csharp
bool HasEvents(DateTime dayLocal)
List<UserEvent> GetEventsForDay(DateTime dayLocal)
```

Cache lifecycle:

- After insert/update/delete:
  - Invalidate()
  - ReloadLastWindow()

## 6. Relation to Calendar

CalendarViewModel queries:
```
UserEventsWindowCache.HasEvents(day)
```
If true → draw triangle marker inside month cell.
Calendar is refreshed via MessagingCenter event:
```
"UserEventsChanged"
```

## 7. DayPage Visualization

- Notes displayed in **Events column**
- Vertical position based on:
  - minutes from day start
  - PixelsPerMinute
- Height based on duration
- Color from ARGBVALUE
- Title displayed if space allows

Overlapping events:

- Column-based layout
- UserEventOverlapHelper assigns:
  - ColumnIndex
  - ColumnCount

## 8. DayPage Interaction Rules

### Tap on empty Events area
- Computes minute position from Y
- Rounds to 15 minutes
- Selects slot
- Second tap on same slot → Create Note

### Tap on existing note
- First tap → Select
- Second tap → Edit Note

## 9. Note Editor Overlay

Overlay contains:

- Header: New note / Edit note
- Start TimePicker
- End TimePicker
- Title Entry
- Description Editor
- Color Palette bar
- Buttons: Delete, Cancel, OK

Behavior:

- Create → Insert
- Edit → Update
- Delete → Delete
- If no changes on Edit + OK → close only

Safety rule:

- If End <= Start → End = Start + 15 minutes

## 10. Color Selection

- Palette bar shows current color
- Stored as ARGB int

## 11. Localization

Overlay texts resolved via:
```
Localization.GetLocalizedText(nativeText, langCode)
```

## 12. Messaging & Refresh

After any change:

- Reload cache window
- Rebuild Events column
- Send:
```
MessagingCenter.Send("UserEventsChanged")
```

## 13. Future Extensions

- Reminders / notifications
- Multi-day notes
- Drag & resize
- Repeating notes
- Tooltip preview from calendar


# User Notes – Local Notifications (Reminder Service)

This section defines the **Stage 1 implementation** of local notifications for **User Notes** (USER_EVENTS).
The goal is to provide reliable “remind me before start time” behavior even when PADMA is not open,
using **OS-scheduled local notifications**.

This is a **global (app-wide / profile-wide)** reminder setting.
Per-note overrides are out of scope for this stage.

## 1. Goal and Scope (Stage 1)

### Goal
- Schedule local notifications for upcoming User Notes based on a **global reminder offset**
  (e.g., 5 / 15 / 30 minutes before note start).

### Scope (included)
- Global reminder setting stored in `APPSETTING` under a dedicated `GROUPCODE`.
- OS-level scheduling (no polling, no background timers).
- Refresh scheduling on “convenient occasions”.
- Horizon window scheduling: **7 days ahead**.
- Safety limit: **max 64 notifications** per active profile inside the horizon.
- Simple settings UI page for choosing reminder mode.
- Notification message includes **time range and note name**.

### Out of scope (later)
- Per-note reminder toggle/offset.
- Repeating events.
- Advanced notification channel / sound / vibration customization UI.
- Cross-device sync.

## 2. Global Reminder Setting (APPSETTING)

### 2.1 Database table
Existing table (no structural changes):
```
CREATE TABLE IF NOT EXISTS APPSETTING
(
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    GROUPCODE TEXT,
    SETTINGCODE TEXT,
    ACTIVE SMALLINT
);
```
### 2.2 Setting group
- GROUPCODE = 'NOTEREMINDER'

### 2.3 Options
- OFF
- MIN5
- MIN15
- MIN30

Default:
- OFF is active.

### 2.4 Effective reminder minutes
- OFF   → null
- MIN5  → 5
- MIN15 → 15
- MIN30 → 30

## 3. Scheduling Strategy

### 3.1 Why OS scheduling
Notifications must work even when PADMA is closed or suspended.

### 3.2 Horizon and limits
- Horizon: 7 days
- Max scheduled per profile: 64

### 3.3 Refresh triggers
- App start (after profile ready)
- "UserEventsChanged"
- "ProfileChanged"
- Reminder setting changed

## 4. Service Design

### Interface
```
public interface IUserNoteReminderService
{
    Task RefreshAsync(CancellationToken ct = default);
    Task CancelAllAsync(CancellationToken ct = default);
}
```
## 5. Notification Provider
```
public interface ILocalNotificationProvider
{
    Task<bool> EnsurePermissionsAsync();
    Task ScheduleAsync(int notificationId, DateTime fireTimeLocal, string title, string body);
    Task CancelAsync(int notificationId);
    Task CancelManyAsync(IEnumerable<int> notificationIds);
}
```
## 6. Scheduling Rules

fireTimeLocal = note.StartLocal - reminderMinutes

Before scheduling:
- If DateTime.Kind == Unspecified → treat as Local
- Else → convert to Local

Skip if:
- reminder OFF
- fireTimeLocal <= now
- outside horizon
- limit reached

NotificationId = note.Id

## 7. Message Content

Title: Personal Astrological Diary
Body: HH:mm–HH:mm • NoteName

## 8. Status

Stage 1 implemented and verified (Android emulator).

## 9. Future

- Per-note reminders
- Repeat rules
- Channel/sound settings


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
