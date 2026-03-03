using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

namespace ProcessEventPics.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		// Enable ImageSharp's preference for contiguous image buffers to improve performance and reduce memory fragmentation
		SixLabors.ImageSharp.Configuration.Default.PreferContiguousImageBuffers = true;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Montserrat-Regular.ttf", "MontserratRegular");
				fonts.AddFont("Montserrat-Bold.ttf", "MontserratBold");
				fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
