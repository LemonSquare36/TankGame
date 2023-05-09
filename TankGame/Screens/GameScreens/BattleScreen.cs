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
using TankGame.Objects.Entities;

namespace TankGame
{
    internal class BattleScreen : ScreenManager
    {
        //hold the level file location
        private string file;

        public virtual void Initialize()
        {
            base.Initialize();
        }

        public virtual void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            //load the board (load for host before sending to peer)
            LoadBoardfromFile();
        }

        public override void Update()
        {
            base.Update();
        }

        public virtual void Draw()
        {

        }

        public virtual void ButtonReset()
        {

        }
        private void LoadBoardfromFile()
        {
            file = relativePath + "\\TankGame\\" + selectedFile + ".lvl";
            if (file != relativePath + "\\TankGame\\" + "" + ".lvl")
            {
                try
                {
                    levelManager.LoadLevel(file);
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
