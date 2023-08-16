using Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minesweeper.Windows
{
  /// <summary>
  /// CustomGameWindow.xaml 的交互逻辑
  /// </summary>
  public partial class CustomGameWindow : Window
  {
    MainWindow mainWindow;
    public CustomGameWindow(MainWindow mainWindow)
    {
      InitializeComponent();
      this.mainWindow = mainWindow;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
      var gameinfo = (GameInfo)DataContext;
      try
      {
        gameinfo.Row = int.Parse(rowTextBox.Text);
        gameinfo.Column = int.Parse(columnTextBox.Text);
        gameinfo.BombCount = int.Parse(bombCountTextBox.Text);
        mainWindow.InitGame();
        Close();
      }
      catch (FormatException)
      {
        MessageBox.Show("请输入正确的数值","错误");
      }
    }
  }
}
