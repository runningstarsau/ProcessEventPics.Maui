# Process Running Pics - MAUI Application

## Overview
Cross-platform desktop application for processing parkrun and Running Stars event photos.

## Features
- **Multi-platform support**: Windows, macOS, and Linux
- **Automatic camera detection**: Finds DCIM folder on removable drives
- **Smart defaults**: Saturday → parkrun, Sunday → Running Stars
- **Persistent settings**: Saves all folder locations and event lists
- **Image processing**:
  - Copy original JPGs to organized folders
  - Copy RAW (.NEF) files separately
  - Resize and compress JPGs for Facebook (2048px max, ~500KB)
- **Event management**: Add/delete events from lists (persistent)
- **One-off events**: Process without adding to permanent list

## Folder Structure

### JPG Files (E:\EventPhotos\FullSize or custom):
```
E:\EventPhotos\FullSize\
├── parkrun\
│   ├── Auburn Botanic Gardens\
│   │   └── 20260301\
│   │       ├── Auburn_Botanic_Gardens_parkrun_2026_03_01_0001.jpg
│   │       └── ...
│   └── ...
└── RunningStars\
    ├── Mona Vale\
    │   └── 20260302\
    │       ├── Running_Stars_Mona_Vale_2026_03_02_0001.jpg
    │       └── ...
    └── ...
```

### RAW Files (P:\ or custom):
```
P:\
├── parkrun\
│   ├── Auburn Botanic Gardens\
│   │   └── 20260301\
│   │       ├── Auburn_Botanic_Gardens_parkrun_2026_03_01_0001.nef
│   │       └── ...
│   └── ...
└── Running Stars\
    ├── Mona Vale\
    │   └── 20260302\
    │       ├── Running_Stars_Mona_Vale_2026_03_02_0001.nef
    │       └── ...
    └── ...
```

### EventPhotosCompressed Files (E:\EventPhotos\Compressed or custom):
```
E:\EventPhotos\Compressed\
├── parkrun\
│   ├── Auburn Botanic Gardens\
│   │   └── 20260301\
│   │       ├── Auburn_Botanic_Gardens_parkrun_2026_03_01_0001.jpg (compressed)
│   │       └── ...
│   └── ...
└── RunningStars\
    └── ...
```

## Usage

1. **Connect Camera**: Plug in your Nikon D850 or copy files to source folder
2. **Click "Find Camera"**: Auto-detects DCIM folder on removable drives
3. **Select Event Type**: Click parkrun or Running Stars icon
4. **Choose Event/Location**: Select from list or enter custom name
5. **Configure Options**:
   - ☑ Save to EventPhotosFullSize folder
   - ☑ Convert and compress for Facebook
6. **Click "Process Images"**: Application will:
   - Copy JPGs to EventPhotosFullSize
   - Copy RAWs to RAW folder
   - Resize/compress JPGs to EventPhotosCompressed folder
   - Open the EventPhotosCompressed folder

## Custom Icons

Replace these placeholder icons with your own:
- `Resources/Images/parkrun_icon.png`
- `Resources/Images/runningstars_icon.png`

## Building

### Windows
```bash
dotnet build -f net10.0-windows10.0.19041.0
```

### macOS
```bash
dotnet build -f net10.0-maccatalyst
```

### Linux (requires Avalonia backend)
Note: Standard MAUI doesn't support Linux desktop. Consider using Avalonia UI for Linux support.

## Original Console Application

The original console application (`ProcessEventPics.csproj`) is still available in the solution and can be run independently.
