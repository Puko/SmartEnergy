using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartEnergy.ViewModels
{
    public partial class LoadingViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string _message;
    }
}
