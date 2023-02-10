using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Database;
using SmartEnergy.Database.Repositories;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;
using SmartEnergy.Views;

namespace SmartEnergy;

public static class MauiProgram
{
	 public static MauiApp CreateMauiApp()
	 {
		  var builder = MauiApp.CreateBuilder();

#if __ANDROID__
            SmartEnergy.Platforms.Android.AndroidHandlers.Init();
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
		  builder.Services.AddSingleton<SmartEnergyApiService>();
		  builder.Services.AddSingleton(s =>
		  {
				var ws = new WebsocketClient(new Uri("wss://backend.merito.tech/api2-ws"));
            return ws;
		  });

		  builder.Services.AddTransient<AddEditSceneViewModel>();
		  builder.Services.AddTransient<SceneViewModel>();
		  builder.Services.AddTransient<LoginViewModel>();
		  builder.Services.AddTransient<SceneViewModel>();
		  builder.Services.AddTransient<SceneDeviceViewModel>();
		  builder.Services.AddTransient<MessagePopupViewModel>();
		  builder.Services.AddTransient<SettingsDeviceViewModel>();
		  builder.Services.AddTransient<LogsViewModel>();
		  builder.Services.AddTransient<MainViewModel>();
		  builder.Services.AddTransient<LoadingViewModel>();

		  builder.Services.AddTransient<AddEditSceneView>();
		  builder.Services.AddTransient<MainView>();
		  builder.Services.AddTransient<LoginView>();
		  builder.Services.AddTransient<ScenePopupView>();
		  builder.Services.AddTransient<SceneDevicePopupView>();
		  builder.Services.AddTransient<MessagePopupView>();
		  builder.Services.AddTransient<SettingsDevicePopupView>();
		  builder.Services.AddTransient<LogsView>();
		  builder.Services.AddTransient<LoadingPopupView>();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, @"SmartEnergy.db");
        builder.Services.AddDbContext<SmartEnergyDb>(options => options.UseSqlite($"Data Source={dbPath}"));

		  builder.Services.AddSingleton<INavigationService, NavigationService>();
		  builder.Services.AddSingleton<ILogService, LogService>();

        var app = builder.Build();

		  var navigationService = app.Services.GetRequiredService<INavigationService>();
        navigationService.RegisterView<MainView, MainViewModel>();
        navigationService.RegisterView<AddEditSceneView, AddEditSceneViewModel>();
        navigationService.RegisterView<LoginView, LoginViewModel>();
        navigationService.RegisterView<ScenePopupView, ScenePopupViewModel>();
        navigationService.RegisterView<SceneDevicePopupView, SceneDeviceViewModel>();
        navigationService.RegisterView<MessagePopupView, MessagePopupViewModel>();
        navigationService.RegisterView<SettingsDevicePopupView, SettingsDeviceViewModel>();
        navigationService.RegisterView<LoadingPopupView, LoadingViewModel>();

		  var db = app.Services.GetRequiredService<SmartEnergyDb>();
		  db.Database.EnsureCreated();

		  return app;
	 }
}
