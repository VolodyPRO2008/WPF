using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;
using WpfApp1.Properties;

namespace WpfApp1
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cell> PlayerCells { get; } = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> EnemyCells { get; } = new ObservableCollection<Cell>();
        public List<Ship> EnemyShips { get; } = new List<Ship>();


        private Random _random = new Random();

        public ICommand NewColor { get; }

        public MainViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PlayerCells.Add(new Cell(i, j));
                    EnemyCells.Add(new Cell(i, j));
                }
            }
            NewColor = new RelayCommand<Cell>(MyColor);

            List<Ship> GenerateShips()
            {
                int[] dx = { 0, -1, -1, -1, 0, 0, 1, 1, 1 };
                int[] dy = { 0, -1, 0, 1, -1, 1, -1, 0, 1 }; // массивы для просмотра рядом есть корабли или нет

                for (int i = 4; i > 0; i--) // вид корабля
                {
                    for (int j = 1; j <= 4; j++) // количество кораблей
                    {
                        while (true)
                        {
                            int x = _random.Next(0, 9);
                            int y = _random.Next(0, 9);

                            int spatialOrientation = _random.Next(0, 3);
                            bool canPlace = true;

                            List<Cell> cellForShips = new List<Cell>();

                            for (int k = 0; k < i; k++)
                            {
                                int currentX = x;
                                int currentY = y;
                                switch (spatialOrientation)
                                {
                                    case 0: if (y >= i) { currentY = y - k; break; } else { canPlace = false; break ; } // корабль смотрит вверх
                                    case 1: if (x >= i) { currentX = x - k; break; } else { canPlace = false; break ; } // корабль смотрит влево
                                    case 2: if (y <= 10 - i) { currentY = y + k; break; } else { canPlace = false; break; } // корабль смотрит вниз 
                                    case 3: if (x <= 10 - i) { currentX = x + k; break; } else { canPlace = false; break; } // корабль смотрит вправо
                                }

                                for (int h = 0; h < 9; h++)
                                {
                                    int nx = currentX + dx[h];
                                    int ny = currentY + dy[h];

                                    if (EnemyCells[ny * 10 + nx].State == CellState.Ship || canPlace == false)
                                    {
                                        canPlace = false;
                                        break;
                                    }
                                }
                                if (canPlace == false)
                                {
                                    break;
                                }
                                cellForShips.Add(EnemyCells[currentY * 10 + currentX]);
                            }

                            if (canPlace == true)
                            {
                                foreach (var item in cellForShips)
                                {
                                    item.State = CellState.Ship;
                                    ;
                                }
                            }

                        }
                    }
                }
                return 
            }
        }


        public void ComputerShooting(int x, int y, CellState newState, Cell cell)
        {
            PlayerCells[(x - 1) * 10 + (y - 1)].State = CellState.Ship;

        }





        private void MyColor(Cell cell)
        {
            if (cell.Color == Brushes.Red)
            {
                cell.State = CellState.Miss;
                ComputerShooting(3, 3, CellState.Ship, cell);
            }
            else
            {
                cell.State = CellState.Hit;
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
