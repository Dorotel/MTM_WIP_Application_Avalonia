# ReactiveUI.XamForms Assembly Loading Issue - Troubleshooting Guide

## Problem Analysis

The error `Could not load file or assembly 'ReactiveUI.XamForms, Version=20.1.0.0'` indicates that the application is trying to load the Xamarin Forms version of ReactiveUI instead of the Avalonia version. This is a common issue when there are:

1. **Incorrect package references** (transitive dependencies)
2. **Assembly binding conflicts**
3. **Cached assemblies in bin/obj folders**
4. **Version mismatches in packages**

## Current Package Analysis

From your project file, the packages look correct:
```xml
<PackageReference Include="Avalonia.ReactiveUI" Version="11.3.4" />
```

However, the error suggests a version mismatch (20.1.0.0 vs 11.3.4).

## Resolution Steps

### Step 1: Clean Build Environment

```powershell
# Clean all build artifacts
dotnet clean
Remove-Item -Recurse -Force bin, obj -ErrorAction SilentlyContinue

# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages fresh
dotnet restore --force
```

### Step 2: Check for Package Conflicts

Run this command to see all package dependencies:
```powershell
dotnet list package --include-transitive
```

Look for any packages that might be pulling in Xamarin Forms or incorrect ReactiveUI versions.

### Step 3: Verify Assembly Binding

Create or update `app.config` in your project root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="ReactiveUI" publicKeyToken="552dd7e47bc69ba8" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-20.1.0.0" newVersion="20.1.63.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Reactive" publicKeyToken="94bc3704cddfc263" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```

### Step 4: Update Project File

Ensure your project file has explicit version constraints:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>

  <!-- Force specific ReactiveUI version -->
  <ItemGroup>
    <PackageReference Include="ReactiveUI" Version="20.1.63" />
    <PackageReference Include="System.Reactive" Version="6.0.1" />
    <PackageReference Include="Avalonia.ReactiveUI" Version="11.3.4" />
  </ItemGroup>

  <!-- Existing packages... -->
</Project>
```

### Step 5: Check for Global Tools Conflicts

Sometimes global tools can interfere:
```powershell
dotnet tool list -g
# If you see any Xamarin-related tools, consider uninstalling them
```

### Step 6: Force Package Update

```powershell
# Update all packages to latest compatible versions
dotnet add package ReactiveUI --version 20.1.63
dotnet add package System.Reactive --version 6.0.1
dotnet restore
```

### Step 7: Diagnostic Build

Add this to your project file temporarily for diagnostics:

```xml
<PropertyGroup>
  <MSBuildVerbosity>diagnostic</MSBuildVerbosity>
  <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
</PropertyGroup>
```

Then build and check the output for assembly loading details.

## Alternative Solution: Use PackageReference with ExcludeAssets

If there are conflicting transitive dependencies:

```xml
<PackageReference Include="Avalonia.ReactiveUI" Version="11.3.4">
  <ExcludeAssets>none</ExcludeAssets>
  <IncludeAssets>all</IncludeAssets>
</PackageReference>

<!-- Explicitly exclude problematic packages -->
<PackageReference Include="ReactiveUI.XamForms" Version="20.1.0.0">
  <ExcludeAssets>all</ExcludeAssets>
</PackageReference>
```

## Root Cause Analysis

The most likely causes are:

1. **Mixed package versions**: Some package is pulling in an older ReactiveUI version
2. **NuGet cache corruption**: Old cached packages interfering
3. **Assembly binding issues**: .NET runtime loading wrong assembly version
4. **Transitive dependency conflicts**: Another package depending on Xamarin Forms ReactiveUI

## Verification Steps

After applying fixes:

1. **Clean build**: `dotnet clean && dotnet build`
2. **Check dependencies**: `dotnet list package --include-transitive | findstr ReactiveUI`
3. **Run application**: `dotnet run`
4. **Check loaded assemblies**: Use Process Monitor or Fusion Log Viewer

## Prevention

To prevent this issue in future:

1. **Pin package versions** explicitly in project file
2. **Use Directory.Build.props** for version management across multiple projects
3. **Regular package audits** with `dotnet list package --outdated`
4. **Automated dependency scanning** in CI/CD pipeline

## Emergency Workaround

If the issue persists, try this temporary workaround in your `Program.cs`:

```csharp
static Program()
{
    // Force load correct ReactiveUI assembly
    AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
    {
        if (args.Name.StartsWith("ReactiveUI.XamForms"))
        {
            // Redirect to Avalonia ReactiveUI
            return typeof(ReactiveUI.ReactiveObject).Assembly;
        }
        return null;
    };
}
```

This will redirect any attempts to load ReactiveUI.XamForms to the correct ReactiveUI assembly.
