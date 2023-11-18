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
using TankGame.GameInfo;
using TankGame.Tools;

namespace TankGame.Objects.Entities.Items
{
    internal class Items
    {
        protected RectangleF[,] UITiles;

        #region sweeper
        public void UseSweeper(BoardState boardState, Board curBoard, Pathfinder pathfinder, Point curGridLocation, bool drawTankInfo, int activeTankNum, ButtonState curLeftClick, ButtonState oldLeftClick)
        {
            UITiles = curBoard.getSubGrid(new Vector2(curGridLocation.X - 1, curGridLocation.Y - 1), new Vector2(4, 4));
            //check if the sweeper is used on a square
            if (curLeftClick == ButtonState.Pressed && oldLeftClick != ButtonState.Pressed)
            {
                if (boardState.playerList[boardState.curPlayerNum].inventory.sweeps > 0)
                {
                    boardState.playerList[boardState.curPlayerNum].inventory.sweeps--;
                    for (int i = 0; i < UITiles.GetUpperBound(0); i++)
                    {
                        for (int j = 0; j < UITiles.GetUpperBound(1); j++)
                        {
                            for (int k = 0; k < boardState.playerList.Count; k++)
                            {
                                if (k != boardState.curPlayerNum) //means its an enemy player
                                {
                                    foreach (Mine mine in boardState.playerList[k].mines)
                                    {
                                        if (curBoard.getGridSquare(mine.gridLocation.X, mine.gridLocation.Y).Location == UITiles[i, j].Location)
                                        {
                                            boardState.playerList[boardState.curPlayerNum].mines.Add(mine);
                                            GameScreensManager.UpdatePathFinderWithMines(boardState, pathfinder);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }
        public void DrawSweeper(SpriteBatch spriteBatch, Texture2D UITexture)
        {
            if (UITiles.LongLength > 0)
            {
                for (int i = 0; i < UITiles.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < UITiles.GetUpperBound(1); j++)
                    {
                        spriteBatch.Draw(UITexture, UITiles[i, j].Location, null, Color.LightSlateGray, 0, Vector2.Zero, UITiles[i, j].Size, SpriteEffects.None, 0);
                    }
                }
            }
        }
        #endregion
    }
}
