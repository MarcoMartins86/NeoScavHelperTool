using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NeoScavHelperTool.Converters
{
    class SumConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (values == null || values.Any(v => v == DependencyProperty.UnsetValue))
            {
                return DependencyProperty.UnsetValue;
            }

            double sum = 0.0;
            foreach (var value in values)
            {
                if (value is double d)
                {
                    sum += d;
                }
            }
            return sum;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture
        )
        {
            // Not needed for this one-way binding scenario
            throw new NotImplementedException();
        }
    }
}
