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
        protected List<Point> wallLocations = new List<Point>();
        protected List<Vector2> wallsInCircle = new List<Vector2>();
        protected RectangleF[,] CircleTiles;

        protected bool drawCircle = false;
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
    }
}
