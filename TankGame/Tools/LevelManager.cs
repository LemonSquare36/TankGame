using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using TankGame.Objects.Entities;
using TankGame.Objects;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        StreamReader reader;
        StreamWriter writer;
        Board board;
        List<Entity> entities = new List<Entity>();

        /// <summary>
        /// Loads the level information from the file selected when initializing the manager
        /// </summary>
        public void LoadLevel(string FileLocation)
        {
            //
            string[] cords;
            //set reader to the file needing to load
            reader = new StreamReader(FileLocation);
            //read the lines with this
            string line = reader.ReadLine();
            //first line is a category call
            string category = line;
            //reset the list
            entities.Clear();
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
                    //string[] pos = reader.ReadLine().Split(',');
                    //float size = (float)Convert.ToDouble(reader.ReadLine());
                    string[] RowCol = reader.ReadLine().Split(',');
                    int border = Convert.ToInt16(reader.ReadLine());
                    string[] color1 = reader.ReadLine().Split(',');
                    string[] color2 = reader.ReadLine().Split(',');
                    string[] color3 = reader.ReadLine().Split(',');

                    //create the board with the data
                    float size = Camera.ViewboxScale.Y * 0.9F;
                    Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.Y * .05F), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * .05F)));

                    board = new Board(pos,
                        new Point(Convert.ToInt16(size), Convert.ToInt16(size)),
                        Convert.ToInt16(RowCol[1]), Convert.ToInt16(RowCol[0]), border);
                    //set the boards color
                    board.setColor(new Color(Convert.ToInt16(color1[0]), Convert.ToInt16(color1[1]), Convert.ToInt16(color1[2])),
                        new Color(Convert.ToInt16(color2[0]), Convert.ToInt16(color2[1]), Convert.ToInt16(color2[2])),
                        new Color(Convert.ToInt16(color3[0]), Convert.ToInt16(color3[1]), Convert.ToInt16(color3[2])));
                    //sets line END after getting board info
                    line = reader.ReadLine();
                }
                else if (category == "WALLS")
                {
                    //line reads looking for END and passes the data into an array split by the comma
                    line = reader.ReadLine();
                    if (line != "END")
                    {
                        cords = line.Split(',');
                        entities.Add(new Wall(board.getGridSquare(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1])), 
                            new Point(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1]))));
                    }
                }
                else if (category == "ITEMBOXS")
                {
                    line = reader.ReadLine();
                    if (line != "END")
                    {
                        cords = line.Split(',');
                        entities.Add(new ItemBox(board.getGridSquare(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1])),
                            new Point(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1]))));
                    }
                }
                }
            reader.Close();
        }
        /// <summary>
        /// Saves the level information to a file selected when initializing the manager
        /// </summary>
        public void SaveLevel(string FileLocation, Board board, List<Entity> E)
        {
            string relativePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TankGame";
            if (!System.IO.Directory.Exists(relativePath))
            {
                System.IO.Directory.CreateDirectory(relativePath);
            }
            //delete the file if it exists, to recreate it and make it empty
            if (File.Exists(FileLocation))
            { File.Delete(FileLocation); }
           //createfile and then write to it
            File.Create(FileLocation).Close();           
            try
            {
                writer = new StreamWriter(FileLocation);
            }
            catch
            {
                return;
            }
            //write board info
            writer.WriteLine("GAMEBOARD");
            writer.WriteLine(Convert.ToString(board.Rows) + "," + Convert.ToString(board.Columns));
            writer.WriteLine(board.BorderThickness);
            //checkerboard colors
            writer.WriteLine(Convert.ToString(board.Color1.R) + "," + Convert.ToString(board.Color1.G) + "," + Convert.ToString(board.Color1.B));
            writer.WriteLine(Convert.ToString(board.Color2.R) + "," + Convert.ToString(board.Color2.G) + "," + Convert.ToString(board.Color2.B));
            //border color
            writer.WriteLine(Convert.ToString(board.Color3.R) + "," + Convert.ToString(board.Color3.G) + "," + Convert.ToString(board.Color3.B));
            writer.WriteLine("END");


            //write walls
            writer.WriteLine("WALLS");            
            for (int i = 0; i < E.Count; i++)
            {
                if (E[i].Type == "wall")
                {
                    writer.WriteLine(E[i].gridLocation.X + "," + E[i].gridLocation.Y);
                }
            }
            writer.WriteLine("END");
            writer.WriteLine("ITEMBOXS");
            for (int i = 0; i < E.Count; i++)
            {
                if (E[i].Type == "itembox")
                {
                    writer.WriteLine(E[i].gridLocation.X + "," + E[i].gridLocation.Y);
                }
            }
            writer.WriteLine("END");
            writer.Close();
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
