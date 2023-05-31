using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;

namespace TankGame.Tools
{
    internal class Pathfinder
    {
        public List<Cell> cellMap;

        public Pathfinder(List<Cell> CellMap)
        {
            cellMap = CellMap;
        }

        public List<Cell> getPath(Cell start, Cell end)
        {
            List<Cell> checkList = new List<Cell>();
            checkList.Add(start);
            //set starts distance to end to the raw distance with no obstacles
            start.RawDistance(end.X, end.Y);
            //loop while we still have cells to check
            while (checkList.Count > 0)
            {
              // Cell currentCell = checkList.Find(checkList => checkList.Contains())

            }
        }

        public List<Cell> constructPath(Cell endCell)
        {

        }

        public List<Cell> getNeighborCells(Cell cell)
        {

        }
    }
}
