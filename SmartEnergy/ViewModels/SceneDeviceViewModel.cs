using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SmartEnergy.Api;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Models;
using SmartEnergy.Services;

namespace SmartEnergy.ViewModels
{
    public partial class SceneDeviceViewModel : PopupViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly SmartEnergyApiService _smartEnergyApi;
        private List<Database.Models.Device> _devices;

        [ObservableProperty]
        private List<SelectDataViewModel> _list;    
        
        [ObservableProperty]
        private SelectDataViewModel _selectedData;

        [ObservableProperty]
        private bool _isBusy;
        
        public SceneDeviceViewModel(INavigationService navigationService, SmartEnergyApiService smartEnergyApi) 
            : base(navigationService)
        {
            _navigationService = navigationService;
            _smartEnergyApi = smartEnergyApi;
        }

        public Database.Models.Device Device { get; private set; }
        public string Relay { get; private set; }
        public int RelayOrder { get; private set; }

        public void SetData(List<Database.Models.Device> data)
        {
            _devices = data;
            
            List = data.Select(x => new SelectDataViewModel { Id = x.Id, Value = x.Mac }).ToList();
        }

        [RelayCommand]
        public async Task SelectDeviceAsync()
        {
            if (SelectedData?.IsRelay == false)
            {
                Device = _devices.FirstOrDefault(x => x.Id == SelectedData.Id);
                ApiResult<RelaySettingsResponse> settings;
                IsBusy = true;

                try
                {
                    settings = await _smartEnergyApi.GetDeviceRelaySettings(Device.Token);
                    if (!settings.Succes)
                    {
                        await _navigationService.ShowPopupAsync<InfoViewModel>(x =>
                            x.Message = $"{settings.Response?.Message}\n Server response code: {settings.Response?.Code}");
                        return;
                    }

                    List<SelectDataViewModel> relays = new List<SelectDataViewModel>
                    {
                        new SelectDataViewModel
                        {
                            Id = 1,
                            Value = settings.Value.Settings.R1.Name,
                            IsRelay = true
                        },
                        new SelectDataViewModel
                        {
                            Id = 2,
                            Value = settings.Value.Settings.R2.Name,
                            IsRelay = true
                        },
                        new SelectDataViewModel
                        {
                            Id = 3,
                            Value = settings.Value.Settings.R3.Name,
                            IsRelay = true
                        },
                        new SelectDataViewModel
                        {
                            Id = 4,
                            Value = settings.Value.Settings.R4.Name,
                            IsRelay = true
                        },
                    };

                    List = new List<SelectDataViewModel>(relays);
                }
                catch (Exception e)
                {
                    await _navigationService.ShowPopupAsync<InfoViewModel>(x =>
                        x.Message = "Something went wrong. Please check logs.");
                }
                finally
                {
                    IsBusy = false;
                }

                SelectedData = null;
            }
            else
            {
                Relay = SelectedData.Value;
                RelayOrder = SelectedData.Id;

                await _navigationService.ClosePopupAsync();
            }
        }
    }
}
