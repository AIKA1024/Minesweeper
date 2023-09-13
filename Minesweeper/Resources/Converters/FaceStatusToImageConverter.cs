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
  internal class FaceStatusToImageConverter : IValueConverter
  {
    public static FaceStatusToImageConverter Instance = new FaceStatusToImageConverter();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      BitmapImage image = new BitmapImage();
      image.BeginInit();
      FaceStatus faceStatus = (FaceStatus)value;
      switch (faceStatus)
      {
        case FaceStatus.Normal:
          image.UriSource = new Uri("./Resources/Pictures/NormalFace.png", UriKind.RelativeOrAbsolute);
          break;
        case FaceStatus.Clicked:
          image.UriSource = new Uri("./Resources/Pictures/ClickedFace.png", UriKind.RelativeOrAbsolute);
          break;
        case FaceStatus.Dead:
          image.UriSource = new Uri("./Resources/Pictures/DeadFace.png", UriKind.RelativeOrAbsolute);
          break;
        case FaceStatus.Win:
          image.UriSource = new Uri("./Resources/Pictures/WinFace.png", UriKind.RelativeOrAbsolute);
          break;
      }
      image.EndInit();
      return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
