# Adding Montserrat Font to the MAUI App

## Steps to Add Montserrat Font

### 1. Download Montserrat Font
- Visit [Google Fonts - Montserrat](https://fonts.google.com/specimen/Montserrat)
- Click "Download family"
- Extract the ZIP file

### 2. Copy Font Files
Copy these font files to `ProcessEventPics.Maui/Resources/Fonts/`:
- `Montserrat-Regular.ttf`
- `Montserrat-Bold.ttf`
- `Montserrat-SemiBold.ttf` (optional)
- `Montserrat-Medium.ttf` (optional)

### 3. Font Already Registered
The MauiProgram.cs has already been updated to register Montserrat fonts.

### 4. Using in XAML
```xml
<Label Text="Hello parkrun" FontFamily="MontserratRegular" />
<Label Text="Bold Text" FontFamily="MontserratBold" />
```

### 5. Using in Styles
The Styles.xaml will automatically use Montserrat as the default font family once the font files are added.

## Current Status
⚠️ **Font files not yet added** - Application will use default system fonts until Montserrat TTF files are copied to Resources/Fonts/.

Once you add the Montserrat font files, the app will automatically use them throughout the interface!
