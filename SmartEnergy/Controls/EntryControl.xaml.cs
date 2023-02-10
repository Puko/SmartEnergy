using SmartEnergy.Material;

namespace SmartEnergy.Controls
{
    public partial class EntryControl : ContentView
    {
        private bool _showPassword = false;

        public static BindableProperty IsPasswordProperty = BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(EntryControl));
        public static BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryControl));
        public static BindableProperty HelpTextProperty = BindableProperty.Create(nameof(HelpText), typeof(string), typeof(EntryControl));
        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryControl));

        public EntryControl()
        {
            InitializeComponent();
        }

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public string HelpText
        {
            get => (string)GetValue(HelpTextProperty);
            set => SetValue(HelpTextProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            _showPassword = !_showPassword;
            ImageSource.Glyph = _showPassword ? MaterialIcon.EyeOff : MaterialIcon.Eye;
            Entry.IsPassword = !_showPassword;
        }
    }
}