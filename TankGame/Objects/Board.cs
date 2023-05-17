using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TankGame.Objects
{
    internal class Board
    {
        Texture2D Outline;
        RectangleF[,] gridarray, gridLines;
        Vector2 TopLeft, BottomRight;

        RectangleF InnerRectangle, 
            horizontalOutline, horizontalOutline2,
            verticalOutline, verticalOutline2;
        Color color, color2, borderColor;
        float borderThickness;
        int Col, Row;
        public Vector2 Location, FullSize;
        private Vector2 individualSize;
        public Vector2 IndividualSize { get { return individualSize; } }

        public int Columns
        {
            get { return Col; }
        }
        public int Rows
        {
            get { return Row;  }
        }
        /// <summary>Checkerboard color 1</summary>
        public Color Color1
        {
            get { return color; }
        }
        /// <summary>checkboard color 2</summary>
        public Color Color2
        {
            get { return color2; }
        }
        /// <summary>Border Color</summary> 
        public Color Color3
        {
            get { return borderColor; }
        }
        /// <summary>Border Thickness</summary> 
        public float BorderThickness
        {
            get { return borderThickness; }
        }

        /// <summary>
        /// gets the information needs to create a 2d array that makes up the board
        /// </summary>
        /// <param name="topLeft">position to draw the board</param>
        /// <param name="bottomRight">size of the board</param>
        /// <param name="col">columns to draw</param>
        /// <param name="rows">rows to draw</param>
        /// <param name="thickness">border thickness</param>
        public Board(Point topLeft, Point bottomRight, int col, int rows, int thickness)
        {
            Location = new Vector2(topLeft.X, topLeft.Y);
            TopLeft = new Vector2(topLeft.X, topLeft.Y);
            FullSize = new Vector2(bottomRight.X, bottomRight.Y);
            BottomRight = new Vector2(bottomRight.X-thickness, bottomRight.Y - thickness);
            Col = col;
            Row = rows;         

            borderThickness = thickness;
            individualSize = new Vector2((BottomRight.X - borderThickness + 1) / Col, (BottomRight.Y - borderThickness + 1) / Row);
            getBoard();
        }

        //loads the texture for the gameboard
        public void LoadContent()
        {
            Outline = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");

            gridLines = getGridLines(gridarray);
            setBorderDimensions();
           
        }
        /// <summary>
        /// sets the colors for the checkers and border
        /// </summary>
        /// <param name="C1">first checker color</param>
        /// <param name="C2">second checker color</param>
        /// <param name="C3">border color</param>
        public void setColor(Color C1, Color C2, Color C3)
        {
            color = C1;
            color2 = C2;
            borderColor = C3;
        }

        //draws each rectangle through for loops to make the grid
        public void drawGrid(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i <= gridLines.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= gridLines.GetUpperBound(1); j++)
                {
                    spriteBatch.Draw(Outline, gridLines[i, j].Location,null, color,0,Vector2.Zero, gridLines[i, j].Size, SpriteEffects.None, 0);
                }
            }
        }
        //draws all the rectangles in a checkboard pattern
        public void drawCheckers(SpriteBatch spriteBatch)
        {

            for (int i = 0; i <= gridarray.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= gridarray.GetUpperBound(1); j++)
                {
                    if (i % 2 == 0 || i == 0)
                    {
                        if (j % 2 == 0 || j == 0)
                        {
                            spriteBatch.Draw(Outline, gridarray[i, j].Location, null, color, 0, Vector2.Zero, gridarray[i, j].Size, SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(Outline, gridarray[i, j].Location, null, color2, 0, Vector2.Zero, gridarray[i, j].Size, SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        if (j % 2 == 0 || j == 0)
                        {
                            spriteBatch.Draw(Outline, gridarray[i, j].Location, null, color2, 0, Vector2.Zero, gridarray[i, j].Size, SpriteEffects.None, 0);
                            
                        }
                        else
                        {
                            spriteBatch.Draw(Outline, gridarray[i, j].Location, null, color, 0, Vector2.Zero, gridarray[i, j].Size, SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
        //draws the border
        public void DrawOutline(SpriteBatch spriteBatch)
        {
            //draws an outline for the board creating a thicker border
            spriteBatch.Draw(Outline, horizontalOutline.Location, null, borderColor, 0, Vector2.Zero, horizontalOutline.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(Outline, verticalOutline.Location, null, borderColor, 0, Vector2.Zero, verticalOutline.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(Outline, horizontalOutline2.Location, null, borderColor, 0, Vector2.Zero, horizontalOutline2.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(Outline, verticalOutline2.Location, null, borderColor, 0, Vector2.Zero, verticalOutline2.Size, SpriteEffects.None, 0);

        }

        //creates the rectangles that go into the array - populates it
        public void getBoard()
        {
            Vector2 location = TopLeft + new Vector2(borderThickness, borderThickness);
            InnerRectangle = new RectangleF(Location + new Vector2(borderThickness, borderThickness), new Vector2((BottomRight.X - (borderThickness + 1)), (BottomRight.Y - (borderThickness + 1))));

            gridarray = new RectangleF[Row, Col];

            for (int i = 0; i <= gridarray.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= gridarray.GetUpperBound(1); j++)
                {
                    gridarray[i, j] = new RectangleF(location, individualSize);

                    location.X += individualSize.X;
                }          
                location.X = TopLeft.X + (borderThickness);
                location.Y += individualSize.Y;
            }
            //var last = gridarray[Row-1, Col-1];
        }
        /// <summary>
        /// returns the array that the board uses for its smaller rectangles
        /// </summary>
        public RectangleF[,] getGrid()
        {
            return gridarray;
        }
        /// <summary>Gets the specific rectangle from grid array with Col and Row</summary>
        public RectangleF getGridSquare(int Row, int Col)
        {
            return gridarray[Col,Row];
        }
        /// <summary>Gets the specific rectangle from grid array with Vector2 Position</summary>
        public RectangleF getGridSquare(Vector2 Position)
        {
            Vector2 gridPos = Position - getInnerRectangle().Location / IndividualSize;
            return gridarray[Convert.ToInt16(Math.Floor(Convert.ToDouble(gridPos.X))), Convert.ToInt16(Math.Floor(Convert.ToDouble(gridPos.Y)))];
        }
        /// <summary>Gets the specific rectangle from grid array with Vector2 Position, and returns the grid loction</summary>
        public RectangleF getGridSquare(Vector2 Position, out Point gridLocation)
        {
            Vector2 gridPos = (Position - getInnerRectangle().Location) / IndividualSize;
            gridLocation.X = Convert.ToInt16(Math.Floor(Convert.ToDouble(gridPos.X)));
            gridLocation.Y = Convert.ToInt16(Math.Floor(Convert.ToDouble(gridPos.Y)));
            return gridarray[gridLocation.Y, gridLocation.X];
        }
        public Vector2 getOutlineSize()
        {
            return BottomRight;
        }
        public RectangleF getInnerRectangle()
        {
            return InnerRectangle;
        }
        public RectangleF[,] getRectanglesInRadius(Vector2 origin, int radius)
        {
            //get the sub grid for looping
            RectangleF[,] subGrid = getSubGrid(new Vector2(origin.X-radius, origin.Y-radius), new Vector2(radius*2, radius*2));
            for (int i = 0; i < subGrid.GetLength(0); i++)
            {
                //columns
                for (int j = 0; j < subGrid.GetLength(1); i++)
                {
                  //make sure the rectangle isnt a defualt rectangle
                    if (!subGrid[i, j].Null)
                    {
                        //find distance between the center of the circle and center of current rectangle center
                        //returns true if the rectangle center is out of the radius
                        if (radius <= Math.Sqrt(Math.Pow(subGrid[i, j].Center.X - origin.X, 2) + Math.Pow(subGrid[i, j].Center.Y - origin.Y, 2)))
                        {
                            //set all rectangles not in radius to null
                            subGrid[i, j] = new RectangleF();
                        }
                    }
                }
            }
            //return the subGrid with all rectangles in the radius being normal and all others null
            return subGrid;
        }
        private RectangleF[,] getSubGrid(Vector2 location, Vector2 size)
        {
            RectangleF[,] subGrid = new RectangleF[(int)size.X, (int)size.Y];
            //rows
            for (int i = 0; i < size.X; i++)
            {
                //columns
                for (int j = 0; j < size.Y; i++)
                {
                    try
                    {
                        subGrid[i, j] = gridarray[(int)location.X + i, (int)location.Y + j];
                    }
                    catch { subGrid[i, j] = new RectangleF(); }
                }
            }
            return subGrid;
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
        private void setBorderDimensions()
        {
            horizontalOutline = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(BottomRight.X, borderThickness));
            verticalOutline = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(borderThickness, BottomRight.Y));

            horizontalOutline2 = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(BottomRight.X, borderThickness));
            verticalOutline2 = new RectangleF(new Vector2(TopLeft.X, TopLeft.Y), new Vector2(borderThickness, BottomRight.Y));

            horizontalOutline2.Y += BottomRight.Y;
            horizontalOutline2.Width += borderThickness;
            verticalOutline2.X += BottomRight.X;
        }
    }
}
