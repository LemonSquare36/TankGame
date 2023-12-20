using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.Objects;
using TankGame.Objects.Entities.Items;

namespace TankGame.GameInfo
{
    internal class BoardState
    {
        #region board Information
        public List<ItemBox> itemBoxes = new List<ItemBox>();
        public List<Wall> walls = new List<Wall>();
        public List<Hole> holes = new List<Hole>();

        public List<Point> gridLocations = new List<Point>();
        public List<Point> wallLocations = new List<Point>();
        public List<Point> tankGridLocations = new List<Point>();

        public List<Entity> objectsInLOS = new List<Entity>();
        #endregion

        public List<Player> playerList = new List<Player>();
        public int curPlayerNum;

        public BoardState(List<Wall> Walls, List<ItemBox> ItemBoxes, List<Hole> Holes)
        {
            curPlayerNum = 0;
            walls = Walls;
            itemBoxes = ItemBoxes;
            holes = Holes;

            getWallLocations();

        }
        public static BoardState SavePreviousBoardState(BoardState curBoardState, Board curBoard)
        {
            //create new boardstate to become the previous state and take the values from the current boardstate
            BoardState @newBoardState = new BoardState(new(), new(), new());
            //clone the players in the player list
            for (int i = 0; i < curBoardState.playerList.Count; i++)
            {
                @newBoardState.playerList.Add(new Player(curBoardState.playerList[i].inventory.sweeps));
                foreach (Tank tank in curBoardState.playerList[i].tanks)
                {
                    @newBoardState.playerList[i].tanks.Add(Tank.Clone(tank));
                }
                foreach (Mine mine in curBoardState.playerList[i].mines)
                {
                    @newBoardState.playerList[i].mines.Add(Mine.Clone(mine));
                }
                //inventory clone for players
                @newBoardState.playerList[i].inventory = Inventory.Clone(curBoardState.playerList[i].inventory);
            }
            //clone the other lists boardState needs
            //walls list
            foreach (Wall wall in curBoardState.walls)
                @newBoardState.walls.Add(Wall.Clone(wall, curBoard));
            //itemBox list
            foreach (ItemBox itemBox in curBoardState.itemBoxes)
                @newBoardState.itemBoxes.Add(ItemBox.Clone(itemBox));
            //curplayerNum Clone
            @newBoardState.curPlayerNum = curBoardState.curPlayerNum;

            return newBoardState;
        }

        public void SetToPreviousState(BoardState previousState, Board curBoard)
        {
            //clear the current lists of values in the current state
            gridLocations.Clear();
            itemBoxes.Clear();
            walls.Clear();

            //clear player lists
            for  (int i = 0; i < playerList.Count; i++)
            {
                playerList[i].tanks.Clear();
                //remember the sweeps they had before going back
                int tempSweeps = playerList[i].inventory.sweeps;
                int tempSuperSweeps = playerList[i].inventory.superSweeper;

                playerList[i].inventory = previousState.playerList[i].inventory;
                //that way we can reassign those values to prevent sweepers from getting undone
                playerList[i].inventory.superSweeper = tempSuperSweeps;
                playerList[i].inventory.sweeps = tempSweeps;
            }
            //clone to values from the previous state into the current state
            //create clones instead of references for the mines and tanks to prevent unwanted list updates
            //ally tanks from previous state get set to current
            foreach (Tank tank in previousState.playerList[curPlayerNum].tanks)
            {
                playerList[curPlayerNum].tanks.Add(Tank.Clone(tank));
            }
            //cycle through enemies and take thier old tanks and set them as current
            for (int i = 0; i < previousState.playerList.Count; i++)
            {
                if (i != curPlayerNum)
                {
                    foreach (Tank tank in previousState.playerList[i].tanks)
                    {
                        playerList[i].tanks.Add(Tank.Clone(tank));
                    }
                }
            }
            //get walls
            foreach (Wall wall in previousState.walls)
            {
                walls.Add(Wall.Clone(wall, curBoard));
            }
            getWallLocations();
            //get itemboxes
            foreach (ItemBox itembox in previousState.itemBoxes)
            {
                itemBoxes.Add(ItemBox.Clone(itembox));
            }

            //redo the gridlocations list
            getGridLocations();
        }
        #region information grabbing
        public List<Point> getWallLocations()
        {
            wallLocations.Clear();
            foreach (Wall w in walls)
            {
                if (!w.multiWall)
                wallLocations.Add(w.gridLocation);
                else
                {
                    foreach (Point point in w.gridLocations)
                    {
                        wallLocations.Add(point);
                    }
                }
            }
            return wallLocations;
        }
        /// <summary>updates the entities list based on the other active lists (walls, player.mines, player.tank, itemboxes)</summary>

        public List<Point> getTankLocations()
        {
            tankGridLocations.Clear();
            //take all the grid locations of tanks for each of the players and put them into a list of points
            foreach (Player player in playerList)
            {
                foreach (Tank tank in player.tanks)
                {
                    tankGridLocations.Add(tank.gridLocation);
                }
            }
            return tankGridLocations;
        }
        public void getGridLocations()
        {
            gridLocations.Clear();

            getWallLocations();
            foreach (Point w in wallLocations)
            {
                gridLocations.Add(w);
            }
            foreach (Hole hole in holes)
            {
                gridLocations.Add(hole.gridLocation);
            }
            foreach (ItemBox item in itemBoxes)
            {
                gridLocations.Add(item.gridLocation);
            }
            try
            {
                //add ally tanks
                foreach (Tank tank in playerList[curPlayerNum].tanks)
                {
                    gridLocations.Add(tank.gridLocation);
                }
                //add enemy tanks
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (i != curPlayerNum)//means its enemy player
                    {
                        foreach (Tank tank in playerList[i].tanks)
                        {
                            gridLocations.Add(tank.gridLocation);
                        }
                    }
                }
                //add ally mines
                foreach (Mine mine in playerList[curPlayerNum].mines)
                {
                    gridLocations.Add(mine.gridLocation);
                }
                //add enemy mines
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (i != curPlayerNum)//means its enemy player
                    {
                        foreach (Mine mine in playerList[i].mines)
                        {
                            gridLocations.Add(mine.gridLocation);
                        }
                    }
                }
            }
            catch { }
        }
        public void LoadEntities()
        {
            foreach (Wall w in walls)
            {
               w.LoadContent();
            }
            foreach (Hole h in holes)
            {
                h.LoadContent();
            }
            foreach (ItemBox item in itemBoxes)
            {
                item.LoadContent();
            }
            try
            {
                //add ally tanks
                foreach (Tank tank in playerList[curPlayerNum].tanks)
                {
                    tank.LoadContent();
                }
                //add enemy tanks
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (i != curPlayerNum)//means its enemy player
                    {
                        foreach (Tank tank in playerList[i].tanks)
                        {
                            tank.LoadContent();
                        }
                    }
                }
                //add ally mines
                foreach (Mine mine in playerList[curPlayerNum].mines)
                {
                    mine.LoadContent();
                }
                //add enemy mines
                for (int i = 0; i < playerList.Count; i++)
                {
                    if (i != curPlayerNum)//means its enemy player
                    {
                        foreach (Mine mine in playerList[i].mines)
                        {
                            mine.LoadContent();
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region add and removal
        /// <summary></summary>
        /// <param name="i">the list position of the wall you want to remove</param>
        public void RemoveWall(int i)
        {
            walls.Remove(walls[i]); //remove from wall object list
            wallLocations.Remove(wallLocations[i]); //remove from wall locations (vector2) list
        }
        public void AddWall(Point gridPosition, Board curBoard, bool destroyable)
        {
            walls.Add(new Wall(curBoard.getGridSquare(gridPosition.X, gridPosition.Y), gridPosition, destroyable)); //remove from wall object list
            wallLocations.Add(gridPosition); //remove from wall locations (vector2) list
        }
        #endregion


    }
}
