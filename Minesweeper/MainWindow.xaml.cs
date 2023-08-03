﻿using Minesweeper.Class;
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
    readonly List<int> offsetList = new List<int>(9);
    readonly List<Cell?> lastPressList = new List<Cell?>();
    public ObservableCollection<Cell> CellList { get; set; }
      = new ObservableCollection<Cell>(new List<Cell>(480));
    public MainWindow()
    {
      InitializeComponent();
      for (int i = 0; i < maxLattice; i++)
      {
        CellList.Add(new Cell());
      }
      ReCalculateOffset();
      DataContext = this;
    }
    private void ReCalculateOffset()
    {
      offsetList.Clear();
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
      foreach (Cell? lattice in lastPressList)
      {
        if (lattice == null)
          continue;
        lattice.Pressed = false;
        lattice.Flag = CellFlag.None;
      }
      lastPressList.Clear();

      if (listBox.SelectedItem == null)
        return;

      if (leftRightBtnPressed)
      {
        List<Cell?> aroundCell = GetAroundCell((Cell)listBox.SelectedItem);
        for (int i = 0; i < aroundCell.Count; i++)
        {
          if (aroundCell[i] == null)
            continue;

          if (i % 3 == 0)//判断是不是在边上，左上右下
            aroundCell[i].Flag = CellFlag.Left;
          if (i / 3 == 0)
            aroundCell[i].Flag |= CellFlag.Top;
          if (i % 3 == 2)
            aroundCell[i].Flag |= CellFlag.Right;
          if (i > 5)
            aroundCell[i].Flag |= CellFlag.Bottom;

          aroundCell[i].Pressed = true;
          lastPressList.Add(aroundCell[i]);
        }
      }
      else
      {
        Cell cell = (Cell)listBox.SelectedItem;
        cell.Flag = CellFlag.Left | CellFlag.Right | CellFlag.Top | CellFlag.Bottom;
        cell.Pressed = true;
        lastPressList.Add((Cell)listBox.SelectedItem);
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

    private List<Cell?> GetAroundCell(Cell lattice)
    {
      int index = CellList.IndexOf(lattice);
      int inRow = index / row;
      int inColumn = index % column;
      var result = new List<Cell?>();

      for (int i = 0; i < offsetList.Count; i++)
      {
        int offset = index + offsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < maxLattice &&
          offset / column == (int)Math.Floor(((float)offsetList[offsetRow * 3 + 1] + index) / (float)column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
        {
          result.Add(CellList[offset]);
        }
        else
        {
          result.Add(null);
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
        ((Cell)item).Pressed = false;
      }
      UpdatePressLattice();
    }
  }
}
