using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankGame.Objects.Entities.Items;

namespace TankGame.GameInfo
{
    internal class RuleSet
    {
        public int tankPoints, numOfMines, startingSweeps, numOfPlayers;
        public List<string> allowedItems = new();
        public List<string> allowedTanks = new();

        /// <summary>
        /// This constructor is for testing purposes
        /// </summary>
        /// <param name="NumOfPlayers"></param>
        public RuleSet(int NumOfPlayers)
        {
            numOfPlayers = NumOfPlayers;
            tankPoints = 30;
            numOfMines = 3;
            startingSweeps = 3;

            allowedItems.Add("sweeper");
            //allowedItems.Add("sweeper");
            allowedTanks.Add("Regular");
            allowedTanks.Add("Sniper");
            allowedTanks.Add("Scout");
        }
        /// <summary>
        /// Create custom rule sets to start the game with. Maps and Rule Sets are seperate.
        /// </summary>
        /// <param name="TankPoints">Tank Points players can spend at the start</param>
        /// <param name="NumOfMines">Number of mines players can place</param>
        /// <param name="StartingSweeps">Number of Sweepers players have</param>
        /// <param name="AllowedItems">Current list of items to random from when a box is picked up</param>
        /// <param name="NumOfPlayers">Can only be as high as the map supports</param>
        public RuleSet(int TankPoints, int NumOfMines, int StartingSweeps, List<string> AllowedItems, List<string> AllowedTanks, int NumOfPlayers)
        {
            tankPoints = TankPoints;
            numOfMines = NumOfMines;   
            startingSweeps = StartingSweeps;
            allowedItems = AllowedItems;
            allowedTanks = AllowedTanks;
            numOfPlayers = NumOfPlayers;
        }
    }
}
