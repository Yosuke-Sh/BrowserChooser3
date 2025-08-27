# Installation Guide

This guide will help you install and set up BrowserChooser3 on your Windows system.

## 📋 System Requirements

### Minimum Requirements
- **Operating System**: Windows 10 (version 1903 or later) or Windows 11
- **Architecture**: x64 (64-bit)
- **.NET Runtime**: .NET 8.0 Runtime
- **RAM**: 100 MB available memory
- **Storage**: 50 MB free disk space

### Recommended Requirements
- **Operating System**: Windows 11 (latest version)
- **RAM**: 200 MB available memory
- **Storage**: 100 MB free disk space

## 🔽 Download Options

### Option 1: Pre-built Executable (Recommended)
1. Go to the [Releases page](https://github.com/your-username/BrowserChooser3/releases)
2. Download the latest `BrowserChooser3.exe` file
3. Save it to your desired location (e.g., `C:\Programs\BrowserChooser3\`)

### Option 2: Build from Source
If you prefer to build from source or want the latest development version:

```bash
git clone https://github.com/your-username/BrowserChooser3.git
cd BrowserChooser3
dotnet build --configuration Release
```

## 🛠️ Installation Steps

### Step 1: Install .NET 8.0 Runtime
BrowserChooser3 requires .NET 8.0 Runtime to run.

1. Visit [Microsoft .NET Download](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Download the **.NET 8.0 Runtime** (not SDK) for Windows x64
3. Run the installer and follow the setup wizard
4. Restart your computer if prompted

### Step 2: Download BrowserChooser3
1. Download `BrowserChooser3.exe` from the releases page
2. Create a folder for BrowserChooser3 (e.g., `C:\Programs\BrowserChooser3\`)
3. Place the executable in this folder

### Step 3: First Run Setup
1. Double-click `BrowserChooser3.exe` to run it
2. The application will create a configuration file (`BrowserChooser3Config.xml`)
3. Press `O` to open the options dialog
4. Configure your browsers and settings as needed

## 🔧 Installation Methods

### Portable Installation (Recommended)
BrowserChooser3 is designed to be portable and doesn't require installation.

**Advantages:**
- No registry modifications
- Easy to move between computers
- No uninstall process needed
- Can run from USB drives

**Steps:**
1. Create a folder: `C:\Programs\BrowserChooser3\`
2. Download and place `BrowserChooser3.exe` in this folder
3. Run the executable

### System-wide Installation
For system-wide access, you can add BrowserChooser3 to your system PATH.

**Steps:**
1. Place `BrowserChooser3.exe` in a permanent location
2. Add the folder to your system PATH:
   - Open System Properties → Advanced → Environment Variables
   - Add the BrowserChooser3 folder to the PATH variable
3. Restart your command prompt or terminal

### Auto-start Configuration
To make BrowserChooser3 start automatically:

**Method 1: Startup Folder**
1. Press `Win + R`, type `shell:startup`, and press Enter
2. Create a shortcut to `BrowserChooser3.exe` in the startup folder

**Method 2: Task Scheduler**
1. Open Task Scheduler
2. Create a new task
3. Set the action to start `BrowserChooser3.exe`
4. Configure the trigger (e.g., at logon)

## 🧪 Verification

After installation, verify that BrowserChooser3 is working correctly:

### Test 1: Basic Functionality
```bash
# Open Command Prompt and navigate to BrowserChooser3 folder
cd C:\Programs\BrowserChooser3
BrowserChooser3.exe https://www.google.com
```

### Test 2: Options Dialog
1. Run BrowserChooser3
2. Press `O` to open options
3. Verify all tabs are accessible
4. Check that browser detection is working

### Test 3: Transparency Features
1. Open options dialog
2. Go to Display tab
3. Enable transparency
4. Adjust opacity and test rounded corners

## 🚨 Troubleshooting Installation

### Common Issues

**Issue: "Application failed to start"**
- **Solution**: Install .NET 8.0 Runtime
- **Check**: Verify .NET installation with `dotnet --version`

**Issue: "File not found" errors**
- **Solution**: Ensure all files are in the same folder
- **Check**: Verify file permissions

**Issue: BrowserChooser3 doesn't respond**
- **Solution**: Run as administrator
- **Check**: Check Windows Defender or antivirus settings

**Issue: Transparency not working**
- **Solution**: Update graphics drivers
- **Check**: Verify Windows transparency effects are enabled

### Logging for Troubleshooting
If you encounter issues, enable logging:

```bash
BrowserChooser3.exe --log
```

Check the `Logs/` folder for detailed error information.

## 🔄 Updates

### Automatic Updates
BrowserChooser3 can check for updates automatically:
1. Open options dialog (`O` key)
2. Go to "Other" tab
3. Enable "Automatic Updates"

### Manual Updates
1. Download the latest release
2. Replace the old `BrowserChooser3.exe` with the new one
3. Your configuration file will be preserved

## 🗑️ Uninstallation

Since BrowserChooser3 is portable, uninstallation is simple:

1. **Delete the folder** containing BrowserChooser3
2. **Remove shortcuts** you may have created
3. **Remove from PATH** if you added it system-wide
4. **Remove from startup** if configured for auto-start

**Note**: Your configuration file (`BrowserChooser3Config.xml`) will be deleted along with the application folder. If you want to preserve settings, back up this file before deletion.

## 📞 Support

If you encounter installation issues:

1. **Check the FAQ** for common solutions
2. **Search existing issues** on GitHub
3. **Create a new issue** with detailed information:
   - Windows version
   - .NET version
   - Error messages
   - Steps to reproduce

---

*For more detailed information, see the [Configuration Guide](Configuration-Guide) and [Troubleshooting](Troubleshooting) pages.*
