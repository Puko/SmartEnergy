using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.Database.Models;

namespace SmartEnergy.ViewModels
{
    public partial class SceneListItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _name;       
        
        [ObservableProperty]
        private bool _selected;
        
        public SceneListItemViewModel(Scene scene)
        {
            Scene = scene;
            Name = scene.Name;
        }

        public Scene Scene { get; }
    }
}
