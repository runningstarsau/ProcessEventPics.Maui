# Brand Visibility Fix

## Problem
After pulling changes from the repository, brand buttons were not visible on the Settings page and Main page on Mac. However, attempting to create duplicate brands (parkrun, Running Stars) showed they still existed in the system.

## Root Cause
The issue was caused by incomplete or corrupted brand data stored in the application's preferences (local storage). When the Brand model was updated with new color properties and the FavoriteLocations feature, the previously stored JSON data didn't match the new structure. This caused:

1. **Null Lists**: The `Locations` and `FavoriteLocations` lists were not initialized properly during deserialization
2. **Missing Color Properties**: Brands deserialized from old JSON didn't have values for the new color properties
3. **Rendering Issues**: Missing or invalid data caused the UI to fail to render the brand buttons

## Fixes Applied

### 1. Brand Model Constructor Improvements
**File**: `ProcessEventPics.Maui/Models/Brand.cs`

- Added initialization of `FavoriteLocations` list in both constructors to prevent null reference exceptions
- Parameterless constructor now initializes both `Locations` and `FavoriteLocations` lists

### 2. Robust Deserialization in SettingsService
**File**: `ProcessEventPics.Maui/Services/SettingsService.cs`

- Added try-catch around brand deserialization to handle corrupted data gracefully
- Added validation after deserialization to ensure:
  - `Locations` and `FavoriteLocations` lists are never null
  - All color properties have valid default values
- Falls back to default brands if deserialization fails completely

### 3. Reset to Defaults Feature
**Files**: 
- `ProcessEventPics.Maui/ViewModels/MainViewModel.cs`
- `ProcessEventPics.Maui/SettingsPage.xaml`

Added a new "Reset" button in the Settings page that allows you to:
- Clear all stored preferences
- Reload default brands (parkrun and Running Stars with correct settings)
- Reset to a clean state if data becomes corrupted

## How to Use the Fix

### Option 1: Use the Reset Button (Recommended)
1. Open the application
2. Navigate to Settings (⚙ gear icon)
3. In the "Brands" section, click the orange "Reset" button
4. Your brands will be restored to their default state with correct data

### Option 2: Manual Preferences Clear (Alternative)
If the app crashes or the UI is completely broken:

**On Mac:**
1. Close the application completely
2. Open Terminal
3. Run: `defaults delete com.runningstars.processeventpics` (adjust bundle ID as needed)
4. Restart the application

**Alternative on Mac:**
1. Close the application
2. Delete the preferences file at:
   - `~/Library/Preferences/com.runningstars.processeventpics.plist`
3. Restart the application

## What Data Gets Reset
When you use the Reset button, the following will be restored to defaults:

- **parkrun brand**:
  - Locations: Auburn Botanic Gardens, Centennial, Chipping Norton, Cooks River, Cronulla, Curl Curl, and 18 other locations
  - Colors: Official parkrun dark purple (#2b233d) with apricot accents
  - Logo: parkrun_icon.png

- **Running Stars brand**:
  - Locations: Curl Curl, Mona Vale
  - Colors: Deep blue (#1e3a8a) with golden yellow star accents
  - Logo: runningstars_icon.png

## Prevention
To avoid this issue in the future:
- Always pull and sync changes regularly
- If you notice visual glitches after updating, use the Reset button
- The application now handles data migration automatically, so future updates should work smoothly

## Technical Details
The fixes ensure backward compatibility by:
1. Validating deserialized data and filling in missing properties
2. Initializing all required collections to prevent null references
3. Providing fallback default values for all properties
4. Offering a manual reset option when automated recovery fails

## Testing the Fix
After applying these changes:
1. Build and run the application
2. You should see brand buttons in both Main page and Settings page
3. Each brand button should show:
   - Brand name label
   - Colored background (dark purple for parkrun, white for Running Stars)
   - Border that highlights when selected
4. Clicking a brand should select it and show its locations
