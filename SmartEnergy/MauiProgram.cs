using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Mopups.Hosting;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Database;
using SmartEnergy.Database.Repositories;
using SmartEnergy.Interfaces;
using SmartEnergy.Localization;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;
using SmartEnergy.Views;
using AddSceneDeviceViewModel = SmartEnergy.Views.AddSceneDeviceViewModel;

namespace SmartEnergy;

public static class MauiProgram
{
	 public static MauiApp CreateMauiApp()
	 {
		  var builder = MauiApp.CreateBuilder();

#if __ANDROID__
            SmartEnergy.Platforms.Android.AndroidHandlers.Init();
#endif

#if __ANDROID__
        ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => PrependToMappingImageSource(handler, view));
#endif

        builder
           .UseMauiApp<App>()
			  .UseMauiCommunityToolkitCore()
			  .UseMauiCommunityToolkit()
			  .ConfigureFonts(fonts =>
			  {
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					fonts.AddFont("FFMarkPro.Regular.otf", "FFMarkProRegular");
					fonts.AddFont("FFMarkPro.Light.otf", "FFMarkProLight");
					fonts.AddFont("FFMarkPro.Bold.otf", "FFMarkProBold");
					fonts.AddFont("materialdesignicons-webfont.ttf", "MaterialFont");
			  })
			 .ConfigureMopups();

#if DEBUG
        builder.Logging.AddDebug();
#endif



        builder.Services.AddSingleton<SceneRepository>();
		  builder.Services.AddSingleton<UserInformationRepository>();
		  builder.Services.AddSingleton<SceneDeviceRepository>();

		  builder.Services.AddSingleton<UserService>();
		  builder.Services.AddSingleton<SceneService>();
		  builder.Services.AddSingleton<SceneDeviceService>();
		  builder.Services.AddSingleton<SmartEnergyApiService>();
		  builder.Services.AddSingleton(s =>
		  {
				var ws = new WebsocketClient(new Uri("wss://backend.merito.tech/api2-ws"));
			 return ws;
		  });

		  builder.Services.AddTransient<AddEditSceneViewModel>();
		  builder.Services.AddTransient<SceneListViewModel>();
		  builder.Services.AddTransient<LoginViewModel>();
		  builder.Services.AddTransient<SceneListViewModel>();
		  builder.Services.AddTransient<SceneDeviceViewModel>();
		  builder.Services.AddTransient<ViewModels.InfoViewModel>();
		  builder.Services.AddTransient<LogsViewModel>();
		  builder.Services.AddTransient<MainViewModel>();
		  builder.Services.AddTransient<LoadingViewModel>();
		  builder.Services.AddTransient<StateViewModel>();

		  builder.Services.AddTransient<AddEditSceneView>();
		  builder.Services.AddTransient<MainView>();
		  builder.Services.AddTransient<LoginView>();
		  builder.Services.AddTransient<EntryView>();
		  builder.Services.AddTransient<AddSceneDeviceViewModel>();
		  builder.Services.AddTransient<InfoView>();
		  builder.Services.AddTransient<LogsView>();
		  builder.Services.AddTransient<LoadingPopupView>();
		  builder.Services.AddTransient<StateView>();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, @"SmartEnergy.db");
        builder.Services.AddDbContext<SmartEnergyDb>(options => options.UseSqlite($"Data Source={dbPath}"));

		  builder.Services.AddSingleton<INavigationService, NavigationService>();
		  builder.Services.AddSingleton<ILogService, LogService>();
		  builder.Services.AddSingleton<LocalizationResourceManager>(s =>
		  {
				return LocalizationResourceManager.Instance;
        });

        var app = builder.Build();

		  var navigationService = app.Services.GetRequiredService<INavigationService>();
        navigationService.RegisterView<MainView, MainViewModel>();
        navigationService.RegisterView<AddEditSceneView, AddEditSceneViewModel>();
        navigationService.RegisterView<LoginView, LoginViewModel>();
        navigationService.RegisterView<EntryView, EntryViewModel>();
        navigationService.RegisterView<AddSceneDeviceViewModel, SceneDeviceViewModel>();
        navigationService.RegisterView<InfoView, ViewModels.InfoViewModel>();
        navigationService.RegisterView<LoadingPopupView, LoadingViewModel>();
        navigationService.RegisterView<StateView, StateViewModel>();

		  var db = app.Services.GetRequiredService<SmartEnergyDb>();
		  db.Database.EnsureCreated();

		  return app;
	 }

#if __ANDROID__
    public static void PrependToMappingImageSource(IImageHandler handler, Microsoft.Maui.IImage image)
    {
        handler.PlatformView?.Clear();
    }
#endif
}
