using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;
using System.IO;

namespace TankGame.Tools
{
    internal class Pathfinder
    {
        private StreamWriter writer;
        public Cell[,] cellMap;
        private Cell[,] originalCellMap;

        public Pathfinder(Cell[,] CellMap)
        {
            cellMap = new Cell[CellMap.GetLength(0), CellMap.GetLength(1)];
            originalCellMap = CellMap;
            for (int i = 0; i < cellMap.GetLength(0); i++)
            {
                for (int j = 0; j < cellMap.GetLength(1); j++)
                {
                    cellMap[i, j] = new Cell(CellMap[i, j].X, CellMap[i, j].Y, CellMap[i, j].Cost);
                    if (CellMap[i, j].Identifier == 1)
                    {
                        cellMap[i, j].Identifier = 1;
                    }
                }
            }
            
        }

        public List<Cell> getPath(Cell start, Cell end)
        {
            string FileLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TankGame\\log.txt";
            if (File.Exists(FileLocation))
            { File.Delete(FileLocation); }
            //createfile and then write to it
            File.Create(FileLocation).Close();
            writer = new StreamWriter(FileLocation);
            //
            Cell curCell;
            List<Cell> openList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();
            List<Cell> neighborList = new List<Cell>();
            start.RawDistance(end.X, end.Y);
            openList.Add(start);

            //loop while we still have cells to check
            while (openList.Count > 0)
            {

                //find the index with the lowest cost distance in the to check list
                curCell = findLowestCostDistance(openList);
                //
                writer.WriteLine(Convert.ToString(curCell.X) + "," + Convert.ToString(curCell.Y));
                //
                //if we found the goal then return the path
                if (curCell.X == end.X && curCell.Y == end.Y)
                {
                    return constructPath(curCell, start);
                }
                //remove it from the list of cells to check since we are checking it now
                openList.Remove(curCell);
                //add it to the list of checked cells
                closedList.Add(curCell);
                //check the neighbor cells of the current cell
                neighborList = getNeighborCells(curCell, start);
                foreach (Cell nCell in neighborList)
                {
                    nCell.RawDistance(end.X, end.Y); //gets the distance to it can return costDistance

                    if (openList.Contains(nCell))
                    {
                        if(nCell.CostDistance <= curCell.CostDistance)
                        {
                            break;
                        }
                    }
                    else if(closedList.Contains(nCell))
                    {
                        if (nCell.CostDistance <= curCell.CostDistance)
                        {
                            break;
                        }
                        openList.Add(nCell);
                    }
                    else
                    {
                        openList.Add(nCell);
                    }
                        if (curCell.Parent != nCell) //prevent cells from being eachothers parent
                        {
                            nCell.Parent = curCell;
                        }                   
                }
                closedList.Add(curCell);
            }
            //create and return an empty list if there is no path
            List<Cell> emptyList = new List<Cell>();
            resetCellMap();
            writer.Close();
            return emptyList;
        }

        public List<Cell> constructPath(Cell endCell, Cell startCell)
        {
            writer.Close();
            List<Cell> path = new List<Cell>();
            path.Add(endCell);
            int tracker = 0;
            while (endCell.Parent != null)
            {
                //if the parent exists set the end cell the parent to construct a path backwards
                endCell = endCell.Parent;
                //add the new cell that was a parent to the path
                path.Add(endCell);
                //and check if there is a new parent when looping
                tracker++;
                if (tracker == 1000)
                {

                }
                if (endCell.X == startCell.X && endCell.Y == startCell.Y)
                {
                    break;
                }
            }
            //when there is no more parents we are at the start cell and return the path
            resetCellMap();
            return path;

        }
        /// <summary>
        /// gets the nieghbor cells without obstacles in them using the cellMap
        /// </summary>
        /// <param name="originCell">the cell looking for neighbors</param>
        /// <returns></returns>
        public List<Cell> getNeighborCells(Cell originCell, Cell startCell)
        {
            //startCell.
            //create a list to add the neighbors too
            List<Cell> neighbors = new List<Cell>();
            Cell neighborCell;
            //get the cords for the first nieghbor cell to check
            int iStart = originCell.X-1;
            int jStart = originCell.Y-1;

            //checking from i and 2 more cells over
            for (int i = iStart; i < iStart + 3; i++)
            {
                for (int j = jStart; j < jStart + 3; j++)
                {
                    if (i >= 0 && i < cellMap.GetLength(0) && j >= 0 && j < cellMap.GetLength(1))
                    {
                        neighborCell = cellMap[i, j];

                        //check if the cell is free from obstacles and isnt the originCell
                        if (neighborCell.Identifier == 0 && neighborCell != originCell)
                        {
                            //if its a diagonal make it cost more
                            if (neighborCell.X == originCell.X || neighborCell.Y == originCell.Y) //if true its not diagonal
                            {
                                neighborCell.Cost = 1;                               
                            }
                            else
                            {
                                neighborCell.Cost = 1.40;
                            }
                            //add it to neighbor
                            neighbors.Add(neighborCell);

                        }
                        else if (neighborCell.Identifier == 1)
                        {

                        }
                    }
                }
            }
            return neighbors;            
        }
        /// <summary>
        /// finds the lowest cost cell towards the final destination in the in the cell check list
        /// </summary>
        /// <param name="cells">the current list of cells to check</param>
        /// <param name="target">the cell that is the final destination</param>
        private Cell findLowestCostDistance(List<Cell> cells)
        {
            //lowcost is the current lowest recorded. -1 means none recorded
            double lowCost = -1;
            Cell selectedCell = new Cell(-1,-1,-1);
            foreach (Cell cell in cells)
            {
                if (lowCost == -1)
                {
                    lowCost = cell.CostDistance;
                    selectedCell = cell;
                }
                else
                {
                    //compare new cells cost distance to the current lowest
                    if (lowCost > cell.CostDistance)
                    {
                        //if the new cost distance is lower then set the lowcost to that
                        lowCost = cell.CostDistance;
                        selectedCell = cell;
                    }
                    else if (lowCost == cell.CostDistance)
                    {
                        if (selectedCell.Parent != null)
                        {
                            if (selectedCell.Parent.X != selectedCell.X && selectedCell.Parent.Y != selectedCell.Y)
                            {
                                if (cell.Parent != null)
                                {
                                    if (cell.Parent.X == cell.X || cell.Parent.Y == cell.Y)
                                    {
                                        selectedCell = cell;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return selectedCell;
        }
        private void resetCellMap()
        {
            for (int i = 0; i < cellMap.GetLength(0); i++)
            {
                for (int j = 0; j < cellMap.GetLength(1); j++)
                {
                    cellMap[i, j] = new Cell(originalCellMap[i, j].X, originalCellMap[i, j].Y, originalCellMap[i, j].Cost);
                    if (originalCellMap[i,j].Identifier == 1)
                    {
                        cellMap[i, j].Identifier = 1;
                    }
                }
            }
        }
    }
}
