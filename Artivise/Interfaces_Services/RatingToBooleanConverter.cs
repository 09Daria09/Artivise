using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Artivise.Interfaces_Services
{
    public class RatingToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            int currentRating = (int)value;
            int thisRating = System.Convert.ToInt32(parameter);
            return currentRating == thisRating;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return System.Convert.ToInt32(parameter);

            return Binding.DoNothing;
        }
    }


}
