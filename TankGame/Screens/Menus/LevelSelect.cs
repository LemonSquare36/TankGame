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
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.GameInfo;

namespace TankGame
{
    internal class LevelSelect : ScreenManager
    {
        //file selection stuff
        ListBox levelSelection;
        Button Select;
        string file;
        //keeps track if level info is loaded or not
        bool levelLoaded = false;


        //Holds Initialize
        public override void Initialize()
        {
            base.Initialize();
            //the level selection box for the level select screen
            levelSelection = new ListBox(new Vector2(1070, Camera.ViewboxScale.Y * .055F), new Vector2(800, 800), 16, Color.Black, Color.LightGreen, Color.DarkGray, 6);
            levelSelection.GetEvent += DisplayPreview;

            //button stuff
            Select = new Button(new Vector2(1270, 900), 400, 150, "Buttons/LevelSelect/Select", "select", 0);
            Select.ButtonClicked += ScreenChangeEvent;
        }
        //Holds LoadContent and the font if called
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;
            LevelListLoad();
            levelSelection.OffSetColor = new Vector3(-60, -60, -60);
        }
        //Holds Update
        public override void Update()
        {
            base.Update();
            selectedFile = levelSelection.curSelection;
            levelSelection.Update(mouse, worldPosition);
            Select.Update(mouse, worldPosition);
        }
        //Holds Draw
        public override void Draw()
        {
            levelSelection.Draw(spriteBatch);
            Select.Draw(spriteBatch);
            //if the level is loaded, draw the preview
            if (levelLoaded)
            {
                curBoard.drawCheckers(spriteBatch);
                curBoard.DrawOutline(spriteBatch);

                foreach (Entity e in boardState.entities)
                {
                    e.Draw(spriteBatch);
                }               
            }
        }

        //Holds the Function
        public override void ButtonReset()
        {

        }

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
        private void DisplayPreview(Object sender, EventArgs e)
        {
            file = relativePath + "\\TankGame\\" + levelSelection.curSelection + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.028F, 0.05F);
                    //grab the informatin from the levelManager
                    boardState = new BoardState(levelManager.getEntities(), levelManager.getWalls(), levelManager.getItemBoxes());
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    //finish loading the board
                    curBoard.LoadContent();
                    boardState.LoadEntities();
                    //level can be drawn and updated now
                    levelLoaded = true;
                }
                catch {  }
            }
        }
    }
}
