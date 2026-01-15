using Minesweeper.Class;
using Minesweeper.Models;
using Minesweeper.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        MouseButtonState leftButton = MouseButtonState.Released;
        MouseButtonState rightButton = MouseButtonState.Released;
        readonly List<Cell?> lastPressList = new List<Cell?>();
        readonly List<Cell> openCellList = new List<Cell>(100);
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < GameInfo.Instance.MaxCell; i++)
            {
                GameInfo.Instance.CellList.Add(CellPool.GetCell(i));
            }
            ReCalculateOffset();
            DataContext = GameInfo.Instance;
        }
        public void InitGame()
        {
            GameInfo.Instance.CurrFaceStatus = FaceStatus.Normal;
            while (GameInfo.Instance.CellList.Count < GameInfo.Instance.MaxCell)
                GameInfo.Instance.CellList.Add(CellPool.GetCell(GameInfo.Instance.CellList.Count));
            while (GameInfo.Instance.MaxCell < GameInfo.Instance.CellList.Count)
            {
                CellPool.ReturnCell(GameInfo.Instance.CellList[GameInfo.Instance.CellList.Count - 1]);
                GameInfo.Instance.CellList.RemoveAt(GameInfo.Instance.CellList.Count - 1);
            }
            if (GameInfo.Instance.Started || GameInfo.Instance.GameOver)
                foreach (var cell in GameInfo.Instance.CellList)
                    cell.Init();
            GameInfo.Instance.Started = false;
            GameInfo.Instance.GameOver = false;
            openCellList.Clear();
            GameInfo.Instance.FlagList.Clear();
            GameInfo.Instance.TimeCost = 0;
            ReCalculateOffset();
        }
        private void Swap<T>(List<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
        private void StartGame()
        {
            GameInfo.Instance.Started = true;
            GameInfo.Instance.GameOver = false;
            Cell SeleCell = (Cell)listBox.SelectedItem;
            GameInfo.Instance.BombList.Clear();
            var tempCellList = new List<Cell>(GameInfo.Instance.CellList);//用于快速计算布雷
            int tail = GameInfo.Instance.CellList.Count - 1;
            Swap(tempCellList, listBox.SelectedIndex, tail);//不让被点击的是雷，所以直接换到后面
                                                            //布雷
            for (int i = 0; i < GameInfo.Instance.BombCount; i++)
            {
                tail--;
                int bombIndex = Random.Shared.Next(0, tail);
                var bombCell = tempCellList[bombIndex];
                bombCell.IsBomb = true;
                GameInfo.Instance.BombList.Add(bombCell);
                Swap(tempCellList, bombIndex, tail);

                foreach (var cell in GetAroundValidCell(bombCell))
                {
                    if (!cell.IsBomb)
                    {
                        cell.AroundBombNum++;
                    }
                }
            }
        }
        private void ReCalculateOffset()
        {
            GameInfo.Instance.OffsetList.Clear();
            ///九宫格的偏移位置
            GameInfo.Instance.OffsetList.Add(-GameInfo.Instance.Column - 1);
            GameInfo.Instance.OffsetList.Add(-GameInfo.Instance.Column);
            GameInfo.Instance.OffsetList.Add(-GameInfo.Instance.Column + 1);

            GameInfo.Instance.OffsetList.Add(-1);
            GameInfo.Instance.OffsetList.Add(0);
            GameInfo.Instance.OffsetList.Add(1);

            GameInfo.Instance.OffsetList.Add(GameInfo.Instance.Column - 1);
            GameInfo.Instance.OffsetList.Add(GameInfo.Instance.Column);
            GameInfo.Instance.OffsetList.Add(GameInfo.Instance.Column + 1);
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
        private List<Cell> GetUDLRCell(Cell cell)
        {
            var result = new List<Cell>();
            for (int i = 1; i < 8; i += 2)
            {
                int offset = cell.Index + GameInfo.Instance.OffsetList[i];
                int offsetRow = i / 3;
                if (offset >= 0 && offset < GameInfo.Instance.MaxCell &&
                  offset / GameInfo.Instance.Column == (int)Math.Floor(((float)GameInfo.Instance.OffsetList[offsetRow * 3 + 1] + cell.Index) / GameInfo.Instance.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
                {
                    result.Add(GameInfo.Instance.CellList[offset]);
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

            for (int i = 0; i < GameInfo.Instance.OffsetList.Count; i++)
            {
                int offset = cell.Index + GameInfo.Instance.OffsetList[i];
                int offsetRow = i / 3;
                if (offset >= 0 && offset < GameInfo.Instance.MaxCell &&
                  offset / GameInfo.Instance.Column == (int)Math.Floor(((float)GameInfo.Instance.OffsetList[offsetRow * 3 + 1] + cell.Index) / GameInfo.Instance.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
                {
                    result.Add(GameInfo.Instance.CellList[offset]);
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
            for (int i = 0; i < GameInfo.Instance.OffsetList.Count; i++)
            {
                int offset = cell.Index + GameInfo.Instance.OffsetList[i];
                int offsetRow = i / 3;
                if (offset >= 0 && offset < GameInfo.Instance.MaxCell &&
                  offset / GameInfo.Instance.Column == (int)Math.Floor(((float)GameInfo.Instance.OffsetList[offsetRow * 3 + 1] + cell.Index) / GameInfo.Instance.Column))//格子位置在最小、最大值范围内，并且九宫格有三行，该偏移量的位置和该行中间的偏移量对列数的商相等，说明没有换行
                {
                    result.Add(GameInfo.Instance.CellList[offset]);
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
                GameInfo.Instance.CurrFaceStatus = FaceStatus.Dead;
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
            if (openCellList.Count == GameInfo.Instance.MaxCell - GameInfo.Instance.BombCount)
                GameOver(true);
        }
        private void GameOver(bool winOrlose)
        {
            if (GameInfo.Instance.GameOver)
                return;

            GameInfo.Instance.WinOrLose = winOrlose;
            if (winOrlose)
                GameInfo.Instance.CurrFaceStatus = FaceStatus.Win;
            else
                GameInfo.Instance.CurrFaceStatus = FaceStatus.Dead;

            GameInfo.Instance.GameOver = true;
            openCellList.Clear();
            foreach (var cell in GameInfo.Instance.BombList)
            {
                if (winOrlose)
                {
                    cell.CellMark = CellMark.Flag;
                }
                else
                {
                    if (cell.CellMark != CellMark.Flag)
                        cell.IsOpened = true;
                }
            }

            if (!winOrlose)
                return;

            string gameScaleStr = "";
            if (GameInfo.Instance.Row == 9 && GameInfo.Instance.Column == 9 && GameInfo.Instance.BombCount == 10)
                gameScaleStr = "初级";
            else if (GameInfo.Instance.Row == 16 && GameInfo.Instance.Column == 16 && GameInfo.Instance.BombCount == 40)
                gameScaleStr = "中级";
            else if (GameInfo.Instance.Row == 16 && GameInfo.Instance.Column == 30 && GameInfo.Instance.BombCount == 99)
                gameScaleStr = "高级";
            else
                return;
            //新纪录
            if (!RankManager.RankDic.ContainsKey(gameScaleStr) ||
              RankManager.RankDic[gameScaleStr].Duration > GameInfo.Instance.TimeCost)
            {
                var recoredWind = new NewRecordWindow();
                recoredWind.DataContext = gameScaleStr;
                recoredWind.ShowDialog();
                RankManager.RankDic[gameScaleStr] = new ScoreRecord() { Duration = GameInfo.Instance.TimeCost };
            }
        }

        #region 控件事件处理器
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
                if (!GameInfo.Instance.Started)
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
                        if (GameInfo.Instance.UseMark)
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
            InitGame();
        }
        private void listBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            listBox.SelectedItem = null;
        }
        private void CustomMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomGameWindow customGameWindow = new CustomGameWindow(this);
            customGameWindow.ShowDialog();
        }
        private void me_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !GameInfo.Instance.GameOver)
                GameInfo.Instance.CurrFaceStatus = FaceStatus.Clicked;
        }
        private void me_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!GameInfo.Instance.GameOver)
                GameInfo.Instance.CurrFaceStatus = FaceStatus.Normal;
        }

        private void StandardDifficItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header)
            {
                case "初级":
                    GameInfo.Instance.Row = 9;
                    GameInfo.Instance.Column = 9;
                    GameInfo.Instance.BombCount = 10;
                    break;
                case "中级":
                    GameInfo.Instance.Row = 16;
                    GameInfo.Instance.Column = 16;
                    GameInfo.Instance.BombCount = 40;
                    break;
                case "高级":
                    GameInfo.Instance.Row = 16;
                    GameInfo.Instance.Column = 30;
                    GameInfo.Instance.BombCount = 99;
                    break;
            }
            InitGame();
            //ReCalculateOffset();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MarkMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GameInfo.Instance.UseMark = !GameInfo.Instance.UseMark;
        }

        private void RankMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new RankingListWindow().ShowDialog();
        }

        private void AllFlagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (GameInfo.Instance.GameOver)
                return;

            foreach (var item in GameInfo.Instance.CellList)
            {
                if (item.CellMark != CellMark.Flag && !item.IsOpened)
                {
                    item.CellMark = CellMark.Flag;
                }
            }
        }

        private void CancelAllFlagMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (GameInfo.Instance.GameOver)
                return;

            for (int i = GameInfo.Instance.FlagList.Count - 1; i > -1; i--)
                GameInfo.Instance.FlagList[i].CellMark = CellMark.None;
        }
        #endregion


    }
}
