using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class GameScreensManager : ScreenManager
    {
        //variables needed by the the local and networking battlescreens
        protected Player curPlayer, P1, P2;
        //Action points per turn
        int AP = 4;
        //turn tracker
        int turn = 1;

        //info storage for circles in the grid
        protected List<Point> wallLocations = new List<Point>();
        protected List<Vector2> wallsInCircle = new List<Vector2>();
        protected RectangleF[,] CircleTiles;
        protected bool drawCircle = false;

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
                        //get the circle around the selected tank
                        CircleTiles = curBoard.getRectanglesInRadius(new Vector2(tank.gridLocation.X, tank.gridLocation.Y), tank.range, wallLocations, out wallsInCircle);
                        findTilesInLoS();
                        drawCircle = true;
                    }
                }
            }
        }

        /// <summary>Find out which tiles are in the line of sight of the tank (not blocked by walls)</summary>
        protected void findTilesInLoS()
        {
            //use each wall to check what tiles they are blocking
            foreach(Vector2 wall in wallsInCircle)
            {
                //where the loops will start when iterating the 2darray
                int starti = 0;
                int startj = 0;
                //find out what quater of the circle the wall is in
                int width = CircleTiles.GetLength(0); //the columns number as width
                int height = CircleTiles.GetLength(1); //rows number as height

                int X = (int)wall.X;
                int Y = (int)wall.Y;

                //get the center point
                int CenterX = width / 2;
                int CenterY = height / 2;
                Vector2 Center = CircleTiles[CenterX, CenterY].Center;

                //if the wall is to the left of the center tile
                if (wall.X < CenterX)
                { width = X; } //shrink the iterations so nothing in front sideways is checked

                //if the wall is to the right of the center tile
                else if (wall.X > CenterX)
                { starti = X; } //start the iteration at that value, only checking what is behind the wall

                //if the wall is above the center tile
                if (wall.Y < CenterY)
                { height = Y; } //only check tiles equal or above the wall

                //if the wall is below the center tile
                else if (wall.Y > CenterY)
                { startj = Y; }//only check tiles equal or below the wall

                //if the tile falls on the center for the rows or columns, then leave it alone and check the whole row lenght or column length

                //iterate the 2darray quadrant that the wall is in and check to see if its blocking any blocks
                for (int i = starti; i < width; i++)
                {
                    for (int j = startj; j < height; j++)
                    {
                        //if the rectangle isnt null
                        if (!CircleTiles[X, Y].Null)
                        {
                            //check if it is visiable with the current wall as argument
                            Line templine = new Line(CircleTiles[X, Y].Center, Center);
                            if (templine.LineSegmentIntersectsRectangle(CircleTiles[i, j]))
                            {
                                //if it is blocked then empty/null the rectangle
                                CircleTiles[X, Y] = new RectangleF();
                            }
                        }
                    }
                }
            }
        }
    }
}
