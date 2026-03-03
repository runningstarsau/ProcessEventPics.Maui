using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ProcessEventPics.Maui.Services;
using ProcessEventPics.Maui.Models;

namespace ProcessEventPics.Maui.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly SettingsService _settingsService;
    private readonly ImageProcessingService _imageProcessingService;
    private readonly IFolderPickerService? _folderPickerService;
    private Page? _page;

    private string _sourceFolder;
    private bool _saveToEventPhotosFullSize = true;
    private bool _convertImages = true;
    private Brand? _selectedBrand;
    private string _selectedLocation = string.Empty;
    private string _locationSearchText = string.Empty;
    private int _jpgFileCount;
    private int _rawFileCount;
    private bool _isProcessing;
    private string _statusMessage = string.Empty;
    private double _progressValue;
    private Dictionary<DayOfWeek, string> _dayBrandMappings;
    private int _imageResizeSize = 2048;
    private int _jpgCompression = 85;

    public bool IsCompressionAvailable => !(OperatingSystem.IsMacCatalyst() || OperatingSystem.IsMacOS());
    public bool IsCompressionOptionEnabled => IsCompressionAvailable && ConvertImages;
    public bool IsCompressionNotAvailableForMac => !IsCompressionAvailable;
    public double CompressionOptionOpacity => IsCompressionOptionEnabled ? 1.0 : 0.45;

    public MainViewModel(IFolderPickerService? folderPickerService = null)
    {
        _settingsService = new SettingsService();
        _imageProcessingService = new ImageProcessingService();
        _folderPickerService = folderPickerService;

        if (_folderPickerService == null)
        {
#if WINDOWS
            _folderPickerService = new Platforms.Windows.FolderPickerService();
#elif MACCATALYST || MACOS
            _folderPickerService = new Platforms.MacCatalyst.FolderPickerService();
#endif
        }

        _sourceFolder = _settingsService.SourceFolder;
        _saveToEventPhotosFullSize = _settingsService.SaveToEventPhotosFullSize;
        _convertImages = _settingsService.ConvertImages;
        if (!IsCompressionAvailable)
        {
            _convertImages = false;
            _settingsService.ConvertImages = false;
        }
        _dayBrandMappings = _settingsService.DayBrandMappings;
        _imageResizeSize = _settingsService.ImageResizeSize;
        _jpgCompression = _settingsService.JpgCompression;

        Brands = new ObservableCollection<Brand>(_settingsService.Brands);

        // Diagnostic: Log event loading
        System.Diagnostics.Debug.WriteLine($"MainViewModel: Loaded {Brands.Count} events");
        foreach (var brand in Brands)
        {
            System.Diagnostics.Debug.WriteLine($"  - Event: {brand.Name}, Logo: {brand.LogoFileName}, Locations: {brand.Locations.Count}");
        }

        BrowseSourceCommand = new Command(async () => await BrowseSourceFolderAsync());
        RefreshFilesCommand = new Command(async () => await RefreshFileCountAsync());
        FindCameraCommand = new Command(async () => await FindCameraAsync());
        ProcessImagesCommand = new Command(async () => await ProcessImagesAsync(), () => !IsProcessing);
        AddLocationDialogCommand = new Command(async () => await AddLocationDialogAsync());
        AddBrandDialogCommand = new Command(async () => await AddBrandDialogAsync());
        DeleteLocationCommand = new Command<string>(DeleteLocation);
        DeleteBrandCommand = new Command(async () => await DeleteBrandAsync());
        SelectBrandCommand = new Command<Brand>(SelectBrand);
        NavigateToSettingsCommand = new Command(async () => await NavigateToSettingsAsync());
        ShowInfoCommand = new Command(async () => await ShowInfoAsync());
        ResetBrandColorsCommand = new Command(ResetBrandColors);
        ToggleFavoriteCommand = new Command<string>(ToggleFavorite);
        ResetToDefaultsCommand = new Command(ResetToDefaults);

        SetDefaultBrand();
        _ = RefreshFileCountAsync();
    }

    public ObservableCollection<Brand> Brands { get; }

    public Brand? SelectedBrand
    {
        get => _selectedBrand;
        set
        {
            if (_selectedBrand != value)
            {
                _selectedBrand = value;
                LocationSearchText = string.Empty; // Clear search when brand changes
                OnPropertyChanged();
                OnPropertyChanged(nameof(BrandName));
                OnPropertyChanged(nameof(CurrentLocations));
                OnPropertyChanged(nameof(FilteredLocations));
                OnPropertyChanged(nameof(SampleFileName));
            }
        }
    }

    public string BrandName => SelectedBrand?.Name ?? "Select Event";

    public ObservableCollection<string> CurrentLocations =>
        SelectedBrand != null
            ? new ObservableCollection<string>(SelectedBrand.Locations.OrderBy(l => l))
            : new ObservableCollection<string>();

    public string LocationSearchText
    {
        get => _locationSearchText;
        set
        {
            _locationSearchText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FilteredLocations));
        }
    }

    public ObservableCollection<string> FilteredLocations
    {
        get
        {
            if (SelectedBrand == null)
                return new ObservableCollection<string>();

            // Split locations into favorites and non-favorites
            var favorites = SelectedBrand.Locations
                .Where(l => SelectedBrand.FavoriteLocations.Contains(l))
                .OrderBy(l => l);

            var nonFavorites = SelectedBrand.Locations
                .Where(l => !SelectedBrand.FavoriteLocations.Contains(l))
                .OrderBy(l => l);

            // Apply search filter if present
            if (!string.IsNullOrWhiteSpace(LocationSearchText))
            {
                favorites = favorites.Where(l => 
                    l.Contains(LocationSearchText, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(l => l);
                nonFavorites = nonFavorites.Where(l => 
                    l.Contains(LocationSearchText, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(l => l);
            }

            // Concatenate favorites first, then non-favorites
            var result = favorites.Concat(nonFavorites);
            return new ObservableCollection<string>(result);
        }
    }

    public string SelectedLocation
    {
        get => _selectedLocation;
        set
        {
            _selectedLocation = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SampleFileName));
        }
    }

    public string SourceFolder
    {
        get => _sourceFolder;
        set
        {
            if (_sourceFolder != value)
            {
                _sourceFolder = value;
                _settingsService.SourceFolder = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SourceFolderName));
                _ = RefreshFileCountAsync();
            }
        }
    }

    public bool SaveToEventPhotosFullSize
    {
        get => _saveToEventPhotosFullSize;
        set
        {
            _saveToEventPhotosFullSize = value;
            _settingsService.SaveToEventPhotosFullSize = value;
            OnPropertyChanged();
        }
    }

    public bool ConvertImages
    {
        get => _convertImages;
        set
        {
            var effectiveValue = IsCompressionAvailable && value;
            _convertImages = effectiveValue;
            _settingsService.ConvertImages = effectiveValue;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsCompressionOptionEnabled));
            OnPropertyChanged(nameof(CompressionOptionOpacity));
            OnPropertyChanged(nameof(ShowNoResizeNoCompression));
        }
    }

    public bool ShowNoResizeNoCompression => !ConvertImages;

    public Dictionary<DayOfWeek, string> DayBrandMappings
    {
        get => _dayBrandMappings;
        set
        {
            _dayBrandMappings = value;
            _settingsService.DayBrandMappings = value;
            OnPropertyChanged();
        }
    }

    // Helper properties for day-specific brand selection in UI
    public Brand? MondayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Monday) ? DayBrandMappings[DayOfWeek.Monday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Monday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? TuesdayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Tuesday) ? DayBrandMappings[DayOfWeek.Tuesday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Tuesday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? WednesdayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Wednesday) ? DayBrandMappings[DayOfWeek.Wednesday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Wednesday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? ThursdayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Thursday) ? DayBrandMappings[DayOfWeek.Thursday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Thursday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? FridayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Friday) ? DayBrandMappings[DayOfWeek.Friday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Friday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? SaturdayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Saturday) ? DayBrandMappings[DayOfWeek.Saturday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Saturday] = value.Name; OnPropertyChanged(); }
    }

    public Brand? SundayBrand
    {
        get => Brands.FirstOrDefault(b => b.Name == (DayBrandMappings.ContainsKey(DayOfWeek.Sunday) ? DayBrandMappings[DayOfWeek.Sunday] : string.Empty));
        set { if (value != null) DayBrandMappings[DayOfWeek.Sunday] = value.Name; OnPropertyChanged(); }
    }

    public int JpgFileCount
    {
        get => _jpgFileCount;
        set { _jpgFileCount = value; OnPropertyChanged(); }
    }

    public int RawFileCount
    {
        get => _rawFileCount;
        set { _rawFileCount = value; OnPropertyChanged(); }
    }

    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            OnPropertyChanged();
            ((Command)ProcessImagesCommand).ChangeCanExecute();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public double ProgressValue
    {
        get => _progressValue;
        set { _progressValue = value; OnPropertyChanged(); }
    }

    public int ImageResizeSize
    {
        get => _imageResizeSize;
        set
        {
            if (_imageResizeSize != value)
            {
                _imageResizeSize = value;
                _settingsService.ImageResizeSize = value;
                OnPropertyChanged();
            }
        }
    }

    public int JpgCompression
    {
        get => _jpgCompression;
        set
        {
            if (_jpgCompression != value)
            {
                _jpgCompression = value;
                _settingsService.JpgCompression = value;
                OnPropertyChanged();
            }
        }
    }

    public string SourceFolderName => Path.GetFileName(SourceFolder) ?? SourceFolder;

    public string SampleFileName
    {
        get
        {
            if (SelectedBrand == null || string.IsNullOrEmpty(SelectedLocation))
                return "Select an event and location to see sample filename";

            DateTime now = DateTime.Now;
            string year = now.ToString("yyyy");
            string month = now.ToString("MM");
            string day = now.ToString("dd");
            string locationFormatted = SelectedLocation.Replace(" ", "_");
            string brandNameFormatted = SelectedBrand.Name.Replace(" ", "_");

            if (SelectedBrand.LocationFirstInFilename)
                return $"{locationFormatted}_{brandNameFormatted}_{year}_{month}_{day}_0001.jpg";
            else
                return $"{brandNameFormatted}_{locationFormatted}_{year}_{month}_{day}_0001.jpg";
        }
    }

    public ICommand BrowseSourceCommand { get; }
    public ICommand RefreshFilesCommand { get; }
    public ICommand FindCameraCommand { get; }
    public ICommand ProcessImagesCommand { get; }
    public ICommand AddLocationDialogCommand { get; }
    public ICommand AddBrandDialogCommand { get; }
    public ICommand DeleteLocationCommand { get; }
    public ICommand DeleteBrandCommand { get; }
    public ICommand SelectBrandCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand ShowInfoCommand { get; }
    public ICommand ResetBrandColorsCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand ResetToDefaultsCommand { get; }

    public void SetPage(Page page)
    {
        _page = page;
    }

    private void SetDefaultBrand()
    {
        DayOfWeek today = DateTime.Now.DayOfWeek;

        // Check if there's a mapping for today
        if (DayBrandMappings.TryGetValue(today, out string? brandName))
        {
            var brand = Brands.FirstOrDefault(b => b.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase));
            if (brand != null)
            {
                SelectedBrand = brand;
                return;
            }
        }

        // Fallback to first brand if no mapping found
        if (Brands.Any())
            SelectedBrand = Brands.First();
    }

    private void SelectBrand(Brand brand)
    {
        SelectedBrand = brand;

        // Move selected brand to the top of the collection
        if (brand != null && Brands.Contains(brand))
        {
            int currentIndex = Brands.IndexOf(brand);
            if (currentIndex > 0)
            {
                Brands.Move(currentIndex, 0);
            }
        }
    }

    private async Task BrowseSourceFolderAsync()
    {
        if (_folderPickerService == null)
            return;

        var path = await _folderPickerService.PickFolderAsync();
        if (!string.IsNullOrEmpty(path))
        {
            var hasAccess = await _folderPickerService.EnsureFolderAccessAsync(path);
            if (!hasAccess)
            {
                StatusMessage = "Folder access was denied.";
                if (_page != null)
                    await _page.DisplayAlertAsync("Access Required", "Unable to access the selected folder. Please select a folder you have granted access to.", "OK");
                return;
            }

            SourceFolder = path;
        }
    }

    private async Task RefreshFileCountAsync()
    {
        if (string.IsNullOrWhiteSpace(SourceFolder) || _folderPickerService == null)
        {
            JpgFileCount = 0;
            RawFileCount = 0;
            return;
        }

        var hasAccess = await _folderPickerService.EnsureFolderAccessAsync(SourceFolder, promptIfNeeded: false);
        if (!hasAccess)
        {
            JpgFileCount = 0;
            RawFileCount = 0;
            StatusMessage = "Access to source folder is denied. Use Browse to re-select the folder.";
            return;
        }

        var (jpgFiles, rawFiles) = await _imageProcessingService.GetFilesAsync(SourceFolder);
        JpgFileCount = jpgFiles.Length;
        RawFileCount = rawFiles.Length;
    }

    private async Task FindCameraAsync()
    {
        StatusMessage = "Searching for camera...";
        var cameraPath = await _imageProcessingService.FindCameraAsync();
        if (cameraPath != null)
        {
            if (_folderPickerService != null)
            {
                var hasAccess = await _folderPickerService.EnsureFolderAccessAsync(cameraPath, promptIfNeeded: false);
                if (!hasAccess)
                {
                    StatusMessage = "Camera found but access is denied. Please browse to the camera folder manually.";
                    return;
                }
            }

            SourceFolder = cameraPath;
            StatusMessage = $"Camera found at {cameraPath}";
        }
        else
        {
            if (OperatingSystem.IsMacCatalyst())
            {
                StatusMessage = "Use Browse to select the camera DCIM folder on Mac.";
            }
            else
            {
                StatusMessage = "No camera found on removable drives.";
            }
        }
    }

    private async Task ProcessImagesAsync()
    {
        if (SelectedBrand == null || string.IsNullOrEmpty(SelectedLocation))
        {
            if (_page != null)
                await _page.DisplayAlertAsync("Error", "Please select an event and location.", "OK");
            return;
        }

        IsProcessing = true;
        ProgressValue = 0;

        try
        {
            if (_folderPickerService != null)
            {
                var hasAccess = await _folderPickerService.EnsureFolderAccessAsync(SourceFolder);
                if (!hasAccess)
                {
                    if (_page != null)
                        await _page.DisplayAlertAsync("Access Required", "Access to the source folder is denied. Please browse and select the source folder again.", "OK");
                    return;
                }
            }

            var (jpgFiles, rawFiles) = await _imageProcessingService.GetFilesAsync(SourceFolder);

            if (jpgFiles.Length == 0 && rawFiles.Length == 0)
            {
                if (_page != null)
                    await _page.DisplayAlertAsync("Error", "No files found to process.", "OK");
                return;
            }

            DateTime now = DateTime.Now;
            string dateFolder = now.ToString("yyyyMMdd");
            string year = now.ToString("yyyy");
            string month = now.ToString("MM");
            string day = now.ToString("dd");

            string locationFormatted = SelectedLocation.Replace(" ", "_");
            string brandNameFormatted = SelectedBrand.Name.Replace(" ", "_");

            string jpgDestination = Path.Combine(SelectedBrand.EventPhotosFullSizeFolder, SelectedBrand.Name, SelectedLocation, dateFolder);
            string rawDestination = Path.Combine(SelectedBrand.RawFolder, SelectedBrand.Name, SelectedLocation, dateFolder);
            string? finalFolderToOpen = null;
            
            string filePrefix;
            if (SelectedBrand.LocationFirstInFilename)
                filePrefix = $"{locationFormatted}_{brandNameFormatted}_{year}_{month}_{day}_";
            else
                filePrefix = $"{brandNameFormatted}_{locationFormatted}_{year}_{month}_{day}_";

            // Copy JPG files
            if (SaveToEventPhotosFullSize && jpgFiles.Length > 0)
            {
                StatusMessage = "Copying JPG files...";
                var progress = new Progress<(int current, int total, string fileName)>(p =>
                {
                    ProgressValue = (double)p.current / p.total;
                    StatusMessage = $"Copying JPG {p.current}/{p.total}: {p.fileName}";
                });

                await _imageProcessingService.CopyFilesAsync(jpgFiles, jpgDestination, filePrefix, progress);
                finalFolderToOpen = jpgDestination;
            }

            // Copy RAW files
            if (SaveToEventPhotosFullSize && rawFiles.Length > 0)
            {
                StatusMessage = "Copying RAW files...";
                var progress = new Progress<(int current, int total, string fileName)>(p =>
                {
                    ProgressValue = (double)p.current / p.total;
                    StatusMessage = $"Copying RAW {p.current}/{p.total}: {p.fileName}";
                });

                await _imageProcessingService.CopyFilesAsync(rawFiles, rawDestination, filePrefix, progress);
            }

            // Resize and compress for Facebook
            if (ConvertImages && IsCompressionAvailable && jpgFiles.Length > 0)
            {
                StatusMessage = "Resizing and compressing images...";
                
                string relativePath = Path.GetRelativePath(SelectedBrand.EventPhotosFullSizeFolder, jpgDestination);
                string eventPhotosCompressedDestination = Path.Combine(SelectedBrand.EventPhotosCompressedFolder, relativePath);

                var progress = new Progress<(int current, int total, string fileName, long originalSize, long newSize)>(p =>
                {
                    ProgressValue = (double)p.current / p.total;
                    StatusMessage = $"Processing {p.current}/{p.total}: {p.fileName} ({p.originalSize / 1024}KB → {p.newSize / 1024}KB)";
                });

                StatusMessage = $"Resizing and compressing images from {jpgDestination} to {eventPhotosCompressedDestination}";

                await _imageProcessingService.ResizeAndCompressAsync(jpgDestination, eventPhotosCompressedDestination, progress);
                finalFolderToOpen = eventPhotosCompressedDestination;
            }
            else if (!ConvertImages && jpgFiles.Length > 0)
            {
                StatusMessage = "No resizing and No Compression. Completed copy and rename.";
            }

            if (!string.IsNullOrWhiteSpace(finalFolderToOpen))
            {
                _imageProcessingService.OpenFolder(finalFolderToOpen);
            }

            ProgressValue = 1.0;
            StatusMessage = "Processing complete!";
            if (_page != null)
                await _page.DisplayAlertAsync("Success", "All files processed successfully!", "OK");
        }
        catch (Exception ex)
        {
            if (_page != null)
                await _page.DisplayAlertAsync("Error", $"Processing failed: {ex.Message}", "OK");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task AddLocationDialogAsync()
    {
        if (_page == null || SelectedBrand == null)
            return;

        string result = await _page.DisplayPromptAsync(
            $"Add {SelectedBrand.Name} Location",
            "Enter the location name:",
            placeholder: "e.g., Auburn Botanic Gardens",
            maxLength: 50);

        if (!string.IsNullOrWhiteSpace(result))
        {
            if (!SelectedBrand.Locations.Contains(result.Trim()))
            {
                SelectedBrand.Locations.Add(result.Trim());
                SelectedBrand.Locations = SelectedBrand.Locations.OrderBy(l => l).ToList();
                SaveBrands();
                OnPropertyChanged(nameof(CurrentLocations));
                OnPropertyChanged(nameof(FilteredLocations));
                SelectedLocation = result.Trim();
            }
            else
            {
                await _page.DisplayAlertAsync("Duplicate", $"The location '{result.Trim()}' already exists.", "OK");
            }
        }
    }

    private async Task AddBrandDialogAsync()
    {
        if (_page == null)
            return;

        string brandName = await _page.DisplayPromptAsync(
            "Add New Event",
            "Enter the event name:",
            placeholder: "e.g., Color Run",
            maxLength: 50);

        if (string.IsNullOrWhiteSpace(brandName))
            return;

        if (Brands.Any(b => b.Name.Equals(brandName.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            await _page.DisplayAlertAsync("Duplicate", $"The event '{brandName.Trim()}' already exists.", "OK");
            return;
        }

        string logoFileName = await _page.DisplayPromptAsync(
            "Event Logo",
            "Enter the logo filename (optional):",
            placeholder: "e.g., event_icon.png",
            maxLength: 100);

        var newBrand = new Brand(brandName.Trim(), logoFileName?.Trim() ?? string.Empty)
        {
            EventPhotosFullSizeFolder = GetDefaultFolder("EventPhotosFullSize"),
            EventPhotosCompressedFolder = GetDefaultFolder("EventPhotosCompressed"),
            RawFolder = GetDefaultFolder("RAW"),
            LocationFirstInFilename = false
        };

        Brands.Add(newBrand);
        SaveBrands();
        SelectedBrand = newBrand;

        await _page.DisplayAlertAsync("Success", $"Event '{brandName.Trim()}' has been created.", "OK");
    }

    private string GetDefaultFolder(string folderType)
    {
        if (OperatingSystem.IsWindows())
        {
            return folderType switch
            {
                "EventPhotosFullSize" => @"E:\EventPhotos\FullSize",
                "EventPhotosCompressed" => @"E:\EventPhotos\Compressed",
                "RAW" => @"P:\",
                _ => string.Empty
            };
        }
        else
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            return folderType switch
            {
                "EventPhotosFullSize" => Path.Combine(basePath, "EventPhotosFullSize"),
                "EventPhotosCompressed" => Path.Combine(basePath, "EventPhotosCompressed"),
                "RAW" => Path.Combine(basePath, "RAW"),
                _ => string.Empty
            };
        }
    }


    private async void DeleteLocation(string location)
    {
        if (string.IsNullOrEmpty(location) || SelectedBrand == null)
            return;

        if (_page != null)
        {
            bool result = await _page.DisplayAlertAsync("Confirm Delete", 
                $"Are you sure you want to delete this location?\n\n{location}", 
                "Yes", "No");

            if (!result)
                return;
        }

        SelectedBrand.Locations.Remove(location);
        SelectedBrand.FavoriteLocations.Remove(location);
        SaveBrands();
        OnPropertyChanged(nameof(CurrentLocations));
        OnPropertyChanged(nameof(FilteredLocations));
    }

    private void ToggleFavorite(string location)
    {
        if (string.IsNullOrEmpty(location) || SelectedBrand == null)
            return;

        if (SelectedBrand.FavoriteLocations.Contains(location))
        {
            SelectedBrand.FavoriteLocations.Remove(location);
        }
        else
        {
            SelectedBrand.FavoriteLocations.Add(location);
        }

        SaveBrands();
        OnPropertyChanged(nameof(FilteredLocations));
    }

    private async Task DeleteBrandAsync()
    {
        if (SelectedBrand == null || _page == null)
            return;

        // Prevent deleting the last brand
        if (Brands.Count <= 1)
        {
            await _page.DisplayAlertAsync("Cannot Delete", 
                "You must have at least one event. Cannot delete the last event.", 
                "OK");
            return;
        }

        string brandName = SelectedBrand.Name;
        int locationCount = SelectedBrand.Locations.Count;

        bool result = await _page.DisplayAlertAsync("Confirm Delete Event", 
            $"Are you sure you want to delete the event '{brandName}'?\n\n" +
            $"This will permanently delete:\n" +
            $"• The event configuration\n" +
            $"• All {locationCount} location(s)\n" +
            $"• Day of week mappings for this event\n\n" +
            $"This action cannot be undone.", 
            "Yes, Delete", "Cancel");

        if (!result)
            return;

        // Remove from day-of-week mappings
        var keysToUpdate = DayBrandMappings
            .Where(kvp => kvp.Value.Equals(brandName, StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToUpdate)
        {
            // Set to first available brand that isn't being deleted
            var firstOtherBrand = Brands.FirstOrDefault(b => b != SelectedBrand);
            if (firstOtherBrand != null)
            {
                DayBrandMappings[key] = firstOtherBrand.Name;
            }
        }

        // Remove the brand
        Brands.Remove(SelectedBrand);

        // Select another brand
        SelectedBrand = Brands.FirstOrDefault();

        // Save changes
        SaveBrands();
        _settingsService.DayBrandMappings = DayBrandMappings;

        // Notify day properties to refresh
        OnPropertyChanged(nameof(MondayBrand));
        OnPropertyChanged(nameof(TuesdayBrand));
        OnPropertyChanged(nameof(WednesdayBrand));
        OnPropertyChanged(nameof(ThursdayBrand));
        OnPropertyChanged(nameof(FridayBrand));
        OnPropertyChanged(nameof(SaturdayBrand));
        OnPropertyChanged(nameof(SundayBrand));

        await _page.DisplayAlertAsync("Event Deleted", 
            $"Event '{brandName}' and all its locations have been deleted.", 
            "OK");
    }

    private void SaveBrands()
    {
        _settingsService.Brands = Brands.ToList();
    }

    public void SaveBrandsSettings()
    {
        SaveBrands();
        _settingsService.DayBrandMappings = DayBrandMappings;
    }

    private async Task NavigateToSettingsAsync()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "ViewModel", this }
        };
        await Shell.Current.GoToAsync("SettingsPage", navigationParameter);
    }

    private async Task ShowInfoAsync()
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { "ViewModel", this }
        };
        await Shell.Current.GoToAsync("AboutPage", navigationParameter);
    }

    private void ResetBrandColors()
    {
        if (SelectedBrand == null)
            return;

        // Reset to default colors based on brand name
        if (SelectedBrand.Name.Equals("parkrun", StringComparison.OrdinalIgnoreCase))
        {
            SelectedBrand.PrimaryColor = "#2b233d";
            SelectedBrand.SecondaryColor = "#ffa300";
            SelectedBrand.TertiaryColor = "#00ceae";
            SelectedBrand.BackgroundColor = "#2b233d";
            SelectedBrand.ForegroundColor = "#FFFFFF";
            SelectedBrand.TextColor = "#FFFFFF";
        }
        else if (SelectedBrand.Name.Equals("Running Stars", StringComparison.OrdinalIgnoreCase))
        {
            SelectedBrand.PrimaryColor = "#60a5fa";
            SelectedBrand.SecondaryColor = "#FFFF00";
            SelectedBrand.TertiaryColor = "#1e3a8a";
            SelectedBrand.BackgroundColor = "#FFFFFF";
            SelectedBrand.ForegroundColor = "#1e3a8a";
            SelectedBrand.TextColor = "#FFFFFF";
        }
        else
        {
            // Generic defaults for custom brands
            SelectedBrand.PrimaryColor = "#60a5fa";
            SelectedBrand.SecondaryColor = "#FFFF00";
            SelectedBrand.TertiaryColor = "#1e3a8a";
            SelectedBrand.BackgroundColor = "#FFFFFF";
            SelectedBrand.ForegroundColor = "#1e3a8a";
            SelectedBrand.TextColor = "#FFFFFF";
        }

        // Notify UI of all color changes
        OnPropertyChanged(nameof(SelectedBrand));
        SaveBrands();
    }

    public void ResetToDefaults()
    {
        // Clear all preferences
        Preferences.Default.Clear();

        // Reinitialize settings service to load defaults
        var defaultBrands = new SettingsService().Brands;

        // Clear and reload brands
        Brands.Clear();
        foreach (var brand in defaultBrands)
        {
            Brands.Add(brand);
        }

        // Reset selected brand
        SetDefaultBrand();

        // Notify UI
        OnPropertyChanged(nameof(Brands));
        OnPropertyChanged(nameof(SelectedBrand));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
