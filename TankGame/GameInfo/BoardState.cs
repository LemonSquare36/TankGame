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
        public List<Entity> entities = new List<Entity>();

        public List<Point> gridLocations = new List<Point>();
        public List<Point> wallLocations = new List<Point>();
        public List<Point> tankGridLocations = new List<Point>();
        #endregion

        public List<Player> playerList = new List<Player>();
        public int curPlayerNum;

        public BoardState(List<Entity> Entities, List<Wall> Walls, List<ItemBox> ItemBoxes)
        {
            curPlayerNum = 0;
            entities = Entities;
            walls = Walls;
            itemBoxes = ItemBoxes;

            getWallLocations();
        }
        public static BoardState SavePreviousBoardState(BoardState curBoardState)
        {
            //create new boardstate to become the previous state and take the values from the current boardstate
            BoardState @newBoardState = new BoardState(new(), new(), new());
            //clone the players in the player list
            for (int i = 0; i < curBoardState.playerList.Count; i++)
            {
                @newBoardState.playerList.Add(new Player((int)curBoardState.playerList[i].AP, curBoardState.playerList[i].inventory.sweeps));
                foreach (Tank tank in curBoardState.playerList[i].tanks)
                {
                    @newBoardState.playerList[i].tanks.Add(Tank.Clone(tank));
                }
                foreach (Mine mine in curBoardState.playerList[i].mines)
                {
                    @newBoardState.playerList[i].mines.Add(Mine.Clone(mine));
                }
                foreach (Items item in curBoardState.playerList[i].Items)
                {
                    //@newBoardState.playerList[i].Items.Add(item.Clone(item));
                }
            }
            //clone the other lists boardState needs
            //entities list
            foreach (Entity entity in curBoardState.entities)
                @newBoardState.entities.Add(Entity.Clone(entity));
            //walls list
            foreach (Wall wall in curBoardState.walls)
                @newBoardState.walls.Add(Wall.Clone(wall));
            //itemBox list
            foreach (ItemBox itemBox in curBoardState.itemBoxes)
                @newBoardState.itemBoxes.Add(ItemBox.Clone(itemBox));
            //curplayerNum Clone
            @newBoardState.curPlayerNum = curBoardState.curPlayerNum;

            return newBoardState;
        }

        public void SetToPreviousState(BoardState previousState)
        {
            //clear the current lists of values in the current state
            gridLocations.Clear();
            itemBoxes.Clear();
            walls.Clear();

            //clear player lists
            foreach (Player player in playerList)
            {
                player.tanks.Clear();
            }
            //clone to values from the previous state into the current state
            //create clones instead of references for the items and tanks to prevent unwanted list updates
            foreach (Items item in previousState.playerList[curPlayerNum].Items)
            {
                //playerList[curPlayer].items.Add(new Items(item))
            }
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
                walls.Add(Wall.Clone(wall));
            }
            getWallLocations();
            //get itemboxes
            foreach (ItemBox itembox in previousState.itemBoxes)
            {
                itemBoxes.Add(ItemBox.Clone(itembox));
            }

            //redo all entities list
            getAllEntities();
            //redo the gridlocations list
            getGridLocations();

            playerList[curPlayerNum].AP = previousState.playerList[curPlayerNum].AP;
        }
        #region information grabbing
        public List<Point> getWallLocations()
        {
            wallLocations.Clear();
            foreach (Wall w in walls)
            {
                wallLocations.Add(w.gridLocation);
            }
            return wallLocations;
        }
        /// <summary>updates the entities list based on the other active lists (walls, player.mines, player.tank, itemboxes)</summary>
        protected List<Entity> getAllEntities()
        {
            entities.Clear();

            foreach (Wall w in walls)
            {
                entities.Add(w);
            }
            //add ally tanks
            foreach (Tank tank in playerList[curPlayerNum].tanks)
            {
                entities.Add(tank);
            }
            //add enemy tanks
            for (int i = 0; i < playerList.Count; i++)
            {
                if (i != curPlayerNum)//means its enemy player
                {
                    foreach (Tank tank in playerList[i].tanks)
                    {
                        entities.Add(tank);
                    }
                }
            }
            //add ally mines
            foreach (Mine mine in playerList[curPlayerNum].mines)
            {
                entities.Add(mine);
            }
            //add enemy mines
            for (int i = 0; i < playerList.Count; i++)
            {
                if (i != curPlayerNum)//means its enemy player
                {
                    foreach (Mine mine in playerList[i].mines)
                    {
                        entities.Add(mine);
                    }
                }
            }
            foreach (ItemBox item in itemBoxes)
            {
                entities.Add(item);
            }
            return entities;
        }
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
            for (int i = 0; i < entities.Count; i++)
            {
                gridLocations.Add(entities[i].gridLocation);
            }
        }
        public void LoadEntities()
        {
            foreach (Entity entity in entities)
            {
                entity.LoadContent();
            }
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
        public void AddWall(Point gridPosition, Board curBoard)
        {
            walls.Add(new Wall(curBoard.getGridSquare(gridPosition.X, gridPosition.Y), gridPosition)); //remove from wall object list
            wallLocations.Add(gridPosition); //remove from wall locations (vector2) list
        }
        #endregion


    }
}
