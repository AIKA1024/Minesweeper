using Minesweeper.Models;
using System;
using System.Windows;

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
        MessageBox.Show("请输入正确的数值", "错误");
      }
    }
  }
}
