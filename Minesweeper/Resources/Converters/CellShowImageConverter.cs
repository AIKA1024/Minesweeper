using Minesweeper.Class;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Minesweeper.Resources.Converters
{
  internal class CellShowImageConverter : IMultiValueConverter
  {
    public static CellShowImageConverter Instance = new CellShowImageConverter();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      bool isFlaged = (bool)values[0];
      bool isBomb = (bool)values[1];
      var image = new BitmapImage();
      image.BeginInit();
      if (isFlaged)
      {
        image.UriSource = new Uri("./Resources/Flag.png", UriKind.RelativeOrAbsolute);
        image.EndInit();
      }
      else if (isBomb)
      {
        image.UriSource = new Uri("./Resources/Bomb.png", UriKind.RelativeOrAbsolute);
        image.EndInit();
      }
      return image;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
