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
        public int sweeps;
        public int missile, shotgun, builder, wallDestroyer, teleport, superSweeper;
        Items item = new Items("", "");
        protected List<Animation> activeAnimations = new();
        Sweeper sweeper;

        public Inventory()
        {
            sweeps = 0;
            missile = 0;
            shotgun = 0;
            builder = 0;
            wallDestroyer = 0;
            teleport = 0;
            superSweeper = 0;

            InitializeItems();
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

            InitializeItems();
        }
        private void InitializeItems()
        {
            sweeper = new Sweeper("Sounds/sweeper", "GameSprites/SpriteSheets/BattleSprites/radar");
        }
        //inventory needs relevent information to do a few tasks
        public void UseItem(string selectedItem, BoardState boardState, Board curBoard, Pathfinder pathfinder, Point curGridLocation, bool drawTankInfo, int activeTankNum, ButtonState curLeftClick, ButtonState oldLeftClick)
        {
            switch (selectedItem)
            {

                case "sweeper":
                    item = sweeper;
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
            item.GetUIUpdates(curBoard, curGridLocation);
            if (curLeftClick == ButtonState.Pressed && oldLeftClick != ButtonState.Pressed)
            {
                if (getSelectedItemsCount(selectedItem) > 0)
                {
                    setSelectedItemsCount(selectedItem, getSelectedItemsCount(selectedItem)-1);
                    item.UseItem(boardState, curBoard, pathfinder, curGridLocation, drawTankInfo, activeTankNum, curLeftClick, oldLeftClick);
                    activeAnimations.Add(item.CreateNewAnimation(item.locationOfAnimation));
                }
            }
        }
        public void DrawItemUI(string selectedItem, SpriteBatch spriteBatch, Texture2D UITexture)
        {
            switch (selectedItem)
            {
                case "sweeper":
                    item.DrawItemUI(spriteBatch, UITexture);
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
        public void DrawItemAnimation(SpriteBatch spriteBatch, float scale)
        {
            for (int i = 0; i < activeAnimations.Count; i++)
            {
                activeAnimations[i].PlayAnimation(spriteBatch, scale);
                if (activeAnimations[i].stopped)
                {
                    activeAnimations.Remove(activeAnimations[i]);
                }
            }
        }
        public int getSelectedItemsCount(string selectedItem)
        {

            switch (selectedItem)
            {
                case "sweeper":
                    return sweeps;
                    
                case "superSweeper":
                    return superSweeper;
                    
                case "missile":
                    return missile;
                    
                case "shotgun":
                    return shotgun;
                    
                case "builder":
                    return builder;
                    
                case "wallDestroyer":
                    return wallDestroyer;
                    
                case "teleport":
                    return teleport;
                    
                default:
                    return 0;
                    
            }
        }
        public void setSelectedItemsCount(string selectedItem, int value)
        {
            switch (selectedItem)
            {
                case "sweeper":
                    sweeps = value;
                    break;
                case "superSweeper":
                    superSweeper = value;
                    break;
                case "missile":
                    missile = value;
                    break;
                case "shotgun":
                    shotgun = value;
                    break;
                case "builder":
                    builder = value;
                    break;
                case "wallDestroyer":
                    wallDestroyer = value;
                    break;
                case "teleport":
                    teleport = value;
                    break;
                default:
                    break;
            }
        }
        public static Inventory Clone(Inventory inventory)
        {
            Inventory @new = new Inventory(inventory.sweeps);
            @new.missile = inventory.missile;
            @new.shotgun = inventory.shotgun;
            @new.builder = inventory.builder;
            @new.wallDestroyer = inventory.wallDestroyer;
            @new.teleport = inventory.teleport;
            @new.superSweeper = inventory.superSweeper;
            @new.activeAnimations = inventory.activeAnimations;
            @new.InitializeItems();
            return inventory;
        }
    }
}
