using ProcessEventPics.Maui.ViewModels;

namespace ProcessEventPics.Maui;

public partial class AboutPage : ContentPage
{
    private MainViewModel? _viewModel;

    public AboutPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        // Get the ViewModel from the navigation parameter or create a new one
        if (BindingContext is MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnWebsiteTapped(object sender, EventArgs e)
    {
        try
        {
            var uri = new Uri("https://runningstars.org.au");
            await Launcher.OpenAsync(uri);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unable to open website: {ex.Message}", "OK");
        }
    }

    private async void OnEmailTapped(object sender, EventArgs e)
    {
        try
        {
            var message = new EmailMessage
            {
                Subject = "Process Event Pics - Inquiry",
                To = new List<string> { "info@runningstars.org.au" }
            };
            
            await Email.ComposeAsync(message);
        }
        catch (Exception ex)
        {
            // If email client is not available, try to copy to clipboard
            await Clipboard.SetTextAsync("info@runningstars.org.au");
            await DisplayAlert("Email", "Email address copied to clipboard:\ninfo@runningstars.org.au", "OK");
        }
    }
}
