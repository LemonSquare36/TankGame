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
    internal class BattleScreenLocal : ScreenManager
    {
        //hold the level file location
        private string file;
        int activePlayer;
        private bool placementStage, battleStarted;

        InputBox tanksCount, minesCount;
        Button tanks, mines;

        int tanksUsed = 0, minesUsed = 0;
        

        public override void Initialize()
        {
            base.Initialize();
            placementStage = true;
            battleStarted = false;
            activePlayer = 1;
            //create input boxes for displaying tanks and mine count
            tanksCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1550, 700), new Vector2(80, 70), 0);
            minesCount = new InputBox(Color.Black, Color.LightGreen, new Vector2(1750, 700), new Vector2(80, 70), 0);

        }

        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            //load the board (load for host before sending to peer)
            LoadBoardfromFile();

            //SendLoadToPeer();

            //load inputboxes
            tanksCount.LoadContent();
            minesCount.LoadContent();
            //populate the information to show how many tanks and mines they get to place
            tanksCount.Text = Convert.ToString(TanksAndMines.X);
            minesCount.Text = Convert.ToString(TanksAndMines.Y);

        }

        public override void Update()
        {
            base.Update();
            //update code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                //update the text to show how many tanks are left
                tanksCount.Text = Convert.ToString(TanksAndMines.X - tanksUsed);
                minesCount.Text = Convert.ToString(TanksAndMines.Y - minesUsed);
                //code for placing and picking up tanks
                Placement();
            }
            //update code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {

            }
        }

        public override void Draw()
        {
            //board draw 
            curBoard.drawCheckers(spriteBatch);
            curBoard.DrawOutline(spriteBatch);
            foreach (Entity e in entities)
            {
                e.Draw(spriteBatch);
            }
            //draw code for the placement stage. Placing tanks and mines
            if (placementStage)
            {
                minesCount.Draw(spriteBatch);
                tanksCount.Draw(spriteBatch);
            }
            //draw code for if the battle has started. All players ready to begin the fight
            else if (battleStarted)
            {

            }
        }

        public override void ButtonReset()
        {

        }
       
        private void Placement()
        {

        }

        private void LoadBoardfromFile()
        {
            //load the board and additional data from the file passed in levelselect.
            file = relativePath + "\\TankGame\\" + selectedFile + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file, 0.2468F, 0.05F);
                    //grab the informatin from the levelManager
                    entities = levelManager.getEntities();
                    curBoard = levelManager.getGameBoard();
                    TanksAndMines = levelManager.getTanksAndMines();
                    sweeps = levelManager.getSweeps();
                    //finish loading the board
                    curBoard.LoadContent();
                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].LoadContent();
                        gridLocations.Add(entities[i].gridLocation);
                    }                    
                    RowsCol = curBoard.Rows;
                }
                catch {  }
            }
            else { }
        }
    }
}
