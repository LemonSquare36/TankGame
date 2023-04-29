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
using TankGame.Objects.Entities;

namespace TankGame.Tools
{
    internal class GameBoard : GameScreenManager
    {
        Board gameBoard;
        int boarderThickness;
        Point boardPos;

        Wall tw;

        public override void Initialize()
        {
            base.Initialize();
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;
            boarderThickness = 8;
            boardPos = new Point(0, 0);

            gameBoard = new Board(boardPos, new Point(Convert.ToInt16(Camera.ViewboxScale.Y), Convert.ToInt16(Camera.ViewboxScale.Y)), 20, 20, boarderThickness);

            tw = new Wall(gameBoard.getGridSquare(5, 5), new Point(5,5));

        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            gameBoard.LoadContent();
            gameBoard.setColor(new Color(200,0,0), new Color(100,0,0), Color.Black);
            tw.LoadContent();
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {       

            gameBoard.drawCheckers(spriteBatch);
            tw.Draw(spriteBatch);
            //gameBoard.drawGrid(spriteBatch, Color.Black);
            //end scalling and call again for next classes/objects

            //draw the outline of the board over the old viewport
            gameBoard.DrawOutline(spriteBatch);

        }
       
    }
}
