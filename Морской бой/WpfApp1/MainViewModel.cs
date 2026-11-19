using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;
using WpfApp1.Properties;

namespace WpfApp1
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public string PlayerName { get; set; }
        
        public ObservableCollection<Cell> PlayerCells { get; } = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> EnemyCells { get; } = new ObservableCollection<Cell>();
        public List<Ship> EnemyShips { get; } = new List<Ship>();
        public List<Ship> PlayerShips { get; } = new List<Ship>();


        private Random _random = new Random();

        int[] dx = { 0, -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { 0, -1, 0, 1, -1, 1, -1, 0, 1 }; // массивы для просмотра рядом есть корабли или нет

        public ICommand NewColor { get; }
        public ICommand NewGameCommand { get; }

        public int PlayerShipsLeft { get { return ShipsLeft(PlayerShips); } }
        public int EnemyShipsLeft { get { return ShipsLeft(EnemyShips); } }



        public MainViewModel(string playerName)

        {
            PlayerName = playerName;

            NewColor = new RelayCommand<Cell>(MyColor);
            NewGameCommand = new RelayCommand<Cell>(_ => NewGame());



            NewGame();
        }

        public int ShipsLeft(List<Ship> User)
        {
            int _shipsLeft = 0;

            foreach (var item in User)
            {
                if (item.IsDestroyed == false)
                {
                    _shipsLeft++;
                }
            }
            return _shipsLeft;
        }



        public List<Ship> GenerateShips(List<Ship> UserShips, ObservableCollection<Cell> UserField, bool isEnemy)
        {

            int t = 0;

            for (int i = 4; i > 0; i--) // вид корабля
            {
                t++;
                for (
                    int j = 1; j <= t; j++) // количество кораблей
                {
                    while (true)
                    {
                        int x = _random.Next(0, 10);
                        int y = _random.Next(0, 10);

                        int spatialOrientation = _random.Next(0, 4);
                        bool canPlace = true;

                        List<Cell> cellForShips = new List<Cell>();


                        for (int k = 0; k < i; k++)
                        {
                            int currentX = x;
                            int currentY = y;
                            switch (spatialOrientation)
                            {
                                case 0: if (y >= (i - 1)) { currentY = y - k; break; } else { canPlace = false; break; } // корабль смотрит вверх
                                case 1: if (x >= (i - 1)) { currentX = x - k; break; } else { canPlace = false; break; } // корабль смотрит влево
                                case 2: if (y <= 10 - i) { currentY = y + k; break; } else { canPlace = false; break; } // корабль смотрит вниз 
                                case 3: if (x <= 10 - i) { currentX = x + k; break; } else { canPlace = false; break; } // корабль смотрит вправо
                            }
                            if (canPlace == false)
                            {
                                break;

                            }

                            for (int h = 0; h < 9; h++)
                            {
                                int nx = currentX + dx[h];
                                int ny = currentY + dy[h];
                                if (ny >= 10 || ny < 0 || nx >= 10 || nx < 0)
                                {
                                    continue;
                                }

                                if (UserField[ny * 10 + nx].State == CellState.Ship || UserField[ny * 10 + nx].State == CellState.HiddenShip || canPlace == false)
                                {
                                    canPlace = false;
                                    break;
                                }
                            }
                            if (canPlace == false)
                            {
                                break;
                            }
                            cellForShips.Add(UserField[currentY * 10 + currentX]);
                        }

                        if (canPlace == true)
                        {
                            switch (isEnemy)
                            {
                                case true:
                                    foreach (var item in cellForShips)
                                    {
                                        item.State = CellState.HiddenShip;
                                    }
                                    break;
                                case false:
                                    foreach (var item in cellForShips)
                                    {
                                        item.State = CellState.Ship;
                                    }
                                    break;
                            }

                            UserShips.Add(new Ship(cellForShips));
                            break;
                        }
                    }

                }

            }
            return UserShips;
        }

        public void ComputerShooting()
        {
            while (true) {

                int x = _random.Next(0, 10);
                int y = _random.Next(0, 10);



                if (PlayerCells[y * 10 + x].State == CellState.Ship)
                {
                    PlayerCells[y * 10 + x].State = CellState.Hit;
                    DeadShip(PlayerShips, PlayerCells);
                    if (Vicroty(PlayerShips) == true)
                    {
                        MessageBox.Show("Комьютер победил");
                        (Application.Current.MainWindow as MainWindow).ReturnToStart();
                    }
                }
                else if (PlayerCells[y * 10 + x].State == CellState.Empty)
                {
                    PlayerCells[y * 10 + x].State = CellState.Miss;
                    break;
                }
                else
                {
                    continue;
                }
            }
            OnPropertyChanged(nameof(PlayerShipsLeft));
        }





        private void MyColor(Cell cell)
        {
            if (cell.State == CellState.Hit || cell.State == CellState.Ship || cell.State == CellState.Miss)
            {
                return;
            }
            else
            {
                if (cell.State == CellState.HiddenShip)
                {
                    cell.State = CellState.Hit;

                    DeadShip(EnemyShips, EnemyCells);
                    if (Vicroty(EnemyShips) == true)
                    {
                        MessageBox.Show($"{PlayerName} победил");
                        (Application.Current.MainWindow as MainWindow ).ReturnToStart();
                    }

                }
                else if (cell.State == CellState.Empty)
                {
                    cell.State = CellState.Miss;
                    ComputerShooting();
                }
                else
                {
                    return;
                }
            }
            OnPropertyChanged(nameof(EnemyShipsLeft));
        }

        private bool Vicroty(List<Ship> EnemyUserShip)
        {
            int _damagedShips = 0;

            foreach (var item in EnemyUserShip)
            {
                if (item.IsDestroyed == true)
                {
                    _damagedShips++;
                }
            }
            if (_damagedShips == EnemyUserShip.Count)
            {
                return true;
            }
            else { return false; }
        }

        private void DeadShip(List<Ship> User, ObservableCollection<Cell> UserField)
        {
            foreach (var item in User)
            {
                if (item.IsDestroyed == true)
                {
                    foreach (var cells in item.Cells)
                    {
                        cells.State = CellState.ShipsDead;
                        for (int i = 1; i < 9; i++)
                        {
                            int nx = cells._xIndex + dx[i];
                            int ny = cells._yIndex + dy[i];
                            if (ny >= 10 || ny < 0 || nx >= 10 || nx < 0)
                            {
                                continue;
                            }
                            int index = ny * 10 + nx;
                            if (UserField[index].State == CellState.Empty)
                            {
                                UserField[index].State = CellState.Miss;
                            }
                        }
                    }
                }
            }
        }

        private void NewGame()
        {
            PlayerCells.Clear();
            EnemyCells.Clear();
            PlayerShips.Clear();
            EnemyShips.Clear();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PlayerCells.Add(new Cell(i, j));
                    EnemyCells.Add(new Cell(i, j));
                }
            }
            GenerateShips(PlayerShips, PlayerCells, false);
            GenerateShips(EnemyShips, EnemyCells, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}