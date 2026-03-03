namespace ProcessEventPics.Maui.Services;

public interface IFolderPickerService
{
    Task<string?> PickFolderAsync();
    Task<bool> EnsureFolderAccessAsync(string folderPath, bool promptIfNeeded = true);
}
