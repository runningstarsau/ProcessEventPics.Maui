# Brand Theme Configuration

## parkrun Brand Colors

The app now uses the official parkrun color palette:

| Color Name | Hex | RGB | Usage |
|------------|-----|-----|-------|
| **Dark Purple** | `#2b233d` | rgb(43, 35, 61) | Primary color, backgrounds, header |
| **Apricot** | `#ffa300` | rgb(255, 163, 0) | Secondary color, buttons, accents |
| **Aqua Green** | `#00ceae` | rgb(0, 206, 174) | Tertiary color, highlights |
| **Other Dark Purple** | `#47173F` | rgb(71, 23, 63) | Alternative purple |
| **Pink** | `#F41C22` | rgb(244, 28, 34) | Error/Alert color |
| **Greys** | `#2A3436` | rgb(42, 52, 54) | Tonal elements |
| **Text** | White/Black | - | Dynamic based on background |

### parkrun Theme Application
- **Header**: Dark Purple (#2b233d) background with White text
- **Buttons**: Apricot (#ffa300) with appropriate contrast text
- **Highlights**: Aqua Green (#00ceae)
- **Selected items**: Border and background using brand colors

## Running Stars Brand Colors

Based on runningstars.org.au website:

| Color Name | Hex | RGB | Usage |
|------------|-----|-----|-------|
| **Deep Blue** | `#1e3a8a` | rgb(30, 58, 138) | Primary color, text |
| **Golden Yellow** | `#fbbf24` | rgb(251, 191, 36) | Secondary color, star accents |
| **Light Blue** | `#60a5fa` | rgb(96, 165, 250) | Tertiary color, highlights |
| **White** | `#FFFFFF` | - | Background |
| **Text** | Black/Deep Blue | - | Main text colors |

### Running Stars Theme Application
- **Header**: Deep Blue (#1e3a8a) background with White text
- **Buttons**: Golden Yellow (#fbbf24) with contrasting text
- **Highlights**: Light Blue (#60a5fa)
- **Background**: Clean white for readability

## Typography

**Font Family**: Montserrat (Google Fonts)

### Font Weights Used:
- **Regular** (400): Body text, entries, labels
- **SemiBold** (600): Subheadings, emphasis
- **Bold** (700): Headers, buttons, important text

### To Add Montserrat Font:
1. Download from [Google Fonts - Montserrat](https://fonts.google.com/specimen/Montserrat)
2. Copy these files to `ProcessEventPics.Maui/Resources/Fonts/`:
   - `Montserrat-Regular.ttf`
   - `Montserrat-Bold.ttf`
   - `Montserrat-SemiBold.ttf`
3. Font registration already configured in `MauiProgram.cs`

**Note**: Until Montserrat fonts are added, the app will fallback to OpenSans.

## Dynamic Theming

The app dynamically applies the selected brand's theme throughout:
- Header background changes to brand color
- All buttons use brand secondary color
- Text colors adjust for contrast (white on dark, black on light)
- Highlights and accents use tertiary colors
- Selected items show brand-colored borders

## Brand Selection

Default brand selection by day:
- **Saturday** → parkrun
- **Sunday** → Running Stars
- **Other days** → parkrun (can be changed manually)

Users can override the default by clicking the brand icon at any time.
