#if WINDOWS
using ProcessEventPics.Maui.Services;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ProcessEventPics.Maui.Platforms.Windows;

public class FolderPickerService : IFolderPickerService
{
    public async Task<string?> PickFolderAsync()
    {
        var folderPicker = new FolderPicker();

        var window = (MauiWinUIWindow?)Application.Current?.Windows[0]?.Handler?.PlatformView;
        if (window == null)
            return null;
        var hwnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        folderPicker.FileTypeFilter.Add("*");

        var folder = await folderPicker.PickSingleFolderAsync();
        return folder?.Path;
    }

    public Task<bool> EnsureFolderAccessAsync(string folderPath, bool promptIfNeeded = true)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return Task.FromResult(false);

        return Task.FromResult(Directory.Exists(folderPath));
    }
}
#endif
