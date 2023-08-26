using System;
using System.Windows;
using System.Windows.Data;

namespace EquipmentDowntime.Converters
{
    class IntToVisibilityHiddenConverter : System.Windows.Markup.MarkupExtension, IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility ReturnValue = Visibility.Hidden;
            int val = (int)value;
            if (val > 0)
            {
                ReturnValue = Visibility.Visible;
            }
            else
            {
                ReturnValue = Visibility.Hidden;
            }
            return ReturnValue;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ReturnValue = false;
            return ReturnValue;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
