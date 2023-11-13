using Minesweeper.Class;
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
  /// RankingListWindow.xaml 的交互逻辑
  /// </summary>
  public partial class RankingListWindow : Window
  {
    public RankingListWindow()
    {
      InitializeComponent();
      DataContext = RankManager.RankDic;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void RescoreBtn_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show("清除记录？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        RankManager.RankDic.Clear();
        RankManager.SaveChanged();
        DataContext = null;
        DataContext = RankManager.RankDic;
      }
    }
  }
}
