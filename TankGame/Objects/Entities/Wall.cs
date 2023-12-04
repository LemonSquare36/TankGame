
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;



namespace TankGame.Objects.Entities
{
    internal class Wall : Entity
    {
        public List<Point> gridLocations = new();
        public List<RectangleF> gridSquares = new();
        public RectangleF[,] multiWallArray;
        public bool multiWall = false;
        public bool destroyable;

        public Vector2 RealHPStart = new Vector2();

        private Texture2D shadow, crack1, crack2, crack3;
        private List<Texture2D> shadows = new List<Texture2D>();
        private List<Texture2D> mulitCracks1 = new List<Texture2D>();
        private List<Texture2D> mulitCracks2 = new List<Texture2D>();
        private List<Texture2D> mulitCracks3 = new List<Texture2D>();
        Point shadowOffSet = new Point();
        List<Point> shadowOffSetList = new List<Point>();

        public Wall(RectangleF CurrentSquare, Point GridLocation, bool Destroyable) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            if (destroyable)
            {
                texFile = "GameSprites/BattleSprites/WallSprites/Wall";
            }
            else
            {
                texFile = "GameSprites/BattleSprites/WallSprites/IndWalls/Wall";
            }
            type = "wall";
            size = curSquare.Size / spriteSize;

            HP = 100;
            curHP = HP;
            SetHPBarPos();
            alive = true;
            destroyable = Destroyable;
        }
        public Wall(List<Point> GridLocations, Board curBoard, bool Destroyable) : base(new RectangleF(), new Point(-1, -1))
        {
            destroyable = Destroyable;
            if (destroyable)
            {
                texFile = "GameSprites/BattleSprites/WallSprites/Wall";
            }
            else
            {
                texFile = "GameSprites/BattleSprites/WallSprites/IndWalls/Wall";
            }
            type = "wall";
            size = curBoard.getGridSquare(0, 0).Size / spriteSize;


            HP = GridLocations.Count * 100 + (GridLocations.Count*25);
            curHP = HP;

            alive = true;

            multiWall = true;
            //find the bottom center of the multiwall while setting up whe grid locations
            RectangleF lowest = new RectangleF();
            RectangleF mostleft = new RectangleF();
            RectangleF mostright = new RectangleF();
            int highY = 0, lowY = 0, highX = 0;

            for (int i = 0; i < GridLocations.Count; i++)
            {
                gridLocations.Add(GridLocations[i]);
                RectangleF tempRect = curBoard.getGridSquare(GridLocations[i].X, GridLocations[i].Y);
                gridSquares.Add(tempRect);
                if (i == 0)
                {
                    highX = GridLocations[i].X;
                    lowY = GridLocations[i].Y;
                    highY = GridLocations[i].Y;
                    lowest = tempRect;
                    mostright = tempRect;
                    mostleft = tempRect;
                }
                else
                {
                    if (GridLocations[i].Y > highY)
                    {
                        highY = GridLocations[i].Y;
                        mostright = tempRect;
                    }
                    if (GridLocations[i].X > highX)
                    {
                        highX = GridLocations[i].X;
                        lowest = tempRect;
                    }
                    else if (GridLocations[i].Y < lowY)
                    {
                        lowY = GridLocations[i].X;
                        mostleft = tempRect;
                    }
                }
            }
            Vector2 bottomCenter = new Vector2(mostleft.Location.X + (mostleft.Location.X - mostright.Location.X), lowest.Y);
            hpBarLocStart = new Vector2(bottomCenter.X - (30 * size.X), bottomCenter.Y + (60 * size.Y));
            RealHPStart = hpBarLocStart;
            hpBarLoc = new Vector2(hpBarLocStart.X + (20 * size.X), hpBarLocStart.Y);

            findTexturePlacement(curBoard);
        }
        public Wall() : base(new RectangleF(), new Point(-1, -1))
        {

        }
        public override void Initialize(RectangleF newRectangle)
        {
            curSquare = newRectangle;
            size = curSquare.Size / spriteSize;
        }
        public void Initialize(Board curBoard)
        {
            gridSquares.Clear();
            size = curBoard.getGridSquare(0, 0).Size / spriteSize;
            for (int i = 0; i < gridLocations.Count; i++)
            {
                if (gridLocations[i].X < curBoard.Rows && gridLocations[i].Y < curBoard.Rows)
                    gridSquares.Add(curBoard.getGridSquare(gridLocations[i].X, gridLocations[i].Y));
            }
        }
        public override void LoadContent()
        {
            if (!multiWall)
            {
                base.LoadContent();
                if (destroyable)
                {
                    shadow = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowFull");
                    //crack texture for destroyable walls
                    crack1 = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/WallCrack1");
                    crack2 = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/WallCrack2");
                    crack3 = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/WallCrack3");
                }
                else
                {
                    shadow = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowFull");
                    //crack texture for non destoyable walls (rarely can be damaged once, then broken)
                    crack1 = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/WallCrack2");
                }
                
            }
            else
            {
                base.LoadContent();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!multiWall)
            {
                spriteBatch.Draw(tex, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                //code for drawing regular wall cracks
                if (destroyable)
                { 
                float percentHP = ((float)curHP / (float)HP) * 100;
                    switch (percentHP)
                    {
                        case > 75:
                            break;
                        case > 50:
                            spriteBatch.Draw(crack1, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                            break;
                        case > 25:
                            spriteBatch.Draw(crack2, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                            break;
                        case <= 25:
                            spriteBatch.Draw(crack3, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < gridLocations.Count; i++)
                {
                    spriteBatch.Draw(tex, gridSquares[i].Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                    if (destroyable)
                    { 
                        float percentHP = ((float)curHP / (float)HP) * 100;
                        if (mulitCracks1[i] != null)
                        {
                            switch (percentHP)
                            {
                                case > 75:
                                    break;
                                case > 50:
                                    spriteBatch.Draw(mulitCracks1[i], gridSquares[i].Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                                    break;
                                case > 25:
                                    spriteBatch.Draw(mulitCracks2[i], gridSquares[i].Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                                    break;
                                case <= 25:
                                    spriteBatch.Draw(mulitCracks3[i], gridSquares[i].Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                                    break;
                            }
                        }
                    }
                }
            }
        }
        public void DrawShadows(SpriteBatch spriteBatch)
        {
            if (!multiWall)
            {
                spriteBatch.Draw(shadow, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            else
            {
                for (int i = 0; i < shadows.Count; i++)
                {
                    if (shadows[i] != null)
                    {
                        spriteBatch.Draw(shadows[i], gridSquares[i].Location - (new Vector2(shadowOffSetList[i].X, shadowOffSetList[i].Y) * size), null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);//shadowLayerList[i]
                    }
                }
            }
        }
        public override void setInLOS(bool InLOS)
        {
            base.setInLOS(InLOS);
            if (InLOS)
            {
                drawColor = new Color(255, 205, 205);
            }
            else if (!InLOS)
            {
                if (!multiWall)
                    drawColor = Color.White;
                else
                    drawColor = Color.White;
            }
        }
        private void findTexturePlacement(Board curBoard)
        {
            Point offset;
            MultiWallArray(curBoard, out offset);

            //check over the array to find what parts of the multiwall are connected
            List<Point> toCheck = new();
            for (int i = 0; i < multiWallArray.GetLength(0); i++)
            {
                for (int j = 0; j < multiWallArray.GetLength(1); j++)
                {
                    //if its a wall spot
                    if (multiWallArray[i, j].identifier == 1)
                    {
                        //add it to toCheck to see if it has neighbors
                        toCheck.Add(new Point(i, j));
                    }
                    //wall was chosen to check further
                    while (toCheck.Count > 0)
                    {
                        if (shadows.Count == 5)
                        {

                        }
                        //see if this wall has neighbors other than ones already checked for neighbors (only neighbors in 4 cardinal directions/no diagonal)
                        for (int k = 0; k < toCheck.Count; k++)
                        {
                            bool left = false, right = false, top = false, bottom = false;

                            int X = toCheck[k].X, Y = toCheck[k].Y;
                            //mark its been looked at
                            multiWallArray[X, Y].identifier = 2;
                            #region neighborChecks
                            if (X > 0)
                            {
                                //<---
                                //check its neighbors for walls spots
                                if (multiWallArray[X - 1, Y].identifier == 1 || multiWallArray[X - 1, Y].identifier == 2)
                                {
                                    if (multiWallArray[X - 1, Y].identifier == 1)
                                    {
                                        //add it to the list of blocks to check
                                        toCheck.Add(new Point(X - 1, Y));
                                    }
                                    //if its a nieghbor then set both to 2 for connected to a block
                                    multiWallArray[X - 1, Y].identifier = 2;
                                    top = true;
                                }
                            }
                            if (Y > 0)
                            {
                                //^
                                //|
                                //|
                                if (multiWallArray[X, Y - 1].identifier == 1 || multiWallArray[X, Y - 1].identifier == 2)
                                {
                                    if (multiWallArray[X, Y - 1].identifier == 1)
                                    {
                                        //add it to the list of blocks to check
                                        toCheck.Add(new Point(X, Y - 1));
                                    }
                                    //if its a nieghbor then set both to 2 for connected to a block
                                    multiWallArray[X, Y - 1].identifier = 2;
                                    left = true;
                                }
                            }
                            if (X < multiWallArray.GetUpperBound(0))
                            {

                                //--->
                                if (multiWallArray[X + 1, Y].identifier == 1 || multiWallArray[X + 1, Y].identifier == 2)
                                {
                                    if (multiWallArray[X + 1, Y].identifier == 1)
                                    {
                                        //add it to the list of blocks to check
                                        toCheck.Add(new Point(X + 1, Y));
                                    }
                                    //if its a nieghbor then set both to 2 for connected to a block
                                    multiWallArray[X + 1, Y].identifier = 2;
                                    bottom = true;
                                }
                            }
                            if (Y < multiWallArray.GetUpperBound(1))
                            {
                                //|
                                //|
                                //V
                                if (multiWallArray[X, Y + 1].identifier == 1 || multiWallArray[X, Y + 1].identifier == 2)
                                {
                                    if (multiWallArray[X, Y + 1].identifier == 1)
                                    {
                                        //add it to the list of blocks to check
                                        toCheck.Add(new Point(X, Y + 1));
                                    }
                                    //if its a nieghbor then set both to 2 for connected to a block
                                    multiWallArray[X, Y + 1].identifier = 2;
                                    right = true;
                                }
                            }
                            #endregion
                            //remove the wall that just got neighbor checked
                            toCheck.Remove(toCheck[k]);

                            //if its surronded then dont give it a shadow
                            if (bottom && top && right && left)
                            {
                                shadowOffSetList.Add(shadowOffSet);
                                shadowOffSet = new Point();
                                shadows.Add(tex);
                                mulitCracks1.Add(tex);
                                mulitCracks2.Add(tex);
                                mulitCracks3.Add(tex);
                                continue;
                            }
                            //some shadows need a two pixel offset to cover corners
                            #region bool checks for sides
                            if (top && left && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Bottom/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Bottom/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Bottom/Crack3"));
                                    shadowOffSet.X = 2;
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/Bottom/Crack2"));
                                    shadowOffSet.X = 5;
                                }
                                
                                
                            }
                            else if (top && bottom && left)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Right/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Right/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Right/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/Right/Crack2"));
                                    shadowOffSet.Y = 5;
                                }
                               
                            }
                            else if (top && bottom && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Left/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Left/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Left/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/Left/Crack2"));
                                    shadowOffSet.Y = 5;
                                }
                                
                            }
                            else if (top && bottom)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowLeftRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/LeftRight/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/LeftRight/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/LeftRight/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowLeftRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/LeftRight/Crack2"));
                                    shadowOffSet.Y = 5;
                                }
                                
                            }
                            else if (top && left)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowBottomRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomRight/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomRight/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomRight/Crack3"));
                                    shadowOffSet.X = 2;
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowBottomRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/BottomRight/Crack2"));
                                    shadowOffSet.X = 5;
                                    shadowOffSet.Y = 5;
                                }
                                
                                
                            }
                            else if (top && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowBottomLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomLeft/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomLeft/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/BottomLeft/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowBottomLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/BottomLeft/Crack2"));
                                    shadowOffSet.Y = 5;
                                }
                               
                            }
                            else if (top)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowNoTop"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoTop/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoTop/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoTop/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowNoTop"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/NoTop/Crack2"));
                                    shadowOffSet.Y = 5;
                                }
                                                    
                            }
                            //at this point we know top is false
                            else if (bottom && left && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowTop"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Top/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Top/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/Top/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowTop"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/Top/Crack2"));
                                    shadowOffSet.X = 5;
                                }

                            }
                            else if (bottom && left)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowTopRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopRight/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopRight/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopRight/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowTopRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/TopRight/Crack2"));
                                }
                            }
                            else if (bottom && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowTopLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopLeft/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopLeft/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopLeft/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowTopLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/TopLeft/Crack2"));
                                }

                            }
                            else if (bottom)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowNoBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoBottom/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoBottom/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoBottom/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowNoBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/NoBottom/Crack2"));
                                }
                                
                            }
                            //at this point we know top and bottom are false
                            //so it must be left and/or right
                            else if (left && right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowTopBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopBottom/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopBottom/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/TopBottom/Crack3"));
                                    shadowOffSet.X = 2;
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowTopBottom"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/TopBottom/Crack2"));
                                    shadowOffSet.X = 5;
                                }
                                
                                
                            }
                            else if (right)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowNoRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoRight/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoRight/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoRight/Crack3"));
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowNoRight"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/NoRight/Crack2"));
                                }
                                
                            }
                            else if (left)
                            {
                                if (destroyable)
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/WallShadowNoLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoLeft/Crack1"));
                                    mulitCracks2.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoLeft/Crack2"));
                                    mulitCracks3.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/Cracks/NoLeft/Crack3"));
                                    shadowOffSet.X = 2;
                                }
                                else
                                {
                                    shadows.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/WallShadowNoLeft"));
                                    mulitCracks1.Add(Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/WallSprites/IndWalls/Cracks/NoLeft/Crack2"));
                                    shadowOffSet.X = 5;
                                }
                                
                                
                            }
                            shadowOffSetList.Add(shadowOffSet);
                            shadowOffSet = new Point();
                            #endregion
                        }
                    }
                }
            }
        }
        public RectangleF[,] MultiWallArray(Board curBoard, out Point offset)
        {
            int lowX = 0, highX = 0, lowY = 0, highY = 0;
            if (multiWall)
            {
                #region find the bounds of the array
                for (int i = 0; i < gridLocations.Count; i++)
                {
                    if (i == 0)
                    {
                        lowX = gridLocations[i].X;
                        highX = gridLocations[i].X;
                        lowY = gridLocations[i].Y;
                        highY = gridLocations[i].Y;
                    }
                    else
                    {
                        if (lowX > gridLocations[i].X)
                        {
                            lowX = gridLocations[i].X;
                        }
                        if (highX < gridLocations[i].X)
                        {
                            highX = gridLocations[i].X;
                        }
                        if (lowY > gridLocations[i].Y)
                        {
                            lowY = gridLocations[i].Y;
                        }
                        if (highY < gridLocations[i].Y)
                        {
                            highY = gridLocations[i].Y;
                        }
                    }
                }
                #endregion
                //get the subgrid using the bounds of the gridlocations. Marked with identifiers of 1 for where the walls are
                multiWallArray = curBoard.getSubGrid(lowX, highX, lowY, highY, gridLocations);
            }
            offset = new Point(lowX, lowY);
            return multiWallArray;
        }
        public static Wall Clone(Wall ItemToClone, Board curBoard)
        {
            Wall @new = new Wall();
            if (ItemToClone.multiWall)
            {
                @new = new Wall(ItemToClone.gridLocations, curBoard, ItemToClone.destroyable);
            }
            else
            {
                @new = new Wall(ItemToClone.curSquare, ItemToClone.gridLocation, ItemToClone.destroyable);
                @new.shadow = ItemToClone.shadow;
            }
            @new.Active = ItemToClone.Active;
            @new.alive = ItemToClone.alive;
            @new.curHP = ItemToClone.curHP;
            @new.HP = ItemToClone.HP;
            @new.hpBar = ItemToClone.hpBar;
            @new.hpBarSize = ItemToClone.hpBarSize;
            @new.tex = ItemToClone.tex;
            @new.crack1 = ItemToClone.crack1;
            @new.crack2 = ItemToClone.crack2;
            @new.crack3 = ItemToClone.crack3;
            @new.texFile = ItemToClone.texFile;
            @new.type = ItemToClone.type;

            return @new;
        }
    }
}
