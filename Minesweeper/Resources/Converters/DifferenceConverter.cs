using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Minesweeper.Resources.Converters
{
  class DifferenceConverter : IMultiValueConverter
  {
    public static DifferenceConverter Instance = new DifferenceConverter();
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      int temp = (int)values[0] - (int)values[1];
      
      return temp>-1?temp.ToString("D3"): temp.ToString("D2");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
