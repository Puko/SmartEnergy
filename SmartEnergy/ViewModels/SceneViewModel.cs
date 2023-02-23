using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Database.Models;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using System.Collections.ObjectModel;

namespace SmartEnergy.ViewModels
{
    public partial class SceneViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly UserService _userService;
        private readonly SceneService _sceneService;

        [ObservableProperty]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();

        public SceneViewModel(INavigationService navigationService, UserService userService,
            SceneService sceneService)
        {
            _navigationService = navigationService;
            _userService = userService;
            _sceneService = sceneService;
        }

        public override Task InitializeAsync()
        {
            Scenes = new ObservableCollection<Scene>(_sceneService.GetScenes().ToList());

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task AddSceneAsync()
        {
            await _navigationService.NavigateAsync<AddEditSceneViewModel>(x => x.Saved = async () => await InitializeAsync());
        }

        [RelayCommand]
        public async Task SelectSceneAsync(Scene scene)
        {
            await _navigationService.NavigateAsync<AddEditSceneViewModel>(x =>
            {
                x.SceneId = scene.Id;
                x.Saved = async () => await InitializeAsync();
            });
        }

        [RelayCommand]
        public async Task DeleteSceneAsync(Scene scene)
        {
            var vm = await _navigationService.ShowPopupAsync<MessagePopupViewModel>((Action<MessagePopupViewModel>)(x =>
            {
                x.Message = string.Format(Localization["DeleteSceneMessage"].ToString(), scene.Name);
                x.IsConfirmation = true;
            }));

            if (!vm.Cancelled)
            {
                _sceneService.Delete(scene);
                Scenes.Remove(scene);
            }
        }
    }
}
