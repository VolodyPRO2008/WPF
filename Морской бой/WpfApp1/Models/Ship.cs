using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Ship
    {

        private List<Cell> _cells;


        public bool IsDestroyed
        {
            get
            {
                foreach (var cell in _cells)
                {
                    if (cell.State != CellState.Hit && cell.State != CellState.ShipsDead)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public List<Cell> Cells
        {
            get => _cells;
        }
        public Ship(List<Cell> cells) 
        {
            _cells = cells;
        }

    }
}
