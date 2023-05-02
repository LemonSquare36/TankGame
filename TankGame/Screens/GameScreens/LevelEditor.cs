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
using System.Windows.Forms;
using System.Threading;
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class LevelEditor : GameScreenManager
    {
        //for words
        SpriteFont font;
        //button declares
        Texture2D wallTex;
        //generic buttons
        Button Load, Save, New;
        //add object buttons
        Button addWall, addItem, erase;
        List<Button> Buttons = new List<Button>();
        //file loading decalres
        OpenFileDialog openDialog;
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
        InputBox nameField;
        //Initialize
        public override void Initialize()
        {
            base.Initialize();
            levelManager = new LevelManager();
            //create the browser for searching for levels
            openDialog = new OpenFileDialog();
            openDialog.Title = "Select A File";
            openDialog.Filter = "Level Files (*.lvl)|*.lvl";
            //create the text field
            nameField = new InputBox(new Color(235, 235, 235), Color.Black, new Vector2(1350, 200), new Vector2(300, 50));

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
            Load = new Button(new Vector2(1300, 100), 100, 50, "Buttons/Editor/Load", "load");
            Save = new Button(new Vector2(1450, 100), 100, 50, "Buttons/Editor/Save", "save");
            New = new Button(new Vector2(1600, 100), 100, 50, "Buttons/Editor/New", "new");

            addWall = new Button(new Vector2(1350, 400), 50, 50, "Buttons/Editor/Wall", "addWall", "toggle");
            addItem = new Button(new Vector2(1450, 400), 50, 50, "Buttons/Editor/ItemBox", "addItem", "toggle");
            #endregion
            #region Button Events
            Load.ButtonClicked += LoadPressed;
            Save.ButtonClicked += SavePressed;
            New.ButtonClicked += NewPressed;

            addWall.ButtonClicked += SelectWall;
            addItem.ButtonClicked += SelectItem;
            #endregion
            #region ButtonList
            Buttons.Add(Load);
            Buttons.Add(Save);
            Buttons.Add(New);
            Buttons.Add(addWall);
            Buttons.Add(addItem);
            #endregion 

            nameField.LoadContent();
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
            //
            nameField.Update(mouse, worldPosition, keyState, keyHeldState);
            //if the level is loaded
            if (levelLoaded)
            {
                //if walls are selected this code allows for the addition of walls
                if (wallSelected)
                {
                    AddWall();
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
            //
            nameField.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Add an Object", new Vector2(1350, 300), Color.Black);
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
        #endregion


        #region Load and Save and New
        //Events for saving and loading
        public void LoadPressed(object sender, EventArgs e)
        {
            if (!threadActive)
            {
                threadActive = true;
                levelLoaded = false;
                //thread created so I can run the form in STA - required
                Thread explorerThread = new Thread(() => exploreForFile());
                explorerThread.SetApartmentState(ApartmentState.STA);
                explorerThread.Start();
            }
        }
        private void exploreForFile()
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                file = openDialog.FileName;
                levelManager.LoadLevel(file);
                loadLevel();
            }
        }
        private void loadLevel()
        {
            //grab the informatin from the levelManager
            entities = levelManager.getEntities();
            curBoard = levelManager.getGameBoard();
            //finish loading the board
            curBoard.LoadContent();
            foreach (Entity e in entities)
            {
                e.LoadContent();
                gridLocations.Add(e.gridLocation);
            }
            nameField.Text = "test";
            //level can be drawn and updated now. New thread can be made
            levelLoaded = true;
            threadActive = false;
        }
        public void SavePressed(object sender, EventArgs e)
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
                    levelManager.SaveLevel(file, curBoard, entities);
                }
            }
        }
        //creates a fresh new board
        public void NewPressed(object sender, EventArgs e)
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
        }
        #endregion


        #region Add Objects Events
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
            /*if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }*/
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
            /*if (erase.Texture == erase.Pressed)
            {
                erase.toggleTexture();
            }*/
        }
        #endregion 
    }
}
