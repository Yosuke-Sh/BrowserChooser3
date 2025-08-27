# Accessibility Features

BrowserChooser3 is designed with accessibility in mind, providing comprehensive support for users with different abilities and preferences.

## ♿ Overview

The accessibility features in BrowserChooser3 include:
- **Visual Focus Indicators**: Clear focus highlighting for keyboard navigation
- **Keyboard Navigation**: Full keyboard support for all functions
- **High Contrast Support**: Enhanced visibility options
- **Customizable Focus Appearance**: Adjustable focus box colors and sizes
- **Screen Reader Compatibility**: Proper labeling and structure

## 🎯 Visual Focus System

### Focus Indicators
BrowserChooser3 provides multiple types of focus indicators:

#### Standard Focus
- **Default**: System-standard focus rectangle
- **Usage**: Standard Windows focus indication
- **Accessibility**: Works with screen readers

#### Visual Focus Box
- **Customizable**: Color, width, and style options
- **Enhanced**: More prominent than standard focus
- **Configurable**: Adjustable through settings

### Configuration Options

#### Show Focus
- **Setting**: Enable/disable focus indicators
- **Default**: Enabled
- **Location**: Options → Display → Accessibility Settings

#### Focus Box Color
- **Setting**: Custom color for focus indicators
- **Default**: Transparent (uses system default)
- **Range**: Any RGB color
- **Usage**: Choose high-contrast colors for better visibility

#### Focus Box Width
- **Setting**: Thickness of focus indicators
- **Default**: 2 pixels
- **Range**: 1-10 pixels
- **Usage**: Thicker lines for better visibility

#### Visual Focus
- **Setting**: Enhanced visual focus display
- **Default**: Disabled
- **Effect**: More prominent focus indicators
- **Usage**: For users who need stronger visual cues

## ⌨️ Keyboard Navigation

### Navigation Keys
BrowserChooser3 supports full keyboard navigation:

#### Tab Navigation
- **Tab**: Move to next control
- **Shift+Tab**: Move to previous control
- **Arrow Keys**: Navigate within control groups
- **Enter/Space**: Activate selected control

#### Browser Selection
- **Arrow Keys**: Navigate between browser buttons
- **Enter**: Select highlighted browser
- **Escape**: Cancel selection
- **Number Keys**: Quick selection (if configured)

#### Options Dialog
- **O Key**: Open options dialog
- **Tab/Shift+Tab**: Navigate between tabs and controls
- **Enter**: Activate buttons and checkboxes
- **Escape**: Close dialog

### Shortcuts
- **O**: Open options dialog
- **Escape**: Cancel/close dialogs
- **Enter**: Confirm selections
- **F1**: Help (if implemented)

## 🎨 High Contrast Support

### Windows High Contrast Mode
BrowserChooser3 automatically adapts to Windows High Contrast Mode:

#### Automatic Detection
- **Detection**: Monitors system high contrast settings
- **Adaptation**: Adjusts colors and contrast automatically
- **Persistence**: Maintains accessibility across sessions

#### Color Adjustments
- **Background**: Adapts to high contrast background
- **Text**: Uses high contrast text colors
- **Focus**: Enhanced focus indicators
- **Borders**: High contrast border colors

### Custom High Contrast
You can also create custom high contrast settings:

#### Focus Colors
- **High Contrast**: Use bright, contrasting colors
- **Examples**: Yellow on black, white on dark blue
- **Testing**: Verify with different backgrounds

#### Background Colors
- **Solid Colors**: Avoid transparency in high contrast mode
- **Contrasting**: Ensure sufficient contrast with text
- **Consistent**: Use consistent colors throughout

## 🔧 Accessibility Settings

### Accessing Settings
1. Launch BrowserChooser3
2. Press `O` to open Options
3. Go to **Display** tab
4. Click **Accessibility Settings** button

### Settings Dialog

#### Focus Display Options
- **Show Focus**: Enable/disable focus indicators
- **Focus Box Color**: Choose custom focus color
- **Focus Box Width**: Set focus line thickness
- **Visual Focus**: Enable enhanced focus display

#### Advanced Options
- **Accessible Rendering**: Use accessible rendering mode
- **High Contrast Override**: Force high contrast mode
- **Focus Timeout**: Set focus display duration

## 🧪 Testing Accessibility

### Manual Testing
Test accessibility features manually:

#### Keyboard Navigation
1. **Start BrowserChooser3**
2. **Use only keyboard** to navigate
3. **Verify all functions** are accessible
4. **Test with different** focus settings

#### Visual Focus
1. **Enable visual focus**
2. **Navigate with keyboard**
3. **Verify focus indicators** are visible
4. **Test with different** color schemes

#### High Contrast
1. **Enable Windows High Contrast**
2. **Launch BrowserChooser3**
3. **Verify readability** of all elements
4. **Test navigation** in high contrast

### Automated Testing
Use accessibility testing tools:

#### Screen Readers
- **NVDA**: Free screen reader for Windows
- **JAWS**: Commercial screen reader
- **Windows Narrator**: Built-in Windows screen reader

#### Testing Tools
- **Accessibility Insights**: Microsoft's accessibility testing tool
- **axe DevTools**: Browser extension for accessibility testing
- **WAVE**: Web accessibility evaluation tool

## 🎯 Best Practices

### For Users
- **Enable focus indicators** if you use keyboard navigation
- **Use high contrast colors** for better visibility
- **Test with screen readers** if you rely on them
- **Report accessibility issues** to the development team

### For Developers
- **Follow WCAG guidelines** for accessibility
- **Test with real users** who have disabilities
- **Provide multiple ways** to access functions
- **Ensure keyboard navigation** works throughout

## 🚨 Troubleshooting

### Common Issues

#### Focus Not Visible
**Symptoms**: Can't see focus indicators
**Solutions**:
1. Enable "Show Focus" in settings
2. Increase focus box width
3. Choose high-contrast focus color
4. Enable visual focus mode

#### Keyboard Navigation Not Working
**Symptoms**: Can't navigate with keyboard
**Solutions**:
1. Check if focus is on the application
2. Try Alt+Tab to focus the window
3. Verify keyboard shortcuts are not conflicting
4. Test with different keyboard layouts

#### High Contrast Issues
**Symptoms**: Poor visibility in high contrast mode
**Solutions**:
1. Disable transparency in high contrast
2. Use solid background colors
3. Increase focus indicator thickness
4. Choose contrasting colors

#### Screen Reader Problems
**Symptoms**: Screen reader doesn't announce elements
**Solutions**:
1. Ensure proper labeling of controls
2. Use standard Windows controls
3. Provide alternative text for images
4. Test with multiple screen readers

### Advanced Troubleshooting

#### Enable Debug Logging
```bash
BrowserChooser3.exe --log
```

Check logs for accessibility-related issues:
- Focus indicator rendering
- Keyboard event handling
- High contrast detection

#### Registry Settings
Check Windows accessibility settings:
```
HKEY_CURRENT_USER\Control Panel\Accessibility
```

## 📚 Standards and Guidelines

### WCAG Compliance
BrowserChooser3 aims to comply with Web Content Accessibility Guidelines:

#### Level A Compliance
- **Keyboard Accessible**: All functionality available via keyboard
- **No Keyboard Traps**: Users can navigate away from any control
- **Focus Visible**: Focus indicators are clearly visible

#### Level AA Compliance
- **Contrast Ratio**: Sufficient color contrast (4.5:1 minimum)
- **Resize Text**: Text can be resized up to 200%
- **Multiple Ways**: Multiple ways to access functionality

### Windows Accessibility Guidelines
- **MSAA Support**: Microsoft Active Accessibility
- **UIA Support**: UI Automation
- **High Contrast**: Windows High Contrast Mode support

## 🔄 Future Improvements

### Planned Features
- **Voice Navigation**: Voice command support
- **Gesture Support**: Touch and gesture navigation
- **Custom Themes**: Accessibility-focused themes
- **Advanced Screen Reader**: Enhanced screen reader support

### Community Contributions
We welcome contributions to improve accessibility:
- **User Testing**: Test with real users who have disabilities
- **Feature Requests**: Suggest new accessibility features
- **Bug Reports**: Report accessibility issues
- **Code Contributions**: Implement accessibility improvements

## 📞 Support

### Getting Help
If you encounter accessibility issues:

1. **Check this guide** for common solutions
2. **Search existing issues** on GitHub
3. **Create a new issue** with detailed information:
   - Your accessibility needs
   - Steps to reproduce the issue
   - Screen reader or assistive technology used
   - Windows version and accessibility settings

### Community Resources
- **GitHub Issues**: Report accessibility bugs
- **GitHub Discussions**: Ask accessibility questions
- **Wiki**: Contribute to accessibility documentation

---

*For more information about BrowserChooser3's features, see the [Configuration Guide](Configuration-Guide) and [User Guide](User-Guide).*
