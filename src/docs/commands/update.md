# Update Command

The `update` command keeps your QCLI tool and templates up to date with the latest versions. It can check for and install both tool updates and template updates.

## Syntax

```bash
qcli update [options]
```

## Options

| Option | Alias | Description | Default |
|--------|-------|-------------|---------|
| `--check` | `-c` | Check for updates without installing | `false` |
| `--templates` | `-t` | Update templates only | `false` |
| `--help` | `-h` | Show help information | - |

## Usage Examples

### Check for Tool Updates

```bash
# Check if new QCLI version is available
qcli update --check

# Example output:
# 🔄 Checking for updates...
# 
# 📦 New version available: 1.0.1
# Current version: 1.0.0
# 
# Run 'qcli update' to install the latest version.
```

### Update QCLI Tool

```bash
# Update QCLI to the latest version
qcli update

# Example output:
# 🔄 Checking for updates...
# 
# 📦 New version available: 1.0.1
# Current version: 1.0.0
# 
# Do you want to update now? (y/N) y
# 🔄 Updating QCLI to version 1.0.1...
# 
# Downloading update... [████████████████████] 100%
# Installing update...   [████████████████████] 100%
# 
# ✅ QCLI updated successfully!
# Restart your terminal to use the new version.
```

### Update Templates Only

```bash
# Check for template updates
qcli update --templates --check

# Update templates
qcli update --templates

# Example output:
# 🔄 Checking for updates...
# 
# Checking for template updates...
# 📋 Template updates available:
# 
# ┌─────────────────────┬─────────┬──────────────────┐
# │ Template            │ Version │ Status           │
# ├─────────────────────┼─────────┼──────────────────┤
# │ clean-architecture  │ 1.1.0   │ Update Available │
# │ minimal             │ 1.0.0   │ Up to date       │
# │ ddd                 │ 1.0.1   │ Update Available │
# └─────────────────────┴─────────┴──────────────────┘
# 
# Update all templates? (y/N) y
# Updating templates... [████████████████████] 100%
# ✓ Updated template: clean-architecture
# ✓ Updated template: ddd
# ✅ All templates updated successfully!
```

## Update Sources

### Tool Updates
- **Source**: NuGet package registry
- **Package**: `QuillySOFT.CLI`
- **Update Method**: `dotnet tool update -g QuillySOFT.CLI`
- **Verification**: Version comparison via NuGet API

### Template Updates
- **Source**: Template repository
- **Types**: Project scaffolding templates, code generation templates
- **Available Templates**:
  - `clean-architecture` - Complete Clean Architecture setup
  - `minimal` - Minimal project structure
  - `ddd` - Domain-Driven Design template
  - `microservice` - Microservice architecture
  - `blazor` - Blazor frontend with Clean Architecture backend

## Update Process

### Tool Update Flow
1. **Version Check**: Query NuGet API for latest version
2. **Comparison**: Compare with current installed version
3. **Download**: Download new version if available
4. **Install**: Replace current installation
5. **Verification**: Confirm successful update

### Template Update Flow
1. **Template Discovery**: Scan available templates
2. **Version Check**: Check each template version
3. **Update Available**: Identify templates with updates
4. **Selective Update**: Update individual or all templates
5. **Validation**: Verify template integrity

## Interactive Mode

The update command provides interactive prompts for user confirmation:

```bash
# Tool update confirmation
Do you want to update now? (y/N)

# Template update confirmation  
Update all templates? (y/N)
```

## Automatic Updates

### Check-Only Mode
```bash
# Just check, don't install
qcli update --check
qcli update --templates --check
```

### Silent Updates (Future)
```bash
# Future feature: Silent updates
qcli update --silent
qcli update --templates --silent
```

## Error Handling

### Common Issues and Solutions

#### Network Connectivity
```bash
# Error output:
# ❌ Error checking for updates: Network unreachable

# Solution:
# - Check internet connection
# - Verify firewall settings
# - Try again later
```

#### Update Failure
```bash
# Error output:
# ❌ Update failed: Access denied

# Solution:
# Try updating manually: dotnet tool update -g QuillySOFT.CLI
```

#### Template Corruption
```bash
# Error output:
# ❌ Template validation failed

# Solution:
# - Run 'qcli doctor' to diagnose
# - Reinstall templates manually
# - Check template directory permissions
```

## Integration with Other Commands

### With Doctor Command
```bash
# Check for updates as part of health check
qcli doctor

# Output includes update status:
# 📋 Update Status:
# ├─ QCLI Version: 1.0.0 (latest: 1.0.1) ⚠️
# └─ Templates: 2 updates available ⚠️
```

### With Configuration
```bash
# Configure update behavior
qcli config set update.checkInterval "weekly"
qcli config set update.autoCheckTemplates true
```

## Best Practices

### Regular Updates
- Check for updates weekly
- Update templates before starting new projects
- Keep QCLI tool updated for latest features

### Template Management
- Update templates independently from tool
- Test templates after updates in development environment
- Backup custom templates before updates

### Version Control
- Document tool and template versions in project documentation
- Consider version pinning for enterprise environments
- Test updates in staging environment first

## Troubleshooting

### Update Stuck
If an update appears to hang:
1. Cancel with `Ctrl+C`
2. Try manual update: `dotnet tool update -g QuillySOFT.CLI`
3. Clear NuGet cache: `dotnet nuget locals all --clear`

### Template Conflicts
If template updates fail:
1. Run `qcli doctor` to check template health
2. Backup and remove template directory
3. Re-run `qcli update --templates`

### Permission Issues
If update fails with permission errors:
1. Run terminal as administrator (Windows)
2. Use `sudo` (macOS/Linux): `sudo dotnet tool update -g QuillySOFT.CLI`
3. Check file system permissions

## Related Commands

- [`qcli doctor`](./doctor.md) - Health check including update status
- [`qcli config`](./config.md) - Configure update settings
- [`qcli list templates`](./list.md) - List available templates and versions

## Version History

- **1.0.0**: Initial update command
- **1.0.1**: Added template-specific updates
- **1.1.0**: Enhanced error handling and progress display

## See Also

- [Configuration Guide](../configuration/configuration.md)
- [Template System](../configuration/templates.md)
- [Troubleshooting](../advanced/troubleshooting.md)
