# QCLI Installation Guide

## 📦 Installing QCLI

### Option 1: Install from NuGet (Recommended)

```bash
# Install globally as a .NET tool
dotnet tool install --global QuillySOFT.CLI

# Verify installation
qcli --version
```

### Option 2: Install from Source

```bash
# Clone the repository
git clone https://github.com/QuillySoft/QCli.git
cd QCli

# Build and pack
dotnet pack src/Apps/Tools.Cli/Tools.Cli.csproj -c Release

# Install locally
dotnet tool install --global --add-source ./src/Apps/Tools.Cli/bin/Release QuillySOFT.CLI
```

## 🚀 Quick Start

### 1. Initialize a New Project

```bash
# Create a new project with CLIO architecture
qcli scaffold MyProject

# Or initialize QCLI in an existing project
cd MyExistingProject
qcli init
```

### 2. Generate Your First Entity

```bash
# Generate complete CRUD operations
qcli add Order --all

# Generate specific operations
qcli add Product --create --read
qcli add Customer --all --entity-type FullyAudited
```

### 3. Check Project Health

```bash
# Diagnose and fix common issues
qcli doctor --fix

# View current configuration
qcli config show
```

## 📋 Command Reference

### Core Commands

| Command | Description | Example |
|---------|-------------|---------|
| `init` | Initialize QCLI in project | `qcli init -i` |
| `add` | Generate CRUD operations | `qcli add Order --all` |
| `scaffold` | Create new project | `qcli scaffold MyApi` |
| `config` | Manage configuration | `qcli config show` |
| `doctor` | Diagnose issues | `qcli doctor --fix` |
| `list` | List templates/entities | `qcli list templates` |
| `update` | Check for updates | `qcli update --check` |

### Add Command Options

| Option | Description |
|--------|-------------|
| `--all` | Generate all CRUD operations |
| `--create` | Generate create operation |
| `--read` | Generate read operations |
| `--update` | Generate update operation |
| `--delete` | Generate delete operation |
| `--entity-type` | Entity type (Audited/FullyAudited) |
| `--no-tests` | Skip generating tests |
| `--no-permissions` | Skip generating permissions |
| `--template` | Use specific template |
| `--dry-run` | Preview without creating files |

## ⚙️ Configuration

QCLI uses `qcli.json` for configuration:

```json
{
  "project": {
    "name": "MyProject",
    "namespace": "MyProject"
  },
  "paths": {
    "domainPath": "src/Domain",
    "applicationPath": "src/Application",
    "persistencePath": "src/Infrastructure/Persistence",
    "webApiPath": "src/WebApi"
  },
  "codeGeneration": {
    "generateTests": true,
    "generatePermissions": true,
    "generateEvents": true,
    "useRecords": true
  }
}
```

## 🔧 Troubleshooting

### Common Issues

1. **Command not found**: Ensure .NET tools are in your PATH
2. **Permission errors**: Run with elevated privileges if needed
3. **Template errors**: Run `qcli update --templates` to update templates

### Getting Help

```bash
# Get help for any command
qcli add --help
qcli config --help

# Check project health
qcli doctor
```

## 🔄 Updating QCLI

```bash
# Check for updates
qcli update --check

# Update to latest version
qcli update

# Update templates only
qcli update --templates
```
