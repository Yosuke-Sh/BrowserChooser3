# Troubleshooting

This comprehensive troubleshooting guide will help you resolve common issues with BrowserChooser3.

## 🚨 Quick Diagnostics

### Enable Debug Logging
First, enable logging to get detailed information about any issues:

```bash
BrowserChooser3.exe --log
```

This will create a `Logs/` folder with detailed log files that can help diagnose problems.

### Check System Requirements
Ensure your system meets the minimum requirements:
- Windows 10 (1903+) or Windows 11
- .NET 8.0 Runtime installed
- x64 architecture
- 100 MB available RAM

## 🔧 Common Issues

### Application Won't Start

#### Issue: "Application failed to start" or "Missing .NET Framework"
**Symptoms**: Error message about missing .NET or application won't launch
**Solutions**:
1. **Install .NET 8.0 Runtime**:
   - Download from [Microsoft .NET Download](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Choose "Runtime" (not SDK) for Windows x64
   - Run the installer and restart your computer

2. **Verify Installation**:
   ```bash
   dotnet --version
   ```
   Should show version 8.0.x

3. **Check Architecture**:
   - Ensure you downloaded the x64 version
   - Verify your system is 64-bit

#### Issue: "File not found" or "Path not found"
**Symptoms**: Error about missing files or paths
**Solutions**:
1. **Check File Location**: Ensure all files are in the same folder
2. **Verify Permissions**: Run as administrator if needed
3. **Check Antivirus**: Some antivirus software may block the application

#### Issue: Application starts but immediately closes
**Symptoms**: BrowserChooser3 appears briefly then disappears
**Solutions**:
1. **Check Command Line**: Run from command prompt to see error messages
2. **Enable Logging**: Use `--log` flag to capture startup errors
3. **Check Dependencies**: Verify all required files are present

### Transparency Issues

#### Issue: Transparency not working
**Symptoms**: Window appears opaque despite transparency settings
**Solutions**:
1. **Check Windows Settings**:
   - Go to Settings → Personalization → Colors
   - Ensure "Transparency effects" is enabled
   - Restart BrowserChooser3

2. **Update Graphics Drivers**:
   - Check for graphics driver updates
   - Restart after updating drivers

3. **Try Different Settings**:
   - Change transparency color
   - Adjust opacity value
   - Test with different corner radius

4. **Check .NET Version**:
   - Ensure .NET 8.0 is installed
   - Try reinstalling .NET if needed

#### Issue: Rounded corners appear jagged
**Symptoms**: Corners are pixelated or not smooth
**Solutions**:
1. **Enable Hardware Acceleration**:
   - Update graphics drivers
   - Check Windows graphics settings

2. **Adjust DPI Settings**:
   - Check Windows DPI scaling
   - Try different DPI settings

3. **Reduce Corner Radius**:
   - Use smaller radius values
   - Test with radius 5-10

### Browser Detection Issues

#### Issue: Browsers not detected automatically
**Symptoms**: No browsers appear in the list
**Solutions**:
1. **Manual Browser Addition**:
   - Press `O` to open options
   - Go to "Browsers" tab
   - Click "Add" to manually add browsers

2. **Check Browser Installation**:
   - Verify browsers are properly installed
   - Check if browsers are in standard locations

3. **Registry Issues**:
   - Some browsers may not register properly
   - Try reinstalling problematic browsers

#### Issue: Custom browser won't launch
**Symptoms**: Browser appears in list but won't start
**Solutions**:
1. **Check Path**: Verify the executable path is correct
2. **Check Arguments**: Ensure launch arguments are valid
3. **Test Manually**: Try running the browser manually with the same arguments
4. **Check Permissions**: Ensure BrowserChooser3 has permission to launch the browser

### Performance Issues

#### Issue: Slow startup or high CPU usage
**Symptoms**: Application takes long to start or uses excessive CPU
**Solutions**:
1. **Disable Transparency**: Temporarily disable transparency effects
2. **Reduce Corner Radius**: Use smaller corner radius values
3. **Check Background Processes**: Close unnecessary background applications
4. **Update Graphics Drivers**: Ensure drivers are up to date

#### Issue: Memory usage keeps increasing
**Symptoms**: Memory usage grows over time
**Solutions**:
1. **Restart Application**: Close and restart BrowserChooser3
2. **Check for Memory Leaks**: Monitor with Task Manager
3. **Report Issue**: Create a GitHub issue with memory usage details

### Configuration Issues

#### Issue: Settings not saving
**Symptoms**: Changes to settings are lost after restart
**Solutions**:
1. **Check File Permissions**: Ensure write access to the configuration file
2. **Check File Location**: Verify `BrowserChooser3Config.xml` is in the correct location
3. **Run as Administrator**: Try running with elevated privileges

#### Issue: Configuration file corrupted
**Symptoms**: Application won't start or shows errors
**Solutions**:
1. **Backup and Delete**: Rename or delete `BrowserChooser3Config.xml`
2. **Restart Application**: BrowserChooser3 will create a new configuration file
3. **Restore Settings**: Manually reconfigure your settings

### Display Issues

#### Issue: Window appears in wrong location
**Symptoms**: Window opens off-screen or in unexpected position
**Solutions**:
1. **Reset Position**: Delete configuration file to reset window position
2. **Check Multiple Monitors**: Ensure window isn't on a disconnected monitor
3. **Use Alt+Tab**: Try Alt+Tab to bring window to foreground

#### Issue: Text appears blurry or pixelated
**Symptoms**: Text is hard to read or appears fuzzy
**Solutions**:
1. **Check DPI Settings**: Adjust Windows DPI scaling
2. **Update Graphics Drivers**: Ensure drivers support high DPI
3. **Disable Transparency**: Try without transparency effects

## 🔍 Advanced Troubleshooting

### Log Analysis

#### Enable Detailed Logging
```bash
BrowserChooser3.exe --log
```

#### Key Log Files
- `BrowserChooser3.log`: Main application log
- `Startup.log`: Startup and initialization log
- `Error.log`: Error-specific log

#### Common Log Messages
- `"Application started"`: Normal startup
- `"Transparency settings applied"`: Transparency working
- `"Browser detected"`: Browser detection working
- `"Configuration loaded"`: Settings loaded successfully

### Registry Issues

#### Check Browser Registration
```bash
reg query "HKEY_CLASSES_ROOT\http\shell\open\command"
reg query "HKEY_CLASSES_ROOT\https\shell\open\command"
```

#### Reset Browser Associations
1. Open Settings → Apps → Default apps
2. Choose default browser
3. Restart BrowserChooser3

### Network Issues

#### Issue: URLs not opening
**Symptoms**: Clicking browser buttons doesn't open URLs
**Solutions**:
1. **Check URL Format**: Ensure URLs start with `http://` or `https://`
2. **Test Browser**: Try opening the URL directly in the browser
3. **Check Firewall**: Ensure firewall isn't blocking the application
4. **Check Antivirus**: Some antivirus software may block URL opening

## 🛠️ Diagnostic Tools

### Built-in Diagnostics
BrowserChooser3 includes several diagnostic features:

#### System Information
- Press `O` to open options
- Check system information in the "Other" tab
- Verify .NET version and system details

#### Browser Detection Test
- Go to "Browsers" tab in options
- Click "Detect Browsers" to test detection
- Check if browsers are found

### External Tools

#### Process Monitor
Use Process Monitor to track file and registry access:
1. Download Process Monitor from Microsoft
2. Run BrowserChooser3 with Process Monitor
3. Look for access denied errors

#### Dependency Walker
Use Dependency Walker to check for missing DLLs:
1. Download Dependency Walker
2. Open BrowserChooser3.exe
3. Check for missing dependencies

## 📞 Getting Help

### Before Asking for Help
1. **Enable Logging**: Use `--log` flag
2. **Check This Guide**: Look for similar issues
3. **Search GitHub Issues**: Check existing reports
4. **Gather Information**: Collect system details and error messages

### Information to Include
When reporting issues, include:
- **Windows Version**: Windows 10/11 version
- **.NET Version**: Output of `dotnet --version`
- **Error Messages**: Exact error text
- **Steps to Reproduce**: How to trigger the issue
- **Log Files**: Relevant log entries
- **System Specs**: CPU, RAM, graphics card

### Reporting Issues
1. **GitHub Issues**: Create a new issue on GitHub
2. **Include Logs**: Attach relevant log files
3. **Be Specific**: Provide detailed information
4. **Test Solutions**: Try suggested solutions and report results

## 🔄 Recovery Procedures

### Complete Reset
If all else fails, perform a complete reset:

1. **Close BrowserChooser3**
2. **Delete Configuration File**: Remove `BrowserChooser3Config.xml`
3. **Delete Log Files**: Remove `Logs/` folder
4. **Restart Application**: Launch BrowserChooser3
5. **Reconfigure Settings**: Set up your preferences again

### Partial Reset
For specific issues, try partial resets:

#### Reset Transparency Settings
1. Open options dialog
2. Go to Display tab
3. Disable transparency
4. Restart application
5. Re-enable transparency with default settings

#### Reset Browser Settings
1. Open options dialog
2. Go to Browsers tab
3. Remove all browsers
4. Click "Detect Browsers"
5. Manually add any missing browsers

## 📚 Related Resources

- [Installation Guide](Installation-Guide)
- [Configuration Guide](Configuration-Guide)
- [Transparency Settings](Transparency-Settings)
- [Accessibility Features](Accessibility-Features)

---

*If you can't find a solution here, please [create a GitHub issue](https://github.com/your-username/BrowserChooser3/issues) with detailed information about your problem.*

