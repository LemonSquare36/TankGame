using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        Button mines, ready, endturn, undo, sweep;
        Button regTank, sniperTank, scoutTank, heavyTank;
        List<Button> placementButtonList = new List<Button>();
        List<Button> battleButtonList = new List<Button>();
        List<Button> inventoryButtonList = new List<Button>();

        int tanksUsed = 0, minesUsed = 0;


        public override void Initialize()
        {
            base.Initialize();
            placementStage = true;
            battleStarted = false;
            //activePlayer = 1;
            //create input boxes for displaying tanks and mine count
            tanksCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1550, 610), new Vector2(80, 70), 0);
            minesCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1750, 610), new Vector2(80, 70), 0);

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
            LoadLevelFile();

            //font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            //textures
            UI_bg = Main.GameContent.Load<Texture2D>("Backgrounds/Battlescreen/UI_BG");
            UI_filler = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            spawnTex = Main.GameContent.Load<Texture2D>("GameSprites/spawnHighLight");

            //SendLoadToPeer();

            #region load buttons
            //load tank buttons when allowed
            foreach (string allowed in rules.allowedTanks)
            {
                switch (allowed)
                {
                    case "Regular":
                        regTank = new Button(new Vector2(1520, 550), 50, 50, "GameSprites/BattleSprites/Tanks/Tank", "Regular", "toggle");
                        regTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Sniper":
                        sniperTank = new Button(new Vector2(1620, 550), 50, 50, "GameSprites/BattleSprites/Tanks/SniperTank", "Sniper", "toggle");
                        sniperTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Scout":
                        scoutTank = new Button(new Vector2(1720, 550), 50, 50, "GameSprites/BattleSprites/Tanks/ScoutTank", "Scout", "toggle");
                        scoutTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Heavy":
                        heavyTank = new Button(new Vector2(1820, 490), 50, 50, "GameSprites/BattleSprites/Tanks/HeavyTank", "Heavy", "toggle");
                        heavyTank.ChangeButtonColor(Color.White);
                        break;
                }
            }
            //load tank buttons when not allowed
            foreach (string notAllowed in rules.notAllowedTanks)
            {
                switch (notAllowed)
                {
                    case "Regular":
                        regTank = new Button(new Vector2(1520, 550), 50, 50, "GameSprites/BattleSprites/Tanks/TankDead", "Regular", "toggleOneTex");
                        regTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Sniper":
                        sniperTank = new Button(new Vector2(1620, 550), 50, 50, "GameSprites/BattleSprites/Tanks/TankDead", "Sniper", "toggleOneTex");
                        sniperTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Scout":
                        scoutTank = new Button(new Vector2(1720, 550), 50, 50, "GameSprites/BattleSprites/Tanks/TankDead", "Scout", "toggleOneTex");
                        scoutTank.ChangeButtonColor(Color.Gray);
                        break;
                    case "Heavy":
                        heavyTank = new Button(new Vector2(1820, 490), 50, 50, "GameSprites/BattleSprites/Tanks/TankDead", "Heavy", "toggleOneTex");
                        heavyTank.ChangeButtonColor(Color.Gray);
                        break;
                }
            }
            //load other placement code buttons
            mines = new Button(new Vector2(1800, 550), 50, 50, "GameSprites/BattleSprites/Mine", "mines", "toggle");
            ready = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/Ready", "ready");
            endturn = new Button(new Vector2(1590, 800), 200, 100, "Buttons/BattleScreen/EndTurn", "endturn");
            undo = new Button(new Vector2(1590, 600), 100, 50, "Buttons/BattleScreen/Undo", "undo");
            sweep = new Button(new Vector2(110, 100), 50, 50, "Buttons/BattleScreen/Sweep", "sweeper", "toggle");


            #endregion
            #region input box load
            //load inputboxes
            tanksCount.LoadContent();
            minesCount.LoadContent();
            //populate the information to show how many tanks and mines they get to place
            tanksCount.Text = Convert.ToString(rules.tankPoints);
            minesCount.Text = Convert.ToString(rules.numOfMines);
            #endregion

            #region list adds
            //palcement stage buttons
            placementButtonList.Add(regTank);
            placementButtonList.Add(sniperTank);
            placementButtonList.Add(scoutTank);
            placementButtonList.Add(heavyTank);
            placementButtonList.Add(mines);
            placementButtonList.Add(ready);
            //battle stage buttons
            battleButtonList.Add(endturn);
            battleButtonList.Add(undo);
            //inventory buttons
            inventoryButtonList.Add(sweep);
            #endregion

            #region ButtonNoises Load
            AssignButtonNoise(placementButtonList, "Sounds/click");
            AssignButtonNoise(battleButtonList, "Sounds/click");
            AssignButtonNoise(inventoryButtonList, "Sounds/click");
            #endregion

            #region Event listeners
            regTank.ButtonClicked += AddTankPressed;
            sniperTank.ButtonClicked += AddTankPressed;
            scoutTank.ButtonClicked += AddTankPressed;
            heavyTank.ButtonClicked += AddTankPressed;
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
                tanksCount.Text = Convert.ToString(rules.tankPoints - tanksUsed);
                minesCount.Text = Convert.ToString(rules.numOfMines - minesUsed);

                //buttons in the placement stage
                foreach (Button b in placementButtonList)
                {
                    b.Update(mouse, worldPosition);
                    if (b.ButtonActive)
                    {
                        anyObjectActive = true;
                    }
                }
                //code for placing and picking up tanks
                Placement();
            }
            //update code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {
                boardState.getTankLocations();
                //non inventoy button update
                foreach (Button b in battleButtonList)
                {
                    b.Update(mouse, worldPosition);
                    //look for active buttons to see if there is an active object
                    if (b.ButtonActive)
                    {
                        anyObjectActive = true;
                    }
                }
                //update for inventory buttons
                foreach (Button b in inventoryButtonList)
                {
                    b.Update(mouse, worldPosition);
                    //look for active buttons to see if there is an active object
                    if (b.ButtonActive)
                    {
                        anyObjectActive = true;
                    }
                    //if the button name == selected item name then it is that items buttons
                    if (b.bName == selectedItem)
                    {
                        //if the button is no longer active, then that item is deselected
                        if (!b.ButtonActive)
                        {
                            itemActive = false;
                        }
                    }
                }
                //check if the mouse is in the board
                if (mouseInBoard)
                {
                    checkSelectedTank();
                }
                else if (!mouseInBoard)
                {
                    path.Clear();
                }
                MoveOrShoot();
                //item use code
                UseItem(selectedItem);
                //if the selected item is empty then reset the button responsible and make itemActive false
                if (boardState.playerList[boardState.curPlayerNum].inventory.getSelectedItemsCount(selectedItem) <= 0 && selectedItem != null)
                {
                    try
                    {
                        inventoryButtonList.Find(x => x.bName == selectedItem).ButtonReset();
                        itemActive = false;
                        selectedItem = null;
                    }
                    catch { }
                }
            }
            //unselect the button(s) that are currently active
            if (escapePressed && anyObjectActive)
            {
                ButtonReset();
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

            //draw walls
            DrawWalls();

            //draw itemboxes
            foreach (ItemBox itembox in boardState.itemBoxes)
            {
                itembox.Draw(spriteBatch);
            }
            //draw holes
            foreach (Hole hole in boardState.holes)
            {
                hole.Draw(spriteBatch);
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

                //give a warning the player pressed ready when they had tanks and mines left
                if (placementWarning)
                {
                    spriteBatch.DrawString(font, "You still have Tanks and\n    Mines left to place", new Vector2(1485, 100), Color.DarkRed);
                }
                //draw the tank spawn region highlight for visual indication of where tanks can go
                for (int i = 0; i < levelManager.getPlayerSpawns().Count; i++)
                {
                    foreach (SpawnTile spawntile in levelManager.getPlayerSpawns()[i])
                    {
                        if (i == boardState.curPlayerNum)
                        {
                            spriteBatch.Draw(spawnTex, spawntile.curSquare.Location, null, Color.Green, 0, Vector2.Zero, spawntile.curSquare.Size, SpriteEffects.None, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(spawnTex, spawntile.curSquare.Location, null, Color.LightGray, 0, Vector2.Zero, spawntile.curSquare.Size, SpriteEffects.None, 0);
                        }
                    }
                }
            }

            //draw code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {
                //draw buttons
                foreach (Button b in battleButtonList)
                {
                    b.Draw(spriteBatch);
                }
                foreach (Button b in inventoryButtonList)
                {
                    b.Draw(spriteBatch);
                }
                if (DrawTankInfo)
                {
                    spriteBatch.DrawString(font, "Current AP: " + boardState.playerList[boardState.curPlayerNum].tanks[activeTankNum].AP, new Vector2(1600, 350), Color.Black);
                    foreach (RectangleF rF in CircleTiles)
                    {
                        if (!rF.Null)
                        {
                            if (rF.identifier != 1) //not marked as a tank or wall (those are done differently)
                            {
                                //draw green to show range
                                spriteBatch.Draw(spawnTex, rF.Location, null, Color.LightGreen, 0, Vector2.Zero, rF.Size, SpriteEffects.None, 0);
                            }
                        }
                    }
                    DrawPath(spriteBatch, font);
                }
                if (mouseInBoard)
                {
                    DrawItemUI(selectedItem, spawnTex, font, Color.Red, 2500);
                }
                PlayItemAnimations(spriteBatch);
                //UI info
                spriteBatch.DrawString(font, "Items", new Vector2(150, 30), Color.Black);
                spriteBatch.DrawString(font, "______", new Vector2(130, 35), Color.Black);
                spriteBatch.DrawString(font, Convert.ToString(boardState.playerList[boardState.curPlayerNum].inventory.sweeps), new Vector2(170, 100), Color.Black);
            }

            //draw current players turn info
            spriteBatch.DrawString(font, "Current Player: " + (boardState.curPlayerNum + 1), new Vector2(1550, 250), Color.Black);
        }

        public override void ButtonReset()
        {
            foreach (Button b in placementButtonList)
            {
                b.ButtonReset();
            }
            foreach (Button b in battleButtonList)
            {
                b.ButtonReset();
            }
            foreach (Button b in inventoryButtonList)
            {
                b.ButtonReset();
            }
        }

        #region placement Phase Code
        private void Placement()
        {
            if (regTank.Texture == regTank.Pressed)
            {
                AddEntity("regTank", tanksCount, minesCount, ref minesUsed, ref tanksUsed);
            }
            if (sniperTank.Texture == sniperTank.Pressed)
            {
                AddEntity("sniperTank", tanksCount, minesCount, ref minesUsed, ref tanksUsed);
            }
            if (scoutTank.Texture == scoutTank.Pressed)
            {
                AddEntity("scoutTank", tanksCount, minesCount, ref minesUsed, ref tanksUsed);
            }
            if (heavyTank.Texture == heavyTank.Pressed)
            {
                AddEntity("heavyTank", tanksCount, minesCount, ref minesUsed, ref tanksUsed);
            }
            else if (mines.Texture == mines.Pressed)
            {
                AddEntity("mine", tanksCount, minesCount, ref minesUsed, ref tanksUsed);
            }
            RemoveTankOrMine(ref tanksUsed, ref minesUsed);
        }

        #region Add objects code
        private void AddTankPressed(object sender, EventArgs e)
        {
            //reset all buttons not the current one in the placement list of buttons
            foreach (Button button in placementButtonList)
            {
                if (button.bName != ((Button)sender).bName)
                {
                    button.ButtonReset();
                }
            }
            //if its not in allowed list, dont let the button be pressable
            if (!rules.allowedTanks.Contains(((Button)sender).bName))
            {
                ((Button)sender).ButtonReset();
            }
        }
        private void AddMinePressed(object sender, EventArgs e)
        {
            //reset all buttons not the current one in the placement list of buttons
            foreach (Button button in placementButtonList)
            {
                if (button.bName != ((Button)sender).bName)
                {
                    button.ButtonReset();
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
                if (boardState.curPlayerNum < rules.numOfPlayers - 1)
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
                    previousBoardState = BoardState.SavePreviousBoardState(boardState, curBoard);
                }

                UpdatePathFinderWithMines(boardState, pathfinder);
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
            if (boardState.playerList[boardState.curPlayerNum].inventory.sweeps <= 0)
            {
                itemActive = false;
                sweep.ButtonReset();
            }
            itemActive = true;
            selectedItem = "sweeper";
        }
        #endregion

    }
}

