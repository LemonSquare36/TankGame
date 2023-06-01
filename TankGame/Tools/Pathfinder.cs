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
        public Cell[,] cellMap;


        public Pathfinder(Cell[,] CellMap)
        {
            cellMap = CellMap;
        }

        public List<Cell> getPath(Cell start, Cell end)
        {
            Cell curCell;
            List<Cell> checkList = new List<Cell>();
            List<Cell> closedList = new List<Cell>();
            start.RawDistance(end.X, end.Y);
            checkList.Add(start);

            //set starts distance to end to the raw distance with no obstacles
            start.RawDistance(end.X, end.Y);

            //loop while we still have cells to check
            while (checkList.Count > 0)
            {
                //lowestCostDistance = findLowestCostDistance(checkList, end);

                //find the index with the lowest cost distance in the to check list
                curCell = checkList[checkList.FindIndex(a => a.CostDistance == findLowestCostDistance(checkList))];
                //if we found the goal then return the path
                if (curCell == end)
                {
                    return constructPath(curCell);
                }
                //remove it from the list of cells to check since we are checking it now
                checkList.Remove(curCell);
                //add it to the list of checked cells
                closedList.Add(curCell);
                //check the neighbor cells of the current cell
                foreach (Cell nCell in getNeighborCells(curCell))
                {
                    //if the nCell (neighbor cell) has never been looked at
                    if (!closedList.Contains(nCell))
                    {
                        //find its cost distance
                        nCell.RawDistance(end.X, end.Y); //gets the distance to it can return costDistance
                        //if its not in the to check List
                        if (!checkList.Contains(nCell))
                        {
                            //add it to that list
                            checkList.Add(nCell);
                        }
                        else //otherwise
                        {
                            //if the cost isnt always 1 then this is where you would update a faster path using same tiles
                            //my cost is always one to so the shortest path to the tile will always be 1
                        }
                    }
                }
            }
            //create and return an empty list if there is no path
            List<Cell> emptyList = new List<Cell>();
            return emptyList;
        }

        public List<Cell> constructPath(Cell endCell)
        {
            List<Cell> path = new List<Cell>();
            path.Add(endCell);
            
            while (endCell.Parent != null)
            {
                //if the parent exists set the end cell the parent to construct a path backwards
                endCell = endCell.Parent;
                //add the new cell that was a parent to the path
                path.Add(endCell);
                //and check if there is a new parent when looping
            }
            //when there is no more parents we are at the start cell and return the path
            return path;

        }
        /// <summary>
        /// gets the nieghbor cells without obstacles in them using the cellMap
        /// </summary>
        /// <param name="originCell">the cell looking for neighbors</param>
        /// <returns></returns>
        public List<Cell> getNeighborCells(Cell originCell)
        {
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
                    neighborCell = cellMap[i, j];
                    //check if the cell is free from obstacles and isnt the originCell
                    if (neighborCell.Identifier == 0 && neighborCell != originCell)
                    {
                        //if the cell to check is in the bounds of the array
                        if (neighborCell.X >= 0 && neighborCell.X < cellMap.GetLength(0) && neighborCell.Y >= 0 && neighborCell.Y < cellMap.GetLength(1))
                        {
                            //add it to neighbor
                            neighbors.Add(neighborCell);
                            //set the cell it came from. Used for constructing the final path later
                            neighborCell.Parent = originCell;

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
        private int findLowestCostDistance(List<Cell> cells)
        {
            //lowcost is the current lowest recorded. -1 means none recorded
            int lowCost = -1;
            foreach (Cell cell in cells)
            {
                if (lowCost == -1)
                {
                    lowCost = cell.CostDistance;
                }
                else
                {
                    //compare new cells cost distance to the current lowest
                    if (lowCost > cell.CostDistance)
                    {
                        //if the new cost distance is lower then set the lowcost to that
                        lowCost = cell.CostDistance;
                    }
                }
            }
            return lowCost;
        }
    }
}
