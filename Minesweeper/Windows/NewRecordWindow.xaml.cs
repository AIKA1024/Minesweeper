using Minesweeper.Class;
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
  /// NewRecordWindow.xaml 的交互逻辑
  /// </summary>
  public partial class NewRecordWindow : Window
  {
    public NewRecordWindow()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (RankManager.RankDic.ContainsKey((string)DataContext))
      {
        RankManager.RankDic[(string)DataContext].PlayerName = playerNameTextBox.Text;
        RankManager.RankDic[(string)DataContext].Duration = GameInfo.Instance.TimeCost;
      }
      else
        RankManager.RankDic.Add((string)DataContext, new ScoreRecord() {PlayerName = playerNameTextBox.Text ,Duration =GameInfo.Instance.TimeCost});
      RankManager.SaveChanged();
      Close();
      new RankingListWindow().ShowDialog();
    }
  }
}
