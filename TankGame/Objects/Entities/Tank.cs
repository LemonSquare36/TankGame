using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TankGame.Tools;
using TankGame.GameInfo;


namespace TankGame.Objects.Entities
{
    internal class Tank : Entity
    {             
        public int range;
        public int damage;
        private string texFileDead;
        private Texture2D texDead;

        public Tank(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/TankUH";
            texFileDead = "GameSprites/BattleSprites/TankDead";
            type = "tank";
            size = curSquare.Size / spriteSize;
            range = 4;
            damage = 2; 
            HP = 4;
            curHP = 4;
            SetHPBarPos();
            alive = true;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            texDead = Main.GameContent.Load<Texture2D>(texFileDead);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            if (!alive)
            {
                spriteBatch.Draw(texDead, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            drawHPBar(spriteBatch);            
        }
        public int getHP()
        {
            return HP;
        }
        public static Tank Clone(Tank ItemToClone)
        {
            Tank @new = new Tank(ItemToClone.curSquare, ItemToClone.gridLocation);
            @new.Active = ItemToClone.Active;
            @new.alive = ItemToClone.alive;
            @new.curHP = ItemToClone.curHP;
            @new.HP = ItemToClone.HP;
            @new.hpBar = ItemToClone.hpBar;
            @new.hpBarLoc = ItemToClone.hpBarLoc;
            @new.hpBarLocStart = ItemToClone.hpBarLocStart;
            @new.hpBarSize = ItemToClone.hpBarSize;
            @new.showHealth = ItemToClone.showHealth;
            @new.tex = ItemToClone.tex;
            @new.texFile = ItemToClone.texFile;
            @new.texDead = ItemToClone.texDead;
            @new.type = ItemToClone.type;

            return @new;
        }


        /// <summary>Find out which tiles are in the line of sight of the tank (not blocked by walls)</summary>
        public static RectangleF[,] findTilesInLOS(Tank selectedTank, RectangleF[,] CircleTiles, List<Vector2> blockersInCircle, BoardState boardState)
        {
            int defualtRows = CircleTiles.GetLength(0);
            int defualtCols = CircleTiles.GetLength(1);
            //get the center point
            int CenterX = defualtRows / 2;
            int CenterY = defualtCols / 2;
            Vector2 Center = CircleTiles[CenterX, CenterY].Center;

            //use each wall to check what tiles they are blocking
            foreach (Vector2 @object in blockersInCircle)
            {
                //where the loops will start when iterating the 2darray
                int starti = 0;
                int startj = 0;
                int rows = defualtRows;
                int cols = defualtCols;

                int X = (int)@object.X;
                int Y = (int)@object.Y;

                //if the wall is to the left of the center tile
                if (@object.X < CenterX)
                { rows = X + 1; } //shrink the iterations so nothing in front sideways is checked

                //if the wall is to the right of the center tile
                else if (@object.X > CenterX)
                { starti = X; } //start the iteration at that value, only checking what is behind the wall

                //if the wall is above the center tile
                if (@object.Y < CenterY)
                { cols = Y + 1; } //only check tiles equal or above the wall

                //if the wall is below the center tile
                else if (@object.Y > CenterY)
                { startj = Y; }//only check tiles equal or below the wall

                //if the tile falls on the center for the rows or columns, then leave it alone and check the whole row lenght or column length

                //iterate the 2darray quadrant that the wall is in and check to see if its blocking any blocks
                for (int i = starti; i < rows; i++)
                {
                    for (int j = startj; j < cols; j++)
                    {
                        if (i == 3 && j == 6)
                        {

                        }
                        //if the rectangle isnt null
                        if (!CircleTiles[i, j].Null)
                        {
                            //make sure the wall and current tile checking arent the same
                            if (!(i == X && j == Y))
                            {
                                //check if it is visiable with the current wall as argument
                                Line templine = new Line(CircleTiles[i, j].Center, Center);
                                if (templine.LineSegmentIntersectsRectangle(CircleTiles[X, Y]))
                                {
                                    //if its a blocker that is getting blocked, we still need it so dont null yet
                                    if(blockersInCircle.Contains(new Vector2(i,j)))
                                    {
                                        CircleTiles[i, j].identifier = 2;
                                    }
                                    else //other wise null
                                    {
                                        //if it is blocked then empty/null the rectangle
                                        CircleTiles[i, j] = new RectangleF();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //check which blockers are marked to be nulled
            foreach (Vector2 blocker in blockersInCircle)
            {
                //if the rectangle was marked to be nulled
                if (CircleTiles[(int)blocker.X, (int)blocker.Y].identifier == 2)
                {
                    //KILL IT
                    CircleTiles[(int)blocker.X, (int)blocker.Y] = new RectangleF();
                }
            }
            foreach (Vector2 @object in blockersInCircle)
            {
                //check the walls/tanks to see if thier rectangle isnt null. 
                if (!CircleTiles[(int)@object.X, (int)@object.Y].Null)
                {
                    //If it isnt null then it is in sight and give the identifier of 1 to make it draw different
                    CircleTiles[(int)@object.X, (int)@object.Y].identifier = 1;
                    //get the subgrids location relative to the board to see if a tank is there
                    Point subgridLocation = new Point(selectedTank.gridLocation.X - selectedTank.range, selectedTank.gridLocation.Y - selectedTank.range);
                    //check if the object is a friendly tank and make the rectangle null/untargetable (remove it from eligble targets)
                    foreach (Tank tank in boardState.playerList[boardState.curPlayerNum].tanks)
                    {
                        Point tankLocation = tank.gridLocation - subgridLocation;
                        if (tankLocation == new Point((int)@object.X, (int)@object.Y))
                        {
                            CircleTiles[(int)@object.X, (int)@object.Y] = new RectangleF();
                        }
                    }
                    //make the selected tanks rectangle null
                    Point selectedTankLocation = selectedTank.gridLocation - subgridLocation;
                    CircleTiles[selectedTankLocation.X, selectedTankLocation.Y] = new RectangleF();
                }
            }
            return CircleTiles;
        }
    }
}
