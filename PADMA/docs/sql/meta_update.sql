UPDATE APP_META SET VALUE = '0.0.62' WHERE KEY = 'DB_VERSION';


dotnet publish PADMA.csproj -f net9.0-android -c Release

dotnet build PADMA.csproj -f net9.0-android -c Debug -r android-arm64

For Studio:
-------------------------
  <PropertyGroup Condition="'$(TargetFramework)'=='net9.0-android' AND '$(Configuration)'=='Debug'">
    <RuntimeIdentifier>android-x64</RuntimeIdentifier>
  </PropertyGroup>
-------------------------

For publish-debug:
-------------------------
   <PropertyGroup Condition="'$(TargetFramework)'=='net9.0-android' AND '$(Configuration)'=='Debug'">
    <RuntimeIdentifier>android-arm64</RuntimeIdentifier>
  </PropertyGroup>
--------------------

.\adb.exe logcat -c ; .\adb.exe logcat --pid=$(.\adb.exe shell pidof -s com.s.sheremet.padma) > log_padma.txt
.\adb.exe logcat -c ; .\adb.exe logcat --pid=$(.\adb.exe shell pidof -s com.s.sheremet.padma) | findstr /i "PADMA-NOTIF" > padma_notif_log.txt

--------------------

SELECT *
FROM TRANSIT_DESC
WHERE TRANSITID = 1


UPDATE TRANSIT_DESC
SET DESCRIPTION = CASE LANGUAGECODE
    WHEN 'en' THEN 'English text'
    WHEN 'uk' THEN 'Ukrainian text'
    WHEN 'pl' THEN 'Polish text'
    WHEN 'ru' THEN 'Russian text'
END
WHERE TRANSITID = 1
  AND LANGUAGECODE IN ('en','uk','pl','ru');
  
----------------------  