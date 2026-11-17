using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfApp1.Models
{
    public enum CellState { Empty, Ship, Hit, Miss }
    public class Cell : INotifyPropertyChanged
    {


        public int _rowIndex { get; set; }
        public int _rolumnIndex { get; set;}
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
                        return Brushes.Green;
                    case CellState.Ship: 
                        return Brushes.Blue;
                    default: 
                        return Brushes.White;
                }   
                ;
            }
        }

        public Cell(int rowIndex, int columnIndex)
        {
            _rowIndex = rowIndex;
            _rolumnIndex = columnIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}