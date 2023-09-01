using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Minesweeper.Models
{
  public enum FaceStatus
  {
    Normal,
    Clicked,
    Dead
  }
  public class GameInfo : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    private System.Timers.Timer timer;
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
    private int flagCount = 30;
    public int FlagCount
    {
      get => FlagList.Count;
    }
    private int timeCost;
    public int TimeCost
    {
      get { return timeCost; }
      set
      {
        timeCost = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TimeCost)));
      }
    }

    public int maxCell = 480;
    public bool useMark = true;
    private bool started;
    public bool Started
    {
      get { return started; }
      set
      {
        if (started == value)
          return;

        if (value)
          timer.Start();
        else
          timer.Stop();

        started = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Started)));
      }
    }

    private bool gameOver;
    public bool GameOver
    {
      get { return gameOver; }
      set
      {
        if (started == value)
          return;

        if (value)
          timer.Stop();

        gameOver = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameOver)));
      }
    }

    private FaceStatus faceStatus;

    public FaceStatus CurrFaceStatus
    {
      get { return faceStatus; }
      set
      {
        if (faceStatus != value)
        {
          faceStatus = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrFaceStatus)));
        }
      }
    }

    public readonly List<int> OffsetList = new List<int>(9);
    public readonly ObservableCollection<Cell> FlagList = new ObservableCollection<Cell>();
    public readonly List<Cell> BombList = new List<Cell>(10);
    public ObservableCollection<Cell> CellList { get; set; }

    public GameInfo()
    {
      timer = new() { Interval = 1000 };
      timer.Elapsed += (s, e) => { TimeCost++; };
      CellList = new ObservableCollection<Cell>(new List<Cell>(Row * Column));
      FlagList.CollectionChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlagCount)));
    }
  }
}
