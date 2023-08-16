using Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Minesweeper.Resources.Converters
{
  class CellMarkNotFlagConvert : IValueConverter
  {
    public static CellMarkNotFlagConvert Instance = new CellMarkNotFlagConvert();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      CellMark mark = (CellMark)value;
      return mark != CellMark.Flag;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
