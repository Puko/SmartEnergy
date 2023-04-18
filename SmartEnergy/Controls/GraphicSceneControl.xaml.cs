using SmartEnergy.Database.Models;
using SmartEnergy.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
#if ANDROID
using SmartEnergy.Platforms.Android;
#endif

namespace SmartEnergy.Controls;

public partial class GraphicSceneControl : ScrollView
{
    public static readonly BindableProperty DevicesProperty =
        BindableProperty.Create(nameof(Devices), typeof(ObservableCollection<AddSceneDeviceViewModel>), typeof(GraphicSceneControl),
            new ObservableCollection<AddSceneDeviceViewModel>(), propertyChanged: HandleOnDevicesChanged);

    public static readonly BindableProperty SceneProperty =
        BindableProperty.Create(nameof(Scene), typeof(Scene), typeof(GraphicSceneControl), null);

    public static readonly BindableProperty EditDeviceCommandProperty =
     BindableProperty.Create(nameof(EditDeviceCommand), typeof(ICommand), typeof(GraphicSceneControl), null);


    private ControlButton _selected;

    public GraphicSceneControl()
    {
        InitializeComponent();
    }
    
    public ICommand EditDeviceCommand
    {
        get => (ICommand)GetValue(EditDeviceCommandProperty);
        set => SetValue(EditDeviceCommandProperty, value);
    }

    public Scene Scene
    {
        get => (Scene)GetValue(SceneProperty);
        set => SetValue(SceneProperty, value);
    }

    public ObservableCollection<AddSceneDeviceViewModel> Devices
    {
        get => (ObservableCollection<AddSceneDeviceViewModel>)GetValue(DevicesProperty);
        set => SetValue(DevicesProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        var vm = (AddEditSceneViewModel)BindingContext;
        vm.PropertyChanged -= Vm_PropertyChanged;
        vm.PropertyChanged += Vm_PropertyChanged;
    }
    
    private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            var sd = (AddSceneDeviceViewModel)e.NewItems[0];

            ControlButton btn = CreateButton(sd);
            btn.Move(ScrollX + btn.Width + Width / 2, ScrollY + btn.Height + Height / 2);
            Grid.Children.Add(btn);

            _selected = btn;
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            var sd = (AddSceneDeviceViewModel)e.OldItems[0];

            var ctrlBtn = Grid.Children.Cast<ControlButton>().FirstOrDefault(x => x.Device == sd);
            if (ctrlBtn != null)
            {
                Grid.Children.Remove(ctrlBtn);
            }
        }
    }
    
    private ControlButton CreateButton(AddSceneDeviceViewModel sd)
    {
        var btn = ControlButton.Create(sd, Scene.OriginalWidth, Scene.OriginalHeight);
        btn.Selected += Btn_Selected;
        btn.Edit += Btn_Edit;
        return btn;
    }

    private void Btn_Selected(object sender, EventArgs e)
    {
        _selected = (ControlButton)sender;
    }

    private void Btn_Edit(object sender, EventArgs e)
    {
        var sc = ((ControlButton)sender).Device;
        EditDeviceCommand?.Execute(sc);
    }

    private void InitCollection(ObservableCollection<AddSceneDeviceViewModel> devices)
    {
        devices.CollectionChanged += Devices_CollectionChanged;
        foreach (var item in devices)
        {
            var btn = CreateButton(item);
            Grid.Children.Add(btn);
        }
    }

    private static void HandleOnDevicesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is ObservableCollection<AddSceneDeviceViewModel> devices)
        {
            ((GraphicSceneControl)bindable).InitCollection(devices);
        }
    }

    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(AddEditSceneViewModel.SceneImage))
        {
            var imageToRemove = MainGrid.Children.FirstOrDefault(x => x is Image);
            if(imageToRemove != null) 
            { 
                MainGrid.Children.Remove(imageToRemove);
            }

            var image = new Image
            {
                Aspect = Aspect.Center,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = ((AddEditSceneViewModel)BindingContext).SceneImage,
                ZIndex = -1
            };


            MainGrid.Children.Add(image);
        }
    }

    private void Grid_OnHandlerChanged(object sender, EventArgs e)
    {
#if ANDROID
        var layout = (ScrollView)sender;
        var pw = (Android.Views.View)((ScrollView)sender).Handler?.PlatformView;
        pw.HandleDrop(layout, p =>
        {
            _selected.Move(p.X, p.Y);
        });
#endif
    }
}