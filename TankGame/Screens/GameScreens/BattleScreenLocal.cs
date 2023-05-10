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

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            //load the board (load for host before sending to peer)
            LoadBoardfromFile();

            //SendLoadToPeer();
        }

        public override void Update()
        {
            base.Update();
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
        }

        public override void ButtonReset()
        {

        }
        private void LoadBoardfromFile()
        {
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
