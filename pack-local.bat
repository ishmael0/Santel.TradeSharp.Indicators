@echo off
REM Local NuGet Package Builder (without publishing)
REM Use this to test package creation before publishing

echo ========================================
echo Local Package Builder
echo ========================================
echo.

set PROJECT_FILE=Santel.TradeSharp.Indicators.csproj
set OUTPUT_DIR=.\nupkg

echo Cleaning previous builds...
dotnet clean --configuration Release
echo.

echo Building project...
dotnet build --configuration Release
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo.

echo Creating NuGet package...
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"
dotnet pack %PROJECT_FILE% --configuration Release --output %OUTPUT_DIR%
if errorlevel 1 (
    echo ERROR: Pack failed!
    pause
    exit /b 1
)
echo.

echo ========================================
echo Package created successfully!
echo ========================================
echo.
echo Package location: %OUTPUT_DIR%
echo.
dir /B %OUTPUT_DIR%\*.nupkg
echo.
echo To publish, run: publish-nuget.bat YOUR_API_KEY
echo.
pause
