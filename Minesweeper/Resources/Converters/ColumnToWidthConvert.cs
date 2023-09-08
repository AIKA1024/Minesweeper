using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Minesweeper.Resources.Converters
{
  class ColumnToWidthConvert : IValueConverter
  {
    public static ColumnToWidthConvert Instance = new ColumnToWidthConvert();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return 19 * (int)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
