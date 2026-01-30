UPDATE APP_META SET VALUE = '0.0.35' WHERE KEY = 'DB_VERSION';


dotnet publish PADMA.csproj -f net9.0-android -c Release