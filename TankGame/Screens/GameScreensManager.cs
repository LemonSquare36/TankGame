using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using System.Threading.Tasks;
using System.Diagnostics;
using TankGame.Objects.Entities.Items;
using TankGame.GameInfo;
using System.Linq;

namespace TankGame
{
    internal class GameScreensManager : ScreenManager
    {
        //hold the level file location
        private string file;
        //Action points per turn
        int AP = 4;
        //turn tracker
        int activeTankNum = -1;

        Texture2D trail, end;


        //info storage for circles in the grid
        protected List<Point> objectLocations = new List<Point>();
        protected List<Vector2> blockersInCircle = new List<Vector2>();
        protected List<Vector2> wallsInGrid = new List<Vector2>();
        protected RectangleF[,] CircleTiles;
        protected bool drawTankInfo = false;
        protected bool DrawTankInfo
        {
            get { return drawTankInfo; }
            set
            {
                drawTankInfo = value; if (!value) //if its set to false
                {
                    resetLOSObjects();                   
                }
            }
        }



        //pathfinding information
        private Cell[,] cellMap;
        protected Pathfinder pathfinder;
        protected List<Cell> path = new List<Cell>();

        protected bool ActiveItemWarning = false, itemActive = false;
        protected string selectedItem;


        #region base functions
        public override void Initialize()
        {
            base.Initialize();
            //create the player classes
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            //sprites used to drawing the pathfinder path
            trail = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/pathTrail");
            end = Main.GameContent.Load<Texture2D>("GameSprites/BattleSprites/pathEnd");
        }
        public override void Update()
        {
            base.Update();
            //if a tank is selected then an object is active
            if (DrawTankInfo)
            {
                anyObjectActive = true;
            }
            //if escape was pressed with an active object turn off the selected tank
            if (escapePressed && anyObjectActive)
            {
                DrawTankInfo = false;
                boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].Active = false;
                activeTankNum = -1;
            }
        }
        #endregion
        protected void LoadLevelFile()
        {
            //load the board and additional data from the file passed in levelselect.
            file = relativePath + "\\TankGame\\LevelFiles\\" + selectedFile + ".lvl";
            if (file != relativePath + "\\TankGame\\LevelFiles\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.2468F, 0.05F);
                    //grab the informatin from the levelManager
                    boardState = new BoardState(levelManager.getWalls(), levelManager.getItemBoxes(), levelManager.getHoles());

                    //load ruleSet

                    //get player amount and make players with spawn regions for each one
                    for (int i = 0; i < rules.numOfPlayers; i++)
                    {
                        boardState.playerList.Add(new Player(AP, rules.startingSweeps));
                        boardState.playerList[i].SpawnTiles = levelManager.getPlayerSpawns()[i];
                    }

                    curBoard = levelManager.getGameBoard();

                    //finish loading the board
                    curBoard.LoadContent();
                    boardState.LoadEntities();
                    boardState.getGridLocations();

                    RowsCol = curBoard.Rows;
                    //get the cellMap and load the pathfinder with it
                    cellMap = levelManager.getCellMap();
                    pathfinder = new Pathfinder(cellMap);

                    //set noises
                    Tank.setTankNoises("Sounds/tankshot", "Sounds/tankdeath", "Sounds/click");
                    Mine.SetMineSoundEffects("Sounds/tankdeath");
                }
                catch { }
            }
            else { }
        }
        #region entity handling
        /// <summary></summary>
        /// <param name="i">the list position of the wall you want to remove</param>
        private void removeWallDuringGame(int i)
        {
            if (!boardState.walls[i].multiWall)
            {
                pathfinder.AlterCell(boardState.walls[i].gridLocation, 0); //remove its cellmap marker
            }
            else
            {
                //remove all map markers
                foreach (Point gridLocation in boardState.walls[i].gridLocations)
                {
                    pathfinder.AlterCell(gridLocation, 0); //remove its cellmap marker
                }
            }            
            boardState.RemoveWall(i);
            getObjectLocationsForVision(); //redo the circle tiles object list
            getLOS(); //redo vision check
        }
        private void addWallDuringGame(Point gridPosition)
        {
            if (!boardState.wallLocations.Contains(gridPosition))
            {
                pathfinder.AlterCell(gridPosition, 1); //add its cellmap marker
                boardState.AddWall(gridPosition, curBoard, true);
                getObjectLocationsForVision(); //redo the circle tiles object list
                getLOS(); //redo vision check
            }
        }
        protected void AddEntity(string type, InputBox tanksCount, InputBox minesCount, ref int minesUsed, ref int tanksUsed)
        {
            Mine tempMine;
            Tank tempTank;
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is left clicked once inside the board (add code)
                if (mouse.LeftButton == ButtonState.Pressed && oldLeftClick != curLeftClick)
                {
                    //get the current rectangle the mouse is within
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);

                    //this is to create local objects to prevent it from getting angy for there not being declared objects in use
                    tempMine = new Mine(curGrid, curGridLocation);
                    tempTank = new Tank(curGrid, curGridLocation, "Regular");

                    switch (type)
                    {
                        case "regTank":

                            break;
                        case "sniperTank":
                            tempTank = new Tank(curGrid, curGridLocation, "Sniper");

                            break;
                        case "scoutTank":
                            tempTank = new Tank(curGrid, curGridLocation, "Scout");

                            break;
                        case "mine":

                            break;
                        default:
                            break;
                    }
                    //if there is not an object currently in that tile
                    if (!boardState.gridLocations.Contains(curGridLocation))
                    {
                        //if can afford another mine
                        if (type == "mine" && Convert.ToInt16(minesCount.Text) > 0)
                        {
                            //cant be placed in a spawn tile
                            if (!levelManager.getAllSpawnTiles().Any(x => x.gridLocation == curGridLocation))
                            {
                                //load and add mine
                                minesUsed++;
                                tempMine.LoadContent();
                                boardState.playerList[boardState.curPlayerNum].mines.Add(tempMine);
                                boardState.gridLocations.Add(tempMine.gridLocation);
                            }
                        }
                        //must be a tank //must be able to afford
                        else if (Convert.ToInt16(tanksCount.Text) >= tempTank.buildCost)
                        {
                            //must be inside a spawn tile for the correct player
                            if (boardState.playerList[boardState.curPlayerNum].SpawnTiles.Any(x => x.gridLocation == curGridLocation))
                            {
                                //load and add tank
                                tanksUsed += tempTank.buildCost;
                                tempTank.LoadContent();
                                boardState.playerList[boardState.curPlayerNum].tanks.Add(tempTank);
                                boardState.gridLocations.Add(tempTank.gridLocation);
                            }
                        }
                        else
                        {
                            //couldnt place object so put failure code here
                        }
                    }
                }
            }
        }
        protected void RemoveTankOrMine(ref int tanksUsed, ref int minesUsed)
        {
            if (mouseInBoard)
            {
                //if mouse is right clicked once in the board (remove code)
                if (mouse.RightButton == ButtonState.Pressed && oldRightClick != curRightClick)
                {
                    //if the gridlocations tracker has an object there
                    if (boardState.gridLocations.Contains(curGridLocation))
                    {
                        //if tank list has any tanks with that current grid location
                        if (boardState.playerList[boardState.curPlayerNum].tanks.Any(x => x.gridLocation == curGridLocation))
                        {
                            //find which tank index is the current one based on gridlocation to remove it
                            int i = boardState.playerList[boardState.curPlayerNum].tanks.FindIndex(x => x.gridLocation == curGridLocation);

                            //give back build cost before removing tank
                            tanksUsed -= boardState.playerList[boardState.curPlayerNum].tanks[i].buildCost;
                            //remove its records
                            boardState.playerList[boardState.curPlayerNum].tanks.Remove(boardState.playerList[boardState.curPlayerNum].tanks[i]);
                            boardState.gridLocations.Remove(curGridLocation);
                        }

                        //if mines list has any mines with that current grid location
                        else if (boardState.playerList[boardState.curPlayerNum].mines.Any(x => x.gridLocation == curGridLocation))
                        {
                            //find which tank index is the current one based on gridlocation to remove it
                            int i = boardState.playerList[boardState.curPlayerNum].mines.FindIndex(x => x.gridLocation == curGridLocation);

                            //give back build cost before removing mine
                            minesUsed--;
                            //remove its records
                            boardState.playerList[boardState.curPlayerNum].mines.Remove(boardState.playerList[boardState.curPlayerNum].mines[i]);
                            boardState.gridLocations.Remove(curGridLocation);
                        }
                    }
                }
            }
        }

        #endregion
        /// <summary>sets the current selected tank as active and gets the circleTiles(line of sight applied). Sets draw circle to true</summary>
        protected void checkSelectedTank()
        {
            //check each tank for the mouse inside it
            foreach (Tank tank in boardState.playerList[boardState.curPlayerNum].tanks)
            {
                //returns true if the mouse is inside the tank
                if (tank.curSquare.Contains(worldPosition))
                {
                    //if the mouse is clicked on the tank and the tanks isnt already active
                    if (curLeftClick == ButtonState.Pressed && !tank.Active && tank.alive)
                    {
                        //set all tanks to inactive
                        foreach (Tank tank2 in boardState.playerList[boardState.curPlayerNum].tanks)
                        {
                            tank2.Active = false;
                        }
                        //set this tank to active
                        tank.Active = true;
                        activeTankNum = boardState.playerList[boardState.curPlayerNum].getActiveTankNum();
                        //get the walls + current enemy tanks
                        getObjectLocationsForVision();
                        //get the circle around the selected tank                   
                        getLOS();
                        DrawTankInfo = true;
                        Tank.playSelectSoundEffect();
                    }
                }

                if (DrawTankInfo)
                {
                    pathFind(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation);
                }
            }
        }

        #region LOS Methods
        private void getObjectLocationsForVision()
        {

            objectLocations = new List<Point>(boardState.getWallLocations());
            foreach (Point t in boardState.tankGridLocations)
            {
                objectLocations.Add(t);
                //make sure to remove the current selected tank
                objectLocations.Remove(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation);
            }
        }
        private void getLOS()
        {
            resetLOSObjects();
            CircleTiles = curBoard.getRectanglesInRadius(
            new Vector2(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation.X, boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation.Y),
            boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].range, objectLocations, out blockersInCircle);
            Tank.findTilesInLOS(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum], CircleTiles, blockersInCircle, boardState);
        }
        private void resetLOSObjects()
        {
            boardState.objectsInLOS.Clear();
            //all walls set to true, now set to false
            foreach (Wall wall in boardState.walls)
            {
                if (wall.getInLOS())
                {
                    wall.setInLOS(false);
                }
            }
            //all tanks set to true, now set to false
            foreach (Player player in boardState.playerList)
            {
                foreach(Tank tank in player.tanks)
                {
                    if (tank.getInLOS())
                    {
                        tank.setInLOS(false);
                    }
                }
            }
        }
        #endregion
        protected void pathFind(Point start)
        {
            Point end = new Point();
            curBoard.getGridSquare(worldPosition, out end);

            if (DrawTankInfo)
            {
                path = pathfinder.getPath(cellMap[start.X, start.Y], cellMap[end.X, end.Y], boardState.tankGridLocations);
            }
        }


        #region turnTakingCode
        //information for start of turn state

        /// <summary> Get the old information and apply it to the tracked information. 
        /// This will act as an undo effect. Setting the turn back to the beginning </summary>
        public void SetTurnState()
        {
            boardState.SetToPreviousState(previousBoardState, curBoard);
            activeTankNum = -1;
            DrawTankInfo = false;
            path = new List<Cell>();

        }
        protected void MoveOrShoot()
        {
            //ensure the tank is active and has player has ap left
            if (activeTankNum != -1)
            {
                if (boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].Active && boardState.playerList[boardState.curPlayerNum].AP > 0)
                {
                    //if the mouse is within the board
                    if (mouseInBoard)
                    {
                        //if the mouse gets clicked while there is a mouse
                        if (curLeftClick == ButtonState.Pressed && oldLeftClick != ButtonState.Pressed)
                        {
                            //if there is a path
                            if (path != null)
                            {
                                bool itemGotten;
                                bool tankDied;
                                boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].TankMove(curBoard, ref boardState, ref previousBoardState, path, out tankDied, out itemGotten);
                                if (tankDied)
                                    DrawTankInfo = false;
                                //redo the Line of Set check for the new position
                                if (!tankDied)
                                    getLOS();
                                if (itemGotten)
                                {
                                    GivePlayerItem();
                                    //getting an item is permanant descision
                                    previousBoardState = BoardState.SavePreviousBoardState(boardState, curBoard);
                                }
                            }
                        }
                        //-----------------------------------------------------------------------
                        //FIRE CODE
                        //-----------------------------------------------------------------------
                        else if (curRightClick == ButtonState.Pressed && oldRightClick != ButtonState.Pressed)
                        {
                            //this goes into the function and returns with a value that could mean something
                            int wallToRemove;

                            boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].TankShoot(curBoard, boardState, CircleTiles,worldPosition, pathfinder, out wallToRemove);
                            if (wallToRemove != -1)
                            {
                                removeWallDuringGame(wallToRemove);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gives the player a random item from allowed items
        /// </summary>
        private void GivePlayerItem()
        {
            //get a random number
            Random rand = new Random();
            int i = rand.Next(0, rules.allowedItems.Count - 1);
            //use the random number to get an item from allowed items list, then set that item to +1 its current value
            boardState.playerList[boardState.curPlayerNum].inventory.setSelectedItemsCount(rules.allowedItems[i],
                boardState.playerList[boardState.curPlayerNum].inventory.getSelectedItemsCount(rules.allowedItems[i]) + 1);
        }
        #endregion

        #region BUTTON EVENTS
        protected void EndTurnPressed(object sender, EventArgs e)
        {
            //make number of players 0 based. If the current player isnt the last player move to next
            if (boardState.curPlayerNum < rules.numOfPlayers - 1)
            {
                boardState.curPlayerNum++;
            }
            else //if it is the last player set it to the first player (which is 0 in a 0 base)
            {
                boardState.curPlayerNum = 0;
            }

            //reset all tanks to inactive
            activeTankNum = -1;
            foreach (Tank tank in boardState.playerList[boardState.curPlayerNum].tanks)
            {
                tank.Active = false;
            }
            //reset the players AP to the turn start AP
            boardState.playerList[boardState.curPlayerNum].AP = boardState.playerList[boardState.curPlayerNum].startAP;
            //clear the path for the pathfinder
            path = new List<Cell>();
            //dont draw tank info immediatly
            DrawTankInfo = false;
            //update the pathfiner with mine locations
            UpdatePathFinderWithMines(boardState, pathfinder);
            //set the previous board state to be the start of the next turn
            previousBoardState = BoardState.SavePreviousBoardState(boardState, curBoard);
        }
        #endregion

        #region random methods without home
        /// <summary>this method makes friendly mines avoided by the pathfinder while making enemy mines invisible to the pathfinder </summary>
        public static void UpdatePathFinderWithMines(BoardState boardState, Pathfinder pathfinder)
        {
            //update enemies first to invis before allied to wall.
            //If an allied on is on an enemies, it should override the invis portion making it still unpassable
            //make enemy mines invisible to the pathfinder
            for (int i = 0; i < boardState.playerList.Count; i++)
            {
                if (i != boardState.curPlayerNum)
                {
                    foreach (Mine mine in boardState.playerList[i].mines)
                    {
                        pathfinder.AlterCell(mine.gridLocation, 0);
                    }
                }
            }
            //make the friendly mines "walls" to go around
            foreach (Mine mine in boardState.playerList[boardState.curPlayerNum].mines)
            {
                pathfinder.AlterCell(mine.gridLocation, 1);
            }
        }
        /// <summary>
        /// runs the update code for items if an item is currently selected
        /// </summary>
        /// <param name="Item">string name for the item in use</param>
        protected void UseItem(string Item)
        {
            if (itemActive)
            {
                if (Item == "sweeper")
                {
                    DeselectTank();
                }
                //use the update code if the mouse is in the board
                if (mouseInBoard)
                    boardState.playerList[boardState.curPlayerNum].inventory.UseItem(Item, boardState, curBoard, pathfinder, curGridLocation, DrawTankInfo, activeTankNum, curLeftClick, oldLeftClick);
            }
        }
        /// <summary>
        ///  runs the draw code for items if an item is currently selected
        /// </summary>
        /// <param name="item">string name for the item in use</param>
        /// <param name="UITexture">Texture2D needed for the Items unique drawing to the screen. THe items UI</param>
        /// <param name="font"></param>
        /// <param name="warningMessageColor"></param>
        /// <param name="warningLength">in milliseconds</param>
        protected void DrawItemUI(string item, Texture2D UITexture, SpriteFont font, Color warningMessageColor, int warningLength)
        {
            if (itemActive)
                boardState.playerList[boardState.curPlayerNum].inventory.DrawItemUI(selectedItem, spriteBatch, UITexture);
        }
        protected void PlayItemAnimations(SpriteBatch spriteBatch)
        {
            boardState.playerList[boardState.curPlayerNum].inventory.DrawItemAnimation(spriteBatch, curBoard.getGridSquare(1, 1).Size.X / 50);
        }
        private void DeselectTank()
        {
            //deselect tanks when selecting sweeper
            if (DrawTankInfo)
            {
                DrawTankInfo = false;
                boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].Active = false;
                activeTankNum = -1;
            }
        }
        public void DrawPath(SpriteBatch spriteBatch, SpriteFont font)
        {
            RectangleF cellRect = new RectangleF();
            for (int i = 0; i < path.Count; i++)
            {
                if (path.Count > 9)
                {

                }
                //dont do the final path cuase no parent (its where the tank is)
                if (path[i].Parent != null)
                {
                    //the cellRectangle of the previous cell in the list
                    RectangleF oldCellRect = cellRect;
                    cellRect = curBoard.getGridSquare(path[i].X, path[i].Y);
                    Vector2 rectScale = cellRect.Size / new Vector2(48, 48);
                    //finds the count in the non reverse way (for drawing text to screen)
                    int reverseListCounter = path.Count - 1 - i;
                    //if it is the first item in the list and out of range, draw in red
                    if (i == 0 && reverseListCounter > boardState.playerList[boardState.curPlayerNum].AP / boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].movementCost)
                    {
                        //we devide by 50 here since that is the size of the sprite (make sure it scales with the size of the board rectangles)
                        spriteBatch.Draw(end, cellRect.Location, null, Color.DarkRed, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                        //48 is the internal tile size at the standard map size of 20. The calculations are based on the internal size for a "standard" tile\
                        //just change the float it multiplies with to create scale since at the standard (48) * 1 the font would fill the whole rectangle. Dont make larger than 1
                        spriteBatch.DrawString(font, Convert.ToString(reverseListCounter), cellRect.Center + new Vector2(-10 * rectScale.X, -10 * rectScale.Y), Color.Black, 0, Vector2.Zero, .6f * cellRect.Width / 48, SpriteEffects.None, 0);
                    }//if its the first one but not out of range
                    else if (i == 0)
                    {
                        //we devide by 50 here since that is the size of the sprite (make sure it scales with the size of the board rectangles)
                        spriteBatch.Draw(end, cellRect.Location, null, Color.White, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                        //48 is the internal tile size at the standard map size of 20. The calculations are based on the internal size for a "standard" tile\
                        //just change the float it multiplies with to create scale since at the standard (48) * 1 the font would fill the whole rectangle. Dont make larger than 1
                        spriteBatch.DrawString(font, Convert.ToString(reverseListCounter), cellRect.Center + new Vector2(-10 * rectScale.X, -10 * rectScale.Y), Color.Black, 0, Vector2.Zero, .6f * cellRect.Width / 48, SpriteEffects.None, 0);

                    }
                    //if the reverse counter is less than equals the amount the tank can move, then draw that differently, so the player knows where they will stop
                    else if (reverseListCounter == boardState.playerList[boardState.curPlayerNum].AP / boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].movementCost)
                    {
                        //we devide by 50 here since that is the size of the sprite (make sure it scales with the size of the board rectangles)
                        spriteBatch.Draw(end, cellRect.Location, null, Color.White, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                        //48 is the internal tile size at the standard map size of 20. The calculations are based on the internal size for a "standard" tile\
                        //just change the float it multiplies with to create scale since at the standard (48) * 1 the font would fill the whole rectangle. Dont make larger than 1
                        spriteBatch.DrawString(font, Convert.ToString(reverseListCounter), cellRect.Center + new Vector2(-10 * rectScale.X, -10 * rectScale.Y), Color.Black, 0, Vector2.Zero, .6f * cellRect.Width / 48, SpriteEffects.None, 0);
                    }
                    //its not a special spot but it is over the amount they can move (draw red)
                    else if (i != 0 && reverseListCounter > boardState.playerList[boardState.curPlayerNum].AP / boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].movementCost)
                    {
                        spriteBatch.Draw(trail, cellRect.Center + (new Vector2(-8, -8) * rectScale), null, Color.DarkRed, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                    }
                    else //otherwise draw trail in white
                    {
                        spriteBatch.Draw(trail, cellRect.Center + (new Vector2(-8, -8) * rectScale), null, Color.White, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                    }
                    #region old code with angle finding
                    /*else if (i == 1)
                    {

                        spriteBatch.Draw(trail, cellRect.Center + new Vector2(-7,-7), null, Color.Black, 0, Vector2.Zero, rectScale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        Vector2 curLocation = cellRect.Center;

                        //gets the distance, but does not unsquare it
                        double Distance = 50;
                        float scale = cellRect.Size.Y / 48; //scale to draw the texture too later
                        float DistancePer = 13 * scale;//size of the sprite distance
                        float DistanceTraveled = 0;//how far weve gone to the next point
                        Vector2 rotationVector = oldCellRect.Center - cellRect.Center;
                        float Rotation = (float)(Math.Atan2(rotationVector.Y, rotationVector.X)) + (float)Math.PI / 2;

                        while (DistanceTraveled <= Distance*scale)
                        {
                            spriteBatch.Draw(trail, curLocation, null, Color.Black, Rotation, new Vector2(4,4), rectScale, SpriteEffects.None, 0);

                            //path of travel is to the up and left
                            if (cellRect.Center.X > oldCellRect.Center.X && cellRect.Center.Y > oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(-DistancePer, -DistancePer);
                            }
                            //path of travel is to the down and right
                            else if (cellRect.Center.X < oldCellRect.Center.X && cellRect.Center.Y < oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(DistancePer, DistancePer);
                            }
                            //path of travel is to the down and left
                            else if (cellRect.Center.X > oldCellRect.Center.X && cellRect.Center.Y < oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(-DistancePer, DistancePer);
                            }
                            //path of travel is to the up and right
                            else if (cellRect.Center.X < oldCellRect.Center.X && cellRect.Center.Y > oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(DistancePer, -DistancePer);
                            }
                            //path of travel is to the up
                            else if (cellRect.Center.X == oldCellRect.Center.X && cellRect.Center.Y > oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(0, -DistancePer);
                            }
                            //path of travel is to the down
                            else if (cellRect.Center.X == oldCellRect.Center.X && cellRect.Center.Y < oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(0, DistancePer);
                            }
                            //path of travel is to the left
                            else if (cellRect.Center.X > oldCellRect.Center.X && cellRect.Center.Y == oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(-DistancePer, 0);
                            }
                            //path of travel is to the right
                            else if (cellRect.Center.X < oldCellRect.Center.X && cellRect.Center.Y == oldCellRect.Center.Y)
                            {
                                curLocation += new Vector2(DistancePer, 0);
                            }
                            DistanceTraveled += DistancePer;
                        }

                    }*/
                    #endregion
                }
            }
        }
        #endregion
        #region Sounds and Music
        /// <summary>Assign a noise to every button in the list</summary>
        protected void AssignButtonNoise(List<Button> buttonList, string FileLocation)
        {
            foreach (Button button in buttonList)
            {
                button.addSoundEffect(FileLocation);
            }
        }
        /// <summary>assign a noise to only one button</summary>
        protected void AssignButtonNoise(Button button, string FileLocation)
        {
            button.addSoundEffect(FileLocation);
        }
        #endregion
    }
}
