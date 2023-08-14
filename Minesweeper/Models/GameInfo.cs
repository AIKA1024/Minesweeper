using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Models
{
    public class GameInfo : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    public int row = 16;
    public int column = 30;
    public int bombCount = 50;
    public int maxCell = 480;
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
      = new ObservableCollection<Cell>(new List<Cell>(480));

    public GameInfo()
    {

    }
  }
}
