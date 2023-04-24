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

namespace TankGame
{
    internal class GameBoard : GameScreenManager
    {
        Board gameBoard;
        Tank testTank;
        public override void Initialize()
        {
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;
            gameBoard = new Board(new Point(100,1), new Point(600,600), 6, 6);
            testTank = new Tank(gameBoard.getGridSquare(1,4));
            
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;
            gameBoard.LoadContent();
            testTank.LoadContent();
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            //spriteBatch.Draw(Outline,new Rectangle(new Point(100, 100), new Point(600, 600)) , Color.Blue);
            //rectangle - first point is the top left corner, second point is the bottom right corner based on the position of point 1
            gameBoard.draw(spriteBatch, Color.Black);
            testTank.Draw(spriteBatch);
        }
       
    }
}
