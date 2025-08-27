# Configuration Guide

This comprehensive guide covers all configuration options available in BrowserChooser3.

## 🎛️ Accessing Configuration

### Opening the Options Dialog
1. **Launch BrowserChooser3**
2. **Press `O` key** to open the Options dialog
3. **Navigate between tabs** using mouse or keyboard

### Configuration File Location
- **File**: `BrowserChooser3Config.xml`
- **Location**: Same folder as `BrowserChooser3.exe`
- **Format**: XML format for easy editing
- **Backup**: Always backup before manual editing

## 📑 Configuration Tabs

### Display Tab
Controls the visual appearance and transparency settings.

#### Transparency Settings
- **Enable Transparency**: Master switch for transparency effects
- **Transparency Color**: Color that becomes transparent (default: Magenta)
- **Opacity**: Window transparency level (0.01-1.00, default: 0.9)
- **Hide Title Bar**: Remove the window title bar
- **Rounded Corners Radius**: Corner rounding (0-50, default: 0)

#### Visual Settings
- **Show URLs**: Display URLs in the main window
- **Reveal Short URLs**: Expand shortened URLs
- **Background Color**: Custom background color picker

#### Accessibility Settings
- **Accessibility Settings Button**: Opens detailed accessibility options

### Browsers Tab
Manages browser detection and configuration.

#### Browser List
- **Add**: Add a new browser manually
- **Edit**: Modify existing browser settings
- **Delete**: Remove a browser from the list
- **Detect Browsers**: Automatically find installed browsers

#### Browser Properties
- **Name**: Display name for the browser
- **Target**: Path to the browser executable
- **Arguments**: Command-line arguments for launching
- **Icon**: Icon file or index for display
- **Scale**: Icon size multiplier
- **Position**: Grid position (X, Y coordinates)
- **Visible**: Show/hide in the browser list

### Protocols Tab
Configures protocol handlers for different URL schemes.

#### Protocol List
- **Add**: Add a new protocol handler
- **Edit**: Modify existing protocol settings
- **Delete**: Remove a protocol handler

#### Protocol Properties
- **Name**: Protocol name (e.g., "http", "https", "ftp")
- **Description**: Human-readable description
- **Default Browser**: Browser to use for this protocol
- **Custom Arguments**: Special arguments for this protocol

### File Types Tab
Manages file type associations.

#### File Type List
- **Add**: Add a new file type
- **Edit**: Modify existing file type settings
- **Delete**: Remove a file type

#### File Type Properties
- **Extension**: File extension (e.g., ".html", ".pdf")
- **Description**: File type description
- **Default Browser**: Browser to use for this file type
- **MIME Type**: MIME type for the file

### Categories Tab
Organizes browsers into categories.

#### Category Management
- **Add**: Create a new category
- **Edit**: Modify category settings
- **Delete**: Remove a category
- **Assign Browsers**: Move browsers between categories

#### Category Properties
- **Name**: Category display name
- **Description**: Category description
- **Icon**: Category icon
- **Color**: Category color for visual grouping

### Grid Tab
Controls the layout and appearance of browser buttons.

#### Grid Layout
- **Width**: Number of columns (default: 5)
- **Height**: Number of rows (default: 1)
- **Show Grid**: Display grid lines
- **Grid Color**: Color of grid lines

#### Icon Settings
- **Icon Width**: Width of browser icons (default: 90)
- **Icon Height**: Height of browser icons (default: 110)
- **Icon Gap Width**: Horizontal spacing between icons (default: 0)
- **Icon Gap Height**: Vertical spacing between icons (default: 0)
- **Icon Scale**: Global scale factor for all icons (default: 1.0)

### Other Tab
Miscellaneous settings and system options.

#### General Settings
- **Default Message**: Text displayed in the main window
- **Default Delay**: Countdown timer duration (seconds)
- **Options Shortcut**: Keyboard shortcut to open options (default: 'O')
- **Allow Stay Open**: Keep window open after browser selection

#### System Settings
- **Portable Mode**: Run without system installation
- **Check Default on Launch**: Verify default browser on startup
- **Advanced Screens**: Enable advanced screen detection
- **Starting Position**: Window startup position
- **Offset X/Y**: Fine-tune window position

#### Network Settings
- **User Agent**: HTTP user agent string
- **Download Detection File**: Enable download detection
- **Canonicalize**: Normalize URLs before opening
- **Canonicalize Appended Text**: Text to append to canonicalized URLs

#### Logging Settings
- **Enable Logging**: Turn on debug logging
- **Log Level**: Detail level for logging (1-5)
- **Extract DLLs**: Extract embedded DLLs for troubleshooting

#### Update Settings
- **Automatic Updates**: Check for updates automatically
- **Update Check Interval**: How often to check for updates

## ⚙️ Advanced Configuration

### Manual XML Editing
For advanced users, you can edit the configuration file directly:

#### Configuration File Structure
```xml
<?xml version="1.0" encoding="utf-8"?>
<Settings>
  <FileVersion>6</FileVersion>
  <EnableTransparency>true</EnableTransparency>
  <TransparencyColor>-65536</TransparencyColor>
  <Opacity>0.9</Opacity>
  <HideTitleBar>true</HideTitleBar>
  <RoundedCornersRadius>0</RoundedCornersRadius>
  <!-- Additional settings... -->
</Settings>
```

#### Important Notes
- **Backup First**: Always backup before editing
- **Valid XML**: Ensure proper XML formatting
- **Restart Required**: Restart BrowserChooser3 after editing
- **Validation**: Invalid XML will reset to defaults

### Environment Variables
Some settings can be controlled via environment variables:

#### Available Variables
- `BROWSERCHOOSER_IGNORE_SETTINGS`: Ignore settings file
- `BROWSERCHOOSER_ICON_SCALE`: Override icon scale
- `BROWSERCHOOSER_CANONICALIZE`: Force URL canonicalization
- `BROWSERCHOOSER_CANONICALIZE_TEXT`: Canonicalization text

#### Usage Example
```bash
set BROWSERCHOOSER_ICON_SCALE=1.5
BrowserChooser3.exe
```

### Registry Settings
Some system-wide settings are stored in the registry:

#### Registry Locations
- `HKEY_CURRENT_USER\Software\BrowserChooser3`: User settings
- `HKEY_LOCAL_MACHINE\Software\BrowserChooser3`: System settings

#### Important Keys
- `InstallPath`: Installation directory
- `ConfigPath`: Configuration file path
- `LogPath`: Log file directory

## 🎨 Customization Examples

### Modern Glass Effect
```xml
<EnableTransparency>true</EnableTransparency>
<TransparencyColor>-65536</TransparencyColor>
<Opacity>0.8</Opacity>
<HideTitleBar>true</HideTitleBar>
<RoundedCornersRadius>15</RoundedCornersRadius>
```

### High Contrast Mode
```xml
<EnableTransparency>false</EnableTransparency>
<BackgroundColor>-16777216</BackgroundColor>
<ShowFocus>true</ShowFocus>
<FocusBoxColor>-256</FocusBoxColor>
<FocusBoxWidth>3</FocusBoxWidth>
```

### Minimal Interface
```xml
<ShowURLs>false</ShowURLs>
<DefaultMessage>Choose Browser</DefaultMessage>
<IconScale>0.8</IconScale>
<IconGapWidth>5</IconGapWidth>
<IconGapHeight>5</IconGapHeight>
```

### Developer Mode
```xml
<EnableLogging>true</EnableLogging>
<LogLevel>5</LogLevel>
<ShowURLs>true</ShowURLs>
<RevealShortURLs>true</RevealShortURLs>
<Canonicalize>true</Canonicalize>
```

## 🔧 Configuration Management

### Backup and Restore
#### Creating Backups
1. **Copy Configuration File**: Copy `BrowserChooser3Config.xml`
2. **Include Resources**: Backup any custom icons or images
3. **Document Settings**: Note any custom configurations

#### Restoring Backups
1. **Close BrowserChooser3**
2. **Replace Configuration File**: Copy backup over existing file
3. **Restart Application**: Launch BrowserChooser3
4. **Verify Settings**: Check that settings are restored correctly

### Configuration Migration
#### From Browser Chooser 2
BrowserChooser3 doesn't automatically migrate from Browser Chooser 2:
1. **Export Settings**: Note your BC2 settings
2. **Configure Manually**: Set up equivalent settings in BC3
3. **Test Thoroughly**: Verify all functionality works

#### Between BC3 Versions
Settings are generally compatible between versions:
1. **Backup Current**: Save current configuration
2. **Upgrade Application**: Install new version
3. **Test Settings**: Verify settings still work
4. **Report Issues**: Report any compatibility problems

### Configuration Validation
#### Built-in Validation
BrowserChooser3 validates settings on startup:
- **Range Checking**: Ensures values are within valid ranges
- **File Existence**: Verifies referenced files exist
- **Format Validation**: Checks data formats

#### Manual Validation
You can validate settings manually:
1. **Check Logs**: Look for validation errors in logs
2. **Test Functions**: Try each configured feature
3. **Reset if Needed**: Reset to defaults if validation fails

## 🚨 Troubleshooting Configuration

### Common Configuration Issues

#### Settings Not Saving
**Symptoms**: Changes are lost after restart
**Solutions**:
1. Check file permissions
2. Verify write access to configuration file
3. Run as administrator if needed

#### Invalid Configuration
**Symptoms**: Application won't start or shows errors
**Solutions**:
1. Delete configuration file to reset
2. Check XML syntax if editing manually
3. Restore from backup

#### Performance Issues
**Symptoms**: Slow startup or high resource usage
**Solutions**:
1. Reduce icon scale and grid size
2. Disable transparency effects
3. Limit number of browsers
4. Disable logging if not needed

### Configuration Recovery
#### Reset to Defaults
1. Close BrowserChooser3
2. Delete `BrowserChooser3Config.xml`
3. Restart application
4. Reconfigure settings

#### Partial Reset
1. Open configuration file
2. Remove specific problematic settings
3. Restart application
4. Reconfigure removed settings

## 📚 Related Topics

- [Transparency Settings](Transparency-Settings)
- [Accessibility Features](Accessibility-Features)
- [Troubleshooting](Troubleshooting)
- [User Guide](User-Guide)

---

*For specific configuration examples and advanced usage, see the individual feature guides in this wiki.*

