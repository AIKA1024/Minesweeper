using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Class
{
  public class Cell : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

	private bool pressed;

	public bool Pressed
	{
	  get { return pressed; }
	  set 
	  {
		pressed = value; 
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(Pressed)));
	  }
	}

  }
}
