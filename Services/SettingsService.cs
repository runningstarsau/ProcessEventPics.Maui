using ProcessEventPics.Maui.Models;

namespace ProcessEventPics.Maui.Services;

public class SettingsService
{
    private const string SourceFolderKey = "SourceFolder";
    private const string BrandsKey = "Brands";
    private const string SaveToEventPhotosFullSizeKey = "SaveToEventPhotosFullSize";
    private const string ConvertImagesKey = "ConvertImages";
    private const string DayBrandMappingsKey = "DayBrandMappings";
    private const string ImageResizeSizeKey = "ImageResizeSize";
    private const string JpgCompressionKey = "JpgCompression";

    public SettingsService()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        if (!Preferences.Default.ContainsKey(SourceFolderKey))
            SourceFolder = GetDefaultSourceFolder();

        if (!Preferences.Default.ContainsKey(SaveToEventPhotosFullSizeKey))
            SaveToEventPhotosFullSize = true;

        if (!Preferences.Default.ContainsKey(ConvertImagesKey))
            ConvertImages = true;

        if (!Preferences.Default.ContainsKey(BrandsKey))
            Brands = GetDefaultBrands();

        if (!Preferences.Default.ContainsKey(DayBrandMappingsKey))
            DayBrandMappings = GetDefaultDayBrandMappings();

        if (!Preferences.Default.ContainsKey(ImageResizeSizeKey))
            ImageResizeSize = 2048;

        if (!Preferences.Default.ContainsKey(JpgCompressionKey))
            JpgCompression = 85;
    }

    public string SourceFolder
    {
        get => Preferences.Default.Get(SourceFolderKey, GetDefaultSourceFolder());
        set => Preferences.Default.Set(SourceFolderKey, value);
    }

    public bool SaveToEventPhotosFullSize
    {
        get => Preferences.Default.Get(SaveToEventPhotosFullSizeKey, true);
        set => Preferences.Default.Set(SaveToEventPhotosFullSizeKey, value);
    }

    public bool ConvertImages
    {
        get => Preferences.Default.Get(ConvertImagesKey, true);
        set => Preferences.Default.Set(ConvertImagesKey, value);
    }

    public int ImageResizeSize
    {
        get => Preferences.Default.Get(ImageResizeSizeKey, 2048);
        set => Preferences.Default.Set(ImageResizeSizeKey, value);
    }

    public int JpgCompression
    {
        get => Preferences.Default.Get(JpgCompressionKey, 85);
        set => Preferences.Default.Set(JpgCompressionKey, value);
    }

    public List<Brand> Brands
    {
        get
        {
            string json = Preferences.Default.Get(BrandsKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return GetDefaultBrands();
            }

            try
            {
                var brands = System.Text.Json.JsonSerializer.Deserialize<List<Brand>>(json);
                if (brands == null || brands.Count == 0)
                {
                    return GetDefaultBrands();
                }

                // Ensure all brands have initialized lists and valid colors
                foreach (var brand in brands)
                {
                    brand.Locations ??= new List<string>();
                    brand.FavoriteLocations ??= new List<string>();

                    // Ensure colors are not null or empty
                    if (string.IsNullOrWhiteSpace(brand.PrimaryColor))
                        brand.PrimaryColor = "#2196F3";
                    if (string.IsNullOrWhiteSpace(brand.SecondaryColor))
                        brand.SecondaryColor = "#4CAF50";
                    if (string.IsNullOrWhiteSpace(brand.TertiaryColor))
                        brand.TertiaryColor = "#FFC107";
                    if (string.IsNullOrWhiteSpace(brand.BackgroundColor))
                        brand.BackgroundColor = "#FFFFFF";
                    if (string.IsNullOrWhiteSpace(brand.ForegroundColor))
                        brand.ForegroundColor = "#000000";
                    if (string.IsNullOrWhiteSpace(brand.TextColor))
                        brand.TextColor = "#FFFFFF";
                }

                return brands;
            }
            catch
            {
                // If deserialization fails, return defaults
                return GetDefaultBrands();
            }
        }
        set
        {
            string json = System.Text.Json.JsonSerializer.Serialize(value);
            Preferences.Default.Set(BrandsKey, json);
        }
    }

    public Dictionary<DayOfWeek, string> DayBrandMappings
    {
        get
        {
            string json = Preferences.Default.Get(DayBrandMappingsKey, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return GetDefaultDayBrandMappings();
            }
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<DayOfWeek, string>>(json) ?? GetDefaultDayBrandMappings();
        }
        set
        {
            string json = System.Text.Json.JsonSerializer.Serialize(value);
            Preferences.Default.Set(DayBrandMappingsKey, json);
        }
    }

    private string GetDefaultSourceFolder()
    {
        if (OperatingSystem.IsWindows())
            return @"D:\DCIM";
        else if (OperatingSystem.IsMacOS())
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DCIM");
        else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "DCIM");
    }

    private List<Brand> GetDefaultBrands()
    {
        var brands = new List<Brand>();

        // parkrun brand with official parkrun colors
        var parkrun = new Brand("parkrun", "parkrun_icon.png")
        {
            LocationFirstInFilename = false,
            EventPhotosFullSizeFolder = GetDefaultEventPhotosFullSizeFolder("parkrun"),
            EventPhotosCompressedFolder = GetDefaultEventPhotosCompressedFolder("parkrun"),
            RawFolder = GetDefaultRawFolder("parkrun"),
            PrimaryColor = "#2b233d",    // Dark Purple (main brand color)
            SecondaryColor = "#ffa300",  // Apricot (accent)
            TertiaryColor = "#00ceae",   // Aqua Green (highlight)
            BackgroundColor = "#2b233d", // Dark Purple background
            ForegroundColor = "#FFFFFF", // White text on dark background
            TextColor = "#FFFFFF",
            Locations = new List<string>
            {
                "Auburn Botanic Gardens",
                "Centennial",
                "Chipping Norton",
                "Cooks River",
                "Cronulla",
                "Curl Curl",
                "Dolls Point",
                "Elara Riparian Parklands",
                "Glaston",
                "Greenway",
                "Kamay",
                "Macquarie University",
                "Menai",
                "Mosman",
                "Panania",
                "Parramatta",
                "Pirrama",
                "Rhodes",
                "Rouse Hill",
                "St Peters",
                "The Ponds",
                "Wentworth Common",
                "Whalan Reserve",
                "Wildflower"
            }
        };
        brands.Add(parkrun);

        // Running Stars brand (based on runningstars.org.au)
        var runningStars = new Brand("Running Stars", "runningstars_icon.png")
        {
            LocationFirstInFilename = false,
            EventPhotosFullSizeFolder = GetDefaultEventPhotosFullSizeFolder("RunningStars"),
            EventPhotosCompressedFolder = GetDefaultEventPhotosCompressedFolder("RunningStars"),
            RawFolder = GetDefaultRawFolder("Running Stars"),
            PrimaryColor = "#60a5fa",    // Deep Blue (from website)
            SecondaryColor = "#FFFF00",  // Golden Yellow (star color)
            TertiaryColor = "#1e3a8a",   // Light Blue (accent)
            BackgroundColor = "#FFFFFF", // White background
            ForegroundColor = "#1e3a8a", // Deep Blue text on white
            TextColor = "#FFFFFF", // White background
            Locations = new List<string>
            {
                "Curl Curl",
                "Mona Vale"
            }
        };
        brands.Add(runningStars);

        return brands;
    }

    private Dictionary<DayOfWeek, string> GetDefaultDayBrandMappings()
    {
        return new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday, "parkrun" },
            { DayOfWeek.Tuesday, "parkrun" },
            { DayOfWeek.Wednesday, "parkrun" },
            { DayOfWeek.Thursday, "parkrun" },
            { DayOfWeek.Friday, "parkrun" },
            { DayOfWeek.Saturday, "parkrun" },
            { DayOfWeek.Sunday, "Running Stars" }
        };
    }

    private string GetDefaultEventPhotosFullSizeFolder(string brandName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "EventPics", "FullSize");
    }

    private string GetDefaultEventPhotosCompressedFolder(string brandName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "EventPics", "Compressed");
    }

    private string GetDefaultRawFolder(string brandName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "EventPics", "RAW");
    }
}
