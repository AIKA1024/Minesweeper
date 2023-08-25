using Minesweeper.Class;
using Minesweeper.Models;
using Minesweeper.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    readonly List<Cell> openCellList = new List<Cell>(10);
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
    public void InitGame()
    {
      if (!gameInfo.started)
        return;
      gameInfo.CurrFaceStatus = FaceStatus.Normal;
      foreach (var cell in gameInfo.CellList)
      {
        cell.Init();
      }
      gameInfo.started = false;
      gameInfo.GameOver = false;
      openCellList.Clear();
    }
    private void StartGame()
    {
      gameInfo.started = true;
      gameInfo.GameOver = false;
      Cell SeleCell = (Cell)listBox.SelectedItem;
      gameInfo.BombList.Clear();
      //布雷
      for (int i = 0; i < gameInfo.BombCount; i++)
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
        foreach (var cell in GetAroundValidCell(bombCell))
        {
          if (cell.IsBomb == true || cell.AroundBombNum != 0)
            continue;

          foreach (var item in GetAroundValidCell(cell))
          {
            if (item.IsBomb == true)
              cell.AroundBombNum++;
          }
        }
      }
    }
    private void ReCalculateOffset()
    {
      gameInfo.OffsetList.Clear();
      ///九宫格的偏移位置
      gameInfo.OffsetList.Add(-gameInfo.Column - 1);
      gameInfo.OffsetList.Add(-gameInfo.Column);
      gameInfo.OffsetList.Add(-gameInfo.Column + 1);

      gameInfo.OffsetList.Add(-1);
      gameInfo.OffsetList.Add(0);
      gameInfo.OffsetList.Add(1);

      gameInfo.OffsetList.Add(gameInfo.Column - 1);
      gameInfo.OffsetList.Add(gameInfo.Column);
      gameInfo.OffsetList.Add(gameInfo.Column + 1);
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
        if (cell.CellMark == CellMark.Flag) return;
        cell.Flag = CellFlag.Left | CellFlag.Right | CellFlag.Top | CellFlag.Bottom;
        cell.Pressed = true;
        lastPressList.Add((Cell)listBox.SelectedItem);
      }
    }
    private void OpenAroundCellBYFlag()
    {
      Cell SelectCell = (Cell)listBox.SelectedItem;
      if (SelectCell == null)
        return;
      var aroundCell = GetAroundValidCell(SelectCell);
      if (SelectCell.AroundBombNum != aroundCell.Where(c => c.CellMark == CellMark.Flag).Count())
        return;

      foreach (var cell in aroundCell)
      {

        if (cell.CellMark == CellMark.Flag || cell.IsOpened)
          continue;

        if (cell.IsBomb)
        {
          cell.Explode = true;
          GameOver(false);
          return;
        }
        if (cell.CellMark == CellMark.Flag)
          return;

        if (cell.AroundBombNum == 0)
          OpenCell(cell);
        else
        {
          cell.IsOpened = true;
          cell.CellMark = CellMark.None;
          cell.Flag = CellFlag.None;
          CheckGameWin(cell);
        }
      }
    }
    /// <summary>
    /// 获取上下左右四个方向的格子
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    private List<Cell> GetUDLFCell(Cell cell)
    {
      var result = new List<Cell>();
      for (int i = 1; i < 8; i += 2)
      {
        int offset = cell.Index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.Column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + cell.Index) / gameInfo.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
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
      var result = new List<Cell>();

      for (int i = 0; i < gameInfo.OffsetList.Count; i++)
      {
        int offset = cell.Index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.Column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + cell.Index) / gameInfo.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
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
      var result = new List<Cell?>();
      for (int i = 0; i < gameInfo.OffsetList.Count; i++)
      {
        int offset = cell.Index + gameInfo.OffsetList[i];
        int offsetRow = i / 3;
        if (offset >= 0 && offset < gameInfo.maxCell &&
          offset / gameInfo.Column == (int)Math.Floor(((float)gameInfo.OffsetList[offsetRow * 3 + 1] + cell.Index) / gameInfo.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
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
      if (ClickedCell.CellMark == CellMark.Flag)
        return;
      if (ClickedCell.IsBomb)
      {
        ClickedCell.Explode = true;
        gameInfo.CurrFaceStatus = FaceStatus.Dead;
        GameOver(false);
        return;
      }
      if (!ClickedCell.IsOpened)
      {
        ClickedCell.IsOpened = true;
        ClickedCell.CellMark = CellMark.None;
        ClickedCell.Flag = CellFlag.None;
        CheckGameWin(ClickedCell);
      }
      if (ClickedCell.AroundBombNum > 0)
        return;
      var AroundValidCellList = GetAroundValidCell(ClickedCell);
      foreach (var cell in AroundValidCellList)
      {
        if (cell.IsOpened || cell.CellMark == CellMark.Flag || cell.IsBomb)
          continue;
        else
        {
          cell.IsOpened = true;
          CheckGameWin(ClickedCell);
          if (cell.AroundBombNum == 0)
            OpenCell(cell);
        }
      }
    }
    private void CheckGameWin(Cell cell)
    {
      openCellList.Add(cell);
      if (openCellList.Count == gameInfo.maxCell - gameInfo.BombCount)
        GameOver(true);
    }
    private void GameOver(bool winOrlose)
    {
      if (gameInfo.GameOver)
        return;

      if (!winOrlose)
        gameInfo.CurrFaceStatus = FaceStatus.Dead;

      gameInfo.GameOver = true;
      openCellList.Clear();
      foreach (var cell in gameInfo.BombList)
      {
        if (cell.CellMark != CellMark.Flag)
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
        switch (cell.CellMark)
        {
          case CellMark.None:
            cell.CellMark = CellMark.Flag;
            break;
          case CellMark.Flag:
            if (gameInfo.useMark)
              cell.CellMark = CellMark.QuestionMark;
            else
              cell.CellMark = CellMark.None;
            break;
          case CellMark.QuestionMark:
            cell.CellMark = CellMark.None;
            break;
          default:
            break;
        }
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
      if (gameInfo.GameOver || gameInfo.started)
      {
        InitGame();
      }
    }

    private void CustomMenuItem_Click(object sender, RoutedEventArgs e)
    {
      CustomGameWindow customGameWindow = new CustomGameWindow(this) { DataContext = gameInfo };
      customGameWindow.ShowDialog();
    }
    private void me_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && !gameInfo.GameOver)
        gameInfo.CurrFaceStatus = FaceStatus.Clicked;
    }
    private void me_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (!gameInfo.GameOver)
        gameInfo.CurrFaceStatus = FaceStatus.Normal;
    }
  }
}
