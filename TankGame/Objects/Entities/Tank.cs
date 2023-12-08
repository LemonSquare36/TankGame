using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TankGame.Tools;
using TankGame.GameInfo;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using static System.Net.WebRequestMethods;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace TankGame.Objects.Entities
{
    internal class Tank : Entity
    {

        private string texFileDead, gunTexFile, muzzleFlashFile;
        private Texture2D texDead;
        private Texture2D gunTex, muzzleFlash;
        float flashTimer = 0;

        double gunAngle = 0;
        bool gunAimed = false, playSmokePuff = false;
        public bool targetAquired = false;

        Vector2 gunOrigin = new Vector2();
        //what the tank is shooting at
        bool wallFound = false;
        bool tankFound = false;
        bool mineFound = false;
        Entity objectAimedAt;

        private string tankType;
        public int range;
        public int damage, wallDamage;
        public int buildCost;
        public float movementCost, fireCost;

        private static SoundEffectInstance fire, death, select;
        private static Animation smokepuff;

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
            gunTex = Main.GameContent.Load<Texture2D>(gunTexFile);
            muzzleFlash = Main.GameContent.Load<Texture2D>(muzzleFlashFile);
            smokepuff = new Animation("GameSprites/SpriteSheets/BattleSprites/smoke", 3, 3, 7, 1, new Rectangle(0, 0, 75, 75), 300);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                spriteBatch.Draw(tex, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                if (flashTimer == 0)
                {
                    spriteBatch.Draw(gunTex, curSquare.Location - new Vector2(0,15) + gunOrigin, null, drawColor, (float)gunAngle, gunOrigin, size, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(muzzleFlash, curSquare.Location - new Vector2(0, 15) + gunOrigin, null, drawColor, (float)gunAngle, gunOrigin, size, SpriteEffects.None, 0);
                    flashTimer -= (float)Main.gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (flashTimer < 0)
                    {
                        flashTimer = 0;
                    }
                }
                if (playSmokePuff && objectAimedAt != null)
                {
                    smokepuff.PlayAnimation(spriteBatch, size.X);
                    if (smokepuff.stopped)
                    {
                        playSmokePuff = false;
                        smokepuff.ResetAnimation();
                    }
                }
            }
            if (!alive)
            {
                spriteBatch.Draw(texDead, curSquare.Location, null, drawColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            drawHPBar(spriteBatch);
        }
        public override void setInLOS(bool InLOS)
        {
            base.setInLOS(InLOS);
            if (InLOS)
            {
                drawColor = new Color(255, 180, 180);
            }
            else if (!InLOS)
            {
                drawColor = Color.White;
            }
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
            texFileDead = "GameSprites/BattleSprites/Tanks/TankDead";

            if (Type == "Regular")
            {
                texFile = "GameSprites/BattleSprites/Tanks/RegularTankBody";
                gunTexFile = "GameSprites/BattleSprites/Tanks/RegularTankGun";
                muzzleFlashFile = "GameSprites/BattleSprites/Tanks/RegularTankGunFlash";
                gunOrigin = new Vector2(25, 40);
                HP = 4;
                range = 4;
                damage = 2;
                wallDamage = 50;
                curHP = HP;
                movementCost = 1;
                fireCost = 2;
                buildCost = 5;
            }
            else if (Type == "Sniper")
            {
                texFile = "GameSprites/BattleSprites/Tanks/SniperTankBody";
                gunTexFile = "GameSprites/BattleSprites/Tanks/SniperTankGun";
                muzzleFlashFile = "GameSprites/BattleSprites/Tanks/SniperTankGunFlash";
                gunOrigin = new Vector2(25F, 46);
                HP = 4;
                range = 8;
                damage = 1;
                wallDamage = 25;
                curHP = HP;
                movementCost = 1;
                fireCost = 2;
                buildCost = 7;
            }
            else if (Type == "Scout")
            {
                texFile = "GameSprites/BattleSprites/Tanks/ScoutTankBody";
                gunTexFile = "GameSprites/BattleSprites/Tanks/ScoutTankGun";
                muzzleFlashFile = "GameSprites/BattleSprites/Tanks/ScoutTankGunFlash";
                gunOrigin = new Vector2(25, 36);
                HP = 2;
                range = 4;
                damage = 1;
                wallDamage = 75;
                curHP = HP;
                movementCost = 0.5F;
                fireCost = 2;
                buildCost = 3;
            }
        }

        public void TankMove(Board curBoard, ref BoardState boardState, ref BoardState previousBoardState, List<Cell> path, out bool tankDied, out bool itemGotten)
        {
            tankDied = false;
            itemGotten = false;
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
                            if (boardState.itemBoxes[i].alive)
                            {
                                itemGotten = true;
                                boardState.itemBoxes[i].alive = false;
                            }
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
                                    alterHP(-HP);//kill the tank
                                                 //move the tank where the mine was/current tile checked
                                    gridLocation = path[path.Count - 1 - checkedTiles].location;
                                    curSquare = curBoard.getGrid()[path[path.Count - 1 - checkedTiles].location.X, path[path.Count - 1 - checkedTiles].location.Y];

                                    //spend the AP required to move there
                                    boardState.playerList[boardState.curPlayerNum].AP -= checkedTiles * movementCost;
                                    //remove the mine from the game
                                    boardState.playerList[i].mines.Remove(boardState.playerList[i].mines[j]);
                                    tankDied = true;
                                    Active = false;
                                    //make sure you CANNOT UNDO a death by mine by updating the previous turn state
                                    previousBoardState = BoardState.SavePreviousBoardState(boardState, curBoard);
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
        public void TankShoot(Board curBoard, BoardState boardState, RectangleF[,] CircleTiles, Vector2 worldPosition, Pathfinder pathfinder, out int possibleWallRemove, ButtonState curRightClick, ButtonState oldRightClick)
        {
            Point curGridLocation;
            possibleWallRemove = -1;
            bool fired = false;            
            curBoard.getGridSquare(worldPosition, out curGridLocation);
            if (!targetAquired)
            {
                if (curRightClick == ButtonState.Pressed && oldRightClick != ButtonState.Pressed)
                {
                    //does the player have the action points to fire
                    if (boardState.playerList[boardState.curPlayerNum].AP >= fireCost)
                    {
                        //get the rectangle to see if its null 
                        RectangleF targetRectangle = curBoard.getGridSquare(curGridLocation.X, curGridLocation.Y);//set the target rectangle 
                        bool loopBreak = false;                                                                                       //if that location is a blocker (possible target)
                        foreach (Entity e in boardState.objectsInLOS)
                        {
                            if (e.Type == "tank")
                            {
                                if (e.alive)
                                {
                                    if (e.curSquare.Location == targetRectangle.Location)//and the tank is in the target rectangle
                                    {
                                        targetAquired = true;
                                        tankFound = true;
                                        objectAimedAt = e;
                                        break;
                                    }
                                }
                            }
                            else if (e.Type == "wall")
                            {
                                if (!((Wall)e).multiWall)
                                {
                                    if (e.curSquare.Location == targetRectangle.Location)
                                    {                                        
                                        if (((Wall)e).destroyable)
                                        {
                                            targetAquired = true;
                                            wallFound = true;
                                            objectAimedAt = ((Wall)e);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (((Wall)e).destroyable)
                                    {
                                        for (int j = 0; j < ((Wall)e).gridSquares.Count; j++)
                                        {
                                            if (((Wall)e).gridSquares[j].Location == targetRectangle.Location)
                                            {
                                                targetAquired = true;
                                                wallFound = true;
                                                objectAimedAt = ((Wall)e);
                                                loopBreak = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (loopBreak)
                            {
                                break;
                            }
                        }
                        if (!fired) //if it wasnt a fireable object but still non null rectangle then see if they were aiming for a mine
                        {
                            //check friendly mines for one that matches and remove it if it does
                            //alter curgridlocation to fit into circle tiles
                            //get the center tile of circle tiles (which is the tank that is shooting)
                            Point Center = new Point(CircleTiles.GetUpperBound(0) / 2, CircleTiles.GetUpperBound(1) / 2);
                            //find the difference btween curgridclicked and the tanks location
                            Point circleTilesGridLocation = curGridLocation;
                            circleTilesGridLocation -= gridLocation;
                            //then add center and it together to find the tile clicked relevent to circle tiles
                            circleTilesGridLocation += Center;
                            //if the cords are inside the array
                            if (circleTilesGridLocation.X >= 0 && circleTilesGridLocation.Y >= 0 && circleTilesGridLocation.X < CircleTiles.GetLength(0) && circleTilesGridLocation.Y < CircleTiles.GetLength(1))
                            {
                                //if the tile in the array isnt null (in LOS)
                                if (!CircleTiles[circleTilesGridLocation.X, circleTilesGridLocation.Y].Null)
                                {
                                    for (int i = 0; i < boardState.playerList[boardState.curPlayerNum].mines.Count; i++)
                                    {
                                        if (boardState.playerList[boardState.curPlayerNum].mines[i].gridLocation == curGridLocation)
                                        {
                                            targetAquired = true;
                                            mineFound = true;
                                            objectAimedAt = boardState.playerList[boardState.curPlayerNum].mines[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (!gunAimed && targetAquired)
            {
                if (objectAimedAt.Type == "wall")
                {
                    if (((Wall)objectAimedAt).multiWall)
                    {
                        RectangleF centerTile = ((Wall)objectAimedAt).multiWallArray[Convert.ToInt16(((Wall)objectAimedAt).multiWallArray.GetLength(0) / 2), Convert.ToInt16(((Wall)objectAimedAt).multiWallArray.GetLength(1) / 2)];
                        gunAimed = rotateGun(curSquare.Center, centerTile.Center);
                    }
                    else
                        gunAimed = rotateGun(curSquare.Center, objectAimedAt.curSquare.Center);
                }
                else
                    gunAimed = rotateGun(curSquare.Center, objectAimedAt.curSquare.Center);                
            }
            //the gun is done rotating towards its target
            else if (gunAimed && targetAquired)
            {
                if (tankFound)
                {
                    FireGun(boardState, ref fired);
                    objectAimedAt.alterHP(-damage);
                }
                else if (wallFound)
                {
                    FireGun(boardState, ref fired);
                    objectAimedAt.alterHP(-wallDamage);
                    if (!objectAimedAt.alive)
                    {
                        possibleWallRemove = boardState.walls.FindIndex(x => x == ((Wall)objectAimedAt));
                    }
                }
                else if (mineFound)
                {
                    FireGun(boardState, ref fired);
                    //update the pathfinder to say there is not a wall there
                    pathfinder.AlterCell(objectAimedAt.gridLocation, 0);
                    //remove the mine from the cur players mine list
                    boardState.playerList[boardState.curPlayerNum].mines.Remove(boardState.playerList[boardState.curPlayerNum].mines[boardState.playerList[boardState.curPlayerNum].mines.FindIndex(x => x.gridLocation == objectAimedAt.gridLocation)]);
                    //play sound
                    Mine.PlayMineExplosion();
                    //remove from all lists that have it
                    foreach (Player player in boardState.playerList)
                    {
                        for (int i = 0; i < player.mines.Count; i++)
                        {
                            if (player.mines[i].gridLocation == objectAimedAt.gridLocation)
                            {
                                player.mines.Remove(player.mines[i]);
                            }
                        }
                    }
                }
                //give the smoke cloud animation to the location of what is being shot
                smokepuff.AnimationLocation = objectAimedAt.curSquare.Center + new Vector2(-35*size.X,-35*size.Y);
                //reset the bools required
                tankFound = false; mineFound = false; wallFound = false;
                targetAquired = false; gunAimed = false;
                //muzzleflash timer start
                flashTimer = 100;
            }
        }

        private bool rotateGun(Vector2 tankLocation, Vector2 objectLocation)
        {
            bool gunLinedUp = false;
            //get the rotation angle
            Vector2 rotationVector = objectLocation - tankLocation;
            double desiredAngle = (float)(Math.Atan2(rotationVector.Y, rotationVector.X)) + (float)Math.PI / 2;
            //desiredAngle += 6.3; //this gets it to the next loop in rotation, removing negative numbers
            desiredAngle = Math.Round(desiredAngle, 1); //round it to prevent float issues
            //find the fastest route to the correct angle
            double modulus = 5.2; //should be 4.8(max rotation number) but idk why this works better. Would not allow for going min to max
            double toRight = (desiredAngle + modulus + 1 - gunAngle) % (modulus + 1);

            //excute the course till the gun is lined up
            if (toRight > (modulus+1)/2)
            {
                gunAngle -= 0.05F;                
            }
            else 
            {
                gunAngle += 0.05F;
            }
            //loop the numbers (max and min angles connect)
            if (gunAngle < -1.5)
            {
                gunAngle = 4.8;
            }
            if (gunAngle > 4.8)
            {
                gunAngle = -1.5;
            }
            gunAngle = Math.Round(gunAngle, 2);
            if (desiredAngle == gunAngle)
            {
                gunLinedUp = true;
            }
            return gunLinedUp;
        }
        private bool FireGun(BoardState boardState, ref bool fired)
        {
            //spend AP, tell the outside function the gun when off, and play the tank fire sound
            boardState.playerList[boardState.curPlayerNum].AP -= fireCost;
            fired = true;
            playFireSoundEffect();
            playSmokePuff = true;
            return true;
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
                    //to let the drawcode know it was an object and not just a visable empty tile
                    CircleTiles[(int)@object.X, (int)@object.Y].identifier = 1;
                    //if the object was found as a tank before wall checks
                    bool found = false;
                    //get the subgrids location relative to the board to actaully check locations of objects from there boardstate lists
                    Point subgridLocation = new Point(selectedTank.gridLocation.X - selectedTank.range, selectedTank.gridLocation.Y - selectedTank.range);

                    //check if the object is a friendly tank and make the rectangle null/untargetable (remove it from eligble targets)
                    //or if the object is a dead enemy tank
                    for (int i = 0; i < boardState.playerList.Count; i++)
                    {
                        //tanks checks
                        foreach (Tank tank in boardState.playerList[i].tanks)
                        {
                            //means its an ally tank or dead tank
                            if (i == boardState.curPlayerNum || !tank.alive)
                            {
                                Point tankLocation = tank.gridLocation - subgridLocation;
                                if (tankLocation == new Point((int)@object.X, (int)@object.Y))
                                {
                                    CircleTiles[(int)@object.X, (int)@object.Y] = new RectangleF();
                                    found = true;
                                }
                            }
                            //if its not dead or an enemy tank, then mark it as shootable
                            else
                            {
                                Point tankLocation = tank.gridLocation - subgridLocation;
                                if (tankLocation == new Point((int)@object.X, (int)@object.Y))
                                {
                                    tank.setInLOS(true);
                                    found = true;
                                    boardState.objectsInLOS.Add(tank);
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        //wall checks
                        for (int i = 0; i < boardState.walls.Count; i++)
                        {
                            if (boardState.walls[i].destroyable)
                            {
                                if (!boardState.walls[i].multiWall)
                                {
                                    Point wallLocation = boardState.walls[i].gridLocation - subgridLocation;
                                    if (new Point((int)@object.X, (int)@object.Y) == wallLocation)
                                    {
                                        boardState.walls[i].setInLOS(true);
                                        boardState.objectsInLOS.Add(boardState.walls[i]);
                                    }
                                }
                                else
                                {
                                    foreach (Point point in boardState.walls[i].gridLocations)
                                    {
                                        if (new Point((int)@object.X, (int)@object.Y) == point - subgridLocation)
                                        {
                                            boardState.walls[i].setInLOS(true);
                                            boardState.objectsInLOS.Add(boardState.walls[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //make the selected tanks rectangle null
                    Point selectedTankLocation = selectedTank.gridLocation - subgridLocation;
                    CircleTiles[selectedTankLocation.X, selectedTankLocation.Y] = new RectangleF();
                }
            }
            return CircleTiles;
        }
        public static void setTankNoises(string fireFileLocation, string deathFileLocation, string selectFileLocation)
        {
            fire = SoundManager.CreateSound(fireFileLocation);
            death = SoundManager.CreateSound(deathFileLocation);
            select = SoundManager.CreateSound(selectFileLocation);
        }
        /// <summary>handles playing the tank firing noise </summary>
        public static void playFireSoundEffect()
        {
            SoundManager.VolumeChecker(fire);
            fire.Stop();
            fire.Play();
        }
        private static void playDeathSoundEffect()
        {
            SoundManager.VolumeChecker(death);
            death.Stop();
            death.Play();
        }
        public static void playSelectSoundEffect()
        {
            SoundManager.VolumeChecker(select);
            select.Stop();
            select.Play();
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
            @new.gunTex = ItemToClone.gunTex;
            @new.type = ItemToClone.type;
            @new.gunAngle = ItemToClone.gunAngle;
            @new.muzzleFlash = ItemToClone.muzzleFlash;

            return @new;
        }
        #endregion
    }
}
