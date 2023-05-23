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
            foreach(Point wall in wallLocations)
            {
                //where the loops will start when iterating the 2darray
                int starti = 0;
                int startj = 0;
                //find out what quater of the circle the wall is in
                int width = CircleTiles.GetUpperBound(0); //the columns number as width
                int height = CircleTiles.GetUpperBound(1); //rows number as height

                //get the center point 
                Vector2 Center = CircleTiles[((width+1)/2) - 1, ((height + 1) / 2) - 1].Center;

                //find out if the X cordinate is in the left or right of the circle (add 1 since circle is odd number from center tile)
                if (wall.X < (width + 1) / 2)
                { width = (width + 1) / 2; }
                else if (wall.X > (width + 1) / 2)
                { starti = ((width + 1) / 2) - 1; } //-1 to include the middle column
                //if it falls in the middle column - check both left and right (start 0 and full width)

                //find out if the Y cordniate is in the top or bottom of the circle
                if (wall.Y < (height + 1) / 2)
                { height = (height + 1) / 2; }
                else if(wall.Y > (height + 1) / 2)
                { startj = ((height + 1) / 2) - 1; } //-1 to include the middle row
                //if it falls in the middle row - check both top and bottom (start 0 and full height)

                //iterate the 2darray quadrant that the wall is in and check to see if its blocking any blocks
                for (int i = starti; i < width; i++)
                {
                    for (int j = startj; j < height; j++)
                    {
                        //Line templine = new Line(CircleTiles[i,j].Center, )
                    }
                }
            }
        }
    }
}
