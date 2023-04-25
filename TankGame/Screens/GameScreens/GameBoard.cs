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

namespace TankGame
{
    internal class GameBoard : GameScreenManager
    {
        Board gameBoard;
        Tank testTank;

        Camera gameView, sidebar;
        public override void Initialize()
        {
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;


            //initialize the cameras
            gameView = new Camera(new Point(0,0), new Point(Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height));
            //create new board
            gameBoard = new Board(new Point(0, 0), new Point(Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height), 8, 12);

            testTank = new Tank(gameBoard.getGridSquare(1, 4));

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

            //get the camera factors for scaling the screen
            gameView.scaleCameraStart();
            Main.graphicsDevice.Viewport = gameView.GetViewport();

            //end the current call and begin the one scaled properly
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, gameView.getScalingMatrix());


            gameBoard.draw(spriteBatch, Color.Black);
            testTank.Draw(spriteBatch);
        }
       
    }
}
