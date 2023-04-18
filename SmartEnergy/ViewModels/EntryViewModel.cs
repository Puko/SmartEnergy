using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class EntryViewModel : PopupViewModel
    {
        [ObservableProperty]
        private string _name;    
        
        [ObservableProperty]
        private string _title;

        public EntryViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
        }

        protected override bool Validate() => !string.IsNullOrEmpty(Name);
    }
}
