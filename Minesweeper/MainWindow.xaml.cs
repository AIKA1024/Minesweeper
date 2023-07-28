using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    int row = 16;
    int column = 30;
    int maxLattice = 480;
    List<int> offsetList = new List<int>();
    public ObservableCollection<int> items { get; set; }
      = new ObservableCollection<int>(new List<int>(480));
    public MainWindow()
    {
      InitializeComponent();
      for (int i = 0; i < maxLattice; i++)
      {
        items.Add(i);
      }
      ReCalculateOffset();
      DataContext = this;
    }
    private void ReCalculateOffset()
    {
      offsetList.Clear();
      offsetList.Capacity = 9;
      ///九宫格的偏移位置
      offsetList.Add(-column - 1);
      offsetList.Add(-column);
      offsetList.Add(-column + 1);

      offsetList.Add(-1);
      offsetList.Add(0);
      offsetList.Add(1);

      offsetList.Add(column - 1);
      offsetList.Add(column);
      offsetList.Add(column + 1);
    }
    private void ListBoxItem_MouseButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      //listBox.UnselectAll();
      //listBox.SelectionMode = SelectionMode.Single;
    }
    private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
      {
        listBox.SelectionMode = SelectionMode.Multiple;
        if (listBox.SelectedItem != null)
          foreach (var item in GetAroundLattice((int)listBox.SelectedItem))
            listBox.SelectedItems.Add(item);
      }
    }
    private void listBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      //listBox.UnselectAll();
    }

    private List<int> GetAroundLattice(int Lattice)
    {
      int index = items.IndexOf(Lattice);
      int inRow = index / row;
      int inColumn = Lattice % column;
      var result = new List<int>();
      foreach (var item in offsetList)
      {
        int offset = index + item;
        if (offset >= 0 && offset < maxLattice)
        {
          result.Add(items[offset]);
        }
      }
      return result;
    }

    private void Button_MouseMove(object sender, MouseEventArgs e)
    {
      listBox.UnselectAll();
    }
  }
}
