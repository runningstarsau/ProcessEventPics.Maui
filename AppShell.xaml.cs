using Microsoft.Maui.Controls;

namespace ProcessEventPics.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
		Routing.RegisterRoute("AboutPage", typeof(AboutPage));
	}
}
