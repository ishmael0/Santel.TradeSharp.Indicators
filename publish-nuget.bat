@echo off
REM NuGet Package Publisher for Santel.TradeSharp.Indicators
REM This script builds, packs, and publishes the NuGet package

echo ========================================
echo Santel.TradeSharp.Indicators Publisher
echo ========================================
echo.

REM Check if API key is provided as argument
if "%1"=="" (
    echo ERROR: NuGet API key is required!
    echo Usage: publish-nuget.bat YOUR_NUGET_API_KEY
    echo.
    echo Get your API key from: https://www.nuget.org/account/apikeys
    pause
    exit /b 1
)

set NUGET_API_KEY=%1
set PROJECT_FILE=Santel.TradeSharp.Indicators.csproj
set OUTPUT_DIR=.\nupkg

echo Step 1: Cleaning previous builds...
dotnet clean --configuration Release
if errorlevel 1 (
    echo ERROR: Clean failed!
    pause
    exit /b 1
)
echo Clean completed successfully.
echo.

echo Step 2: Building project in Release mode...
dotnet build --configuration Release
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo Build completed successfully.
echo.

echo Step 3: Creating NuGet package...
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"
dotnet pack %PROJECT_FILE% --configuration Release --output %OUTPUT_DIR% --no-build
if errorlevel 1 (
    echo ERROR: Pack failed!
    pause
    exit /b 1
)
echo Package created successfully.
echo.

echo Step 4: Publishing to NuGet.org...
for %%f in (%OUTPUT_DIR%\*.nupkg) do (
    echo Publishing: %%f
    dotnet nuget push "%%f" --api-key %NUGET_API_KEY% --source https://api.nuget.org/v3/index.json
    if errorlevel 1 (
        echo ERROR: Publish failed!
        pause
        exit /b 1
    )
)

echo.
echo ========================================
echo SUCCESS! Package published to NuGet.org
echo ========================================
echo.
echo Your package will be available at:
echo https://www.nuget.org/packages/Santel.TradeSharp.Indicators
echo.
echo Note: It may take a few minutes for the package to appear in search results.
echo.
pause
