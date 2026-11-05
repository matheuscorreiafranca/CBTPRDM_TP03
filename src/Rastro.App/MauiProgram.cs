using Microsoft.Extensions.Logging;

using Rastro.App.Services;
using Rastro.App.Views;
using Rastro.Core.Services;
using Rastro.Core.ViewModels;

namespace Rastro.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Manrope.ttf", "Manrope");
			});
		builder.Services.AddSingleton<ITrackingService, DemoTrackingService>();
		builder.Services.AddSingleton<ITrackingStore>(_ => new JsonTrackingStore(Path.Combine(FileSystem.AppDataDirectory, "tracking.json")));
		builder.Services.AddSingleton<TrackingNavigation>();
		builder.Services.AddSingleton<ITrackingNavigation>(sp => sp.GetRequiredService<TrackingNavigation>());
		builder.Services.AddSingleton<TrackingViewModel>();
		builder.Services.AddSingleton<HomePage>();
		builder.Services.AddTransient<ResultsPage>();
#if WINDOWS
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CleanEntry", (handler, _) =>
		{
			handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
			handler.PlatformView.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
			handler.PlatformView.Resources["TextControlBorderThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
		});
#elif ANDROID
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CleanEntry", (handler, _) =>
			handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent));
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
