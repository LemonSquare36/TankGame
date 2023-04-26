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
        float imageScaleX, imageScaleY;

        Camera gameView, sidebar;
        public override void Initialize()
        {
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;
            imageScaleX = 1;
            imageScaleY = 1;

            //initialize the cameras
            gameView = new Camera(new Point(2,2), new Point(Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height));
            //create new board
            gameBoard = new Board(new Vector2(0, 0), new Vector2(Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height), 20, 80);

            //testTank = new Tank(gameBoard.getGridSquare(1, 1));

            //get the camera factors for scaling the screen
            gameView.ScaleToViewport();

            //get the amount to scale the board to fit the outline
            Vector2 boardEdge = gameBoard.getRealBoardSize();
            Vector2 outlineinnerEdge = gameBoard.getOutlineSize();
            imageScaleX = outlineinnerEdge.X / boardEdge.X;
            imageScaleY = outlineinnerEdge.Y / (boardEdge.Y+8);

        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;
            gameBoard.LoadContent();
            //testTank.LoadContent();
        }
        public override void Update()
        {

        }

        public override void Draw()
        {
            //spriteBatch.Draw(Outline,new Rectangle(new Point(100, 100), new Point(600, 600)) , Color.Blue);
            //rectangle - first point is the top left corner, second point is the bottom right corner based on the position of point 1

            //set viewport for play zone
            Main.graphicsDevice.Viewport = gameView.Viewport;
            

            //end the current call and begin the one scaled properly
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, 
                gameView.getScalingMatrix(1, 1));


            gameBoard.draw(spriteBatch, Color.Red);
            //testTank.Draw(spriteBatch);
            //end scalling and call again for next classes/objects
            spriteBatch.End();
            //reset viewport to regular client
            Main.graphicsDevice.Viewport = new Viewport(new Rectangle(new Point(0,0), new Point(Main.gameWindow.ClientBounds.Width, Main.gameWindow.ClientBounds.Height)));
            spriteBatch.Begin();
            //draw the outline of the board over the old viewport
            gameBoard.DrawOutline(spriteBatch, Color.Black);
        }
       
    }
}
