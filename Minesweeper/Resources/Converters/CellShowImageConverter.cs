using Minesweeper.Class;
using Minesweeper.Models;
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
      var cellmark = (CellMark)values[0];
      bool isBomb = (bool)values[1];
      bool gameOver = (bool)values[2];
      var image = new BitmapImage();
      image.BeginInit();
      if (gameOver && !isBomb && cellmark == CellMark.Flag)
      {
        image.UriSource = new Uri("./Resources/Pictures/MistakeFlag.png", UriKind.RelativeOrAbsolute);
        image.EndInit();
      }
      else
      {
        if (cellmark == CellMark.Flag)
        {
          image.UriSource = new Uri("./Resources/Pictures/Flag.png", UriKind.RelativeOrAbsolute);
          image.EndInit();
        }
        else if (cellmark == CellMark.Mark)
        {
          image.UriSource = new Uri("./Resources/Pictures/QuestionMark.png", UriKind.RelativeOrAbsolute);
          image.EndInit();
        }
        else if (isBomb)
        {
          image.UriSource = new Uri("./Resources/Pictures/Bomb.png", UriKind.RelativeOrAbsolute);
          image.EndInit();
        }
      }
      return image;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
