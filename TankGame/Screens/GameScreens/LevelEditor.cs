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
using System.Linq;
using TankGame.Objects.Entities.Items;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    internal class LevelEditor : ScreenManager
    {
        #region Declares (there are alot)
        //for words
        SpriteFont font;
        //generic buttons
        Button Load, Save, New, Delete, Back, ArrowRight, ArrowLeft;
        Button SetRowCol;
        //add object buttons
        Button addWall, addItem, erase, addSpawn, addHole;
        List<Button> PageOneButtons = new List<Button>();
        List<Button> PageTwoButtons = new List<Button>();
        //level loading logic
        bool levelLoaded = false, spawnWarning = false;
        string file;
        //objects selected logic
        bool wallSelected = false, itemSelected = false, eraseSelected = false, spawnSelected = false, holeSelected = false;
        bool multiWallEnabled = false;
        bool destroyableWallEnabled = true;
        List<Point> wallDrawnGridLocations = new();
        //input box
        InputBox nameField, sizeField;
        List<InputBox> PageOneFields = new List<InputBox>();
        //List Box
        ListBox levelSelection;
        //colors for text
        Color rowTextColColor;
        Color color1, color2;
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
            holeSelected = false;
            levelLoaded = false;
            spawnWarning = false;

            //boardColors
            color1 = new Color(252, 235, 213);
            color2 = new Color(218, 196, 162);

            //create the text field
            #region initializing textboxes
            nameField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1420, 500), new Vector2(300, 50));
            sizeField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1090, 100), new Vector2(80, 70), 2);


            levelSelection = new ListBox(new Vector2(1100, 570), new Vector2(740, 450), 11, Color.White, Color.Black, Color.DarkGray, 4);
            playerCount = new Selector(new Vector2(1265, 100), new Vector2(100, 100), "PlusMinus", 8, 2, Color.Black);
            selectedPlayer = new Selector(new Vector2(1565, 100), new Vector2(100, 100), "Arrows", 2, 1, Color.Black);
            #endregion

            #region default font colors
            rowTextColColor = Color.Black;
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

            addWall = new Button(new Vector2(1290, 300), 50, 50, "Buttons/Editor/Wall", "addWall", "toggle");
            addItem = new Button(new Vector2(1440, 300), 50, 50, "Buttons/Editor/ItemBox", "addItem", "toggle");
            erase = new Button(new Vector2(1590, 300), 50, 50, "Buttons/Editor/Clear", "erase", "toggle");
            addHole = new Button(new Vector2(1140, 300), 50, 50, "Buttons/Editor/Hole", "hole", "toggle");

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

            addWall.ButtonClicked += SelectWall;
            addItem.ButtonClicked += SelectItem;
            erase.ButtonClicked += SelectErase;
            addHole.ButtonClicked += SelectHole;

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
            PageOneButtons.Add(addHole);
            PageOneButtons.Add(SetRowCol);
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

            sizeField.Activated += RowColPressed;
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

            //hotkeys and shortcuts
            keybinds();
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

                levelSelection.Update(mouse, worldPosition, keyState, keyHeldState);


                //check for changes in the text boxes
                //if changed but not set, change the color to red to indicated unset changes
                #region color checks
                try
                {
                    if (Convert.ToInt16(sizeField.Text) != RowsCol)
                    {
                        rowTextColColor = Color.Red;
                    }
                    else { rowTextColColor = Color.Black; }
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
                    else if(holeSelected)
                    {
                        AddHole();
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
                spriteBatch.DrawString(font, "Size", new Vector2(1095, 50), rowTextColColor);
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
                if (curLeftClick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    Wall newWall = new Wall(curGrid, curGridLocation, destroyableWallEnabled);

                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(newWall.gridLocation))
                    {
                        bool removed = false;
                        //if its a itembox there
                        if (boardState.itemBoxes.Any(x => x.gridLocation == newWall.gridLocation))
                        {
                            //remove that item box
                            boardState.itemBoxes.Remove(boardState.itemBoxes.Find(x => x.gridLocation == newWall.gridLocation));
                            removed = true;
                        }
                        //if its a hole there
                        else if (boardState.holes.Any(x => x.gridLocation == newWall.gridLocation))
                        {
                            //remove that hole
                            boardState.holes.Remove(boardState.holes.Find(x => x.gridLocation == newWall.gridLocation));
                            removed = true;
                        }
                        else
                        {
                            //check the spawns
                            for (int j = 0; j < playerSpawns.Count; j++)
                            {
                                //if it is a spawn
                                if (playerSpawns[j].Any(x => x.gridLocation == newWall.gridLocation))
                                {
                                    //remove it
                                    playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                    removed = true;
                                }
                            }
                        }
                        if (removed)
                        {
                            //add the wall but leave the gridlocations list alone
                            newWall.LoadContent();
                            boardState.walls.Add(newWall);
                            if (!wallDrawnGridLocations.Contains(curGridLocation))
                            {
                                wallDrawnGridLocations.Add(curGridLocation);
                            }
                        }

                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newWall.LoadContent();
                        boardState.walls.Add(newWall);
                        boardState.gridLocations.Add(newWall.gridLocation);
                        if (!wallDrawnGridLocations.Contains(curGridLocation))
                        {
                            wallDrawnGridLocations.Add(curGridLocation);
                        }
                    }
                }
                else
                {
                    if (curLeftClick != Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldLeftClick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {

                    }
                    if (wallDrawnGridLocations.Count > 1 && multiWallEnabled)
                    {
                        int startCount = boardState.walls.Count;
                        for (int i = 0; i < wallDrawnGridLocations.Count; i++)
                        {
                            //for each wall that was drawn together, remove the walls that were most recently added
                            boardState.walls.Remove(boardState.walls[startCount - i - 1]);
                        }
                        //then create a new multiWall
                        Wall multiWall = new Wall(wallDrawnGridLocations, curBoard, destroyableWallEnabled);
                        //find the multiwalls array
                        Point offset = new Point();
                        RectangleF[,] tempArray = multiWall.MultiWallArray(curBoard, out offset);
                        //check over the array to find what parts of the multiwall are connected
                        List<Point> connectPoints = new();
                        List<Point> toCheck = new();
                        bool hasNieghbor = false;
                        for (int i = 0; i < tempArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < tempArray.GetLength(1); j++)
                            {
                                //if its a wall spot
                                if (tempArray[i, j].identifier == 1)
                                {
                                    //add it to toCheck to see if it has neighbors
                                    toCheck.Add(new Point(i, j));
                                }
                                //wall was chosen to check further
                                while (toCheck.Count > 0)
                                {
                                    //see if this wall has neighbors other than ones already checked for neighbors (only neighbors in 4 cardinal directions/no diagonal)
                                    for (int k = 0; k < toCheck.Count; k++)
                                    {
                                        int X = toCheck[k].X, Y = toCheck[k].Y;
                                        //mark its been looked at
                                        tempArray[X, Y].identifier = 2;
                                        connectPoints.Add(toCheck[k]);
                                        #region neighborChecks
                                        if (X > 0)
                                        {
                                            //check its neighbors for walls spots
                                            if (tempArray[X - 1, Y].identifier == 1)
                                            {
                                                //if its a nieghbor then set both to 2 for connected to a block
                                                tempArray[X - 1, Y].identifier = 2;
                                                //add it to the list of blocks to check
                                                toCheck.Add(new Point(X - 1, Y));
                                                hasNieghbor = true;
                                            }
                                        }
                                        if (Y > 0)
                                        {
                                            if (tempArray[X, Y - 1].identifier == 1)
                                            {
                                                //if its a nieghbor then set both to 2 for connected to a block
                                                tempArray[X, Y - 1].identifier = 2;
                                                //add it to the list of blocks to check
                                                toCheck.Add(new Point(X, Y - 1));
                                                hasNieghbor = true;
                                            }
                                        }
                                        if (X < tempArray.GetUpperBound(0))
                                        {
                                            if (tempArray[X + 1, Y].identifier == 1)
                                            {
                                                //if its a nieghbor then set both to 2 for connected to a block
                                                tempArray[X + 1, Y].identifier = 2;
                                                //add it to the list of blocks to check
                                                toCheck.Add(new Point(X + 1, Y));
                                                hasNieghbor = true;
                                            }
                                        }
                                        if (Y < tempArray.GetUpperBound(1))
                                        {
                                            if (tempArray[X, Y + 1].identifier == 1)
                                            {
                                                //if its a nieghbor then set both to 2 for connected to a block
                                                tempArray[X, Y + 1].identifier = 2;
                                                //add it to the list of blocks to check
                                                toCheck.Add(new Point(X, Y + 1));
                                                hasNieghbor = true;
                                            }
                                        }
                                        #endregion
                                        //remove the wall that just got neighbor checked
                                        toCheck.Remove(toCheck[k]);
                                    }
                                }
                                if (toCheck.Count == 0)
                                {
                                    //apply offset to get them back to original board conditions
                                    for (int k = 0; k < connectPoints.Count; k++)
                                    {
                                        connectPoints[k] += offset;
                                    }
                                }
                                //had no neighbors
                                if (connectPoints.Count == 1 && toCheck.Count == 0)
                                {
                                    RectangleF rf = curBoard.getGridSquare(connectPoints[0].X, connectPoints[0].Y);
                                    Wall tempWall = new Wall(rf, connectPoints[0], destroyableWallEnabled);
                                    tempWall.LoadContent();
                                    boardState.walls.Add(tempWall);
                                    connectPoints.Clear();
                                    hasNieghbor = false;
                                }
                                //all eligible walls were looked at and has neighbors
                                if (hasNieghbor && toCheck.Count == 0)
                                {
                                    multiWall = new Wall(connectPoints, curBoard, destroyableWallEnabled);
                                    multiWall.LoadContent();
                                    boardState.walls.Add(multiWall);
                                    connectPoints.Clear();
                                    hasNieghbor = false;
                                }
                            }
                        }
                        //(gridlocations were already added when making the temp walls)
                    }
                    wallDrawnGridLocations.Clear();
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
                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(newItem.gridLocation))
                    {
                        bool removed = false;
                        //if its a hole there
                        if (boardState.holes.Any(x => x.gridLocation == newItem.gridLocation))
                        {
                            //remove that hole
                            boardState.holes.Remove(boardState.holes.Find(x => x.gridLocation == newItem.gridLocation));
                            removed = true;
                        }
                        if (!removed)
                        {
                            //check walls list
                            for (int i = 0; i < boardState.walls.Count; i++)
                            {
                                //for each multiwall
                                if (boardState.walls[i].multiWall)
                                {
                                    //if the multiwall has the gridlocation in it
                                    if (boardState.walls[i].gridLocations.Contains(curGridLocation))
                                    {
                                        //remove all the gridlocations of that wall from boardstates gridlocations, but readd the one we are using currently
                                        foreach (Point point in boardState.walls[i].gridLocations)
                                        {
                                            boardState.gridLocations.Remove(point);
                                        }
                                        boardState.gridLocations.Add(curGridLocation);
                                        boardState.walls.Remove(boardState.walls[i]);
                                        removed = true;
                                    }
                                }
                                else
                                {
                                    if (boardState.walls[i].gridLocation == curGridLocation)
                                    {
                                        //remove that wall
                                        boardState.walls.Remove(boardState.walls.Find(x => x.gridLocation == curGridLocation));
                                        removed = true;
                                    }
                                }
                            }
                        }
                        if (!removed)
                        {
                            //check the spawns
                            for (int j = 0; j < playerSpawns.Count; j++)
                            {
                                //if it is a spawn
                                if (playerSpawns[j].Any(x => x.gridLocation == newItem.gridLocation))
                                {
                                    //remove it
                                    playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                }
                            }
                        }
                        if (removed)
                        {
                            //add the item but leave the gridlocations list alone
                            newItem.LoadContent();
                            boardState.itemBoxes.Add(newItem);
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newItem.LoadContent();
                        boardState.itemBoxes.Add(newItem);
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
                        //check walls list
                        for (int i = 0; i < boardState.walls.Count; i++)
                        {
                            //for each multiwall
                            if (boardState.walls[i].multiWall)
                            {
                                //if the multiwall has the gridlocation in it
                                if (boardState.walls[i].gridLocations.Contains(curGridLocation))
                                {
                                    //remove all the gridlocations of that wall from boardstates gridlocations, but readd the one we are using currently
                                    foreach (Point point in boardState.walls[i].gridLocations)
                                    {
                                        boardState.gridLocations.Remove(point);
                                    }
                                    boardState.walls.Remove(boardState.walls[i]);

                                }
                            }
                            else
                            {
                                if (boardState.walls[i].gridLocation == curGridLocation)
                                {
                                    //remove that wall
                                    boardState.walls.Remove(boardState.walls.Find(x => x.gridLocation == curGridLocation));
                                    boardState.gridLocations.Remove(curGridLocation);
                                }
                            }
                        }
                        //if its a hole there
                        if (boardState.holes.Any(x => x.gridLocation == curGridLocation))
                        {
                            //remove that hole
                            boardState.holes.Remove(boardState.holes.Find(x => x.gridLocation == curGridLocation));
                            boardState.gridLocations.Remove(curGridLocation);
                        }
                        //if its a itembox there
                        else if (boardState.itemBoxes.Any(x => x.gridLocation == curGridLocation))
                        {
                            //remove that item box
                            boardState.itemBoxes.Remove(boardState.itemBoxes.Find(x => x.gridLocation == curGridLocation));
                            boardState.gridLocations.Remove(curGridLocation);
                        }
                        else
                        {
                            //check the spawns
                            for (int j = 0; j < playerSpawns.Count; j++)
                            {
                                //if it is a spawn
                                if (playerSpawns[j].Any(x => x.gridLocation == curGridLocation))
                                {
                                    //remove it
                                    playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                    boardState.gridLocations.Remove(curGridLocation);
                                }
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
                        boardState.gridLocations.Add(newSpawn.gridLocation);

                        playerSpawns[selectedPlayer.Value - 1].Add(newSpawn);
                    }
                }
            }
        }
        private void AddHole()
        {
            //find out if the mouse is inside the board
            if (mouseInBoard)
            {
                //if the mouse is clicked once inside the board
                if (curLeftClick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    Hole newHole = new Hole(curGrid, curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    //if the grid has been used before then remove the current object there and add the new one
                    if (boardState.gridLocations.Contains(newHole.gridLocation))
                    {
                        bool removed = false;
                        //itembox check
                        if (boardState.itemBoxes.Any(x => x.gridLocation == curGridLocation))
                        {
                            //remove that item box
                            boardState.itemBoxes.Remove(boardState.itemBoxes.Find(x => x.gridLocation == curGridLocation));
                            boardState.gridLocations.Remove(curGridLocation);
                            removed = true;
                        }
                        if (!removed)
                        {
                            //check walls list
                            for (int i = 0; i < boardState.walls.Count; i++)
                            {
                                //for each multiwall
                                if (boardState.walls[i].multiWall)
                                {
                                    //if the multiwall has the gridlocation in it
                                    if (boardState.walls[i].gridLocations.Contains(curGridLocation))
                                    {
                                        //remove all the gridlocations of that wall from boardstates gridlocations, but readd the one we are using currently
                                        foreach (Point point in boardState.walls[i].gridLocations)
                                        {
                                            boardState.gridLocations.Remove(point);
                                        }
                                        boardState.gridLocations.Add(curGridLocation);
                                        boardState.walls.Remove(boardState.walls[i]);
                                        removed = true;
                                    }
                                }
                                else
                                {
                                    if (boardState.walls[i].gridLocation == curGridLocation)
                                    {
                                        //remove that wall
                                        boardState.walls.Remove(boardState.walls.Find(x => x.gridLocation == curGridLocation));
                                        removed = true;
                                    }
                                }
                            }
                        }
                        if (!removed)
                        {
                            //check the spawns
                            for (int j = 0; j < playerSpawns.Count; j++)
                            {
                                //if it is a spawn
                                if (playerSpawns[j].Any(x => x.gridLocation == newHole.gridLocation))
                                {
                                    //remove it
                                    playerSpawns[j].Remove(playerSpawns[j].Find(spawn => spawn.gridLocation == curGridLocation));
                                }
                            }
                        }
                        if (removed)
                        {
                            //add the item but leave the gridlocations list alone
                            newHole.LoadContent();
                            boardState.holes.Add(newHole);
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newHole.LoadContent();
                        boardState.holes.Add(newHole);
                        boardState.gridLocations.Add(newHole.gridLocation);
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
                    boardState = new BoardState(levelManager.getWalls(), levelManager.getItemBoxes(), levelManager.getHoles());

                    curBoard = levelManager.getGameBoard();
                    //finish loading the board
                    curBoard.LoadContent();
                    boardState.LoadEntities();
                    boardState.getGridLocations();

                    nameField.Text = Path.GetFileName(file).Split(".")[0];
                    //level can be drawn and updated now. New thread can be made
                    levelLoaded = true;
                    RowsCol = curBoard.Rows;
                    sizeField.Text = Convert.ToString(RowsCol);
                    playerCount.Value = levelManager.getPlayerCount();
                    selectedPlayer.Value = 1;

                    playerSpawns = levelManager.getPlayerSpawns();
                    for (int i = 0; i < playerSpawns.Count; i++)
                    {
                        foreach (SpawnTile tile in playerSpawns[i])
                        {
                            tile.LoadContent();
                            boardState.gridLocations.Add(tile.gridLocation);
                        }
                    }
                    rowTextColColor = Color.Black;
                }
                catch { NewPressed(); }
            }
            else { NewPressed(); }
            levelSelection.UnselectButtons();
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
                    if (playerSpawns[i].Count < rules.tankPoints / 5)
                    {
                        canSave = false;
                        spawnWarning = true;
                    }
                }
                if (canSave)
                {
                    string fileName = nameField.Text.Replace(" ", "");
                    file = relativePath + "\\TankGame\\LevelFiles\\" + fileName + ".lvl";
                    levelManager.SaveLevel(file, fileName, curBoard, boardState.walls, boardState.itemBoxes, boardState.holes, playerSpawns, playerCount.Value);
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
            boardState = new BoardState(new(), new(),new());
            curBoard.LoadContent();
            curBoard.setColor(color1, color2, Color.Black);
            nameField.Text = "New";
            levelLoaded = true;
            sizeField.Text = "20";
            RowsCol = 20;
            playerCount.Value = 2;
            selectedPlayer.Value = 1;
            //player spawn lists cleared and the 2 for the 2 players
            playerSpawns.Clear();
            playerSpawns.Add(new()); playerSpawns.Add(new());

            rowTextColColor = Color.Black;

            levelSelection.UnselectButtons();
        }
        private void NewPressed()
        {
            file = relativePath + "\\TankGame\\";
            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
            curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), 20, 20, 8);
            boardState = new BoardState(new(), new(), new());
            curBoard.LoadContent();
            curBoard.setColor(color1, color2, Color.Black);
            nameField.Text = "New";
            levelLoaded = true;
            sizeField.Text = "20";
            RowsCol = 20;
            playerCount.Value = 2;
            selectedPlayer.Value = 1;
            //player spawn lists cleared and the 2 for the 2 players
            playerSpawns.Clear();
            playerSpawns.Add(new()); playerSpawns.Add(new());

            rowTextColColor = Color.Black;

            levelSelection.UnselectButtons();
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
            holeSelected = false;

            if (addHole.Texture == addHole.Pressed)
            {
                addHole.toggleTexture();
            }
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
            holeSelected = false;

            if (addHole.Texture == addHole.Pressed)
            {
                addHole.toggleTexture();
            }
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
            holeSelected = false;

            if (addHole.Texture == addHole.Pressed)
            {
                addHole.toggleTexture();
            }
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
        private void SelectHole(object sender, EventArgs e)
        {
            spawnWarning = false;
            if (!holeSelected)
            {
                holeSelected = true;
            }
            else
            {
                holeSelected = false;
            }
            itemSelected = false;
            wallSelected = false;
            spawnSelected = false;
            eraseSelected = false;

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
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
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
            holeSelected = false;

            if (addHole.Texture == addHole.Pressed)
            {
                addHole.toggleTexture();
            }
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
                if (RowsCol < 10)
                {
                    //set actaul value, then displayed value
                    RowsCol = 10;
                    sizeField.Text = "10";
                }
                float size = Camera.ViewboxScale.Y * 0.9F;
                Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
                curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), RowsCol, RowsCol, 8);
                curBoard.LoadContent();
                curBoard.setColor(color1, color2, Color.Black);
                //if there are entities to check
                if (boardState != null)
                {
                    for (int i = 0; i < boardState.gridLocations.Count; i++)
                    {
                        //if it is in the board then we will reinitialize the item for the new grid
                        if (boardState.gridLocations[i].X < RowsCol && boardState.gridLocations[i].Y < RowsCol)
                        {
                            bool init = false;
                            //walls
                            foreach (Wall wall in boardState.walls)
                            {                                
                                if (wall.multiWall)
                                {
                                    foreach (Point point in wall.gridLocations)
                                    {
                                        if (point == boardState.gridLocations[i])
                                        {
                                            wall.Initialize(curBoard);
                                            init = true;
                                            break;
                                        }
                                    }                                   
                                }
                                else
                                {
                                    if (wall.gridLocation == boardState.gridLocations[i])
                                    {
                                        wall.Initialize(curBoard.getGridSquare(boardState.gridLocations[i].X, boardState.gridLocations[i].Y));
                                        init = true;
                                    }
                                }
                                if (init)
                                {
                                    break;
                                }
                            }
                            //itemboxes
                            foreach (ItemBox item in boardState.itemBoxes)
                            {
                                if (item.gridLocation == boardState.gridLocations[i])
                                item.Initialize(curBoard.getGridSquare(boardState.gridLocations[i].X, boardState.gridLocations[i].Y));
                            }
                            //holes
                            foreach (Hole hole in boardState.holes)
                            {
                                if (hole.gridLocation == boardState.gridLocations[i])
                                    hole.Initialize(curBoard.getGridSquare(boardState.gridLocations[i].X, boardState.gridLocations[i].Y));
                            }
                            //spawntiles
                            for (int j = 0; j < playerSpawns.Count; j++)
                            {
                                foreach (SpawnTile spawn in playerSpawns[j])
                                {
                                    if (spawn.gridLocation == boardState.gridLocations[i])
                                        spawn.Initialize(curBoard.getGridSquare(boardState.gridLocations[i].X, boardState.gridLocations[i].Y));
                                }
                            }
                        }
                        //other wise it needs to be removed from the boardstate
                        else
                        {
                            //check walls
                            for (int j = 0; j < boardState.walls.Count; j++)
                            {
                                if (!boardState.walls[j].multiWall)
                                {
                                    if (boardState.walls[j].gridLocation == boardState.gridLocations[i])
                                    {
                                        boardState.walls.Remove(boardState.walls[j]);
                                        j--;
                                    }
                                }
                                else
                                {
                                    foreach(Point point in boardState.walls[j].gridLocations)
                                    {
                                        if (point == boardState.gridLocations[i])
                                        {
                                            boardState.walls.Remove(boardState.walls[j]);
                                            j--;
                                            break;
                                        }
                                    }
                                }
                            }
                            //check itemboxes
                            if (boardState.itemBoxes.Any(x => x.gridLocation == boardState.gridLocations[i]))
                            {
                                boardState.itemBoxes.Remove(boardState.itemBoxes.First(x => x.gridLocation == boardState.gridLocations[i]));
                            }
                            //check holes
                            if (boardState.holes.Any(x => x.gridLocation == boardState.gridLocations[i]))
                            {
                                boardState.holes.Remove(boardState.holes.First(x => x.gridLocation == boardState.gridLocations[i]));
                            }
                            //check spawntiles
                            else
                            {
                                for (int j = 0; j < playerSpawns.Count; j++)
                                {
                                    if (playerSpawns[j].Any(x => x.gridLocation == boardState.gridLocations[i]))
                                    {
                                        playerSpawns[j].Remove(playerSpawns[j].First(x => x.gridLocation == boardState.gridLocations[i]));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //if not a number make it a numbe but dont scale anything
            catch { sizeField.Text = Convert.ToString(RowsCol); }
            rowTextColColor = Color.Black;
            boardState.getGridLocations();
            for (int i = 0; i < playerSpawns.Count; i++)
            {
                foreach (SpawnTile tile in playerSpawns[i])
                {
                    boardState.gridLocations.Add(tile.gridLocation);
                }
            }
        }
        private void RowColPressed(object sender, EventArgs e)
        {
            sizeField.Text = "";
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
                    foreach (SpawnTile tile in playerSpawns[playerSpawns.Count - 1])
                    {
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
        private void keybinds()
        {
            if (keyState.IsKeyDown(Keys.W) && !keyHeldState.IsKeyDown(Keys.W))
            {
                if (!multiWallEnabled)
                    multiWallEnabled = true;
                else
                    multiWallEnabled = false;
            }
            if (keyState.IsKeyDown(Keys.D) && !keyHeldState.IsKeyDown(Keys.D))
            {
                if (!destroyableWallEnabled)
                    destroyableWallEnabled = true;
                else
                    destroyableWallEnabled = false;
            }
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
