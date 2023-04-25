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
        Texture2D Outline, Border;
        Rectangle[,] gridarray;
        Point TopLeft, BottomRight;
        Point realBoardSize;

        int Col, Row;
        public int Columns
        {
            get { return Col; }
        }
        public int Rows
        {
            get { return Row;  }
        }

        //gets the information needs to create a 2d array that makes up the board
        public Board(Point topLeft, Point bottomRight, int col, int rows)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Col = col;
            Row = rows;

            gridarray = getBoard();
        }

        //loads the texture for the gameboard
        public void LoadContent()
        {
            Outline = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            Border = Main.GameContent.Load<Texture2D>("GameSprites/WhiteRectangle");
        }

        //draws each rectangle through for loops to make the grid
        public void draw(SpriteBatch spriteBatch, Color color)
        {

            //draws an outline for the board creating a thicker border
            Rectangle horizontalOutline = new Rectangle(new Point(TopLeft.X - 1, TopLeft.Y - 1), new Point(realBoardSize.X, 4));
            Rectangle verticalOutline = new Rectangle(new Point(TopLeft.X - 1, TopLeft.Y - 1), new Point(4, realBoardSize.Y));

            spriteBatch.Draw(Outline, horizontalOutline, color);
            spriteBatch.Draw(Outline, verticalOutline, color);

            horizontalOutline.Y += realBoardSize.Y;
            horizontalOutline.Width += 4;
            verticalOutline.X += realBoardSize.X;

            spriteBatch.Draw(Outline, horizontalOutline, color);
            spriteBatch.Draw(Outline, verticalOutline, color);


            for (int i = 0; i <= gridarray.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= gridarray.GetUpperBound(1); j++)
                {
                    Rectangle horizontalBorder = new Rectangle(gridarray[i, j].Location, new Point(gridarray[i,j].Width,2));
                    Rectangle verticalBorder = new Rectangle(gridarray[i, j].Location, new Point(2, gridarray[i, j].Height));

                    spriteBatch.Draw(Outline, horizontalBorder, color);
                    spriteBatch.Draw(Outline, verticalBorder, color);

                    horizontalBorder.Y += gridarray[i, j].Height;
                    verticalBorder.X += gridarray[i, j].Width;

                    spriteBatch.Draw(Outline, horizontalBorder, color);
                    spriteBatch.Draw(Outline, verticalBorder, color);
                }
            }
        }

        //creates the rectangles that go into the array - populates it
        private Rectangle[,] getBoard()
        {
            Point size = new Point(BottomRight.X / Col, BottomRight.Y / Row);
            Point location = TopLeft;

            Rectangle[,] rectangles = new Rectangle[Row, Col];

            for (int i = 0; i <= rectangles.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= rectangles.GetUpperBound(1); j++)
                {
                    rectangles[i, j] = new Rectangle(location, size);

                    location.X += size.X;
                }          
                location.X = TopLeft.X;
                location.Y += size.Y;
            }
            var last = rectangles[Row-1, Col-1];
            realBoardSize = last.Location + new Point(last.Width+1, last.Height+1);

            return rectangles;
        }
        public Rectangle[,] getGrid()
        {
            return gridarray;
        }
        public Rectangle getGridSquare(int Col, int Row)
        {
            return gridarray[Row, Col];
        }
        public Point getRealBoardSize()
        {
            return realBoardSize;
        }
    }
}
