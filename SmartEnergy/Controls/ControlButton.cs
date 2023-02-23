using SmartEnergy.Converters;
using SmartEnergy.Extensions;
using SmartEnergy.ViewModels;

namespace SmartEnergy.Controls
{
    public class ControlButton : Border
    {
        private const double _width = 70;
        private const double _height = 70;
        private Label _mainBtn;

        private ControlButton(SceneDeviceItemViewModel sceneDevice)
        {
            Device = sceneDevice;

            _mainBtn = new Label 
            {
                BindingContext = sceneDevice,
                FontSize = 10,
                Margin = new Thickness(3),
                TextColor = Colors.White,
                FontAttributes= FontAttributes.Bold,
                FontFamily = "FFMarkProBold",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = sceneDevice.Device.Mac,
                BackgroundColor = App.Current.GetResource<Color>("Black")
            };

            _mainBtn.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(() => Selected?.Invoke(this, EventArgs.Empty))
            });

            _mainBtn.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2,
                Command = new Command(() => Edit?.Invoke(this, EventArgs.Empty))
            });

            StrokeThickness = 3;
            Content = _mainBtn;

            Unselect();
        }

        public event EventHandler Selected;
        public event EventHandler Edit;

        public double OriginalX { get; set; }
        public double OriginalY { get; set; }
        public double OriginalWidth { get; set; }
        public double OriginalHeight { get; set; }
        public SceneDeviceItemViewModel Device { get; private set; }

        public void Select()
        {
            _mainBtn.Opacity = 1;
            _mainBtn.TextColor = Colors.White;
            Stroke = new SolidColorBrush(App.Current.GetResource<Color>("Tertiary"));
        }

        public void Unselect()
        {
            _mainBtn.Opacity = 0.6;
            _mainBtn.TextColor = Colors.White;
            Stroke = Brush.Transparent;
        }

        public void Move(double x, double y)
        {
            OriginalX = x - _width / 2;
            OriginalY = y - _height / 2;

            Device.Device.OriginalX = OriginalX;
            Device.Device.OriginalY = OriginalY;

            AbsoluteLayout.SetLayoutBounds(this, new Rect(OriginalX, OriginalY, _width, _height));
        }

        public static ControlButton Create(SceneDeviceItemViewModel sceneDevice,
            double width, double height)
        {
            ControlButton btn = new ControlButton(sceneDevice)
            {
                OriginalY = sceneDevice.Device.OriginalY,
                OriginalX = sceneDevice.Device.OriginalX,
                OriginalWidth = width,
                OriginalHeight = height
            };

            AbsoluteLayout.SetLayoutBounds(btn, new Rect(sceneDevice.Device.OriginalX, sceneDevice.Device.OriginalY, _width, _height));
            return btn;
        }
    }
}
