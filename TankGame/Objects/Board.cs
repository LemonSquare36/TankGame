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
        Rectangle[,] gridLines;

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

            gridLines = getGridLines(gridarray);

           
        }

        //draws each rectangle through for loops to make the grid
        public void draw(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i <= gridLines.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= gridLines.GetUpperBound(1); j++)
                {
                    spriteBatch.Draw(Outline, gridLines[i,j], color);
                    spriteBatch.Draw(Outline, gridLines[i, j], color);                  
                    spriteBatch.Draw(Outline, gridLines[i, j], color);
                    spriteBatch.Draw(Outline, gridLines[i, j], color);
                }
            }
        }
        public void DrawOutline(SpriteBatch spriteBatch, Color color)
        {

            //draws an outline for the board creating a thicker border
            Rectangle horizontalOutline = new Rectangle(new Point(TopLeft.X, TopLeft.Y), new Point(BottomRight.X+1, 4));
            Rectangle verticalOutline = new Rectangle(new Point(TopLeft.X, TopLeft.Y), new Point(4, BottomRight.Y));

            spriteBatch.Draw(Outline, horizontalOutline, color);
            spriteBatch.Draw(Outline, verticalOutline, color);

            horizontalOutline.Y += BottomRight.Y-4;
            horizontalOutline.Width += 3;
            verticalOutline.X += BottomRight.X;

            spriteBatch.Draw(Outline, horizontalOutline, color);
            spriteBatch.Draw(Outline, verticalOutline, color);

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
        //this gets the lines to draw to make the rectangles on the grid. 
        private static Rectangle[,] getGridLines(Rectangle[,] grid)
        {
            int k = 0;
            Rectangle[,] Lines = new Rectangle[grid.LongLength, 4];
            //itertae through grides to find lines for each rectangle
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    //get top and left walls
                    Rectangle horizontalBorder = new Rectangle(grid[i, j].Location, new Point(grid[i, j].Width, 1));
                    Rectangle verticalBorder = new Rectangle(grid[i, j].Location, new Point(1, grid[i, j].Height));
                    //apply those walls to thier proper posistion and grid rectangle
                    Lines[k, 0] = horizontalBorder;
                    Lines[k, 1] = verticalBorder;

                    //get bottom and right walls
                    horizontalBorder.Y += grid[i, j].Height;
                    verticalBorder.X += grid[i, j].Width;
                    //apply those walls to thier proper posistion and grid rectangle
                    Lines[k, 2] = horizontalBorder;
                    Lines[k, 3] = verticalBorder;

                    //K is the postion in lines that follows gridarray. Adds one each time J increases to indicate a new grid needs lines
                    k++;
                }
            }
            return Lines;
        }
    }
}
