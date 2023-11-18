using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using System.Threading.Tasks;
using System.Diagnostics;
using TankGame.Objects.Entities.Items;
using TankGame.GameInfo;

namespace TankGame.GameInfo
{
    internal class Inventory
    {
        public int selectedItemsCount;
        public int sweeps;
        public int missile, shotgun, builder, wallDestroyer, teleport, superSweeper;
        Items items = new Items();

        public Inventory()
        {
            sweeps = 0;
            missile = 0;
            shotgun = 0;
            builder = 0;
            wallDestroyer = 0;
            teleport = 0;
            superSweeper = 0;
        }
        public Inventory(int Sweeps)
        {
            sweeps = Sweeps;
            missile = 0;
            shotgun = 0;
            builder = 0;
            wallDestroyer = 0;
            teleport = 0;
            superSweeper = 0;
        }
        //inventory needs relevent information to do a few tasks
        public void UseItem(string selectedItem, BoardState boardState, Board curBoard, Pathfinder pathfinder, Point curGridLocation, bool drawTankInfo, int activeTankNum, ButtonState curLeftClick, ButtonState oldLeftClick)
        {            
            switch (selectedItem)
            {
                case "sweeper":
                    items.UseSweeper(boardState, curBoard, pathfinder, curGridLocation, drawTankInfo, activeTankNum, curLeftClick, oldLeftClick);
                    selectedItemsCount = sweeps;
                    break;
                case "superSweeper":

                    break;
                case "missile":

                    break;
                case "shotgun":

                    break;
                case "builder":

                    break;
                case "wallDestroyer":

                    break;
                case "teleport":

                    break;
                default:

                    break;
            }
        }
        public void DrawItemUI(string selectedItem, SpriteBatch spriteBatch, Texture2D UITexture)
        {
            switch (selectedItem)
            {
                case "sweeper":
                    items.DrawSweeper(spriteBatch, UITexture);
                    break;
                case "superSweeper":

                    break;
                case "missile":

                    break;
                case "shotgun":

                    break;
                case "builder":

                    break;
                case "wallDestroyer":

                    break;
                case "teleport":

                    break;
                default:

                    break;
            }
        }
        public void setSelectedItemCount(string selectedItem)
        {
            switch (selectedItem)
            {
                case "sweeper":                    
                    selectedItemsCount = sweeps;
                    break;
                case "superSweeper":
                    selectedItemsCount = superSweeper;
                    break;
                case "missile":
                    selectedItemsCount = missile;
                    break;
                case "shotgun":
                    selectedItemsCount = shotgun;
                    break;
                case "builder":
                    selectedItemsCount = builder;
                    break;
                case "wallDestroyer":
                    selectedItemsCount = wallDestroyer;
                    break;
                case "teleport":
                    selectedItemsCount = teleport;
                    break;
                default:

                    break;
            }
        }
    }
}
