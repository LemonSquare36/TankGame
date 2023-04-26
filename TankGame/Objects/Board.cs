﻿using System;
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
        RectangleF[,] gridarray;
        Vector2 TopLeft, BottomRight;
        Vector2 realBoardSize;
        RectangleF[,] gridLines;

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
        public Board(Vector2 topLeft, Vector2 bottomRight, int col, int rows)
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
                    spriteBatch.Draw(Outline, gridLines[i, j].Location,null, color,0,Vector2.Zero, gridLines[i, j].Size, SpriteEffects.None, 0);
                }
            }
        }
        public void DrawOutline(SpriteBatch spriteBatch, Color color)
        {

            //draws an outline for the board creating a thicker border
            RectangleF horizontalOutline = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(BottomRight.X+1, 4));
            RectangleF verticalOutline = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(4, BottomRight.Y));

            spriteBatch.Draw(Outline, horizontalOutline.Location, null, color, 0, Vector2.Zero, horizontalOutline.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(Outline, verticalOutline.Location, null, color, 0, Vector2.Zero, verticalOutline.Size, SpriteEffects.None, 0);

            horizontalOutline.Y += BottomRight.Y-4;
            horizontalOutline.Width += 3;
            verticalOutline.X += BottomRight.X;

            spriteBatch.Draw(Outline, horizontalOutline.Location, null, color, 0, Vector2.Zero, horizontalOutline.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(Outline, verticalOutline.Location, null, color, 0, Vector2.Zero, verticalOutline.Size, SpriteEffects.None, 0);

        }

        //creates the rectangles that go into the array - populates it
        private RectangleF[,] getBoard()
        {
            Vector2 size = new Vector2((BottomRight.X) / Col, (BottomRight.Y-9) / Row);
            Vector2 location = TopLeft;

            RectangleF[,] rectangles = new RectangleF[Row, Col];

            for (int i = 0; i <= rectangles.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= rectangles.GetUpperBound(1); j++)
                {
                    rectangles[i, j] = new RectangleF(location, size);

                    location.X += size.X;
                }          
                location.X = TopLeft.X;
                location.Y += size.Y;
            }
            var last = rectangles[Row-1, Col-1];
            realBoardSize = last.Location + new Vector2(last.Width+1, last.Height+1);

            return rectangles;
        }
        public RectangleF[,] getGrid()
        {
            return gridarray;
        }
        public RectangleF getGridSquare(int Col, int Row)
        {
            return gridarray[Row, Col];
        }
        public Vector2 getRealBoardSize()
        {
            return realBoardSize;
        }
        public Vector2 getOutlineSize()
        {
            var inner = BottomRight;
            inner.X -= 4;
            return inner;
        }
        //this gets the lines to draw to make the rectangles on the grid. 
        private static RectangleF[,] getGridLines(RectangleF[,] grid)
        {
            int k = 0;
            RectangleF[,] Lines = new RectangleF[grid.LongLength, 4];
            //itertae through grides to find lines for each rectangle
            for (int i = 0; i <= grid.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= grid.GetUpperBound(1); j++)
                {
                    //get top and left walls
                    RectangleF horizontalBorder = new RectangleF(grid[i, j].Location, new Vector2(grid[i, j].Width, 1));
                    RectangleF verticalBorder = new RectangleF(grid[i, j].Location, new Vector2(1, grid[i, j].Height));
                    //apply those walls to thier proper posistion and grid rectangle
                    Lines[k, 0] = horizontalBorder;
                    Lines[k, 1] = verticalBorder;

                    //get bottom and right walls
                    RectangleF horizontalBorder2 = new RectangleF(grid[i, j].Location.X, grid[i, j].Location.Y + grid[i, j].Height, grid[i, j].Width, 1);
                    RectangleF verticalBorder2 = new RectangleF(grid[i, j].Location.X+ grid[i, j].Width, grid[i, j].Location.Y, 1, grid[i, j].Height);

                    //apply those walls to thier proper posistion and grid rectangle
                    Lines[k, 2] = horizontalBorder2;
                    Lines[k, 3] = verticalBorder2;

                    //K is the postion in lines that follows gridarray. Adds one each time J increases to indicate a new grid needs lines
                    k++;
                }
            }
            return Lines;
        }
    }
}
