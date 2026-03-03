# Favorite Locations Feature

## Overview
The Favorite Locations feature allows users to mark frequently-used event locations as favorites. Favorited locations appear at the top of the locations list for quick access.

## Features

### Star Icon Toggle
- **Location**: Next to each location name in the locations list
- **Appearance**: 
  - ⭐ **Filled star** (colored with brand's Secondary Color) when location is a favorite
  - ☆ **Outline star** (greyed out) when location is not a favorite
- **Functionality**: Click/tap the star to toggle favorite status

### Automatic Sorting
- **Favorites First**: All favorited locations appear at the top of the list
- **Alphabetical Order**: 
  - Favorites are sorted alphabetically among themselves
  - Non-favorites are sorted alphabetically among themselves
  - This sorting is maintained even with search filtering

### Search Integration
- When searching locations, the favorite/non-favorite grouping and ordering is preserved
- Favorites matching the search appear first, followed by non-favorites matching the search

## Implementation Details

### Data Storage
- Each brand maintains its own list of favorite locations
- Favorites are stored in the `FavoriteLocations` property of the `Brand` model
- The favorites list is persisted automatically with the brand settings

### User Interface
- The star button is positioned between the location name and the delete button
- The star color matches the brand's Secondary Color theme when active
- Clicking the star immediately toggles the favorite status and re-sorts the list

### Technical Components

#### Brand Model (`Brand.cs`)
```csharp
public List<string> FavoriteLocations { get; set; } = new();
```

#### Main ViewModel (`MainViewModel.cs`)
- `ToggleFavoriteCommand`: Adds or removes a location from favorites
- `FilteredLocations`: Modified to sort favorites first, then non-favorites

#### UI Converter (`IsFavoriteConverter.cs` & `FavoriteStarTextConverter.cs`)
- **IsFavoriteConverter**: Determines the star color based on whether the location is in the favorites list
  - Returns brand's SecondaryColor for favorites, LightGray for non-favorites
- **FavoriteStarTextConverter**: Determines which star character to display
  - Returns "⭐" (filled star) for favorites
  - Returns "☆" (outline star) for non-favorites

#### XAML (`MainPage.xaml`)
- Star button with MultiBinding to location and brand
- Uses `FavoriteStarTextConverter` to determine star character (filled vs outline)
- Uses `IsFavoriteConverter` to determine color (brand color vs grey)
- Executes `ToggleFavoriteCommand` on click

## Usage

1. **To Mark as Favorite**: Click the star icon next to any location name
2. **To Unmark as Favorite**: Click the star icon again (it will grey out)
3. **Result**: Favorited locations automatically move to the top of the list in alphabetical order

## Benefits

- **Quick Access**: Frequently-used locations are always at the top
- **Reduced Scrolling**: No need to scroll through long lists to find common locations
- **Visual Indicator**: Clear visual distinction between favorites and regular locations
- **Per-Brand**: Each brand (parkrun, Running Stars, etc.) maintains its own favorites
- **Persistent**: Favorites are saved and restored when the app restarts

## Brand-Specific Behavior

The star icon color adapts to each brand's theme:
- **parkrun**: Apricot (#ffa300) when active
- **Running Stars**: Golden Yellow (#fbbf24) when active
- **Custom Brands**: Uses the brand's Secondary Color when active

## Integration with Existing Features

- **Delete Location**: Deleting a location also removes it from favorites
- **Search**: Favorites remain at the top even when filtering by search text
- **Brand Selection**: Switching brands shows that brand's specific favorites
- **New Locations**: New locations are not favorited by default
