using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Database.Models;

namespace SmartEnergy.ViewModels
{
    public partial class SceneDeviceItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isOnline;

        public SceneDeviceItemViewModel(SceneDevice device)
        {
            Device = device;
        }

        public SceneDevice Device { get; }
    }
}
