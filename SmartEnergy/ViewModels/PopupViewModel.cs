using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class PopupViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public PopupViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public bool Cancelled { get; set; }

        [RelayCommand]
        public async Task CloseAsync(bool cancelled)
        {
            if (cancelled || Validate())
            {
                Cancelled = bool.Parse(cancelled.ToString());
                OnClosing();
                await _navigationService.ClosePopupAsync();
            }
        }

        protected virtual bool Validate() => true;
        protected virtual void OnClosing() { }
    }
}
