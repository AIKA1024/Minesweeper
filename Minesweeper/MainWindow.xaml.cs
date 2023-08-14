using Minesweeper.Class;
using Minesweeper.Models;
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
    GameInfo gameInfo;
    MouseButtonState leftButton = MouseButtonState.Released;
    MouseButtonState rightButton = MouseButtonState.Released;
    readonly List<Cell?> lastPressList = new List<Cell?>();

    public MainWindow()
    {
      InitializeComponent();
      gameInfo = new GameInfo();
      for (int i = 0; i < gameInfo.maxCell; i++)
      {
        gameInfo.CellList.Add(CellPool.GetCell(i));
      }
      ReCalculateOffset();
      DataContext = gameInfo;
    }
    private void InitGame()
    {
      if (!gameInfo.started)
        return;

      foreach (var cell in gameInfo.CellList)
      {
        cell.Init();
      }
      gameInfo.started = false;
      gameInfo.GameOver = false;
    }
    private void StartGame()
    {
      gameInfo.started = true;
      gameInfo.GameOver = false;
      Cell SeleCell = (Cell)listBox.SelectedItem;
      gameInfo.BombList.Clear();
      //布雷
      for (int i = 0; i < gameInfo.bombCount; i++)
      {
        int bombIndex = Random.Shared.Next(0, gameInfo.maxCell);
        while (bombIndex == SeleCell.Index || gameInfo.CellList[bombIndex].IsBomb == true)
          bombIndex = Random.Shared.Next(0, gameInfo.maxCell);

        gameInfo.CellList[bombIndex].IsBomb = true;
        gameInfo.BombList.Add(gameInfo.CellList[bombIndex]);
      }
      //计算周围雷数
      foreach (var bombCell in gameInfo.BombList)
      {
        foreach (var cell in GetAroundCell(bombCell))
        {
          if (cell == null || cell.IsBomb == true || cell.AroundBombNum != 0)
            continue;

          foreach (var item in GetAroundCell(cell))
          {
            if (item?.IsBomb == true)
              cell.AroundBombNum++;
          }
        }
      }
    }
    private void ReCalculateOffset()
    {
      gameInfo.OffsetList.Clear();
      ///九宫格的偏移位置
      gameInfo.OffsetList.Add(-gameInfo.column - 1);
      gameInfo.OffsetList.Add(-gameInfo.column);
      gameInfo.OffsetList.Add(-gameInfo.column + 1);

      gameInfo.OffsetList.Add(-1);
      gameInfo.OffsetList.Add(0);
      gameInfo.OffsetList.Add(1);

      gameInfo.OffsetList.Add(gameInfo.column - 1);
      gameInfo.OffsetList.Add(gameInfo.column);
      gameInfo.OffsetList.Add(gameInfo.column + 1);
    }

    private void UpdatePressCell()
    {
      foreach (Cell? cell in lastPressList)
      {
        if (cell == null)
          continue;
        cell.Pressed = false;
        cell.Flag = CellFlag.None;
      }
      lastPressList.Clear();

      if (listBox.SelectedItem == null)
        return;

      if (leftButton == MouseButtonState.Pressed && rightButton == MouseButtonState.Pressed)
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
        if (cell.IsFlaged == true) return;
        cell.Flag = CellFlag.Left | CellFlag.Right | CellFlag.Top | CellFlag.Bottom;
        cell.Pressed = true;
        lastPressList.Add((Cell)listBox.SelectedItem);
      }
    }
    private void OpenAroundCellBYFlag()
    {
      Cell SelectCell = (Cell)listBox.SelectedItem;
      var aroundCell = GetAroundValidCell(SelectCell);
      if (SelectCell.AroundBombNum != aroundCell.Where(c=>c.IsFlaged).Count())
        return;

      foreach (var cell in aroundCell)
      {

        if (cell.IsFlaged||cell.IsOpened)
          continue;

        if (cell.IsBomb)
        {
          cell.Explode = true;
          GameOver();
        }
        if (cell.IsFlaged)
          return;

        cell.IsOpened = true;
        cell.IsFlaged = false;
        cell.Flag = CellFlag.None;
      }
    }
    /// <summary>
    /// 获取上下左右四个方向的格子
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private List<Cell> GetUDLFCell(Cell cell)
    {
      int index = gameInfo.CellList.IndexOf(cell);
      var result = new List<Cell>();
      for (int i = 1; i < 8; i += 2)
      {
        int offset = index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + index) / gameInfo.column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
        {
          result.Add(gameInfo.CellList[offset]);
        }
      }
      return result;
    }
    /// <summary>
    /// 获取周围8个格子,不包含空格
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private List<Cell> GetAroundValidCell(Cell cell)
    {
      int index = gameInfo.CellList.IndexOf(cell);
      int inRow = index / gameInfo.row;
      int inColumn = index % gameInfo.column;
      var result = new List<Cell>();

      for (int i = 0; i < gameInfo.OffsetList.Count; i++)
      {
        int offset = index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + index) / gameInfo.column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
        {
          result.Add(gameInfo.CellList[offset]);
        }
      }
      return result;
    }

    /// <summary>
    /// 获取周围8个格子
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private List<Cell?> GetAroundCell(Cell cell)
    {
      int index = gameInfo.CellList.IndexOf(cell);
      int inRow = index / gameInfo.row;
      int inColumn = index % gameInfo.column;
      var result = new List<Cell?>();

      for (int i = 0; i < gameInfo.OffsetList.Count; i++)
      {
        int offset = index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + index) / gameInfo.column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
        {
          result.Add(gameInfo.CellList[offset]);
        }
        else
        {
          result.Add(null);
        }
      }
      return result;
    }

    private void OpenCell(Cell ClickedCell)
    {
      if (ClickedCell.IsBomb)
      {
        ClickedCell.Explode = true;
        GameOver();
      }
      if (ClickedCell.IsFlaged)
        return;

      ClickedCell.IsOpened = true;
      ClickedCell.IsFlaged = false;
      ClickedCell.Flag = CellFlag.None;
      if (ClickedCell.AroundBombNum > 0 || ClickedCell.IsBomb)
        return;
      var UDLRCellList = GetAroundValidCell(ClickedCell);
      foreach (var cell in UDLRCellList)
      {
        if (cell.IsOpened)
          continue;

        if (!cell.IsBomb)
        {
          cell.IsOpened = true;
          if (cell.AroundBombNum == 0)
            OpenCell(cell);
        }
      }
    }

    private void GameOver()
    {
      if (gameInfo.GameOver)
        return;

      gameInfo.GameOver = true;
      foreach (var cell in gameInfo.BombList)
      {
        if (!cell.IsFlaged)
          cell.IsOpened = true;
      }
    }

    private void ListBoxItem_MouseButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      leftButton = e.LeftButton;
      rightButton = e.RightButton;
      Cell seleCell = (Cell)((Grid)sender).DataContext;
      if (seleCell.IsOpened && (leftButton == MouseButtonState.Pressed || rightButton == MouseButtonState.Pressed))
      {
        OpenAroundCellBYFlag();
      }
      if (rightButton == MouseButtonState.Released && e.ChangedButton == MouseButton.Left && listBox.SelectedItem != null)
      {
        if (!gameInfo.started)
          StartGame();

        if (!seleCell.IsOpened)
          OpenCell(seleCell);
      }
      listBox.SelectedItem = null;
      UpdatePressCell();
    }
    private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
    {
      leftButton = e.LeftButton;
      rightButton = e.RightButton;
      if (leftButton == MouseButtonState.Pressed && rightButton == MouseButtonState.Pressed)
      {
        UpdatePressCell();
      }
    }
    private void listBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      listBox.SelectedItem = null;
    }


    private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

      Cell cell = (Cell)listBox.SelectedItem;
      if (leftButton == MouseButtonState.Released && rightButton == MouseButtonState.Pressed &&
         cell != null && !cell.IsOpened)
      {
        cell.IsFlaged = !cell.IsFlaged;
        return;
      }
      foreach (var item in e.RemovedItems)
      {
        ((Cell)item).Pressed = false;
      }
      UpdatePressCell();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (gameInfo.GameOver)
      {
        InitGame();
      }
    }

    private void listBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (gameInfo.GameOver)
        e.Handled = true;
    }
  }
}
