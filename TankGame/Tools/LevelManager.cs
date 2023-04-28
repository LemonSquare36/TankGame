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
using TankGame.Tools;
using TankGame.Objects.Entities;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        string fileLocation;
        StreamReader reader;
        StreamWriter writer;
        GameBoard board;
        Entity[] entities;

        /// <summary>
        /// Loads the level information from the file selected when initializing the manager
        /// </summary>
        public void LoadLevel(string FileLocation)
        {
            
        }
        /// <summary>
        /// Saves the level information to a file selected when initializing the manager
        /// </summary>
        public void SaveLevel(string FileLocation, GameBoard Board, Entity[] E)
        {

        }
        public GameBoard getGameBoard()
        {
            return board;
        }
        public Entity[] getEntities()
        {
            return entities;
        }
    }
}
