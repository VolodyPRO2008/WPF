using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Models
{
    public enum CellState { Empty, Ship, Hit, Miss, HiddenShip, ShipsDead }
    public class Cell : INotifyPropertyChanged
    {


        public int _yIndex { get; set; }
        public int _xIndex { get; set;}
        private CellState _state = CellState.Empty;

        public CellState State 
        {
            get => _state; 
            set { _state = value; OnPropertyChanged();OnPropertyChanged(nameof(Color));}      
        }

        public Brush Color
        {
            get
            {
                switch (_state)
                {
                    case CellState.Hit:
                        return Brushes.Red;
                    case CellState.Miss:
                        return Brushes.Gray;
                    case CellState.Ship:
                        return Brushes.Green;
                    case CellState.HiddenShip: 
                        return Brushes.White;
                    case CellState.ShipsDead:
                        return Brushes.Black;
                    default: 
                        return Brushes.White;
                }   
                ;
            }
        }

        public Cell(int rowIndex, int columnIndex)
        {
            _yIndex = rowIndex;
            _xIndex = columnIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}