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
using System.Windows.Forms;
using System.Threading;

namespace TankGame
{
    internal class LevelEditor : GameScreenManager
    {
        Texture2D LoadH, LoadUH, SaveH, SaveUH;
        Button Load, Save;
        OpenFileDialog openDialog;
        //Initialize
        public override void Initialize()
        {
            base.Initialize();
            //create the thread needed to run the file browser

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
            #endregion
            #region load buttons
            Load = new Button(new Vector2(1400, 100), 100, 50, LoadUH, LoadH, "load", 1);
            Save = new Button(new Vector2(1600, 100), 100, 50, SaveUH, SaveH, "save", 1);
            #endregion
            #region Button Events
            Load.ButtonClicked += LoadPressed;
            #endregion
        }
        //Update
        public override void Update()
        {
            base.Update();

            Load.Update(mouse, worldPosition);
            Save.Update(mouse, worldPosition);
        }
        //Draw
        public override void Draw()
        {
            Load.Draw(spriteBatch);
            Save.Draw(spriteBatch);
        }
        //Events for saving and loading
        public void LoadPressed(object sender, EventArgs e)
        {
            Thread explorerThread = new Thread(() => exploreForFile());
            explorerThread.SetApartmentState(ApartmentState.STA);
            explorerThread.Start();
            explorerThread.Interrupt();
        }
        private void exploreForFile()
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openDialog.FileName;
            }
        }
    }
}
