# 🔧 Installation Guide

Complete guide to installing and setting up QCLI in your development environment.

## Prerequisites

- **.NET 8 SDK** or later
- **PowerShell** (Windows) or **Bash** (Linux/macOS)
- **Git** (for GitHub installation)

## Installation Methods

### Method 1: Install from GitHub (Current)

```powershell
# Clone the repository
git clone https://github.com/QuillySoft/QCli.git
cd QCli

# Build the project
dotnet build --configuration Release

# Pack as NuGet package
dotnet pack src/Apps/Tools.Cli/Tools.Cli.csproj --configuration Release

# Install as global tool
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Release QuillySOFT.CLI
```

### Method 2: Install from NuGet (Coming Soon)

```powershell
# This will be available soon
dotnet tool install --global QuillySOFT.CLI
```

## Verify Installation

```powershell
# Check if QCLI is installed
qcli --version

# Output should show:
# 1.0.0
```

```powershell
# Test basic functionality
qcli --help

# Should display help with available commands
```

## Update Installation

### Update from GitHub

```powershell
# Navigate to your local repository
cd path/to/QCli

# Pull latest changes
git pull origin main

# Uninstall current version
dotnet tool uninstall --global QuillySOFT.CLI

# Rebuild and reinstall
dotnet build --configuration Release
dotnet pack src/Apps/Tools.Cli/Tools.Cli.csproj --configuration Release
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Release QuillySOFT.CLI
```

### Update from NuGet (When Available)

```powershell
# Update to latest version
dotnet tool update --global QuillySOFT.CLI
```

## Uninstallation

```powershell
# Remove QCLI global tool
dotnet tool uninstall --global QuillySOFT.CLI
```

## Troubleshooting Installation

### Common Issues

#### Tool Already Installed
```
Error: Tool 'QuillySOFT.CLI' is already installed.
```

**Solution:**
```powershell
# Uninstall first, then reinstall
dotnet tool uninstall --global QuillySOFT.CLI
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Release QuillySOFT.CLI
```

#### .NET SDK Not Found
```
Error: The command 'dotnet' was not found.
```

**Solution:**
1. Download and install [.NET 8 SDK](https://dotnet.microsoft.com/download)
2. Restart your terminal
3. Verify with `dotnet --version`

#### Permission Errors (Windows)
```
Error: Access to the path is denied.
```

**Solution:**
```powershell
# Run PowerShell as Administrator
# Or install to user profile only
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Release QuillySOFT.CLI --tool-path $env:USERPROFILE\.dotnet\tools
```

#### Build Errors
```
Error: Build failed. Review the build log for details.
```

**Solution:**
```powershell
# Restore packages first
dotnet restore
dotnet clean
dotnet build --configuration Release --verbosity detailed
```

## System Requirements

| Requirement | Minimum | Recommended |
|-------------|---------|-------------|
| .NET SDK | 8.0 | 8.0 or later |
| OS | Windows 10, Linux, macOS | Windows 11, Latest Linux/macOS |
| RAM | 4 GB | 8 GB or more |
| Disk Space | 500 MB | 1 GB |
| PowerShell | 5.1 | 7.0 or later |

## Post-Installation Setup

After successful installation:

1. **Initialize your first project:**
```powershell
mkdir MyProject
cd MyProject
qcli init
```

2. **Verify configuration:**
```powershell
qcli config show
```

3. **Generate your first entity:**
```powershell
qcli add Product --all
```

## Next Steps

- [Quick Start Guide](quick-start.md) - Get started with your first entity
- [Project Setup](project-setup.md) - Initialize QCLI in existing projects
- [Configuration Guide](configuration/configuration.md) - Customize QCLI settings

---

**Having issues?** Check the [Troubleshooting Guide](advanced/troubleshooting.md) or review [Common Errors](reference/error-codes.md).
