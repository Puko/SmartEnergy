using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class InfoViewModel : PopupViewModel
    {
        [ObservableProperty]
        private string _message;

        public InfoViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
        }

        public bool IsConfirmation { get; set; }
    }
}
