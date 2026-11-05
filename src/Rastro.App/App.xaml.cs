using Microsoft.Extensions.DependencyInjection;

using Rastro.App.Services;
using Rastro.App.Views;

namespace Rastro.App;

public partial class App : Application
{
	private readonly IServiceProvider _services;
	private readonly TrackingNavigation _navigation;
	public App(IServiceProvider services, TrackingNavigation navigation)
	{
		InitializeComponent();
		UserAppTheme = AppTheme.Light;
		_services = services;
		_navigation = navigation;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var root = new NavigationPage(_services.GetRequiredService<HomePage>()) { BarBackgroundColor = Color.FromArgb("#F7F8FA") };
		_navigation.Navigator = root.Navigation;
		return new Window(root) { Title = "Rastro · cada etapa, mais perto", Width = 1280, Height = 800, MinimumWidth = 360, MinimumHeight = 640 };
	}
}
