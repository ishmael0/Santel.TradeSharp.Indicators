# Publishing Guide

## Prerequisites

1. **NuGet Account**: Create an account at https://www.nuget.org
2. **API Key**: Generate an API key from https://www.nuget.org/account/apikeys
3. **.NET SDK**: Ensure .NET 8.0 SDK is installed

## Before Publishing

### 1. Update Version
Edit `Santel.TradeSharp.Indicators.csproj` and update the version:
```xml
<Version>1.0.0</Version>
```

### 2. Update Repository URLs
Replace the placeholder URLs in the .csproj file:
```xml
<PackageProjectUrl>https://github.com/yourusername/Santel.TradeSharp.Indicators</PackageProjectUrl>
<RepositoryUrl>https://github.com/yourusername/Santel.TradeSharp.Indicators</RepositoryUrl>
```

### 3. Test Locally
Run the local pack script to ensure everything builds correctly:
```bash
pack-local.bat
```

This will create a `.nupkg` file in the `nupkg` folder without publishing.

## Publishing to NuGet

### Option 1: Using the Batch File (Recommended)

```bash
publish-nuget.bat YOUR_NUGET_API_KEY
```

Replace `YOUR_NUGET_API_KEY` with your actual API key from NuGet.org.

### Option 2: Manual Steps

1. **Clean the project:**
   ```bash
   dotnet clean --configuration Release
   ```

2. **Build in Release mode:**
   ```bash
   dotnet build --configuration Release
   ```

3. **Create the package:**
   ```bash
   dotnet pack Santel.TradeSharp.Indicators.csproj --configuration Release --output ./nupkg
   ```

4. **Publish to NuGet:**
   ```bash
   dotnet nuget push ./nupkg/Santel.TradeSharp.Indicators.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

## After Publishing

1. **Verify**: Check https://www.nuget.org/packages/Santel.TradeSharp.Indicators
2. **Wait**: It may take 5-10 minutes for the package to appear in search results
3. **Install**: Test installation in a new project:
   ```bash
   dotnet add package Santel.TradeSharp.Indicators
   ```

## Version Guidelines

Follow Semantic Versioning (SemVer):
- **Major** (1.0.0): Breaking changes
- **Minor** (1.1.0): New features, backward compatible
- **Patch** (1.0.1): Bug fixes, backward compatible

## Troubleshooting

### "Package already exists"
You cannot overwrite an existing version. Increment the version number in the .csproj file.

### "Invalid API Key"
- Verify your API key at https://www.nuget.org/account/apikeys
- Ensure the key has "Push" permissions
- Check that the key hasn't expired

### Build Errors
Run `dotnet build` and fix any compilation errors before packing.

## Security Note

**Never commit your API key to source control!**
- Keep it in a secure location
- Use environment variables or secret management tools
- Regenerate the key if accidentally exposed
