using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.GameInfo;
using TankGame.Objects.Entities;
using TankGame.Objects.Entities.Items;

namespace TankGame.Objects
{
    //stores the information for a player
    internal class Player
    {
        public List<Items> Items = new List<Items>();
        public List<Tank> tanks = new List<Tank>();
        public List<Mine> mines = new List<Mine>();

        //inventory for the player
        public Inventory inventory;

        private Color playerColor;

        public List<SpawnTile> SpawnTiles;

        public Player(int Sweeps)
        {
            inventory = new Inventory(Sweeps);
        }
        public int getActiveTankNum()
        {
            for (int i = 0; i < tanks.Count; i++)
            {
                if (tanks[i].Active)
                {
                    return i;
                }
            }
            return -2; //return -2 for no tanks active so that it can be compared to a turn start -1 and return true for currrent tank not active
        }
    }
}

