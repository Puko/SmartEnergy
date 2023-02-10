using SmartEnergy.Material;
using System.Globalization;

namespace SmartEnergy.Converters
{
    public class ModeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new MaterialImageSource { Glyph = MaterialIcon.ListStatus } : new MaterialImageSource { Glyph = MaterialIcon.Graph };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
