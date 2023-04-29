using System;
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
using System.Reflection.Metadata;

namespace TankGame
{
    internal class LevelEditor : GameScreenManager
    {
        //button declares
        Texture2D LoadH, LoadUH, SaveH, SaveUH, NewH, NewUH;
        Button Load, Save, New;
        List<Button> Buttons = new List<Button>();
        //file loading decalres
        OpenFileDialog openDialog;
        LevelManager levelManager;
        //board info declares
        List<Entity> entities = new List<Entity>();
        Board curBoard;
        //level loading logic
        bool levelLoaded = false;
        bool threadActive = false;
        string file;
        //Initialize
        public override void Initialize()
        {
            base.Initialize();
            levelManager = new LevelManager();
            //create the browser for searching for levels
            openDialog = new OpenFileDialog();
            openDialog.Title = "Select A File";
            openDialog.Filter = "Level Files (*.lvl)|*.lvl";
        }
        //LoadContent
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            #region load Textures
            LoadH = Main.GameContent.Load<Texture2D>("Buttons/Editor/LoadH");
            LoadUH = Main.GameContent.Load<Texture2D>("Buttons/Editor/LoadUH");
            SaveH = Main.GameContent.Load<Texture2D>("Buttons/Editor/SaveH");
            SaveUH = Main.GameContent.Load<Texture2D>("Buttons/Editor/SaveUH");
            NewH = Main.GameContent.Load<Texture2D>("Buttons/Editor/NewH");
            NewUH = Main.GameContent.Load<Texture2D>("Buttons/Editor/NewUH");
            #endregion
            #region load buttons
            Load = new Button(new Vector2(1300, 100), 100, 50, LoadUH, LoadH, "load", 1);
            Save = new Button(new Vector2(1450, 100), 100, 50, SaveUH, SaveH, "save", 1);
            New = new Button(new Vector2(1600, 100), 100, 50, NewUH, NewH, "new", 1);
            #endregion
            #region Button Events
            Load.ButtonClicked += LoadPressed;
            Save.ButtonClicked += SavePressed;
            New.ButtonClicked += NewPressed;
            #endregion
            #region ButtonList
            Buttons.Add(Load);
            Buttons.Add(Save);
            Buttons.Add(New);
            #endregion 
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
            if (levelLoaded)
            {

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
        }
        #region Load and Save and New
        //Events for saving and loading
        public void LoadPressed(object sender, EventArgs e)
        {
            if (!threadActive)
            {
                threadActive = true;
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
            }
            //level can be drawn and updated now. New thread can be made
            levelLoaded = true;
            threadActive = false;
        }
        public void SavePressed(object sender, EventArgs e)
        {
            if (levelLoaded)
            {
                levelManager.SaveLevel(file, curBoard, entities);
            }
        }
        //creates a fresh new board
        public void NewPressed(object sender, EventArgs e)
        {
            file = "D:\\Projects\\TankGame\\TankGame\\Content\\New.lvl";
            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));
            curBoard = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), 10, 10, 8);
            entities.Clear();
            curBoard.LoadContent();
            curBoard.setColor(new Color(200, 0, 0), new Color(100, 0, 0), Color.Black);
            levelLoaded = true;
        }
        #endregion
    }
}
