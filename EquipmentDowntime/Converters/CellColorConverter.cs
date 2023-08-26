using EquipmentDowntime.DowntimeData;
using System;
using System.Globalization;
using System.Windows.Data;

namespace EquipmentDowntime.Converters
{
    class CellColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var row = values[0] as Downtime;
            if (row == null)
            {
                return System.Windows.Media.Brushes.Beige;
            }
            bool isSelected = (bool)values[1];
            if (isSelected)
            {
                if (row.State == 1)
                {
                    return System.Windows.Media.Brushes.Green;
                }
                else if (row.State == 2)
                {
                    return System.Windows.Media.Brushes.DarkRed;
                }
                return System.Windows.Media.Brushes.DeepSkyBlue;
            }
            if (row.State == 1)
            {
                return System.Windows.Media.Brushes.LightGreen;
            }
            else if (row.State == 2)
            {
                return System.Windows.Media.Brushes.Red;
            }
            return System.Windows.Media.Brushes.White;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
