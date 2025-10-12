# 🪶 PADMA — Project Requirements & Current Implementation
> _Version: October 2025_

---

## 🗓️ Overview

**PADMA** — кроссплатформенное приложение на **.NET MAUI**, предназначенное для отображения календаря, расчётов и пользовательских конфигураций.  
Все данные (настройки, тексты интерфейса, справочники, профили) хранятся в **SQLite-базе** `PADMADB.db3` и кэшируются в памяти при запуске.

---

## 🧱 Architecture

### 📂 Project Structure

| Folder | Description |
|---------|-------------|
| `PADMA/Core` | Business logic, models, services, and utilities |
| `PADMA/Pages` | UI pages (XAML + code-behind) |
| `PADMA/UI/Templates` | Common page templates (e.g. `ConfigBasePage`) |
| `PADMA/Resources/Raw` | Embedded SQLite database and static assets |
| `PADMA/Resources/Styles` | Shared UI styles (fonts, colors, margins) |
| `PADMA/docs` | Documentation and requirement files |

---

## ⚙️ Core Services

### 🔸 DatabaseService
Handles all database operations via SQLite.

**Main methods:**
```csharp
GetAppSettingsList()
UpdateAppSettings(List<AppSettingList>)
SetFirstDayOfWeek(string code)
SetLanguage(string code)
GetAppTextsList(string languageCode)
GetActiveLanguageCode()
GetFirstDayOfWeekFromDb()


### 🔸 DataCache

Central in-memory cache loaded at startup.

Responsibilities:
Loads reference data (languages, colors, planets, localized texts)
Stores the current UI language (CurrentLanguageCode)
Provides fast access to data for all pages

Key method:
LoadAll(DatabaseService db, string preferredUiLang)



🔸 Localization

Utility class for text localization.

public static string GetLocalizedText(string nativeText, string langCode)

If no translation exists — returns the original English version.



🔸 ServiceLocator

Global access point for dependency-injected services (DI container).



🖥️ Implemented Pages

### 🏠 MainPage (Календарь)

Главная страница приложения PADMA — это интерактивный **календарь**, являющийся центральной частью пользовательского интерфейса.  
Она загружается первой при запуске приложения и отображает текущий месяц в зависимости от системной даты, локализации и настроек пользователя.

---

#### 📋 Общие требования
1. Страница отображает **месячный календарь** в виде сетки (`Grid`), содержащей:
   - заголовок месяца и года в верхней части - на тулбаре,
   - строку названий дней недели (верхний левый угол),
   - основной блок дат (42 дня).
   - Каждая ячейка дня содержит в нижней части (две трети общей площади) 6 цветных полосок (в будущем состояние транзитов) - пока в виде такой заглушки.
2. При старте приложения:
   - Определяется активный язык интерфейса (`DataCache.Instance.CurrentLanguageCode`);
   - Определяется первый день недели на основе настройки в БД (`APPSETTING` → `WEEKMONDAY` / `WEEKSUNDAY`);
   - Календарь строится согласно этим настройкам.
3. Страница поддерживает **локализацию** всех текстовых элементов (еще не готово):
   - названия месяцев и дней недели;
   - заголовок месяца/года;
   - всплывающие элементы интерфейса (например, переходы).

---

#### 🧱 Разметка и структура (Layout)

Визуальная структура страницы:

| Область | Элемент | Описание |
|----------|----------|----------|
| Верхняя панель | `<StackLayout>` | Содержит навигационные кнопки и заголовок текущего месяца |
| Навигация | `<ImageButton>` | Кнопки для перехода к предыдущему и следующему месяцу (`left_arrow.png`, `right_arrow.png`) | (пока еще не готово - использован текст "<" ">")
| Название месяца | `<Label>` | Отображает текущий месяц и год, локализовано, выровнено по центру | (на тулбаре)
| Полоса-разделитель | `<BoxView>` | Горизонтальная линия под заголовком месяца |
| Заголовки дней недели | `<Grid>` | Одна строка с 7 ячейками (`Mon, Tue, Wed…` или локализованные версии) |
| Основная сетка календаря | `<Grid>` | 6 строк × 7 колонок, динамически заполняется датами |
| Ячейка дня | `<Frame>` | Не содержит отступов, - весь Календарь - растянут на всю область экрана
| Текст даты | `<Label>` | Цифра дня месяца в левом верхнем углу верхней области (одна треть) ячейки дня |
| Подсветка текущего дня | `<BoxView>` или изменённый `BackgroundColor` в `Frame` | Светло-голубого цвета
| Нижняя разделительная полоса | `<BoxView>` | Тонкая линия для визуального завершения сетки |

---

#### 🎨 Пропорции и стили

| Элемент | Свойства |
|----------|-----------|
| Заголовок месяца | `FontSize="22"`, `FontAttributes="Bold"`, `TextColor="#333"`, `HorizontalOptions="Center"` |
| Подписи дней недели | `FontSize="14"`, `FontAttributes="Bold"`, `TextColor="#555"`, `HorizontalOptions="Center"` |
| Ячейки дней | `Frame` с `CornerRadius="4"`, `Padding="5"`, `HasShadow="False"`, `HeightRequest="48"`, `WidthRequest="48"` |
| Цвета | Основной фон — белый (`#FFFFFF`), границы ячеек — светло-серые (`#DDD`) |
| Подсветка текущего дня | Светло-голубой цвет |
| Разделительные линии | `BoxView HeightRequest="1"`, `Color="#CCC"` |

---

#### ⚙️ Логика и поведение

**1. Построение календаря**
- Метод `BuildCalendar()` вызывается при:
  - старте страницы (`OnAppearing()`),
  - изменении месяца (`PrevMonth_Clicked` / `NextMonth_Clicked`),
  - изменении конфигурации через `MessagingCenter` (например, смена первого дня недели).
- Сетка календаря строится динамически с использованием `DateTime.DaysInMonth(year, month)`.

**2. Определение первого дня недели**
- Получается из БД через `DatabaseService.GetFirstDayOfWeekFromDb()`.
- На основе этого устанавливается порядок заголовков (`Mon-Sun` или `Sun-Sat`).

**3. Навигация по месяцам**
- При нажатии стрелок:
  - Меняется отображаемый месяц.
  - Календарь перестраивается.
  - Заголовок месяца обновляется с учётом локализации.

**4. Выбор даты**
- При нажатии на ячейку дня:
  - Сохраняется выбранная дата.
  - Происходит переход на `DayPage`:
    ```csharp
    await Shell.Current.GoToAsync($"//day?date={selectedDate:yyyy-MM-dd}");
    ```
  - При этом закрывается бургер-меню (если оно открыто).

**5. Реакция на изменение конфигурации**
- Через `MessagingCenter.Subscribe` страница реагирует на событие `"SettingsChanged"`.
- При получении сообщения календарь перерисовывается с учётом новых настроек:
  ```csharp
  BuildCalendar();
  
🚦 Навигация и взаимодействие

Доступ из бургер-меню пунктом Calendar.
Находится по маршруту //main.
При открытии других страниц (например, настроек) бургер-меню автоматически закрывается.
Возврат из DayPage возвращает пользователя обратно на тот же месяц.

Технические требования (итог)

Календарь должен корректно перестраиваться при изменении:
языка;
первого дня недели;
текущего месяца.
Все тексты должны использовать систему локализации. (еще не готова)
Страница должна быть оптимизирована для всех платформ (.NET MAUI: Android, iOS, Windows).
Календарь адаптивен — равномерное распределение колонок по ширине экрана.
Не допускается дублирование текстов напрямую в XAML — всё через Localization.
Поддержка светлой темы (фон белый, текст тёмный, акценты цветные).

Будущие улучшения

Добавить кнопку Today для возврата к текущему месяцу.
Навигация | `<ImageButton>` | Кнопки для перехода к предыдущему и следующему месяцу (`left_arrow.png`, `right_arrow.png`)
Переход к году / месяцу через выпадающий выбор.
Добавление событий и заметок для конкретных дат.
Добавление системы локализации (Тексты и локализация (в APP_TEXTS)).


🌅 DayPage

Displays details for a selected date.

Features:
Opens on day tap from the calendar
Shows detailed information (placeholder for astrological calculations) (пока пусто - в будущем)
Localized text and layout
Returns to calendar via navigation shell

⚙️ ConfigurationPage

Main settings hub.

Features:
Accessed from Settings in the burger menu
Buttons for configuration pages:
LanguagePage
FirstDayOfWeekPage
(future) ThemePage, NotificationPage, etc.
Closes burger menu when opened
Inherits layout and toolbar from ConfigBasePage

🈸 LanguagePage

Page for selecting the application interface language.

Features:
Lists 4 languages: English, Українська, Polski, Русский

Each row contains:
Radio button
Language name
Flag icon with thin border

On selection:
Updates active language in APPSETTING
Refreshes DataCache and re-renders UI
Language persists after restart

Confirmation dialog (localized Yes / No buttons)
Instruction under header: “Choose application language:”

📅 FirstDayOfWeekPage

Page for selecting the first day of the week.

Features:
Options: Monday / Sunday
Updates APPSETTING (group WEEK)

On change:
Shows confirmation dialog “Save changes?”
Updates DB and cache
Notifies MainPage to redraw calendar

Reads current value from DB on load
Localized all texts (title, instruction, dialog)



🔲 ConfigBasePage

Shared base template for all configuration pages.

Provides:
Unified background and spacing
Consistent text styles
Toolbar with close icon (close_icon.png)
Standardized title and label fonts (SectionLabelStyle, InstructionLabelStyle)
Inherited automatically by all configuration pages


🍔 Burger Menu (Shell Flyout)

Implemented in AppShell.xaml.

Structure:
Calendar → MainPage
Settings → ConfigurationPage
Exit → ExitPage

Details:
Always visible except on deep configuration pages
Localized menu item names
Closes automatically when opening a settings page

🧾 Database Schema
Table	Purpose
APPSETTING	Stores all app configuration settings
APP_TEXTS	Localized UI texts
LOCATION	Geographical data (reduced to Kyiv and Chornyi Ostriv)
PROFILE	User profiles linked to LOCATION
PLANET, COLOR, LANGUAGE	Reference tables
*_DESC	Language-specific descriptions for reference tables


🪄 Meta

Last verified commit: 4c3157a8d6ab95402aa20ae3f89e61ab8a496455

