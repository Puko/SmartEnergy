using SmartEnergy.ViewModels;

namespace SmartEnergy.Views;

public partial class MainView : Shell
{
    private BaseViewModel _baseViewModel;

    public MainView()
	 {
		  InitializeComponent();
	 }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        _baseViewModel = (BaseViewModel)BindingContext;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _baseViewModel.Shown();
        if (_baseViewModel.IsInitialized)
            return;

        _baseViewModel.IsInitialized = true;
        await _baseViewModel.InitializeAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await _baseViewModel.Disapear();
    }
}

