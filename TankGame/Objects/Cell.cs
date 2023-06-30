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
        public int X, Y;
        public double h, g, Cost;
        public Point location 
        { 
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public double f 
        { 
            get { return h + g; }
        }

        //Cell we came from
        public Cell Parent;
        //use this to know what excacly is in the cell
        public int Identifier;

        /// <summary>
        /// this gets the distance from point to point with no obstacles
        /// </summary>
        public double getHueristic(double targetX, double targetY)
        {
            //without walls, diagonals will cover the lesser value completly. Only need the larger value to get raw distance
            h = Math.Sqrt(Math.Pow((targetX-this.X), 2) + Math.Pow((targetY-this.Y), 2));
            return h;
        }
        public Cell(int x, int y, double cost)
        {
            X = x;
            Y = y; 
            this.Cost = cost; //all my cells have the same cost initially
            g = 0;
        }
    }
}
