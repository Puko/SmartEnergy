using SmartEnergy.Database.Models;
using SmartEnergy.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartEnergy.Controls;

public partial class GraphicSceneControl : ScrollView
{
    public static readonly BindableProperty DevicesProperty =
        BindableProperty.Create(nameof(Devices), typeof(ObservableCollection<SceneDeviceItemViewModel>), typeof(GraphicSceneControl), 
            new ObservableCollection<SceneDeviceItemViewModel>(), propertyChanged: HandleOnDevicesChanged);

    public static readonly BindableProperty SceneProperty =
        BindableProperty.Create(nameof(Scene), typeof(Scene), typeof(GraphicSceneControl),null);

    public static readonly BindableProperty EditDeviceCommandProperty =
     BindableProperty.Create(nameof(EditDeviceCommand), typeof(ICommand), typeof(GraphicSceneControl), null);


    private ControlButton _selected;

    public GraphicSceneControl()
	 {
		  InitializeComponent();
	 }

    public ICommand EditDeviceCommand
    {
        get { return (ICommand)GetValue(EditDeviceCommandProperty); }
        set { SetValue(EditDeviceCommandProperty, value); }
    }

    public Scene Scene
    {
        get { return (Scene)GetValue(SceneProperty); }
        set { SetValue(SceneProperty, value); }
    }

    public ObservableCollection<SceneDeviceItemViewModel> Devices
    {
        get { return (ObservableCollection<SceneDeviceItemViewModel>)GetValue(DevicesProperty); }
        set { SetValue(DevicesProperty, value);}
    }

    private void Tapped_MoveControl(object sender, TappedEventArgs e)
    {
        if (_selected != null)
        {
            var position = e.GetPosition(Grid);
            if (!position.HasValue)
                return;

            _selected.Move(position.Value.X, position.Value.Y);
        }
    }

    private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        {
            var sd = (SceneDeviceItemViewModel)e.NewItems[0];

            ControlButton btn = CreateButton(sd);
            Grid.Children.Add(btn);

            if (_selected != null)
                _selected.Unselect();

            _selected = btn;

            btn.Select();
        }
        else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            var sd = (SceneDeviceItemViewModel)e.OldItems[0];

            var ctrlBtn = Grid.Children.Cast<ControlButton>().FirstOrDefault(x => x.Device == sd);
            if(ctrlBtn != null)
            {
                Grid.Children.Remove(ctrlBtn);
            }
        }
    }

    private void Btn_Selected(object sender, EventArgs e)
    {
        if (_selected != null)
            _selected.Unselect();

        _selected = (ControlButton)sender;
        _selected.Select();
    }

    private void Btn_Edit(object sender, EventArgs e)
    {
        var sc = ((ControlButton)sender).Device;
        EditDeviceCommand?.Execute(sc);
    }

    private ControlButton CreateButton(SceneDeviceItemViewModel sd)
    {
        var btn = ControlButton.Create(sd, Scene.OriginalWidth, Scene.OriginalHeight);
        btn.Selected += Btn_Selected;
        btn.Edit += Btn_Edit;
        return btn;
    }

    private void InitCollection(ObservableCollection<SceneDeviceItemViewModel> devices)
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
        if(newValue is ObservableCollection<SceneDeviceItemViewModel> devices)
        {
            ((GraphicSceneControl)bindable).InitCollection(devices);
        }
    }
}