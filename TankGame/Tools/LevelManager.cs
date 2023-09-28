using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using TankGame.Objects.Entities;
using TankGame.Objects;
using System.Linq;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        StreamReader reader;
        StreamWriter writer;
        Board board;
        List<Entity> entities = new List<Entity>();
        List<Wall> walls = new List<Wall>();
        Cell[,] cellMap;
        Point tanksMines;
        int sweeps;

        /// <summary>
        /// Loads the level information from the file selected when initializing the manager
        /// </summary>
        /// <param name="FileLocation">Full file location</param>
        /// <param name="offSetX">A decimal value representing perctage of the internal resolution</param>
        /// <param name="offSetY">A decimal value representing perctage of the internal resolution</param>
        public void LoadLevel(string FileLocation, float offSetX, float offSetY)
        {
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
                try {
                    //if END is called, read the next line which is a category - to set the category
                    if (line == "END")
                    {
                        line = reader.ReadLine();
                        category = line;
                    }
                    else if (category == "GAMEBOARD")
                    {

                        //get the data need for making a board
                        //get the number of tanks and mines per side
                        string[] TankMines = reader.ReadLine().Split(',');
                        tanksMines = new Point(Convert.ToInt16(TankMines[0]), Convert.ToInt16(TankMines[1]));
                        //read sweeps
                        sweeps = Convert.ToInt16(reader.ReadLine());
                        //get the board data
                        string[] RowCol = reader.ReadLine().Split(',');
                        int border = Convert.ToInt16(reader.ReadLine());
                        string[] color1 = reader.ReadLine().Split(',');
                        string[] color2 = reader.ReadLine().Split(',');
                        string[] color3 = reader.ReadLine().Split(',');

                        //create the board with the data
                        float size = Camera.ViewboxScale.Y * 0.9F;
                        Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.X * offSetX), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * offSetY)));

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
                            Wall wallToAdd = new Wall(board.getGridSquare(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1])),
                                new Point(Convert.ToInt16(cords[0]), Convert.ToInt16(cords[1])));
                            entities.Add(wallToAdd);
                            walls.Add(wallToAdd);
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
                catch { }
            }           
            populateCellMap();
            reader.Close();
        }
        /// <summary>
        /// Saves the level information to a file selected when initializing the manager
        /// </summary>
        public void SaveLevel(string FileLocation, Board board, List<Entity> E, Point TanksAndMines, int Sweeps)
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
            //write the tanks and mines
            writer.WriteLine(Convert.ToString(TanksAndMines.X) + "," + Convert.ToString(TanksAndMines.Y));
            //write sweeps
            writer.WriteLine(Convert.ToString(Sweeps));
            //write the board data
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
        public Point getTanksAndMines()
        {
            return tanksMines;
        }
        public List<Wall> getWalls()
        {
            return walls;
        }
        public int getSweeps()
        {
            return sweeps;
        }
        public Cell[,] getCellMap()
        {
            return cellMap;           
        }
        private void populateCellMap()
        {
            cellMap = new Cell[board.Rows, board.Columns];
            //create the cellmap with the info aquired
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    cellMap[i, j] = new Cell(i, j, 1); //create a cell per tile on the board with a cost of moment being 1
                    //if walls list contains a wall on the current cell (i j location)
                    if (walls.Contains(walls.FirstOrDefault(a => a.gridLocation.X == i && a.gridLocation.Y == j)))
                    {
                        //set that cell in the map to have a identifier of 1 to indicate a wall existing
                        cellMap[i, j].Identifier = 1; //1 for wall (2 for tank)
                    }
                }
            }
        }
        public List<Wall> getWallsList()
        {
            return walls;
        }
    }
}
