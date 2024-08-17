using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Minesweeper.Models
{
  public enum FaceStatus
  {
    Normal,
    Clicked,
    Dead,
    Win
  }
  public class GameInfo : INotifyPropertyChanged
  {
    private static GameInfo instance;
    public static GameInfo Instance
    {
      get
      {
        instance ??= new GameInfo();
        return instance;
      }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private System.Timers.Timer timer;
    private int row = 9;
    public int Row
    {
      get { return row; }
      set
      {
        if (value < 9)
          row = 9;
        else
          row = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row)));
      }
    }
    private int column = 9;
    public int Column
    {
      get { return column; }
      set
      {
        if (value < 9)
          column = 9;
        else
          column = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Column)));
      }
    }
    private int bombCount = 10;
    public int BombCount
    {
      get { return bombCount; }
      set
      {
        if (value < 10)
          bombCount = 10;
        else if (value >= row * column)
          bombCount = row * column - 1;
        else
          bombCount = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BombCount)));
      }
    }
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
    public int MaxCell
    {
      get => Row * Column;
    }

    private bool useMark = true;

    public bool UseMark
    {
      get { return useMark; }
      set
      {
        useMark = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseMark)));
      }
    }
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
        if (gameOver == value)
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
    private bool winOrLose;

    public bool WinOrLose
    {
      get { return winOrLose; }
      set
      {
        winOrLose = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinOrLose)));
      }
    }



    public readonly List<int> OffsetList = new List<int>(9);
    public readonly ObservableCollection<Cell> FlagList = new ObservableCollection<Cell>();
    public readonly List<Cell> BombList = new List<Cell>(10);
    public ObservableCollection<Cell> CellList { get; set; }

    private GameInfo()
    {
      timer = new() { Interval = 1000 };
      timer.Elapsed += (s, e) => { TimeCost++; };
      CellList = new ObservableCollection<Cell>(new List<Cell>(Row * Column));
      FlagList.CollectionChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlagCount)));
    }
  }
}
