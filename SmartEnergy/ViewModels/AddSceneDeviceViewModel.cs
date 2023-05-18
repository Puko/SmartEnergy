using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Database.Models;

namespace SmartEnergy.ViewModels
{
    public partial class AddSceneDeviceViewModel : ObservableObject
    {
        private bool? _isOnline;    
        
        [ObservableProperty]
        private string _name;

        public AddSceneDeviceViewModel(SceneDevice device)
        {
            Device = device;
            Name = device.Relay;
        }

        public SceneDevice Device { get; }

        public bool? IsOnline
        {
            get => _isOnline;
            set
            {
                _isOnline = value;
                OnPropertyChanged();
            }
        }
    }
}
