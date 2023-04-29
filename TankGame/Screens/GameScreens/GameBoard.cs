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
        Matrix boardMatrix;
        Viewport boardView;

        Wall tw;

        public override void Initialize()
        {
            base.Initialize();
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;
            boarderThickness = 8;
            boardPos = new Point(0, 0);

            //create new board
            boardView = new Viewport(boardPos.X, boardPos.Y, Main.gameWindow.ClientBounds.Height, Main.gameWindow.ClientBounds.Height);
            Camera.setBound(boardView, out boardView);
            boardMatrix = Camera.getScalingMatrix(Camera.ResolutionScale.X, Camera.ResolutionScale.Y);

            gameBoard = new Board(boardPos, new Point(Convert.ToInt16(Camera.ViewboxScale.Y), Convert.ToInt16(Camera.ViewboxScale.Y)), 20, 20, boarderThickness);

            tw = new Wall(gameBoard.getGridSquare(5, 5));

        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            gameBoard.LoadContent();
            tw.LoadContent();
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {       
            //end the current call and begin the one scaled properly
            spriteBatch.End();
            //set viewport for play zone
            Main.graphicsDevice.Viewport = boardView;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, boardMatrix);

            gameBoard.drawCheckers(spriteBatch, Color.Red, Color.DarkRed);
            tw.Draw(spriteBatch);
            //gameBoard.drawGrid(spriteBatch, Color.Black);
            //end scalling and call again for next classes/objects
            spriteBatch.End();
            //reset viewport to regular client
            Main.graphicsDevice.Viewport = Main.DefualtView();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
            //draw the outline of the board over the old viewport
            gameBoard.DrawOutline(spriteBatch, Color.Black);

        }
       
    }
}
