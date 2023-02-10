using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class MessagePopupViewModel : PopupViewModel
    {
        [ObservableProperty]
        private string _message;

        public MessagePopupViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
        }

        public bool IsConfirmation { get; set; }
    }
}
