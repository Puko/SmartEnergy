using SmartEnergy.Extensions;
using System.Globalization;

namespace SmartEnergy.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? App.Current.GetResource<Color>("Primary") 
                : App.Current.GetResource<Color>("Gray200");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
