UPDATE APP_META SET VALUE = '0.0.39' WHERE KEY = 'DB_VERSION';


dotnet publish PADMA.csproj -f net9.0-android -c Release