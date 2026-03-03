# Windows Brand Visibility Fix Instructions

## Status
✅ **Mac Version**: Working correctly  
❌ **Windows Version**: Needs to be fixed

## Problem
The Windows version has old/corrupted brand preference data stored from before the latest updates. The Mac version was fixed because you used it after the code changes, but the Windows machine still has the old data.

## Solution: Use the Reset Button

### Step 1: Run the Updated Application on Windows
1. Pull the latest code on your Windows machine
2. Build and run the application

### Step 2: Check the Main Page
When you open the app, you should see at the top of the brand column:
- A label showing "Brands: 0" or "Brands: 2" (the count)
- If it shows "Brands: 0", you'll see a yellow warning box saying "No brands loaded! Go to Settings (⚙) and click Reset button."

### Step 3: Navigate to Settings
1. Click the ⚙ (gear) icon in the top right
2. This will open the Settings page

### Step 4: Reset to Defaults
1. In the Settings page, look for the "Brands" section
2. You'll see an orange **"Reset"** button next to the green **"+"** button
3. Click the orange **"Reset"** button
4. This will:
   - Clear all corrupted preference data
   - Reload default brands (parkrun and Running Stars)
   - Restore all locations for each brand
   - Reset colors to their correct values

### Step 5: Verify It's Working
After clicking Reset, you should see:
1. **On Settings Page**: Brand buttons for "parkrun" and "Running Stars" in the horizontal scrolling list
2. **On Main Page** (navigate back): 
   - Two brand buttons in the left column (parkrun and Running Stars)
   - The "Brands: 2" label at the top
   - Location list populated when you select a brand

## Alternative: Manual Windows Preferences Clear

If the app crashes or the Reset button doesn't work, you can manually clear preferences:

### Option A: Using Registry Editor (Advanced)
1. Close the application completely
2. Open Registry Editor (Win + R, type `regedit`)
3. Navigate to: `HKEY_CURRENT_USER\Software\YourCompany\ProcessEventPics`
   (The exact path depends on your app's package name)
4. Delete the registry key
5. Restart the application

### Option B: Using Developer Command
1. Close the application
2. Open PowerShell as Administrator
3. Run: 
   ```powershell
   Remove-Item "HKCU:\Software\YourCompany\ProcessEventPics" -Recurse -Force
   ```
4. Restart the application

### Option C: App Data Folder
1. Close the application
2. Navigate to: `%LOCALAPPDATA%\Packages\[YourAppPackageName]\LocalState`
3. Delete any preference or settings files
4. Restart the application

## What Gets Restored

After reset, you'll have:

### parkrun Brand
- **Locations**: Auburn Botanic Gardens, Centennial, Chipping Norton, Cooks River, Cronulla, Curl Curl, Dolls Point, Elara Riparian Parklands, Glaston, Greenway, Kamay, Macquarie University, Menai, Mosman, Panania, Parramatta, Pirrama, Rhodes, Rouse Hill, St Peters, The Ponds, Wentworth Common, Whalan Reserve, Wildflower
- **Colors**: Dark Purple (#2b233d) with Apricot (#ffa300) and Aqua Green (#00ceae) accents
- **Default Folders**:
  - EventPhotosFullSize: `E:\EventPhotos\FullSize`
  - EventPhotosCompressed: `E:\EventPhotos\Compressed`
  - RAW: `P:\`

### Running Stars Brand
- **Locations**: Curl Curl, Mona Vale
- **Colors**: Deep Blue (#1e3a8a) with Golden Yellow (#fbbf24) and Light Blue (#60a5fa) accents
- **Default Folders**:
  - EventPhotosFullSize: `E:\EventPhotos\FullSize`
  - EventPhotosCompressed: `E:\EventPhotos\Compressed`
  - RAW: `P:\`

## Debug Information

The app now includes diagnostic logging. If you're still having issues:

1. Run the app from Visual Studio with debugging enabled
2. Check the Output window for debug messages like:
   ```
   MainViewModel: Loaded 2 brands
     - Brand: parkrun, Logo: parkrun_icon.png, Locations: 24
     - Brand: Running Stars, Logo: runningstars_icon.png, Locations: 2
   ```

3. If you see "Loaded 0 brands", the Reset button will fix it

## Why This Happened

The brand model was updated to include new properties:
- Color properties (PrimaryColor, SecondaryColor, etc.)
- FavoriteLocations list
- Better initialization logic

The old JSON data stored in Windows preferences didn't have these new properties, causing deserialization issues. The Mac version works because it was reset when you first ran the updated code.

## Prevention

After this initial reset, the app will handle future updates automatically. The enhanced error handling ensures that:
- Missing properties get default values
- Null lists are initialized
- Corrupted data triggers fallback to defaults
- The Reset button is always available if needed

## Need More Help?

If the brands still don't appear after using the Reset button:
1. Check the "Brands: X" label on the Main Page
2. Look at the Debug output in Visual Studio
3. Try manually clearing preferences using one of the alternative methods above
4. Ensure you've pulled the latest code changes that include all the fixes
