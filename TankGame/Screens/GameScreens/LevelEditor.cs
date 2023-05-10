using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class LevelEditor : ScreenManager
    {
        #region Declares (there are alot)
        //for words
        SpriteFont font;
        //generic buttons
        Button Load, Save, New, Delete, Back;
        Button SetRowCol, SetTankCount, SetMineCount, SetSweepCount;
        //add object buttons
        Button addWall, addItem, erase;
        List<Button> Buttons = new List<Button>();
        //level loading logic
        bool levelLoaded = false;
        string file;
        //objects selected logic
        bool wallSelected = false, itemSelected = false, eraseSelected = false;
        //input box
        InputBox nameField, sizeField, tankField, mineField, sweepField;
        List<InputBox> Fields = new List<InputBox>();
        //List Box
        ListBox levelSelection;
        //colors for text
        Color rowColColor, tankColor, MineColor, sweepColor;
        #endregion

        //Initialize
        public override void Initialize()
        {
            base.Initialize();

            //create the text field
            #region initializing textboxes
            nameField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1420, 500), new Vector2(300, 50));
            sizeField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1090, 100), new Vector2(80, 70), 2);
            tankField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1290, 100), new Vector2(80, 70), 2);
            mineField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1490, 100), new Vector2(80, 70), 2);
            sweepField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1690, 100), new Vector2(80, 70), 2);


            levelSelection = new ListBox(new Vector2(1100, 570), new Vector2(740, 450), 11, Color.White, Color.Black,Color.DarkGray, 4);
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
            Load = new Button(new Vector2(1130, 440), 100, 50, "Buttons/Editor/Load", "load");
            Save = new Button(new Vector2(1280, 440), 100, 50, "Buttons/Editor/Save", "save");
            New = new Button(new Vector2(1430, 440), 100, 50, "Buttons/Editor/New", "new");
            Delete = new Button(new Vector2(1580, 440), 100, 50, "Buttons/Editor/Delete", "delete");
            Back = new Button(new Vector2(1730, 440), 100, 50, "Buttons/Editor/Back", "back", 0);

            SetRowCol = new Button(new Vector2(1180, 110), 70, 50, "Buttons/Editor/Set", "setrowcol");
            SetTankCount = new Button(new Vector2(1380, 110), 70, 50, "Buttons/Editor/Set", "settankcount");
            SetMineCount = new Button(new Vector2(1580, 110), 70, 50, "Buttons/Editor/Set", "setminecount");
            SetSweepCount = new Button(new Vector2(1780, 110), 70, 50, "Buttons/Editor/Set", "setsweepcount");

            addWall = new Button(new Vector2(1290, 300), 50, 50, "Buttons/Editor/Wall", "addWall", "toggle");
            addItem = new Button(new Vector2(1440, 300), 50, 50, "Buttons/Editor/ItemBox", "addItem", "toggle");
            erase = new Button(new Vector2(1590, 300), 50, 50, "Buttons/Editor/Clear", "erase", "toggle");
            #endregion

            #region Button Events
            Load.ButtonClicked += LoadPressed;
            Save.ButtonClicked += SavePressed;
            New.ButtonClicked += NewPressed;
            Delete.ButtonClicked += DeletePressed;
            Back.ButtonClicked += ScreenChangeEvent;

            SetRowCol.ButtonClicked += SetRowColPressed;
            SetTankCount.ButtonClicked += SetTanksPressed;
            SetMineCount.ButtonClicked += SetMinesPressed;
            SetSweepCount.ButtonClicked += SetSweepsPressed;

            addWall.ButtonClicked += SelectWall;
            addItem.ButtonClicked += SelectItem;
            erase.ButtonClicked += SelectErase;
            #endregion

            #region ButtonList
            Buttons.Clear();
            Buttons.Add(Load);
            Buttons.Add(Save);
            Buttons.Add(New);
            Buttons.Add(Delete);
            Buttons.Add(Back);
            Buttons.Add(addWall);
            Buttons.Add(addItem);
            Buttons.Add(erase);
            Buttons.Add(SetRowCol);
            Buttons.Add(SetTankCount);
            Buttons.Add(SetMineCount);
            Buttons.Add(SetSweepCount);

            #endregion

            #region input box list
            Fields.Add(nameField);
            Fields.Add(sizeField);
            Fields.Add(tankField);
            Fields.Add(mineField);
            Fields.Add(sweepField);
            #endregion

            foreach (InputBox box in Fields)
            {
                box.LoadContent();
            }
            //load the listBox for level selection
            LevelListLoad();


        }
        //Update
        public override void Update()
        {
            base.Update();
            //update buttons
            foreach (Button b in Buttons)
            {
                b.Update(mouse, worldPosition);
            }

            //update to all the text boxes
            foreach (InputBox box in Fields)
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
                //if walls are selected this code allows for the addition of walls
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
        //Draw
        public override void Draw()
        {

            if (levelLoaded)
            {
                //board draw 
                curBoard.drawCheckers(spriteBatch);
                curBoard.DrawOutline(spriteBatch);
                foreach (Entity e in entities)
                {
                    e.Draw(spriteBatch);
                }
            }

            //regular draw
            //draw buttons
            foreach (Button b in Buttons)
            {
                b.Draw(spriteBatch);
            }
            //input box list draw
            foreach (InputBox box in Fields)
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

        #region Adding objects functions
        //if walls are selected this code allows the placing of them. 
        private void AddWall()
        {
            //find out if the mouse is inside the board
            if (new RectangleF(curBoard.getInnerRectangle().Location, curBoard.getInnerRectangle().Size).Contains(worldPosition))
            {
                //if the mouse is clicked once inside the board
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    Wall newWall = new Wall(curGrid, curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    if (gridLocations.Contains(newWall.gridLocation))
                    {
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i].gridLocation == newWall.gridLocation)
                            {
                                entities.Remove(entities[i]);
                                newWall.LoadContent();
                                entities.Add(newWall);
                            }
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newWall.LoadContent();
                        entities.Add(newWall);
                        gridLocations.Add(newWall.gridLocation);
                    }
                }
            }
        }
        //if items are selected this code will allow them to be added to the board
        private void AddItem()
        {
            //find out if the mouse is inside the board
            if (new RectangleF(curBoard.getInnerRectangle().Location, curBoard.getInnerRectangle().Size).Contains(worldPosition))
            {
                oldClick = curClick;
                curClick = mouse.LeftButton;
                //if the mouse is clicked once inside the board
                if (curClick == Microsoft.Xna.Framework.Input.ButtonState.Pressed && oldClick == Microsoft.Xna.Framework.Input.ButtonState.Released)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);
                    ItemBox newItem = new ItemBox(curGrid, curGridLocation);
                    //if the grid has been used before then remove the current object there and add the new one
                    if (gridLocations.Contains(newItem.gridLocation))
                    {
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i].gridLocation == newItem.gridLocation)
                            {
                                entities.Remove(entities[i]);
                                newItem.LoadContent();
                                entities.Add(newItem);
                            }
                        }
                    }
                    //otherwise just add the object. Also tell gridlocations that that spot is now used
                    else
                    {
                        newItem.LoadContent();
                        entities.Add(newItem);
                        gridLocations.Add(newItem.gridLocation);
                    }
                }
            }
        }
        //if erase is selected this code allows the removing of objects. 
        private void EraserTool()
        {
            //find out if the mouse is inside the board
            if (new RectangleF(curBoard.getInnerRectangle().Location, curBoard.getInnerRectangle().Size).Contains(worldPosition))
            {
                //if the mouse is clicked once inside the board
                if (mouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed || mouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    //get the current rectangle the mouse is within
                    Point curGridLocation;
                    RectangleF curGrid = curBoard.getGridSquare(worldPosition, out curGridLocation);                    
                    //if the grid has been used before then remove the current object there and add the new one
                        for (int i = 0; i < entities.Count; i++)
                        {
                            if (entities[i].gridLocation == curGridLocation)
                            {
                                entities.Remove(entities[i]);
                                gridLocations.Remove(curGridLocation);
                            }
                    }
                }
            }
        }
        #endregion


        #region Load and Save and New and Delete
        //Events for saving and loading
        private void LoadPressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.028F, 0.05F);
                    //grab the informatin from the levelManager
                    entities = levelManager.getEntities();
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    sweeps = levelManager.getSweeps();
                    //finish loading the board
                    curBoard.LoadContent();
                    for (int i = 0; i <entities.Count; i++)
                    {
                        entities[i].LoadContent();
                        gridLocations.Add(entities[i].gridLocation);
                    }
                    nameField.Text = Path.GetFileName(file).Split(".")[0];
                    //level can be drawn and updated now. New thread can be made
                    levelLoaded = true;
                    RowsCol = curBoard.Rows;
                    sizeField.Text = Convert.ToString(RowsCol);
                    tankField.Text = Convert.ToString(TanksAndMines.X);
                    mineField.Text = Convert.ToString(TanksAndMines.Y);
                    sweepField.Text = Convert.ToString(sweeps);

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
            if (levelLoaded)
            {
                if (nameField.Text == "")
                {
                    nameField.Text = "Needs a file name";
                }
                else
                {
                    string fileName = nameField.Text.Replace(" ", "");
                    file = relativePath + "\\TankGame\\" + fileName + ".lvl";
                    levelManager.SaveLevel(file, curBoard, entities, TanksAndMines, sweeps);
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
            entities.Clear();
            gridLocations.Clear();
            curBoard.LoadContent();
            curBoard.setColor(new Color(235, 235, 235), new Color(200,200,200), Color.Black);
            nameField.Text = "New";           
            levelLoaded = true;
            sizeField.Text = "20";
            tankField.Text = "3";
            mineField.Text = "3";
            sweepField.Text = "3";
            RowsCol = 20;
            TanksAndMines = new Point(3, 3);
            sweeps = 3;

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
            entities.Clear();
            gridLocations.Clear();
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

            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
            sweepColor = Color.Black;
        }

        private void DeletePressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
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
            
            if (addItem.Texture == addItem.Pressed)
            {
                addItem.toggleTexture();
            }
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }
        }
        private void SelectItem(object sender, EventArgs e)
        {
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

            if (addWall.Texture == addWall.Pressed)
            {
                addWall.toggleTexture();
            }
            if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }
        }
        private void SelectErase(object sender, EventArgs e)
        {
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

            if (addItem.Texture == addItem.Pressed)
            {
                addItem.toggleTexture();
            }
            if (addWall.Texture == addWall.Pressed)
            {
                addWall.toggleTexture();
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
                float size = Camera.ViewboxScale.Y * 0.9F;
                Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
                curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), RowsCol, RowsCol, 8);
                curBoard.LoadContent();
                curBoard.setColor(new Color(235, 235, 235), new Color(200, 200, 200), Color.Black);
                int counter = entities.Count;
                for (int i = 0; i < counter; i++)
                {
                    try
                    {                        
                        entities[i].Initialize(curBoard.getGridSquare(entities[i].gridLocation.X, entities[i].gridLocation.Y)); 
                    }
                    catch {
                        gridLocations.Remove(entities[i].gridLocation);
                        entities.Remove(entities[i]);
                        i--;
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
                tankColor = Color.Black;
            }
            catch { }
        }
        private void SetMinesPressed(object sender, EventArgs e)
        {
            try
            {
                TanksAndMines.Y = Convert.ToInt16(mineField.Text);
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
            }
            catch { }
        }
        #endregion

        private void LevelListLoad()
        {
            //gets all the files in the relative folder and sends them as an array to the listbox to populate the levels
            string[] filepaths = Directory.GetFiles(relativePath + "\\TankGame");
            for (int i = 0; i < filepaths.Length; i++)
            {
                filepaths[i] = Path.GetFileName(filepaths[i].Split(".")[0]);
            }
            levelSelection.LoadContent(filepaths);
        }

        public override void ButtonReset()
        {
            //resets everybutton to prevent unwanted button clicks
            foreach (Button b in Buttons)
            {
                b.ButtonReset();
            }
        }
    }
}
