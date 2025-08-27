# Transparency Settings

BrowserChooser3 features advanced transparency and visual customization options that allow you to create a modern, personalized interface.

## 🎨 Overview

The transparency system in BrowserChooser3 provides:
- **Window Transparency**: Adjustable opacity for the entire window
- **Transparency Key**: Color-based transparency for specific areas
- **Rounded Corners**: Modern rounded corner effects
- **Title Bar Control**: Option to hide the title bar
- **Background Customization**: Custom background colors

## ⚙️ Configuration

### Accessing Transparency Settings
1. Launch BrowserChooser3
2. Press `O` to open the Options dialog
3. Navigate to the **Display** tab
4. Scroll down to the **Transparency Settings** section

### Setting Options

#### Enable Transparency
- **Checkbox**: Enable/disable transparency effects
- **Default**: Enabled
- **Effect**: When enabled, applies transparency settings to the main window

#### Transparency Color
- **Control**: Color picker (PictureBox)
- **Default**: Magenta (`#FF00FF`)
- **Usage**: This color becomes transparent in the window
- **Tip**: Choose a color that doesn't appear in your content

#### Opacity
- **Control**: NumericUpDown (0.01 - 1.00)
- **Default**: 0.9 (90% opacity)
- **Range**: 0.01 (nearly transparent) to 1.00 (fully opaque)
- **Precision**: Two decimal places

#### Hide Title Bar
- **Checkbox**: Show/hide the window title bar
- **Default**: Enabled (hidden)
- **Effect**: Removes the title bar for a cleaner look
- **Note**: When hidden, you can still resize the window by dragging edges

#### Rounded Corners Radius
- **Control**: NumericUpDown (0 - 50)
- **Default**: 0 (disabled)
- **Range**: 0 (square corners) to 50 (very rounded)
- **Effect**: Creates rounded corners on the window

## 🎯 Usage Examples

### Modern Glass Effect
```
Enable Transparency: ✓
Transparency Color: Magenta
Opacity: 0.8
Hide Title Bar: ✓
Rounded Corners Radius: 15
```

### Subtle Transparency
```
Enable Transparency: ✓
Transparency Color: Magenta
Opacity: 0.95
Hide Title Bar: ✗
Rounded Corners Radius: 5
```

### No Transparency (Classic Look)
```
Enable Transparency: ✗
Hide Title Bar: ✗
Rounded Corners Radius: 0
```

## 🔧 Technical Details

### How Transparency Works

#### Window Transparency
- Uses Windows API `SetLayeredWindowAttributes`
- Applies to the entire window
- Affects all content including controls

#### Color Key Transparency
- Uses `TransparencyKey` property
- Makes specific colors completely transparent
- Useful for creating "cut-out" effects

#### Rounded Corners
- Uses `GraphicsPath` and `Region` classes
- Creates smooth rounded rectangles
- Automatically adjusts on window resize

### Performance Considerations

#### Hardware Acceleration
- Transparency effects use hardware acceleration when available
- Performance may vary based on graphics card
- Older systems may experience reduced performance

#### Memory Usage
- Transparency effects require additional memory
- Rounded corners use more resources than square corners
- Consider disabling on low-memory systems

## 🎨 Visual Effects

### Transparency Combinations

#### High Transparency (0.1 - 0.3)
- **Use Case**: Subtle overlay effects
- **Best For**: Background applications
- **Considerations**: May be hard to read

#### Medium Transparency (0.4 - 0.7)
- **Use Case**: Balanced visibility and transparency
- **Best For**: General use
- **Considerations**: Good readability with transparency

#### Low Transparency (0.8 - 0.95)
- **Use Case**: Subtle transparency effect
- **Best For**: Professional appearance
- **Considerations**: Minimal transparency impact

### Rounded Corner Effects

#### Small Radius (1 - 10)
- **Appearance**: Subtle rounding
- **Use Case**: Modern but conservative look
- **Performance**: Minimal impact

#### Medium Radius (11 - 25)
- **Appearance**: Noticeable rounding
- **Use Case**: Modern, friendly appearance
- **Performance**: Moderate impact

#### Large Radius (26 - 50)
- **Appearance**: Very rounded, pill-like
- **Use Case**: Playful, modern design
- **Performance**: Higher impact

## 🚨 Troubleshooting

### Common Issues

#### Transparency Not Working
**Symptoms**: Window appears opaque despite settings
**Solutions**:
1. Check Windows transparency effects are enabled
2. Update graphics drivers
3. Verify .NET 8.0 is installed
4. Try different transparency colors

#### Performance Issues
**Symptoms**: Slow rendering or high CPU usage
**Solutions**:
1. Reduce opacity value
2. Decrease rounded corner radius
3. Disable transparency temporarily
4. Check for graphics driver updates

#### Rounded Corners Not Smooth
**Symptoms**: Jagged or pixelated corners
**Solutions**:
1. Enable hardware acceleration
2. Update graphics drivers
3. Reduce corner radius
4. Check Windows DPI settings

#### Title Bar Issues
**Symptoms**: Can't resize or move window
**Solutions**:
1. Enable title bar temporarily
2. Use window edges for resizing
3. Check window state settings

### Advanced Troubleshooting

#### Enable Debug Logging
```bash
BrowserChooser3.exe --log
```

Check the log file for transparency-related errors:
- `MainForm.ApplyTransparencySettings`
- `MainForm.ApplyRoundedCorners`
- `CreateRoundedRectangleRegion`

#### Registry Settings
If transparency still doesn't work, check Windows registry:
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM
EnableAeroPeek = 1
```

## 💡 Tips and Best Practices

### Color Selection
- **Avoid common colors**: Don't use colors that appear in your content
- **Test thoroughly**: Try different transparency colors
- **Consider contrast**: Ensure text remains readable

### Performance Optimization
- **Start conservative**: Begin with higher opacity values
- **Test on target systems**: Verify performance on slower machines
- **Monitor resources**: Watch CPU and memory usage

### Accessibility
- **Maintain readability**: Ensure sufficient contrast
- **Test with users**: Get feedback from actual users
- **Provide alternatives**: Allow disabling transparency

### Design Guidelines
- **Consistency**: Use similar settings across applications
- **Purpose**: Transparency should enhance, not hinder usability
- **Context**: Consider the desktop environment

## 🔄 Migration from Browser Chooser 2

BrowserChooser3's transparency system is completely new and doesn't migrate settings from Browser Chooser 2. You'll need to:

1. **Configure manually**: Set up transparency settings in BrowserChooser3
2. **Test thoroughly**: Verify the new system works as expected
3. **Adjust as needed**: Fine-tune settings for your preferences

## 📚 Related Topics

- [Configuration Guide](Configuration-Guide)
- [Accessibility Features](Accessibility-Features)
- [Troubleshooting](Troubleshooting)
- [Customization](Customization)

---

*For more information about other display settings, see the [Configuration Guide](Configuration-Guide).*
