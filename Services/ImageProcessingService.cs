using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using ImageSharpImage = SixLabors.ImageSharp.Image;

namespace ProcessEventPics.Maui.Services;

public class ImageProcessingService
{
    private const int MaxImageDimension = 2048;
    private const int JpegQuality = 85;

    public record ProcessingResult(int Successful, int Failed, List<string> Errors);

    private static bool TryCreateDestinationDirectory(string destinationFolder, out string? error)
    {
        try
        {
            Directory.CreateDirectory(destinationFolder);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            error = $"Unable to create destination folder '{destinationFolder}': {ex.Message}";
            return false;
        }
    }

    private static ImageSharpImage LoadImageForProcessing(string sourceFile)
    {
        return ImageSharpImage.Load(sourceFile);
    }

    public async Task<(string[] jpgFiles, string[] rawFiles)> GetFilesAsync(string sourceFolder)
    {
        return await Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(sourceFolder) || !Directory.Exists(sourceFolder))
            {
                return (Array.Empty<string>(), Array.Empty<string>());
            }

            try
            {
                var jpgFiles = Directory.GetFiles(sourceFolder, "*.jpg", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(sourceFolder, "*.JPG", SearchOption.AllDirectories))
                    .Distinct()
                    .OrderBy(f => Path.GetDirectoryName(f))
                    .ThenBy(f => Path.GetFileName(f))
                    .ToArray();

                var rawFiles = Directory.GetFiles(sourceFolder, "*.nef", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(sourceFolder, "*.NEF", SearchOption.AllDirectories))
                    .Distinct()
                    .OrderBy(f => Path.GetDirectoryName(f))
                    .ThenBy(f => Path.GetFileName(f))
                    .ToArray();

                return (jpgFiles, rawFiles);
            }
            catch (UnauthorizedAccessException)
            {
                return (Array.Empty<string>(), Array.Empty<string>());
            }
            catch (IOException)
            {
                return (Array.Empty<string>(), Array.Empty<string>());
            }
        });
    }

    public async Task<string?> FindCameraAsync()
    {
        return await Task.Run(() =>
        {
            if (OperatingSystem.IsMacCatalyst())
            {
                return null;
            }

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady && drive.DriveType == DriveType.Removable)
                {
                    string potentialPath = Path.Combine(drive.Name, "DCIM");
                    if (Directory.Exists(potentialPath))
                    {
                        return potentialPath;
                    }
                }
            }
            return null;
        });
    }

    public async Task<ProcessingResult> CopyFilesAsync(
        string[] sourceFiles,
        string destinationFolder,
        string fileNamePrefix,
        IProgress<(int current, int total, string fileName)> progress,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            if (!TryCreateDestinationDirectory(destinationFolder, out var directoryError))
            {
                return new ProcessingResult(0, sourceFiles.Length, new List<string> { directoryError! });
            }

            int successCount = 0;
            int errorCount = 0;
            List<string> errors = new();

            for (int i = 0; i < sourceFiles.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string sourceFile = sourceFiles[i];
                try
                {
                    string extension = Path.GetExtension(sourceFile).ToLower();
                    string newFileName = $"{fileNamePrefix}{(i + 1).ToString().PadLeft(4, '0')}{extension}";
                    string destFile = Path.Combine(destinationFolder, newFileName);

                    File.Copy(sourceFile, destFile, overwrite: true);
                    progress.Report((i + 1, sourceFiles.Length, newFileName));
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(sourceFile)}: {ex.Message}");
                    errorCount++;
                }
            }

            return new ProcessingResult(successCount, errorCount, errors);
        }, cancellationToken);
    }

    public async Task<ProcessingResult> ResizeAndCompressAsync(
        string sourceFolder,
        string destinationFolder,
        IProgress<(int current, int total, string fileName, long originalSize, long newSize)> progress,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var files = Directory.GetFiles(sourceFolder, "*.jpg", SearchOption.TopDirectoryOnly)
                .Concat(Directory.GetFiles(sourceFolder, "*.JPG", SearchOption.TopDirectoryOnly))
                .Distinct()
                .ToArray();

            if (!TryCreateDestinationDirectory(destinationFolder, out var directoryError))
            {
                return new ProcessingResult(0, files.Length, new List<string> { directoryError! });
            }

            int successCount = 0;
            int errorCount = 0;
            List<string> errors = new();

            for (int i = 0; i < files.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                string sourceFile = files[i];
                try
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string destFile = Path.Combine(destinationFolder, fileName);

                    using (var image = LoadImageForProcessing(sourceFile))
                    {
                        int originalWidth = image.Width;
                        int originalHeight = image.Height;

                        image.Metadata.ExifProfile = null;

                        int newWidth, newHeight;
                        if (originalWidth > originalHeight)
                        {
                            if (originalWidth > MaxImageDimension)
                            {
                                newWidth = MaxImageDimension;
                                newHeight = (int)((double)originalHeight / originalWidth * MaxImageDimension);
                            }
                            else
                            {
                                newWidth = originalWidth;
                                newHeight = originalHeight;
                            }
                        }
                        else
                        {
                            if (originalHeight > MaxImageDimension)
                            {
                                newHeight = MaxImageDimension;
                                newWidth = (int)((double)originalWidth / originalHeight * MaxImageDimension);
                            }
                            else
                            {
                                newWidth = originalWidth;
                                newHeight = originalHeight;
                            }
                        }

                        image.Mutate(x => x.Resize(newWidth, newHeight));

                        var encoder = new JpegEncoder { Quality = JpegQuality };
                        image.Save(destFile, encoder);
                    }

                    FileInfo originalFileInfo = new FileInfo(sourceFile);
                    FileInfo compressedFileInfo = new FileInfo(destFile);

                    progress.Report((i + 1, files.Length, fileName, originalFileInfo.Length, compressedFileInfo.Length));
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(sourceFile)}: {ex.Message}");
                    errorCount++;
                }
            }

            return new ProcessingResult(successCount, errorCount, errors);
        }, cancellationToken);
    }

    public void OpenFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;

        try
        {
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            {
                System.Diagnostics.Process.Start("open", folderPath);
            }
            else if (OperatingSystem.IsLinux())
            {
                System.Diagnostics.Process.Start("xdg-open", folderPath);
            }
        }
        catch
        {
            // Silently fail if can't open folder
        }
    }
}
