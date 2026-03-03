using Microsoft.Maui.Controls;
using ProcessEventPics.Maui.ViewModels;

namespace ProcessEventPics.Maui;

public partial class MainPage : ContentPage
{
	private MainViewModel? _viewModel;

	public MainPage()
	{
		InitializeComponent();
		_viewModel = new MainViewModel();
		_viewModel.SetPage(this);
		BindingContext = _viewModel;
	}
}
