using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Objects.Entities;
using TankGame.Objects.Entities.Items;

namespace TankGame.Objects
{
    //stores the information for a player
    internal class Player
    {
        public int AP;
        public int startAP;
        public int sweeps;
        public List<Items> Items = new List<Items>();
        public List<Tank> tanks = new List<Tank>();
        public List<Mine> mines = new List<Mine>();

        private Color playerColor;

        public List<SpawnTile> SpawnTiles;

        private RectangleF[,] SpawnRows;
        public RectangleF[,] spawnRows
        {
            get
            {
                return SpawnRows;
            }
            set
            {
                SpawnRows = value;
                //set the spawn rectangle based on SpawnRows size information
                int test = SpawnRows.GetUpperBound(0);
                int test2 = SpawnRows.GetUpperBound(1);
                spawn = new RectangleF(SpawnRows[0, 0].Location, (SpawnRows[SpawnRows.GetUpperBound(0), SpawnRows.GetUpperBound(1)].Location - SpawnRows[0, 0].Location) + SpawnRows[0, 0].Size);
            }
        }
        public RectangleF spawn;

        public Player(int ActionPoints, int Sweeps)
        {
            AP = ActionPoints;
            startAP = ActionPoints;
            sweeps = Sweeps;
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
        public void startTurn()
        {
            AP = startAP;
        }
    }
}

