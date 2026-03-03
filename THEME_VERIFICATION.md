# Theme Verification Guide

## What Changed

### 1. Brand Model (Brand.cs)
- **Added `INotifyPropertyChanged`** to the Brand class
- All color properties now trigger UI updates when changed
- This enables real-time theme updates when editing colors

### 2. MainPage Theme Bindings
Fixed incorrect bindings that prevented theme from showing:
- Header title `TextColor` now uses `SelectedBrand.TextColor` (was using BackgroundColor)
- Settings button `TextColor` now uses `SelectedBrand.TextColor`
- Brand card labels use `TextColor` property

### 3. SettingsPage Color Editor
- Grid layout corrected to **3 rows × 6 columns**:
  - **Row 0**: Color labels (Primary, Secondary, Tertiary, Background, Foreground, Text)
  - **Row 1**: Entry fields for hex color values
  - **Row 2**: BoxView color preview swatches (40px height)
- Live color preview updates as you type
- "Save Changes" button for immediate save
- "Reset to Defaults" button to restore original brand colors

## How to Verify Theme Application

### Visual Changes When Selecting Brands

When you click on a brand card, you should immediately see:

#### parkrun Brand (#2b233d, #ffa300, #00ceae)
1. **Header Background**: Dark purple (#2b233d)
2. **Header Text**: White (#FFFFFF)
3. **Location Label**: Dark purple color (#2b233d)
4. **Add Location Button**: Apricot/orange (#ffa300)
5. **Search Box Border**: Aqua green (#00ceae)
6. **Location List Border**: Dark purple (#2b233d)

#### Running Stars Brand (#1e3a8a, #fbbf24, #60a5fa)
1. **Header Background**: Deep blue (#1e3a8a)
2. **Header Text**: Black (#000000)
3. **Location Label**: Deep blue color (#1e3a8a)
4. **Add Location Button**: Golden yellow (#fbbf24)
5. **Search Box Border**: Light blue (#60a5fa)
6. **Location List Border**: Deep blue (#1e3a8a)

### Testing Steps

1. **Launch the application**
2. **Click on the parkrun brand card** (left column)
   - Header should turn dark purple
   - Text should be white
   - Buttons should be apricot/orange
3. **Click on the Running Stars brand card**
   - Header should turn deep blue
   - Text should turn black
   - Buttons should turn golden yellow
4. **Open Settings** (⚙ button in header)
5. **Select a brand** to configure
6. **Scroll to "Theme Colors" section**
7. **Edit a color** (e.g., change Primary from #2b233d to #ff0000)
8. **Watch the preview BoxView** update in real-time
9. **Click "Save Changes"**
10. **Go back to Main Page**
11. **Verify the header** shows your new color

## Color Property Mapping

| Property | Used For | parkrun | Running Stars |
|----------|----------|---------|---------------|
| **Primary** | Header background, borders, main branding | #2b233d (Dark Purple) | #1e3a8a (Deep Blue) |
| **Secondary** | Action buttons, accents | #ffa300 (Apricot) | #fbbf24 (Golden Yellow) |
| **Tertiary** | Search borders, highlights | #00ceae (Aqua Green) | #60a5fa (Light Blue) |
| **Background** | Brand card background | #2b233d (Dark Purple) | #FFFFFF (White) |
| **Foreground** | Text on backgrounds | #FFFFFF (White) | #1e3a8a (Deep Blue) |
| **Text** | Text on primary colored elements | #FFFFFF (White) | #000000 (Black) |

## Troubleshooting

### If theme still doesn't change:
1. **Stop the application completely** (if running in debug)
2. **Restart the application** - The `INotifyPropertyChanged` change requires a fresh start
3. **Try clicking between brands** multiple times
4. **Check the header background** - this is the most obvious visual indicator

### If colors don't update while editing:
1. Colors update in real-time in the preview boxes
2. Click "Save Changes" to persist to settings
3. Navigate back to Main Page to see changes applied
4. Changes are also auto-saved when you leave the Settings page

## Notes

- The .NET 10 preview may have XAML compiler warnings - these don't affect runtime functionality
- All color changes are persisted using the Preferences API
- Brand selection triggers immediate theme updates via data binding
- Color values must be valid hex colors (e.g., #RGB, #RRGGBB, #AARRGGBB)
