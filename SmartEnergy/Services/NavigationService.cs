using CommunityToolkit.Mvvm.ComponentModel;
using Mopups.Pages;
using Mopups.Services;
using SmartEnergy.Interfaces;
using SmartEnergy.ViewModels;

namespace SmartEnergy.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> _registration = new Dictionary<Type, Type>();
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task GoBackAsync()
        {
            return Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task NavigateAsync<TViewModel>(Action<TViewModel> init = null, bool resetNavigation = false) where TViewModel : notnull, BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var keyValue = _registration.FirstOrDefault(x => x.Value == typeof(TViewModel));

            if (keyValue.Key == null)
            {
                throw new InvalidNavigationException("Missing navigation service registration ...");
            }

            var view = (Page)_serviceProvider.GetRequiredService(keyValue.Key);
            init?.Invoke(viewModel);
            view.BindingContext = viewModel;

            if (resetNavigation)
                if (view is Shell)
                    Application.Current.MainPage = view;
                else
                    Application.Current.MainPage = new NavigationPage(view);
            else
                await Application.Current.MainPage.Navigation.PushAsync(view);

        }

        public async Task<TViewModel> ShowPopupAsync<TViewModel>(Action<TViewModel> init = null) where TViewModel : notnull, BaseViewModel
        {
            TaskCompletionSource<TViewModel> result = new TaskCompletionSource<TViewModel>();

            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var keyValue = _registration.FirstOrDefault(x => x.Value == typeof(TViewModel));

            if (keyValue.Key == null)
            {
                throw new InvalidNavigationException("Missing navigation service registration ...");
            }

            var view = (PopupPage)_serviceProvider.GetRequiredService(keyValue.Key);
            init?.Invoke(viewModel);
            view.BindingContext = viewModel;

            EventHandler dissapearing = null;
            dissapearing = (s, e) =>
            {
                view.Disappearing -= dissapearing;
                result.TrySetResult(viewModel);
            };

            view.Disappearing += dissapearing;

            await MopupService.Instance.PushAsync(view);
            return await result.Task;
        }

        public async Task ShowPopupAsyncWithoutResult<TViewModel>(Action<TViewModel> init) where TViewModel : notnull, BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var keyValue = _registration.FirstOrDefault(x => x.Value == typeof(TViewModel));

            if (keyValue.Key == null)
            {
                throw new InvalidNavigationException("Missing navigation service registration ...");
            }

            var view = (PopupPage)_serviceProvider.GetRequiredService(keyValue.Key);
            init.Invoke(viewModel);
            view.BindingContext = viewModel;
            await MopupService.Instance.PushAsync(view);
        }

        public async Task ShowPopupAsync<TViewModel>(TViewModel instance) where TViewModel : notnull, BaseViewModel
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();

            var keyValue = _registration.FirstOrDefault(x => x.Value == instance.GetType());

            if (keyValue.Key == null)
            {
                throw new InvalidNavigationException("Missing navigation service registration ...");
            }

            var view = (PopupPage)_serviceProvider.GetRequiredService(keyValue.Key);
            view.BindingContext = instance;
            EventHandler dissapearing = null;
            dissapearing = (s, e) =>
                {
                    view.Disappearing -= dissapearing;
                    result.TrySetResult(true);
                };

            view.Disappearing += dissapearing;

            await MopupService.Instance.PushAsync(view);
            await result.Task;
        }

        public async Task ClosePopupAsync()
        {
            await MopupService.Instance.PopAsync();
        }

        public void RegisterView<TView, TViewModel>() where TViewModel : notnull, BaseViewModel
        {
            _registration.Add(typeof(TView), typeof(TViewModel));
        }
    }
}
