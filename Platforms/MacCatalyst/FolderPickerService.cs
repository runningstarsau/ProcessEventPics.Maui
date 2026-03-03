#if MACCATALYST
using Foundation;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;
using ProcessEventPics.Maui.Services;
using System.Security.Cryptography;
using System.Text;
using UIKit;
using UniformTypeIdentifiers;

namespace ProcessEventPics.Maui.Platforms.MacCatalyst;

public class FolderPickerService : IFolderPickerService
{
    private const string BookmarkKeyPrefix = "MacCatalystFolderBookmark_";
    private static readonly List<FolderPickerDelegate> ActiveDelegates = new();
    private static readonly Dictionary<string, NSUrl> ActiveSecurityScopedUrls = new(StringComparer.OrdinalIgnoreCase);

    public async Task<string?> PickFolderAsync()
    {
        var selectedUrl = await PickFolderUrlAsync();
        if (selectedUrl == null)
            return null;

        return ActivateAndPersistFolderUrl(selectedUrl);
    }

    public async Task<bool> EnsureFolderAccessAsync(string folderPath, bool promptIfNeeded = true)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return false;

        var normalizedPath = NormalizePath(folderPath);
        if (normalizedPath == null)
            return false;

        if (CanEnumerateFolder(normalizedPath))
            return true;

        if (TryRestoreAccessFromBookmark(normalizedPath))
            return true;

        if (!promptIfNeeded)
            return false;

        var selectedPath = await PickFolderAsync();
        if (string.IsNullOrWhiteSpace(selectedPath))
            return false;

        if (!PathsMatch(selectedPath, normalizedPath))
            return false;

        return CanEnumerateFolder(selectedPath);
    }

    private static Task<NSUrl?> PickFolderUrlAsync()
    {
        var tcs = new TaskCompletionSource<NSUrl?>();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                var presenter = GetTopViewController();
                if (presenter == null)
                {
                    tcs.TrySetResult(null);
                    return;
                }

                var picker = new UIDocumentPickerViewController(new[] { UTTypes.Folder })
                {
                    AllowsMultipleSelection = false
                };

                FolderPickerDelegate? pickerDelegate = null;
                pickerDelegate = new FolderPickerDelegate(tcs, () =>
                {
                    lock (ActiveDelegates)
                    {
                        ActiveDelegates.RemoveAll(x => ReferenceEquals(x, pickerDelegate));
                    }
                });

                lock (ActiveDelegates)
                {
                    ActiveDelegates.Add(pickerDelegate);
                }

                picker.Delegate = pickerDelegate;
                presenter.PresentViewController(picker, true, null);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }

    private static string? ActivateAndPersistFolderUrl(NSUrl folderUrl)
    {
        var folderPath = NormalizePath(folderUrl.Path);
        if (folderPath == null)
            return null;

        var started = folderUrl.StartAccessingSecurityScopedResource();
        if (!started && !CanEnumerateFolder(folderPath))
            return null;

        lock (ActiveSecurityScopedUrls)
        {
            ActiveSecurityScopedUrls[folderPath] = folderUrl;
        }

        PersistBookmark(folderPath, folderUrl);
        return folderPath;
    }

    private static bool TryRestoreAccessFromBookmark(string folderPath)
    {
        try
        {
            var key = GetBookmarkPreferenceKey(folderPath);
            var encodedBookmark = Preferences.Default.Get(key, string.Empty);
            if (string.IsNullOrWhiteSpace(encodedBookmark))
                return false;

            var bookmarkBytes = Convert.FromBase64String(encodedBookmark);
            using var bookmarkData = NSData.FromArray(bookmarkBytes);

            var resolvedUrl = NSUrl.FromBookmarkData(
                bookmarkData,
                0,
                null,
                out var isStale,
                out var error);

            if (resolvedUrl == null || error != null)
                return false;

            var resolvedPath = NormalizePath(resolvedUrl.Path) ?? folderPath;
            if (!PathsMatch(resolvedPath, folderPath))
                return false;

            var started = resolvedUrl.StartAccessingSecurityScopedResource();
            if (!started && !CanEnumerateFolder(resolvedPath))
                return false;

            lock (ActiveSecurityScopedUrls)
            {
                ActiveSecurityScopedUrls[resolvedPath] = resolvedUrl;
            }

            if (isStale)
            {
                PersistBookmark(resolvedPath, resolvedUrl);
            }

            return CanEnumerateFolder(resolvedPath);
        }
        catch
        {
            return false;
        }
    }

    private static void PersistBookmark(string folderPath, NSUrl folderUrl)
    {
        try
        {
            var bookmarkData = folderUrl.CreateBookmarkData(
                0,
                Array.Empty<string>(),
                null,
                out var error);

            if (bookmarkData == null || error != null)
                return;

            var key = GetBookmarkPreferenceKey(folderPath);
            Preferences.Default.Set(key, Convert.ToBase64String(bookmarkData.ToArray()));
        }
        catch
        {
            // Ignore bookmark persistence failures and continue with runtime access.
        }
    }

    private static string GetBookmarkPreferenceKey(string folderPath)
    {
        var normalizedPath = NormalizePath(folderPath) ?? folderPath;
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedPath));
        return BookmarkKeyPrefix + Convert.ToHexString(hash);
    }

    private static string? NormalizePath(string? folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return null;

        try
        {
            return Path.GetFullPath(folderPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        catch
        {
            return null;
        }
    }

    private static bool CanEnumerateFolder(string folderPath)
    {
        try
        {
            if (!Directory.Exists(folderPath))
                return false;

            _ = Directory.EnumerateFileSystemEntries(folderPath).FirstOrDefault();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
    }

    private static bool PathsMatch(string left, string right)
    {
        var leftPath = NormalizePath(left);
        var rightPath = NormalizePath(right);

        if (leftPath == null || rightPath == null)
            return false;

        return string.Equals(leftPath, rightPath, StringComparison.OrdinalIgnoreCase);
    }

    private static UIViewController? GetTopViewController()
    {
        var windowScene = UIApplication.SharedApplication.ConnectedScenes
            .OfType<UIWindowScene>()
            .FirstOrDefault(scene => scene.ActivationState == UISceneActivationState.ForegroundActive);

        var window = windowScene?.Windows.FirstOrDefault(w => w.IsKeyWindow)
                     ?? windowScene?.Windows.FirstOrDefault();

        var controller = window?.RootViewController;
        while (controller?.PresentedViewController != null)
        {
            controller = controller.PresentedViewController;
        }

        return controller;
    }

    private sealed class FolderPickerDelegate : UIDocumentPickerDelegate
    {
        private readonly TaskCompletionSource<NSUrl?> _tcs;
        private readonly Action _onComplete;

        public FolderPickerDelegate(TaskCompletionSource<NSUrl?> tcs, Action onComplete)
        {
            _tcs = tcs;
            _onComplete = onComplete;
        }

        public override void DidPickDocument(UIDocumentPickerViewController controller, NSUrl[] urls)
        {
            _tcs.TrySetResult(urls.FirstOrDefault());
            _onComplete();
        }

        public override void WasCancelled(UIDocumentPickerViewController controller)
        {
            _tcs.TrySetResult(null);
            _onComplete();
        }
    }
}
#endif
