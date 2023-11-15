using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.GameInfo;
namespace TankGame
{
    internal class BattleScreenLocal : GameScreensManager
    {
        //UI stuff
        SpriteFont font;
        Texture2D UI_bg, UI_filler, spawnTex;
        Color bgColor, bgFillerColor;
        int R, G, B;
        Vector3 bgColorOffset;

        //int activePlayer;
        //logic that determines what stage the game is in
        private bool placementStage, battleStarted;
        //logic for various purposes
        private bool placementWarning = false;

        InputBox tanksCount, minesCount;
        Button tanks, mines, ready, endturn, undo, sweep;
        List<Button> placementButtonList = new List<Button>();
        List<Button> battleButtonList = new List<Button>();

        int tanksUsed = 0, minesUsed = 0;


        public override void Initialize()
        {
            base.Initialize();
            placementStage = true;
            battleStarted = false;
            //activePlayer = 1;
            //create input boxes for displaying tanks and mine count
            tanksCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1550, 700), new Vector2(80, 70), 0);
            minesCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1750, 700), new Vector2(80, 70), 0);

            //color stuff for background
            bgFillerColor = new Color(255, 255, 255); //filler color
            bgColor = Color.LightBlue; //current background color shade
            bgColorOffset = new Vector3(bgColor.R, bgColor.G, bgColor.B);
            //offset the filler color to match with the bg color changes applied to the bg sprites
            R = Convert.ToInt16((float)bgFillerColor.R * (float)(bgColorOffset.X / 255));
            G = Convert.ToInt16((float)bgFillerColor.G * (float)(bgColorOffset.Y / 255));
            B = Convert.ToInt16((float)bgFillerColor.B * (float)(bgColorOffset.Z / 255));
            bgFillerColor = new Color(R, G, B);
        }

        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            //load the board (load for host before sending to peer)
            LoadBoardfromFile();
            //player spawn rows calc
            getSpawnRegion();

            //font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            //textures
            UI_bg = Main.GameContent.Load<Texture2D>("Backgrounds/Battlescreen/UI_BG");
            UI_filler = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            spawnTex = Main.GameContent.Load<Texture2D>("GameSprites/spawnHighLight");

            //SendLoadToPeer();

            #region load buttons and input boxes
            //load buttons
            tanks = new Button(new Vector2(1565, 640), 50, 50, "GameSprites/BattleSprites/Tank", "tanks", "toggle");
            tanks.ChangeButtonColor(Color.Black);
            mines = new Button(new Vector2(1765, 640), 50, 50, "GameSprites/BattleSprites/Mine", "mines", "toggle");
            ready = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/Ready", "ready");
            endturn = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/EndTurn", "endturn");
            undo = new Button(new Vector2(1590, 600), 100, 50, "Buttons/BattleScreen/Undo", "undo");
            sweep = new Button(new Vector2(110, 100), 50, 50, "Buttons/BattleScreen/Sweep", "sweep", "toggle");

            //load inputboxes
            tanksCount.LoadContent();
            minesCount.LoadContent();
            //populate the information to show how many tanks and mines they get to place
            tanksCount.Text = Convert.ToString(TanksAndMines.X);
            minesCount.Text = Convert.ToString(TanksAndMines.Y);
            #endregion

            #region list adds
            //palcement stage buttons
            placementButtonList.Add(tanks);
            placementButtonList.Add(mines);
            placementButtonList.Add(ready);
            //battle stage buttons
            battleButtonList.Add(endturn);
            battleButtonList.Add(undo);
            battleButtonList.Add(sweep);
            #endregion

            #region Event listeners
            tanks.ButtonClicked += AddTankPressed;
            mines.ButtonClicked += AddMinePressed;
            ready.ButtonClicked += ReadyPressed;
            undo.ButtonClicked += UndoPressed;
            endturn.ButtonClicked += EndTurnPressed;
            sweep.ButtonClicked += SweepPressed;
            #endregion
        }

        public override void Update()
        {
            base.Update();
            //set MouseInBoard to true or false, if the mouse is in the board
            getMouseInBoard();
            //update code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                //update the text to show how many tanks are left
                tanksCount.Text = Convert.ToString(TanksAndMines.X - tanksUsed);
                minesCount.Text = Convert.ToString(TanksAndMines.Y - minesUsed);

                //buttons in the placement stage
                foreach (Button b in placementButtonList)
                {
                    b.Update(mouse, worldPosition);
                }
                //code for placing and picking up tanks
                Placement();
            }
            //update code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {
                boardState.getTankLocations();
                foreach (Button b in battleButtonList)
                {
                    b.Update(mouse, worldPosition);
                }
                //check if the mouse is in the board
                if (sweep.Texture == sweep.Pressed)
                {
                    SweeperPressed();
                }
                else if (mouseInBoard)
                {
                    checkSelectedTank();
                    MoveOrShoot();
                }
                else if (!mouseInBoard)
                {
                    path.Clear();
                }
            }
        }

        public override void Draw()
        {
            //background for ui elements draw
            spriteBatch.Draw(UI_filler, new Rectangle(0, 0, 1920, 54), bgFillerColor);
            spriteBatch.Draw(UI_filler, new Rectangle(0, 1026, 1920, 54), bgFillerColor);
            spriteBatch.Draw(UI_bg, Vector2.Zero, bgColor);
            spriteBatch.Draw(UI_bg, new Vector2(1446, 0), bgColor);
            //board draw 
            curBoard.drawCheckers(spriteBatch);
            curBoard.DrawOutline(spriteBatch);
            foreach (Entity e in boardState.entities)
            {
                if (placementStage)
                {
                    if (e.Type.ToString() != "mine")
                        e.Draw(spriteBatch);
                }
            }
            //draw code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                minesCount.Draw(spriteBatch);
                tanksCount.Draw(spriteBatch);
                //buttons in the placement stage
                foreach (Button b in placementButtonList)
                {
                    b.Draw(spriteBatch);
                }

                foreach (Mine mine in boardState.playerList[boardState.curPlayerNum].mines)
                {
                    mine.Draw(spriteBatch);
                }

                //give a warning the player pressed ready when they had tanks and mines left
                if (placementWarning)
                {
                    spriteBatch.DrawString(font, "You still have Tanks and\n    Mines left to place", new Vector2(1485, 500), Color.DarkRed);
                }
                //draw the tank spawn region highlight for visual indication of where tanks can go
                spriteBatch.Draw(spawnTex, boardState.playerList[boardState.curPlayerNum].spawn.Location, null, Color.LightGray, 0, Vector2.Zero, boardState.playerList[boardState.curPlayerNum].spawn.Size, SpriteEffects.None, 0);
            }

            //draw code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {
                //draw buttons
                foreach (Button b in battleButtonList)
                {
                    b.Draw(spriteBatch);
                }
                foreach (Wall wall in boardState.walls)
                {
                    wall.Draw(spriteBatch);
                }
                //draw friendly mines
                foreach (Mine mine in boardState.playerList[boardState.curPlayerNum].mines)
                {
                    mine.Draw(spriteBatch);
                }
                //draw tanks
                foreach (Player player in boardState.playerList)
                {
                    foreach (Tank tank in player.tanks)
                    {
                        tank.Draw(spriteBatch);
                    }
                }
                if (drawTankInfo)
                {
                    foreach (RectangleF rF in CircleTiles)
                    {
                        if (!rF.Null)
                        {
                            if (rF.identifier == 1)
                            {//draw red for enemy tanks and walls
                                spriteBatch.Draw(spawnTex, rF.Location, null, Color.DarkRed, 0, Vector2.Zero, rF.Size, SpriteEffects.None, 0);
                            }
                            else
                            {//draw green to show range
                                spriteBatch.Draw(spawnTex, rF.Location, null, Color.Green, 0, Vector2.Zero, rF.Size, SpriteEffects.None, 0);
                            }
                        }
                    }
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (path[i].Parent != null)
                        {
                            RectangleF cellRect = curBoard.getGridSquare(path[i].X, path[i].Y);
                            spriteBatch.Draw(spawnTex, cellRect.Location, null, Color.Blue, 0, Vector2.Zero, cellRect.Size, SpriteEffects.None, 0);

                            int reverseListCounter = path.Count - 1 - i;
                            //48 is the internal tile size at the standard map size of 20. The calculations are based on the internal size for a "standard" tile\
                            //just change the float it multiplies with to create scale since at the standard (48) * 1 the font would fill the whole rectangle. Dont make larger than 1
                            spriteBatch.DrawString(font, Convert.ToString(reverseListCounter), cellRect.Location, Color.Black, 0, Vector2.Zero, .6f * cellRect.Width / 48, SpriteEffects.None, 0);
                        }
                    }
                }
                if (mouseInBoard)
                {
                    if (sweep.Texture == sweep.Pressed)
                    {
                        SweeperPressedDrawUI(spawnTex, font);
                    }
                }
            }

            //draw current players turn info
            spriteBatch.DrawString(font, "Current Player: " + (boardState.curPlayerNum + 1), new Vector2(1550, 350), Color.Black);
            spriteBatch.DrawString(font, "Current AP: " + boardState.playerList[boardState.curPlayerNum].AP, new Vector2(1600, 450), Color.Black);

            spriteBatch.DrawString(font, "Items", new Vector2(150, 30), Color.Black);
            spriteBatch.DrawString(font, "______", new Vector2(130, 35), Color.Black);

            spriteBatch.DrawString(font, Convert.ToString(boardState.playerList[boardState.curPlayerNum].sweeps), new Vector2(170, 100), Color.Black);
        }

        public override void ButtonReset()
        {

        }

        #region placement Phase Code
        private void Placement()
        {
            if (tanks.Texture == tanks.Pressed)
            {
                AddEntity("tank");
            }
            else if (mines.Texture == mines.Pressed)
            {
                AddEntity("mine");
            }
        }
        private void getSpawnRegion()
        {
            int spawnRows = Convert.ToInt32(curBoard.Rows * .1F);
            if (spawnRows == 0) { spawnRows = 1; }

            boardState.playerList[1].spawnRows = curBoard.getSubGrid(new Vector2(0, 0), new Vector2(spawnRows, curBoard.Columns));
            boardState.playerList[0].spawnRows = curBoard.getSubGrid(new Vector2(curBoard.Rows - spawnRows, 0), new Vector2(spawnRows, curBoard.Columns));
        }
        #region Add objects code
        private void AddTankPressed(object sender, EventArgs e)
        {
            if (mines.Texture == mines.Pressed)
            {
                mines.toggleTexture();
            }
        }
        private void AddMinePressed(object sender, EventArgs e)
        {
            if (tanks.Texture == tanks.Pressed)
            {
                tanks.toggleTexture();
            }
        }
        private void AddEntity(string type)
        {
            Entity entity;
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
                    if (type == "tank")
                    {
                        tempTank = new Tank(curGrid, curGridLocation);
                        entity = tempTank;

                        //to satisfy bs of needed to be declared in some way
                        tempMine = new Mine(curGrid, curGridLocation);
                    }
                    else if (type == "mine")
                    {
                        tempMine = new Mine(curGrid, curGridLocation);
                        entity = tempMine;

                        //to satisfy bs of needed to be declared in some way
                        tempTank = new Tank(curGrid, curGridLocation);
                        //
                        //
                    }
                    else
                    {
                        entity = new Entity(curGrid, curGridLocation);
                        tempMine = new Mine(curGrid, curGridLocation);
                        tempTank = new Tank(curGrid, curGridLocation);
                    }
                    if (type == "tank" || type == "mine")
                    {
                        //check if the type of object has some count left //Tanks Check (also checks for mouse in spawn)
                        if (((type == "tank" && Convert.ToInt16(tanksCount.Text) > 0) && boardState.playerList[boardState.curPlayerNum].spawn.Contains(worldPosition))
                            //mines check
                            || (type == "mine" && Convert.ToInt16(minesCount.Text) > 0))
                        {
                            //if the grid has nothing there then add the object
                            if (!boardState.gridLocations.Contains(curGridLocation))
                            {
                                entity.LoadContent();
                                boardState.entities.Add(entity);
                                boardState.gridLocations.Add(entity.gridLocation);
                                //remove one from the count
                                if (type == "tank")
                                {
                                    tanksUsed++;
                                    tempTank.LoadContent();
                                    boardState.playerList[boardState.curPlayerNum].tanks.Add(new Tank(curGrid, curGridLocation));
                                }
                                else if (type == "mine")
                                {
                                    /*foreach (Player player in curBoardState.playerList)
                                    {
                                        if (!player.spawn.Contains(worldPosition))
                                        {

                                        }
                                    }*/
                                    minesUsed++;
                                    tempMine.LoadContent();
                                    boardState.playerList[boardState.curPlayerNum].mines.Add(tempMine);
                                }
                            }
                        }
                    }
                }
                //if mouse is right clicked once in the board (remove code)
                else if (mouse.RightButton == ButtonState.Pressed && oldRightClick != curRightClick)
                {
                    //of the gridlocations tracker has an object there
                    if (boardState.gridLocations.Contains(curGridLocation))
                    {
                        //check which entity is there 
                        for (int i = 0; i < boardState.entities.Count; i++)
                        {
                            if (boardState.entities[i].gridLocation == curGridLocation)
                            {
                                //if we are working with tanks
                                if (type == "tank")
                                {
                                    //if its a tank
                                    if (boardState.entities[i].Type == "tank")
                                    {
                                        for (int j = 0; j < boardState.playerList[boardState.curPlayerNum].tanks.Count; j++)
                                        {
                                            //find which tank is the current one based on gridlocation to remove it
                                            if (boardState.playerList[boardState.curPlayerNum].tanks[j].gridLocation == curGridLocation)
                                            {
                                                boardState.playerList[boardState.curPlayerNum].tanks.Remove(boardState.playerList[boardState.curPlayerNum].tanks[j]);
                                                //remove its records
                                                boardState.entities.Remove(boardState.entities[i]);
                                                boardState.gridLocations.Remove(curGridLocation);
                                                tanksUsed--;
                                            }
                                        }
                                    }
                                }
                                //if we are working with mines
                                else if (type == "mine")
                                {
                                    //if its a mine
                                    if (boardState.entities[i].Type == "mine")
                                    {

                                        for (int j = 0; j < boardState.playerList[boardState.curPlayerNum].mines.Count; j++)
                                        {
                                            //find which mine based on gridlocation and remove it
                                            if (boardState.playerList[boardState.curPlayerNum].mines[j].gridLocation == curGridLocation)
                                            {
                                                boardState.playerList[boardState.curPlayerNum].mines.Remove(boardState.playerList[boardState.curPlayerNum].mines[j]);
                                                //remove its records
                                                boardState.entities.Remove(boardState.entities[i]);
                                                boardState.gridLocations.Remove(curGridLocation);
                                                minesUsed--;
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

        protected void ReadyPressed(object sender, EventArgs e)
        {
            //check to see if the player is actaully done placing thier stuff when they hit ready
            if (tanksCount.Text != "0" || minesCount.Text != "0")
            {
                //if they are done issue a warning (set bool to true for draw)
                placementWarning = true;
            }
            else
            {
                //make number of players 0 based. If the current player isnt the last player move to next
                if (boardState.curPlayerNum < numOfPlayers - 1)
                {
                    //load the new tanks
                    foreach (Tank tank in boardState.playerList[boardState.curPlayerNum].tanks)
                    {
                        tank.LoadContent();
                    }
                    boardState.curPlayerNum++; //make it the next players placement turn
                    //reset the tanks and mines to 0 used
                    tanksUsed = 0;
                    minesUsed = 0;
                    //remove warning for other player
                    placementWarning = false;

                    //remove the gridlocations of the other players mines to make the mines able to be stacked. Removing advatage of later players placing mines
                    for (int i = 0; i < boardState.playerList.Count; i++)
                    {
                        if (i != boardState.curPlayerNum)
                        {
                            foreach (Mine mine in boardState.playerList[i].mines)
                            {
                                boardState.gridLocations.Remove(mine.gridLocation);
                            }
                        }
                    }
                }
                else //if it is the last player set it to the first player (which is 0 in a 0 base)
                {
                    //load the new tanks for the last player
                    foreach (Tank tank in boardState.playerList[boardState.curPlayerNum].tanks)
                    {
                        tank.LoadContent();
                    }
                    boardState.curPlayerNum = 0;
                    placementStage = false; //this starts the next stage
                    battleStarted = true; //this starts the next stage
                    placementWarning = false; //remove warning 
                    previousBoardState = BoardState.SavePreviousBoardState(boardState);
                }

                UpdatePathFinderWithMines();
                foreach (Button b in placementButtonList)
                {
                    b.ButtonReset();
                }
            }
        }
        #endregion

        #region battlestarted button code
        private void UndoPressed(object sender, EventArgs e)
        {
            //go back to the last turnstatesaved
            SetTurnState();
        }
        protected void SweepPressed(object sender, EventArgs e)
        {
        }
        #endregion

    }
}

