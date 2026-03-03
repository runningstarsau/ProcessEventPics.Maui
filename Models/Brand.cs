using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProcessEventPics.Maui.Models;

public class Brand : INotifyPropertyChanged
{
    private string _primaryColor = "#2196F3";
    private string _secondaryColor = "#4CAF50";
    private string _tertiaryColor = "#FFC107";
    private string _backgroundColor = "#FFFFFF";
    private string _foregroundColor = "#000000";
    private string _textColor = "#FFFFFF";

    public string Name { get; set; } = string.Empty;
    public string LogoFileName { get; set; } = string.Empty;
    public bool LocationFirstInFilename { get; set; }
    public string EventPhotosFullSizeFolder { get; set; } = string.Empty;
    public string EventPhotosCompressedFolder { get; set; } = string.Empty;
    public string RawFolder { get; set; } = string.Empty;
    public List<string> Locations { get; set; } = new();
    public List<string> FavoriteLocations { get; set; } = new();

    // Theme Colors with PropertyChanged notifications
    public string PrimaryColor
    {
        get => _primaryColor;
        set
        {
            if (_primaryColor != value)
            {
                _primaryColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string SecondaryColor
    {
        get => _secondaryColor;
        set
        {
            if (_secondaryColor != value)
            {
                _secondaryColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string TertiaryColor
    {
        get => _tertiaryColor;
        set
        {
            if (_tertiaryColor != value)
            {
                _tertiaryColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (_backgroundColor != value)
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string ForegroundColor
    {
        get => _foregroundColor;
        set
        {
            if (_foregroundColor != value)
            {
                _foregroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string TextColor
    {
        get => _textColor;
        set
        {
            if (_textColor != value)
            {
                _textColor = value;
                OnPropertyChanged();
            }
        }
    }

    public Brand()
    {
        Locations = new List<string>();
        FavoriteLocations = new List<string>();
    }

    public Brand(string name, string logoFileName)
    {
        Name = name;
        LogoFileName = logoFileName;
        Locations = new List<string>();
        FavoriteLocations = new List<string>();
    }

    public static Brand CreateParkrun()
    {
        return new Brand
        {
            Name = "parkrun",
            LogoFileName = "parkrun_logo.png",
            LocationFirstInFilename = false,
            PrimaryColor = "#2B233D",        // parkrun Green
            SecondaryColor = "#FFA300",      // parkrun Purple
            TertiaryColor = "#00CAAE",       // White for accents
            BackgroundColor = "#2B233D",     // Dark Purple background
            ForegroundColor = "#FFFFFF",     // White for text on dark
            TextColor = "#FFFFFF",           // White text
            Locations = new List<string>
            {
                "Auburn Botanic Gardens",
                "Australian Botanic Garden Mount Annan",
                "Campbelltown",
                "Casula Parklands",
                "Centennial",
                "Chipping Norton",
                "Cooks River",
                "Cowpasture Reserve, Camden",
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
                "Mt Penang",
                "Nepean River",
                "North Sydney",
                "Panania",
                "Parramatta",
                "Penrith Lakes",
                "Picton",
                "Pirrama",
                "Rex Jackson Oval",
                "Rhodes",
                "Rouse Hill",
                "Rooty Hill",
                "St Peters",
                "The Ponds",
                "Wentworth Common",
                "Werrington Lakes",
                "Willoughby",
                "Whalan Reserve",
                "Wildflower",
                "Woy Woy",
            }
        };
    }

    public static Brand CreateRunningStars()
    {
        return new Brand
        {
            Name = "Running Stars",
            LogoFileName = "runningstars_logo.png",
            LocationFirstInFilename = true,
            PrimaryColor = "#60a5fa",        // Running Stars Blue
            SecondaryColor = "#FFFF00",      // Gold (star color)
            TertiaryColor = "#1e3a8a",       // Blue for accents
            BackgroundColor = "#FFFFFF",     // White background
            ForegroundColor = "#1e3a8a",     // Blue text
            TextColor = "#FFFFFF",           // Black text on white
            FavoriteLocations = new List<string>(),
            Locations = new List<string>
            {
                "Curl Curl",
                "Mona Vale"
            }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
