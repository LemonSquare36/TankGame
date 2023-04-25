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

namespace TankGame
{
    internal class Board
    {
        Texture2D Outline;
        Rectangle[,] gridarray;
        Point TopLeft, BottomRight;
        int Col, Rows;
        Point realBoardSize;

        //gets the information needs to create a 2d array that makes up the board
        public Board(Point topLeft, Point bottomRight, int col, int rows)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Col = col;
            Rows = rows;

            gridarray = getBoard();
        }

        //loads the texture for the gameboard
        public void LoadContent()
        {
            Outline = Main.GameContent.Load<Texture2D>("GameSprites/WhiteRectangle");
        }

        //draws each rectangle through for loops to make the grid
        public void draw(SpriteBatch spriteBatch, Color color)
        {
            //draws an outline for the board creating a thicker border
            spriteBatch.Draw(Outline, new Rectangle(new Point(TopLeft.X - 1, TopLeft.Y - 1), realBoardSize), color);

            for (int i = 0; i <= gridarray.GetUpperBound(1); i++)
            {
                for (int j = 0; j <= gridarray.GetUpperBound(0); j++)
                {
                    spriteBatch.Draw(Outline, gridarray[i, j], color);
                }
            }
        }

        //creates the rectangles that go into the array - populates it
        private Rectangle[,] getBoard()
        {
            Point size = new Point(BottomRight.X / Col, BottomRight.Y / Rows);
            Point location = TopLeft;

            Rectangle[,] rectangles = new Rectangle[Col, Rows];

            for (int i = 0; i <= rectangles.GetUpperBound(1); i++)
            {
                for (int j = 0; j <= rectangles.GetUpperBound(0); j++)
                {
                    rectangles[i, j] = new Rectangle(location, size);

                    location.X += size.X - 1;
                }          
                location.X = TopLeft.X;
                location.Y += size.Y - 1;
            }
            var last = rectangles[Col-1, Rows -1];
            realBoardSize = last.Location + new Point(last.Width, last.Height);
            return rectangles;
        }
        public Rectangle[,] getGrid()
        {
            return gridarray;
        }
        public Rectangle getGridSquare(int Col, int Row)
        {
            return gridarray[Col, Row];
        }
    }
}
