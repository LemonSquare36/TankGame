using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame.Objects
{
    internal class Cell
    {
        public int X, Y, Cost, Distance;
        public Point location 
        { 
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public int CostDistance 
        { 
            get { return Distance + Cost; }
        }

        //Cell we came from
        public Cell Parent;
        //use this to know what excacly is in the cell
        public int Identifier;

        /// <summary>
        /// this gets the distance from point to point with no obstacles
        /// </summary>
        public int RawDistance(int targetX, int targetY)
        {
            //without walls, diagonals will cover the lesser value completly. Only need the larger value to get raw distance
            Distance = (Math.Abs(targetX - X) > Math.Abs(targetY - Y)) ? Math.Abs(targetX - X) : Math.Abs(targetY - Y);
            return Distance;
        }
        public Cell(int x, int y, int cost)
        {
            X = x;
            Y = y; 
            Cost = cost; //all my cells have the same cost
        }
    }
}
