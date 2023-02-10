using CommunityToolkit.Mvvm.ComponentModel;
using SmartEnergy.ViewModels;
using SmartEnergy.Views;

namespace SmartEnergy.Interfaces
{
    public interface INavigationService
    {
        void RegisterView<TView, TViewModel>() where TViewModel : BaseViewModel;
        Task NavigateAsync<TViewModel>(Action<TViewModel> init = null, bool resetNavigation = false) where TViewModel : notnull, BaseViewModel;
        Task<TViewModel> ShowPopupAsync<TViewModel>(Action<TViewModel> init = null) where TViewModel : notnull, BaseViewModel;
        Task ShowPopupAsync<TViewModel>(TViewModel instance) where TViewModel : notnull, BaseViewModel;
        Task ShowPopupAsyncWithoutResult<TViewModel>(Action<TViewModel> init) where TViewModel : notnull, BaseViewModel;
        Task GoBackAsync();
        Task ClosePopupAsync();
    }
}
