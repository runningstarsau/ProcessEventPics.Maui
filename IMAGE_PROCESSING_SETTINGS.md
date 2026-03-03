# Image Processing Settings

## New Configuration Options

Two new global configuration options have been added to control image processing:

### 1. Image Resize Size (pixels)
- **Default**: 2048 pixels
- **Purpose**: Controls the maximum dimension (width or height) when resizing images
- **Usage**: Facebook's recommended size for optimal image quality
- **Location**: Settings > Global Configuration
- **Data Type**: Integer (numeric input)

### 2. JPG Compression (%)
- **Default**: 85%
- **Purpose**: Controls the JPEG compression quality level
- **Range**: 0-100 (higher = better quality, larger file size)
- **Usage**: Balances image quality with file size for Facebook eventPhotosCompresseds
- **Location**: Settings > Global Configuration
- **Data Type**: Integer (numeric input)

## Implementation Details

### SettingsService
Added two new properties to persist settings:
```csharp
public int ImageResizeSize { get; set; } // Default: 2048
public int JpgCompression { get; set; }  // Default: 85
```

### MainViewModel
Exposed properties with automatic save to settings:
```csharp
public int ImageResizeSize { get; set; }
public int JpgCompression { get; set; }
```

### Settings Page (SettingsPage.xaml)
Added input fields under "Processing Options":
- Image Resize Size (pixels): Numeric Entry field
- JPG Compression (%): Numeric Entry field

### Main Page Display (MainPage.xaml)
Settings are displayed under the Sample Filename section:
- Shows current resize dimension: "2048 pixels"
- Shows current compression level: "85%"
- Styled to match the selected brand's theme colors
- Updated in real-time when settings change

## UI Layout

### Settings Page
```
Global Configuration
├── Source Folder: [Entry] [Browse] [Find Camera]
│
Processing Options
├── ☑ Save to EventPhotosFullSize folder
├── ☑ Convert and compress for Facebook
├── Image Resize Size (pixels): [2048]
└── JPG Compression (%): [85]
```

### Main Page - Sample Filename Section
```
Sample Filename:
parkrun_TestLocation_2024_01_15_0001.jpg

────────────────────────
Resize:        2048 pixels
Compression:          85%
```

## Usage

1. **Changing Settings**:
   - Open Settings (⚙ button in header)
   - Scroll to "Processing Options"
   - Update Image Resize Size or JPG Compression
   - Values are saved automatically

2. **Viewing Current Settings**:
   - Check the MainPage
   - Look under "Sample Filename" section
   - Current values are always displayed

## Best Practices

### Image Resize Size
- **2048 pixels**: Standard for Facebook (recommended)
- **1920 pixels**: Good for general web use
- **3840 pixels**: For high-resolution displays (4K)
- Larger values = better quality but slower processing

### JPG Compression
- **85%**: Facebook recommended (good balance)
- **90-95%**: Higher quality, larger files
- **70-80%**: Smaller files, acceptable quality
- **60-70%**: Small files, visible quality loss

## Technical Notes

- Settings persist between application sessions
- Changes take effect immediately for new processing operations
- Existing processed images are not affected by setting changes
- Both values must be positive integers
- Keyboard type is set to "Numeric" for easier input
