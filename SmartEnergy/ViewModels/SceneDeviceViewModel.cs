using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Interfaces;
namespace SmartEnergy.ViewModels
{
    public partial class SceneDeviceViewModel : PopupViewModel
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private List<Database.Models.Device> _datas;    
        
        [ObservableProperty]
        private Database.Models.Device _selectedDevice;

        public SceneDeviceViewModel(INavigationService navigationService) 
            : base(navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        public async Task SelectDeviceAsync()
        {
            await _navigationService.ClosePopupAsync();
        }
    }
}
