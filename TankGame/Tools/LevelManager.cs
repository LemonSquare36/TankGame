using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using TankGame.Objects.Entities;
using TankGame.Objects;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Xml;

namespace TankGame.Tools
{
    internal class LevelManager
    {
        StreamReader reader;
        StreamWriter writer;
        XDocument document;
        Board board;
        List<Entity> entities = new List<Entity>();
        List<Wall> walls = new List<Wall>();
        List<ItemBox> itemBoxes = new List<ItemBox>();
        Cell[,] cellMap;
        Point tanksMines;
        List<List<SpawnTile>> PlayerSpawns = new List<List<SpawnTile>>();
        List<SpawnTile> AllSpawns = new List<SpawnTile>();
        int sweeps, playerCount;

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
            entities.Clear();
            walls.Clear();
            itemBoxes.Clear();
            PlayerSpawns.Clear();

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

            //get tanks
            tanksMines.X = Convert.ToInt16(parentElement.Element("Tanks").Value);
            //get mines
            tanksMines.Y = Convert.ToInt16(parentElement.Element("Mines").Value);
            //get sweeps
            sweeps = Convert.ToInt16(parentElement.Element("Sweeps").Value);
            //get the player count
            playerCount = Convert.ToInt16(parentElement.Element("PlayerCount").Value);


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
                //create the wall with the cordinates extracted
                Wall tempWall = new Wall(board.getGridSquare(X, Y), new Point(X, Y));

                //add the wall to entities for the level editor to use
                entities.Add(tempWall);
                //add it to the walls list
                walls.Add(tempWall);
            }

            //get the itemboxes
            //get the elements label wall
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

                //add the wall to entities for the level editor to use
                entities.Add(tempItemBox);
                //add it to the walls list
                itemBoxes.Add(tempItemBox);
            }

            //get the spawnregions for each player
            for (int i = 0; i < playerCount; i++)
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
                    //add to entities list for level editor
                    entities.Add(tempSpawnTile);
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
        public void SaveLevel(string FileLocation, string FileName, Board board, List<Entity> E, List<List<SpawnTile>> SpawnTiles, Point TanksAndMines, int Sweeps, int playerCount)
        {
            //the path to the appdata folder of the machine
            string relativePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TankGame\\LevelFiles";
            //verify the folder exists for levels and make it if needed
            if (!System.IO.Directory.Exists(relativePath))
            {
                System.IO.Directory.CreateDirectory(relativePath);
            }
            //delete the file if it exists
            //if (File.Exists(FileLocation))
            //{ File.Delete(FileLocation); }


            //createfile and then write to it
            //File.Create(FileLocation).Close();


            document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XComment("Level File for " + FileName),
                new XElement("LevelFile",

                    new XElement("Gameboard",
                        new XElement("Tanks", Convert.ToString(TanksAndMines.X)),
                        new XElement("Mines", Convert.ToString(TanksAndMines.Y)),
                        new XElement("Sweeps", Convert.ToString(Sweeps)),
                        new XElement("Size", Convert.ToString(board.Rows)),
                        new XElement("Color1", Convert.ToString(board.Color1.R) + "," + Convert.ToString(board.Color1.G) + "," + Convert.ToString(board.Color1.B)),
                        new XElement("Color2", Convert.ToString(board.Color2.R) + "," + Convert.ToString(board.Color2.G) + "," + Convert.ToString(board.Color2.B)),
                        new XElement("BorderColor", Convert.ToString(board.Color3.R) + "," + Convert.ToString(board.Color3.G) + "," + Convert.ToString(board.Color3.B)),
                        new XElement("BorderThickness", board.BorderThickness),
                        new XElement("PlayerCount", playerCount)),

                    new XElement("Walls",
                        from Entity in E
                        where Entity.Type == "wall"
                        select new XElement("wall", Entity.gridLocation.X + "," + Entity.gridLocation.Y)),
                    new XElement("ItemBoxes",
                        from Entity in E
                        where Entity.Type == "itembox"
                        select new XElement("itembox", Entity.gridLocation.X + "," + Entity.gridLocation.Y)),
                    new XElement("SpawnTiles",
                        from list in SpawnTiles
                        select new XElement("Player", new XAttribute("Id", SpawnTiles.FindIndex(a => a == list)),
                        from spawnTiles in list
                        select new XElement("spawn", spawnTiles.gridLocation.X + "," + spawnTiles.gridLocation.Y))
                    )));

            document.Save(FileLocation);
        }

        #region getBoardInfo
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
        public List<Wall> getWalls()
        {
            return walls;
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
            return playerCount;
        }
        #endregion
    }
}
