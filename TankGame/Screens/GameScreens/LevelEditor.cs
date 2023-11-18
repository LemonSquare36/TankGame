using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.GameInfo;
using System.Runtime.CompilerServices;

namespace TankGame
{
    internal class LevelEditor : ScreenManager
    {
        #region Declares (there are alot)
        //for words
        SpriteFont font;
        //generic buttons
        Button Load, Save, New, Delete, Back, ArrowRight, ArrowLeft;
        Button SetRowCol, SetTankCount, SetMineCount, SetSweepCount;
        //add object buttons
        Button addWall, addItem, erase, addSpawn;
        List<Button> PageOneButtons = new List<Button>();
        List<Button> PageTwoButtons = new List<Button>();
        //level loading logic
        bool levelLoaded = false, spawnWarning = false;
        string file;
        //objects selected logic
        bool wallSelected = false, itemSelected = false, eraseSelected = false, spawnSelected = false;
        //input box
        InputBox nameField, sizeField, tankField, mineField, sweepField;
        List<InputBox> PageOneFields = new List<InputBox>();
        //List Box
        ListBox levelSelection;
        //colors for text
        Color rowColColor, tankColor, MineColor, sweepColor;
        //selectors
        List<Selector> SelectorList = new List<Selector>();
        Selector playerCount, selectedPlayer;

        int activePage = 1;
        List<List<SpawnTile>> playerSpawns = new List<List<SpawnTile>>();
        #endregion

        //Initialize
        public override void Initialize()
        {
            base.Initialize();
            //reset all bools to correct starts
            wallSelected = false;
            itemSelected = false; 
            eraseSelected = false;
            spawnSelected = false;
            levelLoaded = false;
            spawnWarning = false;

            //create the text field
            #region initializing textboxes
            nameField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1420, 500), new Vector2(300, 50));
            sizeField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1090, 100), new Vector2(80, 70), 2);
            tankField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1290, 100), new Vector2(80, 70), 2);
            mineField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1490, 100), new Vector2(80, 70), 2);
            sweepField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1690, 100), new Vector2(80, 70), 2);


            levelSelection = new ListBox(new Vector2(1100, 570), new Vector2(740, 450), 11, Color.White, Color.Black, Color.DarkGray, 4);
            playerCount = new Selector(new Vector2(1265, 100), new Vector2(100, 100), "PlusMinus", 8, 2, Color.Black);
            selectedPlayer = new Selector(new Vector2(1565, 100), new Vector2(100, 100), "Arrows", 2, 1, Color.Black);
            #endregion

            #region default font colors
            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
            sweepColor = Color.Black;
            #endregion
        }
        //LoadContent
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            #region load Textures
            //font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            #endregion

            #region load buttons
            //page 1 
            Load = new Button(new Vector2(1130, 440), 100, 50, "Buttons/Editor/Load", "load");
            Save = new Button(new Vector2(1280, 440), 100, 50, "Buttons/Editor/Save", "save");
            New = new Button(new Vector2(1430, 440), 100, 50, "Buttons/Editor/New", "new");
            Delete = new Button(new Vector2(1580, 440), 100, 50, "Buttons/Editor/Delete", "delete");
            Back = new Button(new Vector2(1730, 440), 100, 50, "Buttons/Editor/Back", "back", 0);
            ArrowRight = new Button(new Vector2(1690, 300), 100, 50, "Buttons/Editor/ArrowRight", "arrowright");


            SetRowCol = new Button(new Vector2(1180, 110), 70, 50, "Buttons/Editor/Set", "setrowcol");
            SetTankCount = new Button(new Vector2(1380, 110), 70, 50, "Buttons/Editor/Set", "settankcount");
            SetMineCount = new Button(new Vector2(1580, 110), 70, 50, "Buttons/Editor/Set", "setminecount");
            SetSweepCount = new Button(new Vector2(1780, 110), 70, 50, "Buttons/Editor/Set", "setsweepcount");

            addWall = new Button(new Vector2(1290, 300), 50, 50, "Buttons/Editor/Wall", "addWall", "toggle");
            addItem = new Button(new Vector2(1440, 300), 50, 50, "Buttons/Editor/ItemBox", "addItem", "toggle");
            erase = new Button(new Vector2(1590, 300), 50, 50, "Buttons/Editor/Clear", "erase", "toggle");

            //page 2
            ArrowLeft = new Button(new Vector2(1190, 300), 100, 50, "Buttons/Editor/ArrowLeft", "arrowleft");

            addSpawn = new Button(new Vector2(1290, 300), 100, 50, "Buttons/Editor/SpawnButton", "addspawn", "toggle");
            #endregion

            #region Button Events
            //page 1
            Load.ButtonClicked += LoadPressed;
            Save.ButtonClicked += SavePressed;
            New.ButtonClicked += NewPressed;
            Delete.ButtonClicked += DeletePressed;
            Back.ButtonClicked += ScreenChangeEvent;
            ArrowRight.ButtonClicked += ArrowRightPressed;

            SetRowCol.ButtonClicked += SetRowColPressed;
            SetTankCount.ButtonClicked += SetTanksPressed;
            SetMineCount.ButtonClicked += SetMinesPressed;
            SetSweepCount.ButtonClicked += SetSweepsPressed;

            addWall.ButtonClicked += SelectWall;
            addItem.ButtonClicked += SelectItem;
            erase.ButtonClicked += SelectErase;

            //page 2
            ArrowLeft.ButtonClicked += ArrowLeftPressed;

            addSpawn.ButtonClicked += SelectSpawn;
            #endregion

            #region ButtonList
            //clear the list on load so the buttons to stack up
            PageOneButtons.Clear();
            PageOneButtons.Add(Load);
            PageOneButtons.Add(Save);
            PageOneButtons.Add(New);
            PageOneButtons.Add(Delete);
            PageOneButtons.Add(Back);
            PageOneButtons.Add(addWall);
            PageOneButtons.Add(addItem);
            PageOneButtons.Add(erase);
            PageOneButtons.Add(SetRowCol);
            PageOneButtons.Add(SetTankCount);
            PageOneButtons.Add(SetMineCount);
            PageOneButtons.Add(SetSweepCount);
            PageOneButtons.Add(ArrowRight);

            //clear the list on load so the buttons to stack up
            PageTwoButtons.Clear();
            PageTwoButtons.Add(ArrowLeft);
            PageTwoButtons.Add(addSpawn);
            PageTwoButtons.Add(erase);
            #endregion

            #region input box list
            //clear the list on load so the buttons to stack up
            PageOneFields.Clear();
            PageOneFields.Add(nameField);
            PageOneFields.Add(sizeField);
            PageOneFields.Add(tankField);
            PageOneFields.Add(mineField);
            PageOneFields.Add(sweepField);
            #endregion

            #region selector list
            //clear the list on load so the buttons to stack up
            SelectorList.Clear();
            SelectorList.Add(playerCount);
            SelectorList.Add(selectedPlayer);

            playerCount.ValueChanged += SelectorListSizeChange;
            #endregion

            foreach (InputBox box in PageOneFields)
            {
                box.LoadContent();
            }
            //load the listBox for level selection
            LevelListLoad();
            foreach (Selector selector in SelectorList)
            {
                selector.LoadContent();
            }
            //create a new board on entry
            NewPressed();

        }
        //Update
        public override void Update()
        {
            base.Update();
            //update buttons
            if (activePage == 1)
            {
                foreach (Button b in PageOneButtons)
                {
                    b.Update(mouse, worldPosition);
                }

                //update to all the text boxes
                foreach (InputBox box in PageOneFields)
                {
                    box.Update(mouse, worldPosition, keyState, keyHeldState);
                }

                levelSelection.Update(mouse, worldPosition);


                //check for changes in the text boxes
                //if changed but not set, change the color to red to indicated unset changes
                #region color checks
                try
                {
                    if (Convert.ToInt16(sizeField.Text) != RowsCol)
                    {
                        rowColColor = Color.Red;
                    }
                    else { rowColColor = Color.Black; }
                    if (Convert.ToInt16(tankField.Text) != TanksAndMines.X)
                    {
                        tankColor = Color.Red;
                    }
                    else { tankColor = Color.Black; }
                    if (Convert.ToInt16(mineField.Text) != TanksAndMines.Y)
                    {
                        MineColor = Color.Red;
                    }
                    else { MineColor = Color.Black; }
                    if (Convert.ToInt16(sweepField.Text) != sweeps)
                    {
                        sweepColor = Color.Red;
                    }
                    else { MineColor = Color.Black; }
                }
                catch { }
                #endregion


                //if the level is loaded
                if (levelLoaded)
                {
                    //if the board is loaded, check if the mouse is inside it or not
                    getMouseInBoard();
                    //the tools on page 1
                    if (wallSelected)
                    {
                        AddWall();
                    }
                    else if (itemSelected)
                    {
                        AddItem();
                    }
                    else if (eraseSelected)
                    {
                        EraserTool();
                    }
                    if (mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        EraserTool();
                    }
                }
            }
            else if (activePage == 2)
            {
                foreach (Button b in PageTwoButtons)
                {
                    b.Update(mouse, worldPosition);
                }
                foreach (Selector selector in SelectorList)
                {
                    selector.Update(mouse, worldPosition);
                }
                if (levelLoaded)
                {
                    //if the board is loaded, check if the mouse is inside it or not
                    getMouseInBoard();
                    //the tools on page 2
                    if (eraseSelected)
                    {
                        EraserTool();
                    }
                    else if (spawnSelected)
                    {
                        AddSpawn();
                    }
                    if (mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        EraserTool();
                    }
                }
            }

            //always make sure that you cant select a player past the maxium players allowed
            selectedPlayer.max = playerCount.Value;
            if (selectedPlayer.Value > selectedPlayer.max)
            { selectedPlayer.Value = selectedPlayer.max; }
        }
        //Draw
        public override void Draw()
        {

            if (levelLoaded)
            {
                //board draw 
                curBoard.drawCheckers(spriteBatch);
                curBoard.DrawOutline(spriteBatch);
                //draw all the entities accept the spawntiles
                foreach (Entity e in boardState.entities)
                {
                    if (e.Type != "spawn")
                        e.Draw(spriteBatch);
                }
                //for the spawntiles, draw the tiles of the currently selected player brighter than the unselected players
                for (int i = 0; i < playerSpawns.Count; i++)
                {
                    if ((i + 1) == selectedPlayer.Value)
                    {
                        foreach (SpawnTile tile in playerSpawns[i])
                        {
                            tile.Draw(spriteBatch, Color.Green);
                        }
                    }
                    else
                    {
                        foreach (SpawnTile tile in playerSpawns[i])
                        {
                            tile.Draw(spriteBatch);
                        }
                    }
                }
            }

            #region PageOne Draw

            if (activePage == 1)
            {
                //draw buttons
                foreach (Button b in PageOneButtons)
                {
                    b.Draw(spriteBatch);
                }
                //input box list draw
                foreach (InputBox box in PageOneFields)
                {
                    box.Draw(spriteBatch);
                }
                levelSelection.Draw(spriteBatch);


                #region drawing text to screen
                //spriteBatch.DrawString(font, "Add an Object ", new Vector2(1100, 100), Color.Black);
                spriteBatch.DrawString(font, "Right Click to Erase", new Vector2(1300, 350), Color.Black);
                spriteBatch.DrawString(font, "Walls", new Vector2(1270, 250), Color.Black);
                spriteBatch.DrawString(font, "ItemBoxes", new Vector2(1382, 250), Color.Black);
                spriteBatch.DrawString(font, "Eraser", new Vector2(1570, 250), Color.Black);
                spriteBatch.DrawString(font, "Level Name: ", new Vector2(1200, 505), Color.Black);
                spriteBatch.DrawString(font, "Size", new Vector2(1095, 50), rowColColor);
                spriteBatch.DrawString(font, "# of Tanks", new Vector2(1240, 50), tankColor);
                spriteBatch.DrawString(font, "# of Mines", new Vector2(1440, 50), MineColor);
                spriteBatch.DrawString(font, "# of Sweeps", new Vector2(1640, 50), sweepColor);
                spriteBatch.DrawString(font, "Red Text = Change Not Set", new Vector2(1250, 180), Color.Red);
                #endregion
            }
            #endregion
            else if (activePage == 2)
            {
                foreach (Button b in PageTwoButtons)
                {
                    b.Draw(spriteBatch);
                }

                foreach (Selector selector in SelectorList)
                {
                    selector.Draw(spriteBatch, font);
                }
                #region draw text to screen page 2

                spriteBatch.DrawString(font, "Player Count", new Vector2(1205, 50), Color.Black);
                spriteBatch.DrawString(font, "Selected Player", new Vector2(1493, 50), Color.Black);
                spriteBatch.DrawString(font, "Eraser", new Vector2(1570, 250), Color.Black);

                #endregion
            }
            if (spawnWarning)
            {
                spriteBatch.DrawString(font, "You  need  more  spawn  tiles!", new Vector2(65, 350), Color.Red, 0, Vector2.Zero, 1.95F, SpriteEffects.None, 0);
            }
        }

        #region Adding objects functions
        //if walls are selected this code allows the placing of them. 
        private void AddWall()
        {
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is clicked once inside the board
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    Wall newWall = new Wall(curGrid, curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(newWall.gridLocation))
                    {
                        for (int i = 0; i < boardState.entities.Count; i++)
                        {
                            if (boardState.entities[i].gridLocation == newWall.gridLocation)
                            {
                                if (boardState.entities[i].Type == "spawn")
                                {
                                    for (int j = 0; j < playerSpawns.Count; j++)
                                    {
                                        playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                    }
                                }
                                boardState.entities.Remove(boardState.entities[i]);
                                newWall.LoadContent();
                                boardState.entities.Add(newWall);
                            }
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newWall.LoadContent();
                        boardState.entities.Add(newWall);
                        boardState.gridLocations.Add(newWall.gridLocation);
                    }
                }
            }
        }
        //if items are selected this code will allow them to be added to the board
        private void AddItem()
        {
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is clicked once inside the board
                if (curLeftClick == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldLeftClick == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    ItemBox newItem = new ItemBox(curGrid, curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(newItem.gridLocation))
                    {
                        for (int i = 0; i < boardState.entities.Count; i++)
                        {
                            if (boardState.entities[i].gridLocation == newItem.gridLocation)
                            {
                                if (boardState.entities[i].Type == "spawn")
                                {
                                    for (int j = 0; j < playerSpawns.Count; j++)
                                    {
                                        playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                    }
                                }
                                boardState.entities.Remove(boardState.entities[i]);
                                newItem.LoadContent();
                                boardState.entities.Add(newItem);
                            }
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newItem.LoadContent();
                        boardState.entities.Add(newItem);
                        boardState.gridLocations.Add(newItem.gridLocation);
                    }
                }
            }
        }
        //if erase is selected this code allows the removing of objects. 
        private void EraserTool()
        {
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is clicked once inside the board
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(curGridLocation))
                    {
                        for (int i = 0; i < boardState.entities.Count; i++)
                        {
                            if (boardState.entities[i].gridLocation == curGridLocation)
                            {
                                if (boardState.entities[i].Type == "spawn")
                                {
                                    for (int j = 0; j < playerSpawns.Count; j++)
                                    {
                                        playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                    }
                                }
                                boardState.entities.Remove(boardState.entities[i]);
                                boardState.gridLocations.Remove(curGridLocation);
                            }
                        }
                    }
                }
            }
        }
        private void AddSpawn()
        {
            if (mouseInBoard)
            {
                //if the mouse is clicked once inside the board
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    spawnWarning = false;
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    SpawnTile newSpawn = new SpawnTile(curGrid, curGridLocation);
                    if (!boardState.gridLocations.Contains(newSpawn.gridLocation))
                    {
                        newSpawn.LoadContent();
                        boardState.entities.Add(newSpawn);
                        boardState.gridLocations.Add(newSpawn.gridLocation);

                        playerSpawns[selectedPlayer.Value - 1].Add(newSpawn);
                    }
                }
            }
        }
        #endregion


        #region Load and Save and New and Delete
        //Events for saving and loading
        private void LoadPressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\LevelFiles\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\LevelFiles\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.028F, 0.05F);
                    //grab the informatin from the levelManager
                    boardState = new BoardState(levelManager.getEntities(), levelManager.getWalls(), levelManager.getItemBoxes());

                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    sweeps = levelManager.getSweeps();
                    //finish loading the board
                    curBoard.LoadContent();
                    boardState.LoadEntities();
                    boardState.getGridLocations();

                    nameField.Text = Path.GetFileName(file).Split(".")[0];
                    //level can be drawn and updated now. New thread can be made
                    levelLoaded = true;
                    RowsCol = curBoard.Rows;
                    sizeField.Text = Convert.ToString(RowsCol);
                    tankField.Text = Convert.ToString(TanksAndMines.X);
                    mineField.Text = Convert.ToString(TanksAndMines.Y);
                    sweepField.Text = Convert.ToString(sweeps);
                    playerCount.Value = levelManager.getPlayerCount();
                    selectedPlayer.Value = 1;

                    playerSpawns = levelManager.getPlayerSpawns();

                    rowColColor = Color.Black;
                    tankColor = Color.Black;
                    MineColor = Color.Black;
                }
                catch { NewPressed(); }
            }
            else { NewPressed(); }
        }
        private void SavePressed(object sender, EventArgs e)
        {
            bool canSave = true;
            if (levelLoaded)
            {
                if (nameField.Text == "")
                {
                    nameField.Text = "Needs a file name";
                    canSave = false;
                }

                for (int i = 0; i < playerSpawns.Count; i++)
                {
                    if (playerSpawns[i].Count < TanksAndMines.X)
                    {
                        canSave = false;
                        spawnWarning = true;
                    }
                }
                if (canSave)
                {
                    string fileName = nameField.Text.Replace(" ", "");
                    file = relativePath + "\\TankGame\\LevelFiles\\" + fileName + ".lvl";
                    levelManager.SaveLevel(file, fileName, curBoard, boardState.entities, playerSpawns, TanksAndMines, sweeps, playerCount.Value);
                }
            }
            //load the listBox for level selection
            LevelListLoad();
        }

        //creates a fresh new board
        private void NewPressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\";
            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.X * .028F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
            curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), 20, 20, 8);
            boardState = new BoardState(new(), new(), new());
            curBoard.LoadContent();
            curBoard.setColor(new Color(235, 235, 235), new Color(200, 200, 200), Color.Black);
            nameField.Text = "New";
            levelLoaded = true;
            sizeField.Text = "20";
            tankField.Text = "3";
            mineField.Text = "3";
            sweepField.Text = "3";
            RowsCol = 20;
            TanksAndMines = new Point(3, 3);
            sweeps = 3;
            playerCount.Value = 2;
            selectedPlayer.Value = 1;
            //player spawn lists cleared and the 2 for the 2 players
            playerSpawns.Clear();
            playerSpawns.Add(new()); playerSpawns.Add(new());

            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
            sweepColor = Color.Black;
        }
        private void NewPressed()
        {
            file = relativePath + "\\TankGame\\";
            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
            curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), 20, 20, 8);
            boardState = new BoardState(new(), new(), new());
            curBoard.LoadContent();
            curBoard.setColor(new Color(235, 235, 235), new Color(200, 200, 200), Color.Black);
            nameField.Text = "New";
            levelLoaded = true;
            sizeField.Text = "20";
            tankField.Text = "3";
            mineField.Text = "3";
            sweepField.Text = "3";
            RowsCol = 20;
            TanksAndMines = new Point(3, 3);
            sweeps = 3;
            playerCount.Value = 2;
            selectedPlayer.Value = 1;
            //player spawn lists cleared and the 2 for the 2 players
            playerSpawns.Clear();
            playerSpawns.Add(new()); playerSpawns.Add(new());

            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
            sweepColor = Color.Black;
        }

        private void DeletePressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\LevelFiles\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\LevelFiles\\" + "" + ".lvl")
            {
                File.Delete(file);
                LevelListLoad();
            }
        }
        #endregion


        #region Add Objects Events
        //all 3 events
        //set thier button to be the selected one and unselect the others
        //toggle thier highlighted texture one and toggle the rest off
        private void SelectWall(object sender, EventArgs e)
        {
            spawnWarning = false;
            if (!wallSelected)
            {
                wallSelected = true;
            }
            else
            {
                wallSelected = false;
            }
            itemSelected = false;
            eraseSelected = false;
            spawnSelected = false;

            if (addItem.Texture == addItem.Pressed)
            {
                addItem.toggleTexture();
            }
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }
            if (addSpawn.Texture == addSpawn.Pressed)
            {
                addSpawn.toggleTexture();
            }
        }
        private void SelectItem(object sender, EventArgs e)
        {
            spawnWarning = false;
            if (!itemSelected)
            {
                itemSelected = true;
            }
            else
            {
                itemSelected = false;
            }
            wallSelected = false;
            eraseSelected = false;
            spawnSelected = false;

            if (addWall.Texture == addWall.Pressed)
            {
                addWall.toggleTexture();
            }
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }
            if (addSpawn.Texture == addSpawn.Pressed)
            {
                addSpawn.toggleTexture();
            }
        }
        private void SelectErase(object sender, EventArgs e)
        {
            spawnWarning = false;
            if (!eraseSelected)
            {
                eraseSelected = true;
            }
            else
            {
                eraseSelected = false;
            }
            itemSelected = false;
            wallSelected = false;
            spawnSelected = false;

            if (addItem.Texture == addItem.Pressed)
            {
                addItem.toggleTexture();
            }
            if (addWall.Texture == addWall.Pressed)
            {
                addWall.toggleTexture();
            }
            if (addSpawn.Texture == addSpawn.Pressed)
            {
                addSpawn.toggleTexture();
            }
        }
        private void SelectSpawn(object sender, EventArgs e)
        {
            spawnWarning = false;
            if (!spawnSelected)
            {
                spawnSelected = true;
            }
            else
            {
                spawnSelected = false;
            }
            itemSelected = false;
            wallSelected = false;
            eraseSelected = false;

            if (addItem.Texture == addItem.Pressed)
            {
                addItem.toggleTexture();
            }
            if (addWall.Texture == addWall.Pressed)
            {
                addWall.toggleTexture();
            }
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }
        }
        #endregion


        #region other events
        //button events for setting the fields
        private void SetRowColPressed(object sender, EventArgs e)
        {
            //get the number from the field
            try
            {
                RowsCol = Convert.ToInt16(sizeField.Text);
                //limit of board size of 40x40
                if (RowsCol > 40)
                {
                    //set actaul value, then displayed value
                    RowsCol = 40;
                    sizeField.Text = "40";
                }
                if (RowsCol < 40)
                {
                    //set actaul value, then displayed value
                    RowsCol = 10;
                    sizeField.Text = "10";
                }
                float size = Camera.ViewboxScale.Y * 0.9F;
                Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
                curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), RowsCol, RowsCol, 8);
                curBoard.LoadContent();
                curBoard.setColor(new Color(235, 235, 235), new Color(200, 200, 200), Color.Black);
                //if there are entities to check
                if (boardState != null)
                {
                    for (int i = 0; i < boardState.entities.Count; i++)
                    {
                        //see if the current entitie is within the new grid size before initializing it
                        if (boardState.entities[i].gridLocation.X < RowsCol && boardState.entities[i].gridLocation.Y < RowsCol)
                        {
                            boardState.entities[i].Initialize(curBoard.getGridSquare(boardState.entities[i].gridLocation.X, boardState.entities[i].gridLocation.Y));
                        }
                        else //if it isnt in the grid then remove it from exisitance
                        {
                            boardState.gridLocations.Remove(boardState.entities[i].gridLocation);
                            boardState.entities.Remove(boardState.entities[i]);
                            i--;
                        }
                    }
                }
            }
            //if not a number make it a numbe but dont scale anything
            catch { sizeField.Text = Convert.ToString(RowsCol); }
            rowColColor = Color.Black;
        }
        private void SetTanksPressed(object sender, EventArgs e)
        {
            try
            {
                TanksAndMines.X = Convert.ToInt16(tankField.Text);
                //limit of 8 on the tanks
                if (TanksAndMines.X > 8)
                {
                    //set actaul value, then displayed value
                    TanksAndMines.X = 8;
                    tankField.Text = "8";
                }
                if (TanksAndMines.X < 1)
                {
                    //set actaul value, then displayed value
                    TanksAndMines.X = 1;
                    tankField.Text = "1";
                }
                tankColor = Color.Black;
            }
            catch { }
        }
        private void SetMinesPressed(object sender, EventArgs e)
        {
            try
            {
                TanksAndMines.Y = Convert.ToInt16(mineField.Text);
                //limit of 12 on the mines
                if (TanksAndMines.Y > 12)
                {
                    //set actaul value, then displayed value
                    TanksAndMines.Y = 12;
                    mineField.Text = "12";
                }
                if (TanksAndMines.Y < 0)
                {
                    //set actaul value, then displayed value
                    TanksAndMines.Y = 0;
                    mineField.Text = "0";
                }
                MineColor = Color.Black;
            }
            catch { }
        }
        private void SetSweepsPressed(object sender, EventArgs e)
        {
            try
            {
                sweeps = Convert.ToInt16(sweepField.Text);
                sweepColor = Color.Black;
                if (sweeps > 10)
                {
                    sweepField.Text = "10";
                    sweeps = 10;
                }
                if (sweeps < 0)
                {
                    sweepField.Text = "0";
                    sweeps = 0;
                }
            }
            catch { }
        }
        private void SelectorListSizeChange(object sender, EventArgs e)
        {                
            //check if the player count and spawns list count are the same (prevent more spawn zones existing than players
            while (true)
            {
                //if the spawn zones outnumber players
                if (playerCount.Value < playerSpawns.Count)
                {
                    //before removeing the spawnlist, make sure all the spawns are removed from the editors needed lists
                    foreach(SpawnTile tile in playerSpawns[playerSpawns.Count - 1])
                    {
                        Entity entityToRemove = boardState.entities.Find(x => x.gridLocation == tile.gridLocation);
                        boardState.entities.Remove(entityToRemove);

                        Point gridLocationToRemove = boardState.gridLocations.Find(x => x == tile.gridLocation);
                        boardState.gridLocations.Remove(gridLocationToRemove);
                    }
                    //remove a spawnlist until they are equal
                    playerSpawns.Remove(playerSpawns[playerSpawns.Count - 1]);
                }
                //if the players outnumber the spawnzones
                else if (playerCount.Value > playerSpawns.Count)
                {
                    //make a new empty zone for adding spawn tiles
                    playerSpawns.Add(new());
                }
                if (playerCount.Value == playerSpawns.Count)
                {
                    break;
                }
            }
        }
        #endregion
        #region pageChanges
        private void ArrowRightPressed(object sender, EventArgs e)
        {
            activePage = 2;
        }
        private void ArrowLeftPressed(object sender, EventArgs e)
        {
            activePage = 1;
        }
        #endregion

        private void LevelListLoad()
        {
            //gets all the files in the relative folder and sends them as an array to the listbox to populate the levels
            if (!Directory.Exists(relativePath + "\\TankGame\\LevelFiles"))
            {
                Directory.CreateDirectory(relativePath + "\\TankGame\\LevelFiles");
            }
            string[] filepaths = Directory.GetFiles(relativePath + "\\TankGame\\LevelFiles");
            for (int i = 0; i < filepaths.Length; i++)
            {
                filepaths[i] = Path.GetFileName(filepaths[i].Split(".")[0]);
            }
            levelSelection.LoadContent(filepaths);
        }

        public override void ButtonReset()
        {
            //resets everybutton to prevent unwanted button clicks
            foreach (Button b in PageOneButtons)
            {
                b.ButtonReset();
            }
        }
    }
}
