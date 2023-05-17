using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Objects.Entities;

namespace TankGame.Objects
{
    //stores the information for a player
    internal class Player
    {
        public int AP;
        public int sweeps;
        public List<Items> Items = new List<Items>();
        public List<Tank> tanks = new List<Tank>();
        public List<Mine> mines = new List<Mine>();

        //information for start of turn state
        public int oldSweeps;
        public List<Items> oldItems = new List<Items>();
        public List<Tank> oldTanks = new List<Tank>();

        public Player(int ActionPoints, int Sweeps)
        {
            AP = ActionPoints;
            sweeps = Sweeps;
        }
    }
}
