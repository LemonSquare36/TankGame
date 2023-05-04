﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;
using TankGame.Objects;
using TankGame.Tools;
using System.Threading;
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class LevelEditor : GameScreenManager
    {
        #region Declares (there are alot)
        //for words
        SpriteFont font;
        //button declares
        Texture2D wallTex;
        //generic buttons
        Button Load, Save, New, Delete;
        Button SetRowCol, SetTankCount, SetMineCount;
        //add object buttons
        Button addWall, addItem, erase;
        List<Button> Buttons = new List<Button>();
        LevelManager levelManager;
        //board info declares
        List<Entity> entities = new List<Entity>();
        List<Point> gridLocations = new List<Point>();
        Board curBoard;
        //level loading logic
        bool levelLoaded = false, threadActive = false;
        string file, relativePath;
        //objects selected logic
        bool wallSelected = false, itemSelected = false, eraseSelected = false;
        //input box
        InputBox nameField, sizeField, tankField, mineField;
        List<InputBox> Fields = new List<InputBox>();
        //List Box
        Tools.ListBox levelSelection;
        //the rows and columns
        int RowsCol;
        Point TanksAndMines;
        //colors for text
        Color rowColColor, tankColor, MineColor;
        #endregion

        //Initialize
        public override void Initialize()
        {
            base.Initialize();
            levelManager = new LevelManager();

            //create the text field
            #region initializing textboxes
            nameField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1350, 200), new Vector2(300, 50));
            sizeField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1250, 650), new Vector2(80, 70), 2);
            tankField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1450, 650), new Vector2(80, 70), 2);
            mineField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1650, 650), new Vector2(80, 70), 2);

            levelSelection = new ListBox(new Vector2(1170, 770), new Vector2(630, 300), 8, Color.White, Color.Black,Color.DarkGray, 4);
            #endregion

            #region default font colors
            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
            #endregion

            relativePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        //LoadContent
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            #region load Textures
            //font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            //textures
            wallTex = Main.GameContent.Load<Texture2D>("GameSprites/Wall");
            #endregion

            #region load buttons
            Load = new Button(new Vector2(1200, 100), 100, 50, "Buttons/Editor/Load", "load");
            Save = new Button(new Vector2(1350, 100), 100, 50, "Buttons/Editor/Save", "save");
            New = new Button(new Vector2(1500, 100), 100, 50, "Buttons/Editor/New", "new");
            Delete = new Button(new Vector2(1650, 100), 100, 50, "Buttons/Editor/Delete", "delete");

            SetRowCol = new Button(new Vector2(1350, 660), 70, 50, "Buttons/Editor/Set", "setrowcol");
            SetTankCount = new Button(new Vector2(1550, 660), 70, 50, "Buttons/Editor/Set", "settankcount");
            SetMineCount = new Button(new Vector2(1750, 660), 70, 50, "Buttons/Editor/Set", "setminecount");

            addWall = new Button(new Vector2(1320, 400), 50, 50, "Buttons/Editor/Wall", "addWall", "toggle");
            addItem = new Button(new Vector2(1470, 400), 50, 50, "Buttons/Editor/ItemBox", "addItem", "toggle");
            erase = new Button(new Vector2(1620, 400), 50, 50, "Buttons/Editor/Clear", "erase", "toggle");
            #endregion

            #region Button Events
            Load.ButtonClicked += LoadPressed;
            Save.ButtonClicked += SavePressed;
            New.ButtonClicked += NewPressed;
            Delete.ButtonClicked += DeletePressed;

            SetRowCol.ButtonClicked += SetRowColPressed;
            SetTankCount.ButtonClicked += SetTanksPressed;
            SetMineCount.ButtonClicked += SetMinesPressed;

            addWall.ButtonClicked += SelectWall;
            addItem.ButtonClicked += SelectItem;
            erase.ButtonClicked += SelectErase;
            #endregion

            #region ButtonList
            Buttons.Add(Load);
            Buttons.Add(Save);
            Buttons.Add(New);
            Buttons.Add(Delete);
            Buttons.Add(addWall);
            Buttons.Add(addItem);
            Buttons.Add(erase);
            Buttons.Add(SetRowCol);
            Buttons.Add(SetTankCount);
            Buttons.Add(SetMineCount);
            #endregion

            #region input box list
            Fields.Add(nameField);
            Fields.Add(sizeField);
            Fields.Add(tankField);
            Fields.Add(mineField);
            #endregion

            foreach (InputBox box in Fields)
            {
                box.LoadContent();
            }
            //load the listBox for level selection
            LevelListLaod();


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
            spriteBatch.DrawString(font, "Add an Object\nRight Click to Erase", new Vector2(1350, 300), Color.Black);
            spriteBatch.DrawString(font, "Walls", new Vector2(1300, 450), Color.Black);
            spriteBatch.DrawString(font, "ItemBoxes", new Vector2(1412, 450), Color.Black);
            spriteBatch.DrawString(font, "Eraser", new Vector2(1600, 450), Color.Black);
            spriteBatch.DrawString(font, "Level Name", new Vector2(1405, 155), Color.Black);
            spriteBatch.DrawString(font, "Rows/Columns", new Vector2(1150, 580), rowColColor);
            spriteBatch.DrawString(font, "# of Tanks", new Vector2(1410, 580), tankColor);
            spriteBatch.DrawString(font, "# of Mines", new Vector2(1610, 580), MineColor);
            spriteBatch.DrawString(font, "Red Text = Change Not Set", new Vector2(1250, 530), Color.Red);
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
                    levelManager.LoadLevel(file);
                    //grab the informatin from the levelManager
                    entities = levelManager.getEntities();
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
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
                    threadActive = false;
                    RowsCol = curBoard.Rows;
                    sizeField.Text = Convert.ToString(RowsCol);
                    tankField.Text = Convert.ToString(TanksAndMines.X);
                    mineField.Text = Convert.ToString(TanksAndMines.Y);

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
                    levelManager.SaveLevel(file, curBoard, entities, TanksAndMines);
                }
            }
            //load the listBox for level selection
            LevelListLaod();
        }

        //creates a fresh new board
        private void NewPressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\";
            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
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
            RowsCol = 20;
            TanksAndMines = new Point(3, 3);

            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
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
            RowsCol = 20;
            TanksAndMines = new Point(3, 3);

            rowColColor = Color.Black;
            tankColor = Color.Black;
            MineColor = Color.Black;
        }

        private void DeletePressed(object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                File.Delete(file);
                LevelListLaod();
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
            TanksAndMines.X = Convert.ToInt16(tankField.Text);
            tankColor = Color.Black;
        }
        private void SetMinesPressed(object sender, EventArgs e)
        {
            TanksAndMines.Y = Convert.ToInt16(mineField.Text);
            MineColor = Color.Black;
        }
        #endregion

        private void LevelListLaod()
        {
            //gets all the files in the relative folder and sends them as an array to the listbox to populate the levels
            string[] filepaths = Directory.GetFiles(relativePath + "\\TankGame");
            for (int i = 0; i < filepaths.Length; i++)
            {
                filepaths[i] = Path.GetFileName(filepaths[i].Split(".")[0]);
            }
            levelSelection.LoadContent(filepaths);
        }
    }
}
