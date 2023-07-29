using Minesweeper.Class;
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
    bool leftRightBtnPressed = false;
    List<int> offsetList = new List<int>();
    List<Lattice> lastPressList = new List<Lattice>();
    public ObservableCollection<Lattice> items { get; set; }
      = new ObservableCollection<Lattice>(new List<Lattice>(480));
    public MainWindow()
    {
      InitializeComponent();
      for (int i = 0; i < maxLattice; i++)
      {
        items.Add(new Lattice());
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

    private void UpdatePressLattice()
    {
      foreach (Lattice lattice in lastPressList)
        lattice.Pressed = false;
      lastPressList.Clear();

      if (listBox.SelectedItem == null)
        return;

      if (leftRightBtnPressed)
      {
        foreach (var lattice in GetAroundLattice((Lattice)listBox.SelectedItem))
        {
          lattice.Pressed = true;
          lastPressList.Add(lattice);
        }
      }
      else
      {
        ((Lattice)listBox.SelectedItem).Pressed = true;
        lastPressList.Add((Lattice)listBox.SelectedItem);
      }
    }
    private void ListBoxItem_MouseButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      leftRightBtnPressed = false;
      listBox.UnselectAll();
      UpdatePressLattice();
    }
    private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
      {
        leftRightBtnPressed = true;
        UpdatePressLattice();
      }
    }
    private void listBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      listBox.UnselectAll();
    }

    private List<Lattice> GetAroundLattice(Lattice lattice)
    {
      int index = items.IndexOf(lattice);
      int inRow = index / row;
      int inColumn = index % column;
      var result = new List<Lattice>();
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

    private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (var item in e.RemovedItems)
      {
        ((Lattice)item).Pressed = false;
      }
      UpdatePressLattice();
    }
  }
}
