# About Page Implementation

## Overview
Replaced the simple dialog alert with a full custom AboutPage that displays Running Stars company information with professional formatting and interactive elements.

## Files Created

### 1. ProcessEventPics.Maui\AboutPage.xaml
A professionally designed XAML page featuring:
- **Themed Header**: Uses the current event's primary and secondary colors
- **Running Stars Logo**: Displays the runningstars_icon.png in a styled border
- **App Title**: "Process Event Pics" with theme-based coloring
- **Description Section**: Formatted text explaining the app's purpose
- **Contact Information Section**: Two interactive elements:
  - **Website**: Globe icon (🌐) with clickable link to https://runningstars.org.au
  - **Email**: Email icon (✉️) with clickable mailto link to info@runningstars.org.au
- **Footer**: Copyright information

#### Key Features:
- Responsive design with centered content (max-width: 800)
- ScrollView for longer content on smaller screens
- All themed with the current event's brand colors:
  - PrimaryColor: Main text and headers
  - SecondaryColor: Icons and accent text
  - TertiaryColor: Borders and dividers
  - BackgroundColor: Card backgrounds
- Interactive elements with visual feedback (underlined links)

### 2. ProcessEventPics.Maui\AboutPage.xaml.cs
Code-behind with event handlers:
- **OnBackClicked**: Navigates back to main page
- **OnWebsiteTapped**: Opens the Running Stars website in the default browser
  - Includes error handling with user-friendly alerts
- **OnEmailTapped**: Opens default email client with pre-filled email to info@runningstars.org.au
  - Fallback: If email client unavailable, copies email to clipboard

## Files Modified

### 1. ProcessEventPics.Maui\AppShell.xaml.cs
- Registered "AboutPage" route for Shell navigation

### 2. ProcessEventPics.Maui\ViewModels\MainViewModel.cs
- Updated `ShowInfoAsync()` method to navigate to AboutPage instead of showing dialog
- Passes MainViewModel as navigation parameter to maintain theme context

## User Experience Improvements

1. **Professional Presentation**: Full-page layout provides better branding and information hierarchy
2. **Themed Design**: Automatically adapts to the currently selected event's theme colors
3. **Interactive Links**: Users can click to visit website or send email directly
4. **Better Readability**: Larger text areas with proper spacing and formatting
5. **Visual Appeal**: Logo, icons, and styled sections create a polished appearance
6. **Accessibility**: Clear navigation with back button and proper text contrast

## Technical Implementation

- Uses MAUI's `Launcher.OpenAsync()` for opening URLs
- Uses MAUI's `Email.ComposeAsync()` for email functionality
- Implements fallback mechanisms for platforms without email clients
- Maintains consistent theming through data binding to MainViewModel's SelectedBrand
- Follows MVVM pattern with navigation parameters

## Testing Recommendations

1. Test website link opens in default browser on all platforms (Windows, Mac)
2. Test email link opens email client or shows clipboard notification
3. Verify theme colors display correctly when different events are selected
4. Confirm back button navigation returns to main page properly
5. Test on different screen sizes to ensure responsive layout works
