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
using TankGame.Objects;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        string fileLocation;
        StreamReader reader;
        StreamWriter writer;
        Board board;
        List<Entity> entities;

        /// <summary>
        /// Loads the level information from the file selected when initializing the manager
        /// </summary>
        public void LoadLevel(string FileLocation)
        {
            //
            string[] cords;
            //set reader to the file needing to load
            reader = new StreamReader(fileLocation);
            //read the lines with this
            string line = reader.ReadLine();
            //first line is a category call
            string category = line;
            //read until the file is at its end
            while (!reader.EndOfStream)
            {
                //if END is called, read the next line which is a category - to set the category
                if (line == "END")
                {
                    line = reader.ReadLine();
                    category = line;
                }
                else if (category == "GAMEBOARD")
                {
                    //get the data need for making a board
                    string[] pos = reader.ReadLine().Split(',');
                    float size = (float)Convert.ToDouble(reader.ReadLine());
                    string[] RowCol = reader.ReadLine().Split(',');
                    int border = Convert.ToInt16(reader.ReadLine());
                    //create the board with the data
                    board = new Board(new Point(Convert.ToInt16(pos[0]), Convert.ToInt16(pos[1])),
                        new Point(Convert.ToInt16(Main.graphicsDevice.Viewport.Height * size), Convert.ToInt16(Main.graphicsDevice.Viewport.Height * size)),
                        Convert.ToInt16(RowCol[1]), Convert.ToInt16(RowCol[0]), border);
                    //sets line END after getting board info
                    line = reader.ReadLine();
                }
                else if (category == "WALLS")
                {
                    //line reads looking for END and passes the data into an array split by the comma
                    line = reader.ReadLine();
                    cords = line.Split(',');
                    entities.Add(new Wall(board.getGridSquare(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1]))));
                }
                else if (category == "POWERUPS")
                {

                }



                }
        }
        /// <summary>
        /// Saves the level information to a file selected when initializing the manager
        /// </summary>
        public void SaveLevel(string FileLocation, GameBoard Board, Entity[] E)
        {

        }
        public Board getGameBoard()
        {
            return board;
        }
        public List<Entity> getEntities()
        {
            return entities;
        }
    }
}
