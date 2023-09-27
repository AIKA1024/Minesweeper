using System;
using System.ComponentModel;

namespace Minesweeper.Models
{
  [Flags]
  public enum CellFlag
  {
    None = 0,
    Left = 1,
    Top = 2,
    Right = 4,
    Bottom = 8
  }

  public enum CellMark
  {
    None = 0,
    Flag = 1,
    QuestionMark = 2
  }
  public class Cell : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public Cell(int index)
    {
      Index = index;
    }
    public void Init()
    {
      AroundBombNum = 0;
      IsOpened = false;
      CellMark = CellMark.None;
      IsBomb = false;
      Explode = false;
      Pressed = false;
      Flag = CellFlag.None;
    }
    public int Index { get; set; }
    private int aroundBombNum;
    public int AroundBombNum
    {
      get { return aroundBombNum; }
      set
      {
        if (aroundBombNum != value)
        {
          aroundBombNum = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AroundBombNum)));
        }
      }
    }
    private bool isOpened;
    public bool IsOpened
    {
      get { return isOpened; }
      set
      {
        if (isOpened != value)
        {
          isOpened = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOpened)));
        }
      }
    }
    private CellMark cellMark;

    public CellMark CellMark
    {
      get { return cellMark; }
      set
      {
        if (cellMark != value)
        {
          cellMark = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CellMark)));
        }
      }
    }

    private bool isBomb;
    public bool IsBomb
    {
      get { return isBomb; }
      set
      {
        if (isBomb != value)
        {
          isBomb = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBomb)));
        }
      }
    }
    private bool explode;

    public bool Explode
    {
      get { return explode; }
      set
      {
        explode = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Explode)));
      }
    }

    private bool pressed;
    public bool Pressed
    {
      get { return pressed; }
      set
      {
        if (pressed != value)
        {
          pressed = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Pressed)));
        }
      }
    }
    private CellFlag flag;
    /// <summary>
    /// 标识这个格子是在按下区域的哪边，处理按下边框颜色用
    /// </summary>
    public CellFlag Flag
    {
      get { return flag; }
      set
      {
        if (flag != value)
        {
          flag = value;
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Flag)));
        }
      }
    }
  }
}
