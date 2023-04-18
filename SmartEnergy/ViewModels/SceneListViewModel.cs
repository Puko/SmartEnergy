using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using System.Collections.ObjectModel;
using System.Text.Json;
using Newtonsoft.Json;
using SmartEnergy.Database.Models;

namespace SmartEnergy.ViewModels
{
    public partial class SceneListViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ILogService _logService;
        private readonly SceneService _sceneService;

        [ObservableProperty]
        private ObservableCollection<SceneListItemViewModel> _scenes = new ObservableCollection<SceneListItemViewModel>();

        public SceneListViewModel(INavigationService navigationService, ILogService logService, SceneService sceneService)
        {
            _navigationService = navigationService;
            _logService = logService;
            _sceneService = sceneService;
        }

        public override Task InitializeAsync()
        {
            Scenes = new ObservableCollection<SceneListItemViewModel>(_sceneService.GetScenes().Select(x => new SceneListItemViewModel(x)).ToList());

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task ExportScenesAsync()
        {
            List<Scene> toExport = new List<Scene>();
            var selected = Scenes.Where(x => x.Selected).Select(x => x.Scene).ToList();
            if(!selected.Any())
                return;

            string file = null;
            bool success = false;
            await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = $"Exporting scenes...");
            await Task.Delay(1000);

            try
            {
                foreach (var scene in selected)
                {
                    var s = _sceneService.GetSceneById(scene.Id);
                    s.Id = 0;
                    if (s.Devices.Any())
                        s.Devices.ForEach(x => x.Id = 0);

                    toExport.Add(s);
                }

                var toFile = JsonConvert.SerializeObject(toExport, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                string fn = $"export_{DateTime.Now:yy_MM_dd_mm_ss}.json";
                file = Path.Combine(FileSystem.CacheDirectory, fn);

                await File.WriteAllTextAsync(file, toFile);
                
                success = true;
            }
            catch (Exception e)
            {
                _logService.Exception(e, "Export scenes failed.");
            }
            finally
            {
                await _navigationService.ClosePopupAsync();
            }

            if (success)
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Share exported scenes",
                    File = new ShareFile(file)
                });

                foreach (var scene in Scenes)
                {
                    scene.Selected = false;
                }
            }
            else
            {
                await _navigationService.ShowPopupAsync<InfoViewModel>(x =>
                    x.Message = Localization["ExportFailed"].ToString());
            }
        }

        [RelayCommand]
        public async Task ImportScenesAsync()
        {
            bool success = false;
            var file = await FilePicker.Default.PickAsync();
            if (file != null)
            {
                await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = $"Importing scenes...");
                await Task.Delay(1000);

                try
                {
                    var text = await File.ReadAllTextAsync(file.FullPath);
                    var scenes = JsonConvert.DeserializeObject<List<Scene>>(text);

                    foreach (var scene in scenes)
                    {
                        scene.Id = 0;
                        if (scene.Devices.Any())
                        {
                            scene.Devices.ForEach(x => x.Id = 0);
                            scene.DevicesCount = scene.Devices.Count;
                        }
                        
                        _sceneService.Add(scene);
                        Scenes.Add(new SceneListItemViewModel(scene));
                    }

                    success = true;
                }
                catch (Exception e)
                {
                    _logService.Exception(e, "Import scenes failed.");
                }
                finally
                {
                    await _navigationService.ClosePopupAsync();
                }

                if (!success)
                {
                    await _navigationService.ShowPopupAsync<InfoViewModel>(x =>
                        x.Message = Localization["ImportFailed"].ToString());
                }
            }
        }

        [RelayCommand]
        public async Task AddSceneAsync()
        {
            await _navigationService.NavigateAsync<AddEditSceneViewModel>(x => x.Saved = async () => await InitializeAsync());
        }

        [RelayCommand]
        public async Task EditSceneNameAsync(SceneListItemViewModel scene)
        {
            EntryViewModel vm = new EntryViewModel(_navigationService)
            {
                Name = scene.Name,
                Title = Localization["SceneName"].ToString()
            };

            await _navigationService.ShowPopupAsync(vm);

            if (!vm.Cancelled)
            {
                scene.Name = vm.Name;
                scene.Scene.Name = vm.Name;
                _sceneService.Update(scene.Scene);
            }
        }

        [RelayCommand]
        public async Task SelectSceneAsync(SceneListItemViewModel scene)
        {
            await _navigationService.NavigateAsync<AddEditSceneViewModel>(x =>
            {
                x.SceneId = scene.Scene.Id;
                x.Saved = async () => await InitializeAsync();
            });
        }

        [RelayCommand]
        public async Task DeleteSceneAsync(SceneListItemViewModel scene)
        {
            var vm = await _navigationService.ShowPopupAsync<InfoViewModel>((Action<InfoViewModel>)(x =>
            {
                x.Message = string.Format(Localization["DeleteSceneMessage"].ToString(), scene.Name);
                x.IsConfirmation = true;
            }));

            if (!vm.Cancelled)
            {
                _sceneService.Delete(scene.Scene);
                Scenes.Remove(scene);
            }
        }
    }
}
