﻿using System;
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

        //info storage for circles in the grid
        protected List<Point> objectLocations = new List<Point>();
        protected List<Vector2> blockersInCircle = new List<Vector2>();
        protected List<Vector2> wallsInGrid = new List<Vector2>();
        protected RectangleF[,] CircleTiles;
        protected bool drawTankInfo = false;

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
        }
        public override void Update()
        {
            base.Update();
            //if a tank is selected then an object is active
            if (drawTankInfo)
            {
                anyObjectActive = true;
            }
            //if escape was pressed with an active object turn off the selected tank
            if (escapePressed && anyObjectActive)
            {
                drawTankInfo = false;
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
                    boardState = new BoardState(levelManager.getEntities(), levelManager.getWalls(), levelManager.getItemBoxes());
                    sweeps = levelManager.getSweeps();

                    //get player amount and make players with spawn regions for each one
                    int numOfPlayers = levelManager.getPlayerCount();
                    for (int i = 0; i < numOfPlayers; i++)
                    {
                        boardState.playerList.Add(new Player(AP, sweeps));
                        boardState.playerList[i].SpawnTiles = levelManager.getPlayerSpawns()[i];
                        //remove them from the entities list. They dont need to be there. Only there for the level editors purposes
                        foreach (SpawnTile tile in boardState.playerList[i].SpawnTiles)
                        {
                            var e = boardState.entities.Find(x => x.gridLocation == tile.gridLocation);
                            boardState.entities.Remove(e);
                        }
                    }

                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();

                    //finish loading the board
                    curBoard.LoadContent();
                    boardState.LoadEntities();
                    boardState.getGridLocations();

                    RowsCol = curBoard.Rows;
                    //get the cellMap and load the pathfinder with it
                    cellMap = levelManager.getCellMap();
                    pathfinder = new Pathfinder(cellMap);
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
            pathfinder.AlterCell(boardState.walls[i].gridLocation, 0); //remove its cellmap marker
            boardState.RemoveWall(i);
            getObjectLocationsForVision(); //redo the circle tiles object list
            getLOS(); //redo vision check
        }
        private void addWallDuringGame(Point gridPosition)
        {
            if (!boardState.wallLocations.Contains(gridPosition))
            {
                pathfinder.AlterCell(gridPosition, 1); //add its cellmap marker
                boardState.AddWall(gridPosition, curBoard);
                getObjectLocationsForVision(); //redo the circle tiles object list
                getLOS(); //redo vision check
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
                        drawTankInfo = true;
                    }
                }

                if (drawTankInfo)
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
            CircleTiles = curBoard.getRectanglesInRadius(
            new Vector2(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation.X, boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation.Y),
            boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].range, objectLocations, out blockersInCircle);
            Tank.findTilesInLOS(boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum], CircleTiles, blockersInCircle, boardState);
        }
        #endregion
        protected void pathFind(Point start)
        {
            Point end = new Point();
            curBoard.getGridSquare(worldPosition, out end);

            if (drawTankInfo)
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
            boardState.SetToPreviousState(previousBoardState);
            activeTankNum = -1;
            drawTankInfo = false;
            path = new List<Cell>();

        }
        protected void MoveOrShoot()
        {
            //ensure the tank is active and has player has ap left
            if (activeTankNum != -1)
            {
                if (boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].Active && boardState.playerList[boardState.curPlayerNum].AP > 0)
                {
                    //if there is a path
                    if (path != null)
                    {
                        //if the mouse is within the board
                        if (mouseInBoard)
                        {
                            //if the mouse gets clicked while there is a mouse
                            if (curLeftClick == ButtonState.Pressed && oldLeftClick != ButtonState.Pressed)
                            {
                                //get the AP and then move that distance, starting from the back of the list (the list is in reverse move order)
                                if (path.Count > 1)
                                {
                                    //keep track of the tanks path through the list of tiles
                                    int checkedTiles = 0;
                                    //keep checking as long as the player has ap to spend and the path has more tiles to check
                                    while (checkedTiles <= boardState.playerList[boardState.curPlayerNum].AP && checkedTiles < path.Count)
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
                                                        boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].alterHP(-8);//kill the tank
                                                        //move the tank where the mine was/current tile checked
                                                        boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation = path[path.Count - 1 - checkedTiles].location;
                                                        boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].curSquare = curBoard.getGrid()
                                                        [path[path.Count - 1 - checkedTiles].location.X, path[path.Count - 1 - checkedTiles].location.Y];
                                                        //spend the AP required to move there
                                                        boardState.playerList[boardState.curPlayerNum].AP -= checkedTiles;
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
                                    if (boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].alive)
                                    {
                                        checkedTiles--; //do this since if the tank lived, it added 1 extra at the end of the loop
                                        boardState.playerList[boardState.curPlayerNum].AP -= checkedTiles;
                                        boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].gridLocation = path[path.Count - 1 - checkedTiles].location;
                                        boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].curSquare = curBoard.getGrid()
                                            [path[path.Count - 1 - checkedTiles].location.X, path[path.Count - 1 - checkedTiles].location.Y];
                                    }
                                    //redo the Line of Set check for the new position
                                    getLOS();
                                }
                            }
                            else if (curRightClick == ButtonState.Pressed && oldRightClick != ButtonState.Pressed)
                            {
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
                                                    if (boardState.playerList[boardState.curPlayerNum].AP > 1)//if player has ap to fire
                                                    {
                                                        //if (item logic)
                                                        //damge being delt 
                                                        int damage = boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].damage;
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
                                                                            boardState.playerList[boardState.curPlayerNum].AP -= 2;
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
                                                                        boardState.playerList[boardState.curPlayerNum].AP -= 2;
                                                                        boardState.walls[i].alterHP(-(damage - 1));
                                                                        boardState.walls[i].showHealth = true;
                                                                        fired = true;
                                                                        if (!boardState.walls[i].alive)
                                                                        {
                                                                            removeWallDuringGame(i);
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
                        }
                    }
                }
            }
        }

        #endregion   

        #region BUTTON EVENTS
        protected void EndTurnPressed(object sender, EventArgs e)
        {
            //make number of players 0 based. If the current player isnt the last player move to next
            if (boardState.curPlayerNum < levelManager.getPlayerCount() - 1)
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
            drawTankInfo = false;
            //update the pathfiner with mine locations
            UpdatePathFinderWithMines(boardState, pathfinder);
            //set the previous board state to be the start of the next turn
            previousBoardState = BoardState.SavePreviousBoardState(boardState);
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
                //get the amount of the current selected item. 
                boardState.playerList[boardState.curPlayerNum].inventory.setSelectedItemCount(selectedItem);
                //use the update code if the mouse is in the board
                if (mouseInBoard && boardState.playerList[boardState.curPlayerNum].inventory.selectedItemsCount > 0)
                boardState.playerList[boardState.curPlayerNum].inventory.UseItem(Item, boardState, curBoard, pathfinder, curGridLocation, drawTankInfo, activeTankNum, curLeftClick, oldLeftClick);
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
        private void DeselectTank()
        {
            //deselect tanks when selecting sweeper
            if (drawTankInfo)
            {
                drawTankInfo = false;
                boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].Active = false;
                activeTankNum = -1;
            }
        }
        #endregion
    }
}
