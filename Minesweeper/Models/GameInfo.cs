using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Minesweeper.Models
{
  public class GameInfo : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    private int row = 16;
    public int Row
    {
      get { return row; }
      set
      {
        row = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row)));
      }
    }
    private int column = 30;
    public int Column
    {
      get { return column; }
      set
      {
        column = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Column)));
      }
    }
    private int bombCount = 30;
    public int BombCount
    {
      get { return bombCount; }
      set
      {
        bombCount = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BombCount)));
      }
    }
    public int maxCell = 480;
    public bool useMark = true;
    public bool started = false;

    private bool gameOver;
    public bool GameOver
    {
      get { return gameOver; }
      set
      {
        gameOver = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameOver)));
      }
    }
    public readonly List<int> OffsetList = new List<int>(9);
    public readonly List<Cell> BombList = new List<Cell>(10);
    public ObservableCollection<Cell> CellList { get; set; }

    public GameInfo()
    {
      CellList = new ObservableCollection<Cell>(new List<Cell>(Row * Column));
    }
  }
}
