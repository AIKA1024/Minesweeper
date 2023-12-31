﻿using Minesweeper.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Resources.Converters
{
  public class EnumFlagConverter : IValueConverter
  {
    public static EnumFlagConverter Instance = new EnumFlagConverter();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      CellFlag source = (CellFlag)value;
      CellFlag targetEnum = (CellFlag)parameter;
      return targetEnum == (targetEnum & source);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
