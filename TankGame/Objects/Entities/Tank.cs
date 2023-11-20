﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TankGame.Tools;
using TankGame.GameInfo;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace TankGame.Objects.Entities
{
    internal class Tank : Entity
    {

        private string texFileDead;
        private Texture2D texDead;

        private string tankType;
        public int range;
        public int damage, wallDamage;
        public int buildCost;
        public float movementCost, fireCost;

        private static SoundEffectInstance fire, death;

        public Tank(RectangleF CurrentSquare, Point GridLocation, string Type) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            type = "tank";
            size = curSquare.Size / spriteSize;
            SetHPBarPos();
            alive = true;
            tankType = Type;
            getTankStats(Type);
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

        public override void alterHP(int HPChange)
        {
            base.alterHP(HPChange);
            if (!alive)
            {
                KillTank();
            }
        }

        private void getTankStats(string Type)
        {
            //change these on a per tank basis later
            texFile = "GameSprites/BattleSprites/TankUH";
            texFileDead = "GameSprites/BattleSprites/TankDead";

            if (Type == "Regular")
            {
                HP = 4;
                range = 8;
                damage = 3;
                wallDamage = 3;
                curHP = HP;
                movementCost = 0.5F;
                fireCost = 1;
                buildCost = 5;
            }
            else if (Type == "Sniper")
            {
                HP = 4;
                range = 8;
                damage = 1;
                wallDamage = 3;
                curHP = HP;
                movementCost = 1;
                fireCost = 2;
                buildCost = 7;
            }
            else if (Type == "Scout")
            {
                HP = 2;
                range = 4;
                damage = 1;
                wallDamage = 3;
                curHP = HP;
                movementCost = 0.5F;
                fireCost = 2;
                buildCost = 3;
            }
        }

        public void TankMove(Board curBoard, ref BoardState boardState, ref BoardState previousBoardState, List<Cell> path, out bool drawTankInfo)
        {
            drawTankInfo = true;
            //get the AP and then move that distance, starting from the back of the list (the list is in reverse move order)
            if (path.Count > 1)
            {
                //keep track of the tanks path through the list of tiles
                int checkedTiles = 0;
                //keep checking as long as the player has ap to spend and the path has more tiles to check
                while (checkedTiles * movementCost <= (boardState.playerList[boardState.curPlayerNum].AP) && checkedTiles < path.Count)
                {
                    //check each of the item boxes for collision 
                    for (int i = 0; i < boardState.itemBoxes.Count; i++)//(checked before mines since mines killing a tank exits the loop)
                    {
                        if (boardState.itemBoxes[i].gridLocation == path[path.Count - 1 - checkedTiles].location)
                        {

                        }
                    }

                    for (int i = 0; i < boardState.playerList.Count; i++)
                    {
                        if (i != boardState.curPlayerNum)//means its an enemy
                        {
                            for (int j = 0; j < boardState.playerList[i].mines.Count; j++) //check each of the enemies mines for collision
                            {
                                //cross reference the enemy mines locations with the current tile being checked's location
                                if (boardState.playerList[i].mines[j].gridLocation == path[path.Count - 1 - checkedTiles].location)
                                {
                                    //if there was a mine in one of the tiles traveled through
                                    alterHP(-8);//kill the tank
                                                //move the tank where the mine was/current tile checked
                                    gridLocation = path[path.Count - 1 - checkedTiles].location;
                                    curSquare = curBoard.getGrid()[path[path.Count - 1 - checkedTiles].location.X, path[path.Count - 1 - checkedTiles].location.Y];

                                    //spend the AP required to move there
                                    boardState.playerList[boardState.curPlayerNum].AP -= checkedTiles * movementCost;
                                    //remove the mine from the game
                                    boardState.playerList[i].mines.Remove(boardState.playerList[i].mines[j]);
                                    drawTankInfo = false;
                                    //make sure you CANNOT UNDO a death by mine by updating the previous turn state
                                    previousBoardState = BoardState.SavePreviousBoardState(boardState);
                                    break;
                                }
                            }
                        }
                    }
                    checkedTiles++;
                }
                if (alive)
                {
                    checkedTiles--; //do this since if the tank lived, it added 1 extra at the end of the loop
                    boardState.playerList[boardState.curPlayerNum].AP -= checkedTiles * movementCost;
                    gridLocation = path[path.Count - 1 - checkedTiles].location;
                    curSquare = curBoard.getGrid()[path[path.Count - 1 - checkedTiles].location.X, path[path.Count - 1 - checkedTiles].location.Y];
                }
            }
        }
        public void TankShoot(BoardState boardState, RectangleF[,] CircleTiles, List<Vector2> blockersInCircle, Vector2 worldPosition, out int possibleWallRemove)
        {
            possibleWallRemove = -1;
            bool fired = false; //track if something was fired at to prevent unnessacary checks
            foreach (RectangleF tile in CircleTiles)//if its in range
            {
                if (!tile.Null)//and not blocked
                {
                    foreach (Vector2 @object in blockersInCircle)//check which object is targeted
                    {
                        RectangleF targetRectangle;
                        if (CircleTiles[(int)@object.X, (int)@object.Y].Contains(worldPosition))//get the rectangle the mouse is in
                        {
                            targetRectangle = CircleTiles[(int)@object.X, (int)@object.Y];//set the target rectangle 

                            if (tile.identifier == 1 && !fired)//is targetable
                            {
                                if (boardState.playerList[boardState.curPlayerNum].AP >= fireCost)//if player has ap to fire
                                {
                                    //if (item logic)
                                    //damge being delt 
                                    //check to see if it is a tank being fired at 
                                    for (int i = 0; i < boardState.playerList.Count; i++)
                                    {
                                        if (i != boardState.curPlayerNum)
                                        {
                                            foreach (Tank eTank in boardState.playerList[i].tanks)//for each enemy tank
                                            {
                                                if (eTank.alive)
                                                {
                                                    if (eTank.curSquare.Location == targetRectangle.Location)//and the tank is in the target rectangle
                                                    {
                                                        eTank.alterHP(-damage);//tank takes damage
                                                        fired = true;
                                                        playFireSoundEffect();
                                                        boardState.playerList[boardState.curPlayerNum].AP -= fireCost;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if it wasnt a tank, then check which wall it must be then
                                    if (!fired)
                                    {
                                        for (int i = 0; i < boardState.walls.Count; i++)
                                        {
                                            if (boardState.walls[i].Type == "wall")
                                            {
                                                if (boardState.walls[i].curSquare.Location == targetRectangle.Location)
                                                {
                                                    boardState.playerList[boardState.curPlayerNum].AP -= fireCost;
                                                    boardState.walls[i].alterHP(-wallDamage);
                                                    boardState.walls[i].showHealth = true;
                                                    playFireSoundEffect();
                                                    fired = true;
                                                    if (!boardState.walls[i].alive)
                                                    {
                                                        possibleWallRemove = i;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #region static functions of tanks
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
                                    if (blockersInCircle.Contains(new Vector2(i, j)))
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

        public static void setTankNoises(string fireFileLocation, string deathFileLocation)
        {
            fire = SoundManager.CreateSound(fireFileLocation);
            death = SoundManager.CreateSound(deathFileLocation);
        }
        /// <summary>handles playing the tank firing noise </summary>
        public static void playFireSoundEffect()
        {
            fire.Stop();
            fire.Play();
        }
        private static void playDeathSoundEffect()
        {
            death.Stop();
            death.Play();
        }
        private static void KillTank()
        {
            playDeathSoundEffect();
        }
        public static Tank Clone(Tank ItemToClone)
        {
            Tank @new = new Tank(ItemToClone.curSquare, ItemToClone.gridLocation, ItemToClone.tankType);
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
        #endregion
    }
}
