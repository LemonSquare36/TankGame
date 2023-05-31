using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using System.Threading.Tasks;

namespace TankGame
{
    internal class GameScreensManager : ScreenManager
    {
        //hold the level file location
        private string file;
        //variables needed by the the local and networking battlescreens
        protected Player curPlayer, P1, P2, enemyPlayer;
        //Action points per turn
        int AP = 4;
        //turn tracker
        int activeTankNum = -1;

        //info storage for circles in the grid
        protected List<Point> wallLocations = new List<Point>();
        protected List<Point> tankLocations = new List<Point>();
        protected List<Point> objectLocations = new List<Point>();
        protected List<Vector2> blockersInCircle = new List<Vector2>();
        protected List<Vector2> wallsInGrid = new List<Vector2>();
        protected RectangleF[,] CircleTiles;
        protected RectangleF[,] tankMoveSubGrid;
        protected bool drawTankInfo = false;

        #region base functions
        public override void Initialize()
        {
            base.Initialize();
            //create the player classes
            P1 = new Player(AP, sweeps);
            P2 = new Player(AP, sweeps);
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
        }
        #endregion
        protected void LoadBoardfromFile()
        {
            //load the board and additional data from the file passed in levelselect.
            file = relativePath + "\\TankGame\\" + selectedFile + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.2468F, 0.05F);
                    //grab the informatin from the levelManager
                    entities = levelManager.getEntities();
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    sweeps = levelManager.getSweeps();
                    //finish loading the board
                    curBoard.LoadContent();
                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].LoadContent();
                        gridLocations.Add(entities[i].gridLocation);
                    }
                    RowsCol = curBoard.Rows;
                }
                catch { }
            }
            else { }
        }

        protected List<Point> getWallLocations(List<Entity> entityList)
        {
            List<Point> WallLocations = new List<Point>();
            foreach (Entity e in entityList)
            {
                if (e.Type.ToString() == "wall")
                {
                    WallLocations.Add(e.gridLocation);
                }
            }
            return WallLocations;
        }
        protected List<Point> getTankLocations(List<Tank> player1Tanks, List<Tank> player2Tanks)
        {
            //take all the grid locations from both tank lists of players and put them into a list of points
            List<Point> TankGridLocations = new List<Point>();
            //add player1s tanks to the list
            foreach (Tank tank in player1Tanks)
            {
                TankGridLocations.Add(tank.gridLocation);
            }
            //player 2
            foreach (Tank tank in player2Tanks)
            {
                TankGridLocations.Add(tank.gridLocation);
            }
            return TankGridLocations;
        }

        /// <summary>sets the current selected tank as active and gets the circleTiles(line of sight applied). Sets draw circle to true</summary>
        protected void checkSelectedTank()
        {
            //check each tank for the mouse inside it
            foreach (Tank tank in curPlayer.tanks)
            {
                //returns true if the mouse is inside the tank
                if (tank.curSquare.Contains(worldPosition))
                {
                    //if the mouse is clicked on the tank and the tanks isnt already active
                    if (curLeftClick == ButtonState.Pressed && !tank.Active)
                    {
                        //set all tanks to inactive
                        foreach (Tank tank2 in curPlayer.tanks)
                        {
                            tank2.Active = false;
                        }
                        //set this tank to active
                        tank.Active = true;
                        activeTankNum = curPlayer.getActiveTankNum();
                        //get the walls + current enemy tanks
                        objectLocations = wallLocations;
                        foreach (Point t in tankLocations)
                        {
                            objectLocations.Add(t);
                            //make sure to remove the current selected tank
                            objectLocations.Remove(tank.gridLocation);
                        }
                        //get the circle around the selected tank
                        tankMoveSubGrid = curBoard.getSubGrid(new Vector2(tank.gridLocation.X - tank.range, tank.gridLocation.Y - tank.range), 
                            new Vector2((tank.range * 2) + 1, (tank.range * 2) + 1), objectLocations, out wallsInGrid);                       
                        CircleTiles = curBoard.getRectanglesInRadius(new Vector2(tank.gridLocation.X, tank.gridLocation.Y), tank.range, objectLocations, out blockersInCircle);
                        findTilesInLOS(tank);
                        drawTankInfo = true;
                    }
                }
            }
        }

        /// <summary>Find out which tiles are in the line of sight of the tank (not blocked by walls)</summary>
        protected void findTilesInLOS(Tank selectedTank)
        {
            int defualtRows = CircleTiles.GetLength(0);
            int defualtCols = CircleTiles.GetLength(1);
            //get the center point
            int CenterX = defualtRows / 2;
            int CenterY = defualtCols / 2;
            Vector2 Center = CircleTiles[CenterX, CenterY].Center;

            //use each wall to check what tiles they are blocking
            foreach (Vector2 @object in blockersInCircle)
            {
                //where the loops will start when iterating the 2darray
                int starti = 0;
                int startj = 0;
                int rows = defualtRows;
                int cols = defualtCols;

                int X = (int)@object.X;
                int Y = (int)@object.Y;

                //if the wall is to the left of the center tile
                if (@object.X < CenterX)
                { rows = X+1; } //shrink the iterations so nothing in front sideways is checked

                //if the wall is to the right of the center tile
                else if (@object.X > CenterX)
                { starti = X; } //start the iteration at that value, only checking what is behind the wall

                //if the wall is above the center tile
                if (@object.Y < CenterY)
                { cols = Y+1; } //only check tiles equal or above the wall

                //if the wall is below the center tile
                else if (@object.Y > CenterY)
                { startj = Y; }//only check tiles equal or below the wall

                //if the tile falls on the center for the rows or columns, then leave it alone and check the whole row lenght or column length

                //iterate the 2darray quadrant that the wall is in and check to see if its blocking any blocks
                for (int i = starti; i < rows; i++)
                {
                    for (int j = startj; j < cols; j++)
                    {
                        //if the rectangle isnt null
                        if (!CircleTiles[i, j].Null)
                        {
                            //make sure the wall and current tile checking arent the same
                            if (!(i == X && j == Y))
                            {
                                //check if it is visiable with the current wall as argument
                                Line templine = new Line(CircleTiles[i, j].Center, Center);
                                if (templine.LineSegmentIntersectsRectangle(CircleTiles[X, Y]))
                                {
                                    //if it is blocked then empty/null the rectangle
                                    CircleTiles[i, j] = new RectangleF();
                                }                                
                            }                            
                        }
                    }
                }               
            }
            foreach (Vector2 @object in blockersInCircle)
            {
                //check the walls/tanks to see if thier rectangle isnt null. 
                if (!CircleTiles[(int)@object.X, (int)@object.Y].Null)
                {
                    //If it isnt null then it is in sight and give the identifier of 1 to make it draw different
                    CircleTiles[(int)@object.X, (int)@object.Y].identifier = 1;
                    //get the subgrids location relative to the board to see if a tank is there
                    Point subgridLocation = new Point(selectedTank.gridLocation.X - selectedTank.range, selectedTank.gridLocation.Y - selectedTank.range);
                    //check if the object is a friendly tank and make the rectangle null/untargetable
                    foreach (Tank tank in curPlayer.tanks)
                    {
                        Point tankLocation = tank.gridLocation - subgridLocation;
                        if (tankLocation == new Point((int)@object.X, (int)@object.Y))
                        {
                            CircleTiles[(int)@object.X, (int)@object.Y] = new RectangleF();
                        }
                    }
                    //make the selected tanks rectangle null
                    Point selectedTankLocation = selectedTank.gridLocation - subgridLocation;
                    CircleTiles[selectedTankLocation.X, selectedTankLocation.Y] = new RectangleF();
                }
            }
        }
        protected void pathFind(Vector2 start, Vector2 end)
        {
            //check for any walls or other tanks inside the moveable range
            for (int i = 0; i < tankMoveSubGrid.GetLength(0); i++)
            {
                for (int j = 0; j < tankMoveSubGrid.GetLength(1); j++)
                {
                    //check if the tile is inside the shoot subgrid
                    //if its not null in that one make it null in this one
                    if (!CircleTiles[i,j].Null)
                    {
                        tankMoveSubGrid[i, j] = new RectangleF();
                    }
                    //check if it has a wall there
                    else if (wallsInGrid.Contains(new Vector2(i,j)))
                    {
                        tankMoveSubGrid[i, j] = new RectangleF();
                    }
                    //check for a tank there
                    else if (tankLocations.Contains(new Point(i, j)))
                    {
                        tankMoveSubGrid[i, j] = new RectangleF();
                    }
                }
            }
        }

        #region turnTakingCode
        //information for start of turn state
        List<Entity> oldEntities = new List<Entity>();

        /// <summary> Set the old information to represent the start of the turn. </summary>
        private void SetStartofTurnState()
        {
            curPlayer.oldItems = curPlayer.Items;
            curPlayer.oldSweeps = curPlayer.sweeps;
            curPlayer.oldTanks = curPlayer.tanks;

            oldEntities = entities;
        }
        /// <summary> Get the old information and apply it to the tracked information. 
        /// This will act as an undo effect. Setting the turn back to the beginning </summary>
        private void GetStartofTurnState()
        {
            curPlayer.Items = curPlayer.oldItems;
            curPlayer.sweeps = curPlayer.oldSweeps;
            curPlayer.tanks = curPlayer.oldTanks;

            entities = oldEntities;
        }
        protected void MoveOrShoot()
        {
            //ensure the tank is active and has player has ap left
            if (curPlayer.tanks[activeTankNum].Active && curPlayer.AP > 0)
            {

            }
        }
        #endregion
    }
}
