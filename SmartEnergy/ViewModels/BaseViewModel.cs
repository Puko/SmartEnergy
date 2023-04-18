using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Interfaces;
using SmartEnergy.Localization;

namespace SmartEnergy.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        public bool IsInitialized { get; set; }

        public virtual Task InitializeAsync() => Task.CompletedTask;
        public virtual ValueTask Disapear() => ValueTask.CompletedTask;
        public virtual Task Shown() => Task.CompletedTask;

        public LocalizationResourceManager Localization => LocalizationResourceManager.Instance;

        protected async Task<T> Execute<T>(Func<Task<T>> action)
        {
            IsBusy = true;

            var result = await action();

            IsBusy = false;

            return result;
        }

        protected async Task<bool> CheckConnection(INavigationService navigationService)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = "You're offline. Check internet connection.");
                return false;
            }

            return true;
        }
    }
}
