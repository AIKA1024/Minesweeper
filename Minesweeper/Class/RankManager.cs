using Minesweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Minesweeper.Class
{

  public static class RankManager
  {
    private static Dictionary<string, ScoreRecord>? rankDic;
    public static Dictionary<string, ScoreRecord> RankDic
    {
      get
      {
        if (string.IsNullOrEmpty(Properties.Settings.Default.RankingList))
          rankDic = new Dictionary<string, ScoreRecord>();
        else if(rankDic==null)
          rankDic = JsonSerializer.Deserialize(Properties.Settings.Default.RankingList, typeof(Dictionary<string, ScoreRecord>)) as Dictionary<string, ScoreRecord> ?? new Dictionary<string, ScoreRecord>();
        return rankDic;
      }
      set
      {
        rankDic = value;
      }
    } 
    public static void SaveChanged()
    {
      Properties.Settings.Default.RankingList = JsonSerializer.Serialize(RankDic);
      Properties.Settings.Default.Save();
    }
  }
}
