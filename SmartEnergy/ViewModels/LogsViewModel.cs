using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Interfaces;

namespace SmartEnergy.ViewModels
{
    public partial class LogsViewModel : BaseViewModel
    {
        private readonly ILogService _logService;

        [ObservableProperty]
        private string _logs;

        public LogsViewModel(ILogService logService)
        {
            _logService = logService;
        }

        public override Task InitializeAsync()
        {
            Logs = string.Empty;

            foreach (var item in _logService.GetLogs())
            {
                Logs += $"{item}\n\n";
            } 

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            await InitializeAsync();
        }
    }
}
