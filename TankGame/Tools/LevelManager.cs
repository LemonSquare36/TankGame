using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using TankGame.Objects.Entities;
using TankGame.Objects;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        Board board;
        List<Wall> walls = new List<Wall>();
        List<ItemBox> itemBoxes = new List<ItemBox>();
        List<Hole> holes = new List<Hole>();
        Cell[,] cellMap;
        List<List<SpawnTile>> PlayerSpawns = new List<List<SpawnTile>>();
        List<SpawnTile> AllSpawns = new List<SpawnTile>();
        int maxPlayerCount;

        /// <summary>
        /// Loads the level information from the file selected when initializing the manager
        /// </summary>
        /// <param name="FileLocation">Full file location</param>
        /// <param name="offSetX">A decimal value representing perctage of the internal resolution</param>
        /// <param name="offSetY">A decimal value representing perctage of the internal resolution</param>
        public void LoadLevel(string FileLocation, float offSetX, float offSetY)
        {
            //open the xml document we wish to read (the level file)
            XDocument document = XDocument.Load(FileLocation);
            XElement parentElement;

            //reset the lists
            walls.Clear();
            itemBoxes.Clear();
            PlayerSpawns.Clear();
            holes.Clear();

            //get the board information

            float size = Camera.ViewboxScale.Y * 0.9F;
            Point pos = new Point(Convert.ToInt16(Camera.ViewboxScale.X * offSetX), Convert.ToInt16(Convert.ToInt16(Camera.ViewboxScale.Y * offSetY)));

            //viarables needed to create the board
            int RowCol, borderThickness;
            string[] colorString; ; //temp storage to then seperate the RGB values for the colors
            Color color1;
            Color color2;
            Color borderColor;
            //get the xelement with with Gameboard details
            parentElement = document.Element("LevelFile").Element("Gameboard");
            //get the board variables from the level document
            RowCol = Convert.ToInt16(parentElement.Element("Size").Value);
            borderThickness = Convert.ToInt16(parentElement.Element("BorderThickness").Value);
            //get the colors
            colorString = parentElement.Element("Color1").Value.Split(",");
            color1 = new Color(Convert.ToInt16(colorString[0]), Convert.ToInt16(colorString[1]), Convert.ToInt16(colorString[2]));
            colorString = parentElement.Element("Color2").Value.Split(",");
            color2 = new Color(Convert.ToInt16(colorString[0]), Convert.ToInt16(colorString[1]), Convert.ToInt16(colorString[2]));
            colorString = parentElement.Element("BorderColor").Value.Split(",");
            borderColor = new Color(Convert.ToInt16(colorString[0]), Convert.ToInt16(colorString[1]), Convert.ToInt16(colorString[2]));

            //create the board with the aquired information
            board = new Board(pos, new Point(Convert.ToInt16(size), Convert.ToInt16(size)), RowCol, RowCol, borderThickness);
            board.setColor(color1, color2, borderColor);

            //get the player count
            maxPlayerCount = Convert.ToInt16(parentElement.Element("MaxPlayerCount").Value);

            //get the walls
            //get the elements label wall
            IEnumerable<XElement> Elements = document.Descendants("wall");
            //foreach element returned
            foreach (var element in Elements)
            {
                //grab the value and split it, storing it in a string array
                string[] cord = element.Value.Split(",");
                //use the strings to declare the locations in int form
                int X = Convert.ToInt16(cord[0]);
                int Y = Convert.ToInt16(cord[1]);
                bool destroyable = true;
                if (cord[2] == "False")
                {
                    destroyable = false;
                }
                //create the wall with the cordinates extracted
                Wall tempWall = new Wall(board.getGridSquare(X, Y), new Point(X, Y), destroyable);

                //add it to the walls list
                walls.Add(tempWall);
            }
            //get the multiwalls
            parentElement = document.Element("LevelFile").Element("MultiWalls");
            List<Point> mulitWallPoints = new List<Point>();
            Elements = parentElement.Descendants("mWall");
            foreach (var element in Elements)
            {
                bool destroyable = true;
                if (element.Element("destroyable").Value == "False")
                {
                    destroyable = false;
                }
                IEnumerable<XElement> segments = element.Elements("segment");
                foreach (var segment in segments)
                {
                    string[] cord = segment.Value.Split(",");
                    mulitWallPoints.Add(new Point(Convert.ToInt16(cord[0]), Convert.ToInt16(cord[1])));
                }
                Wall multiWall = new Wall(mulitWallPoints, board, destroyable);
                walls.Add(multiWall);
                mulitWallPoints.Clear();
            }
            //get the holes
            //get the elements label hole
            Elements = document.Descendants("hole");
            //foreach element returned
            foreach (var element in Elements)
            {
                //grab the value and split it, storing it in a string array
                string[] cord = element.Value.Split(",");
                //use the strings to declare the locations in int form
                int X = Convert.ToInt16(cord[0]);
                int Y = Convert.ToInt16(cord[1]);
                //create the wall with the cordinates extracted
                Hole tempHole = new Hole(board.getGridSquare(X, Y), new Point(X, Y));

                //add it to the walls list
                holes.Add(tempHole);
            }

            //get the itemboxes
            //get the elements label itembox
            Elements = document.Descendants("itembox");
            //foreach element returned
            foreach (var element in Elements)
            {
                //grab the value and split it, storing it in a string array
                string[] cord = element.Value.Split(",");
                //use the strings to declare the locations in int form
                int X = Convert.ToInt16(cord[0]);
                int Y = Convert.ToInt16(cord[1]);
                //create the wall with the cordinates extracted
                ItemBox tempItemBox = new ItemBox(board.getGridSquare(X, Y), new Point(X, Y));

                //add it to the walls list
                itemBoxes.Add(tempItemBox);
            }

            //get the spawnregions for each player
            for (int i = 0; i < maxPlayerCount; i++)
            {
                //create a new list to store player spawn tiles in 
                PlayerSpawns.Add(new());
                //get the spawn tiles only under one player at a time
                parentElement = document.Descendants("Player")
                    .Where(x => (int)x.Attribute("Id") == i)
                    .FirstOrDefault();

                //get all the xelements with the spawn information into a list
                Elements = parentElement.Descendants("spawn");
                //cycle through the list and get the information from each node
                foreach (XElement element in Elements)
                {
                    //grab the value and split it, storing it in a string array
                    string[] cord = element.Value.Split(",");
                    //use the strings to declare the locations in int form
                    int X = Convert.ToInt16(cord[0]);
                    int Y = Convert.ToInt16(cord[1]);
                    //make a new spawn tile
                    SpawnTile tempSpawnTile = new SpawnTile(board.getGridSquare(X, Y), new Point(X, Y));
                    //add the spawn tile to the appropriate players list
                    PlayerSpawns[i].Add(tempSpawnTile);
                    AllSpawns.Add(tempSpawnTile);
                }
            }
            populateCellMap();
        }

        /// <summary>
        /// Saves the level information to a file
        /// </summary>
        /// <param name="FileLocation">The full file path, to include the name and .lvl</param>
        /// <param name="FileName">Just the name of the file. This is used in the xml document comment</param>
        /// <param name="board">the current board object being saved</param>
        /// <param name="E">entities list being saved</param>
        /// <param name="SpawnTiles">the list of list with players spawn tiles</param>
        /// <param name="TanksAndMines">a vector2 with X as tanks and Y as mines</param>
        /// <param name="Sweeps">sweeps count</param>
        /// <param name="playerCount">player count</param>
        public void SaveLevel(string FileLocation, string FileName, Board board, List<Wall> walls, List<ItemBox> itemBoxes, List<Hole> holes, List<List<SpawnTile>> SpawnTiles, int playerCount)
        {
            //the path to the appdata folder of the machine
            string relativePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TankGame\\LevelFiles";
            //verify the folder exists for levels and make it if needed
            if (!System.IO.Directory.Exists(relativePath))
            {
                System.IO.Directory.CreateDirectory(relativePath);
            }

            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Level File for " + FileName),
                new XElement("LevelFile",

                    new XElement("Gameboard",
                        new XElement("Size", Convert.ToString(board.Rows)),
                        new XElement("Color1", Convert.ToString(board.Color1.R) + "," + Convert.ToString(board.Color1.G) + "," + Convert.ToString(board.Color1.B)),
                        new XElement("Color2", Convert.ToString(board.Color2.R) + "," + Convert.ToString(board.Color2.G) + "," + Convert.ToString(board.Color2.B)),
                        new XElement("BorderColor", Convert.ToString(board.Color3.R) + "," + Convert.ToString(board.Color3.G) + "," + Convert.ToString(board.Color3.B)),
                        new XElement("BorderThickness", board.BorderThickness),
                        new XElement("MaxPlayerCount", playerCount)),

                    new XElement("Walls",
                        from wall in walls
                        where wall.multiWall == false
                        select new XElement("wall", wall.gridLocation.X + "," + wall.gridLocation.Y + "," + wall.destroyable.ToString())),
                    new XElement("MultiWalls",
                        from multiWall in walls
                        where multiWall.multiWall == true
                        select new XElement("mWall",
                            new XElement("destroyable", multiWall.destroyable.ToString()),
                            from segment in multiWall.gridLocations
                            select new XElement("segment", segment.X + "," + segment.Y))),
                    new XElement("Holes",
                        from hole in holes
                        select new XElement("hole", hole.gridLocation.X + "," + hole.gridLocation.Y)),
                    new XElement("ItemBoxes",
                        from itemBox in itemBoxes
                        select new XElement("itembox", itemBox.gridLocation.X + "," + itemBox.gridLocation.Y)),
                    new XElement("SpawnTiles",
                        from list in SpawnTiles
                        select new XElement("Player", new XAttribute("Id", SpawnTiles.FindIndex(a => a == list)),
                        from spawnTiles in list
                        select new XElement("spawn", spawnTiles.gridLocation.X + "," + spawnTiles.gridLocation.Y))
                    ))); ;

            document.Save(FileLocation);
        }

        #region getBoardInfo
        public Board getGameBoard()
        {
            return board;
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
                    bool found = false;
                    cellMap[i, j] = new Cell(i, j, 1); //create a cell per tile on the board with a cost of moment being 1
                    //if walls list contains a wall on the current cell (i j location)
                    foreach (Wall wall in walls)
                    {
                        if (!wall.multiWall)
                        {
                            if (wall.gridLocation.X == i && wall.gridLocation.Y == j)
                            {
                                //set that cell in the map to have a identifier of 1 to indicate a wall existing
                                cellMap[i, j].Identifier = 1; //1 for wall 
                                found = true;
                                break;
                            }
                        }
                        else
                        {
                            foreach (Point gridLocation in wall.gridLocations)
                            {
                                if (gridLocation.X == i && gridLocation.Y == j)
                                {
                                    //set that cell in the map to have a identifier of 1 to indicate a wall existing
                                    cellMap[i, j].Identifier = 1; //1 for wall 
                                    found = true;
                                    break;
                                }
                            }
                            if (found)
                            { break; }

                        }
                    }
                    if (!found)
                    {
                        foreach (Hole hole in holes)
                        {
                            if (hole.gridLocation.X == i && hole.gridLocation.Y == j)
                            {
                                //set that cell in the map to have a identifier of 1 to indicate a wall existing
                                cellMap[i, j].Identifier = 1; //1 for wall 
                                break;
                            }
                        }
                    }
                }
            }
        }
        public List<Wall> getWalls()
        {
            return walls;
        }
        public List<Hole> getHoles()
        {
            return holes;
        }
        public List<ItemBox> getItemBoxes()
        {
            return itemBoxes;
        }
        public List<List<SpawnTile>> getPlayerSpawns()
        {
            return PlayerSpawns;
        }
        public List<SpawnTile> getAllSpawnTiles()
        {
            return AllSpawns;
        }
        public int getPlayerCount()
        {
            return maxPlayerCount;
        }
        #endregion
    }
}
