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
            boarderThickness = 8;
            boardPos = new Point(1000, 500);

            //create new board
            boardView = new Viewport(boardPos.X, boardPos.Y, Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height);
            Camera.setBound(boardView, out boardView);
            boardMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);

            gameBoard = new Board(boardPos, new Point(Convert.ToInt16(Camera.ViewboxScale.Y*.70F), Convert.ToInt16(Camera.ViewboxScale.Y * .70F)), 20, 20, boarderThickness);

            RegView = new Viewport(new Rectangle(new Point(0, 0), new Point(Main.gameWindow.ClientBounds.Width, Main.gameWindow.ClientBounds.Height)));
            Camera.setBound(RegView);
            RegMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);
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
            //end the current call and begin the one scaled properly
            spriteBatch.End();
            //set viewport for play zone
            Main.graphicsDevice.Viewport = boardView;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, boardMatrix);

            gameBoard.drawCheckers(spriteBatch, Color.Red, Color.DarkRed);
            //gameBoard.drawGrid(spriteBatch, Color.Black);
            //end scalling and call again for next classes/objects
            spriteBatch.End();
            //reset viewport to regular client
            Main.graphicsDevice.Viewport = RegView;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, RegMatrix);
            //draw the outline of the board over the old viewport
            gameBoard.DrawOutline(spriteBatch, Color.Black);

        }
       
    }
}
