# PADMA – Структура проекта

## Корень
- `.gitignore`
- `PADMA.sln`

## Папка `PADMA/`
- `App.xaml`
- `App.xaml.cs`
- `AppSettings.cs`
- `AppShell.xaml`
- `AppShell.xaml.cs`
- `CalendarViewModel.cs`
- `ConfigPage.xaml`
- `ConfigPage.xaml.cs`
- `DayItem.cs`
- `MauiProgram.cs`
- `PADMA.csproj`

### Enums
- `FirstDayOfWeek.cs`

### Страницы
- `MainPage.xaml`
- `MainPage.xaml.cs`
- `ConfigPage.xaml`
- `ConfigPage.xaml.cs`
- `ExitPage.xaml`
- `ExitPage.xaml.cs`

### Platforms
- **Android/**
  - `AndroidManifest.xml`
  - `MainActivity.cs`
  - `MainApplication.cs`
  - `Resources/values/colors.xml`
- **MacCatalyst/**
  - `AppDelegate.cs`
  - `Entitlements.plist`
  - `Info.plist`
  - `Program.cs`
- **Tizen/**
  - `Main.cs`
  - `tizen-manifest.xml`
- **Windows/**
  - `App.xaml`
  - `App.xaml.cs`
  - `Package.appxmanifest`
  - `app.manifest`
- **iOS/**
  - `AppDelegate.cs`
  - `Info.plist`
  - `Program.cs`
  - `Resources/PrivacyInfo.xcprivacy`

### Properties
- `launchSettings.json`

### Resources
- **AppIcon/**
  - `appicon.svg`
  - `appiconfg.svg`
- **Fonts/**
  - `FluentUI.cs`
  - `OpenSans-Regular.ttf`
  - `OpenSans-Semibold.ttf`
- **Images/**
  - `dotnet_bot.png`
- **Raw/**
  - `AboutAssets.txt`
- **Splash/**
  - `splash.svg`
- **Styles/**
  - `Colors.xaml`
  - `Styles.xaml`
