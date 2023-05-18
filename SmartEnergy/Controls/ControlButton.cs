using SmartEnergy.Converters;
using SmartEnergy.Extensions;
using SmartEnergy.ViewModels;

#if ANDROID
using SmartEnergy.Platforms.Android;
#endif

namespace SmartEnergy.Controls
{
    public class ControlButton : Border
    {
        private readonly Label _deviceLabel;

        private const double WidthValue = 110;
        private const double HeightValue = 90;

        private ControlButton(AddSceneDeviceViewModel sceneDevice)
        {
            Device = sceneDevice;
            Device.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(AddSceneDeviceViewModel.IsOnline))
                    MainThread.BeginInvokeOnMainThread(() => BackgroundColor = BoolToColorConverter.Convert(Device.IsOnline)); 
                else if (args.PropertyName == nameof(AddSceneDeviceViewModel.Name))
                    MainThread.BeginInvokeOnMainThread(() => _deviceLabel.Text = Device.Name);

            };

            BackgroundColor = Application.Current.GetResource<Color>("Gray200");
            
            StrokeThickness = 3;
            Stroke = Brush.WhiteSmoke;
            
            _deviceLabel = new Label
            {
                FontSize = 15,
                Margin = new Thickness(3),
                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "FFMarkProBold",
                MaxLines = 3,
                LineBreakMode = LineBreakMode.TailTruncation,
                BackgroundColor = Colors.Transparent,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = Device.Name
            };
            
            Content = _deviceLabel;
        }

        public event EventHandler Selected;
        public event EventHandler Edit;

        public double OriginalX { get; set; }
        public double OriginalY { get; set; }
        public double OriginalWidth { get; set; }
        public double OriginalHeight { get; set; }
        public AddSceneDeviceViewModel Device { get; }

        public void Move(double x, double y)
        {
            OriginalX = x - WidthValue / 2;
            OriginalY = y - HeightValue / 2;

            Device.Device.OriginalX = OriginalX;
            Device.Device.OriginalY = OriginalY;

            AbsoluteLayout.SetLayoutBounds(this, new Rect(OriginalX, OriginalY, WidthValue, HeightValue));
        }


        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

#if ANDROID
            var pw = (Android.Views.View)this.Handler?.PlatformView;
            pw.HandleClick(() => Edit?.Invoke(this, EventArgs.Empty));
            pw.StartDragOnLongPress(() => Selected?.Invoke(this, EventArgs.Empty));
#endif
        }

        public static ControlButton Create(AddSceneDeviceViewModel sceneDevice,
            double width, double height)
        {
            ControlButton btn = new ControlButton(sceneDevice)
            {
                OriginalY = sceneDevice.Device.OriginalY,
                OriginalX = sceneDevice.Device.OriginalX,
                OriginalWidth = width,
                OriginalHeight = height
            };

            AbsoluteLayout.SetLayoutBounds(btn, new Rect(sceneDevice.Device.OriginalX, sceneDevice.Device.OriginalY, WidthValue, HeightValue));
            return btn;
        }
    }
}
