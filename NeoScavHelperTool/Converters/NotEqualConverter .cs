using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NeoScavHelperTool.Converters
{
    public class NotEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                return !Equals(value, System.Convert.ChangeType(parameter, value.GetType()));
            }
            else if (value == null)
            {
                return true;
            }
            throw new ArgumentNullException("The parameter \"parameter\" cannot be null");
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
