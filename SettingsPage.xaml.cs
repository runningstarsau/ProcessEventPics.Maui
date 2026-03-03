using Microsoft.Maui.Controls;
using ProcessEventPics.Maui.ViewModels;

namespace ProcessEventPics.Maui;

[QueryProperty(nameof(ViewModel), "ViewModel")]
public partial class SettingsPage : ContentPage
{
    private MainViewModel? _viewModel;

    public MainViewModel? ViewModel
    {
        get => _viewModel;
        set
        {
            _viewModel = value;
            BindingContext = _viewModel;
        }
    }

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Save brand settings when leaving the page
        if (_viewModel != null)
        {
            _viewModel.SaveBrandsSettings();
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.SaveBrandsSettings();
            await DisplayAlertAsync("Settings Saved", "All brand settings and colors have been saved successfully.", "OK");
        }
    }

    private void OnSaveToEventPhotosFullSizeTapped(object? sender, TappedEventArgs e)
    {
        SaveToEventPhotosFullSizeCheck.IsChecked = !SaveToEventPhotosFullSizeCheck.IsChecked;
    }

    private void OnConvertImagesTapped(object? sender, TappedEventArgs e)
    {
        if (!ConvertImagesCheck.IsEnabled)
            return;

        ConvertImagesCheck.IsChecked = !ConvertImagesCheck.IsChecked;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
