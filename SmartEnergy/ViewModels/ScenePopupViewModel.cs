using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class ScenePopupViewModel : PopupViewModel
    {
        [ObservableProperty]
        private string _name;

        public ScenePopupViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
        }

        protected override bool Validate() => !string.IsNullOrEmpty(Name);
    }
}
