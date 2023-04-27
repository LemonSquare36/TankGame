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
using System.Reflection;

namespace TankGame.Tools
{
    internal class GameBoard : GameScreenManager
    {
        Board gameBoard;
        int boarderThickness;
        Point boardPos;
        Matrix boardMatrix, RegMatrix;
        Viewport boardView, RegView;
        //Tank testTank;

        public override void Initialize()
        {
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;
            boarderThickness = 4;
            boardPos = new Point(0, 0);

            //create new board
            boardView = new Viewport(boardPos.X, boardPos.Y, Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height);
            Camera.setBound(boardView);
            boardMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);

            gameBoard = new Board(boardPos, new Point(Convert.ToInt16(Camera.ViewboxScale.Y), Convert.ToInt16(Camera.ViewboxScale.Y)), 100, 100, boarderThickness);
            //testTank = new Tank(gameBoard.getGridSquare(1, 1));

            //get the amount to scale the board to fit the outline

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
       
            //end the current call and begin the one scaled properly
            spriteBatch.End();
            //set viewport for play zone
            Main.graphicsDevice.Viewport = boardView;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, boardMatrix);

            
            gameBoard.drawCheckers(spriteBatch, Color.Red, Color.DarkRed);
           // gameBoard.drawGrid(spriteBatch, Color.CornflowerBlue);
            //testTank.Draw(spriteBatch);
            //end scalling and call again for next classes/objects
            spriteBatch.End();

            //reset viewport to regular client
            Main.graphicsDevice.Viewport = new Viewport(new Rectangle(new Point(0,0), new Point(Main.gameWindow.ClientBounds.Width, Main.gameWindow.ClientBounds.Height)));
            Camera.setBound(Main.graphicsDevice.Viewport);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null,
                Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y));
            //draw the outline of the board over the old viewport
            gameBoard.DrawOutline(spriteBatch, Color.Black);

        }
       
    }
}
