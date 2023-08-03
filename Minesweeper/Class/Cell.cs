using System;
using System.ComponentModel;

namespace Minesweeper.Class
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
  public class Cell : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;
    public Cell(int index)
    {
      Index = index;
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

    private bool isBomb;
    public bool IsBomb 
    {
      get { return isBomb; }
      set
      {
        if (isBomb!=value) 
        {
          isBomb = value;
          PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(IsBomb)));
        }
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
    public override string ToString()
    {
      return "";
    }
  }
}
