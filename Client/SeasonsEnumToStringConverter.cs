using System;
using System.Globalization;
using System.Windows.Data;

namespace Client
{
    public class SeasonsEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SeasonsEnum)
            {
                var val = (SeasonsEnum)value;
                return Helpers.StringifySeasonEnum(val);
            }
            return "Unknown season!";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
