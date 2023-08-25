using Minesweeper.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Minesweeper.Resources.Converters
{
  internal class CellShowImageConverter : IMultiValueConverter
  {
    private readonly BitmapImage MistakeFlag = new();
    private readonly BitmapImage Flag = new();
    private readonly BitmapImage QuestionMark = new();
    private readonly BitmapImage Bomb = new();
    public static CellShowImageConverter Instance = new CellShowImageConverter();
    public CellShowImageConverter()
    {
      MistakeFlag.BeginInit();
      MistakeFlag.UriSource = new Uri("./Resources/Pictures/MistakeFlag.png", UriKind.RelativeOrAbsolute);
      MistakeFlag.EndInit();

      Flag.BeginInit();
      Flag.UriSource = new Uri("./Resources/Pictures/Flag.png", UriKind.RelativeOrAbsolute);
      Flag.EndInit();

      QuestionMark.BeginInit();
      QuestionMark.UriSource = new Uri("./Resources/Pictures/QuestionMark.png", UriKind.RelativeOrAbsolute);
      QuestionMark.EndInit();

      Bomb.BeginInit();
      Bomb.UriSource = new Uri("./Resources/Pictures/Bomb.png", UriKind.RelativeOrAbsolute);
      Bomb.EndInit();
    }
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var cellmark = (CellMark)values[0];
      bool isBomb = (bool)values[1];
      bool gameOver = (bool)values[2];
      if (gameOver && !isBomb && cellmark == CellMark.Flag)
      {
        return MistakeFlag;
      }
      else
      {
        if (cellmark == CellMark.Flag)
        {
          return Flag;
        }
        else if (cellmark == CellMark.QuestionMark)
        {
          return QuestionMark;
        }
        else if (isBomb)
        {
          return Bomb;
        }
      }
      return null;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
